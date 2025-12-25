using Mentalfull.Enums;
using Volo.Abp.Application.Dtos;

namespace Mentalfull.Services.Dtos.JournalEntries;

public class JournalEntryDto : FullAuditedEntityDto<Guid>
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime EntryDate { get; set; }
    public EntryType Type { get; set; }
    public string? AudioUrl { get; set; }
    public int? DurationSeconds { get; set; }
    public bool? IsPinned { get; set; }
    public bool HasAiAnalysis { get; set; }
}
