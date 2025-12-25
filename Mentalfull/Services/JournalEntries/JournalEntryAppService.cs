using Mentalfull.Entities.JournalEntries;
using Mentalfull.Services.Dtos.JournalEntries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Mentalfull.Enums;
using Mentalfull.Services.Analysis;
using Mentalfull.Entities.Analysis;
using Mentalfull.Entities.MoodTracking;
using Mentalfull.Entities.Recommendations;

using Microsoft.AspNetCore.Hosting;
using System.IO;
using Volo.Abp.Content;

namespace Mentalfull.Services.JournalEntries;

[Authorize]
public class JournalEntryAppService : ApplicationService, IJournalEntryAppService
{
    private readonly IRepository<JournalEntry, Guid> _journalEntryRepository;
    private readonly IRepository<EmotionalAnalysisResult, Guid> _analysisRepository;
    private readonly ICurrentUser _currentUser;
    private readonly IVoiceProcessingService _voiceProcessingService;
    private readonly IAnalysisService _analysisService;
    private readonly IWebHostEnvironment _env;
    private readonly IRepository<MoodLog, Guid> _moodLogRepository;
    private readonly IRepository<Recommendation, Guid> _recommendationRepository;

    public JournalEntryAppService(
        IRepository<JournalEntry, Guid> journalEntryRepository,
        IRepository<EmotionalAnalysisResult, Guid> analysisRepository,
        ICurrentUser currentUser,
        IVoiceProcessingService voiceProcessingService,
        IAnalysisService analysisService,
        IWebHostEnvironment env,
        IRepository<MoodLog, Guid> moodLogRepository,
        IRepository<Recommendation, Guid> recommendationRepository)
    {
        _journalEntryRepository = journalEntryRepository;
        _analysisRepository = analysisRepository;
        _currentUser = currentUser;
        _voiceProcessingService = voiceProcessingService;
        _analysisService = analysisService;
        _env = env;
        _moodLogRepository = moodLogRepository;
        _recommendationRepository = recommendationRepository;
    }

    public async Task<JournalEntryDto> GetAsync(Guid id)
    {
        var journalEntry = await _journalEntryRepository.GetAsync(id);
        
        // Ensure user can only access their own entries
        if (journalEntry.UserId != _currentUser.Id)
        {
            throw new Volo.Abp.Authorization.AbpAuthorizationException("You don't have permission to access this journal entry.");
        }

        return ObjectMapper.Map<JournalEntry, JournalEntryDto>(journalEntry);
    }



    public async Task<PagedResultDto<JournalEntryDto>> GetListAsync(PagedAndSortedResultRequestDto input)
    {
        var queryable = await _journalEntryRepository.GetQueryableAsync();
        
        // Filter by current user
        queryable = queryable.Where(x => x.UserId == _currentUser.Id);

        var totalCount = await AsyncExecuter.CountAsync(queryable);

        var journalEntries = await AsyncExecuter.ToListAsync(
            queryable
                .OrderByDescending(x => x.IsPinned)
                .ThenByDescending(x => x.EntryDate)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount)
        );

