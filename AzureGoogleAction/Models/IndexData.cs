using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureGoogleAction.Models
{


	public class IndexData
	{
		public int Symbols_requested { get; set; }
		public int Symbols_returned { get; set; }
		public Datum[] Data { get; set; }
	}

}
