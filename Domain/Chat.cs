using System.Collections.Generic;
using MongoDB.Bson;

namespace Domain
{
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