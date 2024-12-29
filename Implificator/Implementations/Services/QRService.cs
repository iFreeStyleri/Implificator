using System.Drawing;
using Implificator.Abstractions.Services;
using QrCodes.Payloads;
using QrCodes.Renderers.Abstractions;
using QrCodes.Renderers;
using QrCodes;

namespace Implificator.Implementations.Services
{
    public class QRService : IQRService
    {
        public Stream CreateQRCode(string url)
        {
            var qrCode = QrCodeGenerator.Generate(
                plainText: new Url(url).ToString(),
                eccLevel: ErrorCorrectionLevel.High);
            var jpeg = SkiaSharpRenderer.Render(qrCode, new RendererSettings
            {
                BackgroundType = BackgroundType.RoundRectangle,
                DotStyle = BackgroundType.RoundRectangle,
                LightColor = Color.FromArgb( 24, 25, 29),
                DarkColor = Color.RoyalBlue,
                ConnectDots = true,
                PixelsPerModule = 25
            });
            return jpeg.ToStream(FileFormat.Jpeg, 100);
        }
    }
}
