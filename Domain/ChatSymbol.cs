using System;
using System.Collections.Generic;

namespace Domain
{
	public class ChatSymbol
	{
		public string Name { get; set; }
		public decimal PurchasePrice { get; set; }
	}

	public class Symbol
	{
		public Symbol()
		{
			PriceHistory = new Dictionary<DateTimeOffset, decimal>();
		}

		public string Name { get; set; }
		public decimal LastPrice { get; set; }
		public DateTimeOffset LastInfo { get; set; }

		public IDictionary<DateTimeOffset, decimal> PriceHistory { get; set; }
	}
}