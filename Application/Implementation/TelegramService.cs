using System.Threading;
using System.Threading.Tasks;
using Application.Services;
using Telegram.Bot;

namespace Application.Implementation
{
	public class TelegramService : ITelegramService
	{
		private readonly TelegramBotClient _botClient;

		public TelegramService(IConfigService config)
		{
			_botClient = new TelegramBotClient(config.GetTelegramApiKey());
		}

		public async Task SendMessageAsync(string chatId, string message, CancellationToken cancellationToken = default)
		{
			await _botClient.SendTextMessageAsync(chatId, message, cancellationToken: cancellationToken);
		}
	}
}