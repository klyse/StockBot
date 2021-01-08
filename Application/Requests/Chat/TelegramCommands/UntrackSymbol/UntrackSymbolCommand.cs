using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Services;
using MediatR;
using MongoDB.Driver;

namespace Application.Requests.Chat.TelegramCommands.UntrackSymbol
{
	public class UntrackSymbolCommand : BaseTelegramCommand, IRequest
	{
		public UntrackSymbolCommand(string chatId, string command, string fullCommand) : base("/u {Symbol}", chatId, command, fullCommand)
		{
		}

		public class Handler : IRequestHandler<UntrackSymbolCommand, Unit>
		{
			private readonly IStockDb _stockDb;
			private readonly ITelegramService _telegramService;

			public Handler(IStockDb stockDb, ITelegramService telegramService)
			{
				_stockDb = stockDb;
				_telegramService = telegramService;
			}

			public async Task<Unit> Handle(UntrackSymbolCommand request, CancellationToken cancellationToken)
			{
				var symbol = request.Command.Split(' ').FirstOrDefault();

				if (string.IsNullOrWhiteSpace(symbol))
				{
					await _telegramService.SendMessageAsync(request.ChatId, $"Symbol name is empty.</br>Syntax: <i>{request.Syntax}</i>", cancellationToken);
					throw new InvalidTelegramCommand(request.Command);
				}

				symbol = symbol.ToUpperInvariant();

				_stockDb.Chats.FindOneAndUpdate<Domain.Chat>(r => r.ChatId == request.ChatId,
					new UpdateDefinitionBuilder<Domain.Chat>()
						.PullFilter(r => r.Symbols, q => q.Name == symbol)
						.SetOnInsert(r => r.ChatId, request.ChatId)
						.SetOnInsert(r => r.SignupDateTime, DateTime.UtcNow),
					new FindOneAndUpdateOptions<Domain.Chat, Domain.Chat>
					{
						IsUpsert = true
					},
					cancellationToken);

				await _telegramService.SendMessageAsync(request.ChatId, $"Stopped tracking <b>{symbol}</b>", cancellationToken);

				return Unit.Value;
			}
		}
	}
}