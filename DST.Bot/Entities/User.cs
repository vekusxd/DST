using DST.Bot.Features.FirstTimeEncounter;
using DST.Bot.Features.StateManager;

namespace DST.Bot.Entities;

public class User
{
    public long ChatId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DialogState { get; set; } = nameof(FirstTimeEncounter.FirstTimeState);
    public string ArticleSearchTerm { get; set; } = string.Empty;
    public FrontPageData FrontPageData { get; set; } = null!;
    public int PsychologicalTestPoints { get; set; } = 0;
    public GenerateTopicData GenerateTopicData { get; set; } = null!;
    public BookDesignData BookDesignData { get; set; } = null!;
}