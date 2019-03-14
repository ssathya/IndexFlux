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
			var ssmlText = new StringBuilder();
			ssmlText.Append("<speak>\n");
			var modifiedInput = textToConvert.Replace("\n", @"<break strength=""weak"" time=""300ms""/>");
			ssmlText.Append(modifiedInput + "\n");
			ssmlText.Append("</speak>\n");
			var element = XElement.Parse(ssmlText.ToString());
			var settings = new XmlWriterSettings();
			settings.OmitXmlDeclaration = true;
			settings.Indent = true;
			settings.NewLineOnAttributes = true;
			var beautySSML = new StringBuilder();
			using (var xmlWriter = XmlWriter.Create(beautySSML, settings))
			{
				element.Save(xmlWriter);
			}
			var retrunValue = new SimpleResponse
			{
				Ssml = beautySSML.ToString(),
				DisplayText = textToConvert
			};
			return retrunValue;
		}
	}
}
