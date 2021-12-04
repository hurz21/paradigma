
Class EreignisRTFhelp
private sub new

End Sub
    Private Shared Function GetOriginalFullname() As String
        Dim OriginalFullname As String = IO.Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments) & "\" &
                                                                          myGlobalz.sitzung.aktEreignis.ID & ".rtf")
        Return OriginalFullname
    End Function

    Shared Sub RTFdateispeichern(rtftext As String, zielvorgangsid As Integer,byval ArchivSubdir As string)
        'beachte: beim Kopieren von Ereign. wird dies auch benutzt
        '      daher erst den korrekten speicherort feststellen
        If String.IsNullOrEmpty(rtftext) Then
            Exit Sub
        End If
        Dim NumDir As String = ""
        Dim OriginalFullname As String = GetOriginalFullname()
        Dim archivDateiFullname As String = ""
        Dim erfolgreich As Boolean = True
        Dim dokk As New DokArc()

        If EreignisRTFhelp.lokaleDateiErzeugenOk(OriginalFullname, rtftext) Then
            myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir = myGlobalz.sitzung.defineArchivVorgangsDir(zielvorgangsid)
            NumDir = "\hidden_events"
            Dim erfolg As Boolean = myGlobalz.sitzung.aktVorgang.Stammdaten.createArchivsubdir(myGlobalz.Arc.rootDir.ToString,
                                                                                            ArchivSubdir)
            AlteVersionLoeschen()
            Dim dokumentPfad As String = dokk.checkIN_FileArchiv(OriginalFullname, archivDateiFullname, erfolgreich,
                                              NumDir,ArchivSubdir )
            dokk.Dispose
        Else
            nachricht("Fehler in RTFdateispeichern:   konnte nicht erstellt werden: " & OriginalFullname)
        End If
    End Sub

    Shared Function getFokumenttextPfadVonRtfTextfromEreignis() As String
       
        Return GetDokumentPfadImArchiv()

    End Function

    Private Shared Function GetDokumentPfadImArchiv() As String
        'Dim dokumentPfad As String = myGlobalz.Arc.rootDir.ToString & "\" &
        '                                     myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir &
        '                                     "\hidden_events\" &
        '                                     myGlobalz.sitzung.aktEreignis.ID &
        '                                     ".rtf"
        Dim CLEANArcDIR As String
        CLEANArcDIR = myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir.Trim(CChar("\"))
        CLEANArcDIR = CLEANArcDIR.Trim(CChar("/"))
        Dim dokumentPfad As String = myGlobalz.Arc.rootDir.ToString & "\" & IO.Path.Combine(
                                            CLEANArcDIR,
                                            "hidden_events",
                                            myGlobalz.sitzung.aktEreignis.ID &
                                            ".rtf")

        Return dokumentPfad
    End Function
    Private Shared Function lokaleDateiErzeugenOk(OriginalFullname As String, rtftext As String) As Boolean
        Try
            Using schreib As New IO.StreamWriter(OriginalFullname)
                schreib.Write(rtftext)
            End Using
            Return True
        Catch ex As Exception
            nachricht("Fehler in lokaleDateiErzeugenOk:   :" & ex.ToString)
            Return False
        End Try
    End Function


    Public Shared Function NotizRtfInhaltLadenExtracted(ByVal dokumentpfad As String) As String
        If String.IsNullOrEmpty(dokumentpfad) Then
            nachricht("Fehler in NotizRtfInhaltLadenExtracted:  dokumentpfad ungültig oder leer:" & dokumentpfad)
            Return ""
        End If
        Dim rtftext As String = ""
        Try
            Using sr As New IO.StreamReader(dokumentpfad)
                rtftext = sr.ReadToEnd
            End Using
            Return rtftext
        Catch ex As Exception
            nachricht("Fehler in NotizRtfInhaltLadenExtracted:   :" & ex.ToString)
            Return ""
        End Try
    End Function

    Private Shared Sub AlteVersionLoeschen()
        Dim dokumentpfad As String
        dokumentpfad = GetDokumentPfadImArchiv()
        Dim testfile As New IO.FileInfo(dokumentpfad)
        Try
            If testfile.Exists Then
                testfile.Delete()
            End If
        Catch ex As Exception
            nachricht("Fehler in ereignisrtf  AlteVersionLoeschen:   :" & ex.ToString)
            nachricht("Fehler                        dokumentpfad:   :" & dokumentpfad)
        End Try


    End Sub




End Class
