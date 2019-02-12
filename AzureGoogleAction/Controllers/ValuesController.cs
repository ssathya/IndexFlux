using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Dialogflow.V2;
using Google.Protobuf;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AzureGoogleAction.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class ValuesController : ControllerBase
	{
		private static readonly JsonParser jsonParser =
			new JsonParser(JsonParser.Settings.Default.WithIgnoreUnknownFields(true));
		private readonly ILogger<ValuesController> _logger;

		public ValuesController(ILogger<ValuesController> logger)
		{
			_logger = logger;
		}
		// GET api/values
		[HttpGet]
		public ActionResult Get()
		{
			WebhookRequest request;
			using (var reader = new StreamReader(Request.Body))
			{
				try
				{
					request = jsonParser.Parse<WebhookRequest>(reader);
				}
				catch (Exception ex)
				{
					_logger.LogError(ex.Message);
					request = null;
				}
			}
			var fulfillmentText = request != null ? $"Got request from {request.QueryResult.Intent.DisplayName} with query {request.QueryResult.QueryText} " :
				"Request was null";
			WebhookResponse response = new WebhookResponse
			{
				FulfillmentText = fulfillmentText
			};
			var responseJson = response.ToString();
			return Content(responseJson, "application/json");
		}

		// GET api/values/5
		[HttpGet("{id}")]
		public ActionResult<string> Get(int id)
		{
			return "value";
		}

		// POST api/values
		[HttpPost]
		public void Post([FromBody] string value)
		{
		}

		// PUT api/values/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody] string value)
		{
		}

		// DELETE api/values/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}
	}
}
