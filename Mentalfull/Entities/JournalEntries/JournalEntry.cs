using Mentalfull.Entities.Analysis;
using Mentalfull.Enums;
using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Mentalfull.Entities.JournalEntries
{
    public class JournalEntry : FullAuditedAggregateRoot<Guid>
    {
        public Guid UserId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime EntryDate { get; set; }
        
        // Voice Journal Support
        public EntryType Type { get; set; }
        public string? AudioUrl { get; set; } // Path or URL to stored audio file
        public int? DurationSeconds { get; set; }

        public bool? IsPinned { get; set; }
        public bool HasAiAnalysis { get; set; }

        // Relationships
        public virtual IdentityUser User { get; set; }
        public virtual EmotionalAnalysisResult? AnalysisResult { get; set; } // Detailed Analysis
    }
}
