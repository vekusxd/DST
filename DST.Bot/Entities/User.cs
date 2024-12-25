using DST.Bot.Features.StateManager;

namespace DST.Bot.Entities;

public class User
{
    public long ChatId { get; set; }
    public string Name { get; set; } = string.Empty; 
    public DialogStateId DialogStateId { get; set; }
    public FrontPageData FrontPageData { get; set; } = null!;
}