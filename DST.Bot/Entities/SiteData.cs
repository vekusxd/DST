namespace DST.Bot.Entities;

public class SiteData
{
    public long Id { get; set; }
    public string? Title { get; set; }
    public string? Url { get; set; }
    public long UserId { get; set; }
    public User User { get; set; } = null!;
}