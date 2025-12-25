using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mentalfull.Entities.MoodTracking;
using Mentalfull.Entities.Recommendations;
using Mentalfull.Services.Dtos.Recommendations;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;

namespace Mentalfull.Services.Recommendations;

[Authorize]
public class RecommendationAppService : ApplicationService, IRecommendationAppService
{
    private readonly IRepository<Recommendation, Guid> _recommendationRepository;
    private readonly IRepository<MoodLog, Guid> _moodLogRepository;
    private readonly ICurrentUser _currentUser;

    public RecommendationAppService(
        IRepository<Recommendation, Guid> recommendationRepository,
        IRepository<MoodLog, Guid> moodLogRepository,
        ICurrentUser currentUser)
    {
        _recommendationRepository = recommendationRepository;
        _moodLogRepository = moodLogRepository;
        _currentUser = currentUser;
    }

    public async Task<List<RecommendationDto>> GetMyRecommendationsAsync()
    {
        var queryable = await _recommendationRepository.GetQueryableAsync();
        
        var list = await AsyncExecuter.ToListAsync(
            queryable
                .Where(x => x.UserId == _currentUser.Id)
                .OrderByDescending(x => x.CreationTime)
                .Take(20) // Limit to last 20
        );

        return ObjectMapper.Map<List<Recommendation>, List<RecommendationDto>>(list);
    }

    public async Task<List<RecommendationDto>> GenerateRefreshedRecommendationsAsync()
    {
        var moodQuery = await _moodLogRepository.GetQueryableAsync();
        var latestMood = await AsyncExecuter.FirstOrDefaultAsync(
            moodQuery.Where(x => x.UserId == _currentUser.Id)
                     .OrderByDescending(x => x.Timestamp)
        );

        var recommendations = new List<Recommendation>();

        // MOCK LOGIC: Real logic would call AI
        if (latestMood != null)
        {
            // High Stress/Anxiety
            if (latestMood.Intensity > 6 && (latestMood.PrimaryEmotion.Contains("Stress") || latestMood.PrimaryEmotion.Contains("Anxiety") || latestMood.PrimaryEmotion.Contains("Fear")))
            {
                recommendations.Add(new Recommendation
                {
                    UserId = _currentUser.Id!.Value,
                    Type = RecommendationType.Meditation,
                    Title = "5 Dakikalık Nefes Egzersizi",
                    ExternalUrl = "https://example.com/breathing",
                    Reasoning = $"Yüksek stres seviyenize ({latestMood.Intensity}/10) dayanarak.",
                    ContextSource = "MoodLog"
                });
                recommendations.Add(new Recommendation
                {
                    UserId = _currentUser.Id!.Value,
                    Type = RecommendationType.Music,
                    Title = "Sakinleştirici Lo-Fi Listesi",
                    ExternalUrl = "https://spotify.com/lofi",
                    Reasoning = "Rahatlamanıza yardımcı olmak için.",
                    ContextSource = "MoodLog"
                });
            }
            // Sadness/Depression
            else if (latestMood.PrimaryEmotion.Contains("Sad") || latestMood.PrimaryEmotion.Contains("Grief") || latestMood.PrimaryEmotion.Contains("Depress"))
            {
                 recommendations.Add(new Recommendation
                {
                    UserId = _currentUser.Id!.Value,
                    Type = RecommendationType.Music,
                    Title = "Akustik ve Sakin",
                    ExternalUrl = "https://spotify.com/acoustic",
                    Reasoning = "Sizi sarmalayacak yumuşak melodiler.",
                    ContextSource = "MoodLog"
                });
                 recommendations.Add(new Recommendation
                {
                    UserId = _currentUser.Id!.Value,
                    Type = RecommendationType.Book,
                    Title = "Küçük Prens",
                    Reasoning = "Ruhunuza iyi gelecek bir klasik.",
                    ContextSource = "MoodLog"
                });
            }
             // Happiness/Joy
            else if (latestMood.PrimaryEmotion.Contains("Happy") || latestMood.PrimaryEmotion.Contains("Joy") || latestMood.PrimaryEmotion.Contains("Excite"))
            {
                 recommendations.Add(new Recommendation
                {
                    UserId = _currentUser.Id!.Value,
                    Type = RecommendationType.Music,
                    Title = "Enerjik Pop Listesi",
                    ExternalUrl = "https://spotify.com/pop",
                    Reasoning = "Enerjinizi zirvede tutmak için!",
                    ContextSource = "MoodLog"
                });
                  recommendations.Add(new Recommendation
                {
                    UserId = _currentUser.Id!.Value,
                    Type = RecommendationType.Activity,
                    Title = "Dans Et veya Yürüyüşe Çık",
                    Reasoning = "Bu güzel enerjiyi harekete dönüştürün.",
                    ContextSource = "MoodLog"
                });
            }
            // Anger
            else if (latestMood.PrimaryEmotion.Contains("Anger") || latestMood.PrimaryEmotion.Contains("Frustra"))
            {
                 recommendations.Add(new Recommendation
                {
                    UserId = _currentUser.Id!.Value,
                    Type = RecommendationType.Activity,
                    Title = "Tempolu Koşu",
                    Reasoning = "Negatif enerjiyi atmak için birebir.",
                    ContextSource = "MoodLog"
                });
                 recommendations.Add(new Recommendation
                {
                    UserId = _currentUser.Id!.Value,
                    Type = RecommendationType.Music,
                    Title = "Odaklanma & Enstrümantal",
                    ExternalUrl = "https://spotify.com/focus",
                    Reasoning = "Zihninizi sakinleştirmek için.",
                    ContextSource = "MoodLog"
                });
            }
            // Default/Neutral
            else
            {
                recommendations.Add(new Recommendation
                {
                    UserId = _currentUser.Id!.Value,
                    Type = RecommendationType.Activity,
                    Title = "Kısa Bir Yürüyüş Yapın",
                    Reasoning = "Dengeli bir ruh hali için yürüyüş her zaman iyidir.",
                    ContextSource = "General"
                });
                 recommendations.Add(new Recommendation
                {
                    UserId = _currentUser.Id!.Value,
                    Type = RecommendationType.Music,
                    Title = "Günün Keşfi",
                    ExternalUrl = "https://spotify.com/discover",
                    Reasoning = "Modunuza eşlik edecek rastgele tınılar.",
                    ContextSource = "General"
                });
            }
        }
        else
        {
            // Default when no mood log
            recommendations.Add(new Recommendation
            {
                UserId = _currentUser.Id!.Value,
                Type = RecommendationType.Article,
                Title = "Mentalfull'a Hoş Geldiniz: Başlangıç",
                Reasoning = "Başlamanıza yardımcı olmak için.",
                ContextSource = "Onboarding"
            });
             recommendations.Add(new Recommendation
            {
                UserId = _currentUser.Id!.Value,
                Type = RecommendationType.Music,
                Title = "Ruh Halini Yansıt",
                Reasoning = "İlk mod kaydınızı girerek size özel müzikler keşfedin!",
                ContextSource = "Onboarding"
            });
        }

        await _recommendationRepository.InsertManyAsync(recommendations);

        return ObjectMapper.Map<List<Recommendation>, List<RecommendationDto>>(recommendations);
    }
}
