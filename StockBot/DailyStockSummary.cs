using System.Net.Http;
using System.Threading.Tasks;
using Application.Stock.Command.SendStockInfoMessageCommand;
using Application.Stock.Queries.GetChatIds;
using MediatR;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.Http;
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
		public async Task<string> DailyStockSummaryAsync([ActivityTrigger] string? chatId, ILogger log)
		{
			log.LogInformation("Func");

			await _mediator.Send(new SendStockInfoMessageCommand(chatId));
			return "Ok";
		}

		[FunctionName("Orchestration_DailyStockSummary")]
		public async Task<string> Orchestration_DailyStockSummary(
			[OrchestrationTrigger] IDurableOrchestrationContext context,
			ILogger logger)
		{
			logger.LogInformation("Orchestrator");
			var chatIds = await _mediator.Send(new GetChatsIds());

			foreach (var chatId in chatIds.Ids)
			{
				logger.LogInformation("DailyStockSummary {ChatId}", chatId);

				await context.CallActivityAsync<string?>("DailyStockSummary", chatId);
				logger.LogInformation("DailyStockSummary {ChatId}: done", chatId);
			}

			return "ok";
		}

		[FunctionName("Start_DailyStockSummary")]
		public static async Task Start_DailyStockSummary([TimerTrigger("%Timers:DailyStockSummary%")]
			TimerInfo myTimer, ILogger log,
			[DurableClient] IDurableOrchestrationClient starter)
		{
			var instanceId = await starter.StartNewAsync("Orchestration_DailyStockSummary", null);

			log.LogInformation("Started {InstanceId}", instanceId);
		}
		
		
		[FunctionName("Http_DailyStockSummary")]
		public static async Task<HttpResponseMessage> HttpStart(
			[HttpTrigger(AuthorizationLevel.Anonymous, "get", "post")]
			HttpRequestMessage req,
			[DurableClient] IDurableOrchestrationClient starter,
			ILogger log)
		{
			// Function input comes from the request content.
			string instanceId = await starter.StartNewAsync("Orchestration_DailyStockSummary", null);

			log.LogInformation($"Started orchestration with ID = '{instanceId}'.");

			return starter.CreateCheckStatusResponse(req, instanceId);
		}
	}
}