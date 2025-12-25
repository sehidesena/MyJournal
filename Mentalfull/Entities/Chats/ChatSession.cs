using Volo.Abp.Domain.Entities.Auditing;
using System;

namespace Mentalfull.Entities.Chats
{
    public class ChatSession : FullAuditedAggregateRoot<Guid>
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? EndedAt { get; set; }
        public bool IsActive { get; set; }
        public string AiSummary { get; set; }
        public DateTime? LastMessageTime { get; set; }
    }
}
