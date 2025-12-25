using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mentalfull.Entities.MoodTracking;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Mentalfull.Services.MoodTracking
{
    public class MoodStatsAppService : ApplicationService, IMoodStatsAppService
    {
        private readonly IRepository<MoodLog, Guid> _moodLogRepository;
        private readonly ICurrentUser _currentUser;

        public MoodStatsAppService(IRepository<MoodLog, Guid> moodLogRepository, ICurrentUser currentUser)
        {
            _moodLogRepository = moodLogRepository;
            _currentUser = currentUser;
        }

        public async Task<MoodChartDto> GetWeeklySummaryAsync()
        {
            var endDate = DateTime.Now.Date;
            var startDate = endDate.AddDays(-6); 

            var query = await _moodLogRepository.GetQueryableAsync();
            var logs = query
                .Where(x => x.UserId == _currentUser.Id && x.Timestamp >= startDate)
                .OrderBy(x => x.Timestamp)
                .ToList();

            var dto = new MoodChartDto
            {
                Labels = new List<string>(),
                Data = new List<int>()
            };

            // Fill last 7 days even if empty
            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                dto.Labels.Add(date.ToString("dd MMM")); // e.g. "24 Dec"
                
                // Get average intensity for that day, or 0 if none
                var dayLogs = logs.Where(x => x.Timestamp.Date == date).ToList();
                if (dayLogs.Any())
                {
                    dto.Data.Add((int)Math.Round(dayLogs.Average(x => x.Intensity)));
                }
                else
                {
                    dto.Data.Add(0); // Or maybe null? Chart.js handles null/0 differently. 0 implies "no intensity" or "neutral"? 
                    // Let's assume 0 means "No Data" for now, handled in frontend.
                }
            }
            
            if (logs.Any())
            {
                dto.AverageIntensity = logs.Average(x => x.Intensity);
            }

            return dto;
        }

        public async Task<MoodChartDto> GetMonthlySummaryAsync()
        {
            var endDate = DateTime.Now.Date;
            var startDate = endDate.AddDays(-29);

            var query = await _moodLogRepository.GetQueryableAsync();
            var logs = query
                .Where(x => x.UserId == _currentUser.Id && x.Timestamp >= startDate)
                .OrderBy(x => x.Timestamp)
                .ToList();

             var dto = new MoodChartDto
            {
                Labels = new List<string>(),
                Data = new List<int>()
            };

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                 dto.Labels.Add(date.Day.ToString()); // Just Day number for monthly view usually better, or dd.MM
                 
                 var dayLogs = logs.Where(x => x.Timestamp.Date == date).ToList();
                 if (dayLogs.Any())
                 {
                     dto.Data.Add((int)Math.Round(dayLogs.Average(x => x.Intensity)));
                 }
                 else
                 {
                     dto.Data.Add(0);
                 }
            }

            if (logs.Any())
            {
                dto.AverageIntensity = logs.Average(x => x.Intensity);
            }

            return dto;
        }
    }
}
