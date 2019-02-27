using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Cloud.Dialogflow.V2;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace IndexFlux.Utils
{
    public class ObtainNews
    {
		private readonly ILogger _logger;

		public ObtainNews(ILogger log)
		{
			_logger = log;
		}

		internal Task<WebhookResponse> GetExternalNews(JObject parserResult)
		{

			throw new NotImplementedException();
		}
	}
}
