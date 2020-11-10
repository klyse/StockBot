using System;
using Application.Services;
using Domain;
using Microsoft.ApplicationInsights;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;

namespace Persistence
{
	public class StockDb : IStockDb
	{
		private readonly MongoClient _client;
		private readonly IMongoDatabase _db;
		private readonly TelemetryClient _telemetryClient;

		public StockDb(IConfigService config, TelemetryClient telemetryClient)
		{
			_telemetryClient = telemetryClient;
			var connectionString = config.GetMongoDbConnectionString();
			var mongoUrl = MongoUrl.Create(connectionString);
			var mongoClientSettings = MongoClientSettings.FromUrl(mongoUrl);

			mongoClientSettings.ClusterConfigurator = clusterConfigurator =>
			{
				clusterConfigurator.Subscribe<CommandSucceededEvent>(e =>
				{
					if (e.OperationId == null)
						return;

					_telemetryClient.TrackDependency("database", "mongodb", e.CommandName, DateTimeOffset.UtcNow.Subtract(e.Duration), e.Duration, true);
				});
				clusterConfigurator.Subscribe<CommandFailedEvent>(e => { _telemetryClient.TrackDependency("database", "mongodb", e.CommandName, DateTimeOffset.UtcNow.Subtract(e.Duration), e.Duration, false); });
			};

			_client = new MongoClient(mongoClientSettings);
			_db = _client.GetDatabase(mongoUrl.DatabaseName);

			Chats = _db.GetCollection<Chat>(nameof(Chat));
			Symbols = _db.GetCollection<Symbol>(nameof(Symbol));
		}

		public IMongoCollection<Chat> Chats { get; }
		public IMongoCollection<Symbol> Symbols { get; }
	}
}