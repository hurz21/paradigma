'Public Class clsGISfunctions

'Dim latitude As String
'Dim longitude As String

'Function setDefaultThemen() As String
'    Dim themen As String = ""
'    Dim a As String = myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl.Substring(0, 1)
'    Select Case a
'        Case "1", "2", "3", "8"
'            themen = CLstart.myc.MAINiniFile.WertLesen("SachgebietsSpez", "gruppe1")
'        Case "0", "4", "5", "6", "7"
'            themen = CLstart.myc.MAINiniFile.WertLesen("SachgebietsSpez", "gruppe2")
'    End Select
'    Return themen.Replace(";;", ";")
'End Function
' <Obsolete>
'Public Sub GISAufruf_Mittelpunkt(ByVal pt As myPoint)
'    If pt.X < 10000 Then
'        MessageBox.Show("Es konnten keine brauchbaren Koordinaten gefunden werden!", "GIS", MessageBoxButton.OK, MessageBoxImage.Error)
'        Exit Sub
'    End If
'    Dim radius As String = "200"
'    If myGlobalz.sitzung.raumbezugsmodus = "adresse" Then
'        radius = "200"
'    End If
'    Dim http As String = CLstart.mycsimple.getServerHTTPdomainIntranet() & "/cgi-bin/suchdb.cgi?modus=42" & _
'            "&rechts=" & CInt(pt.X) & _
'            "&hoch=" & CInt(pt.Y) & _
'            "&abstand=" & radius & _
'            "&username=" & myGlobalz.sitzung.aktBearbeiter.username & _
'            "&thema=" & setDefaultThemen() '& _
'    '"&format=fix800x600"
'    starten(http)
'End Sub


'Shared Function adresseIstOK(ByVal adr As ParaAdresse) As Boolean
'    If String.IsNullOrEmpty(adr.Gisadresse.gemeindeName) Then
'        Return False
'    End If
'    Return True
'End Function
'<Obsolete>
'Public Sub adressaufrufWebgis()
'    If adresseIstOK(myGlobalz.sitzung.aktADR) Then
'        'Flurstück	 sitzung.aktADR.Gisadresse.gemeindeName
'        ' nachricht_und_Mbox(myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName)
'        'Dim myguid As String = Guid.NewGuid().ToString
'        Dim http$ = CLstart.mycSimple.getServerHTTPdomainIntranet() & "/cgi-bin/suchdb.cgi?modus=1&db=strassehn&gemeinde=" &
'         myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName &
'         "&strasse=" & myGlobalz.sitzung.aktADR.Gisadresse.strasseName &
'         "&hausnummer=" & myGlobalz.sitzung.aktADR.Gisadresse.HausKombi &
'         "&username=" & myGlobalz.sitzung.aktBearbeiter.username &
'         "&thema=" & setDefaultThemen() '& _
'        '"&format=fix800x600" & _
'        '"&guid=" & myguid.ToString
'        starten(http$)
'    Else
'        MessageBox.Show("Leider ist die Adresse unbrauchbar! " & vbCrLf & myGlobalz.sitzung.aktADR.Gisadresse.toString(", "))
'    End If

'End Sub

'Function flurstueckIstOk(ByVal FST As ParaFlurstueck) As Boolean
'    If String.IsNullOrEmpty(FST.normflst.flur.ToString) Then
'        Return False
'    End If
'    Return True
'End Function

'Private Function coordIstOk(ByVal aktp As myPoint) As Boolean
'    If String.IsNullOrEmpty(aktp.X.ToString) Then
'        Return False
'    End If
'    If aktp.X < 340000 Then
'        Return False
'    End If
'    Return True
'End Function

'Public Sub flurstuecksAufrufWebGis(ByVal domain As String)
'    If flurstueckIstOk(myGlobalz.sitzung.aktFST) Then
'    Else
'        MessageBox.Show("Leider ist die Flurstücksangabe unbrauchbar! " & vbCrLf & myGlobalz.sitzung.aktFST.normflst.toString(", "))
'    End If
'    Dim myguid As String = Guid.NewGuid().ToString

