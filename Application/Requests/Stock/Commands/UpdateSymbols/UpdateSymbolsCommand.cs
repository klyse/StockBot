using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Requests.Stock.Commands.UpdateSymbol;
using Application.Services;
using MediatR;
using MongoDB.Driver;

namespace Application.Requests.Stock.Commands.UpdateSymbols
{
	public class UpdateSymbolsCommand : IRequest
	{
		public class Handler : IRequestHandler<UpdateSymbolsCommand, Unit>
		{
			private const int UpdateIntervalMin = 10;
			private readonly IMediator _mediator;
			private readonly IStockDb _stockDb;
			private readonly ITelemetryService _telemetryService;

			public Handler(IStockDb stockDb, ITelemetryService telemetryService, IMediator mediator)
			{
				_stockDb = stockDb;
				_telemetryService = telemetryService;
				_mediator = mediator;
			}

			public async Task<Unit> Handle(UpdateSymbolsCommand request, CancellationToken cancellationToken)
			{
				var symbols = _stockDb.Chats.AsQueryable()
					.SelectMany(r => r.Symbols)
					.GroupBy(r => r.Name.ToUpperInvariant())
					.Select(r => r.Key)
					.ToList();

				_telemetryService.TrackTotalSymbols(symbols.Count);

				// update only symbols that are older than 5 min
				var upToDateSymbols = await _stockDb.Symbols.Find(r => r.LastInfo > DateTime.UtcNow.AddMinutes(-UpdateIntervalMin))
					.Project(r => r.Name.ToUpperInvariant())
					.ToListAsync(cancellationToken);

				symbols = symbols.Except(upToDateSymbols)
					.ToList();

				_telemetryService.TrackSymbolsToRefresh(symbols.Count);

				foreach (var symbol in symbols) await _mediator.Send(new UpdateSymbolCommand(symbol), cancellationToken);

				return Unit.Value;
			}
		}
	}
}