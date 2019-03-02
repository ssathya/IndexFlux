using Google.Cloud.Dialogflow.V2;
using IndexFlux.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IndexFlux.Utils
{
	public class ObtainTrenders
	{

		#region Private Fields

		private const string baseUrl = @"https://api.iextrading.com/1.0/stock/market/list/";
		private const string EntityTrendParameter = "trendParameter";
		private const string TokenPath = @"$.*.parameters";
		private readonly ILogger _logger;

		#endregion Private Fields


		#region Public Constructors

		public ObtainTrenders(ILogger log)
		{
			_logger = log;
		}

		#endregion Public Constructors


		#region Internal Methods

		internal async Task<WebhookResponse> GetTrendingAsync(JObject parserResult)
		{
			var parameters = parserResult.SelectToken(TokenPath);
			var paramValues =
				((JObject)parameters).ToObject<Dictionary<string, string>>();
			var requestAction = paramValues[EntityTrendParameter];
			string urlToUse = "";
			urlToUse = BuildUrlToUse(requestAction, urlToUse, out string readableParameter);
			if (string.IsNullOrWhiteSpace(urlToUse))
			{
				return new WebhookResponse
				{
					FulfillmentText = Utilities.ErrorReturnMsg()
				};
			}
			string outputMsg = await GetIexTradingDataAsync(urlToUse, readableParameter);
			return new WebhookResponse
			{
				FulfillmentText = outputMsg
			};
		}

		#endregion Internal Methods


		#region Private Methods

		private string BuildUrlToUse(string requestAction, string urlToUse, out string readableParameter)
		{
			switch (requestAction.ToLower())
			{
				case "mostactive":
					urlToUse = baseUrl + "mostactive";
					readableParameter = "Most active ";
					break;

				case "gainers":
					urlToUse = baseUrl + "gainers";
					readableParameter = "Top gainers ";
					break;

				case "losers":
					urlToUse = baseUrl + "losers";
					readableParameter = "Biggest losers ";
					break;

				case "infocus":
					urlToUse = baseUrl + "infocus";
					readableParameter = "In focus stocks ";
					break;

				default:
					readableParameter = "Nothing defined";
					break;
			}

			return urlToUse;
		}

		private async Task<string> GetIexTradingDataAsync(string urlToUse, string readableParameter)
		{
			string data;
			using (var wc = new WebClient())
			{
				data = await wc.DownloadStringTaskAsync(urlToUse);
			}
			var trendsRoot = JsonConvert.DeserializeObject<IEnumerable<Trend>>(data);
			var finalOutput = new StringBuilder();
			foreach (var trend in trendsRoot)
			{
				finalOutput.Append(trend.CompanyName + ", ");
			}
			finalOutput.Replace(" Inc.", " ");
			finalOutput.Replace(" Corporation", "");
			data = readableParameter + " for the day are " + finalOutput.ToString();
			return data;
		}

		#endregion Private Methods
	}
}