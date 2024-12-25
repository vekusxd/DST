using DST.Bot.Database;
using DST.Bot.Features.StateManager;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using User = DST.Bot.Entities.User;

namespace DST.Bot.Features.MainMenu;

public class DefaultState : IDialogState
{
    private readonly ITelegramBotClient _botClient;
    private readonly AppDbContext _dbContext;

    public DefaultState(ITelegramBotClient botClient, AppDbContext dbContext)
    {
        _botClient = botClient;
        _dbContext = dbContext;
    }

    public async Task Handle(Message message, User user)
    {
        switch (message.Text)
        {
            case "Создать титульный лист":
                user.DialogStateId = DialogStateId.FrontPageWaitInitials;
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                await _botClient.SendMessage(message.Chat.Id, "Введите ваши инициалы", replyMarkup:new ReplyKeyboardMarkup().AddButton("Отмена"));
                break;
            case "Информация по введению в дипломной работе":
                await _botClient.SendMessage(message.Chat, """
                            ```
                            Введение:
                            Это важная часть, которая задает общий тон всей работы. Оно включает в себя несколько ключевых элементов, таких как актуальность, постановка проблемы, цели и задачи исследования, а также объект и предмет исследования.
                                                            
                            Подзадачи для написания введения:
                                Определение актуальности темы:
                                Провести анализ существующих исследований по выбранной теме.
                                Выявить недостатки, пробелы или нерешенные вопросы в текущих исследованиях.
                                Обосновать, почему данная тема важна для изучения в данный момент.
                                Формулировка проблемы:
                                Четко определить, какая именно проблема будет рассматриваться в работе.
                                Сформулировать проблему в виде вопросов или гипотез, которые будут исследоваться.
                                Обосновать, почему эта проблема требует внимания и исследования.
                                Цели и задачи исследования:
                                Определить основную цель работы (например, анализ, разработка, улучшение).
                                Сформулировать конкретные задачи, которые необходимо решить для достижения цели (например, сбор данных, анализ литературы, проведение эксперимента).
                                Методы исследования:
                                Кратко описать методы, которые будут использованы для решения поставленных задач (например, количественные и качественные методы, анализ данных, эксперименты).
                                Обосновать выбор методов и их соответствие целям исследования.
                                Структура работы:
                                Кратко описать, как будет организована работа, какие разделы будут включены и что будет рассмотрено в каждом из них.
                                Указать, как каждый раздел будет способствовать достижению общей цели исследования
                                ```
                            """
                                                           , ParseMode.MarkdownV2);
                                                           
                break;
            default:
                await _botClient.SendMessage(message.Chat.Id, "Здесь будет меню и сообщение о приветствии",
                    replyMarkup: new ReplyKeyboardMarkup().AddButton("Создать титульный лист").AddNewRow("Информация по введению в дипломной работе"));
                break;
        }
    }

    public DialogStateId DialogStateId { get; } = DialogStateId.DefaultState;
}