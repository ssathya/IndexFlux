using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Google.Cloud.Dialogflow.V2;
using IndexFlux.Utils;

namespace IndexFlux.Responses
{
    public static class ParentFunction
    {

		#region Private Fields

		private const string IntentPath = @"$..displayName";

		#endregion Private Fields


		#region Public Methods

		[FunctionName("ParentFunction")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
			string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
			string intentName = "";
			WebhookResponse returnValue;
			try
			{
				var parserResult = JObject.Parse(requestBody);
				GetAttribute(parserResult, IntentPath, out intentName);
				returnValue = ProcessIntent(intentName, parserResult, out bool success);

			}
			catch (Exception )
			{

				throw;
			}

			log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return name != null
                ? (ActionResult)new OkObjectResult($"Hello, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
		}

		#endregion Public Methods


		#region Private Methods

		private static void GetAttribute(JObject requestBody, string queryPath, out string outString)
		{
			outString = requestBody.SelectToken(queryPath).Value<string>();
		}

		private static WebhookResponse ProcessIntent(string intentName, JObject parserResult, out bool success)
		{
			success = false;
			WebhookResponse returnValue = new WebhookResponse
			{
				FulfillmentText = GenericEndOfMsg.ErrorReturnMsg()
			};
			switch (intentName)
			{
				case "marketSummary":
					break;
				default:
					break;
			}

			return returnValue;
		}

		#endregion Private Methods
	}
}
