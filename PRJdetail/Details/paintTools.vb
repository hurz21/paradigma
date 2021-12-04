Public Class paintTools
    Public Shared Sub DateiFeststellenUndPainten()
        'datei feststellen
        Dim zieldatei As String
        Dim test As New IO.FileInfo(CLstart.myc.kartengen.gifKartenDateiFullName)
        Dim bilder$ = Environment.GetFolderPath(System.Environment.SpecialFolder.MyPictures)
        zieldatei = bilder & "\vorgang_" & myglobalz.sitzung.aktVorgangsID & "_" & Now.Year & Now.Month & Now.Minute & Now.Millisecond & ".png"

        If test.Exists Then
            If Not bilddateiKopierenNachMyPictures(test, zieldatei) Then
                MsgBox("Die Datei sollte nach Dokumente\Bilder kopiert werden. Dabei trat ein Fehler auf! Abbruch. Bitte informieren Sie den Admin!")
                test = Nothing
                Exit Sub
            End If
        Else
            MsgBox("Die Bilddatei existiert noch nicht. Ggf. gleich nochmal versuchen !")
            test = Nothing
            Exit Sub
        End If
        'paintaufrufen
        test = Nothing
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
            nachricht("FEHLER in DateiFeststellenUndPainten: " ,ex)
        End Try
    End Sub
    Private Shared Function bilddateiKopierenNachMyPictures(ByVal test As IO.FileInfo,
                                                            ByVal zieldatei As String) As Boolean
        Try


            test.CopyTo(zieldatei, True)

            Return True
        Catch ex As Exception
            nachricht("Fehler beim kopieren der Bilddateidatei: " ,ex)
            nachricht("von : " & CLstart.myc.kartengen.gifKartenDateiFullName)
            nachricht("nach: " & zieldatei)
            Return False
        End Try
    End Function
End Class
