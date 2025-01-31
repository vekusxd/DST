using DST.Bot.Database;
using DST.Bot.Entities;
using DST.Bot.Features.Common;
using DST.Bot.Features.Hangfire;
using DST.Bot.Features.MainMenu;
using DST.Bot.Features.StateManager;
using Hangfire;
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
                user.BugData.Title = message.Text;
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
                await _userHelper.UpdateUserState(user, nameof(BugReportMenuState));
                await _botClient.SendMessage(message.Chat, "С чем вы столкнулись?",
                    replyMarkup: MenuHelper.BugReportSelectorMarkup);
                break;
            default:
                await _dbContext.Entry(user).Reference(u => u.BugData).LoadAsync();
                user.BugData.Title = message.Text!;
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
    private readonly BackgroundJobs _backgroundJobs;

    public BugReportFinalState(AppDbContext dbContext, MenuHelper menuHelper, UserHelper userHelper,
        ITelegramBotClient botClient, BackgroundJobs backgroundJobs)
    {
        _dbContext = dbContext;
        _menuHelper = menuHelper;
        _userHelper = userHelper;
        _botClient = botClient;
        _backgroundJobs = backgroundJobs;
    }

    public async Task Handle(Message message, User user)
    {
        switch (message.Text)
        {
            case "Отмена":
                await _userHelper.UpdateUserState(user, nameof(BugReportMenuState));
                await _botClient.SendMessage(message.Chat, "С чем вы столкнулись?",
                    replyMarkup: MenuHelper.BugReportSelectorMarkup);
                break;
            default:
                await _dbContext.Entry(user).Reference(u => u.BugData).LoadAsync();
                await _botClient.SendMessage(-1002356200554,
                    $"Новая жалоба от @{message.From!.Username}. Категория: {user.BugData.Title}, проблема: {message.Text}");
                user.BugData.CountThisDay += 1;
                user.DialogState = nameof(DefaultState);
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                BackgroundJob.Schedule(
                    () => _backgroundJobs.DecrementCounter(user),
                    TimeSpan.FromDays(1));
                await _botClient.SendMessage(message.Chat, "Ваша жалоба была отправлена",
                    replyMarkup: MenuHelper.MenuMarkup);
                break;
        }
    }
}