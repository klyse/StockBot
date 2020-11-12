using System.Collections.Generic;

namespace Application.Requests.Stock.Queries.GetSymbols
{
	public class SymbolsDto
	{
		public SymbolsDto(List<string> symbols)
		{
			Symbols = symbols;
		}

		public List<string> Symbols { get; }
	}
}