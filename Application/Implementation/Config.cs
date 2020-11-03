using System;
using Application.Services;

namespace Application.Implementation
{
	public class Config : IConfigService
	{
		public string GetTelegramChatId()
		{
			return Get("TelegramChatId");
		}

		public string GetTelegramApiKey()
		{
			return Get("TelegramApiKey");
		}

		public string GetAlphavantageApiKey()
		{
			return Get("AlphavantageApiKey");
		}

		public string[] GetStockSymbols()
		{
			var stockSymbols = Get("StockSymbols");

			return stockSymbols.Split(',');
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