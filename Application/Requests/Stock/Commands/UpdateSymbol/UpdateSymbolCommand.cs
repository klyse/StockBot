using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Services;
using Application.Services.StockService;
using Domain;
using MediatR;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;

namespace Application.Requests.Stock.Commands.UpdateSymbol
{
	public class UpdateSymbolCommand : IRequest
	{
		public UpdateSymbolCommand(string name)
		{
			Name = name;
		}

		public string Name { get; }

		public class Handler : IRequestHandler<UpdateSymbolCommand, Unit>
		{
			private const int KeepHistoryDays = 10;
			private readonly ILogger<UpdateSymbolCommand> _log;
			private readonly IStockDb _stockDb;
			private readonly IStockService _stockService;

			public Handler(IStockDb stockDb, IStockService stockService, ILogger<UpdateSymbolCommand> log)
			{
				_stockDb = stockDb;
				_stockService = stockService;
				_log = log;
			}

			public async Task<Unit> Handle(UpdateSymbolCommand request, CancellationToken cancellationToken)
			{
				Quote symbolQuote;
				try
				{
					symbolQuote = await _stockService.GetQuoteAsync(request.Name);
				}
				catch (Exception e)
				{
					_log.LogError(e, "Cannot get quote for symbol {Name}", request.Name);
					return Unit.Value;
				}

				var now = DateTimeOffset.UtcNow;

				// update fields
				_stockDb.Symbols.FindOneAndUpdate<Symbol>(r => r.Name == request.Name,
					new UpdateDefinitionBuilder<Symbol>()
						.AddToSet(r => r.PriceHistory, new SymbolPriceHistory
						{
							TimeStamp = now.UtcDateTime,
							Price = symbolQuote.Price
						})
						.Set(r => r.LastPrice, symbolQuote.Price)
						.Set(r => r.LastChangePercent, symbolQuote.ChangePercent)
						.Set(r => r.LastInfo, now.UtcDateTime)
						.SetOnInsert(r => r.Name, request.Name),
					new FindOneAndUpdateOptions<Symbol, Symbol>
					{
						IsUpsert = true
					},
					cancellationToken
				);

				// remove old Price History
				_stockDb.Symbols.FindOneAndUpdate(r => r.Name == request.Name,
					new UpdateDefinitionBuilder<Symbol>()
						.PullFilter(r => r.PriceHistory, r => r.TimeStamp < now.UtcDateTime.AddDays(-KeepHistoryDays)),
					cancellationToken: cancellationToken
				);

				return Unit.Value;
			}
		}
	}
}