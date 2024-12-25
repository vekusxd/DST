namespace DST.Bot.Entities;

public class FrontPageData
{
    public long Id { get; set; }

    /// <summary>
    /// Направление
    /// </summary>
    public string? Course { get; set; }

    /// <summary>
    /// Профиль
    /// </summary>
    public string? Profile { get; set; }

    /// <summary>
    /// Тема работы
    /// </summary>
    public string? Theme { get; set; }

    /// <summary>
    /// Инициалы
    /// </summary>
    public string? Initials { get; set; }

    /// <summary>
    /// Группа
    /// </summary>
    public string? Group { get; set; }

    public int Year { get; init; }

    /// <summary>
    /// Инициалы научного руководителя
    /// </summary>
    public string? SupervisorInitials { get; set; }

    /// <summary>
    /// Ученое звание научного руководителя
    /// </summary>
    public string? SupervisorAcademicTitle { get; set; }

    /// <summary>
    /// Ученая степень научного руководителя
    /// </summary>
    public string? SupervisorAcademicDegree { get; set; }

    /// <summary>
    /// Должность научного руководителя
    /// </summary>
    public string? SupervisorJobTitle { get; set; }

    public long UserId { get; set; }
    public User User { get; set; } = null!;
}