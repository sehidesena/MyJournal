using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mentalfull.Entities.Chats;
using Mentalfull.ObjectMapping;
using Mentalfull.Services.Ai;
using Mentalfull.Services.Dtos.Chats;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Mentalfull.Services.Chats
{
    public class ChatAppService : MentalfullAppService, IChatAppService
    {
        private readonly IRepository<ChatSession, Guid> _chatSessionRepository;
        private readonly IRepository<ChatMessage, Guid> _chatMessageRepository;
        private readonly ChatSessionMapper _chatSessionMapper;
        private readonly ChatMessageMapper _chatMessageMapper;
        private readonly IMentalHealthAgentService _mentalHealthAgentService;

        public ChatAppService(
            IRepository<ChatSession, Guid> chatSessionRepository,
            IRepository<ChatMessage, Guid> chatMessageRepository,
            ChatSessionMapper chatSessionMapper,
            ChatMessageMapper chatMessageMapper,
            IMentalHealthAgentService mentalHealthAgentService)
        {
            _chatSessionRepository = chatSessionRepository;
            _chatMessageRepository = chatMessageRepository;
            _chatSessionMapper = chatSessionMapper;
            _chatMessageMapper = chatMessageMapper;
            _mentalHealthAgentService = mentalHealthAgentService;
        }

        public async Task<PagedResultDto<ChatSessionDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            if (CurrentUser.Id == null)
            {
                return new PagedResultDto<ChatSessionDto>();
            }

            var query = await _chatSessionRepository.GetQueryableAsync();
            query = query.Where(x => x.UserId == CurrentUser.Id);

            var totalCount = await AsyncExecuter.CountAsync(query);

            query = query.OrderByDescending(x => x.CreationTime)
                         .PageBy(input.SkipCount, input.MaxResultCount);

            var items = await AsyncExecuter.ToListAsync(query);

            return new PagedResultDto<ChatSessionDto>(
                totalCount,
                items.Select(item => _chatSessionMapper.Map(item)).ToList()
            );
        }

        public async Task<ChatSessionDto> GetAsync(Guid id)
        {
            var chatSession = await _chatSessionRepository.GetAsync(id);
            return _chatSessionMapper.Map(chatSession);
        }

        public async Task<ChatSessionDto> CreateAsync(CreateUpdateChatSessionDto input)
        {
            var chatSession = new ChatSession
            {
                UserId = CurrentUser.Id ?? Guid.Empty,
                Title = input.Title,
                StartedAt = DateTime.Now,
                IsActive = input.IsActive,
                LastMessageTime = DateTime.Now,
                AiSummary = string.Empty // Default value to prevent DB null constraint error
            };

            await _chatSessionRepository.InsertAsync(chatSession);

            return _chatSessionMapper.Map(chatSession);
        }

        public async Task<ChatSessionDto> UpdateAsync(Guid id, CreateUpdateChatSessionDto input)
        {
            var chatSession = await _chatSessionRepository.GetAsync(id);
            
            chatSession.Title = input.Title;
            chatSession.IsActive = input.IsActive;
            
            if (!input.IsActive && chatSession.EndedAt == null)
            {
                chatSession.EndedAt = DateTime.Now;
            }

            await _chatSessionRepository.UpdateAsync(chatSession);

            return _chatSessionMapper.Map(chatSession);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _chatSessionRepository.DeleteAsync(id);
        }

        public async Task<PagedResultDto<ChatMessageDto>> GetMessagesAsync(Guid chatSessionId, PagedAndSortedResultRequestDto input)
        {
            var query = await _chatMessageRepository.GetQueryableAsync();
            query = query.Where(x => x.ChatSessionId == chatSessionId);

            var totalCount = await AsyncExecuter.CountAsync(query);

            query = query.OrderBy(x => x.CreationTime)
                         .PageBy(input.SkipCount, input.MaxResultCount);

            var items = await AsyncExecuter.ToListAsync(query);

            return new PagedResultDto<ChatMessageDto>(
                totalCount,
                items.Select(item => _chatMessageMapper.Map(item)).ToList()
            );
        }

        public async Task<ChatMessageDto> SendMessageAsync(CreateUpdateChatMessageDto input)
        {
            var chatMessage = new ChatMessage
            {
                ChatSessionId = input.ChatSessionId,
                Sender = input.Sender,
                Content = input.Content,
                HasInlineAnalysis = input.HasInlineAnalysis,
                InlineAnalysisSummary = input.InlineAnalysisSummary ?? string.Empty
            };

            await _chatMessageRepository.InsertAsync(chatMessage);
            
            // Update session last message time
            var session = await _chatSessionRepository.GetAsync(input.ChatSessionId);
            session.LastMessageTime = DateTime.Now;
            await _chatSessionRepository.UpdateAsync(session);

            return _chatMessageMapper.Map(chatMessage);
        }

        public async Task<ChatMessageDto> AskAiAsync(CreateUpdateChatMessageDto input)
        {
            // 1. Save User Message
            var userMessage = await SendMessageAsync(input);

            // 2. Get History
            var historyQuery = await _chatMessageRepository.GetQueryableAsync();
            var history = await AsyncExecuter.ToListAsync(
                historyQuery.Where(x => x.ChatSessionId == input.ChatSessionId && x.Id != userMessage.Id)
                            .OrderBy(x => x.CreationTime)
            );
            
            var historyDtos = history.Select(x => _chatMessageMapper.Map(x)).ToList();

            // 3. Call Agent
            var aiResponseContent = await _mentalHealthAgentService.ChatAsync(input.Content, historyDtos);

            // 4. Save AI Response
            var aiMessageInput = new CreateUpdateChatMessageDto
            {
                ChatSessionId = input.ChatSessionId,
                Sender = ChatSender.Assistant,
                Content = aiResponseContent,
                HasInlineAnalysis = false
            };

            var aiMessage = await SendMessageAsync(aiMessageInput);

            return aiMessage;
        }
    }
}
