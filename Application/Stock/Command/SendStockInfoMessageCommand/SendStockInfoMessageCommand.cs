using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Services;
using Application.Services.StockService;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Application.Stock.Command.SendStockInfoMessageCommand
{
	public class SendStockInfoMessageCommand : IRequest
	{
		public SendStockInfoMessageCommand(string chatId)
		{
			ChatId = chatId;
		}

		public string ChatId { get; }

		public class Handler : IRequestHandler<SendStockInfoMessageCommand, Unit>
		{
			private readonly IConfigService _config;
			private readonly ILogger _logger;
			private readonly IStockDb _stockDb;
			private readonly IStockService _stockService;
			private readonly ITelegramService _telegramService;

			public Handler(IConfigService config, ITelegramService telegramService, ILogger logger, IStockService stockService, IStockDb stockDb)
			{
				_config = config;
				_telegramService = telegramService;
				_logger = logger;
				_stockService = stockService;
				_stockDb = stockDb;
			}

			public async Task<Unit> Handle(SendStockInfoMessageCommand request, CancellationToken cancellationToken)
			{
				var chat = await _stockDb.Chat.Aggregate()
					.Match(c => c.ChatId == request.ChatId)
					.FirstAsync(cancellationToken);

				if (chat is null)
					throw new NullReferenceException("Chat not found");

				var symbolsInfo = new List<Quote>();

				foreach (var symbol in chat.Symbols)
				{
					var quote = await _stockService.GetQuoteAsync(symbol.Name);

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