using Implificator.API.Common;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Implificator.API.Models.Callbacks
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
                await client.EditMessageTextAsync(update.CallbackQuery.From.Id,
                    update.CallbackQuery.Message.MessageId, "Цены на покупку премиума", replyMarkup: GetPremiumMenu());
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
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton("1"){CallbackData = "premium 1"},
                    new InlineKeyboardButton("2"){CallbackData = "premium 2"},
                },
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton("3"){CallbackData = "premium 3"},
                    new InlineKeyboardButton("4"){CallbackData = "premium 4"},
                },
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton("Назад") {CallbackData = "return start"}
                }
            });
    }
}
