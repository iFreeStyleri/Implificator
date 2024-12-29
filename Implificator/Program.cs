
using Implificator;
using Implificator.Abstractions.Services;
using Implificator.DAL.DI;
using Implificator.Implementations.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace Implificator;

public static class Program
{
    public static IHost Host { get; private set; }
    public static IServiceProvider Services => Host.Services;
    public static async Task Main(string[] args)
    {
        var host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args);
        host.ConfigureHostConfiguration(config =>
        {

            config.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", true, true).Build();
        });

        host.ConfigureServices((context, services) =>
        {
            services.AddSingleton<ITelegramBotClient, TelegramBotClient>(
                _ => new TelegramBotClient(context.Configuration.GetSection("TgToken").Value));
            services.AddSingleton<BotWorker>();
            services.AddSingleton<ITelegraphService, TelegraphService>(services => TelegraphService.Create());
            services.ConfigureDAL(context.Configuration);
            services.AddMemoryCache();
            services.AddTransient<IQRStateService, QRStateService>();
            services.AddTransient<IQRService, QRService>();
        });

        Host = host.Build();
        var worker = Services.GetRequiredService<BotWorker>();
        await worker.Echo();
    }
}

