using System;
using Application.Services;

namespace Application.Implementation
{
	public class Config : IConfig
	{
		public string Get(string key)
		{
			var envValue = Environment.GetEnvironmentVariable(key);

			if (string.IsNullOrWhiteSpace(envValue))
				throw new Exception($"empty configuration key {key}");

			return envValue;
		}
	}
}