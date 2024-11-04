using Implificator.Abstractions;
using Implificator.DAL.DI;
using Implificator.Implementations;
using Implificator.Models;
using Kvyk.Telegraph;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Types;
using yoomoney_api.authorize;
using File = System.IO.File;

public static class Program
{
    public static string ApplicationPath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    public static byte[] Logo = File.ReadAllBytes($@"{ApplicationPath}\data\mail.png");
    public static IHost Host { get; private set; }
    public static IServiceProvider Services => Host.Services;
    private static async Task Main(string[] args)
    {
        var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args);
        builder.Configuration
            .AddXmlFile($@"{ApplicationPath}\data\config\ru.config")
            .AddXmlFile($@"{ApplicationPath}\data\config\App.config");
        builder.Services
            .ConfigureDAL(builder.Configuration)
            .ConfigureServices(builder.Configuration);

        Host = builder.Build();
        await Host.StartAsync();
        var token = Services.GetRequiredService<YooToken>();
        var client = Services.GetRequiredService<ITelegramBotClient>();
        client.StartReceiving(UpdateHandler, ErrorHandler);

        await Task.Delay(-1);
    }

    private static async Task ErrorHandler(ITelegramBotClient client, Exception exception, CancellationToken token)
    {
        await Console.Out.WriteLineAsync(exception.Message);
    }

    private static async Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
    {
        try
        {
            switch (update.Type)
            {
                case Telegram.Bot.Types.Enums.UpdateType.Unknown:
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.Message:
                    {
                        var callback = Services.GetRequiredService<CommandCallback>();
                        await callback.DoWork(update);
                        break;
                    }
                case Telegram.Bot.Types.Enums.UpdateType.InlineQuery:
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.ChosenInlineResult:
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.CallbackQuery:
                    {
                        var callback = Services.GetRequiredService<MenuCallback>();
                        await callback.DoWork(update);
                    }
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.EditedMessage:
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.ChannelPost:
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.EditedChannelPost:
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.ShippingQuery:
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.PreCheckoutQuery:
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.Poll:
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.PollAnswer:
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.MyChatMember:
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.ChatMember:
                    break;
                case Telegram.Bot.Types.Enums.UpdateType.ChatJoinRequest:
                    break;
            }

        }
        catch (Exception ex)
        {
            if(update.Message != null)
                await client.SendTextMessageAsync(update.Message.Chat, ex.Message);
            if (update.CallbackQuery != null)
                await client.SendTextMessageAsync(update.CallbackQuery.Message.Chat, ex.Message);

        }
    }


    private static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddTransient<IQRCallback<string>, MessageCallback>();
        services.AddTransient<TelegraphClient>();
        services.AddSingleton<ITelegramBotClient, TelegramBotClient>(services 
            => new TelegramBotClient(config.GetSection("TelegramToken").Value));
        services.AddMemoryCache();
        services.AddTransient<MenuCallback>();
        services.AddTransient<CommandCallback>();
        services.AddTransient<LinkCallback>();
        services.AddTransient<IUserService, UserService>();
        services.AddSingleton(services =>
        {
            var auth = new Authorize(config["YooMoneyClientId"], "https://t.me/InfluencererBot", new[]
            {
                "account-info",
                "operation-history",
                "operation-details",
                "incoming-transfers",
                "payment-p2p",
            });
            var tgClient = services.GetRequiredService<ITelegramBotClient>();
            tgClient.SendTextMessageAsync(long.Parse(config["TgAdmin"]), auth.AuthorizeUrl);
            return auth;
        });
        services.AddSingleton(services =>
        {
            var auth = services.GetRequiredService<Authorize>();
            var code = Console.ReadLine();
            var accessToken = auth.GetAccessToken(code, config["YooMoneyClientId"], "https://t.me/InfluencererBot").Result;
            return new YooToken {AccessToken = accessToken};
        });
        return services;
    }
}