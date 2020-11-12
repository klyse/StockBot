using System;
using Application.Services;

namespace Application.Implementation
{
	public class Config : IConfigService
	{
		public string GetTelegramApiKey()
		{
			return Get("Telegram:ApiKey");
		}

		public string GetAlphavantageApiKey()
		{
			return Get("AlphavantageApiKey");
		}

		public string GetMongoDbConnectionString()
		{
			return Get("ConnectionStrings:MongoDb");
		}

		public string GetTelegramNotificationToken()
		{
			return Get("Telegram:NotificationToken");
		}

		private string Get(string key)
		{
			var envValue = Environment.GetEnvironmentVariable(key);

			if (string.IsNullOrWhiteSpace(envValue))
				throw new Exception($"empty configuration key {key}");

			return envValue;
		}
	}
}