'    Dim http$ = "http://" & domain & "/cgi-bin/suchdb.cgi?modus=1&db=flurstueck" &
'     "&gemarkung=" & myGlobalz.sitzung.aktFST.normflst.gemarkungstext &
'     "&flur=" & myGlobalz.sitzung.aktFST.normflst.flur &
'     "&fstueck=" & myGlobalz.sitzung.aktFST.normflst.fstueckKombi &
'     "&username=" & myGlobalz.sitzung.aktBearbeiter.username &
'     "&thema=" & setDefaultThemen() &
'     "&guid=" & myguid.ToString
'    starten(http)

'    ' "&format=fix1200x900" & _
'End Sub

'Shared Sub starten(ByVal Startstring As String)
'    nachricht(Startstring)
'    Process.Start(New ProcessStartInfo(Startstring))
'End Sub
'<Obsolete>
'Public Overloads Function WebGISmittelpunktsAufruf(ByVal bbox As clsRange, layer As String, ByVal domain As String) As String
'    nachricht("WebGISmittelpunktsAufruf")
'    Dim aktp As New myPoint
'    Dim abstand As Double
'    Dim http As String
'    Try
'        nachricht("bbox: " & bbox.toString)
'        aktp.X = bbox.xl + bbox.xdif() / 2
'        aktp.Y = bbox.yl + bbox.ydif() / 2
'        abstand = bbox.xdif
'        layer = layer & ";" & myglobalz.sitzung.aktBearbeiter.username & ";"
'        If Not coordIstOk(aktp) Then
'            MessageBox.Show("Leider ist die Koordinatenangabe unbrauchbar! " & vbCrLf & aktp.toString())
'            Return "fehler"
'        End If
'        '  Dim myguid As String = Guid.NewGuid().ToString
'        http = "http://" & domain & "/cgi-bin/suchdb.cgi?modus=42" &
'         "&rechts=" & CInt(aktp.X) &
'         "&hoch=" & CInt(aktp.Y) &
'         "&abstand=" & abstand &
'         "&username=" & myglobalz.sitzung.aktBearbeiter.username &
'         "&password=" & myglobalz.sitzung.aktBearbeiter.GISPassword &
'         "&thema=" & layer.Replace(";;", ";")
'        nachricht("mittelpunktsAufruf " & vbCrLf & http)
'        Return http
'    Catch ex As Exception
'        nachricht("fehler in WebGISmittelpunktsAufruf:", ex)
'        Return "fehler"
'    End Try
'End Function
'<Obsolete>
'Public Overloads Function WebGISmittelpunktsAufruf(ByVal aktp As myPoint, ByVal domain As String) As String
'    If Not coordIstOk(aktp) Then
'        MessageBox.Show("Leider ist die Koordinatenangabe unbrauchbar! " & vbCrLf & aktp.toString())
'        Return "fehler"
'    End If
'    '  Dim myguid As String = Guid.NewGuid().ToString

'    Dim http$ = "http://" & domain & "/cgi-bin/suchdb.cgi?modus=42" &
'     "&rechts=" & CInt(aktp.X) &
'     "&hoch=" & CInt(aktp.Y) &
'     "&abstand=1000" &
'     "&username=" & myglobalz.sitzung.aktBearbeiter.username &
'     "&thema=" & setDefaultThemen()
'    Return http
'End Function

'Sub allevorgaengeimKreis()
'    Dim http$ = CLstart.mycSimple.getServerHTTPdomainIntranet() & "/cgi-bin/suchdb.cgi?modus=42" &
'                   "&rechts=" & CInt(484304) &
'                   "&hoch=" & CInt(5540236) &
'                   "&abstand=16000" &
'                   "&username=" & myglobalz.sitzung.aktBearbeiter.username &
'                   "&password=" & myglobalz.sitzung.aktBearbeiter.GISPassword &
'                   "&thema=bauantraege;"
'    starten(http$)
''End Sub
'Public Function GoogleMapsAufruf_MittelpunktMItPunkteUebergabe(ByVal bbox As clsRange, htmlformat As Boolean) As String
'    nachricht("GoogleMapsAufruf_MittelpunktMItPunkteUebergabe -----------------------------------")
'    Dim aktp As New myPoint
'    Dim abstand As Double
'    Dim templateFile As String
'    aktp.X = bbox.xl + bbox.xdif() / 2
'    aktp.Y = bbox.yl + bbox.ydif() / 2
'    abstand = bbox.xdif

