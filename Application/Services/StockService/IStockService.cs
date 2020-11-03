using System.Threading.Tasks;

namespace Application.Services.StockService
{
	public interface IStockService
	{
		Task<Quote> GetQuoteAsync(string symbol);
	}
}