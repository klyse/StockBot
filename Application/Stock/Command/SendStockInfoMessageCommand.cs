using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AlphaVantage.Net.Core.Client;
using AlphaVantage.Net.Stocks;
using AlphaVantage.Net.Stocks.Client;
using Application.Services;
using MediatR;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace Application.Stock.Command
{
	public class SendStockInfoMessageCommand : IRequest
	{
		public SendStockInfoMessageCommand(string[] symbols, string chatId)
		{
			Symbols = symbols;
			ChatId = chatId;
		}

		public string[] Symbols { get; }
		public string ChatId { get; }

		public class Handler : IRequestHandler<SendStockInfoMessageCommand, Unit>
		{
			private readonly IConfig _config;
			private readonly ILogger _logger;

			public Handler(IConfig config, ILogger logger)
			{
				_config = config;
				_logger = logger;
			}

			public async Task<Unit> Handle(SendStockInfoMessageCommand request, CancellationToken cancellationToken)
			{
				var apiKey = _config.Get("TelegramApiKey");
				var alphavantageApiKey = _config.Get("AlphavantageApiKey");

				if (string.IsNullOrWhiteSpace(apiKey))
					throw new Exception("invalid telegram api config");

				using var alphavantageClient = new AlphaVantageClient(alphavantageApiKey);
				using var stockClient = alphavantageClient.Stocks();
				var symbolsInfo = new List<GlobalQuote>();

				foreach (var symbol in request.Symbols.Take(5)) // take only 5 because the alphaVantage api throttles down to 5 requests per min.
				{
					var quote = await stockClient.GetGlobalQuoteAsync(symbol);
					if (quote is null)
					{
						_logger.LogWarning("Cannot get quote info for symbol {Symbol}", symbol);
						continue;
					}

					symbolsInfo.Add(quote);
				}

				var botClient = new TelegramBotClient(apiKey);

				var stringBuilder = new StringBuilder();
				stringBuilder.AppendLine($"Hey here is your daily 📅 stock price update 🌐");
				stringBuilder.AppendLine();

				foreach (var quote in symbolsInfo)
				{
					var emojiChart = quote.ChangePercent >= 0 ? "📈" : "📉";
					var emojiCircle = quote.ChangePercent >= 0 ? "🟢" : "🔴"; // green : red
					stringBuilder.AppendLine($"{quote.Symbol,-9} {emojiCircle}: {quote.ChangePercent:F2}% {emojiChart} ({quote.Price:F2}€)");
				}

				stringBuilder.AppendLine();
				stringBuilder.AppendLine("Hope its going good for you ☺️");

				await botClient.SendTextMessageAsync(request.ChatId, stringBuilder.ToString(), cancellationToken: cancellationToken);

				return Unit.Value;
			}
		}
	}
}