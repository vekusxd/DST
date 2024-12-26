using System.Net;
using System.Text.Json.Serialization;
using DST.Bot.Features.Common;
using DST.Bot.Features.StateManager;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using WikiDotNet;
using User = DST.Bot.Entities.User;

namespace DST.Bot.Features.GigaChat;

public static class GigaChat
{
    public static IServiceCollection AddGigaChat(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.Configure<GigaChatOptions>(configuration.GetRequiredSection("GigaChat"));

        services.AddHttpClient<WikiSearcher>();

        services.AddHttpClient<GigaChatQuestionState>("gigachat");

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
            switch (message.Text)
            {
                case "Отмена":
                    await _userHelper.UpdateUserState(user, DialogStateId.DefaultState);
                    await _menuHelper.SendMainMenu(message, user);
                    break;
                default:
                    await _userHelper.UpdateUserState(user, DialogStateId.DefaultState);
                    await _botClient.SendMessage(message.Chat,
                        $"Здесь будет ответ от гига чата: {await GetResponse()}", replyMarkup: new ReplyKeyboardMarkup()
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

        private async Task<string> GetResponse(string request = "default")
        {
            var rootObject = new TextRequestObject
            {
                model = "GigaChat",
                stream = false,
                update_interval = 0,
                messages =
                [
                    new Messages { role = "system", content = "Отвечай как научный сотрудник" },
                    new Messages { role = "user", content = "Напиши 5 вариантов названий для космической станции" }
                ]
            };

            var response =
                await _gigaChatClient.PostAsJsonAsync(BaseUrl,
                    rootObject);

            if (response.StatusCode == HttpStatusCode.OK)
                return await response.Content.ReadAsStringAsync();

            var token = await GetGigaChatJwtToken();
            _gigaChatClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
            response = await _gigaChatClient.PostAsJsonAsync(
                "https://gigachat.devices.sberbank.ru/api/v1/chat/completions", rootObject);

            return await response.Content.ReadAsStringAsync();
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
        public string model { get; set; }
        public bool stream { get; set; }
        public int update_interval { get; set; }
        public Messages[] messages { get; set; }
    }

    public class Messages
    {
        public string role { get; set; }
        public string content { get; set; }
    }
}