using Implificator.Abstractions.Services;
using Implificator.API.Common;
using Implificator.API.Common;
using Implificator.API.Implementations.Caches;
using Implificator.API.Implementations.Callbacks;
using Implificator.API.Implementations.ChatMembers;
using Implificator.API.Implementations.Commands;
using Implificator.API.Implementations.Contacts;
using Implificator.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Implificator.API
{
    public class BotWorker
    {
        private readonly ITelegramBotClient _client;
        private readonly IQRStateService _stateService;
        private List<CommandBase> _commands;
        private List<ContactBase> _contactCommands;
        private List<CallbackBase> _callbacks;
        private List<UserCacheCommandBase> _cacheCommands;
        public IReadOnlyList<UserCacheCommandBase> CacheCommands => _cacheCommands.AsReadOnly();
        public IReadOnlyList<CommandBase> Commands => _commands.AsReadOnly();
        public IReadOnlyList<ContactBase> ContactCommands => _contactCommands.AsReadOnly();
        public IReadOnlyList<CallbackBase> Callbacks => _callbacks.AsReadOnly();

        public BotWorker(ITelegramBotClient client, IQRStateService stateService)
        {

            _commands = new()
            {
                new StartCommand()
            };
            _callbacks = new()
            {
                new StartCallback(),
                new PremiumCallback(),
                new ShareMessageCallback(),
                new NextQRMessageCallback()
            };
            _contactCommands = new()
            {
                new QrGeneratorContact()
            };
            _cacheCommands = new()
            {
                new SendQRCodeCacheCommand()
            };
            _client = client;
            _stateService = stateService;
            _ = Echo();
        }

        public async Task Echo()
        { 
            await _client.DeleteWebhook();
            await _client.SetWebhook("https://qr-stats.ru/api/update");
        }
    }
}
