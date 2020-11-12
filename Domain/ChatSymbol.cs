using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain
{
	public class ChatSymbol
	{
		public string Name { get; set; } = "";

		[BsonRepresentation(BsonType.Decimal128)]
		public decimal? AvgPurchasePrice { get; set; }

		public int? Quantity { get; set; }
	}
}