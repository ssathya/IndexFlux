using Google.Cloud.Dialogflow.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static Google.Cloud.Dialogflow.V2.Intent.Types;
using static Google.Cloud.Dialogflow.V2.Intent.Types.Message.Types;

namespace IndexFlux.Utils
{
	public static class Utilities
	{
		public static string EndOfCurrentRequest()
		{
			return "\nAnything more? For help say help or say 'bye bye' to quit\n";
		}
		public static string ErrorReturnMsg()
		{
			return "\nWe are experiencing public issues; be assured we'll resolve as soon as possible\n" +
				EndOfCurrentRequest();
		}
		/// <summary>
		/// Converts all to ASCII.
		/// </summary>
		/// <param name="inString">
		/// The in string.
		/// </param>
		/// <returns></returns>
		public static string ConvertAllToASCII(string inString)
		{
			var newStringBuilder = new StringBuilder();
			newStringBuilder.Append(inString.Normalize(NormalizationForm.FormKD)
											.Where(x => x < 128)
											.ToArray());
			return newStringBuilder.ToString();
		}

		public static string ConvertToSSML(string unformatedMsg)
		{
			StringBuilder tempValue = new StringBuilder();
			tempValue.Append("<speak>");
			tempValue.Append(unformatedMsg);
			tempValue.Append("</speak>");			
			var retValue = Regex.Replace(tempValue.ToString(), @"\r\n?|\n|\\n|\\r\\n", @"<break strength='x - strong' time='500ms' />");			
			return retValue;
		}
		public static void PlaceStandardHeaders(WebhookResponse returnValue)
		{

			var platform = new Message
			{
				Platform = Message.Types.Platform.ActionsOnGoogle
			};
			// returnValue.FulfillmentText = "";
			returnValue.FulfillmentMessages.Add(platform);
		}

		public static SimpleResponse BuildTextToSpeech(string textToConvert)
		{			
			var settings = new XmlWriterSettings
			{
				OmitXmlDeclaration = true,
				Indent = true,
				NewLineOnAttributes = true,
				NewLineHandling = NewLineHandling.Replace
			};
			var beautySSML = new StringBuilder();												
			var modifiedInput = Regex.Replace(textToConvert, @"\r\n?|\n|\\n|\\r\\n", @"zyxwvu");
			beautySSML.Clear();
			settings.NewLineOnAttributes = false;
			using (var xmlWriter = XmlWriter.Create(beautySSML, settings))
			{
				xmlWriter.WriteStartElement("speak");
				foreach (var mi in modifiedInput.Split(@"zyxwvu"))
				{
					if (mi.Trim().Length > 0)
					{
						xmlWriter.WriteElementString("s", mi);
						xmlWriter.WriteStartElement("break");
						xmlWriter.WriteAttributeString("time", "300ms");
						xmlWriter.WriteEndElement();
					}
				}
				xmlWriter.WriteEndElement();
				xmlWriter.WriteEndDocument();
			}
			beautySSML.Replace("\r", "");
			modifiedInput = ConvertAllToASCII(beautySSML.ToString());
			var txtToConvert = textToConvert;
			if (txtToConvert.Length > 620)
			{
				var truncatePoint = textToConvert.LastIndexOf(' ', 600);
				txtToConvert = txtToConvert.Substring(0, truncatePoint) +" ...";
			}
			var retrunValue = new SimpleResponse
			{
				Ssml = modifiedInput,				
				DisplayText = txtToConvert
			};						
			return retrunValue;
		}
	}
}
