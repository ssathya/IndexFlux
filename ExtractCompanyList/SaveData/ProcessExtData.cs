using ExtractCompanyList.Models;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExtractCompanyList.SaveData
{
	public class ProcessExtData
	{

		#region Private Fields

		private readonly CompanySymbols _comapnSymobls;
		private readonly Regex _regexPattern1;
		private readonly Regex _regexPattern2;
		private readonly Regex _regexPattern3;
		private readonly Regex _regexPattern4;

		#endregion Private Fields


		#region Public Constructors

		public ProcessExtData(CompanySymbols cs)
		{
			_comapnSymobls = cs;
			_regexPattern1 = new Regex(@"Company$");
			_regexPattern2 = new Regex(@"Ltd$");
			_regexPattern3 = new Regex(@"Limited$");
			_regexPattern4 = new Regex(@"Inc$");
		}

		#endregion Public Constructors


		#region Internal Methods

		internal async Task<bool> WriteDataToFileAsync(string fileToSave)
		{
			//double quotes
			var dq = '"';
			try
			{
				using (var writer = new StreamWriter(fileToSave))
				{
					foreach (var ticker in _comapnSymobls.Tickers)
					{
						var matchResult = ticker.Symbol.IndexOfAny(new char[] { '^' });
						if (matchResult == -1)
						{
							string readableName = MakeNameReadable(ticker.Name);
							await writer.WriteLineAsync($"{dq}{ticker.Symbol.Trim()}{dq},{dq}{ticker.Symbol.Trim()}{dq},{dq}{readableName}{dq}");
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exceptin while writing to file; reason\n{ex.Message}");
				return false;
			}
			return true;
		}

		#endregion Internal Methods


		#region Private Methods

		private string MakeNameReadable(string name)
		{
			name = name.Replace("Inc.", "")
				.Replace("Corporation", "")
				.Replace("&", " and ")
				.Replace("Corp.", "")
				.Replace("Ltd.", "");
			var mulitName = name.Split('(');

			name = mulitName.First().Trim();
			name = _regexPattern1.Replace(name, string.Empty);
			name = _regexPattern2.Replace(name, string.Empty);
			name = _regexPattern3.Replace(name, string.Empty);
			name = _regexPattern4.Replace(name, string.Empty);

			return name.Trim();
		}

		#endregion Private Methods
	}
}