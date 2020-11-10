using Domain;
using MongoDB.Driver;

namespace Application.Services
{
	public interface IStockDb
	{
		IMongoCollection<Chat> Chats { get; }
	}
}