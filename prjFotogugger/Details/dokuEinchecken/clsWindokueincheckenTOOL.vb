Imports paradigmaDetail

Public Class clsWindokueincheckenTOOL
    Public Sub QuellverzeichnisseEinlesen(ByRef _dasdir As String)
        nachricht("QuellverzeichnisseEinlesen -------------------------------")
        Try
            Dim test2 As String = CLstart.myc.userIniProfile.WertLesen("Dokumente", "windowkuEinchecken_quelldir")
            If String.IsNullOrEmpty(test2) Then
                Exit Sub
            End If
            Dim testdir = New IO.DirectoryInfo(test2)
            If Not testdir.Exists Then Exit Sub
            _dasdir = test2
            testdir = Nothing
            nachricht("QuellverzeichnisseEinlesen emde: " & _dasdir)
        Catch ex As Exception
            nachricht("Fehler in QuellverzeichnisseEinlesen emde: " & _dasdir)
        End Try
    End Sub

    Friend Sub quellverzeichnisspeichern(ByVal dasdir As String)
        Try
            nachricht("quellverzeichnisspeichern -------------------------------------")
            nachricht("quellverzeichnisspeichern -- schreiben")
            If Not String.IsNullOrEmpty(dasdir) Then CLstart.myc.userIniProfile.WertSchreiben("Dokumente", "windowkuEinchecken_quelldir", dasdir)
            nachricht("quellverzeichnisspeichern ende")
        Catch ex As Exception
            nachricht(" Fehler in quellverzeichnisspeichern -- schreiben", ex)
        End Try
    End Sub

    Friend Function verzeichnis_isolieren(ByVal filenamen$()) As String
        If filenamen Is Nothing Then Return ""
        If filenamen.Count > 0 Then
            Dim test As New IO.FileInfo(filenamen(0))
            Dim dirname As String = test.DirectoryName
            test = Nothing
            Return dirname
        Else
            Return ""
        End If
    End Function

    Friend Function nurAusgwaehlteDokus(collLokaleDokumente As List(Of clsPresDokumente)) As String()
        l(" MOD nurAusgwaehlteDokus anfang")
        Dim neu As String()
        Dim icoung As Integer = 0
        Try
            For Each ele As clsPresDokumente In collLokaleDokumente
                If ele.ausgewaehlt Then
                    ReDim Preserve neu(icoung)
                    neu(icoung) = IO.Path.Combine(ele.dokumentPfad, ele.DateinameMitExtension)
                    icoung += 1
                End If
            Next
            l(" MOD nurAusgwaehlteDokus ende")
            Return neu
        Catch ex As Exception
            l("Fehler in nurAusgwaehlteDokus: ", ex)
        End Try
    End Function
    Friend Function fotoListeUmwandeln(neueliste As List(Of String), ByRef loeschliste As List(Of String)) As List(Of String)
        Dim neu As New List(Of String)
        Dim icoung As Integer = 0
        Dim neuername As String = ""
        Dim erfolg As Boolean = False
        Dim tempdir As String
        Dim fi As IO.FileInfo
        Try
            l(" MOD fotosumwandeln anfang")
            If neueliste Is Nothing Then
                l("keine dokumente gefunden neueliste Is Nothing")
                Return neu
            End If
            'ReDim neu(neueliste.Length - 1)
            tempdir = IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, Format(Now, "yyyyMMddhhmmss_ffff"))
            IO.Directory.CreateDirectory(tempdir)
            For Each ele As String In neueliste
                If DokArc.istFoto(ele) Then
                    fi = New IO.FileInfo(ele)
                    neuername = getNeuerNameFotoJPG(fi.Name, tempdir)
                    erfolg = verkleinern(ele, neuername, 2560)
                    If erfolg Then
                        neu.Add(neuername)
                        icoung += 1
                        loeschliste.Add(neuername)
                    End If
                Else
                    neu.Add(ele)
                    icoung += 1
                End If
            Next
            l(" MOD fotosumwandeln ende")
            Return neu
        Catch ex As Exception
            l("Fehler in fotosumwandeln: ", ex)
            Return neu
        End Try
    End Function
    Friend Function fotoarrayUmwandeln(neueliste() As String, ByRef loeschliste As List(Of String)) As String()
        Dim neu As String()
        Dim icoung As Integer = 0
        Dim neuername As String = ""
        Dim erfolg As Boolean = False

        Dim fi As IO.FileInfo
        Try
            l(" MOD fotosumwandeln anfang")
            ReDim neu(neueliste.Length - 1)
            Dim tempdir As String = IO.Path.Combine(My.Computer.FileSystem.SpecialDirectories.Temp, Format(Now, "yyyyMMddhhmmss_ffff"))
            IO.Directory.CreateDirectory(tempdir)
            For Each ele As String In neueliste
                If DokArc.istFoto(ele) Then
                    fi = New IO.FileInfo(ele)
                    neuername = getNeuerNameFotoJPG(fi.Name, tempdir)
                    erfolg = verkleinern(ele, neuername, 2560)
                    If erfolg Then
                        neu(icoung) = neuername
                        icoung += 1
                        loeschliste.Add(neuername)
                    End If
                Else
                    neu(icoung) = ele
                    icoung += 1
                End If
            Next
            l(" MOD fotosumwandeln ende")
            Return neu
        Catch ex As Exception
            l("Fehler in fotosumwandeln: ", ex)
            Return neu
        End Try
    End Function

    Private Function verkleinern(altername As String, neuername As String, pixel As Integer) As Boolean
        Dim erfolg As Boolean = False
        Try
            l(" MOD verkleinern anfang")
            erfolg = conv2jpg(altername, neuername, pixel)
            l(" MOD verkleinern ende")
            Return erfolg
        Catch ex As Exception
            l("Fehler in verkleinern: ", ex)
            Return False
        End Try
    End Function
    Function conv2jpg(inn As String, out As String, maxPixels As Integer) As Boolean
        Dim myImage As MagickImage
        Try
            l(" MOD conv2jpg anfang")
            l(" MOD conv2jpg inn " & inn)
            l(" MOD conv2jpg out " & out)
            myImage = New MagickImage(inn)
            If maxPixels > 0 Then
                If myImage.Width > maxPixels Then
                    myImage.Resize(maxPixels, 0)
                End If
                If myImage.Height > maxPixels Then
                    myImage.Resize(0, maxPixels)
                End If
            End If
            'abstract = getAbstract(myImage)
            myImage.Write(out)
            myImage.Dispose()
            myImage = Nothing
            l(" MOD conv2jpg ende")
            Return True
        Catch ex As Exception
            l("Fehler in conv2jpg: ", ex)
            Return False
        End Try
    End Function

    Private Function getNeuerNameFotoJPG(ele As String, tempverzeichnis As String) As String
        Dim neuernamne As String = ""
        Try
            l(" MOD getNeuerNameFotoJPG anfang")
            If ele.ToLower.EndsWith(".jpg") Then
                neuernamne = ele.ToLower.Replace(".jpg", ".jpg")
                neuernamne = IO.Path.Combine(tempverzeichnis, neuernamne)
                Return neuernamne
            End If
            If ele.ToLower.EndsWith(".jpeg") Then
                neuernamne = ele.ToLower.Replace(".jpeg", ".jpg")
                neuernamne = IO.Path.Combine(tempverzeichnis, neuernamne)
                Return neuernamne
            End If
            If ele.ToLower.EndsWith(".heic") Then
                neuernamne = ele.ToLower.Replace(".heic", ".jpg")
                neuernamne = IO.Path.Combine(tempverzeichnis, neuernamne)
                Return neuernamne
            End If
            l(" MOD getNeuerNameFotoJPG ende")
            Return neuernamne
        Catch ex As Exception
            l("Fehler in getNeuerNameFotoJPG: ", ex)
            Return neuernamne
        End Try
    End Function

    Friend Sub loesche_loeschliste(loeschliste As List(Of String))
        Dim fi As IO.FileInfo
        Try
            For Each loeschdatei As String In loeschliste
                fi = New IO.FileInfo(loeschdatei)
                fi.Delete()
            Next
        Catch ex As Exception
            l("Fehler in getNeuerNameFotoJPG: ", ex)
        End Try
    End Sub
End Class
