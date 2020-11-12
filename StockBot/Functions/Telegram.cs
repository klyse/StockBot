using System.IO;
using System.Threading.Tasks;
using Application.Requests.Chat.Commands.TelegramCommand;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace StockBot.Functions
{
	public class Telegram
	{
		private readonly IMediator _mediator;

		public Telegram(IMediator mediator)
		{
			_mediator = mediator;
		}

		[FunctionName("TelegramUpdate")]
		public async Task<IActionResult> TelegramUpdateAsync(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
			HttpRequest req, ILogger log)
		{
			var content = await new StreamReader(req.Body).ReadToEndAsync();

			if (string.IsNullOrWhiteSpace(content))
				return new BadRequestObjectResult("invalid body");

			var update = JsonConvert.DeserializeObject<Update>(content);

			if (update is null)
				return new BadRequestObjectResult("invalid body");

			var chatId = update.Message.Chat.Id.ToString();
			var command = update.Message.Text;

			await _mediator.Send(new TelegramCommand(chatId, command));

			return new OkObjectResult("OK");
		}
	}
}