using System.Collections.Generic;

namespace YahooFinanceAPI.Models
{
    internal class YahooQuote
    {
        public QuoteResponse quoteResponse { get; set; }

        internal class Result
        {
            public string language { get; set; }
            public string quoteType { get; set; }
            public string quoteSourceName { get; set; }
            public string currency { get; set; }
            public double fiftyDayAverage { get; set; }
            public double fiftyDayAverageChange { get; set; }
            public double fiftyDayAverageChangePercent { get; set; }
            public double twoHundredDayAverage { get; set; }
            public double twoHundredDayAverageChange { get; set; }
            public double twoHundredDayAverageChangePercent { get; set; }
            public int sourceInterval { get; set; }
            public string exchangeTimezoneName { get; set; }
            public string exchangeTimezoneShortName { get; set; }
            public int gmtOffSetMilliseconds { get; set; }
            public string exchange { get; set; }
            public int exchangeDataDelayedBy { get; set; }
            public string shortName { get; set; }
            public double regularMarketPrice { get; set; }
            public int regularMarketTime { get; set; }
            public double regularMarketChange { get; set; }
            public double regularMarketOpen { get; set; }
            public double regularMarketDayHigh { get; set; }
            public double regularMarketDayLow { get; set; }
            public long regularMarketVolume { get; set; }
            public string market { get; set; }
            public double regularMarketChangePercent { get; set; }
            public string regularMarketDayRange { get; set; }
            public double regularMarketPreviousClose { get; set; }
            public double bid { get; set; }
            public double ask { get; set; }
            public int bidSize { get; set; }
            public int askSize { get; set; }
            public string fullExchangeName { get; set; }
            public long averageDailyVolume3Month { get; set; }
            public long averageDailyVolume10Day { get; set; }
            public string marketState { get; set; }
            public bool esgPopulated { get; set; }
            public bool tradeable { get; set; }
            public double fiftyTwoWeekLowChange { get; set; }
            public double fiftyTwoWeekLowChangePercent { get; set; }
            public string fiftyTwoWeekRange { get; set; }
            public double fiftyTwoWeekHighChange { get; set; }
            public double fiftyTwoWeekHighChangePercent { get; set; }
            public double fiftyTwoWeekLow { get; set; }
            public double fiftyTwoWeekHigh { get; set; }
            public int priceHint { get; set; }
            public string symbol { get; set; }
        }

        internal class QuoteResponse
        {
            public IList<Result> result { get; set; }
            public object error { get; set; }
        }
    }
}