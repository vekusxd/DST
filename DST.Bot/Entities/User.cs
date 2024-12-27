using DST.Bot.Features.StateManager;

namespace DST.Bot.Entities;

public class User
{
    public long ChatId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DialogStateId DialogStateId { get; set; } = DialogStateId.FirstTimeState;
    public string ArticleSearchTerm { get; set; } = string.Empty;
    public FrontPageData FrontPageData { get; set; } = null!;
    public PsychologicalType PsychologicalType { get; set; } = PsychologicalType.NotSet;
    public int PsychologicalTestPoints { get; set; } = 0;
    public GenerateTopicData GenerateTopicData { get; set; } = null!;
}

public class GenerateTopicData
{
    public long Id { get; set; }
    public string? Country { get; set; }
    public string? Language { get; set; }
    public string? Scope { get; set; }
    public string? TimePeriod { get; set; }
    public long UserId { get; set; }
    public User User { get; set; } = null!;
}