using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Exceptions;
using Application.Requests.Chat.Commands.StartTrackingSymbol;
using Application.Requests.Chat.Commands.StopTrackingSymbol;
using Application.Requests.Chat.Commands.UnsubscribeChat;
using Application.Services;
using MediatR;

namespace Application.Requests.Chat.Commands.TelegramCommand
{
	public class TelegramCommand : IRequest
	{
		private readonly string _chatId;
		private readonly string _command;

		public TelegramCommand(string chatId, string command)
		{
			_chatId = chatId;
			_command = command;
		}

		public class Handler : IRequestHandler<TelegramCommand, Unit>
		{
			private readonly IMediator _mediator;

			public Handler(IStockDb stockDb, IMediator mediator)
			{
				_mediator = mediator;
			}

			public async Task<Unit> Handle(TelegramCommand request, CancellationToken cancellationToken)
			{
				var splitCommand = request._command.Split(' ');

				var command = splitCommand.First();

				string cleanCommand = "";
				if (splitCommand.Length > 1)
					cleanCommand = string.Join(' ', splitCommand.Skip(1));


				IRequest? trackStatusParsed = command switch
				{
					"/t" => new StartTrackingSymbolCommand(request._chatId, cleanCommand, request._command),
					"/u" => new StopTrackingSymbolCommand(request._chatId, cleanCommand, request._command),
					"/unsubscribe" => new UnsubscribeChatCommand(request._chatId, cleanCommand, request._command),
					_ => null
				};

				if (trackStatusParsed is null)
					throw new InvalidTelegramCommand();

				await _mediator.Send(trackStatusParsed, cancellationToken);

				return Unit.Value;
			}
		}
	}
}