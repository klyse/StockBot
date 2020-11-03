using System;

namespace Application.Services.StockService
{
	public class Quote
	{
		public Quote(string symbol, decimal openingPrice, decimal previousClosingPrice, decimal highestPrice, decimal lowestPrice, decimal price, long volume, DateTime latestTradingDay, decimal change, decimal changePercent)
		{
			Symbol = symbol;
			OpeningPrice = openingPrice;
			PreviousClosingPrice = previousClosingPrice;
			HighestPrice = highestPrice;
			LowestPrice = lowestPrice;
			Price = price;
			Volume = volume;
			LatestTradingDay = latestTradingDay;
			Change = change;
			ChangePercent = changePercent;
		}

		public string Symbol { get; }
		public decimal OpeningPrice { get; }
		public decimal PreviousClosingPrice { get; }
		public decimal HighestPrice { get; }
		public decimal LowestPrice { get; }
		public decimal Price { get; }
		public long Volume { get; }
		public DateTime LatestTradingDay { get; }
		public decimal Change { get; }
		public decimal ChangePercent { get; }
	}
}