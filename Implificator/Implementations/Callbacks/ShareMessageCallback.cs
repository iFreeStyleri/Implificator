using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Implificator.API.Common;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace Implificator.Implementations.Callbacks
{
    public class ShareMessageCallback : CallbackBase
    {
        public override List<string> Data { get; }

        public ShareMessageCallback()
        {
            Data = new()
            {
                "get user message"
            };
        }
        public override async Task Execute(ITelegramBotClient client, Update update)
        {
            await client.SendMessage(update.CallbackQuery.Message.Chat, "Выберите друга, которому хотите отправить открытку (:", replyMarkup: GetMenu());
        }

        private ReplyKeyboardMarkup GetMenu()
            => new ReplyKeyboardMarkup(new List<List<KeyboardButton>>()
            {
                new()
                {
                    KeyboardButton.WithRequestUsers("\ud83d\udcf1 Выбрать контакт \ud83d\udcf1",
                        new KeyboardButtonRequestUsers(1)
                        {
                            UserIsBot = false
                        })
                },
                new()
                {
                    new KeyboardButton("\ud83d\udd19 Назад")
                }
            });
    }
}
