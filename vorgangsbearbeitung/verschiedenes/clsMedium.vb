Public Class clsMedium
    Public Property DokumentID() As Integer
    'name mit pfad und laufwerk
    Property fullname() As String
    'name mit pfad 
    Property name() As String

    Property schlagworte() As String

    Property filedatum() As DateTime

    Public Sub New()
        filedatum = #1/1/1900#
        schlagworte = ""
        name = ""
    End Sub
    Public Shared Function collTostring(ByVal meicoll As List(Of String), ByRef delim as string) as  String
        Dim tempo$ = ""
        For Each t In meicoll
            tempo$ &= t.ToString & delim$
        Next
        Return tempo$.Trim
    End Function

    Public Shared Function getPathMetaData(ByVal pathName as string) as  List(Of String)
        Dim _metadata As New List(Of String)
        pathName$ = clsMedium.SchlagworteClean(pathName$)
        Dim teile$() = pathName$.Replace("/", "\").Replace("_", "_").Replace("  ", " ").Split(" "c)
        _metadata.Clear()
        For Each t$ In teile
            If t$.Length > 2 Then
                _metadata.Add(t$)
            End If
        Next
        Return _metadata
    End Function
    ''' <summary>
    ''' einige exiffelder haben vorne 8 byte für formatinfos  das wird hier mit blank überschrieben
    ''' 
    ''' </summary>
    ''' <param name="s">der inhalt des feldes als string</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function EXIF_kuerzen(ByRef s as string) as  String
        If s.Length > 8 Then s = s.Substring(8)
        Return s.Trim
    End Function


    Public Shared Function SchlagworteClean(ByVal s as string) as  String
        'file_schlagworte$ = file.Name.Replace("/", " ")
        Dim korrektur$ = s$.Replace("/", " ")
        korrektur$ = korrektur$.Replace("\", " ")
        korrektur$ = korrektur$.Replace(",", " ")
        korrektur$ = korrektur$.Replace(";", " ")
        korrektur$ = korrektur$.Replace(":", " ")
        korrektur$ = korrektur$.Replace("`", " ")
        'korrektur$ = korrektur$.Replace("-", " ")
        korrektur$ = korrektur$.Replace("'", " ")
        korrektur$ = korrektur$.Replace("""", " ")
        korrektur$ = korrektur$.Replace(".", " ")
        korrektur$ = korrektur$.Replace("   ", " ")
        korrektur$ = korrektur$.Replace("  ", " ")
        Return korrektur$
    End Function
    Public Shared Function getthumb(ByVal size As Integer, ByVal img As System.Drawing.Image) _
             As System.Drawing.Image
        'Aufruf     	
        'prethumb = Image.FromFile(clsME.collJPG(i).fullname.Replace("/", "\"))
        'imgMittel = clsME.getthumb(100, prethumb)
        Dim xy As Double
        xy = img.Width / img.Height
        If xy > 1 Then
            getthumb = img.GetThumbnailImage(size, CType(size / xy, Integer), Nothing, Nothing)
        Else
            getthumb = img.GetThumbnailImage(CType(size * xy, Integer), size, Nothing, Nothing)
        End If
    End Function
End Class

''' <summary>
'''     erfordert nicht mehr die exifworksquick class
''' </summary>
''' <remarks>
'''     
''' </remarks>
Public Class clsMyJPG
    Inherits clsMedium
    Implements IComparable(Of clsMyJPG)
    Public Sub New()
        MyBase.New()
        zeitdiff = 0
        gruppe = 0
        EXIFhersteller = ""
        EXIFDateTimeOriginal = #1/1/1900#
        EXIFusercomment$ = ""
        EXIFtitle$ = ""
        EXIFartist = ""
        EXIFrichtung = ""
        Exifgpslatitude = ""
        Exifgpslongitude = ""
        ExifGpsImgDir = ""
    End Sub

    Function compareto(ByVal other As clsMyJPG) As Integer _
     Implements IComparable(Of clsMyJPG).CompareTo
        Return EXIFDateTimeOriginal.CompareTo(other.EXIFDateTimeOriginal)
    End Function
 
    Public zeitdiff As Long

    Public gruppe As Long
    Property EXIFDateTimeOriginal() As DateTime
    Property EXIFusercomment() As String
    Property EXIFtitle() As String
    Property EXIFartist() As String
    Property EXIFrichtung() As String
    Property ExifHeight() As Integer
    Property ExifWidth() As Integer
    Property ExifGpsImgDir() As String
    Property Exifgpslongitude() As String
    Property Exifgpslatitude() As String
    Property EXIFhersteller() As String
    Property EXIFdescription() As String
    Public Property rechts() As String
    Public Property hoch() As String
    Function hasCoords() As Boolean
        Try
            If Exifgpslongitude = "0#0#0#" Or Exifgpslatitude = "0#0#0#" Then
                Return False
            End If
            If String.IsNullOrEmpty(Exifgpslongitude) Or String.IsNullOrEmpty(Exifgpslatitude) Then
                Return False
            End If
            Return True
        Catch ex As Exception
            nachricht("Fehler in clsmedium hatCoords: " & vbCrLf & ex.ToString)
        End Try
    End Function
End Class