'    If Not coordIstOk(aktp) Then
'        MessageBox.Show("Leider ist die Koordinatenangabe unbrauchbar! " & vbCrLf & aktp.toString())
'        nachricht("GoogleMapsAufruf_MittelpunktMItPunkteUebergabe fehler" & aktp.toString())
'        Return "fehler"
'    End If
'    'Dim punktliste() As myPoint

'    ReDim CLstart.myc.punktarrayInM(0)
'    CLstart.myc.punktarrayInM(0) = aktp
'    nachricht("1")
'    nachricht("aktp: " & aktp.toString)
'    Dim quellstring As String = modKoordTrans.bildeQuellKoordinatenString(CLstart.myc.punktarrayInM)
'    Dim aufruf As String = modKoordTrans.bildeaufruf(quellstring, CLstart.myc.punktarrayInM.Count.ToString, "UTM", "WINKEL_G")
'    Dim hinweis As String = ""
'    Dim result As String = CLstart.meineHttpNet.meinHttpJob(myGlobalz.ProxyString, aufruf, hinweis)
'    nachricht(hinweis)
'    nachricht("result: " & result)
'    modKoordTrans.getLongLatFromResultSingle(result, longitude, latitude)
'    'GMtemplates.SetLatitude(aktp, longitude, latitude)
'    Dim TEXTKOERPER As String = "Bitte benutzen Sie das <b>Snipping Tool</b> um das Luftbild zu kopieren. " &
'        "Sie können es dann über den Knopf <b>Zwischenablage</b> in Paradigma einfügen!"
'    templateFile = initP.getValue("Beide.GoogleMapsTemplateDir") & "Infowindow.htm"
'    nachricht("2")

'    quellstring = modKoordTrans.bildeQuellKoordinatenString(CLstart.myc.punktarrayInM)
'    aufruf = modKoordTrans.bildeaufruf(quellstring, CLstart.myc.punktarrayInM.Count.ToString, "UTM", "WINKEL_G")
'    nachricht("result: " & result)
'    nachricht("aufruf: " & aufruf)

'    result = CLstart.meineHttpNet.meinHttpJob(myGlobalz.ProxyString, aufruf, hinweis)
'    nachricht(hinweis)
'    '  modKoordTrans.getLongLatFromResultBulk(result)
'    nachricht("3")
'    nachricht("result: " & result)
'    Dim punkteInWinkelkoordinaten() As myPoint
'    punkteInWinkelkoordinaten = modKoordTrans.getLongLatFromResultBulk(result)
'    '  punkteInWinkelkoordinaten = GMtemplates.konvertierePunkteArrayVonUTMnachWinkel(myGlobalz.punktarrayInM)

'    Dim templ As String = GMtemplates.templateEinlesen(templateFile)

'    If templ.IsNothingOrEmpty Or Not htmlformat Then
'        'templatedatei fehlt
'        ''https://maps.google.com/maps?ll=50.0030653020894,8.76937026434553&t=h
'        'Dim http As String = "https://maps.google.com/maps?ll=" & latitude.Replace(",", ".") & "," & longitude.Replace(",", ".") & "&t=h"
'        Dim http As String = "https://www.google.com/maps/@" &
'            latitude.Replace(",", ".") & "," & longitude.Replace(",", ".") & ",355a,20y,41.6t/data=!3m1!1e3"
'        'starten(http)
'        nachricht("http: " & http)
'        Return http
'    Else
'        templ = GMtemplates.templateAnpassen(templ,
'                                      coords:=latitude.Replace(",", ".") & "," & longitude.Replace(",", "."),
'                                      title:="Paradigmavorgang Nr: " & myglobalz.sitzung.aktVorgangsID.ToString,
'                                      polygon:=punkteInWinkelkoordinaten,
'                                      TEXTKOERPER:=TEXTKOERPER)
'        templ = templateAuschreiben(templ)
'        'templateStarten(templ)
'        nachricht("templ: " & templ)
'        Return templ
'    End If
'End Function
'End Class
