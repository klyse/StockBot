using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain
{
	public class ChatSymbol
	{
		public string Name { get; set; } = null!;

		[BsonRepresentation(BsonType.Decimal128)]
		public decimal PurchasePrice { get; set; }
	}
}