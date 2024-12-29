using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Implificator.Abstractions.Services;
using Implificator.API.Common;
using Implificator.DAL;
using Implificator.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Implificator.DAL.Entities;
using User = Implificator.DAL.Entities.User;
using Telegram.Bot.Types.ReplyMarkups;

namespace Implificator.API.Implementations.Caches
{
    public class SendQRCodeCacheCommand : UserCacheCommandBase
    {
        public override DataType Type { get; }

        public SendQRCodeCacheCommand()
        {
            Type = DataType.AddMessage;
        }

        public override async Task Execute(ITelegramBotClient client, Update update, UserData userData)
        {
            await using var context = Program.App.Services.GetRequiredService<UserContext>();
            var user = await GetUserById(context, update.Message.From.Id);
            if (user != null)
            {
                var qrService = Program.App.Services.GetRequiredService<IQRService>();
                var telegraphService = Program.App.Services.GetRequiredService<ITelegraphService>();
                var url = await telegraphService.CreatePage(update.Message.Text);
                var sharedUser = await AddUser(context, userData.UserSharedId);
                await AddMessage(context, user, sharedUser, url, !sharedUser.IsBlocked);
                if (!sharedUser.IsBlocked)
                {
                    await using var qrStream = qrService.CreateQRCode(url);
                    qrStream.Seek(0, SeekOrigin.Begin);
                    await client.SendPhoto(sharedUser.TgId, InputFile.FromStream(qrStream));
                    await client.SendMessage(user.TgId, "QR-письмо успешно отправлено (:", replyMarkup: new ReplyKeyboardRemove());
                }
                else
                {
                    await client.SendMessage(user.TgId, "Пользователь не пользуется услугами нашего бота :(\nВы можете с радостью поделиться ссылкой на нашего телеграм бота, и тогда он сразу же прочитает ваше QR-письмо! (:", replyMarkup: new ReplyKeyboardRemove());
                }
            }
        }

        private async Task<User?> GetUserById(UserContext context, long tgId)
        {
            var user = await context.Users.SingleOrDefaultAsync(s => s.TgId == tgId);
            return user;
        }

        private async Task<User> AddUser(UserContext context, long tgId)
        {
            var result = await GetUserById(context, tgId);
            if (result is not null) return result;
            
            var user = context.Users.Add(new User { TgId = tgId, IsBlocked = true});
            await context.SaveChangesAsync();
            return user.Entity;

        }

        private async Task AddMessage(UserContext context, User user, User sharedUser, string url, bool isPublish)
        {
            var message = new QRMessage
            {
                User = user,
                SharedUser = sharedUser,
                URL = url,
                IsPublish = isPublish
            };
            context.QRMessages.Add(message);
            await context.SaveChangesAsync();
        }
    }
}
