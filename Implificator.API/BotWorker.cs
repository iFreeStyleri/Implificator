using Implificator.API.Common;
using Implificator.API.Models.Callbacks;
using Implificator.API.Models.Commands;
using Telegram.Bot;

namespace Implificator.API
{
    public class BotWorker
    {
        private readonly ITelegramBotClient _client;
        private readonly IConfiguration _config;
        private List<CommandBase> _commands;
        private List<CallbackBase> _callbacks;
        public IReadOnlyList<CommandBase> Commands
        {
            get => _commands.AsReadOnly();
        }

        public IReadOnlyList<CallbackBase> Callbacks
        {
            get => _callbacks.AsReadOnly();
        }

        public BotWorker(ITelegramBotClient client, IConfiguration config)
        {

            _commands = new()
            {
                new StartCommand()
            };
            _callbacks = new()
            {
                new StartCallback(),
                new PremiumCallback()
            };

            _client = client;
            _config = config;
        }

        public async Task Echo()
        {
            await _client.SetWebhookAsync(_config.GetSection("ngrok-webhook").Value);
        }
    }
}
