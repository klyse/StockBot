using System.IO;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Requests.Chat.Commands.TelegramCommand;
using Application.Services;
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
		private readonly IConfigService _configuration;
		private readonly IMediator _mediator;

		public Telegram(IMediator mediator, IConfigService configuration)
		{
			_mediator = mediator;
			_configuration = configuration;
		}

		[FunctionName("TelegramNotification")]
		public async Task<IActionResult> TelegramNotificationAsync(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)]
			HttpRequest req, ILogger log)
		{
			var content = await new StreamReader(req.Body).ReadToEndAsync();

			// just make sure the call is really from telegram
			string requestToken = req.Query["token"];
			var notificationToken = _configuration.GetTelegramNotificationToken();

			if (requestToken != notificationToken)
				throw new NotificationKeysDontMatch();

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