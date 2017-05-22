Imports System.Net
Imports System.IO
Imports System.Text.RegularExpressions

''' <summary>
''' Class for fetching token (cookie and crumb) from Yahoo Finance
''' Copyright Dennis Lee
''' 19 May 2017
''' 
''' reference http://stackoverflow.com/questions/44030983/yahoo-finance-url-not-working/44036220#44036220
''' </summary>
Public Class Token
    Public Shared Property Cookie As String = ""
    Public Shared Property Crumb As String = ""
    Private Shared regex_crumb As Regex

    ''' <summary>
    ''' Refresh cookie and crumb value Yahoo Fianance
    ''' </summary>
    ''' <param name="symbol">Stock ticker symbol</param>
    ''' <returns></returns>
    Public Shared Function Refresh(Optional symbol As String = "SPY") As Boolean

        Try
            Token.Cookie = ""
            Token.Crumb = ""

            Dim url_scrape As String = "https://finance.yahoo.com/quote/{0}?p={0}"
            'url_scrape = "https://finance.yahoo.com/quote/{0}/history"

            Dim url As String = String.Format(url_scrape, symbol)

            Dim request As HttpWebRequest = DirectCast(HttpWebRequest.Create(url), HttpWebRequest)

            request.CookieContainer = New CookieContainer()
            request.Method = "GET"

            Using response As HttpWebResponse = request.GetResponse()

                Dim cookie As String = response.GetResponseHeader("Set-Cookie").Split(";")(0)

                Dim html As String = ""

                Using stream As Stream = response.GetResponseStream()
                    html = New StreamReader(stream).ReadToEnd()
                End Using

                If (html.Length < 5000) Then Return False
                Dim crumb As String = getCrumb(html)
                html = ""

                If crumb IsNot Nothing Then
                    Token.Cookie = cookie
                    Token.Crumb = crumb
                    Debug.Print("Crumb: '{0}', Cookie: '{1}'", crumb, cookie)
                    Return True
                End If

            End Using

        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try

        Return False

    End Function

    ''' <summary>
    ''' Get crumb value from HTML
    ''' </summary>
    ''' <param name="html">HTML code</param>
    ''' <returns></returns>
    Private Shared Function getCrumb(html As String) As String

        Dim crumb As String = Nothing

        Try
            'initialize on first time use
            If regex_crumb Is Nothing Then regex_crumb = New Regex("crumb"":""(?<crumb>\w+)""",
                                                                        RegexOptions.CultureInvariant,
                                                                       TimeSpan.FromSeconds(5))

            Dim matches As MatchCollection = regex_crumb.Matches(html)

            If (matches.Count > 0) Then
                crumb = matches(0).Groups("crumb").Value
            Else
                Debug.Print("Regex no match")
            End If

            'prevent regex memory leak
            matches = Nothing

        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try

        GC.Collect()
        Return Crumb

    End Function

End Class