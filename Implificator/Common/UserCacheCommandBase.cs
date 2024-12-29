using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Implificator.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Implificator.Common
{
    public abstract class UserCacheCommandBase
    {
        public abstract DataType Type { get; }
        public abstract Task Execute(ITelegramBotClient client, Update update, UserData userData);
    }
}
