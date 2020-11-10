using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Services;
using Application.Services.StockService;
using Domain;
using MediatR;
using MongoDB.Driver;

namespace Application.Stock.Command.UpdateSymbolsCommand
{
	public class UpdateSymbolsCommand : IRequest
	{
		public class Handler : IRequestHandler<UpdateSymbolsCommand, Unit>
		{
			private const int UpdateIntervalMin = 10;
			private const int KeepHistoryDays = 10;
			private readonly IStockDb _stockDb;
			private readonly IStockService _stockService;

			public Handler(IStockDb stockDb, IStockService stockService)
			{
				_stockDb = stockDb;
				_stockService = stockService;
			}

			public async Task<Unit> Handle(UpdateSymbolsCommand request, CancellationToken cancellationToken)
			{
				var symbols = _stockDb.Chats.AsQueryable()
					.SelectMany(r => r.Symbols)
					.GroupBy(r => r.Name)
					.Select(r => r.Key)
					.ToList();

				// update only symbols that are older than 5 min
				var upToDateSymbols = await _stockDb.Symbols.Find(r => r.LastInfo > DateTime.UtcNow.AddMinutes(-UpdateIntervalMin))
					.Project(r => r.Name)
					.ToListAsync(cancellationToken);

				symbols = symbols.Except(upToDateSymbols)
					.ToList();

				foreach (var symbol in symbols)
				{
					var symbolQuote = await _stockService.GetQuoteAsync(symbol);
					var now = DateTimeOffset.UtcNow;

					// update fields
					_stockDb.Symbols.FindOneAndUpdate<Symbol>(r => r.Name == symbol,
						new UpdateDefinitionBuilder<Symbol>()
							.AddToSet(r => r.PriceHistory, new SymbolPriceHistory
							{
								TimeStamp = now.UtcDateTime,
								Price = symbolQuote.Price
							})
							.Set(r => r.LastPrice, symbolQuote.Price)
							.Set(r => r.LastInfo, now.UtcDateTime)
							.SetOnInsert(r => r.Name, symbol),
						new FindOneAndUpdateOptions<Symbol, Symbol>
						{
							IsUpsert = true
						},
						cancellationToken
					);

					// remove old Price History
					_stockDb.Symbols.FindOneAndUpdate(r => r.Name == symbol,
						new UpdateDefinitionBuilder<Symbol>()
							.PullFilter(r => r.PriceHistory, r => r.TimeStamp < now.UtcDateTime.AddDays(-KeepHistoryDays)),
						cancellationToken: cancellationToken
					);
				}

				return Unit.Value;
			}
		}
	}
}