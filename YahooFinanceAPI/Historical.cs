using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using YahooFinanceAPI.Models;
using YahooFinanceAPI.Utils;

namespace YahooFinanceAPI
{
    /// <summary>
    /// Class for fetching stock historical price from Yahoo Finance
    /// Copyright Dennis Lee
    /// 3 Nov 2017
    ///
    /// </summary>
    public class Historical
    {
        #region Public Methods

        /// <summary>
        /// Get stock historical price from Yahoo Finance
        /// </summary>
        /// <param name="symbol">Stock ticker symbol</param>
        /// <param name="start">Starting datetime</param>
        /// <param name="end">Ending datetime</param>
        /// <returns>List of history price</returns>
        public static async Task<List<HistoryPrice>> GetPriceAsync(string symbol, DateTime start, DateTime end)
        {
            try
            {
                var csvData = await GetRawAsync(symbol, start, end).ConfigureAwait(false);
                if (csvData != null) return await ParsePriceAsync(csvData).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }

            return new List<HistoryPrice>();
        }

        /// <summary>
        /// Get stock historical dividends from Yahoo Finance
        /// </summary>
        /// <param name="symbol">Stock ticker symbol</param>
        /// <param name="start">Starting datetime</param>
        /// <param name="end">Ending datetime</param>
        /// <returns>List of dividends</returns>
        public static async Task<List<Dividend>> GetDividendAsync(string symbol, DateTime start, DateTime end)
        {
            try
            {
                var csvData = await GetRawAsync(symbol, start, end, "div").ConfigureAwait(false);
                if (csvData != null) return await ParseDivAsync(csvData).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }
            return new List<Dividend>();
        }

        /// <summary>
        /// Get raw stock historical price from Yahoo Finance
        /// </summary>
        /// <param name="symbol">Stock ticker symbol</param>
        /// <param name="start">Starting datetime</param>
        /// <param name="end">Ending datetime</param>
        /// <param name="eventType">Event type (e.g: history, div)</param>
        /// <returns>Raw history price string</returns>
        public static async Task<string> GetRawAsync(string symbol, DateTime start, DateTime end, string eventType = "history")
        {
            string csvData = null;

            try
            {
                var url = "https://query1.finance.yahoo.com/v7/finance/download/{0}?period1={1}&period2={2}&interval=1d&events={3}&crumb={4}";

                //if no token found, refresh it
                if (string.IsNullOrEmpty(Token.Cookie) || string.IsNullOrEmpty(Token.Crumb))
                {
                    if (!await Token.RefreshAsync(symbol).ConfigureAwait(false))
                        return await GetRawAsync(symbol, start, end).ConfigureAwait(false);
                }

                url = string.Format(url, symbol, Math.Round(DateTimeConverter.ToUnixTimestamp(start), 0),
                    Math.Round(DateTimeConverter.ToUnixTimestamp(end), 0), eventType, Token.Crumb);

                using (var wc = new WebClient())
                {
                    wc.Headers.Add(HttpRequestHeader.Cookie, Token.Cookie);
                    csvData = await wc.DownloadStringTaskAsync(url).ConfigureAwait(false);
                }
            }
            catch (WebException webEx)
            {
                var response = (HttpWebResponse)webEx.Response;

                //Re-fetching token
                if (response.StatusCode != HttpStatusCode.Unauthorized ||
                    response.StatusCode != HttpStatusCode.NotFound) throw;
                Debug.Print(webEx.Message);
                Token.Cookie = "";
                Token.Crumb = "";
                Debug.Print("Re-fetch token");
                return await GetRawAsync(symbol, start, end).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }

            return csvData;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Parse raw historical price data into list
        /// </summary>
        /// <param name="csvData"></param>
        /// <returns>List of historical price</returns>
        private static async Task<List<HistoryPrice>> ParsePriceAsync(string csvData)
        {
            return await Task.Run(() =>
            {
                var lst = new List<HistoryPrice>();

                try
                {
                    var rows = csvData.Split(Convert.ToChar(10));

                    //row(0) was ignored because is column names
                    //data is read from oldest to latest
                    for (var i = 1; i <= rows.Length - 1; i++)
                    {
                        var row = rows[i];
                        if (string.IsNullOrEmpty(row)) continue;

                        var cols = row.Split(',');
                        if (cols[1] == "null") continue;

                        var itm = new HistoryPrice
                        {
                            Date = DateTime.Parse(cols[0]),
                            Open = Convert.ToDouble(cols[1]),
                            High = Convert.ToDouble(cols[2]),
                            Low = Convert.ToDouble(cols[3]),
                            Close = Convert.ToDouble(cols[4]),
                            AdjClose = Convert.ToDouble(cols[5])
                        };

                        //fixed issue in some currencies quote (e.g: SGDAUD=X)
                        if (cols[6] != "null") itm.Volume = Convert.ToInt64(cols[6]);

                        lst.Add(itm);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                }

                return lst;
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// Parse raw dividend data into list
        /// </summary>
        /// <param name="csvData"></param>
        /// <returns>List of dividends</returns>
        private static async Task<List<Dividend>> ParseDivAsync(string csvData)
        {
            return await Task.Run(() =>
            {
                var lst = new List<Dividend>();
                try
                {
                    var rows = csvData.Split(Convert.ToChar(10));

                    //row(0) was ignored because is column names
                    //data is read from oldest to latest
                    for (var i = 1; i <= rows.Length - 1; i++)
                    {
                        var row = rows[i];
                        if (string.IsNullOrEmpty(row)) continue;

                        var cols = row.Split(',');
                        if (cols[1] == "null") continue;

                        var itm = new Dividend
                        {
                            Date = DateTime.Parse(cols[0]),
                            Div = Convert.ToDouble(cols[1])
                        };

                        lst.Add(itm);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                }

                return lst;
            }).ConfigureAwait(false);
        }

        #endregion Private Methods
    }
}