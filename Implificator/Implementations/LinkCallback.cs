using Implificator.Abstractions;
using Implificator.Models;
using Kvyk.Telegraph;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Implificator.Implementations
{
    public class LinkCallback : ICallback
    {
        private readonly IMemoryCache _cache;
        private readonly ITelegramBotClient _botClient;
        private readonly IConfiguration _config;

        public LinkCallback(ITelegramBotClient botClient, IMemoryCache cache, IConfiguration config)
        {
            _botClient = botClient;
            _cache = cache;
            _config = config;
        }

        public async Task DoWork(Update update)
        {
            await Task.Run(async () =>
            {
                if (update.Message == null) return;
                var request = new StartRequest(update.Message.Text);
                if (request != null && request.Created)
                {
                    _cache.Set(update.Message.From.Id, request);
                    await _botClient.SendTextMessageAsync(update.Message.Chat, _config["messageDescription"]);
                }
            });
        }
    }
}
