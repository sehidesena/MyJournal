using System;
using System.Threading.Tasks;
using Mentalfull.Entities.JournalEntries;
using Mentalfull.Services.Analysis;
using Shouldly;
using Volo.Abp.Domain.Repositories;
using Xunit;

namespace Mentalfull.Services.Analysis;

public class AnalysisServiceTests : MentalfullTestBase
{
    private readonly IAnalysisService _analysisService;

    public AnalysisServiceTests()
    {
        _analysisService = GetRequiredService<IAnalysisService>();
    }

    [Fact]
    public async Task AnalyzeJournalEntryAsync_Should_Return_Positive_Result_For_Happy_Content()
    {
        // Arrange
        var entry = new JournalEntry
        {
            UserId = Guid.NewGuid(),
            Title = "Happy Day",
            Content = "Bugün çok mutlu ve umutlu hissediyorum.",
            EntryDate = DateTime.Now
        };

        // Act
        var result = await _analysisService.AnalyzeJournalEntryAsync(entry);

        // Assert
        result.ShouldNotBeNull();
        result.JournalEntryId.ShouldBe(entry.Id);
        result.SentimentScore.ShouldBeGreaterThan(0);
        result.DominantEmotion.ShouldBe("Mutluluk");
    }

    [Fact]
    public async Task AnalyzeJournalEntryAsync_Should_Return_AnalysisSummary()
    {
         // Arrange
        var entry = new JournalEntry
        {
            UserId = Guid.NewGuid(),
            Title = "Normal Day",
            Content = "Normal bir gündü.",
            EntryDate = DateTime.Now
        };

        // Act
        var result = await _analysisService.AnalyzeJournalEntryAsync(entry);

        // Assert
        result.AnalysisSummary.ShouldNotBeNullOrEmpty();
    }
}
