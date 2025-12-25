using System.ComponentModel.DataAnnotations;

namespace Mentalfull.Services.Dtos.MoodTracking;

public class CreateMoodLogDto
{
    [Required]
    public DateTime Timestamp { get; set; }

    [Range(1, 10)]
    public int Intensity { get; set; }

    [Required]
    [StringLength(50)]
    public string PrimaryEmotion { get; set; } = string.Empty;

    public string? Note { get; set; }
}
