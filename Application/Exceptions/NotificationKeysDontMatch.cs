namespace Application.Exceptions
{
	public class NotificationKeysDontMatch : HttpException
	{
		public NotificationKeysDontMatch() : base(400, "notification keys dont match")
		{
		}
	}
}