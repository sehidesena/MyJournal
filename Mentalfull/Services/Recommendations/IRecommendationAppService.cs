using System.Collections.Generic;
using System.Threading.Tasks;
using Mentalfull.Services.Dtos.Recommendations;
using Volo.Abp.Application.Services;

namespace Mentalfull.Services.Recommendations;

public interface IRecommendationAppService : IApplicationService
{
    Task<List<RecommendationDto>> GetMyRecommendationsAsync();
    
    // Generates personalized recommendations based on current user state
    Task<List<RecommendationDto>> GenerateRefreshedRecommendationsAsync();
}
