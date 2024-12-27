namespace DST.Bot.Entities;

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