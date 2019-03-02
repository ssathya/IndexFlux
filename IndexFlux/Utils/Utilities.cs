using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	}
}
