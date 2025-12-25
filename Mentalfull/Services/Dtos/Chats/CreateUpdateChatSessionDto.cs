using System;
using System.ComponentModel.DataAnnotations;

namespace Mentalfull.Services.Dtos.Chats
{
    public class CreateUpdateChatSessionDto
    {
        [Required]
        public string Title { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
}
