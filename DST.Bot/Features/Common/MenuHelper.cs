using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace DST.Bot.Features.Common;

public class MenuHelper
{
    private readonly ITelegramBotClient _botClient;

    public MenuHelper(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public Task SendMainMenu(Message message)
    {
        return Task.FromResult(_botClient.SendMessage(message.Chat.Id, "Здесь будет меню и какая-то важная информация",
            replyMarkup: new ReplyKeyboardMarkup().AddButton("Создать титульный лист").AddNewRow("Информация по введению в дипломной работе")));
    }
}