# YahooFinanceAPI
[![Language](https://img.shields.io/badge/language-.NET-blue.svg?style=flat)](http://www.visualstudio.com) [![Email](https://img.shields.io/badge/email-dennis%40gsfcorp.com-blue.svg?style=flat)](mailto:dennis@gsfcorp.com)
[![Website](https://img.shields.io/badge/website-gsfcorp.com-lightgrey.svg?style=flat)](http://www.gsfcorp.com)

Starting 16 May 2017, Yahoo finance has discontinued its well used service of EOD (end-of-day) data download without notice or warning. This is confirmed by Yahoo employee in this [forum post][1].  

However, EOD data still works on Yahoo finance pages. To download these data, you will now require "crumb" and cookie "B" for authentication. This code will obtain valid cookie and crumb and also downloads EOD price from Yahoo finance.  

For python version, please visit code from [c0redumb](https://github.com/c0redumb/yahoo_quote_download)  

## Data format changed
The CSV format of the new API has a few difference compared to the original iChart source. Function `Historical.Get` already taken care of this for you, BUT if you plan to use the raw data for further processing (`Historical.GetRaw` function), check your code to make sure that followings differences are taken care of:

1. The historical data of Open, High, and Low are already adjusted. In older API download, these data fields are not adjusted.

2. The order of data fields in each row is slightly different. The fields of the new API are as following (note that the order of the last two fields are swapped from before).
```
Date, Open, High, Low, Close, Adjusted Close, Volume
```

3. The order of the rows for historical quote by the new API is chronical (vs counter-chronical as the old API).

## Usage

### C#
```cs
using YahooFinanceAPI;

...

private void getHistoricalPrice(string symbol)
{

  //first get a valid token from Yahoo Finance
  while (string.IsNullOrEmpty(Token.Cookie) || string.IsNullOrEmpty(Token.Crumb))
  {
    Token.Refresh();
  }

  List<HistoryPrice> hps = Historical.Get(symbol, DateTime.Now.AddMonths(-1), DateTime.Now);

  //do something

}

private void getRawHistoricalPrice(string symbol)
{

  //first get a valid token from Yahoo Finance
  while (string.IsNullOrEmpty(Token.Cookie) || string.IsNullOrEmpty(Token.Crumb))
  {
    Token.Refresh();
  }

  string csvdata = Historical.GetRaw(symbol, DateTime.Now.AddMonths(-1), DateTime.Now);

  //process further

}
```

### VB
```vb
Imports YahooFinanceAPI

...

Private Sub getHistoricalPrice(symbol As String)

    'first get a valid token from Yahoo Finance
     While (Token.Cookie = "" OrElse Token.Crumb = "")
         Token.Refresh()
     End While

     Dim hps As List(Of HistoryPrice) = Historical.Get(symbol, DateTime.Now.AddMonths(-1), DateTime.Now)

     'do something

 End Sub

 Private Sub getHistoricalPriceRaw(symbol As String)

     'first get a valid token from Yahoo Finance
     While (Token.Cookie = "" OrElse Token.Crumb = "")
         Token.Refresh()
     End While

      Dim csvdata as String = Historical.GetRaw(symbol, DateTime.Now.AddMonths(-1), DateTime.Now)

      'process further

  End Sub
```

## References
1. [Yahoo finance URL not working](http://stackoverflow.com/questions/44030983/yahoo-finance-url-not-working/44036220)
2. [yahoo_quote_download - Python version](https://github.com/c0redumb/yahoo_quote_download)

## Author
This code is written by Dennis Lee

[1]: https://forums.yahoo.net/t5/Yahoo-Finance-help/Is-Yahoo-Finance-API-broken/m-p/251241/highlight/true#M3116
[2]: https://github.com/c0redumb/yahoo_quote_download
