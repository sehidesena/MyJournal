using Volo.Abp.Domain.Entities.Auditing;
using System;

namespace Mentalfull.Entities.Chats
{
    public class ChatMessage : FullAuditedAggregateRoot<Guid>
    {
        public Guid ChatSessionId { get; set; }
        public ChatSender Sender { get; set; }
        public string Content { get; set; }
        public bool HasInlineAnalysis { get; set; }
        public string InlineAnalysisSummary { get; set; }
    }
}
