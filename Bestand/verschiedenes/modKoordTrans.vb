Imports System.Net
Imports System.IO
Imports System.Text

Module modKoordTrans
    Public PaareTrenner As Char = CChar("_")
    Public xyTrenner As Char = CChar(";")



    Function bildeQuellKoordinatenString(punktarrayInM As myPoint()) As String
        Dim QkoordString As String = ""
        Dim aktp = New myPoint()
        Try
            For Each punkt In punktarrayInM
                QkoordString = QkoordString & punkt.X.ToString.Replace(",", ".") & xyTrenner & punkt.Y.ToString.Replace(",", ".") & PaareTrenner
            Next
            QkoordString = QkoordString.Substring(0, QkoordString.Length - 1)
            Return QkoordString
        Catch ex As Exception
            nachricht("fehler in bildeQuellKoordinatenString: ",ex)
            Return ""
        End Try
    End Function

    Function bildeaufruf(qkoordstring As String, qkoordcount As String, qkoordSys As String, zkoordSys As String) As String
        Dim a As String
        Try
            a = "http://w2gis02.kreis-of.local/cgi-bin/apps/paradigmaex/KoordinatenServer/KoordinatenServer.cgi?user=" & myGlobalz.sitzung.aktBearbeiter.username &
                      "&Q_CoordString=" & qkoordstring &
                      "&Q_CoordCount=" & qkoordcount &
                      "&Q_Coordsys=" & qkoordSys &
                      "&Z_Coordsys=" & zkoordSys
            Return a
        Catch ex As Exception
            nachricht("fehler in bildeaufruf:",ex)
            Return ""
        End Try
    End Function

    Sub getLongLatFromResultSingle(result As String, ByRef longitude As String, ByRef latitude As String)
        Dim a As String()
        Try
            result = result.Trim
            a = result.Split(xyTrenner)
            longitude = a(0)
            latitude = a(1)
        Catch ex As Exception

        End Try
    End Sub

    Function getLongLatFromResultBulk(result As String) As myPoint()
        Dim paare, a As String()
        Dim punkte() As myPoint
        Try
            result = result.Trim
            paare = result.Split(PaareTrenner)
            ReDim punkte(paare.Count - 1)
            For i = 0 To paare.GetUpperBound(0)
                a = paare(i).Split(xyTrenner)
                punkte(i) = New myPoint
                punkte(i).X = CDbl(a(0))
                punkte(i).Y = CDbl(a(1))
            Next
            Return punkte
        Catch ex As Exception
            l("fehler in " & ex.ToString)
            Return Nothing
        End Try
    End Function
    Public Function Mykoordtransform(proxystring As String, aufrufstring As String) As String
        Dim myProxy As WebProxy
        Dim newUri As Uri
        Dim antwort As String="?"
        Try
            myProxy = New WebProxy()
            If Not String.IsNullOrEmpty(proxystring) Then
                newUri = New Uri(proxystring)
                myProxy.Address = newUri
                myProxy.Credentials = CredentialCache.DefaultCredentials
            End If
            Dim myReq As HttpWebRequest = DirectCast(WebRequest.Create(aufrufstring), 
                                                     HttpWebRequest)
            myReq.Proxy = myProxy
            Dim webResponse As HttpWebResponse
            Try
                webResponse = TryCast(myReq.GetResponse(), HttpWebResponse)
            Catch
                Return Nothing
            End Try
            Dim encode As Encoding = System.Text.Encoding.GetEncoding("utf-8")
            If webResponse IsNot Nothing Then
                If webResponse.StatusCode = HttpStatusCode.OK Then
                    Dim ReceiveStream As Stream = webResponse.GetResponseStream()
                    Dim readStream As New StreamReader(ReceiveStream, encode)
                    antwort = readStream.ReadToEnd()
                    readStream.Dispose()
                    ReceiveStream.Dispose()
                End If
            End If
            Return antwort
        Catch ex As Exception
            nachricht("fehler in Mykoordtransform: ",ex)
            Return ""
        End Try
    End Function

End Module
