using System;

namespace AzureGoogleAction.Models
{
	public class Datum
	{
		public string symbol { get; set; }
		public string name { get; set; }
		public string currency { get; set; }
		public float price { get; set; }
		public float price_open { get; set; }
		public float day_high { get; set; }
		public float day_low { get; set; }
		public string _52_week_high { get; set; }
		public string _52_week_low { get; set; }
		public float day_change { get; set; }
		public float change_pct { get; set; }
		public float close_yesterday { get; set; }
		public string market_cap { get; set; }
		public string volume { get; set; }
		public string volume_avg { get; set; }
		public string shares { get; set; }
		public string stock_exchange_long { get; set; }
		public string stock_exchange_short { get; set; }
		public string timezone { get; set; }
		public string timezone_name { get; set; }
		public string gmt_offset { get; set; }
		public DateTime last_trade_time { get; set; }
	}

}
