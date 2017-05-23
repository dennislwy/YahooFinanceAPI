using System;
using System.Diagnostics;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace YahooFinanceAPI
{
    /// <summary>
    /// Class for fetching token (cookie and crumb) from Yahoo Finance
    /// Copyright Dennis Lee
    /// 19 May 2017
    /// 
    /// </summary>
    public class Token
    {
        public static string Cookie { get; set; }
        public static string Crumb { get; set; }

        private static Regex regex_crumb;

        /// <summary>
        /// Refresh cookie and crumb value
        /// </summary>
        /// <param name="symbol">Stock ticker symbol</param>
        /// <returns></returns>
        public static bool Refresh(string symbol = "SPY")
        {

            try
            {
                Token.Cookie = "";
                Token.Crumb = "";

                string url_scrape = "https://finance.yahoo.com/quote/{0}?p={0}";
                //url_scrape = "https://finance.yahoo.com/quote/{0}/history"

                string url = string.Format(url_scrape, symbol);

                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

                request.CookieContainer = new CookieContainer();
                request.Method = "GET";

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {

                    string cookie = response.GetResponseHeader("Set-Cookie").Split(';')[0];

                    string html = "";

                    using (Stream stream = response.GetResponseStream())
                    {
                        html = new StreamReader(stream).ReadToEnd();
                    }

                    if (html.Length < 5000)
                        return false;
                    string crumb = getCrumb(html);
                    html = "";

                    if (crumb != null)
                    {
                        Token.Cookie = cookie;
                        Token.Crumb = crumb;
                        Debug.Print("Crumb: '{0}', Cookie: '{1}'", crumb, cookie);
                        return true;
                    }

                }

            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }

            return false;

        }

        /// <summary>
        /// Get crumb value from HTML
        /// </summary>
        /// <param name="html">HTML code</param>
        /// <returns></returns>
        private static string getCrumb(string html)
        {

            string crumb = null;

            try
            {
                //initialize on first time use
                if (regex_crumb == null)
                    regex_crumb = new Regex("CrumbStore\":{\"crumb\":\"(?<crumb>\\w+)\"}", 
                        RegexOptions.CultureInvariant | RegexOptions.Compiled, TimeSpan.FromSeconds(5));

                MatchCollection matches = regex_crumb.Matches(html);

                if (matches.Count > 0)
                {
                    crumb = matches[0].Groups["crumb"].Value;
                }
                else
                {
                    Debug.Print("Regex no match");
                }

                //prevent regex memory leak
                matches = null;

            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
            }

            GC.Collect();
            return crumb;

        }

    }
}
