using Telegram.Bot.Types;
using User = DST.Bot.Entities.User;

namespace DST.Bot.Features.StateManager;

public interface IDialogState
{
    public Task Handle(Message message ,User user);
}
