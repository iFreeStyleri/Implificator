using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace Implificator.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateController : ControllerBase
    {

        private readonly ITelegramBotClient _client;
        private readonly BotWorker _worker;

        public UpdateController(ITelegramBotClient client, BotWorker worker)
        {
            _client = client;
            _worker = worker;
        }
        [HttpPost]
        public async Task<IActionResult> CheckUpdate([FromBody] Update update)
        {
            try
            {
                if (update.Message != null)
                {
                    var command = _worker.Commands.SingleOrDefault(s => s.Text.Contains(update.Message.Text));
                    if (command != null)
                        await command.Execute(_client, update);
                }
                else if (update.CallbackQuery != null)
                {
                    var callback = _worker.Callbacks.SingleOrDefault(s => s.Data.Contains(update.CallbackQuery.Data));
                    if (callback != null)
                        await callback.Execute(_client, update);
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.ToString());
            }
            return Ok();
        }

        [HttpGet("check")]
        public IActionResult Check()
        {
            return Ok("Start");
        }
    }
}
