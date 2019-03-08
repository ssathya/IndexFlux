using Google.Cloud.Dialogflow.V2;
using IndexFlux.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IndexFlux.Utils
{
	public class ObtainStockQuotes
	{
		#region Private Fields		
		private const string baseUrl = @"https://api.iextrading.com/1.0/stock/symbol/batch?types=quote";

		private readonly ILogger _logger;
		private readonly WebhookRequest _webhookRequest;

		#endregion Private Fields


		#region Public Constructors

		public ObtainStockQuotes(WebhookRequest webhookRequest, ILogger log)
		{
			_logger = log;
			_webhookRequest = webhookRequest;
		}

		#endregion Public Constructors

		#region Internal Methods

		/// <summary>
		/// Gets the external quotes asynchronous.
		/// </summary>
		/// <returns></returns>
		internal async Task<WebhookResponse> GetExternalQuotesAsync()
		{
			var parameters = _webhookRequest.QueryResult.Parameters;
			var symbol = parameters.Fields["companyNames"].ToString().Replace("\"", "");
			var urlToUse = baseUrl.Replace("symbol", symbol);
			string outMsg = await GetMarketData(urlToUse);

			var returnValue = new WebhookResponse
			{
				FulfillmentText = outMsg.ToString()
			};
			return returnValue;
		}

		#endregion Internal Methods


		#region Private Methods

		private static async Task<string> GetMarketData(string urlToUse)
		{
			string data;
			using (var wc = new WebClient())
			{
				data = await wc.DownloadStringTaskAsync(urlToUse);
			}

			var stockRTD = JsonConvert.DeserializeObject<MarketQuote>(data).Quote;
			var outMsg = new StringBuilder();
			var offset = DateTimeOffset.FromUnixTimeMilliseconds(stockRTD.LatestUpdate);

			var reportingDate = offset.DateTime;
			var tzInfo = TimeZoneInfo.FindSystemTimeZoneById("US Eastern Standard Time");
			reportingDate = TimeZoneInfo.ConvertTimeFromUtc(reportingDate, tzInfo);
			outMsg.Append("As of ");
			outMsg.Append($"{reportingDate.ToString("MMMM dd, hh:mm tt")} EST ");
			outMsg.Append($"{stockRTD.CompanyName} traded ");
			outMsg.Append(stockRTD.Change >= 0 ? " up by " : " down by ");
			outMsg.Append($"{ Math.Abs(stockRTD.Change)} points.\n");
			outMsg.Append($"Its last trade was at {stockRTD.LatestPrice}.\n");
			outMsg.Append($"Days range was between {stockRTD.Low} and {stockRTD.High}.\n");
			return outMsg.ToString();
		}

		#endregion Private Methods
	}
}