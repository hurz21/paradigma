Imports System.Data

Public Class clsVorlagenTools
    Public Shared Function prepareSelectetedVorlageDokument(ByVal insarchiv As Boolean,
                                ByVal dateiname As String,
                                ByRef outfile As String,
                                ByRef resultstring As String,
                                ByVal replaceTextMarkenDict As Dictionary(Of String, String),
                                ByVal ereignisanlegen As Boolean,
                                ByVal Schlagworte As String,
                                ByVal quellpfad As String,
                                ByVal ereignisart As String,
                                ByVal ereignistitel As String,
                                ByRef tauschergebnis As String,
                                dateityp As String,
                                ereignisid As Integer,
                                dateivorname As String,
                                dokumentBookmarks As List(Of String)) As Boolean

        'Dim quelle As String = quellpfad & "\" & outfile & "\" & dateiname
        'quelle = quelle.Replace("\\", "\")
        Dim quelle As String = outfile
        nachricht("ereignisart: " & ereignisart)
        ' Dim quelle$ = String.Format("{0}\{1}\{2}", myGlobalz.VorlagenRoot, myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl, item)
        Dim vorl As clsVorlagedokumente
        Try
            vorl = New clsVorlagedokumente(quelle)
            vorl.aktbeteiligter = myGlobalz.sitzung.aktPerson
            Dim erfolg As Boolean = vorl.VorlageBestimmenUndBearbeiten(insarchiv, resultstring, outfile, replaceTextMarkenDict,
                                                                        Schlagworte,
                                                                       quellpfad, tauschergebnis, dateityp, dateivorname,
                                                                       dokumentBookmarks)
            If erfolg Then
                nachricht(String.Format("clsVorlagenTools:  erfolgreich{0} {1}", outfile, resultstring))
                'hier könnte das Datum des Dokumentes auf NOW gesetzt werden
            Else
                nachricht(String.Format("clsVorlagenTools: nicht erfolgreich{0} {1}", outfile, resultstring))
            End If
            Return erfolg
        Catch ex As Exception
            nachricht("Fehler in prepareSelectetedVorlageDokument: ", ex)
            Return False
        End Try
    End Function

    Shared Function holePersonenliste(ByVal vid As Integer) As List(Of Person) 'myGlobalz.sitzung.VorgangsID
        Dim loklist As New List(Of Person)
        Try
            clsBeteiligteBUSI.refreshBeteiligteListe_dt_erzeugenundMergen(vid)
            loklist = clsBeteiligteBUSI.ConvertDatatable2Personenliste(myGlobalz.sitzung.beteiligteREC.dt)
            'For Each row As DataRow In myGlobalz.sitzung.beteiligteREC.dt.Rows
            '    tmpPers = New Person
            '    'clsBeteiligteBUSI.BeteiligtenRec2Obj(row, tmpPers)

            '    loklist.Add(tmpPers)
            'Next
            Return loklist
        Catch ex As Exception
            nachricht("holePersonenliste: " ,ex)
            Return loklist
        End Try
    End Function

    Shared Sub getliegenschaften(ByRef liegenschaftenDict As Dictionary(Of String, String), ByRef typdict As Dictionary(Of String, Integer))
        nachricht("getliegenschaften: ")
        Try
            Dim erfolg As Boolean = RBtoolsns.initraumbezugsDT_alleDB.exe(myGlobalz.sitzung.aktVorgangsID)
            Dim wert$ = "", funktion$ = "", abstract$ = "", typ% = 0, rid% = 0
            If erfolg Then
                'RBtoolsns.statusSpalteErgaenzenUndFuellen.execute(myGlobalz.sitzung.raumbezugsRec.dt, myGlobalz.sitzung.RaumbezugsIDsDT,
                '                                                  "Status", "RaumbezugsID")
                For i = 0 To myGlobalz.sitzung.raumbezugsRec.dt.Rows.Count - 1
                    typ = CInt(clsDBtools.fieldvalue(myGlobalz.sitzung.raumbezugsRec.dt.Rows(i).Item("typ")))
                   ' funktion = CStr(clsDBtools.fieldvalue(myGlobalz.sitzung.raumbezugsRec.dt.Rows(i).Item("TITEL"))).Trim auf wunnsch von gunilla abgeschaltet
                    abstract = CStr(clsDBtools.fieldvalue(myGlobalz.sitzung.raumbezugsRec.dt.Rows(i).Item("abstract"))).Trim
                    rid = CInt(clsDBtools.fieldvalue(myGlobalz.sitzung.raumbezugsRec.dt.Rows(i).Item("raumbezugsid")))
                    If String.IsNullOrEmpty(funktion) Then
                        wert = CStr(abstract)
                    Else
                        wert = CStr(String.Format("{0}: {1}", funktion, abstract))
                    End If
                    liegenschaftenDict.Add(CStr(i), wert)
                    typdict.Add(rid.ToString, typ)
                Next
                'clsDBtools.fieldvalue(myGlobalz.sitzung.raumbezugsRec.dt.Rows(i).Item("RaumbezugsID"))
                nachricht("getliegenschaften: erfolgreich")
            Else
                nachricht("Keine Raumbezüge")
            End If
        Catch ex As Exception
            nachricht_und_Mbox(ex.ToString)
        End Try
    End Sub

    Shared Sub zeigeTextmarkenListe()
        glob2.OpenDocument(myGlobalz.textmarkenUeberSichtsDatei)
    End Sub

    Shared Function getGemarkungstextFromGEMKRZ(ByVal GEMKRZ As String) As String
        nachricht("getGemarkungstextFromGEMKRZ----------------------------------------------------anfang")
        nachricht("gesucht wird: " & GEMKRZ)
        Dim result As String = ""
        Try
            result = clsGEMKRZXML.LoadVariablenGemarkung(GEMKRZ)
            nachricht("result: " & result)
            nachricht("getGemarkungstextFromGEMKRZ----------------------------------------------------ende")
            Return result
        Catch ex As Exception
            nachricht("Fehler in getGemarkungstextFromGEMKRZ: " ,ex)
            Return ""
        End Try
    End Function

    Shared Function getGemeindetextFromGEMKRZ(ByVal GEMKRZ As String) As String
        nachricht("getGemeindetextFromGEMKRZ----------------------------------------------------anfang")
        nachricht("gesucht wird: " & GEMKRZ)
        Dim result As String = ""
        Try
            result = clsGEMKRZXML.LoadVariablenGemeinde(GEMKRZ)
            nachricht("result: " & result)
            nachricht("getGemeindetextFromGEMKRZ----------------------------------------------------ende")
            Return result
        Catch ex As Exception
            nachricht("Fehler in getGemeindetextFromGEMKRZ: " ,ex)
            Return ""
        End Try
    End Function
    Public Shared Function XLSGetEinCheckErgebnis(dateidatum As Date) As String
        Dim ergebnis As String
        If myGlobalz.Arc.checkINDoku(myGlobalz.sitzung.aktDokument.FullnameCheckout,
                                     myGlobalz.sitzung.aktDokument.Beschreibung,
                                     myGlobalz.sitzung.aktEreignis.ID,
                                     False,
                                     "",
                                     myGlobalz.sitzung.defineArchivVorgangsDir(myGlobalz.sitzung.aktVorgangsID),
                                     myGlobalz.sitzung.aktVorgangsID, False,
                                     dateidatum, myGlobalz.sitzung.aktDokument.DocID,
                                     myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir,
                                     myGlobalz.sitzung.aktDokument.newSaveMode, False,
                                     myGlobalz.sitzung.aktDokument.kompressed, myGlobalz.sitzung.aktBearbeiter.ID) Then
            'datei im Archiv öffnen
            myGlobalz.sitzung.aktDokument.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)

            '##############
            'vdatei$ = myGlobalz.sitzung.aktDokument.FullnameImArchiv
            'outdatei$ = myGlobalz.sitzung.aktDokument.FullnameImArchiv
            'Return True
            ergebnis = "Die Datei wurde dem Archiv hinzugefügt. Sie können Sie unter dem Reiter Dokumente finden!"
            ' MsgBox(ergebnis)
            'glob2.OpenDocument(myGlobalz.sitzung.aktDokument.FullnameCheckout)
        Else
            'nachricht_und_Mbox("Fehler beim Speichern im Archiv!")
            'Return False
            ergebnis = ""
        End If
        Return ergebnis
    End Function

    Public Shared Sub berechneVorlagenverzeichnis(ByRef di As IO.DirectoryInfo, ByVal modus As String, ByRef pfad As String)

        If modus.ToLower = "allgemein" Then
            pfad = myGlobalz.VorlagenRoot & "\allgemein"
            di = New IO.DirectoryInfo(pfad)
        End If
        If modus.ToLower = "aufnahme" Then
            pfad = myGlobalz.VorlagenRoot & "\aufnahme"
            di = New IO.DirectoryInfo(pfad)
        End If
        If modus.ToLower = "ausschreibung" Then
            pfad = myGlobalz.VorlagenRoot & "\ausschreibung"
            di = New IO.DirectoryInfo(pfad)
        End If
        If modus.ToLower = "" Then
            pfad = myGlobalz.VorlagenRoot & ""
            di = New IO.DirectoryInfo(pfad)
        End If
    End Sub
    Public Shared Sub berechneVorlagenverzeichnisSachgebiet(ByRef di As IO.DirectoryInfo, ByVal modus As String, ByRef pfad As String, ByVal sgnr As String)
        If modus.ToLower = "fuersachgebiet" Then
            pfad = myGlobalz.VorlagenRoot & "\" & sgnr
            di = New IO.DirectoryInfo(pfad)
        End If
    End Sub
    Private Shared Sub berechneVorlagenverzeichnisPermanent(ByRef di As IO.DirectoryInfo, ByVal modus As String, ByRef pfad As String, ByVal sgnr As String)
        If modus.ToLower = "_##permanent##_" Then
            pfad = myGlobalz.VorlagenRoot & "" & sgnr & "\permanent"
            di = New IO.DirectoryInfo(pfad)
        End If
    End Sub
    'Public Shared Sub berechneVorlagenverzeichnis(ByRef di As IO.DirectoryInfo, ByVal modus As String, ByRef pfad As String, sgnr As String)
    '    berechneVorlagenverzeichnisAllgemein(di, modus, pfad)
    '    berechneVorlagenverzeichnisSachgebiet(di, modus, pfad, sgnr)
    '    berechneVorlagenverzeichnisPermanent(di, modus, pfad, sgnr)
    'End Sub

    Public Shared Function presentVorlagenListe(ByRef ListeAllerVorlagenImVerz As List(Of IO.FileInfo),
                                            ByRef vorlagenVerzeichnis As IO.DirectoryInfo,
                                            ByVal modus As String,
                                            ByRef pfad As String,
                                            sgnr As String) As Integer
        '   clsVorlagenTools.berechneVorlagenverzeichnis(vorlagenVerzeichnis, modus, pfad, sgnr)

        clsVorlagenTools.berechneVorlagenverzeichnis(vorlagenVerzeichnis, modus, pfad)
        berechneVorlagenverzeichnisSachgebiet(vorlagenVerzeichnis, modus, pfad, sgnr)
        berechneVorlagenverzeichnisPermanent(vorlagenVerzeichnis, modus, pfad, sgnr)

        If Not vorlagenVerzeichnis.Exists Then
            nachricht(String.Format("Es wurden noch keine Vorlagen für die SachgebietsNr: {0} bereitgestellt!",
                                      sgnr))
            IO.Directory.CreateDirectory(vorlagenVerzeichnis.FullName)
            Return 0
        End If

        ' aryFi = di.GetFiles("*.doc")
        'Dim FilterList As New List(Of String)
        'FilterList.Add("*.doc")
        'FilterList.Add("*.docx")
        '  FilterList.Add("*.xls")
        Dim filterstring As String = "*.docx,*.xlsx,*.pdf,*.odt" 'warum nicht xls?
        If modus = "_##permanent##_" Then
            filterstring = "*.docx,*.doc,*.xlsx,*.pdf,*.mdb,*.accdb" '"*" & modus & "*.doc," & "*" & modus & "*.xlsx," & "*" & modus & "*.pdf"
        End If
        ListeAllerVorlagenImVerz.Clear()
        ListeAllerVorlagenImVerz = GetFilesByExtension(vorlagenVerzeichnis.FullName, filterstring)

        nachricht("Es wurden " & ListeAllerVorlagenImVerz.Count & " Vorlagen gefunden.")
        Return ListeAllerVorlagenImVerz.Count
    End Function

    Public Shared Function GetFilesByExtension(ByVal Path As String, ByVal filterString As String) As List(Of IO.FileInfo)
        '  Dim filterString As String 'z.b. "*.jpg,*.bmp,*.tif"
        ' filterString = "*.doc,*.xlsx,*.docx"
        If String.IsNullOrEmpty(filterString) Then
            nachricht(String.Format("Fehler in GetFilesByExtension! filterstring ist leer"))
            Return Nothing
        End If
        Dim FilterList As New List(Of String)
        Dim a() As String
        Try
            a = filterString.Split(CChar(","))
            For i = 0 To a.GetUpperBound(0)
                FilterList.Add(a(i))
            Next
            'FilterList.Add("*.jpg")
            'FilterList.Add("*.bmp")
            'FilterList.Add("*.png")
            'FilterList.Add("*.gif")
            'FilterList.Add("*.tif")
            'Dim files As List(Of IO.FileInfo) = GetFiles(Path, FilterList)
            Dim d As New IO.DirectoryInfo(Path)
            Dim files As List(Of IO.FileInfo) = New List(Of IO.FileInfo)
            'Iterate through the FilterList
            For Each Filter As String In FilterList
                'the files are appended to the file array
                files.AddRange(d.GetFiles(Filter))
            Next
            Return files
        Catch ex As Exception
            nachricht("Fehler in GetFilesByExtension!", ex)
            Return Nothing
        End Try
    End Function
End Class