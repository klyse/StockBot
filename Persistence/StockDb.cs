using Application.Services;
using Domain;
using MongoDB.Driver;

namespace Persistence
{
	public class StockDb : IStockDb
	{
		private readonly MongoClient _client;
		private readonly IMongoDatabase _db;

		public StockDb(IConfigService config)
		{
			var connectionString = config.GetMongoDbConnectionString();
			_client = new MongoClient(connectionString);
			var databaseName = MongoUrl.Create(connectionString).DatabaseName;
			_db = _client.GetDatabase(databaseName);

			Chats = _db.GetCollection<Chat>(nameof(Chat));
		}

		public IMongoCollection<Chat> Chats { get; }
	}
}