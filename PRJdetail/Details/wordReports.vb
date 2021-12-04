Module wordReports
    Public Sub makeReportKoloman(modus As String)
        Dim arguments As String = " /vid=" & myGlobalz.sitzung.aktVorgangsID &
                    "#eid=" & myGlobalz.sitzung.aktEreignis.ID & "#" &
                    CLstart.myc.kartengen.gifKartenDateiFullName & "#" &
                    modus & "#bid=" &
                    myGlobalz.sitzung.aktBearbeiter.ID
        Dim Process As Process = New Process()
        Process.StartInfo.FileName = initP.getValue("ExterneAnwendungen.APPLICATION_KoloDossierExe") '
        Process.StartInfo.Arguments = arguments
        Process.StartInfo.WorkingDirectory = "C:\kreisoffenbach\koloreport"
        Process.StartInfo.ErrorDialog = True
        Process.StartInfo.WindowStyle = ProcessWindowStyle.Normal
        Process.Start()
        Process.WaitForExit(1000 * 60 * 5) '    // Wait up To five minutes.
    End Sub

    'Public Sub AktenotizMitEreignisErzeugen(_Ueberschrift As String)
    '    'If _Ueberschrift.Trim.Trim = String.Empty Then
    '    '    _Ueberschrift = "Überschrift hier ergänzen"
    '    'End If
    '    'Dim filenameImLokalenCache$ = "", ArchivDateiFullname$ = ""
    '    'filenameImLokalenCache = GetFilenameFuerLokalenCache()
    '    'LokalesCacheVerzeichnisAnlegen()
    '    'Dim filename As String = clsBerichte.schreibeInRTFDatei(rtb1, filenameImLokalenCache)
    '    'If filename.StartsWith("Fehler") Then
    '    '    MsgBox("Fehler beim Schreiben in Datei: " & filenameImLokalenCache)
    '    'Else
    '    '    NeuesEreigniserzeugen() 'myGlobalz.sitzung.aktEreignis
    '    '    clsEreignisTools.NeuesEreignisSpeichern_alleDB(myglobalz.sitzung.aktVorgangsID, "neu", myglobalz.sitzung.aktEreignis)
    '    '    Dim docFile As String = glob2.ConvertRtf2Doc(filenameImLokalenCache)
    '    '    glob2.MeinGarbage()

    '    '    If Not docFile.IsNothingOrEmpty Then
    '    '        IO.File.Delete(filenameImLokalenCache)
    '    '        filenameImLokalenCache = docFile
    '    '    End If
    '    '    InsArchivAufnehmen(filenameImLokalenCache, ArchivDateiFullname, Now)
    '    '    MessageBox.Show("Die Datei wurde ins Archiv aufgenommen und findet sich nun unter dem Reiter 'Dokumente'!")
    '    'End If
    'End Sub
End Module
