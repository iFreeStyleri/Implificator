using Telegram.Bot.Types;
using Telegram.Bot;

namespace Implificator.API.Common
{
    public abstract class CallbackBase
    {
        public abstract List<string> Data { get; }
        public abstract Task Execute(ITelegramBotClient client, Update update);

    }
}
