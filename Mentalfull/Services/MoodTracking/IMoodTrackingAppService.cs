using System.Collections.Generic;
using System.Threading.Tasks;
using Mentalfull.Services.Dtos.MoodTracking;
using Volo.Abp.Application.Services;

namespace Mentalfull.Services.MoodTracking;

public interface IMoodTrackingAppService : IApplicationService
{
    Task<MoodLogDto> CreateAsync(CreateMoodLogDto input);
    Task<List<MoodLogDto>> GetHistoryAsync(DateTime startDate, DateTime endDate);
    Task<Dictionary<string, int>> GetEmotionStatsAsync(DateTime startDate, DateTime endDate);
}
