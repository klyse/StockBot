using System.Collections.Generic;
using MongoDB.Bson;

namespace Domain
{
	public class Symbol
	{
		public string Name { get; set; }
		public decimal PurchasePrice { get; set; }
	}

	public class Chat
	{
		public Chat()
		{
			Symbols = new List<Symbol>();
		}

		public ObjectId Id { get; set; }
		public string ChatId { get; set; }
		public ICollection<Symbol> Symbols { get; set; }
	}
}