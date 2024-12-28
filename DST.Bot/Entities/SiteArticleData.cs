namespace DST.Bot.Entities;

public class SiteArticleData
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? SiteTitle { get; set; }
    public string? Url { get; set; } 
    public long UserId { get; set; }
    public User User { get; set; } = null!;
}