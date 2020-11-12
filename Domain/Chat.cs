using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain
{
	public class Chat
	{
		public Chat()
		{
			Symbols = new List<ChatSymbol>();
		}

		public ObjectId Id { get; set; }
		public string ChatId { get; set; } = "";

		[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		public DateTime SignupDateTime { get; set; } = DateTime.MinValue;

		public ICollection<ChatSymbol> Symbols { get; set; }
	}
}