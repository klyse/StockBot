using System.Collections.Generic;
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

		public void TrackTotalSymbols(int count)
		{
			_telemetryClient.TrackMetric("TotalSymbolsCount", count);
		}

		public void TrackSymbolsToRefresh(int count)
		{
			_telemetryClient.TrackMetric("SymbolsToRefresh", count);
		}

		public void TrackMediatrRequest(string requestName, double timeMs)
		{
			_telemetryClient.TrackEvent(requestName, metrics: new Dictionary<string, double>
			{
				{"executionTime", timeMs}
			});
		}
	}
}