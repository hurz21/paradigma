

Module modSerialserver
  
    Public Property area As Double
    Property mycgi As clsCGI4VBNET
    Property enc As Text.Encoding
    Property vid As String
    Property rid As String
    Property serial As String
    Property fs As String
    Property gemcode As String
    Property FsPositionInShapeFile As String = "1"
    Property username As String
    Property postgis As String
    Public host, datenbank, schema, tabelle, dbuser, dbpw, dbport As String
    'http://w2gis02.kreis-of.local/cgi-bin/apps/paradigmaex/serialserver/serialserver.cgi?user=Feinen_J&vid=22535&rid=26929&gemcode=728&fs=FS0607280020000200000
    Sub Main()
        mycgi = New clsCGI4VBNET("dr.j.feinen@kreis-offenbach.de")
        Dim isDebugmode As Boolean = true
        Dim shapefile As String
        Dim hinweis As String
        getCgiParams(isDebugmode)
        protokoll(isDebugmode)
        showCgiParams()
        enc = System.Text.Encoding.GetEncoding(("iso-8859-2"))

        gemcode = "729"
        If isDebugmode Then
            vid = "9609"
            rid = "36537"
            postgis = "1"
            fs = "FS0607290050049100000"
        End If

        If Not eingabeist_ok() Then
            mycgi.SendHeader("Eingaben unvollständig")
            mycgi.Send("Eingaben unvollständig")
            End
        End If
        shapefile = "d:\paradigma\gemcode\" & gemcode & ".shp"
        If isDebugmode Then
            shapefile = shapefile.Replace("d:", "l:")
        End If
        l("Shapefile: " & shapefile)
        Dim erfolg As Boolean

        If postgis = "1" Then
            l("pg erkannt")

            host = "w2gis02" : datenbank = "postgis20" : schema = "flurkarte" : tabelle = "basis_f" : dbuser = "postgres" : dbpw = "lkof4" : dbport = "5432"
            erfolg = modSerialFromPostgis.getSerialFromPostgis(host, datenbank, schema, tabelle, dbuser, dbpw, dbport,
                                                     fs,
                                                     serial,
                                                     CDbl(area))
        Else
            l("shape erkannt")
            erfolg = getSerialFromShapeOHneDLL(shapefile, _
                                                     CStr(FsPositionInShapeFile), _
                                                     fs,
                                                     serial,
                                                     CDbl(area))
        End If

        l("erfolg=" & erfolg & Environment.NewLine() &
                    serial & Environment.NewLine &
                   "area:" & area)
        mycgi.SendHeader("serialserver")

        If erfolg Then
            mycgi.Send("</br>flurstücksgeometrie erfolgreich retrieved")
            mycgi.Send("erfolg=" & erfolg & Environment.NewLine() &
                          serial & Environment.NewLine &
                          "area:" & area)
            showCgiParams()
            l("vor doDatenbank -------------------------------------")
            Dim ret As Integer



            ret = doDatenbank2()
            ret = 1
            If ret > 0 Then
                hinweis = "</br>Datenbankeintrag erfolgreich: " & ret
            Else
                hinweis = "</br>Datenbankeintrag NICHT erfolgreich: " & ret
            End If
            l(hinweis)
            mycgi.Send(hinweis)
        Else
            'abbruch
            mycgi.Send("</br>flurstücksgeometrie NICHT erfolgreich retrieved")
            'meldung an admin
            l("f</br>ehler babbisch wie lakritz Flurstück konnte im Shapefile nicht gefunden werden!")
            l(fs & " in " & shapefile)
            mycgi.Send("fehler babbisch wie lakritz Flurstück konnte im Shapefile nicht gefunden werden")
            mycgi.Send(fs & " in " & shapefile)
        End If

        l("final cut")
    End Sub

    Private Function eingabeist_ok() As Boolean
        l("eingabeist_ok-------------------")
        Try
            If CInt(vid) < 1 Or CInt(rid) < 1 Then
                l("Fehler :vid) < 1 Or CInt(rid) < 1")
                Return False
            End If
            If String.IsNullOrEmpty(fs) Then
                l("Fehler :fs " & fs)
                Return False
            End If
            If String.IsNullOrEmpty(gemcode) Then
                l("Fehler :gemcode " & gemcode)
                '    Return False
            End If
            Return True
        Catch ex As Exception
            l("Fehler ineingabeist_ok : " & ex.ToString)
            Return False
        End Try
    End Function

    Private Sub protokoll(isDebugmode As Boolean)
        With My.Application.Log.DefaultFileLogWriter
            '.CustomLocation = "d:\websys\protokoll\mapshare_prequel.log"
            If Not isDebugmode Then
                .CustomLocation = "d:\websys\" & "protokoll"
            Else
                .CustomLocation = "c:\" & "protokoll"
            End If

            .BaseFileName = "serialserver_" & username & "_" & vid & "_" ' & rid
            '  .Location = Logging.LogFileLocation.ExecutableDirectory
            .AutoFlush = True
            .Append = False
        End With
        l("protokoll now: " & Now)
    End Sub

    Private Sub showCgiParams()
        l("-----------------showCgiParams ---------------------- ")
        l("username: " & username)
        l("rid: " & rid)
        l("vid: " & vid)
        l("gemcode: " & gemcode)
        l("fs: " & fs)
        l("postgis: " & postgis)
        l("---------------- showCgiParams ende ")
    End Sub

    Private Sub getCgiParams(istdebugmode As Boolean)
        l("getCgiParams -------------------------" & istdebugmode)
        Try
            If istdebugmode Then
                'username = "feinen_j"
                'vid = "22535"
                'rid = "26929"
                'fs = "FS0607280020000100700" 'der dateiname kann nicht über cgi geleitet werden. funzt nicht
                'gemcode = "728"
                '        rbtyp   fst = 2
            Else
                username = mycgi.GetCgiValue("user")
                vid = (mycgi.GetCgiValue("vid"))
                rid = (mycgi.GetCgiValue("rid"))
                gemcode = (mycgi.GetCgiValue("gemcode"))
                fs = (mycgi.GetCgiValue("fs"))
                postgis = (mycgi.GetCgiValue("postgis"))
            End If
        Catch ex As Exception
            l("fehler in getCgiParams: " & ex.ToString)
        End Try
    End Sub
    Public Sub l(text As String)
        My.Application.Log.WriteEntry(text)
        If text.ToLower.StartsWith("fehler") Or text.ToLower.StartsWith("error") Then
            Dim test As Boolean = mailrausSMTP(von:="dr.j.feinen@kreis-offenbach.de",
                                                an:="dr.j.feinen@kreis-offenbach.de",
                                                betreff:="Fehler in serialServer: " & text & ", user: " & username &
                                                ", vid: " & vid &
                                                ", rid: " & rid &
                                                ", gemcode: " & gemcode &
                                                ", fs: " & fs,
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





End Module
