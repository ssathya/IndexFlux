using Google.Cloud.Dialogflow.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Google.Cloud.Dialogflow.V2.Intent.Types;

namespace IndexFlux.Utils
{
	internal static class Utilities
	{
		internal static string EndOfCurrentRequest()
		{
			return "\nAnything more? For help say help or say 'bye bye' to quit\n";
		}
		internal static string ErrorReturnMsg()
		{
			return "\nWe are experiencing internal issues; be assured we'll resolve as soon as possible\n" +
				EndOfCurrentRequest();
		}
		/// <summary>
		/// Converts all to ASCII.
		/// </summary>
		/// <param name="inString">
		/// The in string.
		/// </param>
		/// <returns></returns>
		internal static string ConvertAllToASCII(string inString)
		{
			var newStringBuilder = new StringBuilder();
			newStringBuilder.Append(inString.Normalize(NormalizationForm.FormKD)
											.Where(x => x < 128)
											.ToArray());
			return newStringBuilder.ToString();
		}

		internal static string ConvertToSSML(string unformatedMsg)
		{
			StringBuilder tempValue = new StringBuilder();
			tempValue.Append("<speak>");
			tempValue.Append(unformatedMsg);
			tempValue.Append("</speak>");
			tempValue.Replace("\r", "");
			tempValue.Replace("\n\n", "\n");
			tempValue.Replace("\n", @"<break strength='x - strong' time='500ms' />");
			return tempValue.ToString();			
		}
		internal static void PlaceStandardHeaders(WebhookResponse returnValue)
		{
			
			var platform = new Message
			{
				Platform = Message.Types.Platform.ActionsOnGoogle
			};						
			// returnValue.FulfillmentText = "";
			returnValue.FulfillmentMessages.Add(platform);
		}
	}
}
