using Implificator.Abstractions.Services;
using Implificator.API;
using Implificator.API.Implementations.Services;
using Implificator.DAL.DI;
using Microsoft.AspNetCore.HttpOverrides;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

public static class Program
{
    public static WebApplication App { get; private set; }
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var client = new TelegramBotClient(builder.Configuration.GetSection("TgToken").Value);
        await client.DeleteWebhook();
        await client.SetWebhook("https://qr-stats.ru/api/update", allowedUpdates: new List<UpdateType> { UpdateType.Message }, maxConnections: 10);
        var services = builder.Services;
        builder.Services.AddControllers().AddNewtonsoftJson();
        services.AddSingleton<ITelegramBotClient, TelegramBotClient>(
            _ => client);
        services.AddSingleton<BotWorker>();
        services.AddSingleton<ITelegraphService, TelegraphService>(_ => TelegraphService.Create());
        services.AddSwaggerGen();
        services.ConfigureDAL(builder.Configuration);
        services.AddMemoryCache();
        services.AddTransient<IQRStateService, QRStateService>();
        services.AddTransient<IQRService, QRService>();
        
        var app = builder.Build();
        App = app;
        app.UseForwardedHeaders(new ForwardedHeadersOptions
            { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });
        app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapControllers();
        app.Run();
    }
}

