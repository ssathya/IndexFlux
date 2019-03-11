using ExtractCompanyList.ExternalData;
using ExtractCompanyList.SaveData;
using System;

namespace ExtractCompanyList
{
	class Program
	{
		static void Main(string[] args)
		{
			var fileToSave = @"c:/Users/sridh/Downloads/extract.txt";
			Console.WriteLine("Getting External data!");
			var ged = new GetExternalData();
			var result = ged.GetDataFromExternalSource().Result;
			var ped = new ProcessExtData(result);
			bool SaveFileResult = ped.WriteDataToFileAsync(fileToSave).Result;
			if (SaveFileResult == false)
			{
				Console.WriteLine("Could not write data to file");
			}
			Console.WriteLine("Press any key to exit");
			Console.Read();
		}
	}
}
