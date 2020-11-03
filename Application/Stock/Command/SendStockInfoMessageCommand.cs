using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Services;
using Application.Services.StockService;
using MediatR;
using Microsoft.Extensions.Logging;

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
			private readonly IConfigService _config;
			private readonly ILogger _logger;
			private readonly IStockService _stockService;
			private readonly ITelegramService _telegramService;

			public Handler(IConfigService config, ITelegramService telegramService, ILogger logger, IStockService stockService)
			{
				_config = config;
				_telegramService = telegramService;
				_logger = logger;
				_stockService = stockService;
			}

			public async Task<Unit> Handle(SendStockInfoMessageCommand request, CancellationToken cancellationToken)
			{
				var symbolsInfo = new List<Quote>();

				foreach (var symbol in request.Symbols.Take(5)) // take only 5 because the alphaVantage api throttles down to 5 requests per min.
				{
					var quote = await _stockService.GetQuoteAsync(symbol);

					symbolsInfo.Add(quote);
				}

				var stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("Hey here is your daily 📅 stock price update 🌐");
				stringBuilder.AppendLine();

				foreach (var quote in symbolsInfo)
				{
					var emojiChart = quote.ChangePercent >= 0 ? "📈" : "📉";
					var emojiCircle = quote.ChangePercent >= 0 ? "🟢" : "🔴"; // green : red
					stringBuilder.AppendLine($"{quote.Symbol,-9} {emojiCircle}: {quote.ChangePercent:F2}% {emojiChart} ({quote.Price:F2}€)");
				}

				stringBuilder.AppendLine();
				stringBuilder.AppendLine("Hope its going good for you ☺️");

				await _telegramService.SendMessageAsync(request.ChatId, stringBuilder.ToString(), cancellationToken);

				return Unit.Value;
			}
		}
	}
}