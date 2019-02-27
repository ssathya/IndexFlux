using Google.Cloud.Dialogflow.V2;
using IndexFlux.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IndexFlux.Utils
{
	public class ObtainMarterSummary
	{
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

		private WebhookResponse BuildOutputMessage(IndexData indexData)
		{
			StringBuilder tmpStr = new StringBuilder();
			tmpStr.Append("The markets are doing as follows.\n");
			foreach (var idxData in indexData.Data)
			{
				string direction = idxData.change_pct < 0 ? "downward" : "upward";
				tmpStr.Append($"{idxData.name}  is at  {Math.Round(idxData.price, 0)}. ");
				tmpStr.Append(idxData.day_change > 0 ? " Up by " : "Down by ");
				tmpStr.Append($"{Math.Abs(Math.Round(idxData.day_change, 0))} points.\n ");
				tmpStr.Append("\n");
			}
			var returnValue = new WebhookResponse
			{
				FulfillmentText = tmpStr.ToString()
			};
			return returnValue;
		}

		private static async Task<IndexData> ObtainFromWorldTrading(string tickersToUse)
		{
			var apiKey = Environment.GetEnvironmentVariable("WorldTradingDataKey", EnvironmentVariableTarget.Process);
			if (string.IsNullOrWhiteSpace(apiKey))
			{
				apiKey = Environment.GetEnvironmentVariable("WorldTradingDataKey", EnvironmentVariableTarget.Machine);
			}
			if (string.IsNullOrWhiteSpace(apiKey))
			{
				apiKey = Environment.GetEnvironmentVariable("WorldTradingDataKey", EnvironmentVariableTarget.User);
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
	}
}