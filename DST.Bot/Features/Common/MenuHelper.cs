using DST.Bot.Features.CommunicationStyleFactories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = DST.Bot.Entities.User;

namespace DST.Bot.Features.Common;

public class MenuHelper
{
    private readonly ITelegramBotClient _botClient;

    public MenuHelper(ITelegramBotClient botClient)
    {
        _botClient = botClient;
    }

    public static ReplyKeyboardMarkup MenuMarkup => new ReplyKeyboardMarkup()
        .AddNewRow("Создать титульный лист")
        .AddNewRow("Информация по введению в дипломной работе")
        .AddNewRow("Гига чат")
        .AddNewRow("Поиск источников и литературы по теме")
        .AddNewRow("Пройти заново психологический тест");

    public static ReplyKeyboardMarkup GigaChatMenuMarkup => new ReplyKeyboardMarkup()
        .AddNewRow("Подскажи определение термина")
        .AddNewRow("Придумай похожие темы для курсовой")
        .AddNewRow("Помоги с параметрами темы")
        .AddNewRow("Придумай темы для курсовой")
        .AddNewRow("Помоги со структурой работы")
        .AddNewRow("Отмена");

    public Task SendMainMenu(Message message, User user)
    {
        var dialogFactory = ICommunicationStyleFactory.CreateFactory(user.PsychologicalTestPoints);
        return Task.FromResult(_botClient.SendMessage(message.Chat.Id,
            $"Здесь будет меню и сообщение о приветствии. {dialogFactory.GetMainMenuMessage()}. Модель общения: {dialogFactory}(будет отображаться только во время разработки)",
            replyMarkup: MenuMarkup));
    }

    public Task SendGigaChatMenu(Message message, User user)
    {
        return Task.FromResult(_botClient.SendMessage(message.Chat.Id,
            "Чего вы хотите?",
            replyMarkup: GigaChatMenuMarkup));
    }
}