        return new PagedResultDto<JournalEntryDto>(
            totalCount,
            ObjectMapper.Map<List<JournalEntry>, List<JournalEntryDto>>(journalEntries)
        );
    }

    public async Task<List<JournalEntryDto>> GetMyJournalEntriesAsync()
    {
        var queryable = await _journalEntryRepository.GetQueryableAsync();
        
        var journalEntries = await AsyncExecuter.ToListAsync(
            queryable
                .Where(x => x.UserId == _currentUser.Id)
                .OrderByDescending(x => x.EntryDate)
        );

        return ObjectMapper.Map<List<JournalEntry>, List<JournalEntryDto>>(journalEntries);
    }

    public async Task<JournalEntryDto> CreateAsync(CreateUpdateJournalEntryDto input)
    {
        var journalEntry = new JournalEntry
        {
            UserId = _currentUser.Id!.Value,
            Title = input.Title,
            Content = input.Content, // Initial content (empty if voice?)
            EntryDate = input.EntryDate,
            Type = input.Type,
            AudioUrl = input.AudioUrl,
            DurationSeconds = input.DurationSeconds,
            IsPinned = input.IsPinned ?? false,
            HasAiAnalysis = false
        };

        // 1. Handle Voice Logic
        if (input.Type == EntryType.Voice && !string.IsNullOrEmpty(input.AudioUrl))
        {
             // Transcribe
             var transcription = await _voiceProcessingService.TranscribeAudioAsync(input.AudioUrl);
             journalEntry.Content = transcription; 
             
             if (!string.IsNullOrWhiteSpace(input.Content))
             {
                 journalEntry.Content = input.Content + "\n\n" + transcription;
             }
             else
             {
                 journalEntry.Content = transcription;
             }
        }

        await _journalEntryRepository.InsertAsync(journalEntry, autoSave: true);

        // 2. Trigger Analysis & Mood Creation (Centralized)
        await ProcessJournalAnalysisAsync(journalEntry);

        return ObjectMapper.Map<JournalEntry, JournalEntryDto>(journalEntry);
    }

    public async Task<JournalEntryDto> UpdateAsync(Guid id, CreateUpdateJournalEntryDto input)
    {
        var journalEntry = await _journalEntryRepository.GetAsync(id);

        // Ensure user can only update their own entries
        if (journalEntry.UserId != _currentUser.Id)
        {
            throw new Volo.Abp.Authorization.AbpAuthorizationException("You don't have permission to update this journal entry.");
        }

        bool contentChanged = journalEntry.Content != input.Content; // Basic check, could be more robust

        journalEntry.Title = input.Title;
        journalEntry.Content = input.Content;
        // Keep original date usually, but if user updates date, we might need to move mood log too? 
        // For now, let's assume EntryDate might change.
        var oldDate = journalEntry.EntryDate;
        journalEntry.EntryDate = input.EntryDate;
        
        journalEntry.Type = input.Type;
        journalEntry.AudioUrl = input.AudioUrl;
        journalEntry.DurationSeconds = input.DurationSeconds;
        
        journalEntry.IsPinned = input.IsPinned ?? journalEntry.IsPinned;

        await _journalEntryRepository.UpdateAsync(journalEntry, autoSave: true);

        // Re-analyze if content changed
        if (contentChanged || !journalEntry.HasAiAnalysis)
        {
             await ProcessJournalAnalysisAsync(journalEntry);
        }

        return ObjectMapper.Map<JournalEntry, JournalEntryDto>(journalEntry);
    }

    private async Task ProcessJournalAnalysisAsync(JournalEntry journalEntry)
    {
        try 
        {
            var analysisResult = await _analysisService.AnalyzeJournalEntryAsync(journalEntry);
            analysisResult.JournalEntryId = journalEntry.Id; 
            
            // Check if analysis already exists for this journal entry to avoid duplicates (or update it)
            // Ideally we should delete old analysis or update it. 
            // Simple approach: Insert new, but maybe we should clear old ones?
            // For now, let's Just Insert as per original code, but cleaner might be to check.
            await _analysisRepository.InsertAsync(analysisResult);
            
            if (analysisResult.DominantEmotion != "Analiz Yok" && analysisResult.DominantEmotion != "Hata")
            {
                journalEntry.HasAiAnalysis = true;
                journalEntry.AnalysisResult = analysisResult; 
                await _journalEntryRepository.UpdateAsync(journalEntry);

                // --- Manage Mood Log ---
                int intensity = (int)((analysisResult.SentimentScore + 1) * 4.5 + 1);
                intensity = Math.Clamp(intensity, 1, 10);

                // Check for existing mood log at this timestamp (approximate match or exact?)
                // Since CreateAsync set it exactly, we check exactly.
                // Assuming one mood log per journal entry roughly.
                // NOTE: MoodLog doesn't have SourceId, so we rely on Date + User.
                var moodLogs = await _moodLogRepository.GetListAsync(x => x.UserId == _currentUser.Id && x.Timestamp == journalEntry.EntryDate);
                var existingMood = moodLogs.FirstOrDefault();

                if (existingMood != null)
                {
                    // Update existing
                    existingMood.Intensity = intensity;
                    existingMood.PrimaryEmotion = analysisResult.DominantEmotion;
                    existingMood.Note = $"Günlükten güncellendi: {journalEntry.Title}";
                    await _moodLogRepository.UpdateAsync(existingMood);
                }
                else
                {
                    // Create new
                    var moodLog = new MoodLog
                    {
                        UserId = _currentUser.Id!.Value,
                        Timestamp = journalEntry.EntryDate,
                        Intensity = intensity,
                        PrimaryEmotion = analysisResult.DominantEmotion,
                        Note = $"Günlükten otomatik oluşturuldu: {journalEntry.Title}"
                    };
                    await _moodLogRepository.InsertAsync(moodLog);
                }

                // --- Generate Recommendations ---
                var recommendations = await _analysisService.GetRecommendationsAsync(journalEntry, analysisResult);
                foreach (var recText in recommendations)
                {
                    if (string.IsNullOrWhiteSpace(recText)) continue;
                    
                    var recommendation = new Recommendation
                    {
                        UserId = _currentUser.Id!.Value,
                        Type = RecommendationType.General, 
                        Title = recText.Length > 200 ? recText.Substring(0, 197) + "..." : recText,
                        Reasoning = $"Çünkü '{analysisResult.DominantEmotion}' hissettiniz.",
                        ContextSource = "JournalEntry",
                        SourceId = journalEntry.Id
                    };
                    await _recommendationRepository.InsertAsync(recommendation);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Journal Analysis Error]: {ex.Message}");
        }
    }

    public async Task DeleteAsync(Guid id)
    {
        var journalEntry = await _journalEntryRepository.GetAsync(id);

        // Ensure user can only delete their own entries
        if (journalEntry.UserId != _currentUser.Id)
        {
            throw new Volo.Abp.Authorization.AbpAuthorizationException("You don't have permission to delete this journal entry.");
        }

        await _journalEntryRepository.DeleteAsync(id);
    }

    public async Task<JournalEntryDto> TogglePinAsync(Guid id)
    {
        var journalEntry = await _journalEntryRepository.GetAsync(id);

        // Ensure user can only pin/unpin their own entries
        if (journalEntry.UserId != _currentUser.Id)
        {
            throw new Volo.Abp.Authorization.AbpAuthorizationException("You don't have permission to modify this journal entry.");
        }

        journalEntry.IsPinned = !journalEntry.IsPinned;

        await _journalEntryRepository.UpdateAsync(journalEntry);

        return ObjectMapper.Map<JournalEntry, JournalEntryDto>(journalEntry);
    }
    public async Task<string> UploadVoiceAsync(IRemoteStreamContent input)
    {
        if (input == null || input.ContentLength == 0)
        {
            throw new UserFriendlyException("File is empty!");
        }

        var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "voice");
        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(input.FileName)}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await input.GetStream().CopyToAsync(fileStream);
        }

        return $"/uploads/voice/{fileName}";
    }
}
