using System.Collections.Generic;

namespace Application.Requests.Stock.Queries.GetChatIds
{
	public class ChatIdsDto
	{
		public ChatIdsDto(List<string> ids)
		{
			Ids = ids;
		}

		public List<string> Ids { get; }
	}
}