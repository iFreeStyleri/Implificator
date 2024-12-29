using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Implificator.API.Common
{
    public abstract class ChatMemberBase
    {
        public abstract Task Execute(ITelegramBotClient client, Update update);
    }
}
