

Imports paradigmaDetail

Public Class clsPresDokumente
    'zur presentation in den listen statt datagrids
    Inherits Dokument
    'Property ausgewaehlt As Boolean = False

    Property thumbnailFullPath As String = "--"
    Property thumbnailMSGtext As String
    Property defaultForeground As SolidColorBrush

    Private _ForegroundBrush As New SolidColorBrush(Colors.Black)
    Public Property ForegroundBrush() As SolidColorBrush
        Get
            Return _ForegroundBrush
        End Get
        Set(ByVal value As SolidColorBrush)
            _ForegroundBrush = value
            OnPropertyChanged("ForegroundBrush")
        End Set
    End Property



    Private _ausgewaehlt As Boolean = False
    Public Property ausgewaehlt() As Boolean
        Get
            Return _ausgewaehlt
        End Get
        Set(ByVal value As Boolean)
            _ausgewaehlt = value
            OnPropertyChanged("ausgewaehlt")
        End Set
    End Property
    Private _istGeoeffnet As Boolean = False

    Public Sub New(vordergrundfarbe As SolidColorBrush)
        defaultForeground = vordergrundfarbe
    End Sub
    Public Sub New()

    End Sub


    Public Property istGeoeffnet() As Boolean
        Get
            Return _istGeoeffnet
        End Get
        Set(ByVal value As Boolean)
            _istGeoeffnet = value
            OnPropertyChanged("istGeoeffnet")
        End Set
    End Property
    Function dokument2Presdokument(doku As Dokument) As clsPresDokumente
        Dim ad As New clsPresDokumente
        With doku 'myglobalz.sitzung.aktDokument
            ad.DocID = CInt(.DocID)
            ad.dokumentPfad = .dokumentPfad
            ad.DateinameMitExtension = .DateinameMitExtension
            ad.Typ = .Typ
            ad.Beschreibung = .Beschreibung
            ad.Filedatum = CDate(.Filedatum)
            ad.Checkindatum = CDate(.Checkindatum)
            ad.istVeraltet = CBool(.istVeraltet)
            ad.ExifDatum = CDate(.ExifDatum)
            ad.EXIFlon = CStr(.EXIFlon)
            ad.EXIFlat = CStr(.EXIFlat)
            ad.EXIFdir = CStr(.EXIFdir)
            ad.EXIFhersteller = CStr(.EXIFhersteller)
            ad.revisionssicher = CBool(.revisionssicher)
            ad.sizeMb = CDbl(.sizeMb)
            ad.kompressed = CBool(.kompressed)
            ad.Initiale = CStr(.Initiale)
            ad.EreignisID = (.EreignisID)
            ad.VorgangsID = CInt(.VorgangsID)
            ad.newSaveMode = CBool(.newSaveMode)
            ad.FullnameCheckout = CStr(.FullnameCheckout)
            ad.FullnameImArchiv = CStr(.FullnameImArchiv)
            ad.nurzumlesen = CBool(.nurzumlesen)
            ad.ausgewaehlt = False
            ad.ForegroundBrush = defaultForeground
        End With
        Return ad
        Try
            ad.istNurVerwandt = True 'CBool(clsDBtools.toBool(item.dok.status)
        Catch ex As Exception
            nachricht("fehler in ", ex)
            Return Nothing
        End Try
    End Function

    'Public Shared Widening Operator CType(v As DependencyObject) As clsPresDokumente
    '    Throw New NotImplementedException()
    'End Operator
End Class
