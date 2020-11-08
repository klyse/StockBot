using System.Threading.Tasks;
using Application.Services;
using Application.Stock.Command.SendStockInfoMessageCommand;
using Application.Stock.Queries.GetChatIds;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace StockBot
{
	public class DailyStockSummary
	{
		private readonly IConfigService _config;
		private readonly IMediator _mediator;

		public DailyStockSummary(IMediator mediator, IConfigService config)
		{
			_mediator = mediator;
			_config = config;
		}

		[FunctionName("DailyStockSummary")]
		public async Task RunAsync([TimerTrigger("%Timers:DailyStockSummary%")]
			TimerInfo myTimer, ILogger log)
		{
			var chatIds = await _mediator.Send(new GetChatsIds());

			foreach (var id in chatIds.Ids) await _mediator.Send(new SendStockInfoMessageCommand(id));
		}
	}
}