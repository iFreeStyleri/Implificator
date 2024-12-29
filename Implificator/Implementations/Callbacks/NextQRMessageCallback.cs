using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Implificator.Abstractions.Services;
using Implificator.API.Common;
using Implificator.DAL.Entities;
using Implificator.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = Implificator.DAL.Entities.User;
namespace Implificator.Implementations.Callbacks
{
    public class NextQRMessageCallback : CallbackBase
    {
        public override List<string> Data { get; }

        public NextQRMessageCallback()
        {
            Data = new()
            {
                "next qr-message"
            };
        }
        public override async Task Execute(ITelegramBotClient client, Update update)
        {
            var telegraphService = Program.Services.GetRequiredService<ITelegraphService>();
            var qrService = Program.Services.GetRequiredService<IQRService>();
            await using var context = Program.Services.GetRequiredService<UserContext>();
            var message = await GetMessage(context, update.CallbackQuery.From.Id);
            if (message is not null)
            {
                await using var qrStream = qrService.CreateQRCode(message.URL);
                qrStream.Seek(0, SeekOrigin.Begin);
                await client.SendPhoto(update.CallbackQuery.From.Id, InputFile.FromStream(qrStream), replyMarkup: GetNextKeyboardMarkup());
            }
            else
                await client.AnswerCallbackQuery(update.CallbackQuery.Id, "Список qr-сообщений пуст :(");
            await client.EditMessageReplyMarkup(update.CallbackQuery.From.Id, update.CallbackQuery.Message.MessageId, replyMarkup: new InlineKeyboardMarkup());
        }

        private InlineKeyboardMarkup GetNextKeyboardMarkup()
            => new(new List<List<InlineKeyboardButton>>
            {
                new ()
                {
                    new InlineKeyboardButton("Дальше") {CallbackData = "next qr-message"}
                }

            });


        private async Task<QRMessage?> GetMessage(UserContext context, long sharedTgId)
        {
            var message = await context.QRMessages
                .Include(i => i.SharedUser)
                .FirstOrDefaultAsync(s => s.SharedUser.TgId == sharedTgId && !s.IsPublish);
            if (message is not null)
            {
                message.IsPublish = true;
                context.Update(message);
                await context.SaveChangesAsync();
                return message;
            }
            return null;
        }

    }
}
