using System.Diagnostics.CodeAnalysis;

namespace DST.Bot.Features.StateManager;

/// <summary>
/// Уникальные Id для состояний диалога, желательно не создавать реализаций  у которых они совпадают, поведение не тестировалось
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")] //тварь ругается
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
    WaitSourceQueryState,
    FetchArticlesState,
    FirstTimeState,
    PsychologicalTestFirstQuestionState,
    PsychologicalTestSecondQuestionState,
    PsychologicalTestThirdQuestionState,
    PsychologicalTestFourthQuestionState,
    PsychologicalTestFifthQuestionState,
    PsychologicalTestSixthQuestionState,
    PsychologicalTestXSeventhQuestionState,
    GigaChatQuestionState,
    WaitForDefinitionQueryState
}