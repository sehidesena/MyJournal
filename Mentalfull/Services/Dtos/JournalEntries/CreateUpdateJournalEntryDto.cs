using Mentalfull.Enums;
using System.ComponentModel.DataAnnotations;

namespace Mentalfull.Services.Dtos.JournalEntries;

public class CreateUpdateJournalEntryDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;

    [Required]
    public DateTime EntryDate { get; set; }

    public EntryType Type { get; set; }
    
    public string? AudioUrl { get; set; }
    
    public int? DurationSeconds { get; set; }

    public bool? IsPinned { get; set; }
}
