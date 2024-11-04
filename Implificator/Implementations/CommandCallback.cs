using Implificator.Abstractions;
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

namespace Implificator.Implementations
{
    public class CommandCallback : ICallback
    {
        private readonly ITelegramBotClient _telegramBot;
        private readonly IMemoryCache _cache;
        private readonly IQRCallback<string> _messageCallback;
        private readonly LinkCallback _linkCallback;
        private readonly IConfiguration _config;
        private readonly IUserService _userService;

        public CommandCallback(IMemoryCache cache, ITelegramBotClient telegramBot, IQRCallback<string> messageCallback, LinkCallback linkCallback, IConfiguration config, IUserService userService)
        {
            _cache = cache;
            _telegramBot = telegramBot;
            _messageCallback = messageCallback;
            _linkCallback = linkCallback;
            _config = config;
            _userService = userService;
        }

        public async Task DoWork(Update update)
        {
            if (update.Message == null) return;
            switch(update.Message.Text)
            {
                case "/start":
                    await _userService.CreateUser(update.Message.From.Id);
                    await _telegramBot.SendTextMessageAsync(update.Message.Chat,
                        $"https://t.me/InfluencererBot?start=sendRate-{update.Message.From.Id}{_config["title"]}",
                        replyMarkup: GetMenu($"https://t.me/InfluencererBot?start=sendRate-{update.Message.From.Id}"));
                    _cache.Remove(update.Message.From.Id);
                    break;
            }


            await MenuWork(update);
            await _linkCallback.DoWork(update);
        }

        private async Task MenuWork(Update update)
        {
            await _messageCallback.DoWork(update);
            await _messageCallback.DoWorkAnother(update);
        }

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
    }
}
