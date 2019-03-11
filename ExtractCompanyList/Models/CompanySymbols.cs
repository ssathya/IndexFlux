using System;
using System.Collections.Generic;

namespace ExtractCompanyList.Models
{
	public class CompanySymbols
	{
		public List<Ticker> Tickers { get; set; }
	}

	public class Ticker
	{
		public string Symbol { get; set; }
		public string Name { get; set; }
		public DateTime Date { get; set; }
		public bool IsEnabled { get; set; }
		public string Type { get; set; }
		public string IexId { get; set; }
	}
}