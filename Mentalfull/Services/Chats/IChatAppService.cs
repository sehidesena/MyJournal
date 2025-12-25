using System;
using System.Threading.Tasks;
using Mentalfull.Services.Dtos.Chats;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace Mentalfull.Services.Chats
{
    public interface IChatAppService : IApplicationService
    {
        Task<PagedResultDto<ChatSessionDto>> GetListAsync(PagedAndSortedResultRequestDto input);
        Task<ChatSessionDto> GetAsync(Guid id);
        Task<ChatSessionDto> CreateAsync(CreateUpdateChatSessionDto input);
        Task<ChatSessionDto> UpdateAsync(Guid id, CreateUpdateChatSessionDto input);
        Task DeleteAsync(Guid id);
        
        Task<PagedResultDto<ChatMessageDto>> GetMessagesAsync(Guid chatSessionId, PagedAndSortedResultRequestDto input);
        Task<ChatMessageDto> SendMessageAsync(CreateUpdateChatMessageDto input);
        Task<ChatMessageDto> AskAiAsync(CreateUpdateChatMessageDto input);
    }
}
