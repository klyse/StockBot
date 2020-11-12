using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Services;
using MediatR;
using MongoDB.Driver;

namespace Application.Requests.Chat.TelegramCommands.GetEquities
{
	public class GetEquitiesCommand : BaseTelegramCommand, IRequest
	{
		public GetEquitiesCommand(string chatId, string command, string fullCommand) : base("/equities", chatId, command, fullCommand)
		{
		}

		public class Handler : IRequestHandler<GetEquitiesCommand, Unit>
		{
			private readonly IStockDb _stockDb;
			private readonly ITelegramService _telegramService;

			public Handler(IStockDb stockDb, ITelegramService telegramService)
			{
				_stockDb = stockDb;
				_telegramService = telegramService;
			}

			public async Task<Unit> Handle(GetEquitiesCommand request, CancellationToken cancellationToken)
			{
				var chat = _stockDb.Chats.AsQueryable()
					.SingleOrDefault(r => r.ChatId == request.ChatId);

				if (chat is null)
					throw new ChatNotFoundException();

				var stringBuilder = new StringBuilder();

				stringBuilder.AppendLine("Here are your equities🤑:");
				foreach (var dbSymbol in chat.Symbols)
					stringBuilder.AppendLine($"<b>{dbSymbol.Name}</b>: qty: <b>{dbSymbol.Quantity?.ToString() ?? "NotSet"}</b>, avg purchase price: <b>{dbSymbol.AvgPurchasePrice?.ToString() ?? "NotSet"}</b>");

				await _telegramService.SendMessageAsync(request.ChatId, stringBuilder.ToString(), cancellationToken);

				return Unit.Value;
			}
		}
	}
}