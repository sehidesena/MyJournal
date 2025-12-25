using Volo.Abp.Domain.Entities.Auditing;
using Volo.Abp.Identity;

namespace Mentalfull.Entities.MoodTracking;

public class MoodLog : FullAuditedEntity<Guid>
{
    public Guid UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public int Intensity { get; set; } // 1-10
    public string PrimaryEmotion { get; set; } = string.Empty; // e.g., "Happy", "Stressed"
    public string? Note { get; set; }
    
    // Navigation
    public virtual IdentityUser User { get; set; }
}
