Imports System.Net

''' <summary>
''' Class for fetching stock historical price from Yahoo Finance
''' Copyright Dennis Lee
''' 19 May 2017
''' 
''' </summary>
Public Class Historical

    ''' <summary>
    ''' Get stock historical price from Yahoo Finance
    ''' </summary>
    ''' <param name="symbol">Stock ticker symbol</param>
    ''' <param name="start">Starting datetime</param>
    ''' <param name="[end]">Ending datetime</param>
    ''' <returns>List of history price</returns>
    Public Shared Function [Get](symbol As String, start As DateTime, [end] As DateTime) As List(Of HistoryPrice)

        Dim HistoryPrices As New List(Of HistoryPrice)

        Try
            Dim csvData As String = GetRaw(symbol, start, [end])
            If csvData IsNot Nothing Then HistoryPrices = Parse(csvData)

        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try

        Return HistoryPrices

    End Function

    ''' <summary>
    ''' Get raw stock historical price from Yahoo Finance
    ''' </summary>
    ''' <param name="symbol">Stock ticker symbol</param>
    ''' <param name="start">Starting datetime</param>
    ''' <param name="[end]">Ending datetime</param>
    ''' <returns>Raw history price string</returns>
    Public Shared Function GetRaw(symbol As String, start As DateTime, [end] As DateTime) As String

        Dim csvData As String = Nothing

        Try
            Dim url As String = "https://query1.finance.yahoo.com/v7/finance/download/{0}?period1={1}&period2={2}&interval=1d&events=history&crumb={3}"

            'if no token found, refresh it
            If (Token.Cookie = "" Or Token.Crumb = "") Then
                If Not Token.Refresh(symbol) Then Return GetRaw(symbol, start, [end])
            End If

            url = String.Format(url, symbol, Math.Round(DateTimeToUnixTimestamp(start), 0), Math.Round(DateTimeToUnixTimestamp([end]), 0), Token.Crumb)

            Using wc As New WebClient()
                wc.Headers.Add(HttpRequestHeader.Cookie, Token.Cookie)
                csvData = wc.DownloadString(url)
            End Using

        Catch webEx As WebException

            Dim response As HttpWebResponse = DirectCast(webEx.Response, HttpWebResponse)

            'Re-fecthing token
            If (response.StatusCode = HttpStatusCode.Unauthorized) Then
                Debug.Print(webEx.Message)
                Token.Cookie = ""
                Token.Crumb = ""
                Debug.Print("Re-fetch")
                Return GetRaw(symbol, start, [end])
            Else
                Throw
            End If

        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try

        Return csvData

    End Function

    ''' <summary>
    ''' Parse raw historical price data into list
    ''' </summary>
    ''' <param name="csvData"></param>
    ''' <returns></returns>
    Private Shared Function Parse(csvData As String) As List(Of HistoryPrice)

        Dim hps = New List(Of HistoryPrice)

        Try
            Dim rows As String() = csvData.Split(Convert.ToChar(10))

            'row(0) was ignored because is column names 
            For i = 1 To rows.Length - 1 'data is read from oldest to latest

                Dim row As String = rows(i)
                If (String.IsNullOrEmpty(row)) Then Continue For

                Dim cols As String() = row.Split(",")
                If (cols(1) = "null") Then Continue For

                Dim hp As New HistoryPrice()
                With hp
                    .Date = DateTime.Parse(cols(0))
                    .Open = Convert.ToDouble(cols(1))
                    .High = Convert.ToDouble(cols(2))
                    .Low = Convert.ToDouble(cols(3))
                    .Close = Convert.ToDouble(cols(4))
                    .AdjClose = Convert.ToDouble(cols(5))

                    'fixed issue in some currencies quote (e.g: SGDAUD=X)
                    If (cols(6) <> "null") Then .Volume = Convert.ToDouble(cols(6))
                End With

                hps.Add(hp)

            Next

        Catch ex As Exception
            Debug.Print(ex.Message)
        End Try

        Return hps

    End Function

#Region "Unix Timestamp Converter"

    'credits to ScottCher
    'reference http://stackoverflow.com/questions/249760/how-to-convert-a-unix-timestamp-to-datetime-and-vice-versa
    Private Shared Function UnixTimestampToDateTime(unixTimeStamp As Double) As DateTime
        'Unix timestamp Is seconds past epoch
        Return New DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTimeStamp).ToLocalTime()
    End Function

    'credits to Dmitry Fedorkov
    'reference http://stackoverflow.com/questions/249760/how-to-convert-a-unix-timestamp-to-datetime-and-vice-versa
    Private Shared Function DateTimeToUnixTimestamp(dateTime As DateTime) As Double
        'Unix timestamp Is seconds past epoch
        Return (dateTime.ToUniversalTime() - New DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds
    End Function

#End Region

End Class

Public Class HistoryPrice
    Public Property [Date]() As DateTime
    Public Property Open() As Double
    Public Property High() As Double
    Public Property Low() As Double
    Public Property Close() As Double
    Public Property Volume() As Double = 0
    Public Property AdjClose() As Double
End Class