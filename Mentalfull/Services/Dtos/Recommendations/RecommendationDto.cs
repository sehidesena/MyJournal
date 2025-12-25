using Mentalfull.Entities.Recommendations;
using Volo.Abp.Application.Dtos;

namespace Mentalfull.Services.Dtos.Recommendations;

public class RecommendationDto : FullAuditedEntityDto<Guid>
{
    public Guid UserId { get; set; }
    public RecommendationType Type { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? ExternalUrl { get; set; }
    public string? ImageUrl { get; set; }
    public string Reasoning { get; set; } = string.Empty;
}
