using Application.Services;
using Microsoft.ApplicationInsights;

namespace Application.Implementation
{
	public class TelemetryService : ITelemetryService
	{
		private readonly TelemetryClient _telemetryClient;

		public TelemetryService(TelemetryClient telemetryClient)
		{
			_telemetryClient = telemetryClient;
		}

		public void TrackChatCount(int count)
		{
			_telemetryClient.TrackMetric("ChatCount", count);
		}
	}
}