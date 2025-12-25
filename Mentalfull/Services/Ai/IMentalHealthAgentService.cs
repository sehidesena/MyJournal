using System.Collections.Generic;
using System.Threading.Tasks;
using Mentalfull.Services.Dtos.Chats;
using Volo.Abp.Application.Services;

namespace Mentalfull.Services.Ai
{
    public interface IMentalHealthAgentService : IApplicationService
    {
        Task<string> ChatAsync(string userMessage, IEnumerable<ChatMessageDto> history);
        Task<string> AnalyzeJournalAsync(string journalContent);
        Task<string> GenerateRecommendationsAsync(string analysisResult, string journalContent);
    }
}
