Imports System.Data
Imports MySql.Data.MySqlClient
Module Module1
    Public _mydb As New clsDatenbankZugriff
    Public Property myconn As MySqlConnection

    Property mycgi As clsCGI4VBNET
    Property enc As Text.Encoding

    Dim quellKoordinate As New clsGEOPoint
    Property result As Integer
    Property username As String
    Dim mitdom As Integer = 1
    Dim mitdgm As Integer = 0
    '483437, 5539056
    'http://w2gis02.kreis-of.local/cgi-bin/apps/paradigmaex/zwertServer/zwertServer.cgi?user=Feinen_J&rechts=483437&hoch=5539056
    Sub Main()
        mycgi = New clsCGI4VBNET("dr.j.feinen@kreis-offenbach.de")
        Dim isDebugmode As Boolean = false
        Dim erfolg As Boolean
        getCgiParams(isDebugmode)
        protokoll()
        showCgiParams()
        enc = System.Text.Encoding.GetEncoding(("iso-8859-2"))

        Dim defaultDBrec As New clsDBspecMYSQL
        defaultDBrec.mydb.Host = "w2gis02"
        defaultDBrec.mydb.username = "root"
        defaultDBrec.mydb.password = "lkof4"
        defaultDBrec.mydb.Schema = "3d"
        defaultDBrec.mydb.Tabelle = "dom"
        defaultDBrec.mydb.dbtyp = "mysql"
        Dim indexneu As String = "indexneu.txt"
        Dim quelldirDGM, quelldirDOM, resultDGM, resultDOM As String
        If isDebugmode = False Then
            indexneu = "indexneu.txt"
            quelldirDOM = "d:\3d\DOM1"
            quelldirDGM = "d:\3d\DGM1"
        Else
            indexneu = "indexneu.txt"
            quelldirDOM = "l:\3d\DOM1"
            quelldirDGM = "l:\3d\DGM1"
        End If
        l("indexneu: " & indexneu)
        If eingabeist_ok() Then
            If CBool(mitdom) Then
                resultDOM = prepWeb.getZwert(quellKoordinate, defaultDBrec, indexneu, quelldirDOM)
            End If
            If CBool(mitdgm) Then
                resultDGM = prepWeb.getZwert(quellKoordinate, defaultDBrec, indexneu, quelldirDGM)
            End If

            erfolg = True
        Else
            erfolg = False
        End If
        l("vorajax")
        mycgi.SendHeaderAJAX()
        Dim antwortstring As String

        If erfolg Then
            antwortstring = resultDOM & "#" & resultDGM
            mycgi.Send(antwortstring)
            l("-----result")
            l(antwortstring)
            l("-----result")
            If isDebugmode Then
                MsgBox(antwortstring)
            End If
            'mycgi.Send(hinweis)
        Else
            'abbruch
            ' antwortstring = "1_fehler"
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
                                                betreff:="Fehler in zwertserver: " & text & ", user: " & username,
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
                quellKoordinate.GKrechts = 483600 '483601'472586 '483437 
                quellKoordinate.GKhoch = 5539061 ' 5539060'5533476 '5539056
                mitdom = 1
                mitdgm = 0



            Else
                username = mycgi.GetCgiValue("username")
                quellKoordinate.GKrechts = CDbl(mycgi.GetCgiValue("rechts"))
                quellKoordinate.GKhoch = CDbl((mycgi.GetCgiValue("hoch")))
                mitdom = CInt((mycgi.GetCgiValue("mitdom")))
                mitdgm = CInt((mycgi.GetCgiValue("mitdgm")))

            End If
        Catch ex As Exception
            l("fehler in getCgiParams: " & ex.ToString)
        End Try
    End Sub
    Private Sub protokoll()
        With My.Application.Log.DefaultFileLogWriter
            '.CustomLocation = "d:\websys\protokoll\mapshare_prequel.log"
            .CustomLocation = "d:\websys\" & "protokoll"
            .BaseFileName = "zwertServer_" & username
            '  .Location = Logging.LogFileLocation.ExecutableDirectory
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
            stru.Append("rechts: " & quellKoordinate.GKrechts & Environment.NewLine)
            stru.Append(("hoch: " & quellKoordinate.GKhoch & Environment.NewLine))
            stru.Append(("mitdgm: " & mitdgm & Environment.NewLine))
            stru.Append(("mitdom: " & mitdom & Environment.NewLine))
            stru.Append("---------------- showCgiParams ende " & Environment.NewLine)
            l(stru.ToString)
            Return stru.ToString
        Catch ex As Exception
            l("fehler in showCgiParams: " & ex.ToString)
        End Try
    End Function

    Private Function eingabeist_ok() As Boolean
        Return True
    End Function


End Module
