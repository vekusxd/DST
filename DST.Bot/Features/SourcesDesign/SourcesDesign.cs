using System.Text;
using DST.Bot.Database;
using DST.Bot.Entities;
using DST.Bot.Features.Common;
using DST.Bot.Features.MainMenu;
using DST.Bot.Features.StateManager;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using User = DST.Bot.Entities.User;

namespace DST.Bot.Features.SourcesDesign;

public static class SourcesDesign
{
    public class SourcesDesignQuestionState : IDialogState
    {
        private readonly ITelegramBotClient _botClient;
        private readonly MenuHelper _menuHelper;
        private readonly UserHelper _userHelper;

        public SourcesDesignQuestionState(ITelegramBotClient botClient, MenuHelper menuHelper, UserHelper userHelper)
        {
            _botClient = botClient;
            _menuHelper = menuHelper;
            _userHelper = userHelper;
        }

        public async Task Handle(Message message, User user)
        {
            switch (message.Text)
            {
                case "Источник":
                    await _userHelper.UpdateUserState(user, nameof(SourcesDesignMenuState));
                    await _menuHelper.SendDesignSourceSelectorMenu(message, user);
                    break;
                case "Сноску":
                    await _botClient.SendMessage(message.Chat, "Здесь будет оформление сноски");
                    break;
                case "Отмена":
                    await _userHelper.UpdateUserState(user, nameof(DefaultState));
                    await _menuHelper.SendMainMenu(message, user);
                    break;
                default:
                    await _botClient.SendMessage(message.Chat, "Выберите доступный вариант");
                    break;
            }
        }
    }


    public class SourcesDesignMenuState : IDialogState
    {
        private readonly ITelegramBotClient _botClient;
        private readonly MenuHelper _menuHelper;
        private readonly UserHelper _userHelper;

        public SourcesDesignMenuState(ITelegramBotClient botClient, MenuHelper menuHelper, UserHelper userHelper)
        {
            _botClient = botClient;
            _menuHelper = menuHelper;
            _userHelper = userHelper;
        }

        public async Task Handle(Message message, User user)
        {
            switch (message.Text)
            {
                case "Книгу":
                    await _userHelper.UpdateUserState(user, nameof(SourcesGenerateBookDesignWaitAuthorLastName));
                    await _botClient.SendMessage(message.Chat, "Введите фамилию автора",
                        replyMarkup: new ReplyKeyboardMarkup("Отмена"));
                    break;
                case "Сайт":
                    await _botClient.SendMessage(message.Chat, "Здесь будет диалог для оформления сайта");
                    break;
                case "Статью с сайта":
                    await _botClient.SendMessage(message.Chat, "Здесь будет диалог для оформления статьи с сайта");
                    break;
                case "Отмена":
                    await _userHelper.UpdateUserState(user, nameof(SourcesDesignQuestionState));
                    await _menuHelper.SendSourceDesignMenu(message, user);
                    break;
            }
        }
    }

    public class SourcesGenerateBookDesignWaitAuthorLastName : IDialogState
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly AppDbContext _dbContext;
        private readonly UserHelper _userHelper;
        private readonly MenuHelper _menuHelper;

        public SourcesGenerateBookDesignWaitAuthorLastName(ITelegramBotClient telegramBotClient, AppDbContext dbContext,
            UserHelper userHelper, MenuHelper menuHelper)
        {
            _telegramBotClient = telegramBotClient;
            _dbContext = dbContext;
            _userHelper = userHelper;
            _menuHelper = menuHelper;
        }

