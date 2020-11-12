namespace Application.Requests.Chat
{
	public class BaseTelegramCommand
	{
		public BaseTelegramCommand(string syntax, string chatId, string command, string fullCommand)
		{
			Syntax = syntax;
			ChatId = chatId;
			Command = command;
			FullCommand = fullCommand;
		}

		protected string Syntax { get; }
		protected string ChatId { get; }
		protected string Command { get; }
		protected string FullCommand { get; }
	}
}