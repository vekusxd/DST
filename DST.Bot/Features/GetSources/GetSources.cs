using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using DST.Bot.Database;
using DST.Bot.Features.Common;
using DST.Bot.Features.MainMenu;
using DST.Bot.Features.StateManager;
using HtmlAgilityPack;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using User = DST.Bot.Entities.User;

namespace DST.Bot.Features.GetSources;

public static partial class GetSources
{
    public static IServiceCollection AddSources(this IServiceCollection services)
    {
        services.AddHttpClient("cyberleninka", o => { o.BaseAddress = new Uri("https://cyberleninka.ru/"); });
        services.AddHttpClient("rusneb", o => { o.BaseAddress = new Uri("https://rusneb.ru/search/"); });
        services.AddHttpClient("codex", o => { o.BaseAddress = new Uri("https://docs.cntd.ru/search"); });
        services.AddHttpClient("bigenc",
            o => { o.BaseAddress = new Uri("https://c.bigenc.ru/content/search/suggestions"); });

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
                    await _userHelper.UpdateUserState(user, nameof(DefaultState));
                    await _menuHelper.SendMainMenu(message, user);
                    break;
                default:
                    var replyMarkup = new ReplyKeyboardMarkup()
                        .AddNewRow("НЭБ Национальная электронная библиотека")
                        .AddNewRow("Кодекс")
                        .AddNewRow("КиберЛенинка")
                        .AddNewRow("Большая Российская Энциклопедия")
                        .AddNewRow("Отмена");
                    user.DialogState = nameof(FetchArticlesState);
                    user.ArticleSearchTerm = message.Text!;
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                    await _botClient.SendMessage(message.Chat, "Выберите один из источников", replyMarkup: replyMarkup);
                    break;
            }
        }

    }

    public partial class FetchArticlesState : IDialogState
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
            string result;
            switch (message.Text)
            {
                case "НЭБ Национальная электронная библиотека":
                    result = await GetRusNebArticles(user.ArticleSearchTerm);
                    await _botClient.SendDocument(message.Chat,
                        InputFile.FromStream(new MemoryStream(Encoding.UTF8.GetBytes(result)),
                            $"articles-rusneb {DateTime.Now:dd-MM-yyyy hh.mm}.txt"),
                        caption: "Найденные статьи");
                    break;
                case "Кодекс":
                    result = await GetCodexArticles(user.ArticleSearchTerm);
                    await _botClient.SendDocument(message.Chat,
                        InputFile.FromStream(new MemoryStream(Encoding.UTF8.GetBytes(result)),
                            $"articles-codex {DateTime.Now:dd-MM-yyyy hh.mm}.txt"),
                        caption: "Найденные статьи");
                    break;
                case "КиберЛенинка":
                    result = await GetCyberLeninkaArticles(user.ArticleSearchTerm);
                    await _botClient.SendDocument(message.Chat,
                        InputFile.FromStream(new MemoryStream(Encoding.UTF8.GetBytes(result)),
                            $"articles-cyberleninka {DateTime.Now:dd-MM-yyyy hh.mm}.txt"),
                        caption: "Найденные статьи");
                    break;
                case "Большая Российская Энциклопедия":
                    result = await GetBigEncArticles(user.ArticleSearchTerm);
                    await _botClient.SendDocument(message.Chat,
                        InputFile.FromStream(new MemoryStream(Encoding.UTF8.GetBytes(result)),
                            $"articles-bigenc {DateTime.Now:dd-MM-yyyy hh.mm}.txt"),
                        caption: "Найденные статьи");
                    break;
                case "Отмена":
                    await _userHelper.UpdateUserState(user, nameof(DefaultState));
                    await _menuHelper.SendMainMenu(message, user);
                    break;
                default:
                    await _botClient.SendMessage(message.Chat, "Выберите доступный вариант!");
                    break;
            }
        }

        private async Task<string> GetCyberLeninkaArticles(string query)
        {
            var client = _clientFactory.CreateClient("cyberleninka");

            var response = await client.PostAsJsonAsync("api/search",
                new CyberLeninkaRequest { From = 0, Size = 10, Mode = "articles", Query = query });
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadFromJsonAsync<CyberLeninkaResponse>();

            var articles = responseBody!.Articles.Select(r =>
                new CyberLeninkaTelegramResponse
                {
                    Annotation = CleanHtml(r.Annotation),
                    Name = CleanHtml(r.Name),
                    Link = $"https://cyberleninka.ru{r.Link}",
                    Journal = r.Journal,
                    JournalLink = $"https://cyberleninka.ru{r.JournalLink}",
                    AuthorsString = r.Authors == null ?  string.Empty : string.Join(", ", r.Authors),
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

            return sb.ToString();
        }

        private async Task<string> GetCodexArticles(string query)
        {
            var client = _clientFactory.CreateClient("codex");
            var sb = new StringBuilder();

            try
            {
                var response = await client.GetStringAsync($"?q={Uri.EscapeDataString(query)}");
                var htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(response);

                var articleNodes = htmlDocument.DocumentNode.SelectNodes("//li[contains(@class, 'document-list_i')]");


                if (articleNodes != null)
                {
                    foreach (var node in articleNodes)
                    {
                        var titleNode = node.SelectSingleNode(".//div[contains(@class, 'document-list_i_t')]//div");
                        var linkNode = node.SelectSingleNode(".//a[contains(@class, 'document-list_i_lk')]");

                        string title = CleanHtml(titleNode?.InnerText.Trim());
                        string link = linkNode?.GetAttributeValue("href", string.Empty);

                        if (!string.IsNullOrEmpty(link))
                        {
                            link = $"https://docs.cntd.ru{link}";
                        }

                        sb.AppendLine($"Название: {title}");
                        sb.AppendLine($"Ссылка: {link}");
                        sb.AppendLine();
                    }
                }
                else
                {
                    Console.WriteLine("No results found.");
                    return "Нет результатов";
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return "Возникла ошибка при запросе";
            }

            return sb.ToString();
        }

        private async Task<string> GetRusNebArticles(string query)
        {
            var client = _clientFactory.CreateClient("rusneb");
            var sb = new StringBuilder();

            try
            {
                var response = await client.GetAsync($"?q={Uri.EscapeDataString(query)}");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(content);

                var searchResults = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'search-list__item')]");
                HashSet<string> uniqueTitles = [];


                if (searchResults != null)
                {
                    foreach (var item in searchResults)
                    {
                        if (item is null) continue;

                        var titleNode = item.SelectSingleNode(".//div[contains(@class, 'info_5')]/a");
                        var title = titleNode?.InnerText.Trim();

                        if (string.IsNullOrEmpty(title)) continue;

                        title = RusNebRegex().Replace(title, " ");
                        if (!uniqueTitles.Add(title)) continue;

                        var link = titleNode?.GetAttributeValue("href", string.Empty);
                        sb.AppendLine($"Название: {title}");
                        sb.AppendLine($"Ссылка: {link}");
                        sb.AppendLine();
                    }
                }
                else
                {
                    return "Нет результатов";
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request error: {e.Message}");
                return "Возникла ошибка при запросе";
            }

            return sb.ToString();
        }

        private async Task<string> GetBigEncArticles(string query)
        {
            var client = _clientFactory.CreateClient("bigenc");
            var sb = new StringBuilder();

            try
            {
                var formattedQuery = query.Replace(" ", "+");
                var response = await client.GetAsync($"?q={formattedQuery}&limit=10");
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("list", out JsonElement list))
                {
                    foreach (var item in list.EnumerateArray())
                    {
                        string title = CleanHtml(item.GetProperty("title").GetString());
                        string slug = item.GetProperty("link").GetProperty("slug").GetString();
                        string fullLink = $"https://bigenc.ru/c/{slug}";

                        sb.AppendLine($"Название: {title}");
                        sb.AppendLine($"Ссылка: {fullLink}");
                        sb.AppendLine();
                    }
                }
                else
                {
                    return "Нет результатов";
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Ошибка запроса: {e.Message}");
                return "Возникла ошибка при запросе";
            }

            return sb.ToString();
        }



        [System.Text.RegularExpressions.GeneratedRegex(@"\s+")]
        private static partial System.Text.RegularExpressions.Regex RusNebRegex();
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
        public string[]? Authors { get; set; } = [];
        public int Year { get; set; }
        public string Journal { get; set; } = string.Empty;
        public string JournalLink { get; set; } = string.Empty;
    }
}