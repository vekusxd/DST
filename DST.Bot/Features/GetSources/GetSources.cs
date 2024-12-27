using System.Text;
using System.Text.Json.Serialization;
using DST.Bot.Database;
using DST.Bot.Features.Common;
using DST.Bot.Features.StateManager;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using User = DST.Bot.Entities.User;

namespace DST.Bot.Features.GetSources;

public static class GetSources
{
    public static IServiceCollection AddSources(this IServiceCollection services)
    {
        services.AddHttpClient("cyberleninka", o => { o.BaseAddress = new Uri("https://cyberleninka.ru/"); });
        return services;
    }

    public class WaitSourceQueryState : IDialogState
    {
        private readonly AppDbContext _dbContext;
        private readonly ITelegramBotClient _botClient;
        private readonly UserHelper _userHelper;
        private readonly MenuHelper _menuHelper;

        public WaitSourceQueryState(AppDbContext dbContext, ITelegramBotClient botClient, UserHelper userHelper,
            MenuHelper menuHelper)
        {
            _dbContext = dbContext;
            _botClient = botClient;
            _userHelper = userHelper;
            _menuHelper = menuHelper;
        }

        public async Task Handle(Message message, User user)
        {
            switch (message.Text)
            {
                case "Отмена":
                    await _userHelper.UpdateUserState(user, DialogStateId.DefaultState);
                    await _menuHelper.SendMainMenu(message, user);
                    break;
                default:
                    var replyMarkup = new ReplyKeyboardMarkup()
                        .AddNewRow("НЭБ Национальная электронная библиотека")
                        .AddNewRow("Кодекс")
                        .AddNewRow("КиберЛенинка")
                        .AddNewRow("Большая Российская Энциклопедия")
                        .AddNewRow("Отмена");
                    user.DialogStateId = DialogStateId.FetchArticlesState;
                    user.ArticleSearchTerm = message.Text!;
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                    await _botClient.SendMessage(message.Chat, "Выберите один из источников", replyMarkup: replyMarkup);
                    break;
            }
        }

        public DialogStateId DialogStateId { get; } = DialogStateId.WaitSourceQueryState;
    }

    public class FetchArticlesState : IDialogState
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ITelegramBotClient _botClient;
        private readonly UserHelper _userHelper;
        private readonly MenuHelper _menuHelper;

        public FetchArticlesState(IHttpClientFactory clientFactory, ITelegramBotClient botClient,
            UserHelper userHelper, MenuHelper menuHelper)
        {
            _clientFactory = clientFactory;
            _botClient = botClient;
            _userHelper = userHelper;
            _menuHelper = menuHelper;
        }

        public async Task Handle(Message message, User user)
        {
            switch (message.Text)
            {
                case "НЭБ Национальная электронная библиотека":
                    await _botClient.SendMessage(message.Chat, "Здесь должны быть статьи из НЭБ");
                    break;
                case "Кодекс":
                    await _botClient.SendMessage(message.Chat, "Здесь должны быть статьи из <Кодекс>");
                    break;
                case "КиберЛенинка":
                    var stream = await GetCyberLeninkaArticles(message.Text!);
                    await _botClient.SendDocument(message.Chat,
                        InputFile.FromStream(stream, $"articles {DateTime.Now:dd-MM-yyyy hh.mm}.txt"),
                        caption: "Найденные статьи");
                    ;
                    break;
                case "Большая Российская Энциклопедия":
                    await _botClient.SendMessage(message.Chat, "Здесь должны быть статьи из Большой Российской Энциклопедии");
                    break;
                case "Отмена":
                    await _userHelper.UpdateUserState(user, DialogStateId.DefaultState);
                    await _menuHelper.SendMainMenu(message, user);
                    break;
                default:
                    await _botClient.SendMessage(message.Chat, "Выберите доступный вариант!");
                    break;
            }
        }

        private async Task<MemoryStream> GetCyberLeninkaArticles(string search)
        {
            var client = _clientFactory.CreateClient("cyberleninka");

            var response = await client.PostAsJsonAsync("api/search",
                new CyberLeninkaRequest { From = 0, Size = 100, Mode = "articles", Query = search });
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadFromJsonAsync<CyberLeninkaResponse>();

            var articles = responseBody.Articles.Select(r =>
                new CyberLeninkaTelegramResponse
                {
                    Annotation = CleanHtml(r.Annotation),
                    Name = CleanHtml(r.Name),
                    Link = $"https://cyberleninka.ru{r.Link}",
                    Journal = r.Journal,
                    JournalLink = $"https://cyberleninka.ru{r.JournalLink}",
                    AuthorsString = string.Join(", ", r.Authors),
                    Year = r.Year,
                });

            var sb = new StringBuilder();

            foreach (var article in articles)
            {
                sb.AppendLine($"Название статьи: {article.Name}");
                sb.AppendLine($"Аннотация: {article.Annotation}");
                sb.AppendLine($"Ссылка на статью: {article.Link}");
                sb.AppendLine($"Авторы:  {article.AuthorsString}");
                sb.AppendLine($"Год: {article.Year}");
                sb.AppendLine($"Журнал: {article.Journal}");
                sb.AppendLine($"Ссылка на журнал: {article.JournalLink}");
                sb.AppendLine();
            }

            return new MemoryStream(Encoding.UTF8.GetBytes(sb.ToString()));
        }


        public DialogStateId DialogStateId { get; } = DialogStateId.FetchArticlesState;
    }

    private static string CleanHtml(string input) => input
        .Replace("<b>", "")
        .Replace("</b>", "")
        .Replace("<br>", "")
        .Replace("<br/>", "")
        .Replace("<div>", "")
        .Replace("</div>", "")
        .Replace("&quot", "\"")
        .Trim();


    public class CyberLeninkaRequest
    {
        [JsonPropertyName("mode")] public required string Mode { get; set; }
        [JsonPropertyName("q")] public required string Query { get; set; }
        [JsonPropertyName("size")] public int Size { get; set; }
        [JsonPropertyName("from")] public int From { get; set; }
    }

    public class CyberLeninkaResponse
    {
        public int Found { get; set; }
        public CyberLeninkaArticle[] Articles { get; set; } = [];
    }

    public class CyberLeninkaTelegramResponse
    {
        public required string Name { get; set; } = string.Empty;
        public required string Annotation { get; set; } = string.Empty;
        public required string Link { get; set; } = string.Empty;
        public required string AuthorsString { get; set; } = string.Empty;
        public required int Year { get; set; }
        public required string Journal { get; set; } = string.Empty;
        public required string JournalLink { get; set; } = string.Empty;
    }

    public class CyberLeninkaArticle
    {
        public string Name { get; set; } = string.Empty;
        public string Annotation { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
        public string[] Authors { get; set; } = [];
        public int Year { get; set; }
        public string Journal { get; set; } = string.Empty;
        public string JournalLink { get; set; } = string.Empty;
    }
}