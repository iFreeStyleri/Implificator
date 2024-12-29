using Implificator.Abstractions.Services;
using Implificator.API.Implementations.ChatMembers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Implificator.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UpdateController : ControllerBase
    {

        private readonly ITelegramBotClient _client;
        private readonly BotWorker _worker;
        private readonly IQRStateService _stateService;
        public UpdateController(ITelegramBotClient client, BotWorker worker, IQRStateService stateService)
        {
            _client = client;
            _worker = worker;
            _stateService = stateService;
        }
        [HttpPost]
        public async Task<IActionResult> CheckUpdate([FromBody] Update update)
        {
            try
            {
                switch (update.Type)
                {
                    case UpdateType.Message:
                        var userData = _stateService.GetUserData(update.Message.From.Id);
                        if (update.Message.Type == MessageType.Text)
                        {
                            var command = _worker.Commands.SingleOrDefault(s => s.Text.Contains(update.Message.Text));
                            if (command != null)
                            {
                                await command.Execute(_client, update);
                                return Ok();
                            }
                        }
                        else if (update.Message.Type == MessageType.UsersShared)
                        {
                            var contactCommand = _worker.ContactCommands.SingleOrDefault(s =>
                                s.RequestIds.Contains(update.Message.UsersShared.RequestId));
                            if (contactCommand != null)
                            {
                                await contactCommand.Execute(_client, update);
                                return Ok();
                            }
                        }
                        if (userData != null)
                        {
                            var cacheCommand = _worker.CacheCommands.SingleOrDefault(s => s.Type == userData.Type);
                            if (cacheCommand != null)
                                await cacheCommand.Execute(_client, update, userData);
                        }
                        break;
                    case UpdateType.CallbackQuery:
                        var callback = _worker.Callbacks.SingleOrDefault(s => s.Data.Contains(update.CallbackQuery.Data));
                        if (callback != null)
                            await callback.Execute(_client, update);
                        break;
                    case UpdateType.MyChatMember:
                        {
                            var chatMember = new StatusChatMember();
                            await chatMember.Execute(_client, update);
                            break;
                        }
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
