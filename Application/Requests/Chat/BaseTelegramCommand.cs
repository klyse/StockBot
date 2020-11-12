namespace Application.Requests.Chat
{
	public class BaseTelegramCommand
	{
		public BaseTelegramCommand(string chatId, string command, string fullCommand)
		{
			ChatId = chatId;
			Command = command;
			FullCommand = fullCommand;
		}

		protected string ChatId { get; }
		protected string Command { get; }
		protected string FullCommand { get; }
	}
}