﻿using System.Globalization;
using DST.Bot.Database;
using DST.Bot.Entities;
using DST.Bot.Features.Common;
using DST.Bot.Features.MainMenu;
using DST.Bot.Features.StateManager;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = DST.Bot.Entities.User;

namespace DST.Bot.Features.GenerateFrontPage;

/// <summary>
///  Бог поможет..
/// </summary>
public static class GenerateFrontPage
{
    public class WaitInitialsState : IDialogState
    {
        private readonly AppDbContext _dbContext;
        private readonly ITelegramBotClient _botClient;
        private readonly UserHelper _userHelper;
        private readonly MenuHelper _menuHelper;

        public WaitInitialsState(AppDbContext dbContext, ITelegramBotClient botClient, UserHelper userHelper,
            MenuHelper menuHelper)
        {
            _dbContext = dbContext;
            _botClient = botClient;
            _userHelper = userHelper;
            _menuHelper = menuHelper;
        }

        public async Task Handle(Message message, User user)
        {
            if (message.Text!.Equals("Отмена", StringComparison.InvariantCultureIgnoreCase))
            {
                await _userHelper.UpdateUserState(user, nameof(DefaultState));
                await _menuHelper.SendMainMenu(message, user);
            }
            else
            {
                user.DialogState = nameof(WaitCourseState);
                await _dbContext.Entry(user).Reference(u => u.FrontPageData).LoadAsync();
                user.FrontPageData = new FrontPageData { Initials = message.Text, Course = null };
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                await _botClient.SendMessage(message.Chat, "Введите ваш курс");
            }
        }
    }

    public class WaitCourseState : IDialogState
    {
        private readonly ITelegramBotClient _botClient;
        private readonly AppDbContext _dbContext;
        private readonly MenuHelper _menuHelper;
        private readonly UserHelper _userHelper;

        public WaitCourseState(ITelegramBotClient botClient, AppDbContext dbContext, MenuHelper menuHelper,
            UserHelper userHelper)
        {
            _botClient = botClient;
            _dbContext = dbContext;
            _menuHelper = menuHelper;
            _userHelper = userHelper;
        }

        public async Task Handle(Message message, User user)
        {
            if (message.Text!.Equals("Отмена", StringComparison.InvariantCultureIgnoreCase))
            {
                await _userHelper.UpdateUserState(user, nameof(DefaultState));
                await _menuHelper.SendMainMenu(message, user);
            }
            else
            {
                user.DialogState = nameof(WaitProfileState);
                await _dbContext.Entry(user).Reference(u => u.FrontPageData).LoadAsync();
                user.FrontPageData.Course = message.Text;
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                await _botClient.SendMessage(message.Chat, "Введите ваш профиль");
            }
        }
    }

    public class WaitProfileState : IDialogState
    {
        private readonly AppDbContext _dbContext;
        private readonly ITelegramBotClient _botClient;
        private readonly MenuHelper _menuHelper;
        private readonly UserHelper _userHelper;

        public WaitProfileState(AppDbContext dbContext, ITelegramBotClient botClient, MenuHelper menuHelper
        ,UserHelper userHelper)
        {
            _dbContext = dbContext;
            _botClient = botClient;
            _menuHelper = menuHelper;
            _userHelper = userHelper;
        }

        public async Task Handle(Message message, User user)
        {
            if (message.Text!.Equals("Отмена", StringComparison.InvariantCultureIgnoreCase))
            {
                await _userHelper.UpdateUserState(user, nameof(DefaultState));
                await _menuHelper.SendMainMenu(message, user);
            }
            else
            {
                user.DialogState = nameof(WaitThemeState);
                await _dbContext.Entry(user).Reference(u => u.FrontPageData).LoadAsync();
                user.FrontPageData.Profile = message.Text;
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                await _botClient.SendMessage(message.Chat, "Введите тему вашей работы");
            }
        }
    }

    public class WaitThemeState : IDialogState
    {
        private readonly AppDbContext _dbContext;
        private readonly ITelegramBotClient _botClient;
        private readonly MenuHelper _menuHelper;
        private readonly UserHelper _userHelper;

        public WaitThemeState(AppDbContext dbContext, ITelegramBotClient botClient, MenuHelper menuHelper,
            UserHelper userHelper)
        {
            _dbContext = dbContext;
            _botClient = botClient;
            _menuHelper = menuHelper;
            _userHelper = userHelper;
        }

