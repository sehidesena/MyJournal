using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mentalfull.Entities.MoodTracking;
using Mentalfull.Services.Dtos.MoodTracking;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Mentalfull.Services.MoodTracking;

[Authorize]
public class MoodTrackingAppService : ApplicationService, IMoodTrackingAppService
{
    private readonly IRepository<MoodLog, Guid> _moodLogRepository;
    private readonly ICurrentUser _currentUser;

    public MoodTrackingAppService(
        IRepository<MoodLog, Guid> moodLogRepository,
        ICurrentUser currentUser)
    {
        _moodLogRepository = moodLogRepository;
        _currentUser = currentUser;
    }

    public async Task<MoodLogDto> CreateAsync(CreateMoodLogDto input)
    {
        var moodLog = new MoodLog
        {
            UserId = _currentUser.Id!.Value,
            Timestamp = input.Timestamp,
            Intensity = input.Intensity,
            PrimaryEmotion = input.PrimaryEmotion,
            Note = input.Note
        };

        await _moodLogRepository.InsertAsync(moodLog);

        return ObjectMapper.Map<MoodLog, MoodLogDto>(moodLog);
    }

    public async Task<List<MoodLogDto>> GetHistoryAsync(DateTime startDate, DateTime endDate)
    {
        var queryable = await _moodLogRepository.GetQueryableAsync();
        
        var history = await AsyncExecuter.ToListAsync(
            queryable
                .Where(x => x.UserId == _currentUser.Id && x.Timestamp >= startDate && x.Timestamp <= endDate)
                .OrderBy(x => x.Timestamp)
        );

        return ObjectMapper.Map<List<MoodLog>, List<MoodLogDto>>(history);
    }

    public async Task<Dictionary<string, int>> GetEmotionStatsAsync(DateTime startDate, DateTime endDate)
    {
        var queryable = await _moodLogRepository.GetQueryableAsync();

        var logs = await AsyncExecuter.ToListAsync(
            queryable
                .Where(x => x.UserId == _currentUser.Id && x.Timestamp >= startDate && x.Timestamp <= endDate)
        );

        return logs
            .GroupBy(x => x.PrimaryEmotion)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}
