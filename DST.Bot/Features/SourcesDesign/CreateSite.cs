using System.Text;
using DST.Bot.Database;
using DST.Bot.Entities;
using DST.Bot.Features.Common;
using DST.Bot.Features.MainMenu;
using DST.Bot.Features.StateManager;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using User = DST.Bot.Entities.User;

namespace DST.Bot.Features.SourcesDesign;

public static class CreateSite
{
    public class GetSiteTitleState : IDialogState
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly AppDbContext _dbContext;
        private readonly UserHelper _userHelper;
        private readonly MenuHelper _menuHelper;

        public GetSiteTitleState(ITelegramBotClient telegramBotClient, AppDbContext dbContext, UserHelper userHelper,MenuHelper menuHelper)
        {
            _telegramBotClient = telegramBotClient;
            _dbContext = dbContext;
            _userHelper = userHelper;
            _menuHelper = menuHelper;
        }
        
        public async Task Handle(Message message, User user)
        {
            if (message.Text!.Equals("Отмена", StringComparison.InvariantCultureIgnoreCase))
            {
                await _userHelper.UpdateUserState(user, nameof(SourcesDesign.SourcesDesignMenuState));
                await _menuHelper.SendDesignSourceSelectorMenu(message, user);
            }
            else
            {
                user.DialogState = nameof(GetSiteUrlState);
                await _dbContext.Entry(user).Reference(u => u.SiteData).LoadAsync();
                user.SiteData = new SiteData {  Title= message.Text };
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                await _telegramBotClient.SendMessage(message.Chat, "Введите URL сайта");
            }
        }
    }

    public class GetSiteUrlState : IDialogState
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly AppDbContext _dbContext;
        private readonly UserHelper _userHelper;
        private readonly MenuHelper _menuHelper;

        public GetSiteUrlState(ITelegramBotClient telegramBotClient, AppDbContext dbContext, UserHelper userHelper,MenuHelper menuHelper)
        {
            _telegramBotClient = telegramBotClient;
            _dbContext = dbContext;
            _userHelper = userHelper;
            _menuHelper = menuHelper;
        }
        
        public async Task Handle(Message message, User user)
        {
            if (message.Text!.Equals("Отмена", StringComparison.InvariantCultureIgnoreCase))
            {
                await _userHelper.UpdateUserState(user, nameof(SourcesDesign.SourcesDesignMenuState));
                await _menuHelper.SendDesignSourceSelectorMenu(message, user);
            }
            else
            {
                user.DialogState = nameof(DefaultState);
                await _dbContext.Entry(user).Reference(u => u.SiteData).LoadAsync();
                user.SiteData.Url = message.Text;
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                var result = GetGeneratedSite(user.SiteData);
                await _telegramBotClient.SendMessage(message.Chat, result, replyMarkup: MenuHelper.MenuMarkup, parseMode: ParseMode.MarkdownV2);
            }
        }

        private string GetGeneratedSite(SiteData userSiteData)
        {
            var sb = new StringBuilder();
            sb.AppendLine("```");
            sb.AppendLine(
                $"{userSiteData.Title}: сайт.-URL:{userSiteData.Url} (дата обращения: {DateTime.Now.ToString("dd.MM.yyyy")})");
            sb.AppendLine("```");
            return sb.ToString();
        }
    }
}