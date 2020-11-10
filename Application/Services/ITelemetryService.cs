namespace Application.Services
{
	public interface ITelemetryService
	{
		void TrackChatCount(int count);
	}
}