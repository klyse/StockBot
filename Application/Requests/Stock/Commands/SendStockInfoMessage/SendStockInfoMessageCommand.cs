using System;
using System.Collections.Generic;
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
			private const int MaxInformationAgeHours = 1;
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

				decimal totalCurrentValue = 0;
				decimal totalInvestedValue = 0;

				var todaysStockStringList = new List<string>();
				var totalsStockStringList = new List<string>();

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

					var todaysString = GetTodaysString(symbol.Name, quote.LastChangePercent, quote.LastPrice, symbol.AvgPurchasePrice, symbol.Quantity, out var currentValue, out var investedValue, out var capitalGain, out var capitalGainPercent);
					todaysStockStringList.Add(todaysString);

					var totalsString = GetTotalsString(symbol.Name, symbol.AvgPurchasePrice, symbol.Quantity, currentValue, investedValue, capitalGain, capitalGainPercent);
					totalsStockStringList.Add(totalsString);

					if (investedValue.HasValue)
						totalInvestedValue += investedValue.Value;
					if (currentValue.HasValue)
						totalCurrentValue += currentValue.Value;
				}

				foreach (var builder in todaysStockStringList)
					stringBuilder.AppendLine(builder);

				stringBuilder.AppendLine();
				stringBuilder.AppendLine("Totals 💵");

				foreach (var builder in totalsStockStringList)
					stringBuilder.AppendLine(builder);

				stringBuilder.AppendLine();

				var valueGain = totalCurrentValue - totalInvestedValue;
				var valueGainPercent = valueGain / totalInvestedValue * 100;
				var totalChart = valueGainPercent >= 0 ? "📈" : "📉";
				stringBuilder.AppendLine($"Capital gain: <b>{valueGainPercent:N2}</b>%{totalChart} <b>{valueGain:N2}</b>€");

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

			private static string GetTodaysString(string symbol, decimal lastChangePercent, decimal lastPrice, decimal? avgPurchaseValue, decimal? quantity,
				out decimal? currentValue, out decimal? investedValue, out decimal? capitalGain, out decimal? capitalGainPercent)
			{
				capitalGain = null;
				capitalGainPercent = null;
				investedValue = null;
				currentValue = null;

				var emojiCircle = lastChangePercent >= 0 ? "🟢" : "🔴";
				var emojiChart = lastChangePercent >= 0 ? "📈" : "📉";

				string str = $"{symbol}: {emojiCircle}{lastChangePercent:N2}%:{emojiChart} <b>{lastPrice:N2}</b>€";

				if (avgPurchaseValue is {} && quantity is {})
				{
					currentValue = lastPrice * quantity;
					investedValue = avgPurchaseValue * quantity;
					capitalGain = currentValue - investedValue;
					capitalGainPercent = capitalGain / investedValue * 100;
				}

				return str;
			}

			private static string GetTotalsString(string symbol, decimal? avgPurchaseValue, decimal? quantity,
				decimal? currentValue, decimal? investedValue, decimal? capitalGain, decimal? capitalGainPercent)
			{
				var str = "";

				var emojiCircle = capitalGainPercent >= 0 ? "🟢" : "🔴";
				var emojiChart = capitalGainPercent >= 0 ? "📈" : "📉";

				if (avgPurchaseValue.HasValue && quantity.HasValue && currentValue.HasValue && investedValue.HasValue && capitalGain.HasValue && capitalGainPercent.HasValue)
					str = $"{symbol}: {emojiCircle}{capitalGainPercent:N2}%:{emojiChart} <b>{capitalGain:N2}</b>€";

				return str;
			}
		}
	}
}