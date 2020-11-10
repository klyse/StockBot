using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Services;
using Application.Stock.Command.SendStockInfoMessageCommand;
using Application.Stock.Command.UpdateSymbolsCommand;
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
		private readonly ITelemetryService _telemetry;

		public DailyStockSummary(IMediator mediator, ITelemetryService telemetry)
		{
			_mediator = mediator;
			_telemetry = telemetry;
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
			var chatIds = _mediator.Send(new GetChatsIdsQuery()).GetAwaiter().GetResult();

			var sentWithSuccess = new List<string>();

			_telemetry.TrackChatCount(chatIds.Ids.Count);

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
		public async Task Start_DailyStockSummary([TimerTrigger("%Timers:DailyStockSummary%")]
			TimerInfo myTimer, ILogger log,
			[DurableClient] IDurableOrchestrationClient starter)
		{
			await _mediator.Send(new UpdateSymbolsCommand());

			var instanceId = await starter.StartNewAsync("Orchestration_DailyStockSummary", null);
			log.LogInformation("Started {InstanceId}", instanceId);
		}
	}
}