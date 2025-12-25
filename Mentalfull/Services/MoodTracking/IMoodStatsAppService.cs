using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace Mentalfull.Services.MoodTracking
{
    public interface IMoodStatsAppService : IApplicationService
    {
        Task<MoodChartDto> GetWeeklySummaryAsync();
        Task<MoodChartDto> GetMonthlySummaryAsync();
    }

    public class MoodChartDto
    {
        public List<string> Labels { get; set; } = new();
        public List<int> Data { get; set; } = new();
        public double AverageIntensity { get; set; }
    }
}