        public async Task Handle(Message message, User user)
        {
            if (message.Text!.Equals("Отмена", StringComparison.InvariantCultureIgnoreCase))
            {
                await _userHelper.UpdateUserState(user, nameof(DefaultState));
                await _menuHelper.SendMainMenu(message, user);
            }
            else
            {
                user.DialogState = nameof(WaitGroupState);
                await _dbContext.Entry(user).Reference(u => u.FrontPageData).LoadAsync();
                user.FrontPageData.Theme = message.Text;
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                await _botClient.SendMessage(message.Chat, "Введите номер вашей группы");
            }
        }
    }

    public class WaitGroupState : IDialogState
    {
        private readonly AppDbContext _dbContext;
        private readonly ITelegramBotClient _botClient;
        private readonly MenuHelper _menuHelper;
        private readonly UserHelper _userHelper;

        public WaitGroupState(AppDbContext dbContext, ITelegramBotClient botClient, MenuHelper menuHelper,
            UserHelper userHelper)
        {
            _dbContext = dbContext;
            _botClient = botClient;
            _menuHelper = menuHelper;
            _userHelper = userHelper;
        }

        public async Task Handle(Message message, User user)
        {
            if (message.Text!.Equals("Отмена", StringComparison.InvariantCultureIgnoreCase))
            {
                await _userHelper.UpdateUserState(user, nameof(DefaultState));
                await _menuHelper.SendMainMenu(message, user);
            }
            else
            {
                user.DialogState = nameof(WaitSupervisorInitialsState);
                await _dbContext.Entry(user).Reference(u => u.FrontPageData).LoadAsync();
                user.FrontPageData.Group = message.Text;
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                await _botClient.SendMessage(message.Chat, "Введите инициалы вашего научного руководителя");
            }
        }
    }

    public class WaitSupervisorInitialsState : IDialogState
    {
        private readonly AppDbContext _dbContext;
        private readonly ITelegramBotClient _botClient;
        private readonly MenuHelper _menuHelper;
        private readonly UserHelper _userHelper;

        public WaitSupervisorInitialsState(AppDbContext dbContext, ITelegramBotClient botClient, MenuHelper menuHelper,
            UserHelper userHelper)
        {
            _dbContext = dbContext;
            _botClient = botClient;
            _menuHelper = menuHelper;
            _userHelper = userHelper;
        }

        public async Task Handle(Message message, User user)
        {
            if (message.Text!.Equals("Отмена", StringComparison.InvariantCultureIgnoreCase))
            {
                await _userHelper.UpdateUserState(user, nameof(DefaultState));
                await _menuHelper.SendMainMenu(message, user);
            }
            else
            {
                user.DialogState = nameof(WaitSupervisorAcademicTitleState);
                await _dbContext.Entry(user).Reference(u => u.FrontPageData).LoadAsync();
                user.FrontPageData.SupervisorInitials = message.Text;
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                await _botClient.SendMessage(message.Chat, "Введите ученое звание вашего научного руководителя");
            }
        }
    }

    public class WaitSupervisorAcademicTitleState : IDialogState
    {
        private readonly AppDbContext _dbContext;
        private readonly ITelegramBotClient _botClient;
        private readonly MenuHelper _menuHelper;
        private readonly UserHelper _userHelper;

        public WaitSupervisorAcademicTitleState(AppDbContext dbContext, ITelegramBotClient botClient,
            MenuHelper menuHelper, UserHelper userHelper)
        {
            _dbContext = dbContext;
            _botClient = botClient;
            _menuHelper = menuHelper;
            _userHelper = userHelper;
        }

        public async Task Handle(Message message, User user)
        {
            if (message.Text!.Equals("Отмена", StringComparison.InvariantCultureIgnoreCase))
            {
                await _userHelper.UpdateUserState(user, nameof(DefaultState));
                await _menuHelper.SendMainMenu(message, user);
            }
            else
            {
                user.DialogState = nameof(WaitSupervisorAcademicDegreeState);
                await _dbContext.Entry(user).Reference(u => u.FrontPageData).LoadAsync();
                user.FrontPageData.SupervisorAcademicTitle = message.Text;
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                await _botClient.SendMessage(message.Chat, "Введите ученую степень вашего научного руководителя");
            }
        }
    }

    public class WaitSupervisorAcademicDegreeState : IDialogState
    {
        private readonly AppDbContext _dbContext;
        private readonly ITelegramBotClient _botClient;
        private readonly MenuHelper _menuHelper;
        private readonly UserHelper _userHelper;

        public WaitSupervisorAcademicDegreeState(AppDbContext dbContext, ITelegramBotClient botClient,
            MenuHelper menuHelper, UserHelper userHelper)
        {
            _dbContext = dbContext;
            _botClient = botClient;
            _menuHelper = menuHelper;
            _userHelper = userHelper;
        }

