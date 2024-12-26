using DST.Bot.Features.StateManager;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace DST.Bot.Features.SetupBot;

public static class SetupBot
{
    public static IServiceCollection AddTg(this IServiceCollection services, IConfigurationManager configurationManager)
    {
        services.Configure<TgBotOptions>(configurationManager.GetRequiredSection(TgBotOptions.TgBot));
        
        var botOptions = configurationManager
            .GetRequiredSection(TgBotOptions.TgBot)
            .Get<TgBotOptions>() ?? throw new Exception("Missing tg configuration");
        
        services.ConfigureTelegramBot<JsonOptions>(opts => opts.SerializerOptions);
        
        services
            .AddHttpClient("tgWebHook")
            .RemoveAllLoggers()
            .AddTypedClient<ITelegramBotClient>(client => new TelegramBotClient(botOptions.Token, client));

        services.AddScoped<HandleTgUpdate>();
        
        return services;
    }
    
    public static void UseWebHook(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/bot/setWebhook", async (ITelegramBotClient bot, IOptions<TgBotOptions> options) =>
        { 
            await bot.SetWebhook(options.Value.WebHookUrl);
            return $"Webhook set to {options.Value.WebHookUrl}";
        });

        endpoints.MapPost("/api/update", async (ITelegramBotClient bot, Update update, HandleTgUpdate handleTgUpdate, CancellationToken ct) =>
        {
            if (update.Message?.Text is null) return;	
            var msg = update.Message;
            Console.WriteLine($"Received message '{msg.Text}' in {msg.Chat}");
            await handleTgUpdate.HandleUpdateAsync(bot, update, ct);
        });
    }

    
    public class HandleTgUpdate : IUpdateHandler
    {
        private readonly ITelegramBotClient _botClient;
        private readonly DialogContext.DialogContextHandler _dialogContextHandler;

        public HandleTgUpdate(ITelegramBotClient botClient, DialogContext.DialogContextHandler dialogContextHandler)
        {
            _botClient = botClient;
            _dialogContextHandler = dialogContextHandler;
        }
        
        public async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await (update switch
            {
                { Message: { } message } => OnMessage(message),
                { EditedMessage: { } message } => OnMessage(message),
                { CallbackQuery: { } callbackQuery } => OnCallbackQuery(callbackQuery),
                _ => UnknownUpdateHandlerAsync(update)
            });
        }

        public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, HandleErrorSource source,
            CancellationToken cancellationToken)
        {
            if (exception is RequestException)
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
        }
        
        private async Task OnCallbackQuery(CallbackQuery callbackQuery)
        {
            await _botClient.SendMessage(callbackQuery.Message!.Chat.Id, "Some text");
        }

        private async Task UnknownUpdateHandlerAsync(Update update)
        {
            Console.WriteLine($"Unknown update type: {update.Type}");
            await _botClient.SendMessage(update.Message!.Chat.Id, "Unknown update type");
        }

        private async Task OnMessage(Message message)
        {
            if (message.Text is not { } messageText)
                return;

            if (message.Text.StartsWith('/'))
            {
                await _botClient.SendMessage(message.Chat.Id, "Здесь могут быть команды");
            }
            else
            {
                await _dialogContextHandler.Handle(message);
            }
        }
    }
    
    
    public class TgBotOptions
    {
        public const string TgBot = "TgBot";
        public required string Token { get; init; }
        public required string WebHookUrl { get; init; }
    }
}