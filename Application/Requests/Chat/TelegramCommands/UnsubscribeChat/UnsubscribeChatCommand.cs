using System.Threading;
using System.Threading.Tasks;
using Application.Services;
using MediatR;
using MongoDB.Driver;

namespace Application.Requests.Chat.TelegramCommands.UnsubscribeChat
{
	public class UnsubscribeChatCommand : BaseTelegramCommand, IRequest
	{
		public UnsubscribeChatCommand(string chatId, string command, string fullCommand) : base(chatId, command, fullCommand)
		{
		}

		public class Handler : IRequestHandler<UnsubscribeChatCommand, Unit>
		{
			private readonly IStockDb _stockDb;
			private readonly ITelegramService _telegramService;

			public Handler(IStockDb stockDb, ITelegramService telegramService)
			{
				_stockDb = stockDb;
				_telegramService = telegramService;
			}

			public async Task<Unit> Handle(UnsubscribeChatCommand request, CancellationToken cancellationToken)
			{
				await _stockDb.Chats.DeleteOneAsync(r => r.ChatId == request.ChatId, cancellationToken);

				await _telegramService.SendMessageAsync(request.ChatId, "Successfully unsubscribed. Were sorry to see you go 😭", cancellationToken);

				return Unit.Value;
			}
		}
	}
}