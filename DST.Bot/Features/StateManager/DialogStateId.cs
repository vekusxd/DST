namespace DST.Bot.Features.StateManager;

/// <summary>
/// Уникальные Id для состояний диалога, желательно не создавать реализаций  у которых они совпадают, поведение не тестировалось
/// </summary>
public enum DialogStateId
{
    /// <summary>
    /// Состояние по умолчанию, главное меню
    /// </summary>
    DefaultState,
    FrontPageWaitInitials,
    FrontPageWaitCourse,
    FrontPageWaitProfile,
    FrontPageWaitTheme,
    FrontPageWaitGroup,
    FrontPageWaitSupervisorInitials,
    FrontPageWaitSupervisorAcademicTitle,
    FrontPageWaitSupervisorAcademicDegree,
    FrontPageWaitSupervisorJobTitle,
    WaitSourceQueryState
}