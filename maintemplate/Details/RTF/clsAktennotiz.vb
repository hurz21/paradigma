Imports System.Data

Public Class clsAktennotiz
    Private _Ueberschrift$
    Sub gentabelle(ByVal flwMAIN As FlowDocument, ByVal theDT As DataTable, ByVal titel$)
        ' Create the Table...
        Dim table1 = New Table()
        ' ...and add it to the FlowDocument Blocks collection.
        flwMAIN.Blocks.Add(table1)
        ' Set some global formatting properties for the table.
        table1.CellSpacing = 10
        table1.Background = Brushes.White
        ' Create the Table...
        table1 = New Table()
        ' ...and add it to the FlowDocument Blocks collection.
        flwMAIN.Blocks.Add(table1)
        ' Set some global formatting properties for the table.
        table1.CellSpacing = 10
        table1.Background = Brushes.White

        Dim numberOfColumns = theDT.Columns.Count - 1
        Dim x%
        For x = 0 To numberOfColumns
            table1.Columns.Add(New TableColumn())

            ' Set alternating background colors for the middle colums.
            If x Mod 2 = 0 Then
                table1.Columns(x).Background = Brushes.Beige
            Else
                table1.Columns(x).Background = Brushes.LightSteelBlue
            End If
        Next x
        ' Create and add an empty TableRowGroup to hold the table's Rows.
        table1.RowGroups.Add(New TableRowGroup())

        ' Add the first (title) row.
        table1.RowGroups(0).Rows.Add(New TableRow())

        ' Alias the current working row for easy reference.
        Dim currentRow As New TableRow()
        currentRow = table1.RowGroups(0).Rows(0)

        ' Global formatting for the title row.
        currentRow.Background = Brushes.Silver
        currentRow.FontSize = 40
        currentRow.FontWeight = System.Windows.FontWeights.Bold

        ' Add the header row with content, 
        currentRow.Cells.Add(New TableCell(New Paragraph(New Run(titel$))))
        ' and set the row to span all 6 columns.
        currentRow.Cells(0).ColumnSpan = 6
        ' Add the second (header) row.
        table1.RowGroups(0).Rows.Add(New TableRow())
        currentRow = table1.RowGroups(0).Rows(1)

        ' Global formatting for the header row.
        currentRow.FontSize = 12
        currentRow.FontWeight = FontWeights.Bold


        For Each column As DataColumn In theDT.Columns
            currentRow.Cells.Add(New TableCell(New Paragraph(New Run(column.ColumnName))))
        Next
        ' Add the third row.
        table1.RowGroups(0).Rows.Add(New TableRow())
        currentRow = table1.RowGroups(0).Rows(2)
        table1.RowGroups.Add(New TableRowGroup())
        table1.RowGroups(1).Rows.Add(New TableRow())

        For i = 0 To theDT.Rows.Count - 1
            table1.RowGroups(1).Rows.Add(New TableRow())
            currentRow = table1.RowGroups(1).Rows(i)

            ' Global formatting for the row.
            currentRow.FontSize = 12
            currentRow.FontWeight = FontWeights.Normal
            For j = 0 To theDT.Columns.Count - 1
                currentRow.Cells.Add(New TableCell(New Paragraph(New Run(clsDBtools.fieldvalue(theDT.Rows(i).Item(j))))))
            Next
        Next

        'Header		 
        Dim paraHeader As New Paragraph()
        paraHeader.FontSize = 24
        paraHeader.FontWeight = FontWeights.Bold
        paraHeader.Inlines.Add(New Run("------------------"))
        flwMAIN.Blocks.Add(paraHeader)
    End Sub
    Public Shared Sub NurDerHeader(ByVal flw As FlowDocument, ByVal paraHeader As Paragraph, ByVal titel$)
        'Header			Aktennotiz
        paraHeader = New Paragraph()
        paraHeader.FontSize = 24
        paraHeader.FontWeight = FontWeights.Bold
        paraHeader.Inlines.Add(New Run(titel$))
        flw.Blocks.Add(paraHeader)

        'Allg.
        paraHeader = New Paragraph()
        paraHeader.FontSize = 12
        paraHeader.FontWeight = FontWeights.Normal

        paraHeader.Inlines.Add(New Run("Datum: " & Format(myGlobalz.sitzung.aktEreignis.Datum, "dd.MM.yyyy")))
        paraHeader.Inlines.Add(New LineBreak())
        paraHeader.Inlines.Add(New Run("Bearbeiter: " & myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Name & ", " & myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Vorname)) '& " / " & myGlobalz.sitzung.aktEreignis.Quelle & " (" & myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter & ")"))
        paraHeader.Inlines.Add(New LineBreak())
        paraHeader.Inlines.Add(New Run("Vorgang: " & myGlobalz.sitzung.aktVorgangsID))
        paraHeader.Inlines.Add(New LineBreak())
        paraHeader.Inlines.Add(New Run("Aktenzeichen: " & myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt))
        paraHeader.Inlines.Add(New LineBreak())
        paraHeader.Inlines.Add(New Run("Sachgebiet: " & myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header))
        paraHeader.Inlines.Add(New LineBreak())
        paraHeader.Inlines.Add(New Run("Titel: " & myGlobalz.sitzung.aktVorgang.Stammdaten.Beschreibung))
        paraHeader.Inlines.Add(New LineBreak())
        paraHeader.Inlines.Add(New LineBreak())
        flw.Blocks.Add(paraHeader)
    End Sub

    'Public Sub generateHeaderUndBodyEreignis(ByVal flw As FlowDocument, ByVal ueberschrift As String, ByVal Textkoerper As String)
    '    Dim paraHeader As Paragraph = New Paragraph()
    '    paraHeader.Inlines.Add(New Run(Textkoerper))
    '    paraHeader.Inlines.Add(New LineBreak())
    '    flw.Blocks.Add(paraHeader)
    'End Sub

    Public Sub generateHeaderUndBody(ByVal flw As FlowDocument, ByVal ueberschrift As String, ByVal Textkoerper As String)
        Dim paraHeader As Paragraph = New Paragraph()
        NurDerHeader(flw, paraHeader, "Aktennotizchen")
        'Thema.
        paraHeader = New Paragraph()
        paraHeader.FontSize = 20
        paraHeader.FontWeight = FontWeights.Normal
        paraHeader.Inlines.Add(New Run(ueberschrift))
        paraHeader.Inlines.Add(New LineBreak())
        flw.Blocks.Add(paraHeader)
        'Allg.
        paraHeader = New Paragraph()
        paraHeader.FontSize = 14
        paraHeader.FontWeight = FontWeights.Normal
        Dim d$() = Textkoerper.Split(CChar(vbCrLf))

        For i = 0 To d.GetUpperBound(0)
            paraHeader.Inlines.Add(New Run(d(i)))
            paraHeader.Inlines.Add(New LineBreak())
            flw.Blocks.Add(paraHeader)
        Next
    End Sub

    'Public Sub RTFdateispeichernImArchiv(ByVal rtb1 As RichTextBox)
    '    _Ueberschrift$ = "Überschrift hier ergänzen"
    '    Dim filenameImLokalenCache$ = "", ArchivDateiFullname$ = ""
    '    filenameImLokalenCache = GetFilenameFuerLokalenCache()
    '    LokalesCacheVerzeichnisAnlegen()
    '    Dim filename As String = clsBerichte.schreibeInTIFDatei(rtb1, filenameImLokalenCache)
    '    If filename.StartsWith("Fehler") Then
    '        MsgBox("Fehler beim Schreiben in Datei: " & filenameImLokalenCache)
    '    Else
    '        NeuesEreigniserzeugen() 'myGlobalz.sitzung.aktEreignis
    '        clsEreignisTools.NeuesEreignisSpeichern_alleDB(myGlobalz.sitzung.aktVorgangsID, "neu", myGlobalz.sitzung.aktEreignis)
    '          Dim docFile As String = glob2.ConvertRtf2Doc(filenameImLokalenCache)
    '        If Not docFile.IsNothingOrEmpty Then
    '            IO.File.Delete(filenameImLokalenCache)
    '            filenameImLokalenCache = docFile
    '        End If
    '        InsArchivAufnehmenUndOeffnen(filenameImLokalenCache, ArchivDateiFullname, Now)
    '    End If
    'End Sub

    Public Sub RTFdateierzeugenEreignis(ByVal rtb1 As RichTextBox, ByRef filenameImLokalenCache As String, ueberschrift As String)
        _Ueberschrift$ = ueberschrift
        filenameImLokalenCache = GetFilenameFuerLokalenCache()
        LokalesCacheVerzeichnisAnlegen()
        Dim filename$ = clsBerichte.schreibeInTIFDatei(rtb1, filenameImLokalenCache)
    End Sub
    Public Function RTFdateispeichernEreignis(ByVal rtb1 As RichTextBox, ByRef filenameImLokalenCache As String) As Boolean
        _Ueberschrift$ = "Überschrift hier ergänzen"
        filenameImLokalenCache = GetFilenameFuerLokalenCache()
        LokalesCacheVerzeichnisAnlegen()
        Dim filename As String = clsBerichte.schreibeInTIFDatei(rtb1, filenameImLokalenCache)
        If Not filename.StartsWith("Fehler") Then
            'glob2.OpenDocument(filenameImLokalenCache)
            Return True
        Else
            Return False
        End If
    End Function
    Public Sub RTFdateispeichernEreignisRTF(ByVal orginalname As String, ByRef filenameImLokalenCache As String)
        _Ueberschrift$ = "Überschrift hier ergänzen"
        Dim festfi As New IO.FileInfo(orginalname)
        '  filenameImLokalenCache = GetFilenameFuerLokalenCache()
        Dim filename As String = myGlobalz.Arc.lokalerCheckoutcache & "" & myGlobalz.sitzung.aktVorgangsID & "\" & festfi.Name
        LokalesCacheVerzeichnisAnlegen()
        filenameImLokalenCache = filename
        If festfi.Exists Then
            altedateiloeschen(filename)
            festfi.CopyTo(filename)

        End If
    End Sub

    'Private Sub InsArchivAufnehmenUndOeffnen(ByVal filenameImLokalenCache As String, ByVal ArchivDateiFullname As String, dateidatum As Date)
    '    Dim NumDir As String = ""
    '    NumDir = myGlobalz.Arc.getFreshNumDir(myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir)
    '    If Not myGlobalz.Arc.checkINDoku(filenameImLokalenCache, _Ueberschrift,
    '                                     myGlobalz.sitzung.aktEreignis.ID,
    '                                     False, 
    '                                     ArchivDateiFullname,
    '                                     NumDir,
    '                                     myGlobalz.sitzung.aktVorgangsID,
    '                                     False, 
    '                                     dateidatum,
    '                                     myGlobalz.sitzung.aktDokument.DocID,
    '                                     myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir) Then
    '        nachricht_und_Mbox("Fehler beim speichern im Archiv!")
    '    Else
    '        'datei öffnen
    '        nachricht("Wurde im Archiv gespeichert als: " & ArchivDateiFullname)
    '        glob2.OpenDocument(ArchivDateiFullname)
    '    End If
    'End Sub

    'Private Shared Sub NeuesEreignisSpeichern()
    '    If clsEreignisDB.Neu_speichern_Ereignis() Then
    '        nachricht("Ereignis für Aktennotiz wurde gespeichert!")
    '    Else
    '        nachricht_und_Mbox("Ereignis für Aktennotiz wurde nicht gespeichert!")
    '    End If
    'End Sub

    Private Shared Sub LokalesCacheVerzeichnisAnlegen()
        IO.Directory.CreateDirectory(myGlobalz.Arc.lokalerCheckoutcache & "\" & myGlobalz.sitzung.aktVorgangsID)
    End Sub

    Private Shared Function GetFilenameFuerLokalenCache() As string
        Dim filename$
        filename = "Aktennotiz_" & Now.ToString("yyyy-MM-dd_HHmmss") & ".rtf"
        filename = LIBgemeinsames.clsString.normalize_Filename(filename)
        filename = myGlobalz.Arc.lokalerCheckoutcache & "" & myGlobalz.sitzung.aktVorgangsID & "\" & filename
        Return filename
    End Function
    'Private Sub NeuesEreigniserzeugen()
    '    myGlobalz.sitzung.aktEreignis.clearValues()
    '    With myGlobalz.sitzung.aktEreignis
    '        .Datum = Now
    '        .Art = "Aktennotiz"
    '        .Richtung = ""
    '        .Beschreibung = _Ueberschrift
    '        'todo hier kann man mehr machen
    '    End With
    'End Sub

    Private Sub altedateiloeschen(orginalname As String)
        Dim festfi As New IO.FileInfo(orginalname)
        Try
            If festfi.Exists Then
                festfi.Delete()
            End If
        Catch ex As Exception
            nachricht("Fehler in altedateiloeschen" & ex.ToString)
        End Try
    End Sub

End Class
