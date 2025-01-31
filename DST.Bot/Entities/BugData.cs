namespace DST.Bot.Entities;

public class BugData
{
    public long Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int CountThisDay { get; set; }    
    public long UserId { get; set; }
    public User User { get; set; } = null!;
}