using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Stock.Command.SendStockInfoMessageCommand;
using Application.Stock.Queries.GetChatIds;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;

namespace StockBot
{
	public class DailyStockSummary
	{
		private readonly IMediator _mediator;

		public DailyStockSummary(IMediator mediator)
		{
			_mediator = mediator;
		}

		[FunctionName("DailyStockSummary")]
		public async Task<string> DailyStockSummaryAsync([ActivityTrigger] string chatId, ILogger log)
		{
			await _mediator.Send(new SendStockInfoMessageCommand(chatId));
			return chatId;
		}

		[FunctionName("Orchestration_DailyStockSummary")]
		public async Task<IEnumerable<string>> Orchestration_DailyStockSummary(
			[OrchestrationTrigger] IDurableOrchestrationContext context,
			ILogger logger)
		{
			logger.LogInformation("Orchestrator");
			var chatIds = _mediator.Send(new GetChatsIds()).GetAwaiter().GetResult();

			var sentWithSuccess = new List<string>();

			foreach (var chatId in chatIds.Ids)
			{
				logger.LogInformation("DailyStockSummary {ChatId}", chatId);

				var activityResponse = await context.CallActivityAsync<string>("DailyStockSummary", chatId);
				sentWithSuccess.Add(activityResponse);

				logger.LogInformation("DailyStockSummary {ChatId}: done", chatId);
			}

			return sentWithSuccess;
		}

		[FunctionName("Start_DailyStockSummary")]
		public static async Task Start_DailyStockSummary([TimerTrigger("%Timers:DailyStockSummary%")]
			TimerInfo myTimer, ILogger log,
			[DurableClient] IDurableOrchestrationClient starter)
		{
			var instanceId = await starter.StartNewAsync("Orchestration_DailyStockSummary", null);
			log.LogInformation("Started {InstanceId}", instanceId);
		}
	}
}