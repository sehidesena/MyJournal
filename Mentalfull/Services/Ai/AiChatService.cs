using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Volo.Abp.Application.Services;

namespace Mentalfull.Services.Ai
{
    public class AiChatService : MentalfullAppService, IAiChatService
    {
        private readonly Kernel _kernel;
        private readonly IChatCompletionService _chatCompletionService;

        public AiChatService(Kernel kernel)
        {
            _kernel = kernel;
            _chatCompletionService = _kernel.GetRequiredService<IChatCompletionService>();
        }

        public async Task<string> GetResponseAsync(string userMessage, string systemPrompt = null)
        {
            var history = new ChatHistory();
            
            if (!string.IsNullOrEmpty(systemPrompt))
            {
                history.AddSystemMessage(systemPrompt);
            }

            history.AddUserMessage(userMessage);

            var result = await _chatCompletionService.GetChatMessageContentAsync(
                history,
                kernel: _kernel
            );

            return result.Content;
        }
    }
}
