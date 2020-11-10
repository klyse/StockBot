using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain
{
	public class SymbolPriceHistory
	{
		[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		public DateTime TimeStamp { get; set; }

		[BsonRepresentation(BsonType.Decimal128)]
		public decimal Price { get; set; }
	}
}