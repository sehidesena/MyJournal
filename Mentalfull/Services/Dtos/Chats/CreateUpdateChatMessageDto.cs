using System;
using System.ComponentModel.DataAnnotations;
using Mentalfull.Entities.Chats;

namespace Mentalfull.Services.Dtos.Chats
{
    public class CreateUpdateChatMessageDto
    {
        [Required]
        public Guid ChatSessionId { get; set; }
        
        [Required]
        public ChatSender Sender { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        public bool HasInlineAnalysis { get; set; }
        public string? InlineAnalysisSummary { get; set; }
    }
}
