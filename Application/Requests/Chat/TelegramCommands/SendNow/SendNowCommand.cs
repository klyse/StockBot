using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Requests.Stock.Commands.SendStockInfoMessage;
using Application.Requests.Stock.Commands.UpdateSymbol;
using Application.Services;
using MediatR;
using MongoDB.Driver;

namespace Application.Requests.Chat.TelegramCommands.SendNow
{
	public class SendNowCommand : BaseTelegramCommand, IRequest
	{
		public SendNowCommand(string chatId, string command, string fullCommand) : base("/now", chatId, command, fullCommand)
		{
		}

		public class Handler : IRequestHandler<SendNowCommand, Unit>
		{
			private readonly IMediator _mediator;
			private readonly IStockDb _stockDb;

			public Handler(IStockDb stockDb, IMediator mediator)
			{
				_stockDb = stockDb;
				_mediator = mediator;
			}

			public async Task<Unit> Handle(SendNowCommand request, CancellationToken cancellationToken)
			{
				var chat = _stockDb.Chats.AsQueryable()
					.SingleOrDefault(r => r.ChatId == request.ChatId);

				if (chat is null)
					throw new ChatNotFoundException();

				foreach (var symbol in chat.Symbols)
					await _mediator.Send(new UpdateSymbolCommand(symbol.Name), cancellationToken);

				await _mediator.Send(new SendStockInfoMessageCommand(request.ChatId), cancellationToken);

				return Unit.Value;
			}
		}
	}
}