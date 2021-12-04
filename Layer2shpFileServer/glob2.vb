Imports System.Data
Imports Layer2shpFileServer.LIBDB

Public Class glob2
    Private Shared paradigmaServer As String
    Private Shared GIS_Server$, GIS_WebServer$
    Private Shared paradigma_user$, paradigma_PW$, webgis_PW$, webgis_user$, paradigmadatentyp$
    Private Shared paradigma_schema$
    Private Shared halo_server$
    Private Shared halo_schema$
    Private Shared probaug_user$, probaug_pw$



    'Shared Sub ini_haloREC()
    '    myGlobalz.haloREC.mydb.Host = halo_server$
    '    myGlobalz.haloREC.mydb.Schema = halo_schema$
    '    myGlobalz.haloREC.mydb.Tabelle = "featureRange"
    '    myGlobalz.haloREC.mydb.username = webgis_user
    '    myGlobalz.haloREC.mydb.password = webgis_PW
    '    myGlobalz.haloREC.mydb.dbtyp = paradigmadatentyp$
    'End Sub

    Shared Sub ini_DBserverNames_LOKALE_INSEL()
        paradigmaServer = "localhost"
        GIS_Server = "w2gis02"
        GIS_WebServer = "w2gis02.kreis-of.local" '"127.0.0.1"
        paradigma_user = "paradigmaumwelt"
        paradigma_PW = "luftikus"
        webgis_user = "webgis"
        webgis_PW = "luftikus"
        paradigmadatentyp$ = "mysql"
        paradigma_schema$ = "paradigma"
        halo_server$ = paradigmaServer
        halo_schema$ = "halosort"
        probaug_user$ = "probaug"
        probaug_pw$ = "morrojable"
        '	myGlobalz.Paradigma_Sachgebietsdatei = "c:\appsconfig\sachgebiet.xml"

        myGlobalz.gis_serverD$ = "d:"
        myGlobalz.GIS_WebServer$ = "localhost" '"KIS"
        myGlobalz.ArcrootDir$ = "O:\UMWELT-PARADIGMA\gis\GIS\div\backup\archiv" '"O:\UMWELT\B\67.01.02 - Grafische Datenverarbeitung\GIS\div\backup\archiv"
    End Sub
    Shared Sub ini_DBserverNames()
        paradigmaServer = "127.0.0.1"
        GIS_Server = "w2gis02"
        GIS_WebServer = "w2gis02.kreis-of.local" '"127.0.0.1"
        paradigma_user = "paradigmaumwelt"
        paradigma_PW = "luftikus"

        paradigma_user = "Paradigma"
        paradigma_PW = "root"
        webgis_user = "webgis"
        webgis_PW = "luftikus"
        paradigmadatentyp$ = "oracle"
        paradigma_schema$ = "paradigma"
        halo_server$ = "w2gis02"
        halo_schema$ = "halosort"
        probaug_user$ = "probaug"
        probaug_pw$ = "morrojable"
        myGlobalz.gis_serverD$ = "\\w2gis02\gdvell"
        myGlobalz.GIS_WebServer$ = "w2gis02.kreis-of.local" '"KIS"
        myGlobalz.ArcrootDir$ = "O:\UMWELT-PARADIGMA\gis\GIS\div\backup\archiv" '"O:\UMWELT\B\67.01.02 - Grafische Datenverarbeitung\GIS\div\backup\archiv"
        '	myGlobalz.Paradigma_Sachgebietsdatei$ = "\\w2gis02\gdvell\paradigma\sachgebiet.xml"
    End Sub


    Public Shared Sub nachricht(ByVal text$)
        My.Application.Log.WriteEntry(text$)
    End Sub

    Public Shared Sub nachricht_an_admin(ByVal text$)
        My.Application.Log.WriteEntry(text$)
    End Sub

    Public Overloads Shared Sub nachricht_und_Mbox(ByVal text$)
        My.Application.Log.WriteEntry(text$)
        '  MessageBox.Show(text)
    End Sub

    Shared Sub starten(ByVal Startstring$)
        glob2.nachricht(Startstring)
        Process.Start(New ProcessStartInfo(Startstring$))
    End Sub




    'Public Shared Function UNION_SQL_erzeugen(ByVal anyDT As DataTable, ByVal tabellenname$, ByVal ausgabespaltenNr%, ByVal idspalte$) As String
    '    Try
    '        Dim summe$ = ""
    '        For i = 0 To anyDT.Rows.Count - 1
    '            summe$ = summe$ & "(SELECT * FROM " & tabellenname$ & _
    '             " where " & idspalte$ & "=" & anyDT.Rows(i).Item(ausgabespaltenNr).ToString & ")"
    '            If i < anyDT.Rows.Count - 1 Then
    '                summe$ = summe$ & " union "
    '            End If
    '        Next
    '        Return summe$
    '    Catch ex As Exception
    '        Return "-1"
    '    End Try
    'End Function
    Public Shared Function UNION_SQL_erzeugen(ByVal anyDT As DataTable,
                                                      ByVal tabellenname$,
                                                      ByVal ausgabespaltenNr%,
                                                      ByVal idspalte$) As String
        Try
            Dim summe$ = "", andobs$ = "", gesamt$
            If anyDT.Rows.Count < 1 Then Return ""
            If anyDT.Rows.Count = 1 Then
                gesamt$ = "select * from " & tabellenname$ & " where " & idspalte$ & "=" & anyDT.Rows(0).Item(ausgabespaltenNr).ToString
                Return gesamt
            End If
            Dim vorspann$ = "select * from " & tabellenname$ & " where " & idspalte$ & " in ("

            For i = 0 To anyDT.Rows.Count - 1
                summe = summe & anyDT.Rows(i).Item(ausgabespaltenNr).ToString & ","
            Next
            summe = summe.Substring(0, summe.Length - 1)
            gesamt$ = vorspann$ & summe & ")"
            Return gesamt$
        Catch ex As Exception
            glob2.nachricht("fehler in UNION_SQL_erzeugenDokument: " & ex.ToString)
            Return "-1"
        End Try
    End Function


    Public Overloads Shared Function DataTable_auschecken(ByVal zielVerzeichnis$, ByVal aktvid%) As Boolean                 'myGlobalz.Arc.lokalerCheckoutcache
        glob2.nachricht("DataTable_auschecken -----------------------------------------------")
        Dim quelle$ = "", ziel$ = ""
        Dim dokid$
        Try
            For Each dok As DataRow In myGlobalz.ArcRec.dt.AsEnumerable
                Try
                    dokid$ = clsDBtools.fieldvalue(dok.Item("dokumentid").ToString)

                    quelle = myGlobalz.ArcrootDir & dok.Item("relativpfad").ToString.Replace("/", "\")
                    quelle = String.Format("{0}\{1}", quelle, dok.Item("dateinameext"))
                    'ziel = zielVerzeichnis$ & "\" & dok.Item("dateinameext").ToString
                    ziel = String.Format("{0}{1}.{2}",
                                         zielVerzeichnis$, dokid$, dok.Item("typ"))
                    If dokid = "3248" Then
                        Debug.Print("")
                    End If
                    Dim FIziel As New IO.FileInfo(ziel)

                    IO.Directory.CreateDirectory(String.Format("{0}\{1}", zielVerzeichnis$, myGlobalz.VorgangsID))

                    If Not FIziel.Exists Then
                        Try
                            IO.File.Copy(quelle, ziel, True)
                        Catch ex As Exception
                            glob2.nachricht_und_Mbox("Problem beim kopieren von:  " & dok.Item("dateinameext").ToString)
                        End Try
                    Else
                        My.Application.Log.WriteEntry("Datei existiert schon!" & ziel)
                    End If

                Catch ex As Exception
                    glob2.nachricht_und_Mbox(String.Format("Problem beim Auschecken von:  {0}{1}{2}", ziel, vbCrLf, ex))
                    Return False
                End Try
            Next
            glob2.nachricht("DataTable_auschecken # ende#######################")
            Return True
        Catch ex As Exception
            glob2.nachricht("Fehler in: DataTable_auschecken: " & ex.ToString)
        End Try
    End Function


    Public Shared Sub mittelpunktsAufruf(ByVal bbox As clsRange, ByVal layer As String, ByVal sgnummer As String, ByVal domain As String)
        Try
            glob2.nachricht("mittelpunktsAufruf")
            glob2.nachricht(String.Format("bbox{0}{1}", vbCrLf, bbox))
            Dim aktp As New myPoint() With {.X = bbox.xl + (bbox.xdif() / 2),
                                            .Y = bbox.yl + (bbox.ydif() / 2)}
            glob2.nachricht("aktp  : " & aktp.toString)
            Dim abstand% = CInt(bbox.xdif() / 2)
            glob2.nachricht("abstand vorher: " & abstand)
            If abstand < 100 Then
                abstand = 1000
            End If
            glob2.nachricht("abstand nachher: " & abstand)
            Dim pw$ = DB_fork.holePasswordFuerGisUser_dballe()
            glob2.nachricht("PW:" & pw)
            Dim http$ = String.Format("http://{0}/cgi-bin/suchdb.cgi?modus=42&rechts={1}&hoch={2}&abstand={3}&username={4}&password={5}&thema={6}{7};",
                                      domain, CInt(aktp.X), CInt(aktp.Y), abstand, myGlobalz.Bearbeiter, pw, setDefaultThemen(sgnummer), layer)   '& _
            '"&guid=" & myguid.ToString
            glob2.nachricht(String.Format("mittelpunktsAufruf {0}{1}", vbCrLf, http))
            glob2.starten(http$)
        Catch ex As Exception
            glob2.nachricht("fehler in mittelpunktsAufruf: " & ex.ToString)
        End Try
    End Sub

    Shared Function setDefaultThemen(ByVal sgnummer$) As String
        glob2.nachricht(String.Format("{0}, in setDefaultThemen {1}", sgnummer$, vbCrLf))
        Dim themen$ = ""
        Dim a$ = sgnummer   'myGlobalz.Vorgang.Stammdaten.az.sachgebiet.Zahl.Substring(0, 1)
        Try
            Select Case a$
                Case "1", "2", "3", "8"
                    themen = "tk5;altpro09;"
                Case "0", "4", "5", "6", "7"
                    themen = "realshapeopak;natlandgeb;kompensation;gemarkung_;"
                Case Else
                    glob2.nachricht("für diese sgnummer gibts keine themenzuweisung")
            End Select
            If myGlobalz.Bearbeiter.ToLower.Substring(0, 3) = "a32" Then
                themen = "wsgeb;oberfl;messstelle;ueberschw;"
            End If
            glob2.nachricht(String.Format("{0}, aus setDefaultThemen {1}", themen, vbCrLf))
            'If Not themen.Contains("raumbez") Then
            '    themen = themen & "raumbez;"
            'End If
            Return themen$
        Catch ex As Exception
            glob2.nachricht("fehler in setDefaultThemen: " & ex.ToString)
        End Try
    End Function




    Public Shared Function initDokumente4VorgangDatatable(ByVal vid%) As Boolean
        glob2.nachricht("initDokumente4VorgangDatatable -----------------------------------------------")
        'zuerst die personenIDs holen	  
        Dim erfolg As Boolean = False
        '   fortviakopplung_dok_vorgang(erfolg)
        'DB_fork.forkiniraumbezugsDT(erfolg)
        erfolg = DB_fork.forkviakopplung_dok_vorgang(vid)

        If erfolg Then
            myGlobalz.DokumentIDsDT = myGlobalz.tempREC.dt.Copy
            Dim SQL$ = ""
            ' SQL = UNION_SQL_erzeugenDokument(myGlobalz.DokumentIDsDT, "dokumente", 1, "dokumentID", ohneobsdoku)
            SQL = UNION_SQL_erzeugen(myGlobalz.DokumentIDsDT, "dokumente", 1, "dokumentID")
            initdokumentDT_by_SQLstring(SQL$)
            glob2.nachricht("initDokumente4VorgangDatatable sql:" & SQL)
            Return True
        Else
            glob2.nachricht("Es konnten keine Dokumente zu diesem Vorgang gefunden werden!")
            Return False
        End If
    End Function



    'Public Shared Function UNION_SQL_erzeugenDokument(ByVal anyDT As DataTable, ByVal tabellenname$, ByVal ausgabespaltenNr%, ByVal idspalte$, ByVal ohneobsdoku As Boolean) As String
    '    Try
    '        Dim summe$ = "", andobs$ = ""
    '        If ohneobsdoku Then
    '            andobs$ = " and veraltet=false"
    '        Else
    '            andobs$ = ""
    '        End If
    '        For i = 0 To anyDT.Rows.Count - 1
    '            summe$ = summe$ & "(SELECT * FROM " & tabellenname$ & _
    '             " where " & idspalte$ & "=" & anyDT.Rows(i).Item(ausgabespaltenNr).ToString & _
    '             andobs$ & _
    '             ")"
    '            If i < anyDT.Rows.Count - 1 Then
    '                summe$ = summe$ & " union "
    '            End If
    '        Next
    '        Return summe$
    '    Catch ex As Exception
    '        Return "-1"
    '    End Try
    'End Function

    Public Shared Sub initdokumentDT_by_SQLstring(ByVal sql$)
        Try
            myGlobalz.ArcRec.mydb.Host = myGlobalz.ArcRec.mydb.Host
            myGlobalz.ArcRec.mydb.Schema = myGlobalz.ArcRec.mydb.Schema
            myGlobalz.ArcRec.mydb.Tabelle = "dokumente"
            myGlobalz.ArcRec.mydb.SQL = sql$
            glob2.nachricht(" hinweis = " & myGlobalz.ArcRec.getDataDT())
            If myGlobalz.ArcRec.mycount < 1 Then
                glob2.nachricht("Keine RaumbezugsRec gespeichert!")
            Else
                glob2.nachricht(String.Format("{0} Dokumente vorhanden", myGlobalz.ArcRec.mycount))
            End If
        Catch ex As Exception
            glob2.nachricht(" fehler in initdokumentDT_by_SQLstring: " & ex.ToString)
        End Try
    End Sub
End Class