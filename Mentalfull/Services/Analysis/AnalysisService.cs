using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Mentalfull.Entities.Analysis;
using Mentalfull.Entities.JournalEntries;
using Mentalfull.Services.Ai;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace Mentalfull.Services.Analysis;

public class AnalysisService : ApplicationService, IAnalysisService
{
    private readonly IMentalHealthAgentService _mentalHealthAgentService;

    public AnalysisService(IMentalHealthAgentService mentalHealthAgentService)
    {
        _mentalHealthAgentService = mentalHealthAgentService;
    }

    public async Task<EmotionalAnalysisResult> AnalyzeJournalEntryAsync(JournalEntry entry)
    {
        try 
        {
            // Call AI Service
            var jsonResponse = await _mentalHealthAgentService.AnalyzeJournalAsync(entry.Content);
            var jsonResult = ParseAiResponse(jsonResponse);

            if (jsonResult == null) return CreateFallbackResult(entry, "AI yanıtı işlenemedi.");

            // Map to Entity
            return new EmotionalAnalysisResult
            {
                JournalEntryId = entry.Id,
                SentimentScore = (float)(jsonResult["SentimentScore"]?.GetValue<double>() ?? 0),
                DominantEmotion = jsonResult["DominantEmotion"]?.GetValue<string>() ?? "Belirsiz",
                AnalysisSummary = jsonResult["AnalysisSummary"]?.GetValue<string>() ?? "Analiz yapılamadı.",
                EmotionProbabilities = jsonResult["EmotionProbabilities"]?.ToJsonString() ?? "{}",
                ClinicalFlags = null // AI service currently strictly avoids diagnosis, so this is null
            };
        }
        catch (Exception ex)
        {
            return CreateFallbackResult(entry, $"AI Bağlantı Hatası: {ex.Message}");
        }
    }

        public async Task<List<string>> GetRecommendationsAsync(JournalEntry entry, EmotionalAnalysisResult analysisResult)
        {
            try
            {
                var jsonResponse = await _mentalHealthAgentService.GenerateRecommendationsAsync(
                    analysisResult.DominantEmotion + " - " + analysisResult.SentimentScore, 
                    entry.Content
                );

                var jsonNode = ParseAiResponse(jsonResponse);
                if (jsonNode is JsonArray jsonArray)
                {
                    return jsonArray.Select(x => x?.ToString() ?? "").Where(x => !string.IsNullOrEmpty(x)).ToList();
                }

                return new List<string>();
            }
            catch
            {
                return new List<string> { "Küçük bir yürüyüş yapmayı dene.", "Derin bir nefes al ve rahatla." }; // Fallback
            }
        }


    private JsonNode? ParseAiResponse(string responseJson)
    {
        try 
        {
            // Clean Markdown code blocks if present
            var text = Regex.Replace(responseJson, @"^```json\s*", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"^```\s*", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"\s*```$", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);

            return JsonNode.Parse(text);
        }
        catch 
        {
            return null;
        }
    }

    private EmotionalAnalysisResult CreateFallbackResult(JournalEntry entry, string reason)
    {
        return new EmotionalAnalysisResult
        {
            JournalEntryId = entry.Id,
            SentimentScore = 0,
            DominantEmotion = "Analiz Yok", 
            AnalysisSummary = $"Analiz Hatası: {reason}",
            EmotionProbabilities = "{}",
            ClinicalFlags = null
        };
    }
}
