using Application.Implementation;
using Application.Services;
using Application.Services.StockService;
using Application.Stock.Command.SendStockInfoMessageCommand;
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
			builder.Services.AddMediatR(typeof(SendStockInfoMessageCommand));
			builder.Services.AddScoped<IConfigService, Config>();
			builder.Services.AddScoped<ITelegramService, TelegramService>();
			builder.Services.AddScoped<IStockService, AlphavantageStockService>();
			builder.Services.AddScoped<IStockDb, StockDb>();
			builder.Services.AddScoped<ITelemetryService, TelemetryService>();
		}
	}
}