Public Class clsGISfunctions

    Dim latitude As String
    Dim longitude As String

    Function setDefaultThemen() As String
        Dim themen As String = ""
        Dim a As String = myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl.Substring(0, 1)
        Select Case a
            Case "1", "2", "3", "8"
                themen = "tk5;altis12"
            Case "0", "4", "5", "6", "7"
                themen = "realshapeopak;natlandgeb;kompensation;gemarkung_"
        End Select
        ' themen = String.Format("{0};raumbez", themen)
        Return themen.Replace(";;", ";")
    End Function

    Public Sub GISAufruf_Mittelpunkt(ByVal pt As myPoint)
        If pt.X < 10000 Then
            MessageBox.Show("Es konnten keine brauchbaren Koordinaten gefunden werden!", "GIS", MessageBoxButton.OK, MessageBoxImage.Error)
            Exit Sub
        End If
        Dim radius As String = "200"
        If myGlobalz.sitzung.raumbezugsmodus = "adresse" Then
            radius = "200"
        End If
        Dim http As String = CLstart.mycsimple.getServerHTTPdomainIntranet() & "/cgi-bin/suchdb.cgi?modus=42" & _
                "&rechts=" & CInt(pt.X) & _
                "&hoch=" & CInt(pt.Y) & _
                "&abstand=" & radius & _
                "&username=" & myGlobalz.sitzung.aktBearbeiter.username & _
                "&thema=" & setDefaultThemen() '& _
        '"&format=fix800x600"
        starten(http)
    End Sub


    Shared Function adresseIstOK(ByVal adr As ParaAdresse) As Boolean
        If String.IsNullOrEmpty(adr.Gisadresse.gemeindeName) Then
            Return False
        End If
        Return True
    End Function

    Public Sub adressaufruf()
        If adresseIstOK(myGlobalz.sitzung.aktADR) Then
            'Flurstück	 sitzung.aktADR.Gisadresse.gemeindeName
            ' nachricht_und_Mbox(myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName)
            'Dim myguid As String = Guid.NewGuid().ToString
            Dim http$ = CLstart.mycsimple.getServerHTTPdomainIntranet() & "/cgi-bin/suchdb.cgi?modus=1&db=strassehn&gemeinde=" & _
             myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName & _
             "&strasse=" & myGlobalz.sitzung.aktADR.Gisadresse.strasseName & _
             "&hausnummer=" & myGlobalz.sitzung.aktADR.Gisadresse.HausKombi & _
             "&username=" & myGlobalz.sitzung.aktBearbeiter.username & _
             "&thema=" & setDefaultThemen() '& _
            '"&format=fix800x600" & _
            '"&guid=" & myguid.ToString
            starten(http$)
        Else
            MessageBox.Show("Leider ist die Adresse unbrauchbar! " & vbCrLf & myGlobalz.sitzung.aktADR.Gisadresse.toString(", "))
        End If

    End Sub

    Function flurstueckIstOk(ByVal FST As ParaFlurstueck) As Boolean
        If String.IsNullOrEmpty(FST.normflst.flur.ToString) Then
            Return False
        End If
        Return True
    End Function

    Private Function coordIstOk(ByVal aktp As myPoint) As Boolean
        If String.IsNullOrEmpty(aktp.X.ToString) Then
            Return False
        End If
        If aktp.X < 340000 Then
            Return False
        End If
        Return True
    End Function

    Public Sub flurstuecksAufruf(ByVal domain As String)
        If flurstueckIstOk(myGlobalz.sitzung.aktFST) Then
        Else
            MessageBox.Show("Leider ist die Flurstücksangabe unbrauchbar! " & vbCrLf & myGlobalz.sitzung.aktFST.normflst.toString(", "))
        End If
        Dim myguid As String = Guid.NewGuid().ToString

        Dim http$ = "http://" & domain & "/cgi-bin/suchdb.cgi?modus=1&db=flurstueck" & _
         "&gemarkung=" & myGlobalz.sitzung.aktFST.normflst.gemarkungstext & _
         "&flur=" & myGlobalz.sitzung.aktFST.normflst.flur & _
         "&fstueck=" & myGlobalz.sitzung.aktFST.normflst.fstueckKombi & _
         "&username=" & myGlobalz.sitzung.aktBearbeiter.username & _
         "&thema=" & setDefaultThemen() & _
         "&guid=" & myguid.ToString
        starten(http)

        ' "&format=fix1200x900" & _
    End Sub

    Sub starten(ByVal Startstring As String)
        nachricht(Startstring)
        Process.Start(New ProcessStartInfo(Startstring))
    End Sub

    Public Overloads Sub mittelpunktsAufruf(ByVal bbox As clsRange, layer As String, ByVal domain As String, themen As String)
        nachricht("mittelpunktsAufruf")
        Dim aktp As New myPoint
        aktp.X = bbox.xl + bbox.xdif() / 2
        aktp.Y = bbox.yl + bbox.ydif() / 2
        Dim abstand = bbox.xdif
        layer = layer & ";" & myGlobalz.sitzung.aktBearbeiter.username & ";"
        If Not coordIstOk(aktp) Then
            MessageBox.Show("Leider ist die Koordinatenangabe unbrauchbar! " & vbCrLf & aktp.toString())
            Exit Sub
        End If
        '  Dim myguid As String = Guid.NewGuid().ToString
        Dim http As String = "http://" & domain & "/cgi-bin/suchdb.cgi?modus=42" &
         "&rechts=" & CInt(aktp.X) &
         "&hoch=" & CInt(aktp.Y) &
         "&abstand=" & abstand &
         "&username=" & myGlobalz.sitzung.aktBearbeiter.username &
         "&password=" & myGlobalz.sitzung.aktBearbeiter.GISPassword &
         "&thema=" & layer.Replace(";;", ";")
        nachricht("mittelpunktsAufruf " & vbCrLf & http)
        starten(http)
    End Sub

    Public Overloads Sub mittelpunktsAufruf(ByVal aktp As myPoint, ByVal domain As String, themen As String)
        If Not coordIstOk(aktp) Then
            MessageBox.Show("Leider ist die Koordinatenangabe unbrauchbar! " & vbCrLf & aktp.toString())
            Exit Sub
        End If
        Dim abstand As String = "1000"
        If aktp.z > 0 Then
            abstand = aktp.z.ToString
        End If

        Dim http As String = "http://" & domain & "/cgi-bin/suchdb.cgi?modus=42" &
         "&rechts=" & CInt(aktp.X) &
         "&hoch=" & CInt(aktp.Y) &
         "&abstand=" & abstand &
         "&username=" & myGlobalz.sitzung.aktBearbeiter.username &
        "&thema=" & themen
        starten(http)
    End Sub

    Sub allevorgaengeimKreis()
        Dim http$ = CLstart.mycsimple.getServerHTTPdomainIntranet() & "/cgi-bin/suchdb.cgi?modus=42" & _
                       "&rechts=" & CInt(484304) & _
                       "&hoch=" & CInt(5540236) & _
                       "&abstand=16000" & _
                       "&username=" & myGlobalz.sitzung.aktBearbeiter.username & _
                       "&password=" & myGlobalz.sitzung.aktBearbeiter.GISPassword & _
                       "&thema=raumbez;bauantraege;"
        starten(http$)
    End Sub
    Public Sub GoogleMapsAufruf_Mittelpunkt(ByVal bbox As clsRange)
        nachricht("mittelpunktsAufruf")
        Dim aktp As New myPoint
        Dim abstand As Double
        Dim templateFile As String
        aktp.X = bbox.xl + bbox.xdif() / 2
        aktp.Y = bbox.yl + bbox.ydif() / 2
        abstand = bbox.xdif

        If Not coordIstOk(aktp) Then
            MessageBox.Show("Leider ist die Koordinatenangabe unbrauchbar! " & vbCrLf & aktp.toString())
            Exit Sub
        End If
        Dim punktliste() As myPoint
        ReDim punktliste(0)
        punktliste(0) = aktp
        Dim quellstring As String = modKoordTrans.bildeQuellKoordinatenString(punktliste)
        Dim aufruf As String = modKoordTrans.bildeaufruf(quellstring, punktliste.Count.ToString, "UTM", "WINKEL_G")
        Dim hinweis As String
        Dim result As String = CLstart.meineHttpNet.meinHttpJob(myglobalz.ProxyString, aufruf, hinweis)
        nachricht(hinweis)
        modKoordTrans.getLongLatFromResultSingle(result, longitude, latitude)
        'GMtemplates.SetLatitude(aktp, longitude, latitude)
        Dim TEXTKOERPER As String = "Bitte benutzen Sie das <b>Snipping Tool</b> um das Luftbild zu kopieren. " &
            "Sie können es dann über den Knopf <b>Zwischenablage</b> in Paradigma einfügen!"



        templateFile = initP.getValue("Beide.GoogleMapsTemplateDir") & "Infowindow.htm"

        quellstring = modKoordTrans.bildeQuellKoordinatenString(CLstart.myc.punktarrayInM)
        aufruf = modKoordTrans.bildeaufruf(quellstring, punktliste.Count.ToString, "UTM", "WINKEL_G")

        result = CLstart.meineHttpNet.meinHttpJob(myglobalz.ProxyString, aufruf, hinweis)
        nachricht(hinweis)
        '  modKoordTrans.getLongLatFromResultBulk(result)

        Dim punkteInWinkelkoordinaten() As myPoint
        punkteInWinkelkoordinaten = modKoordTrans.getLongLatFromResultBulk(result)
        '  punkteInWinkelkoordinaten = GMtemplates.konvertierePunkteArrayVonUTMnachWinkel(myGlobalz.punktarrayInM)

        Dim templ As String = GMtemplates.templateEinlesen(templateFile)

        If templ.IsNothingOrEmpty Then
            'templatedatei fehlt
            ''https://maps.google.com/maps?ll=50.0030653020894,8.76937026434553&t=h
            Dim http As String = "https://maps.google.com/maps?ll=" & latitude & "," & longitude & "&t=h"
            starten(http)
        Else
            templ = GMtemplates.templateAnpassen(templ,
                                          coords:=latitude.Replace(",", ".") & "," & longitude.Replace(",", "."),
                                          title:="Paradigmavorgang Nr: " & myGlobalz.sitzung.aktVorgangsID.ToString,
                                          polygon:=punkteInWinkelkoordinaten,
                                          TEXTKOERPER:=TEXTKOERPER)
            templ = templateAuschreiben(templ)
            templateStarten(templ)
        End If

    End Sub
End Class
