using Google.Cloud.Dialogflow.V2;
using IndexFlux.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IndexFlux.Utils
{
	public class ObtainMarketSummary
	{

		#region Private Fields

		private readonly ILogger _logger;
		private readonly WebhookRequest _webhookRequest;

		#endregion Private Fields


		#region Public Constructors

		public ObtainMarketSummary(WebhookRequest webhookRequest, ILogger log)
		{
			_logger = log;
			_webhookRequest = webhookRequest;
		}

		#endregion Public Constructors


		#region Public Methods

		public async Task<WebhookResponse> GetIndicesValuesAsync(JObject parserResult)
		{
			var tickers = new List<string>
			{
				"^DJI",
				"^IXIC",
				"^INX"
			};
			var tmpStr = new StringBuilder();
			tmpStr.AppendJoin(',', tickers);

			IndexData indexData = await ObtainFromWorldTrading(tmpStr.ToString());

			WebhookResponse returnValue = BuildOutputMessage(indexData);

			return returnValue;
		}

		#endregion Public Methods


		#region Private Methods

		private  async Task<IndexData> ObtainFromWorldTrading(string tickersToUse)
		{
			var apiKey = Environment.GetEnvironmentVariable("WorldTradingDataKey", EnvironmentVariableTarget.Process);
			if (string.IsNullOrWhiteSpace(apiKey))
			{
				_logger.LogDebug("Did not find api key in process");
				apiKey = Environment.GetEnvironmentVariable("WorldTradingDataKey", EnvironmentVariableTarget.Machine);
			}
			if (string.IsNullOrWhiteSpace(apiKey))
			{
				_logger.LogDebug("Did not find api key in Machine");
				apiKey = Environment.GetEnvironmentVariable("WorldTradingDataKey", EnvironmentVariableTarget.User);
			}
			if (string.IsNullOrWhiteSpace(apiKey))
			{
				_logger.LogDebug("Did not find api key in Machine");
				apiKey = Environment.GetEnvironmentVariable("WorldTradingDataKey");
			}
			if (string.IsNullOrWhiteSpace(apiKey))
			{
				_logger.LogError("Did not find api key; calls will fail");
			}
				string urlStr = $@"https://www.worldtradingdata.com/api/v1/stock?symbol={tickersToUse}&api_token={apiKey}";
			string data = "{}";
			using (var wc = new WebClient())
			{
				data = await wc.DownloadStringTaskAsync(urlStr);
			}
			var indexData = JsonConvert.DeserializeObject<IndexData>(data);
			return indexData;
		}

		private WebhookResponse BuildOutputMessage(IndexData indexData)
		{
			StringBuilder tmpStr = new StringBuilder();
			if (indexData.Data.Length >= 1)
			{
				var dateToUse = indexData.Data[0].last_trade_time;
				tmpStr.Append("As of ");
				tmpStr.Append($"{dateToUse.ToString("MMMM dd, hh:mm tt")} EST ");
			}					
			foreach (var idxData in indexData.Data)
			{
				string direction = idxData.change_pct < 0 ? "downward" : "upward";
				tmpStr.Append($"{idxData.name}  is at  {Math.Round(idxData.price, 0)}. ");
				tmpStr.Append(idxData.day_change > 0 ? " Up by " : "Down by ");
				tmpStr.Append($"{Math.Abs(Math.Round(idxData.day_change, 0))} points.\n\n ");				
			}			
			var returnValue = new WebhookResponse
			{
				FulfillmentText = tmpStr.ToString()
			};
			return returnValue;
		}

		#endregion Private Methods
	}
}