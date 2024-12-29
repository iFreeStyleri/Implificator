using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Implificator.Common
{
    public abstract class ContactBase
    {
        public abstract List<int> RequestIds { get; }
        public abstract Task Execute(ITelegramBotClient client, Update update);

    }
}
