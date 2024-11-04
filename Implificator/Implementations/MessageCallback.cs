using Implificator.Abstractions;
using Implificator.Models;
using Kvyk.Telegraph;
using Kvyk.Telegraph.Models;
using Microsoft.Extensions.Caching.Memory;
using QrCodes;
using QrCodes.Renderers;
using QrCodes.Renderers.Abstractions;
using System.Text.RegularExpressions;
using QrCodes.Payloads;
using Telegram.Bot;
using Telegram.Bot.Types;
using Color = System.Drawing.Color;
using File = System.IO.File;

namespace Implificator.Implementations
{
    public class MessageCallback : IQRCallback<string>, IPageCreator<string>
    {
        private readonly IMemoryCache _cache;
        private readonly ITelegramBotClient _botClient;
        private readonly TelegraphClient _telegraphClient;
        public MessageCallback(ITelegramBotClient botClient, TelegraphClient telegraphClient, IMemoryCache cache)
        {
            _telegraphClient = telegraphClient;
            _botClient = botClient;
            _cache = cache;
        }


        public async Task<string> CreatePage(string description, long idChat)
        {
            if (description.Length < 15)
                throw new ArgumentException("Текст должен состоять минимум из 15 символов");
            var nodes = new List<Node>
            {
                Node.Img("https://i.postimg.cc/13CTps8X/3282669.png"),
                Node.P(description),
                Node.A("https://t.me/InfluencererBot", "@InfluencererBot")
            };
            var acc = await _telegraphClient.CreateAccount("Inf", "Influencer", "https://t.me/InfluencererBot");
            _telegraphClient.AccessToken = acc.AccessToken;
            var page = await _telegraphClient.CreatePage("Письмо", nodes);
            return page.Url;
        }

        public async Task DoWork(Update update)
        {
            if(_cache.TryGetValue(update.Message.From.Id, out MenuType menuType) 
                && menuType == MenuType.Message)
            {
                if (update.Message.Text.Length <= 15)
                    throw new ArgumentException("Сообщение не должно быть меньше 15 символов (:");
                var url = await CreatePage(update.Message.Text, update.Message.Chat.Id);
                await using var qrStream = GetQrCode(url);
                await _botClient.SendPhotoAsync(update.Message.Chat, InputFile.FromStream(qrStream), hasSpoiler: true);
                _cache.Remove(update.Message.From.Id);
            }
        }

        public async Task DoWorkAnother(Update update)
        {
            if (_cache.TryGetValue(update.Message.From.Id, out StartRequest request) 
                && request != null &&  request.MenuType == MenuType.Message)
            {
                if (update.Message.Text.Length <= 15)
                    throw new ArgumentException("Сообщение не должно быть меньше 15 символов (:");
                var url = await CreatePage(update.Message.Text, update.Message.Chat.Id);
                await using var qrStream = GetQrCode(url);
                await _botClient.SendPhotoAsync(request.Value, InputFile.FromStream(qrStream), hasSpoiler: true);
                _cache.Remove(update.Message.From.Id);
            }
        }


        public Stream GetQrCode(string url)
        {
            var qrCode = QrCodeGenerator.Generate(
                plainText: new Url(url).ToString(),
                eccLevel: ErrorCorrectionLevel.High, forceUtf8: true);
            var skia = new SkiaSharpRenderer();

            var pngBytes = skia.RenderToStream(
                qrCode,
                settings: new RendererSettings
                {
                    Quality = 512,
                    DarkColor = Color.DeepSkyBlue,
                    PixelSizeFactor = 5,
                    ConnectDots = true,
                    DotStyle = BackgroundType.Circle,
                    BackgroundColor = Color.White,
                    BackgroundImageStyle = BackgroundImageStyle.Fill,
                    IconBackgroundColor = Color.Black,
                    IconSizePercent = 20,
                    IconBorderWidth = 20,
                    PixelsPerModule = 15,
                    LightColor = Color.Black,
                    QuietZoneStyle = QuietZoneStyle.Flat,
                    IconBytes = Program.Logo
                });
            return pngBytes;
        }
    }
}
