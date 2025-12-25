using System;
using Volo.Abp.Application.Dtos;

namespace Mentalfull.Services.Dtos.Chats
{
    public class ChatSessionDto : AuditedEntityDto<Guid>
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
