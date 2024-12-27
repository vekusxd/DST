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
}