using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Web;
using DST.Bot.Features.Common;
using DST.Bot.Features.StateManager;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = DST.Bot.Entities.User;

namespace DST.Bot.Features.GigaChat;

public static class GigaChat
{
    public static IServiceCollection AddGigaChat(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.Configure<GigaChatOptions>(configuration.GetRequiredSection("GigaChat"));
        services.AddHttpClient<GigaChatQuestionState>("gigachat",
            opts =>
            {
                opts.DefaultRequestHeaders.Add("user_agent", "DST bot v1.0");
            });
        return services;
    }

    public class GigaChatQuestionState : IDialogState
    {
        private readonly ITelegramBotClient _botClient;
        private readonly UserHelper _userHelper;
        private readonly HttpClient _gigaChatClient;
        private readonly MenuHelper _menuHelper;

        private readonly GigaChatOptions _options;
        private const string AuthUrl = "https://ngw.devices.sberbank.ru:9443/api/v2/oauth";
        private const string BaseUrl = "https://gigachat.devices.sberbank.ru/api/v1/chat/completions";

        public GigaChatQuestionState(ITelegramBotClient botClient, IOptions<GigaChatOptions> options,
            UserHelper userHelper, HttpClient gigaChatClient, MenuHelper menuHelper)
        {
            _botClient = botClient;
            _userHelper = userHelper;
            _gigaChatClient = gigaChatClient;
            _menuHelper = menuHelper;

            _options = options.Value;
        }

        public async Task Handle(Message message, User user)
        {
            var replyMarkup = new ReplyKeyboardMarkup()
                .AddNewRow("НЭБ Национальная электронная библиотека")
                .AddNewRow("Кодекс")
                .AddNewRow("КиберЛенинка")
                .AddNewRow("Большая Российская Энциклопедия")
                .AddNewRow("Отмена");
            
            switch (message.Text)
            {

                case "Отмена":
                    await _userHelper.UpdateUserState(user, DialogStateId.DefaultState);
                    await _menuHelper.SendMainMenu(message, user);
                    break;
                default:
                    await _userHelper.UpdateUserState(user, DialogStateId.DefaultState);
                    var response = await GetResponse(message.Text!);
                    var sb = new StringBuilder();
                    foreach (var choice in response!.ChoicesResponseArray)
                    {
                        sb.AppendLine(choice.Message.Content);
                    }

                    var encodedPart = HttpUtility.UrlEncode(message.Text);
                    
                    _gigaChatClient.DefaultRequestHeaders.Add("User-Agent", "My dst bot user-aget");

                    var wikiResponse = await _gigaChatClient.GetAsync(new Uri(
                        $"https://ru.wikipedia.org/w/rest.php/v1/page/{encodedPart}"));

                    if (wikiResponse.IsSuccessStatusCode)
                    {
                        sb.AppendLine($"Ссылка на wiki: https://ru.wikipedia.org/wiki/{encodedPart}");
                        sb.AppendLine($"Ссылка на znanierussia: https://znanierussia.ru/articles/{encodedPart}");
                    }
                    else
                    {
                        Console.WriteLine(wikiResponse.ReasonPhrase);
                    }

                    await _botClient.SendMessage(message.Chat,
                        $"Здесь будет ответ от гига чата: {sb}",
                        replyMarkup: new ReplyKeyboardMarkup()
                            .AddNewRow("Создать титульный лист")
                            .AddNewRow("Информация по введению в дипломной работе")
                            .AddNewRow("Ответ на вопрос")
                            .AddNewRow("Поиск источников и литературы по теме")
                            .AddNewRow("Пройти заново психологический тест"));
                    break;
            }
        }

        private async Task<string> GetGigaChatJwtToken()
        {
            Dictionary<string, string> payload = new()
            {
                { "scope", "GIGACHAT_API_PERS" }
            };

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(AuthUrl),
                Method = HttpMethod.Post,
                Headers =
                {
                    { "X-Version", "1" },
                    { HttpRequestHeader.Authorization.ToString(), $"Basic {_options.AccessToken}" },
                    { HttpRequestHeader.ContentType.ToString(), "application/x-www-form-urlencoded" },
                    { HttpRequestHeader.Accept.ToString(), "application/json" },
                    { "RqUID", "e3be86f2-da10-4e3e-94f3-b8b508b6bca1" }
                },
                Content = new FormUrlEncodedContent(payload)
            };

            var response = await _gigaChatClient.SendAsync(request);
            var result = await response.Content.ReadFromJsonAsync<GigaChatAuthResponse>();
            return result?.AccessToken ?? throw new Exception("Failed to get JWT token");
        }

        private async Task<GigaChatResponse?> GetResponse(string word)
        {
            var rootObject = new TextRequestObject
            {
                UpdateInterval = 0,
                MessagessArray =
                [
                    new Messages
                    {
                        Role = "user",
                        Content =
                            $"Отвечай без вводных предложений, приветствий и подобных конструкций, пиши только то что я прошу. Во-первых напиши определение {word}, во-вторых создай список, первым номером списка будет первый пример предложения, всего список должен состоять из двух примеров предложений в которых обязательно будет {word} как часть этих примеров предложений.",
                    }
                ]
            };

            var response =
                await _gigaChatClient.PostAsJsonAsync(BaseUrl,
                    rootObject);

            if (response.StatusCode == HttpStatusCode.OK)
                return await response.Content.ReadFromJsonAsync<GigaChatResponse>();

            var token = await GetGigaChatJwtToken();
            _gigaChatClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            response = await _gigaChatClient.PostAsJsonAsync(
                "https://gigachat.devices.sberbank.ru/api/v1/chat/completions", rootObject);

            return await response.Content.ReadFromJsonAsync<GigaChatResponse>();
        }

        public DialogStateId DialogStateId { get; } = DialogStateId.GigaChatQuestionState;
    }

    public class GigaChatOptions
    {
        public required string AccessToken { get; init; }
    }

    private class GigaChatAuthResponse
    {
        [JsonPropertyName("access_token")] public string AccessToken { get; set; } = null!;
        [JsonPropertyName("expires_at")] public long ExpiresAt { get; set; }
    }

    public class TextRequestObject
    {
        [JsonPropertyName("model")] public string Model { get; init; } = "GigaChat";
        [JsonPropertyName("stream")] public bool Stream { get; init; } = false;
        [JsonPropertyName("update_interval")] public int UpdateInterval { get; init; } = 0;
        [JsonPropertyName("messages")] public Messages[] MessagessArray { get; init; } = [];
    }

    public class Messages
    {
        [JsonPropertyName("role")] public string Role { get; set; } = null!;
        [JsonPropertyName("content")] public string Content { get; set; } = null!;
    }

    public class GigaChatResponse
    {
        [JsonPropertyName("choices")] public Choices[] ChoicesResponseArray { get; set; } = [];
    }

    public class Choices
    {
        [JsonPropertyName("message")] public ChoiceMessage Message { get; set; } = null!;
        [JsonPropertyName("index")] public int Index { get; set; }

        [JsonPropertyName("finish_reason")] public string FinishReason { get; set; } = null!;
    }

    public class ChoiceMessage
    {
        [JsonPropertyName("content")] public string Content { get; set; } = null!;
        [JsonPropertyName("role")] public string Role { get; set; } = null!;
    }
}