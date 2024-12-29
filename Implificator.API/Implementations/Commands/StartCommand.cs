using Implificator.Abstractions.Services;
using Implificator.API.Common;
using Implificator.DAL;
using Implificator.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using User = Implificator.DAL.Entities.User;

namespace Implificator.API.Implementations.Commands
{
    public class StartCommand : CommandBase
    {
        public override List<string> Text { get; }

        public StartCommand()
        {
            Text = new()
            {
                "/start",
                "\ud83d\udd19 Назад"
            };
        }
        public override async Task Execute(ITelegramBotClient client, Update update)
        {
            await using var context = Program.App.Services.GetRequiredService<UserContext>();
            var qrService = Program.App.Services.GetRequiredService<IQRService>();
            if (update.Message.Text.Contains("\ud83d\udd19 Назад"))
                await client.SendMessage(update.Message.Chat, text:"В главное меню...", replyMarkup: new ReplyKeyboardRemove() );

            await AddUser(context, update);
            await client.SendTextMessageAsync(update.Message?.Chat.Id,
                $"https://t.me/InfluencererBot?start=sendRate-{update.Message.From.Id}\n\n\u2709\ufe0f Поставь ссылку в описание, выложи ее в свой личный блог, отправь ее друзьям и знакомым\n\nЧем больше людей, тем больше оценок",
                replyMarkup: GetMenu($"https://t.me/InfluencererBot?start=sendRate-{update.Message.From.Id}"));

            var message = await GetMessage(context, update.Message.Chat.Id);
            if (message is not null)
            {
                using var qrStream = qrService.CreateQRCode(message.URL);
                qrStream.Seek(0, SeekOrigin.Begin);
                await client.SendPhoto(update.Message.From.Id, InputFile.FromStream(qrStream), replyMarkup: GetNextKeyboardMarkup());
            }

        }

        private InlineKeyboardMarkup GetMenu(string link)
            => new(new List<List<InlineKeyboardButton>>
            {
                new()
                {
                    new InlineKeyboardButton("\ud83d\udc7d Поделиться ссылкой-приглашением \ud83d\udc7d"){ SwitchInlineQuery = link},
                },
                new()
                {
                    new InlineKeyboardButton("\ue1d8 Отправить QR-письмо \ue1d8"){ CallbackData = "get user message"}
                },
                new()
                {
                    new InlineKeyboardButton("\ud83c\udf1f ПРЕМИУМ \ud83c\udf1f") {CallbackData = "premium"}
                }
            });

        private InlineKeyboardMarkup GetNextKeyboardMarkup()
            => new(new List<List<InlineKeyboardButton>>
            {
                new ()
                {
                    new InlineKeyboardButton("Дальше") {CallbackData = "next qr-message"}
                }
                
            });
        private async Task AddUser(UserContext context, Update update)
        {
            var user = await context.Users.SingleOrDefaultAsync(s => s.TgId == update.Message.From.Id);
            if (user is null)
            {
                context.Users.Add(new User { TgId = update.Message.From.Id });
                await context.SaveChangesAsync();
            }
            else
            {
                user.IsBlocked = false;
                context.Update(user);
                await context.SaveChangesAsync();
            }
        }

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
