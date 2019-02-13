using AzureGoogleAction.Models;
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
	// [Route("api/[controller]"), Produces("application/json")]
	[Route("api/[controller]")]
	[ApiController]
	public class IndicesController : Controller
	{
		#region Private Fields + Enums

		private const string apiKeyValue = "Provider:WorldTradingData:apiKey";
		private const string dow30Key = "Provider:WorldTradingData:Indexes:DowJonesIndustrialAverage";
		private const string nasdq100Key = "Provider:WorldTradingData:Indexes:NasdaqComposite";
		private const string spKey = "Provider:WorldTradingData:Indexes:SP500Index";

		private readonly ILogger<IndicesController> _logger;
		private readonly IConfiguration _config;

		#endregion Private Fields + Enums

		#region Public Constructors

		public IndicesController(ILogger<IndicesController> logger,
			IConfiguration config)
		{
			_logger = logger;
			_config = config;
		}

		#endregion Public Constructors

		#region Public Methods

		// GET: api/Indices
		[HttpGet]
		public async Task<IActionResult> Get()
		{
			IndexData indexData = await GetIndexQuotes();
			if (indexData.Data.Length == 0)
			{
				return Ok("Could not get data from remote server");
			}
			var tmpStr = new StringBuilder();
			tmpStr.Append("The markets are doing as follows.\n\t");
			foreach (var data in indexData.Data)
			{
				string tradingStatement;
				string direction = data.change_pct < 0 ? "downward" : "upward";
				switch (Math.Abs(data.change_pct))
				{
					case float n when (n <= 0.2):
						tradingStatement = " trading water; it's current value is  ";
						break;

					case float n when (n > 0.2 && n <= 0.5):
						tradingStatement = $" has a moderate { direction } swing; it's current value is   ";
						break;

					case float n when (n > 0.5 && n <= 1.0):
						tradingStatement = $" has a strong { direction } swing; it's current value is  ";
						break;

					case float n when (n > 1.0 && n < 2.0):
						tradingStatement = data.change_pct < 0 ? " is having a bad day; it's current value is " : " is having a good day; it's current value is ";
						break;

					case float n when (n >= 2.0):
						tradingStatement = data.change_pct < 0 ? " is having a panic attack; it's current value is  " : " is having a blast; it's current value is  ";
						break;

					default:
						tradingStatement = "";
						break;
				}
				tmpStr.Append($"{data.name}  {tradingStatement}  {Math.Round(data.price)}. ");
				tmpStr.Append(data.day_change > 0 ? " Up by " : "Down by ");
				tmpStr.Append($"{Math.Round(data.day_change, 0)} points.\n ");
				tmpStr.Append("\n");
			}

			return Ok(tmpStr.ToString());
		}

		#endregion Public Methods

		#region Private Methods

		private async Task<IndexData> GetIndexQuotes()
		{
			var apiKey = _config[apiKeyValue];
			//var tmpStr = _config["Provider:WorldTradingData:Indexes"];
			var tickers = new List<string>
			{
				_config[dow30Key],
				_config[nasdq100Key],
				_config[spKey]
			};
			var tmpStr = new StringBuilder();
			tmpStr.AppendJoin(',', tickers);

			string urlStr = $@"https://www.worldtradingdata.com/api/v1/stock?symbol={tmpStr.ToString()}&api_token={apiKey}";
			string data = "{}";
			try
			{
				using (var wc = new WebClient())
				{
					data = await wc.DownloadStringTaskAsync(urlStr);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error while getting data from World Trading data; reason \n\t{ex.Message}");
				return new IndexData();
			}
			_logger.LogTrace(urlStr);
			try
			{
				var indexData = JsonConvert.DeserializeObject<IndexData>(data);
				return indexData;
			}
			catch (Exception ex)
			{
				_logger.LogError($"Error while converting external data to Index Data.\n\t{ex.Message}");
				return new IndexData();
			}
		}

		#endregion Private Methods
	}
}