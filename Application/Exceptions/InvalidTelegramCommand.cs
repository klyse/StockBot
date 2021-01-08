namespace Application.Exceptions
{
	public class InvalidTelegramCommand : HttpException
	{
		public InvalidTelegramCommand(string command) : base(500, $"invalid telegram command {command}")
		{
		}
	}
}