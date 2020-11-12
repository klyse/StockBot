using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Services;
using Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Telegram.Bot.Exceptions;

namespace Application.Requests.Stock.Commands.SendStockInfoMessage
{
	public class SendStockInfoMessageCommand : IRequest
	{
		private readonly string _chatId;

		public SendStockInfoMessageCommand(string chatId)
		{
			_chatId = chatId;
		}

		public class Handler : IRequestHandler<SendStockInfoMessageCommand, Unit>
		{
			private const int MaxInformationAgeHours = 2;
			private readonly ILogger<Handler> _log;
			private readonly IStockDb _stockDb;
			private readonly ITelegramService _telegramService;

			public Handler(ITelegramService telegramService, IStockDb stockDb, ILogger<Handler> log)
			{
				_telegramService = telegramService;
				_stockDb = stockDb;
				_log = log;
			}

			public async Task<Unit> Handle(SendStockInfoMessageCommand request, CancellationToken cancellationToken)
			{
				var chat = await _stockDb.Chats.AsQueryable()
					.FirstOrDefaultAsync(c => c.ChatId == request._chatId, cancellationToken);

				if (chat is null)
					throw new NullReferenceException("chat not found");

				var symbolsInfo = (await _stockDb.Symbols.Aggregate()
						.Match(Builders<Symbol>.Filter.In(c => c.Name, chat.Symbols.Select(y => y.Name)))
						.Match(r => r.LastInfo > DateTime.UtcNow.AddHours(-MaxInformationAgeHours))
						.ToListAsync(cancellationToken))
					.ToDictionary(c => c.Name, c => c);

				var stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("Hey here is your 📅 stock update 🌐");
				stringBuilder.AppendLine();

				foreach (var symbol in chat.Symbols)
				{
					Symbol quote;
					if (symbolsInfo.ContainsKey(symbol.Name))
					{
						quote = symbolsInfo[symbol.Name];
					}
					else
					{
						stringBuilder.AppendLine($"Symbol not found: {symbol.Name}");
						continue;
					}

					var emojiChart = quote.LastChangePercent >= 0 ? "📈" : "📉";
					var emojiCircle = quote.LastChangePercent >= 0 ? "🟢" : "🔴"; // green : red
					stringBuilder.AppendLine($"{quote.Name,-9} {emojiCircle}: {quote.LastChangePercent:F2}% {emojiChart} ({quote.LastPrice:F2}€)");
				}

				stringBuilder.AppendLine();
				stringBuilder.AppendLine("Hope its going good for you ☺️");

				try
				{
					await _telegramService.SendMessageAsync(request._chatId, stringBuilder.ToString(), cancellationToken);
				}
				catch (ChatNotFoundException e)
				{
					await _stockDb.Chats.DeleteOneAsync(r => r.Id == chat.Id, cancellationToken);
					_log.LogError(e, "Chat wit id {ChatId} not found, removed it from db", request._chatId);
					throw;
				}

				return Unit.Value;
			}
		}
	}
}