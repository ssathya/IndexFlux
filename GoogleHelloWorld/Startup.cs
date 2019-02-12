using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Google.Cloud.Dialogflow.V2;
using Google.Protobuf;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace GoogleHelloWorld
{
	public class Startup
	{
		private static readonly JsonParser jsonParser =
			new JsonParser(JsonParser.Settings.Default.WithIgnoreUnknownFields(true));
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.Run(async (context) =>
			{
				WebhookRequest request;
				using (var reader = new StreamReader(context.Request.Body))
				{
					request = jsonParser.Parse<WebhookRequest>(reader);
				}
				var resopnse = new WebhookResponse
				{
					FulfillmentText = "Hello from " + request.QueryResult.Intent.DisplayName
				};
				await context.Response.WriteAsync(resopnse.ToString());
			});
		}
	}
}
