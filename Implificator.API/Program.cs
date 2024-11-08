using Implificator.API;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddSingleton<ITelegramBotClient, TelegramBotClient>(
    services => new TelegramBotClient(builder.Configuration.GetSection("TgToken").Value));
builder.Services.AddSingleton<BotWorker>();
var app = builder.Build();
_ = app.Services.GetRequiredService<BotWorker>().Echo();
// Configure the HTTP request pipeline.

app.UseForwardedHeaders(new ForwardedHeadersOptions()
    {ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto});
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Run();
