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

public static class CreateSiteArticle
{
    public class GetArticleTitleState : IDialogState
    {
        private readonly ITelegramBotClient _botClient;
        private readonly AppDbContext _dbContext;
        private readonly UserHelper _userHelper;
        private readonly MenuHelper _menuHelper;

        public GetArticleTitleState(ITelegramBotClient botClient, AppDbContext dbContext, UserHelper userHelper,
            MenuHelper menuHelper)
        {
            _botClient = botClient;
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
                user.DialogState = nameof(GetArticleSiteTitleState);
                await _dbContext.Entry(user).Reference(u => u.SiteArticleData).LoadAsync();
                user.SiteArticleData = new SiteArticleData { Title = message.Text };
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                await _botClient.SendMessage(message.Chat, "Введите название сайта");
            }
        }
    }

    public class GetArticleSiteTitleState : IDialogState
    {
        private readonly ITelegramBotClient _botClient;
        private readonly AppDbContext _dbContext;
        private readonly UserHelper _userHelper;
        private readonly MenuHelper _menuHelper;

        public GetArticleSiteTitleState(ITelegramBotClient botClient, AppDbContext dbContext, UserHelper userHelper,
            MenuHelper menuHelper)
        {
            _botClient = botClient;
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
                user.DialogState = nameof(GetArticleUrlState);
                await _dbContext.Entry(user).Reference(u => u.SiteArticleData).LoadAsync();
                user.SiteArticleData.SiteTitle = message.Text;
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                await _botClient.SendMessage(message.Chat, "Введите URL сайта");
            }
        }
    }

    public class GetArticleUrlState : IDialogState
    {
        private readonly ITelegramBotClient _botClient;
        private readonly AppDbContext _dbContext;
        private readonly UserHelper _userHelper;
        private readonly MenuHelper _menuHelper;

        public GetArticleUrlState(ITelegramBotClient botClient, AppDbContext dbContext, UserHelper userHelper,
            MenuHelper menuHelper)
        {
            _botClient = botClient;
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
                await _dbContext.Entry(user).Reference(u => u.SiteArticleData).LoadAsync();
                user.SiteArticleData.Url = message.Text;
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                var result = GetGeneratedSiteArticle(user.SiteArticleData);
                await _botClient.SendMessage(message.Chat, result, replyMarkup: MenuHelper.MenuMarkup,
                    parseMode: ParseMode.MarkdownV2);
            }
        }

        private string GetGeneratedSiteArticle(SiteArticleData userSiteArticleData)
        {
            var sb = new StringBuilder();
            sb.AppendLine("```");
            sb.AppendLine(
                $"{userSiteArticleData.Title} // {userSiteArticleData.SiteTitle}: сайт. – URL: {userSiteArticleData.Url}  (дата обращения: {DateTime.Now.ToString("dd.MM.yyyy")})");
            sb.AppendLine("```");
            return sb.ToString();
        }
    }
}