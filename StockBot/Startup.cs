using Application.Implementation;
using Application.Requests.Stock.Commands.SendStockInfoMessage;
using Application.Services;
using Application.Services.StockService;
using MediatR;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Persistence;
using StockBot;

[assembly: FunctionsStartup(typeof(Startup))]

namespace StockBot
{
	public class Startup : FunctionsStartup
	{
		public override void Configure(IFunctionsHostBuilder builder)
		{
			builder.Services.AddLogging();
			builder.Services.AddMediatR(typeof(SendStockInfoMessageCommand));
			builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(MediatrTelemetryCollectorBehavior<,>));
			builder.Services.AddScoped<IConfigService, Config>();
			builder.Services.AddScoped<ITelegramService, TelegramService>();
			builder.Services.AddScoped<IStockService, AlphavantageStockService>();
			builder.Services.AddScoped<IStockDb, StockDb>();
			builder.Services.AddScoped<ITelemetryService, TelemetryService>();
		}
	}
}