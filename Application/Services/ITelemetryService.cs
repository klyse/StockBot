namespace Application.Services
{
	public interface ITelemetryService
	{
		void TrackChatCount(int count);
		void TrackTotalSymbols(int count);
		void TrackSymbolsToRefresh(int count);
		void TrackMediatrRequest(string requestName, double timeMs);
	}
}