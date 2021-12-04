Module glob2
    Property ssss As winImageShow
    Property list As winNameId
    Property listGEMKRZ As winGemkrz
    Property tree As winTree
    Public Sub zeigeScreenshot(ByVal xmlfile As String)
        If Not ssss Is Nothing Then ssss.Close()
        ssss = New winImageShow(xmlfile)
        ssss.Show()
    End Sub

    Public Sub zeigeListe(ByVal dataPfad As String, ByVal liste As String)
        Dim xmlfile As String = liste
        list = New winNameId(dataPfad, xmlfile)
        list.ShowDialog()
    End Sub

    Public Sub zeigeTree(ByVal dataPfad As String, ByVal liste As String)
        Dim xmlfile As String = liste
        tree = New winTree(dataPfad, xmlfile)
        tree.ShowDialog()
    End Sub

    Function backupAllXML(ByVal datapfad As String) As Boolean
        Dim filelist() As String
        Dim backuppfad As String
        Dim timestamp As String
        timestamp = Year(Now) & Month(Now) & Day(Now) & Hour(Now) & Minute(Now) & Second(Now)
        backuppfad = IO.Path.Combine(datapfad, "backup")
        IO.Directory.CreateDirectory(backuppfad)
        backuppfad = IO.Path.Combine(backuppfad, timestamp)
        IO.Directory.CreateDirectory(backuppfad)
        Dim bfile As String
        Dim fi As IO.FileInfo
        Try
            filelist = IO.Directory.GetFiles(datapfad, "*.xml")
            If filelist.Count < 1 Then
                Return False
            Else
                For Each filechen In filelist
                    fi = New IO.FileInfo(filechen)
                    bfile = IO.Path.Combine(backuppfad, fi.Name)
                    fi.CopyTo(bfile, True)
                Next
            End If
            MessageBox.Show("Die " & filelist.Count & " Dateien wurden gesichert nach: " & vbCrLf &
                            backuppfad)
            Return True
        Catch ex As Exception
            MsgBox("fehler: " & ex.ToString)
            Return False
        End Try
    End Function

    Public Sub zeigeGEMKRZListe(ByVal dataPfad As String, ByVal liste As String)
        Dim xmlfile As String = liste
        listGEMKRZ = New winGemkrz(dataPfad, xmlfile)
        listGEMKRZ.ShowDialog()
    End Sub

End Module
