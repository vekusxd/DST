using DST.Bot.Database;
using DST.Bot.Entities;
using DST.Bot.Features.MainMenu;
using DST.Bot.Features.PsycholigicalTest;
using Telegram.Bot.Types;
using User = DST.Bot.Entities.User;

namespace DST.Bot.Features.StateManager;

public static class DialogContext
{
    public static IServiceCollection AddStateManagement(this IServiceCollection services)
    {
        // services.Scan(scan => scan
        //     .FromAssemblyOf<IDialogState>()
        //     .AddClasses(classes => classes.AssignableTo<IDialogState>())
        //     .AsSelfWithInterfaces()
        //     .WithScopedLifetime()
        // );
        
        // var states = AppDomain.CurrentDomain.GetAssemblies()
        //     .SelectMany(s => s.GetTypes())
        //     .Where(p => typeof(IDialogState).IsAssignableFrom(p) && p.IsClass);
        //
        // foreach (var state in states)
        // {
        //     Console.WriteLine(state.Name);
        // }
        
        //Позже надо придумать как это регать автоматически
        services.AddKeyedScoped<IDialogState, DefaultState>(nameof(DefaultState));
        services.AddKeyedScoped<IDialogState, PsychologicalTest.FirstQuestionState>(nameof(PsychologicalTest.FirstQuestionState));
        services.AddKeyedScoped<IDialogState, PsychologicalTest.SecondQuestionState>(nameof(PsychologicalTest.SecondQuestionState));
        services.AddKeyedScoped<IDialogState, PsychologicalTest.ThirdQuestionState>(nameof(PsychologicalTest.ThirdQuestionState));
        services.AddKeyedScoped<IDialogState, PsychologicalTest.FourthQuestionState>(nameof(PsychologicalTest.FourthQuestionState));
        services.AddKeyedScoped<IDialogState, PsychologicalTest.FifthQuestionState>(nameof(PsychologicalTest.FifthQuestionState));
        services.AddKeyedScoped<IDialogState, PsychologicalTest.SixthQuestionState>(nameof(PsychologicalTest.SixthQuestionState));
        services.AddKeyedScoped<IDialogState, PsychologicalTest.SeventhQuestionState>(nameof(PsychologicalTest.SeventhQuestionState));
        services.AddKeyedScoped<IDialogState, GigaChat.GigaChat.GigaChatQuestionState>(nameof(GigaChat.GigaChat.GigaChatQuestionState));
        services.AddKeyedScoped<IDialogState, GigaChat.GigaChat.WaitForDefinitionQueryState>(nameof(GigaChat.GigaChat.WaitForDefinitionQueryState));
        services.AddKeyedScoped<IDialogState, GigaChat.GigaChat.WaitForSimilarTopicQueryState>(nameof(GigaChat.GigaChat.WaitForSimilarTopicQueryState));
        services.AddKeyedScoped<IDialogState, GigaChat.GigaChat.WaitForGeneratedContentQueryState>(nameof(GigaChat.GigaChat.WaitForGeneratedContentQueryState));
        services.AddKeyedScoped<IDialogState, GigaChat.GigaChat.WaitForTopicParamQueryState>(nameof(GigaChat.GigaChat.WaitForTopicParamQueryState));
        services.AddKeyedScoped<IDialogState, GigaChat.GigaChat.WaitForGenerateTopicCountryQueryState>(nameof(GigaChat.GigaChat.WaitForGenerateTopicCountryQueryState));
        services.AddKeyedScoped<IDialogState, GigaChat.GigaChat.WaitForGenerateTopicLanguageQueryState>(nameof(GigaChat.GigaChat.WaitForGenerateTopicLanguageQueryState));
        services.AddKeyedScoped<IDialogState, GigaChat.GigaChat.WaitForGenerateTopicScopeQueryState>(nameof(GigaChat.GigaChat.WaitForGenerateTopicScopeQueryState));
        services.AddKeyedScoped<IDialogState, GigaChat.GigaChat.WaitForGenerateTopicTimePeriodQueryState>(nameof(GigaChat.GigaChat.WaitForGenerateTopicTimePeriodQueryState));
        services.AddKeyedScoped<IDialogState, GetSources.GetSources.WaitSourceQueryState>(nameof(GetSources.GetSources.WaitSourceQueryState));
        services.AddKeyedScoped<IDialogState, GenerateFrontPage.GenerateFrontPage.WaitInitialsState>(nameof(GenerateFrontPage.GenerateFrontPage.WaitInitialsState));
        services.AddKeyedScoped<IDialogState, GenerateFrontPage.GenerateFrontPage.WaitCourseState>(nameof(GenerateFrontPage.GenerateFrontPage.WaitCourseState));
        services.AddKeyedScoped<IDialogState, GenerateFrontPage.GenerateFrontPage.WaitProfileState>(nameof(GenerateFrontPage.GenerateFrontPage.WaitProfileState));
        services.AddKeyedScoped<IDialogState, GenerateFrontPage.GenerateFrontPage.WaitThemeState>(nameof(GenerateFrontPage.GenerateFrontPage.WaitThemeState));
        services.AddKeyedScoped<IDialogState, GenerateFrontPage.GenerateFrontPage.WaitGroupState>(nameof(GenerateFrontPage.GenerateFrontPage.WaitGroupState));
        services.AddKeyedScoped<IDialogState, GenerateFrontPage.GenerateFrontPage.WaitSupervisorInitialsState>(nameof(GenerateFrontPage.GenerateFrontPage.WaitSupervisorInitialsState));
        services.AddKeyedScoped<IDialogState, GenerateFrontPage.GenerateFrontPage.WaitSupervisorAcademicTitleState>(nameof(GenerateFrontPage.GenerateFrontPage.WaitSupervisorAcademicTitleState));
        services.AddKeyedScoped<IDialogState, GenerateFrontPage.GenerateFrontPage.WaitSupervisorAcademicDegreeState>(nameof(GenerateFrontPage.GenerateFrontPage.WaitSupervisorAcademicDegreeState));
        services.AddKeyedScoped<IDialogState, GenerateFrontPage.GenerateFrontPage.WaitSupervisorJobTitleState>(nameof(GenerateFrontPage.GenerateFrontPage.WaitSupervisorJobTitleState));
        services.AddKeyedScoped<IDialogState, FirstTimeEncounter.FirstTimeEncounter.FirstTimeState>(nameof(FirstTimeEncounter.FirstTimeEncounter.FirstTimeState));

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
            var newUser = new User { ChatId = message.Chat.Id, Name = message.From!.FirstName, DialogState = nameof(FirstTimeEncounter.FirstTimeEncounter.FirstTimeState)};
            var frontPageData = new FrontPageData { User = newUser };
            await _appDbContext.AddRangeAsync([newUser, frontPageData]);
            await _appDbContext.SaveChangesAsync();
            return newUser;
        }
    }
}