namespace Application.Exceptions
{
	public class InvalidTelegramCommand : HttpException
	{
		public InvalidTelegramCommand() : base(500, "invalid telegram command")
		{
		}
	}
}