using DST.Bot.Features.FirstTimeEncounter;
using Newtonsoft.Json;

namespace DST.Bot.Entities;

public class User
{
    public long ChatId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DialogState { get; set; } = nameof(FirstTimeEncounter.FirstTimeState);
    public string ArticleSearchTerm { get; set; } = string.Empty;
    [JsonIgnore] public FrontPageData FrontPageData { get; set; } = null!;
    public int PsychologicalTestPoints { get; set; } = 0;
    [JsonIgnore] public GenerateTopicData GenerateTopicData { get; set; } = null!;
    [JsonIgnore] public BookDesignData BookDesignData { get; set; } = null!;
    [JsonIgnore] public SiteData SiteData { get; set; } = null!;
    [JsonIgnore] public SiteArticleData SiteArticleData { get; set; } = null!;
    [JsonIgnore] public BugData BugData { get; set; } = null!;
}