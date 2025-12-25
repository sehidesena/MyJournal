using Mentalfull.Entities.JournalEntries;
using Mentalfull.Entities.Chats;
using Mentalfull.Services.Dtos.JournalEntries;
using Mentalfull.Services.Dtos.Chats;
using Mentalfull.Entities.MoodTracking;
using Mentalfull.Services.Dtos.MoodTracking;
using Mentalfull.Entities.Recommendations;
using Mentalfull.Services.Dtos.Recommendations;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace Mentalfull.ObjectMapping;

// JournalEntry to JournalEntryDto mapper
[Mapper]
public partial class JournalEntryMapper : MapperBase<JournalEntry, JournalEntryDto>
{
    public override partial JournalEntryDto Map(JournalEntry source);
    public override partial void Map(JournalEntry source, JournalEntryDto destination);
}

// MoodLog to MoodLogDto mapper
[Mapper]
public partial class MoodLogMapper : MapperBase<MoodLog, MoodLogDto>
{
    public override partial MoodLogDto Map(MoodLog source);
    public override partial void Map(MoodLog source, MoodLogDto destination);
}

// Recommendation to RecommendationDto mapper
[Mapper]
public partial class RecommendationMapper : MapperBase<Recommendation, RecommendationDto>
{
    public override partial RecommendationDto Map(Recommendation source);
    public override partial void Map(Recommendation source, RecommendationDto destination);
}

// ChatSession to ChatSessionDto mapper
[Mapper]
public partial class ChatSessionMapper : MapperBase<ChatSession, ChatSessionDto>
{
    public override partial ChatSessionDto Map(ChatSession source);
    public override partial void Map(ChatSession source, ChatSessionDto destination);
}

// ChatMessage to ChatMessageDto mapper
[Mapper]
public partial class ChatMessageMapper : MapperBase<ChatMessage, ChatMessageDto>
{
    public override partial ChatMessageDto Map(ChatMessage source);
    public override partial void Map(ChatMessage source, ChatMessageDto destination);
}
