using System;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using YahooFinanceAPI.Models;
using YahooFinanceAPI.Utils;

namespace YahooFinanceAPI
{
    public class Quote
    {
        public static async Task<QuotePrice> GetPriceAsync(string symbol)
        {
            try
            {
                var csvData = await GetRawAsync(symbol).ConfigureAwait(false);
                if (csvData != null)
                {
                    var yq = await Json.ToObjectAsync<YahooQuote>(csvData).ConfigureAwait(false);

                    var quote = yq.quoteResponse.result[0];

                    var q = new QuotePrice
                    {
                        Symbol = quote.symbol,
                        Exchange = quote.exchange,
                        Timestamp = DateTimeConverter.ToDateTime(quote.regularMarketTime),
                        Close = Math.Round(quote.regularMarketPrice, 3),
                        Open = Math.Round(quote.regularMarketOpen, 3),
                        High = Math.Round(quote.regularMarketDayHigh, 3),
                        Low = Math.Round(quote.regularMarketDayLow, 3),
                        Volume = quote.regularMarketVolume
                    };

                    return q;
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }

            return new QuotePrice();
        }

        public static async Task<string> GetRawAsync(string symbol)
        {
            string csvData = null;

            try
            {
                var url = $"https://query1.finance.yahoo.com/v7/finance/quote?symbols={symbol}";

                using (var wc = new WebClient())
                {
                    csvData = await wc.DownloadStringTaskAsync(url).ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }

            return csvData;
        }
    }
}