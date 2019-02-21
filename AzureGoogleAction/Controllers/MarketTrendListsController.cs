using AzureGoogleAction.Models;
using AzureGoogleAction.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace AzureGoogleAction.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class MarketTrendListsController : Controller
	{

		#region Private Fields

		private const string baseUrl = @"https://api.iextrading.com/1.0/stock/market/list/";
		private readonly IConfiguration _config;
		private readonly ILogger<MarketTrendListsController> _logger;

		#endregion Private Fields


		#region Public Constructors

		public MarketTrendListsController(ILogger<MarketTrendListsController> logger,
			IConfiguration config)
		{
			_logger = logger;
			_config = config;
		}

		#endregion Public Constructors


		#region Public Methods

		[HttpGet("{parameter}")]
		public async Task<IActionResult> Get(string parameter)
		{
			string urlToUse = "";
			urlToUse = BuildUrlToUse(parameter, urlToUse, out string readableParameter);
			if (string.IsNullOrWhiteSpace(urlToUse))
			{
				return BadRequest("Unsupported action parameter");
			}
			string data;
			try
			{
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
				data = readableParameter + " for the day are " + finalOutput.ToString() + GenericEndOfMsg.EndOfCurrentRequest();
				return Ok(data);
			}
			catch (Exception ex)
			{
				_logger.LogError($"Unhanded exception in MarketTrendListController:Get. Reason\n\t{ex.Message}");
				return Ok(GenericEndOfMsg.ErrorReturnMsg());
				//return StatusCode(StatusCodes.Status500InternalServerError, GenericEndOfMsg.ErrorReturnMsg();
			}
		}

		#endregion Public Methods


		#region Private Methods

		private static string BuildUrlToUse(string parameter, string urlToUse, out string readableParameter)
		{
			switch (parameter.ToLower())
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

		#endregion Private Methods
	}
}