Public Class clsFoto
    Property shortname As String = ""
    Property originalFile As String = ""
    Property jpgFile As String = ""
    Property outfile As String = ""
    Public Property thumbnail As String = ""
    Public Property Exifgpslatitude As String = ""
    Public Property Exifgpslongitude As String = ""
    Public Property abstract As String = ""
    Public Property hatkoordinaten As String = ""
    Public Property mapsurl As String = ""
    Public Property ExifDatum As Date
    Public Property Orientation As New ExifWorksQuick.Orientations
    Public Property GpsImgDirRef As String = ""
    Public Property GpsImgDir As String = ""
End Class
Class fotomanager
    Public Function kopiere(quelle As clsFoto) As clsFoto
        Dim neu As New clsFoto
        Try
            l(" MOD kopiere anfang")
            neu.shortname = quelle.shortname
            neu.originalFile = quelle.originalFile
            neu.jpgFile = quelle.jpgFile
            neu.outfile = quelle.outfile
            neu.thumbnail = quelle.thumbnail
            neu.Exifgpslatitude = quelle.Exifgpslatitude
            neu.Exifgpslongitude = quelle.Exifgpslongitude
            neu.abstract = quelle.abstract
            neu.hatkoordinaten = quelle.hatkoordinaten
            neu.mapsurl = quelle.mapsurl
            neu.ExifDatum = quelle.ExifDatum
            neu.Orientation = quelle.Orientation
            neu.GpsImgDirRef = quelle.GpsImgDirRef
            neu.GpsImgDir = quelle.GpsImgDir
            l(" originalFile AMOD kopiere ende")
            Return neu
        Catch ex As Exception
            l("Foutfile As Strehler in kopiere: " & ex.ToString())
        End Try
    End Function
    Sub l(text As String)

    End Sub
End Class

