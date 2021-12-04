Module dokScanPDF
    Public Function GetFileSize(ByVal path As String) As String
        Dim myFile As IO.FileInfo
        Dim mySize As Single
        Try
            myFile = New IO.FileInfo(path)

            If Not myFile.Exists Then
                mySize = 0
            Else
                mySize = myFile.Length
            End If
            Select Case mySize
                Case 0 To 1023
                    Return mySize & " Bytes"
                Case 1024 To 1048575
                    Return Format(mySize / 1024, "###0.00") & " KB"
                Case 1048576 To 1043741824
                    Return Format(mySize / 1024 ^ 2, "###0.00") & " MB"
                Case Is > 1043741824
                    Return Format(mySize / 1024 ^ 3, "###0.00") & " GB"
            End Select
            myFile = Nothing
            Return "0 bytes"
        Catch ex As Exception
            Return "0 bytes"
        End Try
    End Function
    Public Function GetFileSizeInMB(ByVal path As String) As Double
        'Return 0
        Dim myFile As IO.FileInfo
        Dim mySize As Single
        Try
            myFile = New IO.FileInfo(path)
            If Not myFile.Exists Then
                mySize = 0
                Return 0
            Else
                mySize = myFile.Length
                Return CDbl(Format(mySize / 1024 ^ 2, "###0.000")) ' & " MB"
            End If
            myFile = Nothing
            Return 0
        Catch ex As Exception
            Return 0
        End Try
    End Function
    Friend Function getFileSize4Length(mySize As Double) As String
        Dim result As String = ""
        Try
            l(" MOD getFileSize4Length anfang")
            Select Case mySize
                Case 0 To 1023
                    Return mySize & " Bytes"
                Case 1024 To 1048575
                    Return Format(mySize / 1024, "###0.00") & " KB"
                Case 1048576 To 1043741824
                    Return Format(mySize / 1024 ^ 2, "###0.00") & " MB"
                Case Is > 1043741824
                    Return Format(mySize / 1024 ^ 3, "###0.00") & " GB"
            End Select
            Return "0 bytes"
            l(" MOD getFileSize4Length ende")

        Catch ex As Exception
            l("Fehler in getFileSize4Length: ", ex)
            Return result
        End Try
    End Function

    Public Function PDF_TIFFdateinameErzeugenUndUEbergeben() As String
        Dim uhrzeit As String
        Dim infodatei As String
        Dim folder As String
        Dim PDF_TIFFdateiname As String
        Try
            uhrzeit = Format(Now, "yyMMddHHmmss")

            PDF_TIFFdateiname = IO.Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) & "\paradigma\scans\SCAN_" & uhrzeit & ".pdf")
            infodatei = IO.Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) & "\paradigma\scans\info.txt")

            folder = IO.Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop) & "\paradigma\scans")
            If Not IO.Directory.Exists(folder) Then IO.Directory.CreateDirectory(folder)

            Using sw As New IO.StreamWriter(infodatei)
                sw.WriteLine(PDF_TIFFdateiname)
            End Using
            Return PDF_TIFFdateiname
        Catch ex As Exception
            nachricht("Fehler in : PDF_TIFFdateinameErzeugenUndUEbergeben: ", ex)
            Return ""
        End Try
    End Function

    Friend Function GetFileLength(fullnameImArchiv As String) As Double
        Dim mySize As Double = 0
        Dim myFile As IO.FileInfo
        Try
            l(" MOD GetFileLength anfang " & fullnameImArchiv)
            If fullnameImArchiv.IsNothingOrEmpty Then
                Return 0
            End If
            myFile = New IO.FileInfo(fullnameImArchiv.Trim)
            If Not myFile.Exists Then
                mySize = 0
            Else
                mySize = myFile.Length
            End If
            l(" MOD GetFileLength ende")
            Return mySize
        Catch ex As Exception
            l("Fehler in GetFileLength: " & fullnameImArchiv.Trim & ", ", ex)
            Return mySize
        End Try
    End Function

    Sub warteschleifeScanner(ByVal scanexe As String)
        Dim myProcess As Process = Nothing
        myProcess = Process.Start(scanexe)
        Do
            If Not myProcess.HasExited Then
                If myProcess.Responding Then
                    Console.WriteLine("Status = Running")
                Else
                    Console.WriteLine("Status = Not Responding")
                End If
            End If
        Loop While Not myProcess.WaitForExit(1000)
        myProcess.Dispose()
        myProcess = Nothing
    End Sub

    Public Sub vorschauPDFScan(PDF_TIFFdateiname As String, groesse As String)
        Dim messi As MessageBoxResult
        Try
            messi = MessageBox.Show("Vorschau des Scans ? (" & groesse & ")",
                                    "Vorschau", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No)
            If messi <> MessageBoxResult.No Then
                glob2.OpenDocument(PDF_TIFFdateiname)
            End If
        Catch ex As Exception
            nachricht("Fehler in : vorschauPDFScan: " & PDF_TIFFdateiname, ex)
        End Try
    End Sub

    Public Function dateiFromScanInsArchiv(ByVal PDF_TIFFdateiname As String, Optional eid As Integer = 0) As Boolean
        Dim fio As New IO.FileInfo(PDF_TIFFdateiname)
        If fio.Exists Then
            vorschauPDFScan(PDF_TIFFdateiname, dokScanPDF.GetFileSize(PDF_TIFFdateiname))
            Dim dateimetadata As String = getScanDateiMetaData(PDF_TIFFdateiname)
            If insarchivUebernehmen(PDF_TIFFdateiname, dateimetadata, eid) Then
                fio = Nothing
                Return True
            End If
        End If
        fio = Nothing
        Return False
    End Function

    Private Function getScanDateiMetaData(PDF_TIFFdateiname As String) As String
        Dim groesse As String = dokScanPDF.GetFileSize(PDF_TIFFdateiname)
        Dim seiten As String
        Dim metadateiname As String
        Dim fi As IO.FileInfo
        Try
            fi = New IO.FileInfo(fileName:=PDF_TIFFdateiname)
            metadateiname = fi.Name.Replace(".pdf", "")
            metadateiname = System.IO.Path.Combine(fi.DirectoryName, metadateiname + "_Metadata.txt")
            fi = Nothing
            Using sr As New IO.StreamReader(metadateiname)
                seiten = sr.ReadLine()
            End Using
            Return seiten & "/ " & groesse
        Catch ex As Exception
            nachricht("fehler beim auslesen der scan metadaten: ", ex)
            Return groesse
        End Try
    End Function

End Module
