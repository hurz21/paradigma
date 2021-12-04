
Class tools
    Public _shapeModul As clsKartenerstellungShape
    Public Property Datei_einzelvorgang As String
    Public Property Datei_vorgangsliste As String
    
    Sub KarteErstellen(ByVal vid As Integer,
                   ByVal sgnummer As String,
                   ByVal Mitverwandten As Boolean,
                   ByVal modus As String,
                   ByVal mitetikett As Boolean,
                   username As String)
        l("in KarteErstellen ---------------------------")
        Dim paradigmaXML As String = "paradigma.xml" 'My.Resources.Resources.ParadigmaKonfigFile
        Dim testfile As New IO.FileInfo(paradigmaXML)
        If Not testfile.Exists Then
            MsgBox("Die Konfiguration konnte nicht gefunden werden")
            End
        End If
        myGlobalz.iniDict = clsINIXML.XMLiniReader(paradigmaXML) '"g:\appsconfig\paradigma.xml")
        l("vor ")
        inidatabase.iniall()
        Dim sql, titel As String
        Dim ebenen(0) As Integer
          l("vor  clsKartenerstellungShape , modus: " & modus)
        _shapeModul = New clsKartenerstellungShape()
        Select Case modus
            Case "einzeln"
                getAlleVorgaengeFuerEinzelVorgang(Mitverwandten, ebenen, vid%)
                _shapeModul.appendix = CStr(myGlobalz.Bearbeiter) 'CStr(vid) '& "_" & myGlobalz.Bearbeiter '& "_" & clsString.normalize(Now.ToString)
            Case "liste"
                getAlleVorgaengeFuerListe(Datei_vorgangsliste, ebenen, vid%, sql$, titel$)
                _shapeModul.appendix = CStr(myGlobalz.Bearbeiter) '& "_" & myGlobalz.Bearbeiter '& "_" & clsString.normalize(Now.ToString)
        End Select
        l("_shapeModul.appendix " & _shapeModul.appendix)
        l(ebenen.Count & " Vorgänge werden bearbeitet!   Bitte warten!")
        VerzeichnisseErreichnen()
        Verzeichnisse_ausgeben()
        Verzeichnisse_anlegen(_shapeModul)
        l(exekuteEinzelVorgang(_shapeModul, _shapeModul.aktBox, ebenen))
        glob2.nachricht("vor webgisPreparieren")
        Dim KartenEbenenName As String = _shapeModul.webgisPreparieren(mitetikett, username)
        glob2.nachricht("KartenEbenenName$: " & KartenEbenenName)
        'aufruf des gis
    End Sub

    Function exekuteEinzelVorgang(ByVal dings As clsKartenerstellungShape, ByVal aktBox As clsRange, ByVal ebenen() As Integer) As String
        Dim hinweis$ = ""
        Dim paradigmaRBtyp As RaumbezugsTyp
        glob2.nachricht("point_shpfile_erzeugen ============================================================ vor")
        Dim lResult As String = dings.point_shpfile_erzeugen(aktBox, ebenen)
        glob2.nachricht("point_shpfile_erzeugen ============================================================ ende")

        glob2.nachricht(String.Format("point_shpfile_erzeugen, ergebnis:{0} sollte leerstring sein.dann ok", lResult))

        glob2.nachricht("polygon_shpfile_erzeugen ============================================================ vor")
        lResult = dings.polygon_shpfile_erzeugen(aktBox, ebenen, RaumbezugsTyp.Flurstueck)
        glob2.nachricht(String.Format("polygon_shpfile_erzeugen, ergebnis:{0} sollte leerstring sein.dann ok", lResult))
        glob2.nachricht("polygon_shpfile_erzeugen ============================================================ ende")

        glob2.nachricht("polyline_shpfile_erzeugen ============================================================ vor")
        lResult = dings.polyline_shpfile_erzeugen(aktBox, ebenen, RaumbezugsTyp.Polyline)
        glob2.nachricht(String.Format("polyline_shpfile_erzeugen, ergebnis:{0} sollte leerstring sein.dann ok", lResult))
        glob2.nachricht("polyline_shpfile_erzeugen ============================================================ ende")
        Return hinweis
    End Function
    Private Sub VerzeichnisseErreichnen()
        l("VerzeichnisseErreichnen")
        _shapeModul.KartenMapfileTemplate = myGlobalz.gis_serverD & "/paradigmacache/vorlagen/raumbezug.map"
        _shapeModul.KartenHTMfileTemplate = myGlobalz.gis_serverD & "/paradigmacache/vorlagen/raumbezug.htm"
        _shapeModul.KartenRoot = myGlobalz.gis_serverD & "\paradigmacache"
        _shapeModul.KartenJPGDir = myGlobalz.gis_serverD & "\paradigmacache\"
        _shapeModul.Kartenprojektdir = _shapeModul.KartenRoot & "\" & _shapeModul.appendix
        clsKartenerstellungShape.kartenDatadir = _shapeModul.Kartenprojektdir & "\data"
        _shapeModul.KartenVorgangsDir = _shapeModul.KartenRoot & "\VORGANG\"
        _shapeModul.KartenIMGDir = _shapeModul.Kartenprojektdir & "\images"
        _shapeModul.KartenMAPfile = _shapeModul.Kartenprojektdir & "\raumbezug.map"
        l("VerzeichnisseErreichnen - ende")
    End Sub
    Private Sub Verzeichnisse_ausgeben()
        l("Ausgabe der verzeichnisse:---------------------------")
        l("shapeModul.KartenMapfileTemplate " & _shapeModul.KartenMapfileTemplate)
        l(" _shapeModul.KartenRoot " & _shapeModul.KartenRoot)
        l("_shapeModul.KartenJPGDir " & _shapeModul.KartenJPGDir)
        l(" _shapeModul.Kartenprojektdir " & _shapeModul.Kartenprojektdir)
        l(" _shapeModul.KartenVorgangsDir " & _shapeModul.KartenVorgangsDir)
        l(" _shapeModul.KartenIMGDir " & _shapeModul.KartenIMGDir)
        l("_shapeModul.KartenMAPfile " & _shapeModul.KartenMAPfile)
        l("Ausgabe der verzeichnisse:--------------------------- Ende")
    End Sub
    Public Sub getAlleVorgaengeFuerEinzelVorgang(ByVal Mitverwandten As Boolean, ByRef ebenen%(), ByVal vid%)
        l("in getAlleVorgaengeFuerEinzelVorgang")
        If Mitverwandten Then
            ebenen(0) = vid%     'die erstgenannte ist die hauptebene
            holeAlleverwandtenebenenfallsangekreuzt(vid%)
            fremdvorgangsidInEbenenFuellen(ebenen)
        Else
            ReDim ebenen(0)
            ebenen(0) = vid%
        End If
    End Sub

    Public Sub Verzeichnisse_anlegen(ByVal dings As clsKartenerstellungShape)
        glob2.nachricht("Verzeichnisse_anlegen------------------------------------")
        With dings
            If Not IO.Directory.Exists(.KartenRoot$) Then IO.Directory.CreateDirectory(.KartenRoot)
            If Not IO.Directory.Exists(.KartenVorgangsDir$) Then IO.Directory.CreateDirectory(.KartenVorgangsDir)
            If Not IO.Directory.Exists(.kartenDatadir$) Then IO.Directory.CreateDirectory(.kartenDatadir)
            If Not IO.Directory.Exists(.KartenJPGDir$) Then IO.Directory.CreateDirectory(.KartenJPGDir)
            If Not IO.Directory.Exists(.KartenIMGDir$) Then IO.Directory.CreateDirectory(.KartenIMGDir)
        End With
        glob2.nachricht("Verzeichnisse_anlegen #################ende ####")
    End Sub
    Public Function getAlleVorgaengeFuerListe(ByVal datei_vorgangsliste As String, ByRef ebenen%(), ByVal vid As Integer, ByRef sql$, ByRef titel$) As Boolean
        Dim testdatei As New IO.FileInfo(datei_vorgangsliste)
        Dim alles$ = ""
        Dim recs$()
        Dim icnt% = 0
        '  Dim e2$()
        Try
            If testdatei.Exists Then
                Using fs As New IO.StreamReader(datei_vorgangsliste)
                    alles = fs.ReadToEnd
                    If String.IsNullOrEmpty(alles) Then
                        l("Die Listendatei ist leer! Abbruch!")
                        Return False
                    Else
                        recs = alles.Split(CChar(vbCrLf))
                        ReDim ebenen(recs.Length - 10)
                        sql$ = recs(0)
                        titel = recs(1)
                        'Array.Copy(recs, 10, e2, 10, recs.Length)
                        For i = 10 To recs.GetUpperBound(0)
                            If Not String.IsNullOrEmpty(recs(i).Trim) Then
                                ebenen(i - 10) = CInt(recs(i))
                                icnt += 1
                            End If
                        Next
                    End If
                    fs.Close()
                End Using
                ReDim Preserve ebenen(icnt - 1)
            Else
                glob2.nachricht("Es wurde keine Listendatei angelegt. Abbruch! ")
                Return False
            End If
            Return True
        Catch ex As Exception
            glob2.nachricht("Fehler in getAlleVorgaengeFuerListe: " & ex.ToString)
        End Try
    End Function

    Function holeAlleverwandtenebenenfallsangekreuzt(ByVal vid%) As Boolean
        glob2.nachricht("holeAlleverwandtenebenenfallsangekreuzt --------------------------------")
        Dim sql$ = "select * from vorgang2fremdvorgang where vorgangsid=" & vid

        If inidatabase.vorgang_MYDB.dbtyp = "oracle" Then

            karte_oracle.erzeugeVerwandtenlistezuVorgang(sql$)
        End If
        If inidatabase.vorgang_MYDB.dbtyp = "mysql" Then
            karte_mysql.erzeugeVerwandtenlistezuVorgang(sql$)
        End If
        myGlobalz.VerwandteDT.Clear()
        myGlobalz.VerwandteDT = myGlobalz.tempREC.dt.Copy
        glob2.nachricht("holeAlleverwandtenebenenfallsangekreuzt ##############ende")
    End Function



    Public Sub fremdvorgangsidInEbenenFuellen(ByRef ebenen%())
                l("in fremdvorgangsidInEbenenFuellen")
        For i = 0 To myGlobalz.VerwandteDT.Rows.Count - 1
            ReDim Preserve ebenen(i + 1)
            ebenen(i + 1) = CInt(myGlobalz.VerwandteDT.Rows(i).Item("fremdvorgangsID"))
        Next
    End Sub
End Class
