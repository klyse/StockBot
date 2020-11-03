using System.Threading;
using System.Threading.Tasks;

namespace Application.Services
{
	public interface ITelegramService
	{
		Task SendMessageAsync(string chatId, string message, CancellationToken cancellationToken = default);
	}
}