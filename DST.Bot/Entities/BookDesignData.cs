namespace DST.Bot.Entities;

public class BookDesignData
{
    public long Id { get; set; }
    public string? AuthorSurname { get; set; }
    public string? AuthorInitials { get; set; }
    public string? BookTitle { get; set; }
    public string? PublicationPlace { get; set; }
    public string? Publisher { get; set; }
    public int YearOfPublication { get; set; }
    public int NumberOfPages { get; set; }
    public string? PublicationDetails { get; set; }
    public string? Isbn { get; set; }
    public long UserId { get; set; }
    public User User { get; set; } = null!;
}