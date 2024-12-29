using Implificator.API.Common;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Implificator.Implementations.Callbacks
{
    public class StartCallback : CallbackBase
    {
        public override List<string> Data { get; }

        public StartCallback()
        {
            Data = new()
            {
                "return start"
            };
        }
        public override async Task Execute(ITelegramBotClient client, Update update)
        {
            if (update.CallbackQuery.Message.Photo != null && update.CallbackQuery.Message.Photo.Length != 0)
            {
                await client.DeleteMessage(update.CallbackQuery.Message.Chat, update.CallbackQuery.Message.MessageId);
            }
            await client.SendMessage(update.CallbackQuery.From.Id,
                $"https://t.me/InfluencererBot?start=sendRate-{update.CallbackQuery.From.Id}\n\n\u2709\ufe0f Поставь ссылку в описание, выложи ее в свой личный блог, отправь ее друзьям и знакомым\n\nЧем больше людей, тем больше оценок",
                replyMarkup: GetMenu($"https://t.me/InfluencererBot?start=sendRate-{update.CallbackQuery.From.Id}"));
        }

        private InlineKeyboardMarkup GetMenu(string link)
            => new(new List<List<InlineKeyboardButton>>
            {
                new()
                {
                    new InlineKeyboardButton("\ud83d\udc7d Поделиться ссылкой-приглашением \ud83d\udc7d"){ SwitchInlineQuery = link},
                },
                new()
                {
                    new InlineKeyboardButton("\ue1d8 Отправить QR-письмо \ue1d8"){ CallbackData = "get user message"}
                },
                new()
                {
                    new InlineKeyboardButton("\ud83c\udf1f ПРЕМИУМ \ud83c\udf1f") {CallbackData = "premium"}
                }
            });
    }
}
