using DST.Bot.Database;
using DST.Bot.Entities;
using DST.Bot.Features.MainMenu;
using DST.Bot.Features.PsycholigicalTest;
using DST.Bot.Features.SourcesDesign;
using Telegram.Bot.Types;
using User = DST.Bot.Entities.User;

namespace DST.Bot.Features.StateManager;

public static class DialogContext
{
    public static IServiceCollection AddStateManagement(this IServiceCollection services)
    {
        //Позже надо придумать как это регать автоматически (да, это плохо, но чем-то приходиться жертвовать)
        services.AddKeyedScoped<IDialogState, DefaultState>(nameof(DefaultState));
        services.AddKeyedScoped<IDialogState, PsychologicalTest.FirstQuestionState>(
            nameof(PsychologicalTest.FirstQuestionState));
        services.AddKeyedScoped<IDialogState, PsychologicalTest.SecondQuestionState>(
            nameof(PsychologicalTest.SecondQuestionState));
        services.AddKeyedScoped<IDialogState, PsychologicalTest.ThirdQuestionState>(
            nameof(PsychologicalTest.ThirdQuestionState));
        services.AddKeyedScoped<IDialogState, PsychologicalTest.FourthQuestionState>(
            nameof(PsychologicalTest.FourthQuestionState));
        services.AddKeyedScoped<IDialogState, PsychologicalTest.FifthQuestionState>(
            nameof(PsychologicalTest.FifthQuestionState));
        services.AddKeyedScoped<IDialogState, PsychologicalTest.SixthQuestionState>(
            nameof(PsychologicalTest.SixthQuestionState));
        services.AddKeyedScoped<IDialogState, PsychologicalTest.SeventhQuestionState>(
            nameof(PsychologicalTest.SeventhQuestionState));
        services.AddKeyedScoped<IDialogState, GigaChat.GigaChat.GigaChatQuestionState>(
            nameof(GigaChat.GigaChat.GigaChatQuestionState));
        services.AddKeyedScoped<IDialogState, GigaChat.GigaChat.WaitForDefinitionQueryState>(
            nameof(GigaChat.GigaChat.WaitForDefinitionQueryState));
        services.AddKeyedScoped<IDialogState, GigaChat.GigaChat.WaitForSimilarTopicQueryState>(
            nameof(GigaChat.GigaChat.WaitForSimilarTopicQueryState));
        services.AddKeyedScoped<IDialogState, GigaChat.GigaChat.WaitForGeneratedContentQueryState>(
            nameof(GigaChat.GigaChat.WaitForGeneratedContentQueryState));
        services.AddKeyedScoped<IDialogState, GigaChat.GigaChat.WaitForTopicParamQueryState>(
            nameof(GigaChat.GigaChat.WaitForTopicParamQueryState));
        services.AddKeyedScoped<IDialogState, GigaChat.GigaChat.WaitForGenerateTopicCountryQueryState>(
            nameof(GigaChat.GigaChat.WaitForGenerateTopicCountryQueryState));
        services.AddKeyedScoped<IDialogState, GigaChat.GigaChat.WaitForGenerateTopicLanguageQueryState>(
            nameof(GigaChat.GigaChat.WaitForGenerateTopicLanguageQueryState));
        services.AddKeyedScoped<IDialogState, GigaChat.GigaChat.WaitForGenerateTopicScopeQueryState>(
            nameof(GigaChat.GigaChat.WaitForGenerateTopicScopeQueryState));
        services.AddKeyedScoped<IDialogState, GigaChat.GigaChat.WaitForGenerateTopicTimePeriodQueryState>(
            nameof(GigaChat.GigaChat.WaitForGenerateTopicTimePeriodQueryState));
        services.AddKeyedScoped<IDialogState, GetSources.GetSources.WaitSourceQueryState>(
            nameof(GetSources.GetSources.WaitSourceQueryState));
        services.AddKeyedScoped<IDialogState, GenerateFrontPage.GenerateFrontPage.WaitInitialsState>(
            nameof(GenerateFrontPage.GenerateFrontPage.WaitInitialsState));
        services.AddKeyedScoped<IDialogState, GenerateFrontPage.GenerateFrontPage.WaitCourseState>(
            nameof(GenerateFrontPage.GenerateFrontPage.WaitCourseState));
        services.AddKeyedScoped<IDialogState, GenerateFrontPage.GenerateFrontPage.WaitProfileState>(
            nameof(GenerateFrontPage.GenerateFrontPage.WaitProfileState));
        services.AddKeyedScoped<IDialogState, GenerateFrontPage.GenerateFrontPage.WaitThemeState>(
            nameof(GenerateFrontPage.GenerateFrontPage.WaitThemeState));
        services.AddKeyedScoped<IDialogState, GenerateFrontPage.GenerateFrontPage.WaitGroupState>(
            nameof(GenerateFrontPage.GenerateFrontPage.WaitGroupState));
        services.AddKeyedScoped<IDialogState, GenerateFrontPage.GenerateFrontPage.WaitSupervisorInitialsState>(
            nameof(GenerateFrontPage.GenerateFrontPage.WaitSupervisorInitialsState));
        services.AddKeyedScoped<IDialogState, GenerateFrontPage.GenerateFrontPage.WaitSupervisorAcademicTitleState>(
            nameof(GenerateFrontPage.GenerateFrontPage.WaitSupervisorAcademicTitleState));
        services.AddKeyedScoped<IDialogState, GenerateFrontPage.GenerateFrontPage.WaitSupervisorAcademicDegreeState>(
            nameof(GenerateFrontPage.GenerateFrontPage.WaitSupervisorAcademicDegreeState));
        services.AddKeyedScoped<IDialogState, GenerateFrontPage.GenerateFrontPage.WaitSupervisorJobTitleState>(
            nameof(GenerateFrontPage.GenerateFrontPage.WaitSupervisorJobTitleState));
        services.AddKeyedScoped<IDialogState, FirstTimeEncounter.FirstTimeEncounter.FirstTimeState>(
            nameof(FirstTimeEncounter.FirstTimeEncounter.FirstTimeState));
        services.AddKeyedScoped<IDialogState, GetSources.GetSources.FetchArticlesState>(
            nameof(GetSources.GetSources.FetchArticlesState));
        services.AddKeyedScoped<IDialogState, SourcesDesign.SourcesDesign.SourcesDesignQuestionState>(
            nameof(SourcesDesign.SourcesDesign.SourcesDesignQuestionState));
        services.AddKeyedScoped<IDialogState, SourcesDesign.SourcesDesign.SourcesDesignMenuState>(
            nameof(SourcesDesign.SourcesDesign.SourcesDesignMenuState));
        services.AddKeyedScoped<IDialogState, CreateBook.SourcesGenerateBookDesignWaitAuthorLastName>(
            nameof(CreateBook.SourcesGenerateBookDesignWaitAuthorLastName));
        services.AddKeyedScoped<IDialogState, CreateBook.SourcesGenerateBookDesignWaitAuthorInitials>(
            nameof(CreateBook.SourcesGenerateBookDesignWaitAuthorInitials));
        services.AddKeyedScoped<IDialogState, CreateBook.SourcesGenerateBookDesignWaitBookTitle>(
            nameof(CreateBook.SourcesGenerateBookDesignWaitBookTitle));
        services
            .AddKeyedScoped<IDialogState, CreateBook.SourcesGenerateBookDesignWaitPublicationPlace>(
                nameof(CreateBook.SourcesGenerateBookDesignWaitPublicationPlace));
        services
            .AddKeyedScoped<IDialogState, CreateBook.SourcesGenerateBookDesignWaitPublisher>(
                nameof(CreateBook.SourcesGenerateBookDesignWaitPublisher));
        services
            .AddKeyedScoped<IDialogState, CreateBook.SourcesGenerateBookDesignWaitYearOfPublication>(
                nameof(CreateBook.SourcesGenerateBookDesignWaitYearOfPublication));
        services
            .AddKeyedScoped<IDialogState, CreateBook.SourcesGenerateBookDesignWaitNumberOfPages>(
                nameof(CreateBook.SourcesGenerateBookDesignWaitNumberOfPages));
        services
            .AddKeyedScoped<IDialogState, CreateBook.SourcesGenerateBookDesignWaitPublicationDetails>(
                nameof(CreateBook.SourcesGenerateBookDesignWaitPublicationDetails));
        services
            .AddKeyedScoped<IDialogState, CreateBook.SourcesGenerateBookDesignWaitIsbn>(
                nameof(CreateBook.SourcesGenerateBookDesignWaitIsbn));

        services.AddKeyedScoped<IDialogState, CreateSite.GetSiteTitleState>(nameof(CreateSite.GetSiteTitleState));
        services.AddKeyedScoped<IDialogState, CreateSite.GetSiteUrlState>(nameof(CreateSite.GetSiteUrlState));

        services.AddKeyedScoped<IDialogState, CreateSiteArticle.GetArticleTitleState>(
            nameof(CreateSiteArticle.GetArticleTitleState));
        services.AddKeyedScoped<IDialogState, CreateSiteArticle.GetSiteTitleState>(
            nameof(CreateSiteArticle.GetSiteTitleState));
        services.AddKeyedScoped<IDialogState, CreateSiteArticle.GetUrlState>(nameof(CreateSiteArticle.GetUrlState));

        services.AddScoped<DialogContextHandler>();

        return services;
    }

    public class DialogContextHandler
    {
        private readonly AppDbContext _appDbContext;
        private readonly IServiceProvider _keyedServiceProvider;

        public DialogContextHandler(AppDbContext appDbContext, IServiceProvider keyedServiceProvider)
        {
            _appDbContext = appDbContext;
            _keyedServiceProvider = keyedServiceProvider;
        }

        public async Task Handle(Message message)
        {
            var user = await GetUser(message) ?? await RegisterUser(message);
            await _keyedServiceProvider.GetRequiredKeyedService<IDialogState>(user.DialogState).Handle(message, user);
        }

        private async Task<User?> GetUser(Message message)
        {
            var user = await _appDbContext.Users.FindAsync(message.Chat.Id);
            return user;
        }

        private async Task<User> RegisterUser(Message message)
        {
            var newUser = new User
            {
                ChatId = message.Chat.Id, Name = message.From!.FirstName,
                DialogState = nameof(FirstTimeEncounter.FirstTimeEncounter.FirstTimeState)
            };
            var frontPageData = new FrontPageData { User = newUser };
            await _appDbContext.AddRangeAsync([newUser, frontPageData]);
            await _appDbContext.SaveChangesAsync();
            return newUser;
        }
    }
}