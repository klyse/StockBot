using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Services;
using MediatR;
using MongoDB.Driver;

namespace Application.Stock.Queries.GetChatIds
{
	public class GetChatsIdsQuery : IRequest<ChatIdsDto>
	{
		public class Handler : IRequestHandler<GetChatsIdsQuery, ChatIdsDto>
		{
			private readonly IStockDb _stockDb;

			public Handler(IStockDb stockDb)
			{
				_stockDb = stockDb;
			}

			public Task<ChatIdsDto> Handle(GetChatsIdsQuery request, CancellationToken cancellationToken)
			{
				var chatIds = _stockDb.Chats.AsQueryable()
					.Select(r => r.ChatId)
					.ToList();

				return Task.FromResult(new ChatIdsDto(chatIds));
			}
		}
	}
}