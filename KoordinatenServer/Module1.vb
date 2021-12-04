Module Module1
    Property mycgi As clsCGI4VBNET
    Property enc As Text.Encoding

    Property Q_CoordString As String
    Property Q_CoordCount As String ' anhzahl der Koordinatenpaare
    Property Q_Coordsys As String
    Property Q_Elipsoid As String
    Property Q_notation As String
    Property Q_Algoritmus As String ' coordtrans oder coordtrans3 
    Property Z_CoordString As String
    Property Z_Coordsys As String
    Property Z_Elipsoid As String
    Property Z_notation As String
    Property Z_streifen As String
    Property result As Integer
    Property username As String
    'http://w2gis02.kreis-of.local/cgi-bin/apps/paradigmaex/KoordinatenServer/KoordinatenServer.cgi?user=Feinen_J&Q_CoordString=484740.0;5545390.0_482571.0;5543487.0_487410.0;5544391.0&Q_CoordCount=3&Q_Coordsys=UTM&Z_Coordsys=WINKEL_G
    Sub Main()
        mycgi = New clsCGI4VBNET("dr.j.feinen@kreis-offenbach.de")
        Dim isDebugmode As Boolean = False
        Dim erfolg As Boolean
        getCgiParams(isDebugmode)
        protokoll()
        l("ööööööööööö " & mycgi.GetCgiValue("user"))
        l("ääääää " & mycgi.GetCgiValue("username"))
        showCgiParams()
        enc = System.Text.Encoding.GetEncoding(("iso-8859-2"))
        If eingabeist_ok() Then
            erfolg = prepWeb.doKonvertierung(Q_CoordString,
                                              Q_CoordCount,
                                              Q_Coordsys,
                                              Z_Coordsys,
                                              Z_CoordString)
        Else
            erfolg = False
        End If
        mycgi.SendHeaderAJAX()
        Dim antwortstring As String

        If erfolg Then
            antwortstring = Z_CoordString
            mycgi.Send(antwortstring)
            'l(hinweis)
            'mycgi.Send(hinweis)
        Else
            'abbruch
            antwortstring = "1_fehler"
            mycgi.Send(antwortstring)
        End If
        l("final cut")
    End Sub
    Sub nachricht(text As String)
        l(text)
    End Sub

    Public Sub l(text As String)
        My.Application.Log.WriteEntry(text)
        If text.ToLower.StartsWith("fehler") Or text.ToLower.StartsWith("error") Then
            Dim test As Boolean = mailrausSMTP(von:="dr.j.feinen@kreis-offenbach.de",
                                                an:="dr.j.feinen@kreis-offenbach.de",
                                                betreff:="Fehler in koordinatenserver: " & text & ", user: " & username,
                                                nachricht:=text.Replace(vbCrLf, "<br>"),
                                                anHang:="",
                                                iminternet:=False,
                                                mailserverinternet:="",
                                                mailserverintranet:="",
                                                hinweis:="",
                                                inifile:="",
                                                CC:="")
        End If
    End Sub
    Private Sub getCgiParams(istdebugmode As Boolean)
        l("getCgiParams -------------------------" & istdebugmode)
        Try
            If istdebugmode Then
                Q_CoordString = "484740.0;5545390.0_482571.0;5543487.0_487410.0;5544391.0"
                Q_CoordCount = "3"
                Q_Coordsys = "UTM"
                Z_Coordsys = "WINKEL_G"

                Q_CoordString = "8.7913743;50.0155642"
                Q_CoordCount = "3"
                Q_Coordsys = "WINKEL_G"
                Z_Coordsys = "UTM"
            Else
                username = mycgi.GetCgiValue("username")
                If username = String.Empty Then
                    username = mycgi.GetCgiValue("user")
                End If
                Q_CoordString = mycgi.GetCgiValue("Q_CoordString")
                Q_CoordCount = (mycgi.GetCgiValue("Q_CoordCount"))
                Q_Coordsys = (mycgi.GetCgiValue("Q_Coordsys"))
                Z_Coordsys = (mycgi.GetCgiValue("Z_Coordsys"))
            End If
        Catch ex As Exception
            l("fehler in getCgiParams: " & ex.ToString)
        End Try
    End Sub
    Private Sub protokoll()
        With My.Application.Log.DefaultFileLogWriter
#If DEBUG Then
            .CustomLocation = "c:\\" & "protokoll"
#Else
               .CustomLocation = "d:\websys\" & "protokoll"
#End If

            .BaseFileName = "KoordinatenServer_" & username
            .AutoFlush = True
            .Append = False
        End With
        l("protokoll now: " & Now)
    End Sub

    Private Function showCgiParams() As String
        l("-----------------showCgiParams ---------------------- ")
        Try

            Dim stru As New Text.StringBuilder
            stru.Append("EingabeParameter:----------------" & Environment.NewLine)
            stru.Append("username: " & username & Environment.NewLine)
            stru.Append("Q_CoordString: " & Q_CoordString & Environment.NewLine)
            stru.Append("Q_CoordString: " & Q_CoordCount & Environment.NewLine)
            stru.Append(("q_coordsys: " & Q_Coordsys & Environment.NewLine))
            'stru.Append(("Q_Elipsoid: " & Q_Elipsoid))
            'stru.Append("Q_notation: " & Q_notation)
            'stru.Append("Q_Algoritmus: " & Q_Algoritmus)

            stru.Append("Z_CoordString: " & Z_CoordString & Environment.NewLine)
            stru.Append("Z_coordsys: " & Z_Coordsys & Environment.NewLine)
            'stru.Append("Z_Elipsoid: " & Z_Elipsoid)
            'stru.Append("Z_notation: " & Z_notation)
            'stru.Append("Z_streifen: " & Z_streifen)
            stru.Append("---------------- showCgiParams ende " & Environment.NewLine)
            l(stru.ToString)
            Return stru.ToString
        Catch ex As Exception
            l("fehler in showCgiParams: " & ex.ToString)
        End Try
    End Function

    Private Function eingabeist_ok() As Boolean
        l("eingabeist_ok-------------------")
        Try

            'If CInt(IN_x) < 1 Or CInt(IN_y) < 1 Then
            '    l("Fehler :vid) < 1 Or CInt(rid) < 1")
            '    Return False
            'End If
            'If String.IsNullOrEmpty(fs) Then
            '    l("Fehler :fs " & fs)
            '    Return False
            'End If
            'If String.IsNullOrEmpty(gemcode) Then
            '    l("Fehler :gemcode " & gemcode)
            '    Return False
            'End If
            Return True
        Catch ex As Exception
            l("Fehler ineingabeist_ok : " & ex.ToString)
            Return False
        End Try
    End Function
End Module
