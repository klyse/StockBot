using System.Collections.Generic;
using MongoDB.Bson;

namespace Domain
{
	public class Chat
	{
		public Chat()
		{
			Symbols = new List<ChatSymbol>();
		}

		public ObjectId Id { get; set; }
		public string ChatId { get; set; } = null!;
		public ICollection<ChatSymbol> Symbols { get; set; }
	}
}