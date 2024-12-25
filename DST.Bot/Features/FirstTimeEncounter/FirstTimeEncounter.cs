using DST.Bot.Features.Common;
using DST.Bot.Features.StateManager;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = DST.Bot.Entities.User;

namespace DST.Bot.Features.FirstTimeEncounter;

public static class FirstTimeEncounter
{
    public class FirstTimeState : IDialogState
    {
        private readonly UserHelper _userHelper;
        private readonly ITelegramBotClient _telegramBotClient;

        public FirstTimeState(UserHelper userHelper, ITelegramBotClient telegramBotClient)
        {
            _userHelper = userHelper;
            _telegramBotClient = telegramBotClient;
        }
        
        public async Task Handle(Message message, User user)
        {
            await _userHelper.UpdateUserState(user, DialogStateId.PsychologicalTestFirstQuestionState);
            await _telegramBotClient.SendMessage(message.Chat, """
                                                            Прохождение теста.
                Как вы обычно реагируете на критику?
                """, replyMarkup: new ReplyKeyboardMarkup()
                .AddNewRow("Принимаю её с благодарностью и стараюсь улучшиться")
                .AddNewRow("Чувствую себя обиженным, но стараюсь не показывать это")
                .AddNewRow("Защищаюсь и объясняю свою точку зрения")
                .AddNewRow("Игнорирую и продолжаю действовать по-своему"));
        }

        public DialogStateId DialogStateId { get; } = DialogStateId.FirstTimeState;
    }
}