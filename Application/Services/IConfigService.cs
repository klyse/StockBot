namespace Application.Services
{
	public interface IConfigService
	{
		string GetTelegramNotificationToken();
		string GetTelegramApiKey();
		string GetAlphavantageApiKey();
		string GetMongoDbConnectionString();
	}
}