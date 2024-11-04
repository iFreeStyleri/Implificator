using Implificator.API;
using Telegram.Bot;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
builder.Services.AddSingleton<ITelegramBotClient, TelegramBotClient>(
    services => new TelegramBotClient("7857131026:AAF9i5457bpsgwF-3_wSRqgSOabcF3UinbE"));
builder.Services.AddSingleton<BotWorker>();
var app = builder.Build();
_ = app.Services.GetRequiredService<BotWorker>().Echo();
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
