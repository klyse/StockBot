namespace Application.Exceptions
{
	public class ChatNotFoundException : NotFoundException
	{
		public ChatNotFoundException() : base("chat not found")
		{
		}
	}
}