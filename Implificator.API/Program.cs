using Implificator.Abstractions.Services;
using Implificator.API;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using Implificator.API.Implementations;
using Implificator.API.Implementations.Services;
using Implificator.DAL.DI;

public static class Program
{
    public static WebApplication App { get; private set; }
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.
        var services = builder.Services;
        builder.Services.AddControllers().AddNewtonsoftJson();
        services.AddSingleton<ITelegramBotClient, TelegramBotClient>(
            _ => new TelegramBotClient(builder.Configuration.GetSection("TgToken").Value));
        services.AddSingleton<BotWorker>();
        services.AddSingleton<ITelegraphService, TelegraphService>(services => TelegraphService.Create());
        services.ConfigureDAL(builder.Configuration);
        services.AddMemoryCache();
        services.AddTransient<IQRStateService, QRStateService>();
        services.AddTransient<IQRService, QRService>();

        var app = builder.Build();
        // Configure the HTTP request pipeline.
        App = app;
        app.UseForwardedHeaders(new ForwardedHeadersOptions
            { ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto });
        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        app.Run();
    }
}

