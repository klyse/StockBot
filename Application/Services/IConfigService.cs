namespace Application.Services
{
	public interface IConfigService
	{
		string GetTelegramChatId();
		string GetTelegramApiKey();
		string GetAlphavantageApiKey();
		string GetMongoDbConnectionString();
	}
}