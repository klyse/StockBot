namespace Application.Exceptions
{
	public class NotFoundException : HttpException
	{
		public NotFoundException(string message) : base(404, message)
		{
		}
	}
}