        public async Task Handle(Message message, User user)
        {
            if (message.Text!.Equals("Отмена", StringComparison.InvariantCultureIgnoreCase))
            {
                await _userHelper.UpdateUserState(user, nameof(DefaultState));
                await _menuHelper.SendMainMenu(message, user);
            }
            else
            {
                user.DialogState = nameof(WaitSupervisorJobTitleState);
                await _dbContext.Entry(user).Reference(u => u.FrontPageData).LoadAsync();
                user.FrontPageData.SupervisorAcademicDegree = message.Text;
                _dbContext.Update(user);
                await _dbContext.SaveChangesAsync();
                await _botClient.SendMessage(message.Chat, "Введите должность вашего научного руководителя");
            }
        }
    }

    public class WaitSupervisorJobTitleState : IDialogState
    {
        private readonly AppDbContext _dbContext;
        private readonly ITelegramBotClient _botClient;
        private readonly MenuHelper _menuHelper;

        public WaitSupervisorJobTitleState(AppDbContext dbContext, ITelegramBotClient botClient, MenuHelper menuHelper)
        {
            _dbContext = dbContext;
            _botClient = botClient;
            _menuHelper = menuHelper;
        }

        public async Task Handle(Message message, User user)
        {
            user.DialogState = nameof(DefaultState);
            await _dbContext.Entry(user).Reference(u => u.FrontPageData).LoadAsync();
            user.FrontPageData.SupervisorJobTitle = message.Text;
            _dbContext.Update(user);
            await _dbContext.SaveChangesAsync();
            await _botClient.SendDocument(message.Chat,
                new InputFileStream(GeneratePdf(user.FrontPageData),
                    $"титульный_лист {DateTime.Now:dd-MM-yyyy hh.mm}.pdf"),
                caption: "Ваш документ, преобразовать в DOCX можно здесь: https://pdf2docx.com/",
                replyMarkup: MenuHelper.MenuMarkup);
        }
    }

