using Volo.Abp.Application.Dtos;

namespace Mentalfull.Services.Dtos.MoodTracking;

public class MoodLogDto : FullAuditedEntityDto<Guid>
{
    public Guid UserId { get; set; }
    public DateTime Timestamp { get; set; }
    public int Intensity { get; set; }
    public string PrimaryEmotion { get; set; } = string.Empty;
    public string? Note { get; set; }
}
