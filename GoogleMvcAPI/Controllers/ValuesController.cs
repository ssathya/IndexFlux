using Google.Cloud.Dialogflow.V2;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.IO;

namespace GoogleMvcAPI.Controllers
{
	[Route("api/[controller]")]
	public class ValuesController : Controller
	{
		#region Private Fields + Enums

		private static readonly JsonParser jsonParser =
			new JsonParser(JsonParser.Settings.Default.WithIgnoreUnknownFields(true));

		private readonly ILogger _logger;
		private WebhookRequest request;
		private Struct payload;

		#endregion Private Fields + Enums

		#region Public Constructors

		public ValuesController(ILogger<ValuesController> logger)
		{
			_logger = logger;
		}

		#endregion Public Constructors

		#region Public Methods

		// GET api/values
		[HttpGet]
		public IActionResult Get()
		{
			// Sends a message to configured loggers, including the Stackdriver logger.
			// The Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker logger will log all controller actions with
			// log level information. This log is for additional information.
			//_logger.LogDebug("Values retrieved!");
			//return new string[] { "value1", "value2" };
			using (var reader = new StreamReader(Request.Body))
			{
				request = jsonParser.Parse<WebhookRequest>(reader);
			}
			var a = request.QueryResult.QueryText;
			var b = request.QueryResult.Intent.Name;
			var formatQuery = $"You initiated {b} with the following query {a}" +
				"\n Your request will be processed soon; until then please hang-on";
			var fulfillmentMsg = $"This is fulfillment message for query {a}";

			var response = new WebhookResponse
			{
				FulfillmentText = formatQuery,
			};

			return Ok(response);
		}

		// GET api/values/5
		[HttpGet("{id}")]
		public string Get(int id)
		{
			return "value";
		}

		// POST api/values
		[HttpPost]
		public void Post([FromBody]string value)
		{
			_logger.LogInformation("Value added");
		}

		// PUT api/values/5
		[HttpPut("{id}")]
		public void Put(int id, [FromBody]string value)
		{
		}

		// DELETE api/values/5
		[HttpDelete("{id}")]
		public void Delete(int id)
		{
		}

		#endregion Public Methods
	}
}