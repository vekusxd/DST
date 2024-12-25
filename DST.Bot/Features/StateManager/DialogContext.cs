using DST.Bot.Database;
using DST.Bot.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = DST.Bot.Entities.User;

namespace DST.Bot.Features.StateManager;

public static class DialogContext
{
    public static IServiceCollection AddStateManagement(this IServiceCollection services)
    {
        services.Scan(scan => scan
            .FromAssemblyOf<IDialogState>()
            .AddClasses(classes => classes.AssignableTo<IDialogState>())
            .AsImplementedInterfaces()
            .WithScopedLifetime()
        );

        services.AddScoped<DialogContextHandler>();
        return services;
    }

    public class DialogContextHandler
    {
        private readonly AppDbContext _appDbContext;
        private readonly Dictionary<DialogStateId, IDialogState> _states;

        public DialogContextHandler(IEnumerable<IDialogState> dialogStates, AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
            _states = dialogStates.ToDictionary(
                state => state.DialogStateId,
                state => state
            );
        }

        public async Task Handle(Message message)
        {
            var user = await GetUser(message) ?? await RegisterUser(message);
            await _states[user.DialogStateId].Handle(message, user);
        }

        private async Task<User?> GetUser(Message message)
        {
            var user = await _appDbContext.Users.FindAsync(message.Chat.Id);
            return user;
        }

        private async Task<User> RegisterUser(Message message)
        {
            var newUser = new User { ChatId = message.Chat.Id, Name = message.From!.FirstName};
            var frontPageData = new FrontPageData { User = newUser };
            await _appDbContext.AddRangeAsync([newUser, frontPageData]);
            await _appDbContext.SaveChangesAsync();
            return newUser;
        }
    }
}