using Application.Implementation;
using Application.Services;
using Application.Stock.Command;
using MediatR;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using StockBot;

[assembly: FunctionsStartup(typeof(Startup))]

namespace StockBot
{
	public class Startup : FunctionsStartup
	{
		public override void Configure(IFunctionsHostBuilder builder)
		{
			builder.Services.AddMediatR(typeof(SendStockInfoMessageCommand));
			builder.Services.AddScoped<IConfig, Config>();
		}
	}
}