using DST.Bot.Database;
using DST.Bot.Features.Common;
using DST.Bot.Features.MainMenu;
using DST.Bot.Features.StateManager;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = DST.Bot.Entities.User;

namespace DST.Bot.Features.PsycholigicalTest;

public static class PsychologicalTest
{
    public class FirstQuestionState : IDialogState
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly AppDbContext _dbContext;
        private readonly UserHelper _userHelper;

        public FirstQuestionState(ITelegramBotClient telegramBotClient, AppDbContext dbContext, UserHelper userHelper)
        {
            _telegramBotClient = telegramBotClient;
            _dbContext = dbContext;
            _userHelper = userHelper;
        }
        
        public async Task Handle(Message message, User user)
        {
            var newQuestionAnswersMarkup = new ReplyKeyboardMarkup()
                .AddNewRow("Лично, лицом к лицу")
                .AddNewRow("По телефону")
                .AddNewRow("В мессенджерах или по электронной почте")
                .AddNewRow("Предпочитаю избегать общения");
            
            switch (message.Text)
            {
                case "Принимаю её с благодарностью и стараюсь улучшиться":
                    user.PsychologicalTestPoints += 30;
                    user.DialogState = nameof(SecondQuestionState);
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                    await _telegramBotClient.SendMessage(message.Chat, "Как вы предпочитаете общаться с людьми?", replyMarkup: newQuestionAnswersMarkup);
                    break;
                case "Чувствую себя обиженным, но стараюсь не показывать это":
                    user.PsychologicalTestPoints += 20;
                    user.DialogState = nameof(SecondQuestionState);
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                    await _telegramBotClient.SendMessage(message.Chat, "Как вы предпочитаете общаться с людьми?", replyMarkup: newQuestionAnswersMarkup);
                    break;
                case "Защищаюсь и объясняю свою точку зрения":
                    user.PsychologicalTestPoints += 10;
                    user.DialogState = nameof(SecondQuestionState);
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                    await _telegramBotClient.SendMessage(message.Chat, "Как вы предпочитаете общаться с людьми?", replyMarkup: newQuestionAnswersMarkup);
                    break;
                case "Игнорирую и продолжаю действовать по-своему":
                    await _userHelper.UpdateUserState(user,  nameof(SecondQuestionState));
                    await _telegramBotClient.SendMessage(message.Chat, "Как вы предпочитаете общаться с людьми?", replyMarkup: newQuestionAnswersMarkup);
                    break;
                default:
                    await _telegramBotClient.SendMessage(message.Chat, "Выберите доступный вариант");
                    break;
            }
        }

    }

    public class SecondQuestionState : IDialogState
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly AppDbContext _dbContext;
        private readonly UserHelper _userHelper;

        public SecondQuestionState(ITelegramBotClient telegramBotClient, AppDbContext dbContext, UserHelper userHelper)
        {
            _telegramBotClient = telegramBotClient;
            _dbContext = dbContext;
            _userHelper = userHelper;
        }
        
        public async Task Handle(Message message, User user)
        {
            var newQuestionAnswersMarkup = new ReplyKeyboardMarkup()
                .AddNewRow("С радостью знакомлюсь с новыми людьми")
                .AddNewRow("Знакомлюсь, но с осторожностью")
                .AddNewRow("Предпочитаю оставаться в кругу знакомых")
                .AddNewRow("Избегаю новых знакомств");

            switch (message.Text)
            {
                case "Лично, лицом к лицу":
                    user.PsychologicalTestPoints += 30;
                    user.DialogState = nameof(ThirdQuestionState);
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                    await _telegramBotClient.SendMessage(message.Chat, "Как вы относитесь к новым знакомствам?", replyMarkup: newQuestionAnswersMarkup);
                    break;
                case "По телефону":
                    user.PsychologicalTestPoints += 20;
                    user.DialogState = nameof(ThirdQuestionState);
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                    await _telegramBotClient.SendMessage(message.Chat, "Как вы относитесь к новым знакомствам?", replyMarkup: newQuestionAnswersMarkup);
                    break;
                case "В мессенджерах или по электронной почте":
                    user.PsychologicalTestPoints += 10;
                    user.DialogState = nameof(ThirdQuestionState);
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                    await _telegramBotClient.SendMessage(message.Chat, "Как вы относитесь к новым знакомствам?", replyMarkup: newQuestionAnswersMarkup);
                    break;
                case "Предпочитаю избегать общения":
                    await _userHelper.UpdateUserState(user, nameof(ThirdQuestionState));
                    await _telegramBotClient.SendMessage(message.Chat, "Как вы относитесь к новым знакомствам?", replyMarkup: newQuestionAnswersMarkup);
                    break;
                default:
                    await _telegramBotClient.SendMessage(message.Chat, "Выберите доступный вариант");
                    break;
            }
        }

    }

    public class ThirdQuestionState : IDialogState
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly AppDbContext _dbContext;
        private readonly UserHelper _userHelper;

        public ThirdQuestionState(ITelegramBotClient telegramBotClient, AppDbContext dbContext, UserHelper userHelper)
        {
            _telegramBotClient = telegramBotClient;
            _dbContext = dbContext;
            _userHelper = userHelper;
        }

        public async Task Handle(Message message, User user)
        {
            var newQuestionAnswersMarkup = new ReplyKeyboardMarkup()
                .AddNewRow("Быстро и интуитивно")
                .AddNewRow("Обдумываю все возможные варианты")
                .AddNewRow("Спрашиваю мнение других")
                .AddNewRow("Откладываю решение на потом");

            switch (message.Text)
            {
                case "С радостью знакомлюсь с новыми людьми":
                    user.PsychologicalTestPoints += 30;
                    user.DialogState = nameof(FourthQuestionState);
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                    await _telegramBotClient.SendMessage(message.Chat, "Как вы обычно принимаете решения?", replyMarkup: newQuestionAnswersMarkup);
                    break;
                case "Знакомлюсь, но с осторожностью ":
                    user.PsychologicalTestPoints += 20;
                    user.DialogState = nameof(FourthQuestionState);
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                    await _telegramBotClient.SendMessage(message.Chat, "Как вы обычно принимаете решения?", replyMarkup: newQuestionAnswersMarkup);
                    break;
                case "Предпочитаю оставаться в кругу знакомых":
                    user.PsychologicalTestPoints += 10;
                    user.DialogState = nameof(FourthQuestionState);
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                    await _telegramBotClient.SendMessage(message.Chat, "Как вы обычно принимаете решения?", replyMarkup: newQuestionAnswersMarkup);
                    break;
                case "Избегаю новых знакомств":
                    await _userHelper.UpdateUserState(user, nameof(FourthQuestionState));
                    await _telegramBotClient.SendMessage(message.Chat, "Как вы обычно принимаете решения?", replyMarkup: newQuestionAnswersMarkup);
                    break;
                default:
                    await _telegramBotClient.SendMessage(message.Chat, "Выберите доступный вариант");
                    break;
            }
        }

    }

    public class FourthQuestionState : IDialogState
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly AppDbContext _dbContext;
        private readonly UserHelper _userHelper;

        public FourthQuestionState(ITelegramBotClient telegramBotClient, AppDbContext dbContext, UserHelper userHelper)
        {
            _telegramBotClient = telegramBotClient;
            _dbContext = dbContext;
            _userHelper = userHelper;
        }
        
        public async Task Handle(Message message, User user)
        {
            var newQuestionAnswersMarkup = new ReplyKeyboardMarkup()
                .AddNewRow("С лёгкостью принимаю и адаптируюсь")
                .AddNewRow("Сначала испытываю стресс, но потом привыкаю")
                .AddNewRow("Сопротивляюсь изменениям")
                .AddNewRow("Избегаю изменений любой ценой");

            switch (message.Text)
            {
                case "Быстро и интуитивно":
                    user.PsychologicalTestPoints += 30;
                    user.DialogState = nameof(FifthQuestionState);
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                    await _telegramBotClient.SendMessage(message.Chat, "Как вы реагируете на изменения в жизни?", replyMarkup: newQuestionAnswersMarkup);
                    break;
                case "Обдумываю все возможные варианты":
                    user.PsychologicalTestPoints += 20;
                    user.DialogState = nameof(FifthQuestionState);
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                    await _telegramBotClient.SendMessage(message.Chat, "Как вы реагируете на изменения в жизни?", replyMarkup: newQuestionAnswersMarkup);
                    break;
                case "Спрашиваю мнение других":
                    user.PsychologicalTestPoints += 10;
                    user.DialogState = nameof(FifthQuestionState);
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                    await _telegramBotClient.SendMessage(message.Chat, "Как вы реагируете на изменения в жизни?", replyMarkup: newQuestionAnswersMarkup);
                    break;
                case "Откладываю решение на потом":
                    await _userHelper.UpdateUserState(user,  nameof(FifthQuestionState));
                    await _telegramBotClient.SendMessage(message.Chat, "Как вы реагируете на изменения в жизни?", replyMarkup: newQuestionAnswersMarkup);
                    break;
                default:
                    await _telegramBotClient.SendMessage(message.Chat, "Выберите доступный вариант");
                    break;
            }
        }

    }

    public class FifthQuestionState : IDialogState
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly AppDbContext _dbContext;
        private readonly UserHelper _userHelper;

        public FifthQuestionState(ITelegramBotClient telegramBotClient, AppDbContext dbContext, UserHelper userHelper)
        {
            _telegramBotClient = telegramBotClient;
            _dbContext = dbContext;
            _userHelper = userHelper;
        }
        
        public async Task Handle(Message message, User user)
        {
            var newQuestionAnswersMarkup = new ReplyKeyboardMarkup()
                .AddNewRow("В компании друзей")
                .AddNewRow("Наедине с собой")
                .AddNewRow("Занимаясь хобби")
                .AddNewRow("Работая или учась");

            switch (message.Text)
            {
                case "С лёгкостью принимаю и адаптируюсь":
                    user.PsychologicalTestPoints += 30;
                    user.DialogState = nameof(SixthQuestionState);
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                    await _telegramBotClient.SendMessage(message.Chat, "Как вы предпочитаете проводить свободное время?", replyMarkup: newQuestionAnswersMarkup);
                    break;
                case "Сначала испытываю стресс, но потом привыкаю":
                    user.PsychologicalTestPoints += 20;
                    user.DialogState = nameof(SixthQuestionState);
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                    await _telegramBotClient.SendMessage(message.Chat, "Как вы предпочитаете проводить свободное время?", replyMarkup: newQuestionAnswersMarkup);
                    break;
                case "Сопротивляюсь изменениям":
                    user.PsychologicalTestPoints += 10;
                    user.DialogState = nameof(SixthQuestionState);
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                    await _telegramBotClient.SendMessage(message.Chat, "Как вы предпочитаете проводить свободное время?", replyMarkup: newQuestionAnswersMarkup);
                    break;
                case "Избегаю изменений любой ценой":
                    await _userHelper.UpdateUserState(user, nameof(SixthQuestionState));
                    await _telegramBotClient.SendMessage(message.Chat, "Как вы предпочитаете проводить свободное время?", replyMarkup: newQuestionAnswersMarkup);
                    break;
                default:
                    await _telegramBotClient.SendMessage(message.Chat, "Выберите доступный вариант");
                    break;
            }
        }

    }

    public class SixthQuestionState : IDialogState
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly AppDbContext _dbContext;
        private readonly UserHelper _userHelper;

        public SixthQuestionState(ITelegramBotClient telegramBotClient, AppDbContext dbContext, UserHelper userHelper)
        {
            _telegramBotClient = telegramBotClient;
            _dbContext = dbContext;
            _userHelper = userHelper;
        }
        
        public async Task Handle(Message message, User user)
        {
            var newQuestionAnswersMarkup = new ReplyKeyboardMarkup()
                .AddNewRow("Открыто и честно")
                .AddNewRow("Сдержанно, но делаю это")
                .AddNewRow("С трудом выражаю свои чувства")
                .AddNewRow("Предпочитаю не показывать эмоции");

            switch (message.Text)
            {
             case "В компании друзей":
                 user.PsychologicalTestPoints += 30;
                 user.DialogState = nameof(SeventhQuestionState);
                 _dbContext.Update(user);
                 await _dbContext.SaveChangesAsync();
                 await _telegramBotClient.SendMessage(message.Chat, "Как вы обычно выражаете свои эмоции?", replyMarkup: newQuestionAnswersMarkup);
                 break;
             case "Наедине с собой":
                 user.PsychologicalTestPoints += 20;
                 user.DialogState = nameof(SeventhQuestionState);
                 _dbContext.Update(user);
                 await _dbContext.SaveChangesAsync();
                 await _telegramBotClient.SendMessage(message.Chat, "Как вы обычно выражаете свои эмоции?", replyMarkup: newQuestionAnswersMarkup);
                 break;
             case "Занимаясь хобби":
                 user.PsychologicalTestPoints += 10;
                 user.DialogState = nameof(SeventhQuestionState);
                 _dbContext.Update(user);
                 await _dbContext.SaveChangesAsync();
                 await _telegramBotClient.SendMessage(message.Chat, "Как вы обычно выражаете свои эмоции?", replyMarkup: newQuestionAnswersMarkup);
                 break;
             case "Работая или учась":
                 await _userHelper.UpdateUserState(user, nameof(SeventhQuestionState));
                 await _telegramBotClient.SendMessage(message.Chat, "Как вы обычно выражаете свои эмоции?", replyMarkup: newQuestionAnswersMarkup);
                 break;
                default:
                    await _telegramBotClient.SendMessage(message.Chat, "Выберите доступный вариант");
                    break;
            }
        }

    }
    
    public class SeventhQuestionState : IDialogState
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly AppDbContext _dbContext;
        private readonly UserHelper _userHelper;
        private readonly MenuHelper _menuHelper;

        public SeventhQuestionState(ITelegramBotClient telegramBotClient, AppDbContext dbContext, UserHelper userHelper, MenuHelper menuHelper)
        {
            _telegramBotClient = telegramBotClient;
            _dbContext = dbContext;
            _userHelper = userHelper;
            _menuHelper = menuHelper;
        }
        
        public async Task Handle(Message message, User user)
        {
            switch (message.Text)
            {
                case "Открыто и честно":
                    user.PsychologicalTestPoints += 30;
                    user.DialogState = nameof(DefaultState);
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                    await _menuHelper.SendMainMenu(message, user);
                    break;
                case "Сдержанно, но делаю это":
                    user.PsychologicalTestPoints += 20;
                    user.DialogState = nameof(DefaultState);
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                    await _menuHelper.SendMainMenu(message, user);
                    break;
                case "С трудом выражаю свои чувства":
                    user.PsychologicalTestPoints += 10;
                    user.DialogState = nameof(DefaultState);
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                    await _menuHelper.SendMainMenu(message, user);
                    break;
                case "Предпочитаю не показывать эмоции":
                    await _userHelper.UpdateUserState(user, nameof(DefaultState));
                    await _menuHelper.SendMainMenu(message, user);
                    break;
                default:
                    await _telegramBotClient.SendMessage(message.Chat, "Выберите доступный вариант");
                    break;
            }
        }

    }
}