using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Services;
using MediatR;
using MongoDB.Driver;

namespace Application.Requests.Stock.Queries.GetSymbols
{
	public class GetSymbolsQuery : IRequest<SymbolsDto>
	{
		public class Handler : IRequestHandler<GetSymbolsQuery, SymbolsDto>
		{
			private readonly IStockDb _stockDb;

			public Handler(IStockDb stockDb)
			{
				_stockDb = stockDb;
			}

			public Task<SymbolsDto> Handle(GetSymbolsQuery request, CancellationToken cancellationToken)
			{
				var chatIds = _stockDb.Symbols.AsQueryable()
					.Select(r => r.Name)
					.ToList();

				if (chatIds is null)
					throw new NullReferenceException("symbols are null");

				return Task.FromResult(new SymbolsDto(chatIds));
			}
		}
	}
}