Module window1Tools
    Public Function holeModus(ByVal datei_einzelvorgang As String, ByVal datei_vorgangsliste As String) As String
        Dim einzeln As New IO.FileInfo(datei_einzelvorgang)
        Dim liste As New IO.FileInfo(datei_vorgangsliste)
        '  Dim toleranz As New TimeSpan(600000000)
        holeModus = "auswahl"
        If einzeln.LastWriteTime > liste.LastWriteTime Then
            holeModus = "einzeln"
        Else
            holeModus = "liste"
        End If
        Return holeModus
    End Function

    Public Sub LoggingInit()
        With My.Log.DefaultFileLogWriter
            .Location = Logging.LogFileLocation.LocalUserApplicationDirectory
            .AutoFlush = False
            .Append = False
            .Delimiter = ";"
        End With
        My.Log.WriteEntry("startlog: " & Now.ToString)
        My.Log.WriteEntry("startlog: " & Environment.GetFolderPath(CType(Logging.LogFileLocation.CommonApplicationDirectory, Environment.SpecialFolder)))
    End Sub

    Public Sub getAlleVorgaengeFuerEinzelVorgang(ByVal Mitverwandten As Boolean, ByRef ebenen%(), ByVal vid%)
        If Mitverwandten Then
            ebenen(0) = vid%     'die erstgenannte ist die hauptebene
            holeAlleverwandtenebenenfallsangekreuzt(vid%)
            fremdvorgangsidInEbenenFuellen(ebenen)
        Else
            ReDim ebenen(0)
            ebenen(0) = vid%
        End If
    End Sub

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
        For i = 0 To myGlobalz.VerwandteDT.Rows.Count - 1
            ReDim Preserve ebenen(i + 1)
            ebenen(i + 1) = CInt(myGlobalz.VerwandteDT.Rows(i).Item("fremdvorgangsID"))
        Next
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
                        MessageBox.Show("Die Listendatei ist leer! Abbruch!")
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
                                icnt% += 1
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

    Sub Datei_einzelvorgang_parameterEinlesen(ByRef datei$, ByRef logfile$, ByRef vid%, ByRef sgnummer$, ByRef verbose As Boolean, ByRef mitgis As Boolean)
        Dim testdatei As New IO.FileInfo(datei)
        If testdatei.Exists Then
            Using fs As New IO.StreamReader(datei)
                Dim vorgangsid$ = ""
                Dim sgnr$ = "", dummy = "", az$ = """"
                vorgangsid$ = fs.ReadLine()
                vid = CInt(vorgangsid)
                Try
                    sgnr$ = fs.ReadLine()
                Catch ex As Exception
                    glob2.nachricht("Fehler: parameterEinlesen 2.zeile fehlt$ ")
                End Try
                Try
                    logfile = fs.ReadLine()
                Catch ex As Exception
                    glob2.nachricht("Fehler: parameterEinlesen 3.zeile fehlt$ ")
                End Try
                Try
                    dummy = fs.ReadLine()
                    dummy = fs.ReadLine()
                    dummy = fs.ReadLine()
                Catch ex As Exception
                    glob2.nachricht("Fehler: parameterEinlesen 3.zeile fehlt$ ")
                End Try
                Try
                    az = fs.ReadLine()
                Catch ex As Exception
                    glob2.nachricht("Fehler: parameterEinlesen 3.zeile fehlt$ ")
                End Try
                Try
                    Dim tempo$ = fs.ReadLine()
                    If Not String.IsNullOrEmpty(tempo) Then
                        If tempo.Trim = "0" Then
                            verbose = False
                        Else
                            verbose = True
                        End If
                    Else
                        verbose = True
                    End If
                Catch ex As Exception
                    glob2.nachricht("Fehler: parameterEinlesen 3.zeile fehlt$ ")
                End Try
                Try
                    Dim tempo$ = fs.ReadLine()
                    If Not String.IsNullOrEmpty(tempo) Then
                        If tempo.Trim = "0" Then
                            mitgis = False
                        Else
                            mitgis = True
                        End If
                    Else
                        mitgis = True
                    End If
                Catch ex As Exception
                    glob2.nachricht("Fehler: parameterEinlesen 3.zeile fehlt$ ")
                End Try
                fs.Close()
                glob2.nachricht("vorgangsid$ " & vorgangsid$)
                glob2.nachricht("sgnummer$$ " & sgnr$)

                sgnummer = sgnr$
            End Using
        Else
            glob2.nachricht("Es wurde keine ParameterDatei angelegt$ ")
        End If
    End Sub
End Module
