using System;
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
		public StartTrackingSymbolCommand(string chatId, string command, string fullCommand) : base(chatId, command, fullCommand)
		{
			var symbol = command.Split(' ').FirstOrDefault();

			if (string.IsNullOrWhiteSpace(symbol))
				throw new InvalidTelegramCommand();

			Symbol = symbol.ToUpperInvariant();
		}

		private string Symbol { get; }

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
				_stockDb.Chats.FindOneAndUpdate<Domain.Chat>(r => r.ChatId == request.ChatId,
					new UpdateDefinitionBuilder<Domain.Chat>()
						.AddToSet(r => r.Symbols,
							new ChatSymbol
							{
								Name = request.Symbol
							})
						.SetOnInsert(r => r.ChatId, request.ChatId)
						.SetOnInsert(r => r.SignupDateTime, DateTime.UtcNow),
					new FindOneAndUpdateOptions<Domain.Chat, Domain.Chat>
					{
						IsUpsert = true
					},
					cancellationToken);

				await _telegramService.SendMessageAsync(request.ChatId, $"Started tracking {request.Symbol} 🗠", cancellationToken);

				return Unit.Value;
			}
		}
	}
}