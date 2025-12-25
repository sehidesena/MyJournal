using System.Threading.Tasks;
using Mentalfull.Entities.Analysis;
using Mentalfull.Entities.JournalEntries;
using Volo.Abp.Application.Services;

namespace Mentalfull.Services.Analysis;

public interface IAnalysisService : IApplicationService
{
    Task<EmotionalAnalysisResult> AnalyzeJournalEntryAsync(JournalEntry entry);
    Task<List<string>> GetRecommendationsAsync(JournalEntry entry, EmotionalAnalysisResult analysisResult);
}
