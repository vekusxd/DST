using DST.Bot.Database;
using DST.Bot.Entities;
using DST.Bot.Features.Common;
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
        var states = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => typeof(IDialogState).IsAssignableFrom(p) && p.IsClass);
        
        foreach (var state in states)
        {
            if (services.FirstOrDefault(t => (string) t.ServiceKey! == state.Name) != null) 
                throw new Exception($"State with this name: {state.Name}, already registered, rename it please.");
            services.AddKeyedScoped(typeof(IDialogState), state.Name,state);
        }
        
        services.AddScoped<DialogContextHandler>();

        return services;
    }

    public class DialogContextHandler
    {
        private readonly AppDbContext _appDbContext;
        private readonly IServiceProvider _keyedServiceProvider;
        private readonly UserHelper _userHelper;

        public DialogContextHandler(AppDbContext appDbContext, IServiceProvider keyedServiceProvider, UserHelper userHelper)
        {
            _appDbContext = appDbContext;
            _keyedServiceProvider = keyedServiceProvider;
            _userHelper = userHelper;
        }

        public async Task Handle(Message message)
        {
            var user = await GetUser(message) ?? await RegisterUser(message);
            try
            {
                var service = _keyedServiceProvider.GetRequiredKeyedService<IDialogState>(user.DialogState);
                await service.Handle(message, user);
            }
            //Возникает если в бд хранится отсутствующий id состояния, например был переименован класса стейта.
            //В таких случая просто ставится default состояние
            catch (InvalidOperationException ex)
            {
                await _userHelper.UpdateUserState(user, nameof(DefaultState));
                Console.WriteLine(ex.Message);
                var service = _keyedServiceProvider.GetRequiredKeyedService<IDialogState>(nameof(DefaultState));
                await service.Handle(message, user);
            }
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