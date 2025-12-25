using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Mentalfull.Entities.Recommendations;


public class Recommendation : FullAuditedEntity<Guid>
{
    public Guid UserId { get; set; }
    
    public RecommendationType Type { get; set; }
    public string Title { get; set; }
    public string? ExternalUrl { get; set; } // e.g., Spotify link
    public string? ImageUrl { get; set; }
    
    public string Reasoning { get; set; } // "Because you felt stressed..."
    
    // Optional: Link to source of recommendation
    public string? ContextSource { get; set; } // "MoodLog", "JournalEntry"
    public Guid? SourceId { get; set; }

    // Navigation
    public virtual IdentityUser User { get; set; }
}
