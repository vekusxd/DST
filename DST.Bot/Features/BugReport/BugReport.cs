using DST.Bot.Database;
using DST.Bot.Entities;
using DST.Bot.Features.Common;
using DST.Bot.Features.MainMenu;
using DST.Bot.Features.StateManager;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = DST.Bot.Entities.User;

namespace DST.Bot.Features.BugReport;

public class BugReportMenuState : IDialogState
{
    private readonly AppDbContext _dbContext;
    private readonly UserHelper _userHelper;
    private readonly ITelegramBotClient _botClient;
    private readonly MenuHelper _menuHelper;

    public BugReportMenuState(AppDbContext dbContext, UserHelper userHelper, ITelegramBotClient botClient,
        MenuHelper menuHelper)
    {
        _dbContext = dbContext;
        _userHelper = userHelper;
        _botClient = botClient;
        _menuHelper = menuHelper;
    }

    public async Task Handle(Message message, User user)
    {
        switch (message.Text)
        {
            case "Отмена":
                await _userHelper.UpdateUserState(user, nameof(DefaultState));
                await _menuHelper.SendMainMenu(message, user);
                break;
            case "Проблемы с функционалом":
            case "Некорректные результаты":
            case "Проблемы с интерфейсом":
            case "Технические сбои":
                await _dbContext.Entry(user).Reference(u => u.BugData).LoadAsync();
                user.BugData = new BugData { Title = message.Text };
                user.DialogState = nameof(BugReportFinalState);
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                await _botClient.SendMessage(message.Chat, "Опишите возникшую проблему",
                    replyMarkup: MenuHelper.CancelButtonMarkup);
                break;
            case "Другое":
                await _userHelper.UpdateUserState(user, nameof(BugReportWaitCategoryState));
                await _botClient.SendMessage(message.Chat,
                    "С чем у вас возникли проблемы?", replyMarkup: MenuHelper.CancelButtonMarkup);
                break;
            default:
                await _menuHelper.SendDefaultErrorMessage(message, user);
                break;
        }
    }
}

public class BugReportWaitCategoryState : IDialogState
{
    private readonly ITelegramBotClient _botClient;
    private readonly AppDbContext _dbContext;
    private readonly MenuHelper _menuHelper;
    private readonly UserHelper _userHelper;

    public BugReportWaitCategoryState(ITelegramBotClient botClient, AppDbContext dbContext, MenuHelper menuHelper,
        UserHelper userHelper)
    {
        _botClient = botClient;
        _dbContext = dbContext;
        _menuHelper = menuHelper;
        _userHelper = userHelper;
    }

    public async Task Handle(Message message, User user)
    {
        switch (message.Text)
        {
            case "Отмена":
                await _userHelper.UpdateUserState(user, nameof(DefaultState));
                await _menuHelper.SendMainMenu(message, user);
                break;
            default:
                await _dbContext.Entry(user).Reference(u => u.BugData).LoadAsync();
                user.BugData = new BugData { Title = message.Text! };
                user.DialogState = nameof(BugReportFinalState);
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                await _botClient.SendMessage(message.Chat, "Опишите возникшую проблему",
                    replyMarkup: MenuHelper.CancelButtonMarkup);
                break;
        }
    }
}

public class BugReportFinalState : IDialogState
{
    private readonly AppDbContext _dbContext;
    private readonly MenuHelper _menuHelper;
    private readonly UserHelper _userHelper;
    private readonly ITelegramBotClient _botClient;

    public BugReportFinalState(AppDbContext dbContext, MenuHelper menuHelper, UserHelper userHelper,
        ITelegramBotClient botClient)
    {
        _dbContext = dbContext;
        _menuHelper = menuHelper;
        _userHelper = userHelper;
        _botClient = botClient;
    }

    public async Task Handle(Message message, User user)
    {
        switch (message.Text)
        {
            case "Отмена":
                await _userHelper.UpdateUserState(user, nameof(DefaultState));
                break;
            default:
                await _dbContext.Entry(user).Reference(u => u.BugData).LoadAsync();
                await _userHelper.UpdateUserState(user, nameof(DefaultState));
                await _botClient.SendMessage(-1002356200554,
                    $"Новая жалоба от @{message.From!.Username}. Категория: {user.BugData.Title}, проблема: {message.Text}");
                break;
        }

        await _menuHelper.SendMainMenu(message, user);
    }
}