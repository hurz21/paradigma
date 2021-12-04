Public Class PDFDruckTool

    'Shared Sub PDFKarteEreignisErzeugen(Beschreibung As String, datei As String)
    '    Dim erfolgreich As Boolean
    '    'ereignis erzeugen
    '    'ereignis speichern mit zuordnung
    '    Try
    '        If Beschreibung.IsNothingOrEmpty Then Beschreibung = "PDFkarte erstellt"
    '        Dim neuereignis As New clsEreignis
    '        initEreignis(neuereignis, Beschreibung)
    '        clsEreignisTools.NeuesEreignisSpeichern_alleDB(myGlobalz.sitzung.aktVorgangsID, "neu", neuereignis)

    '        clstart.myc.aLog.komponente = "Ereignis" : clstart.myc.aLog.aktion = myGlobalz.sitzung.aktEreignis.ID & " " &
    '            myGlobalz.sitzung.aktEreignis.Beschreibung & ": neu angelegt" : clstart.myc.aLog.log()

    '        erfolgreich = myGlobalz.Arc.checkINDoku(datei,
    '                                                neuereignis.ID,
    '                                                Beschreibung,
    '                                                myGlobalz.sitzung.aktVorgangsID,
    '                                                False,
    '                                                myGlobalz.Arc.getFreshNumDir(myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir),
    '                                                Now,myGlobalz.sitzung.aktDokument.DocID,
    '                                                myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir)
    '    Catch ex As Exception
    '        nachricht("fehler in PDFKarteEreignisErzeugen:" & ex.ToString)
    '    End Try
    'End Sub

    'Private Shared Sub initEreignis(ByVal neuereignis As clsEreignis, Beschreibung As String)
    '    With neuereignis
    '        .Art = "info"
    '        .Beschreibung = Beschreibung
    '        .Datum = Now
    '        .istRTF = False
    '        .Notiz = ""
    '        .DokumentID = 0
    '        .Quelle = myGlobalz.sitzung.aktBearbeiter.Initiale
    '        .Richtung = ""
    '    End With
    'End Sub

    'Shared Sub PDFKarteInsArchiv(Beschreibung As String, datei As String)
    '    Dim erfolgreich As Boolean
    '    Try
    '        If Beschreibung.IsNothingOrEmpty Then Beschreibung = "PDFkarte erstellt"
    '        erfolgreich = myGlobalz.Arc.checkINDoku(datei,
    '                                                 0,
    '                                                Beschreibung,
    '                                                myGlobalz.sitzung.aktVorgangsID,
    '                                                False,
    '                                                myGlobalz.Arc.getFreshNumDir(myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir),
    '                                                Now,
    '                                                myGlobalz.sitzung.aktDokument.DocID,
    '                                                myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir)
    '    Catch ex As Exception
    '        nachricht("fehler in PDFKarteInsArchiv:" & ex.ToString)
    '    End Try
    'End Sub

End Class
