Module Module1
    Public Property area As Double
    Property mycgi As clsCGI4VBNET
    Property enc As Text.Encoding
    Property vid As String
    Property modus As String
    Property serial As String
    Property fs As String
    Property gemcode As String
    Property FsPositionInShapeFile As String = "1"
    Property username As String
    'http://w2gis02.kreis-of.local/cgi-bin/apps/paradigmaex/layer2shpfile/layer2shpfile.cgi?user=Feinen_J&vid=9779&modus=einzeln
    'http://w2gis02.kreis-of.local/cgi-bin/apps/paradigmaex/layer2shpfile/layer2shpfile.cgi?user=Feinen_J&vid=23608&modus=einzeln
    Sub Main()
        mycgi = New clsCGI4VBNET("dr.j.feinen@kreis-offenbach.de")
        Dim isDebugmode As Boolean = False
        Dim shapefile As String
        Dim hinweis As String
        getCgiParams(isDebugmode)
        protokoll()
        showCgiParams()
        enc = System.Text.Encoding.GetEncoding(("iso-8859-2"))

        'username = "feij"
        'vid = "22535"
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
        'Dim erfolg As Boolean = getSerialFromShapeOHneDLL(shapefile, _
        '                                                  CStr(FsPositionInShapeFile), _
        '                                                  fs,
        '                                                  serial,
        '                                                  CDbl(area))
        Dim ttt As New tools
        myGlobalz.Bearbeiter = username
        ttt.KarteErstellen(CInt(vid), "", False, "einzeln", False, myGlobalz.Bearbeiter)
        ' erfolg = ttt.shapefileLayerErzeugen(username, vid)
        l("erfolg=" & erfolg & Environment.NewLine() &
                    serial & Environment.NewLine &
                   "area:" & area)
        mycgi.SendHeader("serialserver")

        If erfolg Then
            mycgi.Send("</br>shapefiles werden erzeugt-----------")
            mycgi.Send("erfolg=" & erfolg & Environment.NewLine() &
                          serial & Environment.NewLine &
                          "area:" & area)
            showCgiParams()
            l("vor doDatenbank -------------------------------------")
            Dim ret As Integer
            ret = doDatenbank()
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
    Public Sub l(text As String)
        My.Application.Log.WriteEntry(text)
    End Sub
    Private Sub getCgiParams(isDebugmode As Boolean)
        l("getCgiParams -------------------------" & isDebugmode)
        Try
            If isDebugmode Then
                username = "feinen_j"
                vid = "9779"
                modus = "einzeln"
                'rid = "26929"
                'fs = "FS0607280020000100700" 'der dateiname kann nicht über cgi geleitet werden. funzt nicht
                'gemcode = "728"
                '        rbtyp   fst = 2
            Else
                username = mycgi.GetCgiValue("user")
                vid = (mycgi.GetCgiValue("vid"))
                modus = (mycgi.GetCgiValue("modus"))
            End If
        Catch ex As Exception
            l("fehler in getCgiParams: " & ex.ToString)
        End Try
    End Sub
    Private Sub showCgiParams()
        l("-----------------showCgiParams ---------------------- ")
        l("username: " & username)
        l("modus: " & modus)
        l("vid: " & vid)
        l("gemcode: " & gemcode)
        l("fs: " & fs)
        l("---------------- showCgiParams ende ")
    End Sub
    Private Sub protokoll()
        With My.Application.Log.DefaultFileLogWriter
            .CustomLocation = "d:\websys\" & "protokoll"
            '.CustomLocation = "c:\" & "protokoll"
            .BaseFileName = "layer2shpfileServer" & username & "_" & vid
            .AutoFlush = True
            .Append = False
        End With
        l("protokoll now: " & Now)
    End Sub
    Private Function eingabeist_ok() As Boolean
        l("eingabeist_ok-------------------")
        Try
            If CInt(vid) < 1 Then
                l("Fehler :vid) < 1  ")
                Return False
            End If
            If String.IsNullOrEmpty(username) Then
                l("Fehler :username " & username)
                Return False
            End If

            Return True
        Catch ex As Exception
            l("Fehler ineingabeist_ok : " & ex.ToString)
            Return False
        End Try
    End Function
End Module
