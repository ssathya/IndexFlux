using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Dialogflow.V2;
using IndexFlux.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IndexFlux.Utils
{
    public class ObtainNews
    {
		private const string newsParameter = "newsSource";
		private const string ParametersPath = @"$.*.parameters";
		private readonly ILogger _logger;

		public ObtainNews(ILogger log)
		{
			_logger = log;
		}

		internal async Task<WebhookResponse> GetExternalNews(JObject parserResult)
		{
			var parameters = parserResult.SelectToken(ParametersPath);
			var paramValues =
				((JObject)parameters).ToObject<Dictionary<string, string>>();
			var newsSourceRequested = paramValues[newsParameter];
			string urlToUse = "";
			urlToUse = BuildUrlToUse(newsSourceRequested, urlToUse, out string readableParameter);
			NewsExtract extracts = await ObtainNewAPIDta(urlToUse);
			string returnString = ExtractHeadlines(extracts);
			return new WebhookResponse
			{
				FulfillmentText = returnString
			};
		}

		private string ExtractHeadlines(NewsExtract extracts)
		{
			var returnMsg = new StringBuilder();
			if (extracts.totalResults == 0 || !extracts.status.Equals("ok"))
			{
				return "Cannot obtain news at this time\n";
			}
			returnMsg.Append("Here are the headlines from " + extracts.articles[0].source.name +"\n");
			foreach (var article in extracts.articles)
			{
				returnMsg.Append(article.title + "\n");
				returnMsg.Append("                   " + "\n");
			}
			return returnMsg.ToString();
			
		}

		private async Task<NewsExtract> ObtainNewAPIDta(string urlToUse)
		{
			string data = "{}";
			using (var wc = new WebClient())
			{
				data = await wc.DownloadStringTaskAsync(urlToUse);
			}
			var newsData = JsonConvert.DeserializeObject<NewsExtract>(data);
			return newsData;
		}

		private string BuildUrlToUse(string newsSourceRequested, string urlToUse, out string readableParameter)
		{
			readableParameter = "";

			
			string newsMedia = "sources=";
			switch (newsSourceRequested.ToLower())
			{
				case "cnbc":
					newsMedia += newsSourceRequested.ToLower().Trim();
					break;
				case "the-new-york-times":
					newsMedia += newsSourceRequested.ToLower().Trim();
					break;
				case "the-wall-street-journal":
					newsMedia += newsSourceRequested.ToLower().Trim();
					break;
				case "the-hindu":
					newsMedia += newsSourceRequested.ToLower().Trim();
					break;
				default:
					newsMedia = "country=us";
					break;
			}
			string apiKey = GetApiKey();
			var urlStr = $"https://newsapi.org/v2/top-headlines?{newsMedia}&apiKey={apiKey}";
			return urlStr;
		}

		private string GetApiKey()
		{
			var apiKey = Environment.GetEnvironmentVariable("NewsAPIKey", EnvironmentVariableTarget.Process);
			if (string.IsNullOrWhiteSpace(apiKey))
			{
				_logger.LogDebug("Did not find api key in process");
				apiKey = Environment.GetEnvironmentVariable("NewsAPIKey", EnvironmentVariableTarget.Machine);
			}
			if (string.IsNullOrWhiteSpace(apiKey))
			{
				_logger.LogDebug("Did not find api key in Machine");
				apiKey = Environment.GetEnvironmentVariable("NewsAPIKey", EnvironmentVariableTarget.User);
			}
			if (string.IsNullOrWhiteSpace(apiKey))
			{
				_logger.LogError("Did not find api key; calls will fail");
			}
			return apiKey;
		}
	}
}
