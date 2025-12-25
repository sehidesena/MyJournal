using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Mentalfull.Entities.Chats;
using Mentalfull.Services.Dtos.Chats;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Embeddings;
using Volo.Abp.Application.Services;
using System.Linq;
using Mentalfull.Entities.JournalEntries;
using Mentalfull.Entities.MoodTracking;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using System;

namespace Mentalfull.Services.Ai
{
    public class MentalHealthAgentService : MentalfullAppService, IMentalHealthAgentService
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatCompletionService;
        private readonly ISemanticTextMemory _memory;
        private readonly IRepository<JournalEntry, Guid> _journalRepository;
        private readonly IRepository<MoodLog, Guid> _moodLogRepository;
        private readonly ICurrentUser _currentUser;

        private const string MemoryCollectionName = "mentalfull-memory";

        public MentalHealthAgentService(
            Kernel kernel, 
            ISemanticTextMemory memory,
            IRepository<JournalEntry, Guid> journalRepository,
            IRepository<MoodLog, Guid> moodLogRepository,
            ICurrentUser currentUser)
        {
            _kernel = kernel;
            _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
            _memory = memory;
            _journalRepository = journalRepository;
            _moodLogRepository = moodLogRepository;
            _currentUser = currentUser;
        }

        public async Task<string> ChatAsync(string userMessage, IEnumerable<ChatMessageDto> history)
        {
            // 1. Search in Memory (RAG)
            var memoryBuilder = new StringBuilder();
            try 
            {
                // Re-enabled Memory Search
                await foreach (var memoryResult in _memory.SearchAsync(MemoryCollectionName, userMessage, limit: 5, minRelevanceScore: 0.0))
                {
                    // Fallback to Description if Text is empty (common in some Pinecone integrations)
                    var info = !string.IsNullOrWhiteSpace(memoryResult.Metadata.Text) 
                        ? memoryResult.Metadata.Text 
                        : memoryResult.Metadata.Description;

                    memoryBuilder.AppendLine($"- {info}");
                }
            }
            catch (System.Exception ex)
            {
                // Silently fail if RAG has issues to keep chat working
                Console.WriteLine($"[RAG SEARCH ERROR]: {ex.Message}");
            }

            var relevantMemories = memoryBuilder.ToString();

            // 2. Define System Prompt with Context
            var instructions = "Sen şefkatli, anlayışlı ve profesyonel bir ruh sağlığı asistanısın. " +
                               "Kullanıcılara yardımcı olmak için Bilişsel Davranışçı Terapi (BDT) tekniklerini kullanırsın. " +
                               "Her zaman empatik, yargılayıcı olmayan bir dil kullan ve kullanıcının iyi hissetmesini önceliklendir. " +
                               "Cevaplarını her zaman samimi ve doğal bir Türkçe ile ver. " +
                               "ASLA tıbbi bir tanı koyma (Depresyon, Anksiyete bozukluğu vb. gibi terimler kullanma). " +
                               "Kendini bir terapist veya doktor olarak tanıtma. Sadece destekleyici bir yapay zeka olduğunu unutma. " +
                               "Eğer kullanıcı kendine zarar verme düşünceleri veya şiddetli bir kriz belirtisi gösterirse, nazikçe ama net bir şekilde derhal profesyonel yardım almasını veya acil durum hatlarını aramasını öner.\n\n";

            // 2.1 Fetch Recent Journal & Mood Context (Last 3 days or 3 entries)
            try
            {
                if (_currentUser.Id.HasValue)
                {
                    var journalQuery = await _journalRepository.GetQueryableAsync();
                    var recentJournals = journalQuery
                        .Where(x => x.UserId == _currentUser.Id.Value)
                        .OrderByDescending(x => x.EntryDate)
                        .Take(3)
                        .Select(x => $"[{x.EntryDate:dd.MM}]: {x.Content} (Duygu: {x.AnalysisResult.DominantEmotion})")
                        .ToList();

                    if (recentJournals.Any())
                    {
                        instructions += "Kullanıcının son günlük girişleri:\n" + string.Join("\n", recentJournals) + "\n\n";
                    }

                    var moodQuery = await _moodLogRepository.GetQueryableAsync();
                    var recentMoods = moodQuery
                        .Where(x => x.UserId == _currentUser.Id.Value)
                        .OrderByDescending(x => x.Timestamp)
                        .Take(3)
                        .Select(x => $"[{x.Timestamp:dd.MM}]: {x.PrimaryEmotion} ({x.Intensity}/10)")
                        .ToList();

                     if (recentMoods.Any())
                    {
                        instructions += "Kullanıcının son duygu durumu:\n" + string.Join("\n", recentMoods) + "\n\n";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Context Fetch Error]: {ex.Message}");
            }

            if (!string.IsNullOrEmpty(relevantMemories))
            {
                instructions += $"İşte önceki konuşmalardan veya kullanıcı verilerinden hatırladığın bazı bağlamlar:\n{relevantMemories}\n\n" +
                                "Bu bilgileri cevabında doğal bir şekilde kullan (örneğin 'Daha önce X demiştin' gibi), ama bağlam/hafıza kelimelerini teknik olarak kullanma.";
            }

            ChatHistory chatHistory = new();
            chatHistory.AddSystemMessage(instructions);
            
            foreach (var msg in history)
            {
                if (msg.Sender == ChatSender.User)
                {
                    chatHistory.AddUserMessage(msg.Content);
                }
                else
                {
                    chatHistory.AddAssistantMessage(msg.Content);
                }
            }

            chatHistory.AddUserMessage(userMessage);

            // 3. Get response
            var result = await _chatCompletionService.GetChatMessageContentAsync(
                chatHistory,
                kernel: _kernel
            );

            var responseContent = result.Content ?? string.Empty;

            // 4. Save interactions to memory
            if (!string.IsNullOrWhiteSpace(userMessage))
            {
                 try
                 {
                    // Use SaveInformationAsync instead of SaveReferenceAsync for better text persistence
                    await _memory.SaveInformationAsync(
                        collection: MemoryCollectionName,
                        text: $"User: {userMessage} | AI: {responseContent}",
                        id: Guid.NewGuid().ToString(),
                        description: userMessage
                    );
                 }
                 catch (System.Exception ex) 
                 { 
                     Console.WriteLine($"[RAG SAVE ERROR]: {ex.Message}");
                 }
            }

            return responseContent;
        }

        public async Task<string> AnalyzeJournalAsync(string journalContent)
        {
            var prompt = $@"
Aşağıdaki günlük metnini analiz et.

GÜNLÜK METNİ:
""{journalContent}""

ANALİZ KURALLARI:
1. Baskın duyguyu belirle (Örn: Mutlu, Üzgün, Kaygılı, Umutlu, Yorgun, Kızgın, Nötr).
2. Duygu yoğunluğunu -1.0 (Çok Negatif) ile +1.0 (Çok Pozitif) arasında bir puan olarak belirle.
3. Günlük içeriğini özetleyen, empatik ve kısa bir özet yaz (Maksimum 2 cümle).
4. Olası duyguların olasılıklarını tahmin et.
5. Asla tıbbi tanı koyma. Sadece metindeki duygu durumunu analiz et.

ÇIKTI FORMATI (Sadece bu JSON formatında çıktı ver, başka metin yazma):
{{
  ""DominantEmotion"": ""Duygu Adı"",
  ""SentimentScore"": 0.0,
  ""AnalysisSummary"": ""Empatik özet..."",
  ""EmotionProbabilities"": {{ ""Mutlu"": 0.1, ""Üzgün"": 0.8 }}
}}
";
            
            var result = await _chatCompletionService.GetChatMessageContentAsync(
                prompt,
                kernel: _kernel
            );

            return result.Content ?? "{}";
        }

        public async Task<string> GenerateRecommendationsAsync(string analysisResult, string journalContent)
        {
            var prompt = $@"
Kullanıcının günlüğü ve yapılan duygu analizi aşağıdadır. Buna göre kullanıcıya iyi gelecek 3 adet kısa ve uygulanabilir öneri (Recommendation) oluştur.

GÜNLÜK: ""{journalContent}""
ANALİZ: {analysisResult}

KURALLAR:
1. Öneriler tıbbi tavsiye içermemeli.
2. Basit, günlük hayatta uygulanabilir şeyler olmalı (Yürüyüş yap, müzik dinle, derin nefes al vb.).
3. Samimi ve destekleyici bir dil kullan.
4. Terapist gibi davranma, arkadaşça öneri ver.

ÇIKTI FORMATI (Sadece JSON listesi):
[
  ""Öneri 1 metni..."",
  ""Öneri 2 metni..."",
  ""Öneri 3 metni...""
]
";

            var result = await _chatCompletionService.GetChatMessageContentAsync(
                prompt,
                kernel: _kernel
            );

            return result.Content ?? "[]";
        }
    }
}
