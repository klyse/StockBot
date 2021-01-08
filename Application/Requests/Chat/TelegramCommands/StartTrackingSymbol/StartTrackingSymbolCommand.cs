using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Services;
using Domain;
using MediatR;
using MongoDB.Driver;

namespace Application.Requests.Chat.TelegramCommands.StartTrackingSymbol
{
	public class StartTrackingSymbolCommand : BaseTelegramCommand, IRequest
	{
		public StartTrackingSymbolCommand(string chatId, string command, string fullCommand) : base("/t {Symbol} {Quantity:optional} {AvgPurchasePrice:optional}", chatId, command, fullCommand)
		{
		}

		public class Handler : IRequestHandler<StartTrackingSymbolCommand, Unit>
		{
			private readonly IStockDb _stockDb;
			private readonly ITelegramService _telegramService;

			public Handler(IStockDb stockDb, ITelegramService telegramService)
			{
				_stockDb = stockDb;
				_telegramService = telegramService;
			}

			public async Task<Unit> Handle(StartTrackingSymbolCommand request, CancellationToken cancellationToken)
			{
				var splitCommand = request.Command.Split(' ');

				if (!GetCommand(splitCommand, 0, out var symbol))
				{
					await _telegramService.SendMessageAsync(request.ChatId, $"Symbol name is empty. Syntax: <i>{request.Syntax}</i>", cancellationToken);
					throw new InvalidTelegramCommand(request.Command);
				}

				int? quantity = null;
				if (GetCommand(splitCommand, 1, out var quantityStr))
				{
					if (!string.IsNullOrWhiteSpace(quantityStr) && int.TryParse(quantityStr, out var tmpQuantity))
					{
						quantity = tmpQuantity;
					}
					else
					{
						await _telegramService.SendMessageAsync(request.ChatId, $"Cannot parse Quantity (optional). Syntax: {request.Syntax}", cancellationToken);
						throw new InvalidTelegramCommand(request.Command);
					}
				}

				decimal? avgPurchasePrice = null;
				if (GetCommand(splitCommand, 2, out var avgPurchasePriceStr))
				{
					if (!string.IsNullOrWhiteSpace(avgPurchasePriceStr) && int.TryParse(avgPurchasePriceStr, out var tmpAvgPurchasePrice))
					{
						avgPurchasePrice = tmpAvgPurchasePrice;
					}
					else
					{
						await _telegramService.SendMessageAsync(request.ChatId, $"Cannot parse Average Purchase Price (optional). Syntax: {request.Syntax}", cancellationToken);
						throw new InvalidTelegramCommand(request.Command);
					}
				}

				symbol = symbol.ToUpperInvariant();

				_stockDb.Chats.FindOneAndUpdate<Domain.Chat>(r => r.ChatId == request.ChatId,
					new UpdateDefinitionBuilder<Domain.Chat>()
						.AddToSet(r => r.Symbols,
							new ChatSymbol
							{
								Name = symbol,
								Quantity = quantity,
								AvgPurchasePrice = avgPurchasePrice
							})
						.SetOnInsert(r => r.ChatId, request.ChatId)
						.SetOnInsert(r => r.SignupDateTime, DateTime.UtcNow),
					new FindOneAndUpdateOptions<Domain.Chat, Domain.Chat>
					{
						IsUpsert = true
					},
					cancellationToken);

				await _telegramService.SendMessageAsync(request.ChatId, $"Started tracking <b>{symbol}</b> 📇", cancellationToken);

				return Unit.Value;
			}

			private static bool GetCommand(IReadOnlyCollection<string> command, int pos, out string symbol)
			{
				symbol = "";
				if (command.Count < pos + 1) return false;

				symbol = command.ElementAt(pos);

				return !string.IsNullOrWhiteSpace(symbol);
			}
		}
	}
}