    private static MemoryStream GeneratePdf(FrontPageData frontPageData)
    {
        var culture = CultureInfo.CreateSpecificCulture("ru-RU");
        var doc = QuestPDF.Fluent.Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.MarginLeft(2, Unit.Centimetre);
                page.MarginRight(2, Unit.Centimetre);
                page.MarginTop(2, Unit.Centimetre);

                page.DefaultTextStyle(TextStyle.Default.LineHeight(1.15f));

                page.Header().Column(column =>
                {
                    column.Spacing(10);


                    var firstColumnItem = column.Item();
                    var headerDescriptor = firstColumnItem.Text(
                        "Министерство науки и высшего образования Российской Федерации Федеральное государственное автономное образовательное учреждение высшего образования");
                    headerDescriptor.Bold();
                    headerDescriptor.FontSize(14);
                    headerDescriptor.FontFamily(["Times New Roman"]);
                    headerDescriptor.AlignCenter();

                    var secondColumnItem = column.Item();
                    var secondHeaderDescriptor =
                        secondColumnItem.Text("«КАЗАНСКИЙ (ПРИВОЛЖСКИЙ) ФЕДЕРАЛЬНЫЙ УНИВЕРСИТЕТ»");

                    secondHeaderDescriptor.Bold();
                    secondHeaderDescriptor.FontSize(14);
                    secondHeaderDescriptor.FontFamily(["Times New Roman"]);
                    secondHeaderDescriptor.AlignCenter();

                    var thirdColumnItem = column.Item();
                    var thirdColumnDescriptor = thirdColumnItem.Text("Институт международных отношений");

                    thirdColumnDescriptor.FontSize(14);
                    thirdColumnDescriptor.FontFamily(["Times New Roman"]);
                    thirdColumnDescriptor.AlignCenter();

                    var fourthColumnItem = column.Item();
                    var fourthColumnDescriptor =
                        fourthColumnItem.Text("Кафедра регионоведения и цифровой гуманитаристики");
                    fourthColumnDescriptor.FontSize(14);
                    fourthColumnDescriptor.FontFamily(["Times New Roman"]);
                    fourthColumnDescriptor.AlignCenter();

                    column.Item().Height(20);

                    var fifthColumnItem = column.Item();
                    var fifthColumnDescriptor = fifthColumnItem.Text($"Направление подготовки: {frontPageData.Course}");
                    fifthColumnDescriptor.FontSize(14);
                    fifthColumnDescriptor.FontFamily(["Times New Roman"]);
                    fifthColumnDescriptor.AlignCenter();

                    var sixthColumnItem = column.Item();
                    var sixthColumnDescriptor = sixthColumnItem.Text($"Профиль: {frontPageData.Profile}");
                    sixthColumnDescriptor.FontSize(14);
                    sixthColumnDescriptor.FontFamily(["Times New Roman"]);
                    sixthColumnDescriptor.AlignCenter();
                });

                page.Content().Column(column =>
                {
                    column.Spacing(10);

                    column.Item().Height(20);

                    var contentTitleDescriptor = column.Item().Text("КУРСОВАЯ РАБОТА");
                    contentTitleDescriptor.FontSize(14);
                    contentTitleDescriptor.FontFamily(["Times New Roman"]);
                    contentTitleDescriptor.AlignCenter();

                    var contentThemeTitleDescriptor = column.Item().Text(frontPageData.Theme);
                    contentThemeTitleDescriptor.FontSize(14);
                    contentThemeTitleDescriptor.FontFamily(["Times New Roman"]);
                    contentThemeTitleDescriptor.AlignCenter();
                    contentThemeTitleDescriptor.Bold();

                    column.Item().PaddingTop(30).Column(contentColumn =>
                    {
                        contentColumn.Spacing(70);
                        contentColumn.Item().Row(row =>
                        {
                            row.RelativeItem().Column(info =>
                            {
                                info.Spacing(5);
                                var firstColumnDescriptor =
                                    info.Item().Text($"Студент(ка) {frontPageData.Course} курса");
                                firstColumnDescriptor.FontSize(14);
                                firstColumnDescriptor.FontFamily(["Times New Roman"]);

                                var secondColumnDescriptor = info.Item().Text($"группы {frontPageData.Group}");
                                secondColumnDescriptor.FontSize(14);
                                secondColumnDescriptor.FontFamily(["Times New Roman"]);


                                var currentDate = DateTime.Now;
                                var thirdColumnDescriptor = info.Item()
                                    .Text(
                                        $"«{currentDate.Day}» {culture.DateTimeFormat.MonthGenitiveNames.ElementAt(currentDate.Month - 1)} {currentDate.Year} г.");
                                thirdColumnDescriptor.FontSize(14);
                                thirdColumnDescriptor.FontFamily(["Times New Roman"]);

                                firstColumnDescriptor.LineHeight(1.5f);
                                secondColumnDescriptor.LineHeight(1.5f);
                                thirdColumnDescriptor.LineHeight(1.5f);
                            });

                            var initialsDescriptor = row
                                .AutoItem()
                                .AlignBottom()
                                .Text(frontPageData.Initials);
                            initialsDescriptor.FontSize(14);
                            initialsDescriptor.FontFamily(["Times New Roman"]);
                        });

                        contentColumn.Item().Row(row =>
                        {
                            row.RelativeItem().Column(info =>
                            {
                                info.Spacing(5);
                                var firstColumnDescriptor = info.Item().Text("Научный руководитель");
                                firstColumnDescriptor.FontSize(14);
                                firstColumnDescriptor.FontFamily(["Times New Roman"]);

                                var secondColumnDescriptor = info.Item()
                                    .Text(
                                        $"{frontPageData.SupervisorAcademicDegree}, {frontPageData.SupervisorAcademicTitle}");
                                secondColumnDescriptor.FontSize(14);
                                secondColumnDescriptor.FontFamily(["Times New Roman"]);

                                var currentDate = DateTime.Now;
                                var thirdColumnDescriptor = info.Item()
                                    .Text(
                                        $"«{currentDate.Day}» {culture.DateTimeFormat.MonthGenitiveNames.ElementAt(currentDate.Month - 1)} {currentDate.Year} г.");
                                thirdColumnDescriptor.FontSize(14);
                                thirdColumnDescriptor.FontFamily(["Times New Roman"]);

                                firstColumnDescriptor.LineHeight(1.5f);
                                secondColumnDescriptor.LineHeight(1.5f);
                                thirdColumnDescriptor.LineHeight(1.5f);
                            });

                            var initialsDescriptor = row
                                .AutoItem()
                                .AlignBottom()
                                .Text(frontPageData.SupervisorInitials);
                            initialsDescriptor.FontSize(14);
                            initialsDescriptor.FontFamily(["Times New Roman"]);
                        });
                    });
                });

                var footerDescriptor = page.Footer().PaddingBottom(80).Text($"Казань – {DateTime.Now.Year}");
                footerDescriptor.FontSize(14);
                footerDescriptor.FontFamily(["Times New Roman"]);
                footerDescriptor.AlignCenter();
            });
        });
        return new MemoryStream(doc.GeneratePdf());
    }
}