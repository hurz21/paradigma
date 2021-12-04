Public Class paintTools
    Public Shared Sub DateiFeststellenUndPainten()
        'datei feststellen
        Dim zieldatei As String
        Dim test As New IO.FileInfo(clstart.myc.kartengen.gifKartenDateiFullName)
        Dim bilder$ = Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures)
        zieldatei = bilder & "\vorgang_" & myGlobalz.sitzung.aktVorgangsID & "_" & Now.Year & Now.Month & Now.Minute & Now.Millisecond & ".png"

        If test.Exists Then
            If Not bilddateiKopierenNachMyPictures(test, bilder, zieldatei) Then
                MsgBox("Die Datei sollte nach Dokumente\Bilder kopiert werden. Dabei trat ein Fehler auf! Abbruch. Bitte informieren Sie den Admin!")
                Exit Sub
            End If
        Else
            MsgBox("Die Bilddatei existiert noch nicht. Ggf. gleich nochmal versuchen !")
            Exit Sub
        End If
        'paintaufrufen
        Paintaufrufen(zieldatei)
    End Sub
    Private Shared Sub Paintaufrufen(ByVal zieldatei As String)
        Try
            '%windir%\system32\mspaint.exe
            Using p As New Process()
                p.StartInfo.FileName = "mspaint.exe "
                p.StartInfo.Arguments = zieldatei
                p.StartInfo.UseShellExecute = False
                p.Start()
            End Using
        Catch ex As Exception
            nachricht("FEHLER in DateiFeststellenUndPainten: " & ex.ToString)
        End Try
    End Sub
    Private Shared Function bilddateiKopierenNachMyPictures(ByVal test As IO.FileInfo, ByVal bilder$, ByVal zieldatei as string) as  Boolean
        Try
            Dim testziel As New IO.FileInfo(zieldatei)
            If testziel.Exists Then
                'Dim diares As MessageBoxResult
                'diares = MessageBox.Show("Vorsicht! Die Datei existiert schon Abspeichern ?", "Personen", _
                '     MessageBoxButton.YesNo, MessageBoxImage.Exclamation, MessageBoxResult.OK)
                'If Not diares = MessageBoxResult.No Then
                '    e.Cancel = True
                'End If
            End If
            test.CopyTo(zieldatei, True)
            Return True
        Catch ex As Exception
            nachricht("Fehler beim kopieren der Bilddateidatei: " & ex.ToString)
            nachricht("von : " & clstart.myc.kartengen.gifKartenDateiFullName)
            nachricht("nach: " & zieldatei)
            Return False
        End Try
    End Function
End Class
