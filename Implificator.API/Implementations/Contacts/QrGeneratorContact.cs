using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Implificator.Abstractions.Services;
using Implificator.API.Common;
using Implificator.Models;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Implificator.API.Implementations.Contacts
{
    public class QrGeneratorContact : ContactBase
    {
        public override List<int> RequestIds { get; }

        public QrGeneratorContact()
        {
            RequestIds = new()
            {
                1
            };
        }
        public override Task Execute(ITelegramBotClient client, Update update)
        {
            var user = update.Message.UsersShared.Users.FirstOrDefault();
            var qrStateService = Program.App.Services.GetRequiredService<IQRStateService>();
            qrStateService.SetUserData(update.Message.From.Id, new UserData { Type = DataType.AddMessage, UserSharedId = user.UserId});
            client.SendMessage(update.Message.Chat, "Введите текст, который вы хотите передать человеку (:");
            return Task.CompletedTask;
        }
    }
}
