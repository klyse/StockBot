using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain
{
	public class SymbolPriceHistory
	{
		[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		public DateTime TimeStamp { get; set; }

		public decimal Price { get; set; }
	}
}