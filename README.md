# YahooFinanceApi 
![Language](https://img.shields.io/badge/.NET%20Framework-4.5-blue.svg?style=flat) [![License](https://img.shields.io/badge/License-MIT%20License-blue.svg?style=flat)](LICENSE)

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

## Quick Start  
1. Run **build.bat** to build DLL  
2. Copy bin\YahooFinanceApi.dll and bin\YahooFinanceApi.xml files into your Visual Studio project folder  
3. In Visual Studio, add reference YahooFinanceApi.dll  

## Usage

### C#
```cs
using YahooFinanceApi;

...

private async Task GetHistoricalPrice(string symbol)
{

  //first get a valid token from Yahoo Finance
  while (string.IsNullOrEmpty(Token.Cookie) || string.IsNullOrEmpty(Token.Crumb))
  {
    await Token.RefreshAsync().ConfigureAwait(false);
  }

  List<HistoryPrice> hps = await Historical.GetAsync(symbol, DateTime.Now.AddMonths(-1), DateTime.Now).ConfigureAwait(false);

  //do something

}

private async Task GetRawHistoricalPrice(string symbol)
{

  //first get a valid token from Yahoo Finance
  while (string.IsNullOrEmpty(Token.Cookie) || string.IsNullOrEmpty(Token.Crumb))
  {
    await Token.RefreshAsync().ConfigureAwait(false);
  }

  string csvdata = await Historical.GetRawAsync(symbol, DateTime.Now.AddMonths(-1), DateTime.Now).ConfigureAwait(false);

  //process further

}
```

### VB
```vb
Imports YahooFinanceApi

...

Private Async Sub GetHistoricalPrice(symbol As String)

    'first get a valid token from Yahoo Finance
     While (Token.Cookie = "" OrElse Token.Crumb = "")
         Await Token.RefreshAsync().ConfigureAwait(False)
     End While

     Dim hps As List(Of HistoryPrice) = Await Historical.GetAsync(symbol, DateTime.Now.AddMonths(-1), DateTime.Now).ConfigureAwait(False)

     'do something

 End Sub

 Private Async Sub GetHistoricalPriceRaw(symbol As String)

     'first get a valid token from Yahoo Finance
     While (Token.Cookie = "" OrElse Token.Crumb = "")
         Await Token.RefreshAsync().ConfigureAwait(False)
     End While

      Dim csvdata as String = Await Historical.GetRawAsync(symbol, DateTime.Now.AddMonths(-1), DateTime.Now).ConfigureAwait(False)

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
