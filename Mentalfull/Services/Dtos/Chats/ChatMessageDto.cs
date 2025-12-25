using System;
using Mentalfull.Entities.Chats;
using Volo.Abp.Application.Dtos;

namespace Mentalfull.Services.Dtos.Chats
{
    public class ChatMessageDto : AuditedEntityDto<Guid>
    {
        public Guid ChatSessionId { get; set; }
        public ChatSender Sender { get; set; }
        public string Content { get; set; }
        public bool HasInlineAnalysis { get; set; }
        public string InlineAnalysisSummary { get; set; }
    }
}
