using Implificator.API.Common;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Implificator.API.Implementations.Callbacks
{
    public class PremiumCallback : CallbackBase
    {
        public override List<string> Data { get; }

        public PremiumCallback()
        {
            Data = new()
            {
                "premium",
                "premium 1",
                "premium 2",
                "premium 3",
                "premium 4"
            };
        }
        public override async Task Execute(ITelegramBotClient client, Update update)
        {
            var premiumParams = update.CallbackQuery.Data.Split(' ');
            if (update.CallbackQuery.Data == "premium")
            {
                await client.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                var message = await client.EditMessageText(update.CallbackQuery.From.Id,
                    update.CallbackQuery.Message.MessageId, "Цены на покупку премиума", replyMarkup: GetPremiumMenu());
                await client.EditMessageMedia(update.CallbackQuery.From.Id, message.MessageId,
                    new InputMediaPhoto(InputFile.FromStream(new FileStream("Assets/Board.png", FileMode.Open))),
                    replyMarkup: GetPremiumMenu());
            }
            else if (premiumParams.Length == 2)
            {
                await client.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                premiumParams.Last();
            }
        }

        private InlineKeyboardMarkup GetPremiumMenu()
            => new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>
            {
                new()
                {
                    new InlineKeyboardButton("1 - 24\u20bd"){CallbackData = "premium 1"},
                    new InlineKeyboardButton("2 - 49\u20bd"){CallbackData = "premium 2"},
                },
                new()
                {
                    new InlineKeyboardButton("3 - 99\u20bd"){CallbackData = "premium 3"},
                    new InlineKeyboardButton("4 - 199\u20bd"){CallbackData = "premium 4"},
                },
                new()
                {
                    new InlineKeyboardButton("Назад") {CallbackData = "return start"}
                }
            });
    }
}