        public async Task Handle(Message message, User user)
        {
            if (message.Text!.Equals("Отмена", StringComparison.InvariantCultureIgnoreCase))
            {
                await _userHelper.UpdateUserState(user, nameof(SourcesDesignMenuState));
                await _menuHelper.SendDesignSourceSelectorMenu(message, user);
            }
            else
            {
                user.DialogState = nameof(SourcesGenerateBookDesignWaitAuthorInitials);
                await _dbContext.Entry(user).Reference(u => u.BookDesignData).LoadAsync();
                user.BookDesignData = new BookDesignData { AuthorSurname = message.Text };
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                await _telegramBotClient.SendMessage(message.Chat, "Введите инициалы автора");
            }
        }
    }

    public class SourcesGenerateBookDesignWaitAuthorInitials : IDialogState
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly AppDbContext _dbContext;
        private readonly UserHelper _userHelper;
        private readonly MenuHelper _menuHelper;

        public SourcesGenerateBookDesignWaitAuthorInitials(ITelegramBotClient telegramBotClient, AppDbContext dbContext,
            UserHelper userHelper, MenuHelper menuHelper)
        {
            _telegramBotClient = telegramBotClient;
            _dbContext = dbContext;
            _userHelper = userHelper;
            _menuHelper = menuHelper;
        }


        public async Task Handle(Message message, User user)
        {
            if (message.Text!.Equals("Отмена", StringComparison.InvariantCultureIgnoreCase))
            {
                await _userHelper.UpdateUserState(user, nameof(SourcesDesignMenuState));
                await _menuHelper.SendDesignSourceSelectorMenu(message, user);
            }
            else
            {
                user.DialogState = nameof(SourcesGenerateBookDesignWaitBookTitle);
                await _dbContext.Entry(user).Reference(u => u.BookDesignData).LoadAsync();
                user.BookDesignData.AuthorInitials = message.Text;
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                await _telegramBotClient.SendMessage(message.Chat, "Введите название книги");
            }
        }
    }

    public class SourcesGenerateBookDesignWaitBookTitle : IDialogState
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly AppDbContext _dbContext;
        private readonly UserHelper _userHelper;
        private readonly MenuHelper _menuHelper;

        public SourcesGenerateBookDesignWaitBookTitle(ITelegramBotClient telegramBotClient, AppDbContext dbContext,
            UserHelper userHelper, MenuHelper menuHelper)
        {
            _telegramBotClient = telegramBotClient;
            _dbContext = dbContext;
            _userHelper = userHelper;
            _menuHelper = menuHelper;
        }


        public async Task Handle(Message message, User user)
        {
            if (message.Text!.Equals("Отмена", StringComparison.InvariantCultureIgnoreCase))
            {
                await _userHelper.UpdateUserState(user, nameof(SourcesDesignMenuState));
                await _menuHelper.SendDesignSourceSelectorMenu(message, user);
            }
            else
            {
                user.DialogState = nameof(SourcesGenerateBookDesignWaitPublicationPlace);
                await _dbContext.Entry(user).Reference(u => u.BookDesignData).LoadAsync();
                user.BookDesignData.BookTitle = message.Text;
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                await _telegramBotClient.SendMessage(message.Chat, "Введите место издания");
            }
        }
    }

    public class SourcesGenerateBookDesignWaitPublicationPlace : IDialogState
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly AppDbContext _dbContext;
        private readonly UserHelper _userHelper;
        private readonly MenuHelper _menuHelper;

        public SourcesGenerateBookDesignWaitPublicationPlace(ITelegramBotClient telegramBotClient,
            AppDbContext dbContext,
            UserHelper userHelper, MenuHelper menuHelper)
        {
            _telegramBotClient = telegramBotClient;
            _dbContext = dbContext;
            _userHelper = userHelper;
            _menuHelper = menuHelper;
        }

        public async Task Handle(Message message, User user)
        {
            if (message.Text!.Equals("Отмена", StringComparison.InvariantCultureIgnoreCase))
            {
                await _userHelper.UpdateUserState(user, nameof(SourcesDesignMenuState));
                await _menuHelper.SendDesignSourceSelectorMenu(message, user);
            }
            else
            {
                user.DialogState = nameof(SourcesGenerateBookDesignWaitPublisher);
                await _dbContext.Entry(user).Reference(u => u.BookDesignData).LoadAsync();
                user.BookDesignData.PublicationPlace = message.Text;
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                await _telegramBotClient.SendMessage(message.Chat, "Введите издательство");
            }
        }
    }

    public class SourcesGenerateBookDesignWaitPublisher : IDialogState
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly AppDbContext _dbContext;
        private readonly UserHelper _userHelper;
        private readonly MenuHelper _menuHelper;

        public SourcesGenerateBookDesignWaitPublisher(ITelegramBotClient telegramBotClient, AppDbContext dbContext,
            UserHelper userHelper, MenuHelper menuHelper)
        {
            _telegramBotClient = telegramBotClient;
            _dbContext = dbContext;
            _userHelper = userHelper;
            _menuHelper = menuHelper;
        }

        public async Task Handle(Message message, User user)
        {
            if (message.Text!.Equals("Отмена", StringComparison.InvariantCultureIgnoreCase))
            {
                await _userHelper.UpdateUserState(user, nameof(SourcesDesignMenuState));
                await _menuHelper.SendDesignSourceSelectorMenu(message, user);
            }
            else
            {
                user.DialogState = nameof(SourcesGenerateBookDesignWaitYearOfPublication);
                await _dbContext.Entry(user).Reference(u => u.BookDesignData).LoadAsync();
                user.BookDesignData.Publisher = message.Text;
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                await _telegramBotClient.SendMessage(message.Chat, "Введите год издания книги");
            }
        }
    }

    public class SourcesGenerateBookDesignWaitYearOfPublication : IDialogState
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly AppDbContext _dbContext;
        private readonly UserHelper _userHelper;
        private readonly MenuHelper _menuHelper;

        public SourcesGenerateBookDesignWaitYearOfPublication(ITelegramBotClient telegramBotClient,
            AppDbContext dbContext,
            UserHelper userHelper, MenuHelper menuHelper)
        {
            _telegramBotClient = telegramBotClient;
            _dbContext = dbContext;
            _userHelper = userHelper;
            _menuHelper = menuHelper;
        }

        public async Task Handle(Message message, User user)
        {
            if (message.Text!.Equals("Отмена", StringComparison.InvariantCultureIgnoreCase))
            {
                await _userHelper.UpdateUserState(user, nameof(SourcesDesignMenuState));
                await _menuHelper.SendDesignSourceSelectorMenu(message, user);
            }
            else
            {
                if (!int.TryParse(message.Text, out var yearOfPublication))
                {
                    await _telegramBotClient.SendMessage(message.Chat, "Введите число!");
                    return;
                }

                user.DialogState = nameof(SourcesGenerateBookDesignWaitNumberOfPages);
                await _dbContext.Entry(user).Reference(u => u.BookDesignData).LoadAsync();
                user.BookDesignData.YearOfPublication = yearOfPublication;
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                await _telegramBotClient.SendMessage(message.Chat, "Введите количество страниц");
            }
        }
    }

    public class SourcesGenerateBookDesignWaitNumberOfPages : IDialogState
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly AppDbContext _dbContext;
        private readonly UserHelper _userHelper;
        private readonly MenuHelper _menuHelper;

        public SourcesGenerateBookDesignWaitNumberOfPages(ITelegramBotClient telegramBotClient, AppDbContext dbContext,
            UserHelper userHelper, MenuHelper menuHelper)
        {
            _telegramBotClient = telegramBotClient;
            _dbContext = dbContext;
            _userHelper = userHelper;
            _menuHelper = menuHelper;
        }

        public async Task Handle(Message message, User user)
        {
            if (message.Text!.Equals("Отмена", StringComparison.InvariantCultureIgnoreCase))
            {
                await _userHelper.UpdateUserState(user, nameof(SourcesDesignMenuState));
                await _menuHelper.SendDesignSourceSelectorMenu(message, user);
            }
            else
            {
                if (!int.TryParse(message.Text, out var numberOfPages))
                {
                    await _telegramBotClient.SendMessage(message.Chat, "Введите число!");
                    return;
                }

                user.DialogState = nameof(SourcesGenerateBookDesignWaitPublicationDetails);
                await _dbContext.Entry(user).Reference(u => u.BookDesignData).LoadAsync();
                user.BookDesignData.NumberOfPages = numberOfPages;
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                await _telegramBotClient.SendMessage(message.Chat, "Введите сведения об издании (можно пропустить)",
                    replyMarkup: new ReplyKeyboardMarkup()
                        .AddNewRow("Пропустить")
                        .AddNewRow("Отмена"));
            }
        }
    }

    public class SourcesGenerateBookDesignWaitPublicationDetails : IDialogState
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly AppDbContext _dbContext;
        private readonly UserHelper _userHelper;
        private readonly MenuHelper _menuHelper;

        public SourcesGenerateBookDesignWaitPublicationDetails(ITelegramBotClient telegramBotClient,
            AppDbContext dbContext,
            UserHelper userHelper, MenuHelper menuHelper)
        {
            _telegramBotClient = telegramBotClient;
            _dbContext = dbContext;
            _userHelper = userHelper;
            _menuHelper = menuHelper;
        }

        public async Task Handle(Message message, User user)
        {
            var replyKeyboardMarkup = new ReplyKeyboardMarkup()
                .AddNewRow("Пропустить")
                .AddNewRow("Отмена");

            switch (message.Text)
            {
                case "Пропустить":
                    user.DialogState = nameof(SourcesGenerateBookDesignWaitIsbn);
                    await _dbContext.Entry(user).Reference(u => u.BookDesignData).LoadAsync();
                    user.BookDesignData.PublicationDetails = null;
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                    await _telegramBotClient.SendMessage(message.Chat, "Введите ISBN (можно пропустить)",
                        replyMarkup: replyKeyboardMarkup);
                    break;
                case "Отмена":
                    await _userHelper.UpdateUserState(user, nameof(SourcesDesignMenuState));
                    await _menuHelper.SendDesignSourceSelectorMenu(message, user);
                    break;
                default:
                    user.DialogState = nameof(SourcesGenerateBookDesignWaitIsbn);
                    await _dbContext.Entry(user).Reference(u => u.BookDesignData).LoadAsync();
                    user.BookDesignData.PublicationDetails = message.Text;
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                    await _telegramBotClient.SendMessage(message.Chat, "Введите ISBN (можно пропустить)",
                        replyMarkup: replyKeyboardMarkup);
                    break;
            }
        }
    }

    public class SourcesGenerateBookDesignWaitIsbn : IDialogState
    {
        private readonly ITelegramBotClient _telegramBotClient;
        private readonly AppDbContext _dbContext;
        private readonly UserHelper _userHelper;
        private readonly MenuHelper _menuHelper;

        public SourcesGenerateBookDesignWaitIsbn(ITelegramBotClient telegramBotClient, AppDbContext dbContext,
            UserHelper userHelper, MenuHelper menuHelper)
        {
            _telegramBotClient = telegramBotClient;
            _dbContext = dbContext;
            _userHelper = userHelper;
            _menuHelper = menuHelper;
        }

        public async Task Handle(Message message, User user)
        {
            string result;
            switch (message.Text)
            {
                case "Пропустить":
                    await _userHelper.UpdateUserState(user, nameof(DefaultState));
                    await _dbContext.Entry(user).Reference(u => u.BookDesignData).LoadAsync();
                    user.BookDesignData.Isbn = null;
                    result = GetBook(user.BookDesignData);
                    await _telegramBotClient.SendMessage(message.Chat, result, replyMarkup: MenuHelper.MenuMarkup);
                    break;
                case "Отмена":
                    await _userHelper.UpdateUserState(user, nameof(SourcesDesignMenuState));
                    await _menuHelper.SendSourceDesignMenu(message, user);
                    break;
                default:
                    user.DialogState = nameof(DefaultState);
                    await _dbContext.Entry(user).Reference(u => u.BookDesignData).LoadAsync();
                    user.BookDesignData.Isbn = message.Text;
                    _dbContext.Update(user);
                    await _dbContext.SaveChangesAsync();
                    result = GetBook(user.BookDesignData);
                    await _telegramBotClient.SendMessage(message.Chat, result, replyMarkup: MenuHelper.MenuMarkup);
                    break;
            }
        }

        private string GetBook(BookDesignData bookDesignData)
        {
            var sb = new StringBuilder();
            var b = bookDesignData;
            if (bookDesignData.PublicationDetails == null)
            {
                sb.AppendLine(
                    $"{b.AuthorSurname}, {b.AuthorInitials} {b.BookTitle} / {b.AuthorInitials} {b.AuthorSurname}.- {b.PublicationDetails}.- {b.PublicationPlace}:{b.Publisher},{b.YearOfPublication}.- {b.NumberOfPages} с.");
            }
            else
            {
                sb.AppendLine(
                    $"{b.AuthorSurname}, {b.AuthorInitials} {b.BookTitle} / {b.AuthorInitials} {b.AuthorSurname}.- {b.PublicationPlace}:{b.Publisher},{b.YearOfPublication}.- {b.NumberOfPages} с.");
            }
            
            if (bookDesignData.Isbn != null)
                sb.Append($"-ISBN {bookDesignData.Isbn}");
            
            return sb.ToString();
        }
    }
}