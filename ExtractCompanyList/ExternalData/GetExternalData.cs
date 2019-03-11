using ExtractCompanyList.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ExtractCompanyList.ExternalData
{
    public class GetExternalData
    {
		public CompanySymbols CompanySymbols { get; set; }
		public GetExternalData()
		{
			CompanySymbols = new CompanySymbols();
		}
		public async Task<CompanySymbols> GetDataFromExternalSource()
		{
			var urlToUse = @"https://api.iextrading.com/1.0/ref-data/symbols";
			var data = "{}";
			try
			{
				using (var wc = new WebClient())
				{
					data = await wc.DownloadStringTaskAsync(urlToUse);
				}
					
				var dataArray = JsonConvert.DeserializeObject<IEnumerable<Ticker>>(data);
				
				CompanySymbols.Tickers = dataArray.Where(
					da => (da.Type.Equals("cs") ||
					da.Type.Equals("et")) && 
					da.IsEnabled).ToList();
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Excepition when running GetDataFromExternalSource\n{ex.Message}");
				if (ex.InnerException != null)
				{
					Console.WriteLine(ex.InnerException.Message);
				}
			}
			return CompanySymbols;			
		}
    }
}
