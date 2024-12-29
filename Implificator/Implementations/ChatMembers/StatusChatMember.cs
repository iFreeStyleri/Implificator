using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Implificator.Common;
using Implificator.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Implificator.Implementations.ChatMembers
{
    public class StatusChatMember : ChatMemberBase
    {
        public override async Task Execute(ITelegramBotClient client, Update update)
        {
            await using var context = Program.Services.GetRequiredService<UserContext>();
            var user = await context.Users.SingleOrDefaultAsync(s => s.TgId == update.MyChatMember.From.Id);
            if (user is not null)
            {
                switch (update.MyChatMember.NewChatMember.Status)
                {
                    case ChatMemberStatus.Kicked:
                        user.IsBlocked = true;
                        break;
                    case ChatMemberStatus.Left:
                        user.IsBlocked = false;
                        break;
                }

                context.Update(user);
                await context.SaveChangesAsync();
            }
        }
    }
}
