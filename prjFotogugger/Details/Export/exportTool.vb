
Module exportTool
    Public laufwerk As String = ""
    Public trennerInFileNames As String = "_"
    Public pfad, root As String

    Public Function DirLeeraeumen(Verzeichnis As String) As Boolean
        l("ExchangeDirLeeraeumen--------------------------------------------")
        Dim fii As IO.FileInfo
        Try
            Dim dateiein As String() = getfilesAusDir(Verzeichnis)
            For Each datei In dateiein
                fii = New IO.FileInfo(datei)
                'If Not fii.Name="_files_.txt" Then
                fii.Delete()
                'End If

            Next
            fii = Nothing
            Return True
        Catch ex As Exception
            l("fehler in ExchangeDirLeeraeumen: " ,ex)
            Return False
        End Try
    End Function

    Public Sub ExportpfadFeststellenUndVerzeichnisseErstellen(ByVal vid As Integer, root As String)

        pfad = IO.Path.Combine(root, myglobalz.sitzung.aktBearbeiter.Initiale)
        pfad = IO.Path.Combine(pfad, vid.ToString & trennerInFileNames &
                               LIBgemeinsames.clsString.kuerzeTextauf(
                                   LIBgemeinsames.clsString.normalize_Filename(myglobalz.sitzung.aktVorgang.Stammdaten.Beschreibung, trennerInFileNames), 40)).Replace(".", "")

        pfad = pfad.Replace("__", "_").Replace("__", "_").Replace("__", "_").Replace("__", "_")
        IO.Directory.CreateDirectory(pfad)
        IO.Directory.CreateDirectory(pfad & "\Verlauf")
        IO.Directory.CreateDirectory(pfad & "\Dokumente")
        IO.Directory.CreateDirectory(pfad & "\Fotos")
        IO.Directory.CreateDirectory(pfad & "\Beteiligte")
        IO.Directory.CreateDirectory(pfad & "\Raumbezug")
    End Sub
    Public Sub entfDoppelteTrenner(ByRef dateiname As String)
        dateiname = dateiname.Replace("__", "_")
        dateiname = dateiname.Replace("__", "_")
        dateiname = dateiname.Replace("__", "_")
        dateiname = dateiname.Replace("  ", " ")
    End Sub
End Module
