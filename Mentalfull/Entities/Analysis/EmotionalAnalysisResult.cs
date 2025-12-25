using Mentalfull.Entities.JournalEntries;
using Volo.Abp.Domain.Entities.Auditing;

namespace Mentalfull.Entities.Analysis;

public class EmotionalAnalysisResult : FullAuditedEntity<Guid>
{
    public Guid JournalEntryId { get; set; }
    
    public float SentimentScore { get; set; } // -1.0 (Negative) to +1.0 (Positive)
    public string DominantEmotion { get; set; } = string.Empty;
    public string AnalysisSummary { get; set; } = string.Empty;
    public string EmotionProbabilities { get; set; } = string.Empty; // JSON string: {"Joy": 0.8, "Sadness": 0.1}
    public string? ClinicalFlags { get; set; } // JSON string for risk markers

    // Navigation
    public virtual JournalEntry JournalEntry { get; set; }
}
