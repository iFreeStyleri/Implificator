using Implificator.Abstractions;
using Implificator.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using yoomoney_api.account;
using yoomoney_api.quickpay;

namespace Implificator.Implementations
{
    public class MenuCallback : ICallback
    {
        private readonly ITelegramBotClient _telegramBot;
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _config;
        private readonly YooToken _token;
        public MenuCallback(IMemoryCache cache, ITelegramBotClient telegramBot, IConfiguration config, YooToken token)
        {
            _cache = cache;
            _telegramBot = telegramBot;
            _config = config;
            _token = token;
        }
        public async Task DoWork(Update update)
        {
            if(update.CallbackQuery != null)
            {
                switch (update.CallbackQuery.Data)
                {
                    case "message":
                        await SendMessage(update);
                        break;
                    case "premium":
                        await _telegramBot.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                        await _telegramBot.EditMessageTextAsync(update.CallbackQuery.From.Id,
                            update.CallbackQuery.Message.MessageId, "Цены на покупку премиума", replyMarkup: GetPremiumMenu());
                        break;
                    case "forward premium":
                        await _telegramBot.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                        await _telegramBot.EditMessageTextAsync(update.CallbackQuery.From.Id, 
                            update.CallbackQuery.Message.MessageId,
                            $"https://t.me/InfluencererBot?start=sendRate-{update.CallbackQuery.Message.From.Id}{_config["title"]}",
                            replyMarkup: GetMenu($"\nhttps://t.me/InfluencererBot?start=sendRate-{update.CallbackQuery.Message.From.Id}"));
                        break;
                    case { } data when data.Contains("buy premium"):
                        await CreateMoneyRequest(update, int.Parse(data.Replace("buy premium ", "")));
                        break;
                    case { } data when data.Contains("check premium"):
                        await CheckDonut(update, data.Replace("check premium ", ""));
                        break;


                }
            }
        }

        private async Task CheckDonut(Update update, string label)
        {
            var client = new Client(_token.AccessToken);
            var history = client.GetOperationHistory(_token.AccessToken, label: label);
            var operation = history.Operations.SingleOrDefault(w => w.Label == label);
            if (operation is {Status: "success"})
            {
                await _telegramBot.SendTextMessageAsync(update.CallbackQuery.From.Id, "Вы успешно оплатили премиум!");
            }
        }
        private async Task CreateMoneyRequest(Update update, int choice)
        {
            var label = Guid.NewGuid().ToString();
            label += $"={update.CallbackQuery.Message.Chat.Id}";
            var quickpay = new Quickpay(receiver: "4100118870308060", quickpayForm: "shop", sum: 10,
                label: label, paymentType: "AC");
            _telegramBot.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
            _telegramBot.SendTextMessageAsync(update.CallbackQuery.Message.Chat, "Откройте средство оплаты премиума, а после оплаты проверьте его статус!",
                replyMarkup: GetMoneyMenu(quickpay.LinkPayment, quickpay.Label));
        }
        private async Task SendMessage(Update update)
        {
            await _telegramBot.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
            await _telegramBot.SendTextMessageAsync(update.CallbackQuery.From.Id, "Напишите письмо, длинною не менее 15 символов (:");
            _cache.Set(update.CallbackQuery.From.Id, MenuType.Message, DateTimeOffset.Now.AddMinutes(10));
        }

        private InlineKeyboardMarkup GetPremiumMenu()
            => new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton("1"){CallbackData = "buy premium 1"},
                    new InlineKeyboardButton("2"){CallbackData = "buy premium 2"},
                },
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton("3"){CallbackData = "buy premium 3"},
                    new InlineKeyboardButton("4"){CallbackData = "buy premium 4"},
                },
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton(_config["forward"]) {CallbackData = "forward premium"}
                }
            });
        private InlineKeyboardMarkup GetMenu(string link)
            => new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton(_config["buttonLink"]){ SwitchInlineQuery = link},
                },
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton(_config["buttonQR"]){ CallbackData = "message"}
                },
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton(_config["buttonPremium"]) {CallbackData = "premium"}
                }
            });

        private InlineKeyboardMarkup GetMoneyMenu(string link, string label)
            => new InlineKeyboardMarkup(new List<List<InlineKeyboardButton>>
            {
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton("Открыть"){ Url = link}
                },
                new List<InlineKeyboardButton>
                {
                    new InlineKeyboardButton("Проверить") {CallbackData = $"check premium {label}"}
                }
            });
    }
}
