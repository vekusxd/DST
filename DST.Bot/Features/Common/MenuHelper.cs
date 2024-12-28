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
        .AddNewRow()
        .AddButton("Создать титульный лист").AddButton("Информация по введению в дипломной работе")
        .AddNewRow()
        .AddButton("Помощь с работой").AddButton("Поиск источников и литературы по теме")
        .AddNewRow()
        .AddButton("Оформление источников и сносок").AddButton("Пройти заново психологический тест");

    public static ReplyKeyboardMarkup GigaChatMenuMarkup => new ReplyKeyboardMarkup()
        .AddNewRow()
        .AddButton("Подскажи определение термина").AddButton("Придумай похожие темы для курсовой")
        .AddNewRow()
        .AddButton("Помоги с параметрами темы").AddButton("Придумай темы для курсовой")
        .AddNewRow()
        .AddButton("Помоги со структурой работы").AddNewRow("Отмена");

    public static ReplyKeyboardMarkup DesignMenuMarkup => new ReplyKeyboardMarkup()
        .AddNewRow("Источник")
        .AddNewRow("Сноску")
        .AddNewRow("Отмена");

    public static ReplyKeyboardMarkup DesignSourceSelectorMarkup => new ReplyKeyboardMarkup()
        .AddNewRow("Книгу")
        .AddNewRow("Сайт")
        .AddNewRow("Статью с сайта")
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

    public Task SendSourceDesignMenu(Message message, User user)
    {
        return Task.FromResult(_botClient.SendMessage(message.Chat, "Что вы хотите оформить?",
            replyMarkup: DesignMenuMarkup));
    }

    
    public Task SendDesignSourceSelectorMenu(Message message, User user)
    {
        return Task.FromResult(_botClient.SendMessage(message.Chat, "Что вам нужно оформить?",
            replyMarkup: DesignSourceSelectorMarkup));
    }
}