Public Class clstools
    Function conv2jpg(inn As String, out As String, maxPixels As Integer, ByRef abstract As String) As Boolean
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
            abstract = getAbstract(myImage)
            myImage.Write(out)
            myImage.Dispose()
            myImage = Nothing
            l(" MOD conv2jpg ende")
            Return True
        Catch ex As Exception
            l("Fehler in conv2jpg: " & ex.ToString())
            Return False
        End Try
    End Function

    Private Function getAbstract(myImage As MagickImage) As String
        Dim profile As ExifProfile
        Dim trenner = ", "
        Dim abs As String = ""
        Dim b1 As Object
        Dim b2 As Object
        Dim b3 As Object
        Try
            l(" MOD getAbstract anfang")
            profile = CType(myImage.GetExifProfile, ExifProfile)
            If (profile IsNot Nothing) Then
                'b1 = profile.GetValue(ExifTag.GPSLatitude)
                'b2 = b1.ToString
                'b3 = b1(1)
                'For i = 0 To profile.Values.Count

                '    If profile.Values(i).Tag.ToString = "GPSLongitude" Then
                '        Debug.Print(profile.Values(35).IsArray.ToString)

                '        'aaa = CType(profile.Values(35), IExifValue)

                '        'Debug.Print(aaa.GetValue(1).ToString)
                '        'Debug.Print(aaa.GetValue(2).ToString)
                '    End If
                'Next 
                abs = profile.Values(2).ToString & trenner &
                      profile.Values(3).ToString & trenner &
                      profile.Values(9).ToString & trenner &
                      profile.Values(35).Tag.ToString

            End If
            l(" MOD getAbstract ende")
            Return abs
        Catch ex As Exception
            l("Fehler in getAbstract: " & ex.ToString())
            Return abs
        End Try
    End Function

    Sub setLogfile()
        'MsgBox(strGlobals.localDocumentCacheRoot)
        With My.Log.DefaultFileLogWriter
#If DEBUG Then
            '.CustomLocation = mgisUserRoot & "logs\"
