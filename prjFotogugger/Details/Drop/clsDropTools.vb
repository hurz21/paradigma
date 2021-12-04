Imports System
Imports System.IO.Compression
Public Class clsDropTools
    Friend Shared Function getExtractDirectory() As String
        Dim extractPath As String = ""
#Disable Warning BC42024 ' Unused local variable: 'filenames'.
        Dim filenames As String()
#Enable Warning BC42024 ' Unused local variable: 'filenames'.
        nachricht("  in getFilenamesFromZipFile: ")
        Try
            extractPath = IO.Path.GetTempPath()
            extractPath = IO.Path.Combine(extractPath, "_ZIP_Conject_" & LIBgemeinsames.clsString.date2string(Now, 1))
            IO.Directory.CreateDirectory(extractPath)

            'filenames = getfilesAusDir(extractPath)
            Return extractPath
        Catch ex As Exception
            nachricht("Fehler in getFilenamesFromZipFile: " ,ex)
            Return Nothing
        End Try
    End Function
    Friend Shared Function getFilenamesFromZipFile(extractPath As String) As String()
        Dim filenames As String()
        nachricht("  in getFilenamesFromZipFile: ")
        Try
            filenames = getfilesAusDir(extractPath)
            Return filenames
        Catch ex As Exception
            nachricht("Fehler in getFilenamesFromZipFile: " ,ex)
            Return Nothing
        End Try
    End Function

    Friend Shared Function hasZipfiles(filenames() As String) As Boolean
        nachricht("hasZipfiles: ")
        Try
            For i = 0 To filenames.Count - 1
                If filenames(i).ToLower.Trim.EndsWith("zip") Then
                    Return True
                End If
            Next
            Return False
        Catch ex As Exception
            nachricht("Fehler in hasZipfiles: " ,ex)
            Return False
        End Try
    End Function
    Friend Shared Function hasConjectfiles(filenames() As String) As Boolean
        nachricht("hasConjectfiles: ")
        Try
            For i = 0 To filenames.Count - 1
                If filenames(i).ToLower.Trim.Contains("conject") Then
                    Return True
                End If
            Next
            Return False
        Catch ex As Exception
            nachricht("Fehler in hasConjectfiles: " ,ex)
            Return False
        End Try
    End Function

    Friend Shared Function getAllFilenames(filenames() As String, ByRef listeZippedFiles As List(Of String),
                                         listeNOnZipFiles As List(Of String), ByRef allFeiles As List(Of String)) As String()
        Dim newFilesNames As String()
        Dim extractDir As String ' die dateinen sind meist gleich
        Dim tempFiles As New List(Of String)
        Try
            For i = 0 To filenames.Count - 1
                extractDir = clsDropTools.getExtractDirectory()
                If filenames(i).ToLower.Trim.EndsWith("zip") Then
                    extrahiereZipNach(filenames(i), extractDir)
                    tempFiles = clsDropTools.getFilenamesFromZipFile(extractDir).ToList
                    For Each item In tempFiles
                        listeZippedFiles.Add(item)
                    Next
                Else
                    listeNOnZipFiles.Add(filenames(i))
                End If
            Next
            '    Array.Clear(filenames, 0, filenames.Length)
            ' allFeiles = CType(listeZippedFiles.Concat(listeNOnZipFiles), List(Of String))
            For Each item In listeZippedFiles
                allFeiles.Add(item)
            Next
            For Each item In listeNOnZipFiles
                allFeiles.Add(item)
            Next

            ReDim newFilesNames(allFeiles.Count - 1)
            For i = 0 To allFeiles.Count - 1
                newFilesNames(i) = allFeiles.Item(i)
            Next
            Return newFilesNames
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Shared Sub extrahiereZipNach(filename As String, extractDir As String)
        Try
            ZipFile.ExtractToDirectory(filename, extractDir)
        Catch ex As Exception
            nachricht("Fehler beim Auspacken von  " & filename, ex)
            MsgBox("Fehler beim Auspacken von: " & filename)
        End Try

    End Sub
End Class
