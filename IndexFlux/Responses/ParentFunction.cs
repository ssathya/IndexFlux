using Google.Cloud.Dialogflow.V2;
using IndexFlux.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndexFlux.Responses
{
	public static class ParentFunction
	{
		#region Private Fields

		private const string IntentPath = @"$..displayName";

		#endregion Private Fields

		#region Public Methods

		/// <summary>
		/// Runs the specified req.
		/// </summary>
		/// <param name="req">
		/// The req.
		/// </param>
		/// <param name="log">
		/// The log.
		/// </param>
		/// <returns></returns>
		[FunctionName("ParentFunction")]
		public static async Task<IActionResult> Run(
			[HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
			ILogger log)
		{
			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			string intentName = "";
			WebhookResponse returnValue = new WebhookResponse
			{
				FulfillmentText = Utilities.ErrorReturnMsg()
			};
			var retrunErrorString = JsonConvert.SerializeObject(returnValue,
				Formatting.Indented,
				new JsonSerializerSettings
				{
					ContractResolver = new CamelCasePropertyNamesContractResolver()
				});
			try
			{
				var parserResult = JObject.Parse(requestBody);
				GetAttribute(parserResult, IntentPath, out intentName);
				returnValue = await ProcessIntent(intentName, parserResult, log);
			}
			catch (Exception ex)
			{
				log.LogError($"Exception while running ParentFunction\nDetails:{ex.Message}");
				return new ContentResult
				{
					Content = retrunErrorString,
					ContentType = "application/json",
					StatusCode = 200
				};
			}
			if (!returnValue.FulfillmentText.Contains(@"'bye bye' to quit"))
			{
				returnValue.FulfillmentText = returnValue.FulfillmentText + "\n" + Utilities.EndOfCurrentRequest();
			}
			returnValue.FulfillmentText = Utilities.ConvertAllToASCII(returnValue.FulfillmentText);
			log.LogInformation("C# HTTP trigger function processed a request.");
			//var returnString = JsonConvert.SerializeObject(returnValue,
			//	Formatting.Indented,
			//	new JsonSerializerSettings
			//	{
			//		ContractResolver = new CamelCasePropertyNamesContractResolver()
			//	});
			var returnString = returnValue.ToString();
			return new ContentResult
			{
				Content = returnString,
				ContentType = "application/json",
				StatusCode = 200
			};
		}

		#endregion Public Methods

		#region Private Methods

		

		private static void GetAttribute(JObject requestBody, string queryPath, out string outString)
		{
			outString = requestBody.SelectToken(queryPath).Value<string>();
		}

		/// <summary>
		/// Processes the intent.
		/// </summary>
		/// <param name="intentName">
		/// Name of the intent.
		/// </param>
		/// <param name="parserResult">
		/// The parser result.
		/// </param>
		/// <param name="log">
		/// The log.
		/// </param>
		/// <returns></returns>
		private static async Task<WebhookResponse> ProcessIntent(string intentName, JObject parserResult, ILogger log)
		{
			WebhookResponse returnValue = new WebhookResponse
			{
				FulfillmentText = Utilities.ErrorReturnMsg()
			};
			switch (intentName)
			{
				case "marketSummary":
					var msActor = new ObtainMarketSummary(log);
					returnValue = await msActor.GetIndicesValuesAsync(parserResult);
					break;

				case "marketTrends":
					var mtActor = new ObtainTrenders(log);
					returnValue = await mtActor.GetTrendingAsync(parserResult);
					break;

				case "newsFetch":
					var newsActor = new ObtainNews(log);
					returnValue = await newsActor.GetExternalNews(parserResult);
					break;

				default:
					break;
			}

			return returnValue;
		}
		#endregion Private Methods
	}
}