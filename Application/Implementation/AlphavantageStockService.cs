using System;
using System.Threading.Tasks;
using AlphaVantage.Net.Core.Client;
using AlphaVantage.Net.Stocks.Client;
using Application.Services;
using Application.Services.StockService;

namespace Application.Implementation
{
	public class AlphavantageStockService : IStockService, IDisposable
	{
		private readonly AlphaVantageClient? _alphavantageClient;
		private readonly StocksClient? _stockClient;

		public AlphavantageStockService(IConfigService config)
		{
			_alphavantageClient = new AlphaVantageClient(config.GetAlphavantageApiKey());
			_stockClient = _alphavantageClient.Stocks();
		}

		public void Dispose()
		{
			_alphavantageClient?.Dispose();
			_stockClient?.Dispose();
		}

		public async Task<Quote> GetQuoteAsync(string symbol)
		{
			if (_stockClient == null) throw new NullReferenceException("stock client not initialized");

			var quote = await _stockClient.GetGlobalQuoteAsync(symbol);

			if (quote is null)
				throw new NullReferenceException($"cannot get stock quote for symbol {symbol}");

			return new Quote(quote.Symbol,
				quote.OpeningPrice,
				quote.PreviousClosingPrice,
				quote.HighestPrice,
				quote.LowestPrice,
				quote.Price,
				quote.Volume,
				quote.LatestTradingDay.ToDateTimeUnspecified(),
				quote.Change,
				quote.ChangePercent);
		}
	}
}