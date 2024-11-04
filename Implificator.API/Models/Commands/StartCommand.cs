using Implificator.API.Common;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Implificator.API.Models.Commands
{
    public class StartCommand : CommandBase
    {
        public override List<string> Text { get; }

        public StartCommand()
        {
            Text = new()
            {
                "/start"
            };
        }
        public override async Task Execute(ITelegramBotClient client, Update update)
        {
            await client.SendTextMessageAsync(update.Message?.Chat.Id,
                $"https://t.me/InfluencererBot?start=sendRate-{update.Message.From.Id}\n\n\u2709\ufe0f Поставь ссылку в описание, выложи ее в свой личный блог, отправь ее друзьям и знакомым\n\nЧем больше людей, тем больше оценок",
                replyMarkup: GetMenu($"https://t.me/InfluencererBot?start=sendRate-{update.Message.From.Id}"));
        }

        private InlineKeyboardMarkup GetMenu(string link)
            => new (new List<List<InlineKeyboardButton>>
            {
                new()
                {
                    new InlineKeyboardButton("\ud83d\udc7d Поделиться ссылкой-приглашением \ud83d\udc7d"){ SwitchInlineQuery = link},
                },
                new()
                {
                    new InlineKeyboardButton("\ue1d8 Создать QR-письмо \ue1d8"){ CallbackData = "message"}
                },
                new()
                {
                    new InlineKeyboardButton("\ud83c\udf1f ПРЕМИУМ \ud83c\udf1f") {CallbackData = "premium"}
                }
            });
    }
}
