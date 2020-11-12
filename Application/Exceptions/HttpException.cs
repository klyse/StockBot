using System;

namespace Application.Exceptions
{
	public class HttpException : Exception
	{
		public HttpException()
		{
		}

		public HttpException(string message) : base(message)
		{
		}

		public HttpException(int httpCode, string message) : base(message)
		{
			HttpCode = httpCode;
		}

		public int HttpCode { get; }
	}
}