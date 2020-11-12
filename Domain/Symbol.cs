using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain
{
	public class Symbol
	{
		public Symbol()
		{
			PriceHistory = new List<SymbolPriceHistory>();
		}

		public ObjectId Id { get; set; }
		public string Name { get; set; } = "";

		[BsonRepresentation(BsonType.Decimal128)]
		public decimal LastPrice { get; set; } = 0;

		[BsonRepresentation(BsonType.Decimal128)]
		public decimal LastChangePercent { get; set; } = 0;

		[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		public DateTime LastInfo { get; set; } = DateTime.MinValue;

		public List<SymbolPriceHistory> PriceHistory { get; set; }
	}
}