#Else
#End If
            '.CustomLocation = My.Computer.FileSystem.SpecialDirectories.Temp & "\mgis_logs\"
            .CustomLocation = "c:\kreisoffenbach\heic" & "\logs\"
            '.BaseFileName = GisUser.username & "_" & Format(Now, "yyyyMMddhhmmss")
            .BaseFileName = "heic_" '& Format(Now, "yyyyMMddhhmmss")
            .AutoFlush = False
            .Append = False
        End With
    End Sub

    Sub l(text As String)
        text = text.Replace("DefaultSource	Information	0	", "")
        'text = text & Environment.NewLine & ToLogString(exec, "")

        'text = text & exec.ToString
        'If myglobalz.minErrorMessages Then
        '    If Not (text.ToLower.StartsWith("fehler") Or text.ToLower.StartsWith("warnung")) Then
        '        Exit Sub
        '    End If
        'End If
        My.Log.WriteEntry("clstoools: " & text)
    End Sub
    Function calcOutfileName(endung As String, filename As String, direktoryname As String) As String
        Dim out As String
        Dim nameneu As String
        Dim fi As IO.FileInfo
        Try
            l(" MOD calcOutfileName anfang")
            fi = New IO.FileInfo(filename)
            nameneu = fi.Name.ToLower.Replace(".heic", endung)
            nameneu = nameneu.Replace(".bmp", endung)
            nameneu = nameneu.Replace(".gif", endung)
            nameneu = nameneu.Replace(".jpg", endung)
            nameneu = nameneu.Replace(".tif", endung)
            nameneu = nameneu.Replace(".tiff", endung)
            nameneu = nameneu.Replace(".png", endung)
            nameneu = nameneu.Replace(".png", endung)
            nameneu = nameneu.Replace(".tiff", endung)
            out = IO.Path.Combine(direktoryname, nameneu)
            l(" MOD calcOutfileName ende")
            Return out

        Catch ex As Exception
            l("Fehler in calcOutfileName: " & ex.ToString())
            Return ""
        End Try
    End Function

    Friend Function alteInhalteloeschen(sPath As String) As Boolean
        Dim flist() As String
        Try
            l(" MOD alteInhalteloeschen anfang")
            Dim oDir As New System.IO.DirectoryInfo(sPath)
            Dim oFiles As System.IO.FileInfo() = oDir.GetFiles()
            Dim oFile As System.IO.FileInfo
            For Each oFile In oFiles
                oFile.Delete()
            Next
            l(" MOD alteInhalteloeschen ende")
            Return True
        Catch ex As Exception
            l("Fehler in alteInhalteloeschen: " & ex.ToString())
            Return False
        End Try
    End Function

    Friend Sub erzeugeVerzeichnisse(verz As String)
        Try
            IO.Directory.CreateDirectory(verz)

        Catch ex As Exception
            l("fehler beim anlegen der verzeichnisse " & verz)
        End Try
    End Sub

    Friend Sub verzeichnisOeffnen(ausgabeVerzeichnis As String)
        Try
            l(" MOD verzeichnisOeffnen anfang")
            System.Diagnostics.Process.Start(ausgabeVerzeichnis)
            l(" MOD verzeichnisOeffnen ende")
        Catch ex As Exception
            l("Fehler in verzeichnisOeffnen: " & ex.ToString())
        End Try
    End Sub

    Friend Function ArrayBereinigen(filenames() As String) As String()
        Dim neuarray As String()
        Dim icnt As Integer = -1
        Try
            l(" MOD ArrayBereinigen anfang")
            For Each dateiname In filenames
                If isBildDatei(dateiname) Then
                    icnt += 1
                    ReDim Preserve neuarray(icnt)
                    neuarray(icnt) = dateiname
                End If
            Next
            l(" MOD ArrayBereinigen ende")
            Return neuarray
        Catch ex As Exception
            l("Fehler in ArrayBereinigen: " & ex.ToString())
            Return neuarray
        End Try
    End Function

    Private Function isBildDatei(dateiname As String) As Boolean
        Try
            l(" MOD isBildDatei anfang")
            If dateiname.ToLower.EndsWith(".jpeg") Then Return True
            If dateiname.ToLower.EndsWith(".jpg") Then Return True
            If dateiname.ToLower.EndsWith(".gif") Then Return True
            If dateiname.ToLower.EndsWith(".tiff") Then Return True
            If dateiname.ToLower.EndsWith(".heic") Then Return True
            If dateiname.ToLower.EndsWith(".png") Then Return True
            If dateiname.ToLower.EndsWith(".tif") Then Return True
            If dateiname.ToLower.EndsWith(".bmp") Then Return True
            If dateiname.ToLower.EndsWith(".gif") Then Return True
            l(" MOD isBildDatei ende")
            Return False
        Catch ex As Exception
            l("Fehler in isBildDatei: " & ex.ToString())
            Return False
        End Try
    End Function

    Friend Sub koordinatenHolen(bilder As List(Of clsFoto))
        Try
            l(" MOD koordinatenHolen anfang")
            For Each bild As clsFoto In bilder
                Dim exifQ As ExifWorksQuick
                exifQ = New ExifWorksQuick(bild.jpgFile)

                If exifQ Is Nothing Then
                    l("datei im eimer, exifprüfung: " & bild.jpgFile)
                    Continue For
                End If
                'aktJPG.ExifQ.Width
                If exifQ.Width < 0 Then
                    l("datei de   dateidefekt += 1 ")
                    exifQ.Dispose()
                    exifQ = Nothing
                    Continue For
                End If
                bild.ExifDatum = exifQ.DateTimeOriginal
                'bild.EXIFhersteller = exifQ.EquipmentMaker
                bild.Orientation = exifQ.Orientation
                If exifQ.gpslatitude = "0#0#0#" Then
                    bild.hatkoordinaten = ""
                    Continue For
                End If
                bild.hatkoordinaten = "hat Koordinaten"
                If bild.GpsImgDir <> String.Empty Then
                    bild.abstract = bild.abstract & Environment.NewLine &
                        "Blick:" & bild.GpsImgDir & " (" & bild.GpsImgDirRef & ")"
                End If
                bild.GpsImgDir = exifQ.GpsImgDir
                bild.GpsImgDirRef = exifQ.GpsImgDirRef
                bild.Exifgpslatitude = exifQ.gpslatitude
                bild.Exifgpslongitude = exifQ.gpslongitude
                Dim lat() As String = bild.Exifgpslatitude.Split("#"c)
                Dim lon() As String = bild.Exifgpslongitude.Split("#"c)
                'bild.mapsurl ="https://www.google.com/maps/place/50%C2%B002'03.1%22N+8%C2%B052'47.2%22E"
                bild.mapsurl = "https://www.google.com/maps/place/"
                bild.mapsurl = bild.mapsurl & lat(0) & "%C2%B00" & lat(1) & "'" & lat(2).Replace(",", ".") & "%22N+"
                bild.mapsurl = bild.mapsurl & lon(0) & "%C2%B0" & lon(1) & "'" & lon(2).Replace(",", ".") & "%22E"
                'bild.ExifGpsImgDir = exifQ.GpsImgDir
                ''  enthält ggf die GK koordinaten
                'bild.EXIFdescription = exifQ.Description
            Next
            l(" MOD koordinatenHolen ende")
        Catch ex As Exception
            l("Fehler in koordinatenHolen: " & ex.ToString())
        End Try
    End Sub


End Class
