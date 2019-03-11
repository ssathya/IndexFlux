using ExtractCompanyList.ExternalData;
using System;

namespace ExtractCompanyList
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Hello World!");
			var ged = new GetExternalData();
			var result = ged.GetDataFromExternalSource().Result;
			Console.Read();
		}
	}
}
