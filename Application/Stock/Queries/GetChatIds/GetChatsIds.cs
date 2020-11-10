using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Services;
using MediatR;
using MongoDB.Driver;

namespace Application.Stock.Queries.GetChatIds
{
	public class GetChatsIds : IRequest<ChatIdsDto>
	{
		public class Handler : IRequestHandler<GetChatsIds, ChatIdsDto>
		{
			private readonly IStockDb _stockDb;

			public Handler(IStockDb stockDb)
			{
				_stockDb = stockDb;
			}

			public async Task<ChatIdsDto> Handle(GetChatsIds request, CancellationToken cancellationToken)
			{
				var chatIds = await _stockDb.Chat.Aggregate()
					.Project(r => new {r.ChatId})
					.ToListAsync(cancellationToken);

				return new ChatIdsDto
				{
					Ids = chatIds.Select(r => r.ChatId).ToList()
				};
			}
		}
	}
}