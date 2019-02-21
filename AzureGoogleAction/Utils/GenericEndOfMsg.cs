using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AzureGoogleAction.Utils
{
	internal static class GenericEndOfMsg
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
	}
}
