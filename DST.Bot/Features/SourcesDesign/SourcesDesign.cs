﻿using System.Text;
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
                    await _userHelper.UpdateUserState(user,
                        nameof(CreateBook.SourcesGenerateBookDesignWaitAuthorLastName));
                    await _botClient.SendMessage(message.Chat, "Введите фамилию автора",
                        replyMarkup: new ReplyKeyboardMarkup("Отмена"));
                    break;
                case "Сайт":
                    await _userHelper.UpdateUserState(user, nameof(CreateSite.GetSiteTitleState));
                    await _botClient.SendMessage(message.Chat, "Введите название сайта",
                        replyMarkup: new ReplyKeyboardMarkup("Отмена"));
                    break;
                case "Статью с сайта":
                    await _userHelper.UpdateUserState(user, nameof(CreateSiteArticle.GetArticleTitleState));
                    await _botClient.SendMessage(message.Chat, "Введите название статьи", replyMarkup: new ReplyKeyboardMarkup("Отмена"));
                    break;
                case "Отмена":
                    await _userHelper.UpdateUserState(user, nameof(SourcesDesignQuestionState));
                    await _menuHelper.SendSourceDesignMenu(message, user);
                    break;
            }
        }
    }
}