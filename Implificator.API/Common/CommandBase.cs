using Telegram.Bot.Types;
using Telegram.Bot;

namespace Implificator.API.Common
{
    public abstract class CommandBase
    {
        public abstract List<string> Text { get; }
        public abstract Task Execute(ITelegramBotClient client, Update update);
    }
}
