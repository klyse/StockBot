using System;
using System.Threading.Tasks;
using Application.Services;
using Application.Stock.Command;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace StockBot
{
	public class DailyStockSummary
	{
		private readonly IConfig _config;
		private readonly IMediator _mediator;

		public DailyStockSummary(IMediator mediator, IConfig config)
		{
			_mediator = mediator;
			_config = config;
		}

		[FunctionName("DailyStockSummary")]
		public async Task RunAsync([TimerTrigger("%Timers:DailyStockSummary%")] TimerInfo myTimer, ILogger log)
		{
			log.LogInformation("Trigger function started execution");

			await _mediator.Send(new SendStockInfoMessageCommand(
				_config.Get("StockSymbols").Split(','),
				_config.Get("TelegramChatId")));

			log.LogInformation("Trigger function completed execution");
		}
	}
}