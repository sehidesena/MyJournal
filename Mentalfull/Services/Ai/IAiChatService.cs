using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Mentalfull.Services.Ai
{
    public interface IAiChatService : IApplicationService
    {
        Task<string> GetResponseAsync(string userMessage, string systemPrompt = null);
    }
}
