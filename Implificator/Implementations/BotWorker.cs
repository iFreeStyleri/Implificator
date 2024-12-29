using Implificator.Abstractions.Services;
using Implificator.API.Common;
using Implificator.Common;
using Implificator.Implementations.Caches;
using Implificator.Implementations.Callbacks;
using Implificator.Implementations.ChatMembers;
using Implificator.Implementations.Commands;
using Implificator.Implementations.Contacts;
using Implificator.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Implificator
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
        }

        public async Task Echo()
        { 
            await _client.DeleteWebhook();
            await _client.ReceiveAsync(Updator, ErrorUpdator);
            await Task.Delay(-1);
        }

        private async Task ErrorUpdator(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            await Console.Out.WriteLineAsync(exception.ToString());
        }

        private async Task Updator(ITelegramBotClient client, Update update, CancellationToken token)
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        var userData = _stateService.GetUserData(update.Message.From.Id);
                        if (update.Message.Type == MessageType.Text)
                        {
                            var command = Commands.SingleOrDefault(s => s.Text.Contains(update.Message.Text));
                            if (command != null)
                            {
                                await command.Execute(_client, update);
                                return;
                            }
                        }
                        else if (update.Message.Type == MessageType.UsersShared)
                        {
                            var contactCommand = ContactCommands.SingleOrDefault(s =>
                                s.RequestIds.Contains(update.Message.UsersShared.RequestId));
                            if (contactCommand != null)
                            {
                                await contactCommand.Execute(client, update);
                                return;
                            }
                        }
                        if (userData != null)
                        {
                            var cacheCommand = CacheCommands.SingleOrDefault(s => s.Type == userData.Type);
                            if (cacheCommand != null)
                                await cacheCommand.Execute(client, update, userData);
                        }
                        break;
                    case UpdateType.CallbackQuery:
                        var callback = Callbacks.SingleOrDefault(s => s.Data.Contains(update.CallbackQuery.Data));
                        if (callback != null)
                            await callback.Execute(_client, update);
                        break;
                    case UpdateType.MyChatMember:
                    {
                        var chatMember = new StatusChatMember();
                        await chatMember.Execute(client, update);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.ToString());
            }
        }
    }
}
