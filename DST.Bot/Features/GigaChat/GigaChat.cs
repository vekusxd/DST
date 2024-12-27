﻿using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Web;
using DST.Bot.Features.Common;
using DST.Bot.Features.StateManager;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = DST.Bot.Entities.User;

namespace DST.Bot.Features.GigaChat;

public static class GigaChat
{
    public static IServiceCollection AddGigaChat(this IServiceCollection services, IConfigurationManager configuration)
    {
        services.Configure<GigaChatOptions>(configuration.GetRequiredSection("GigaChat"));
        services.AddHttpClient("wikipedia", opts =>
        {
            opts.DefaultRequestHeaders.UserAgent.ParseAdd("Emil/1.0 (batmanovemil@gmail.com) bot");
            opts.BaseAddress = new Uri("https://ru.wikipedia.org/w/rest.php/v1/page/");
        });
        services.AddHttpClient("gigachat",
            opts => opts.BaseAddress = new Uri("https://gigachat.devices.sberbank.ru/api/v1/"));
        services.AddHttpClient("gigachat-auth");

        services.AddScoped<GigaChatFetcher>();
        return services;
    }

    public class GigaChatQuestionState : IDialogState
    {
        private readonly ITelegramBotClient _botClient;
        private readonly UserHelper _userHelper;
        private readonly MenuHelper _menuHelper;

        public GigaChatQuestionState(ITelegramBotClient botClient, UserHelper userHelper, MenuHelper menuHelper)
        {
            _botClient = botClient;
            _userHelper = userHelper;
            _menuHelper = menuHelper;
        }

        public async Task Handle(Message message, User user)
        {
            switch (message.Text)
            {
                case "Подскажи определение термина":
                    await _userHelper.UpdateUserState(user, DialogStateId.WaitForDefinitionQueryState);
                    await _botClient.SendMessage(message.Chat, "Введите термин, о котором хотите узнать информацию",
                        replyMarkup: new ReplyKeyboardMarkup("Отмена"));
                    break;
                case "Придумай темы для курсовой":
                    await _botClient.SendMessage(message.Chat, "В процессе реализации");
                    break;
                case "Помоги с параметрами темы":
                    await _botClient.SendMessage(message.Chat, "В процессе реализации");
                    break;
                case "Помоги с темами для курсовой":
                    await _botClient.SendMessage(message.Chat, "В процессе реализации");
                    break;
                case "Помоги со структурой работы":
                    await _botClient.SendMessage(message.Chat, "В процессе реализации");
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

        public DialogStateId DialogStateId { get; } = DialogStateId.GigaChatQuestionState;
    }

    public class WaitForDefinitionQueryState : IDialogState
    {
        private readonly ITelegramBotClient _botClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserHelper _helper;
        private readonly MenuHelper _menuHelper;
        private readonly GigaChatFetcher _gigaChatFetcher;

        public WaitForDefinitionQueryState(ITelegramBotClient botClient, IHttpClientFactory httpClientFactory,
            UserHelper helper, MenuHelper menuHelper, GigaChatFetcher gigaChatFetcher)
        {
            _botClient = botClient;
            _httpClientFactory = httpClientFactory;
            _helper = helper;
            _menuHelper = menuHelper;
            _gigaChatFetcher = gigaChatFetcher;
        }

        public async Task Handle(Message message, User user)
        {
            switch (message.Text)
            {
                case "Отмена":
                    await _helper.UpdateUserState(user, DialogStateId.GigaChatQuestionState);
                    await _menuHelper.SendMainMenu(message, user);
                    break;
                default:
                    var response = await _gigaChatFetcher.GetDefinitionResponse(message.Text!);
                    await _helper.UpdateUserState(user, DialogStateId.GigaChatQuestionState);
                    await _botClient.SendMessage(message.Chat, response, replyMarkup: MenuHelper.GigaChatMenuMarkup);
                    break;
            }
        }

        public DialogStateId DialogStateId { get; } = DialogStateId.WaitForDefinitionQueryState;
    }

    public class GigaChatFetcher
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IOptions<GigaChatOptions> _options;

        public GigaChatFetcher(IHttpClientFactory clientFactory, IOptions<GigaChatOptions> options)
        {
            _clientFactory = clientFactory;
            _options = options;
        }

        public async Task<string> GetDefinitionResponse(string word)
        {
            var client = _clientFactory.CreateClient("gigachat");

            var sb = new StringBuilder();

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

            var response = await client.PostAsJsonAsync("chat/completions",
                rootObject);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                var token = await GetGigaChatJwtToken();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                response = await client.PostAsJsonAsync("chat/completions", rootObject);
            }

            var responseObject = await response.Content.ReadFromJsonAsync<GigaChatResponse>();

            foreach (var choice in responseObject!.ChoicesResponseArray)
            {
                sb.AppendLine(choice.Message.Content);
            }

            var encodedPart = HttpUtility.UrlEncode(word);

            var wikiClient = _clientFactory.CreateClient("wikipedia");

            var wikiResponse = await wikiClient.GetAsync(encodedPart);

            if (!wikiResponse.IsSuccessStatusCode) return sb.ToString();
            
            sb.AppendLine($"Ссылка на wiki: https://ru.wikipedia.org/wiki/{encodedPart}");
            sb.AppendLine($"Ссылка на znanierussia: https://znanierussia.ru/articles/{encodedPart}");
            return sb.ToString();
        }

        private async Task<string> GetGigaChatJwtToken()
        {
            var authClient = _clientFactory.CreateClient("gigachat-auth");

            Dictionary<string, string> payload = new()
            {
                { "scope", "GIGACHAT_API_PERS" }
            };

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri("https://ngw.devices.sberbank.ru:9443/api/v2/oauth"),
                Method = HttpMethod.Post,
                Headers =
                {
                    { "X-Version", "1" },
                    { HttpRequestHeader.Authorization.ToString(), $"Basic {_options.Value.AccessToken}" },
                    { HttpRequestHeader.ContentType.ToString(), "application/x-www-form-urlencoded" },
                    { HttpRequestHeader.Accept.ToString(), "application/json" },
                    { "RqUID", "e3be86f2-da10-4e3e-94f3-b8b508b6bca1" }
                },
                Content = new FormUrlEncodedContent(payload)
            };

            var response = await authClient.SendAsync(request);
            var result = await response.Content.ReadFromJsonAsync<GigaChatAuthResponse>();
            return result?.AccessToken ?? throw new Exception("Failed to get JWT token");
        }
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