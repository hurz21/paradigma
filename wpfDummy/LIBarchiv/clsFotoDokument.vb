
Public Class clsFotoDokument
    Inherits Dokument
    Private _thumbfullname As String = ""
    Public Property thumbfullname() As String
        Get
            Return _thumbfullname
        End Get
        Set(ByVal value As String)
            _thumbfullname = value
            OnPropertyChanged("thumbfullname")
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

    Private _pixelWidth As Integer
    Public Property pixelWidth() As Integer
        Get
            Return _pixelWidth
        End Get
        Set(ByVal value As Integer)
            _pixelWidth = value
            OnPropertyChanged("pixelWidth")
        End Set
    End Property

    Private _pixelHeight As Integer
    Public Property pixelHeight() As Integer
        Get
            Return _pixelHeight
        End Get
        Set(ByVal value As Integer)
            _pixelHeight = value
            OnPropertyChanged("pixelHeight")
        End Set
    End Property

    Private _ThumbWidth As Integer = 128
    Public Property ThumbWidth() As Integer
        Get
            Return _ThumbWidth
        End Get
        Set(ByVal value As Integer)
            _ThumbWidth = value
            OnPropertyChanged("ThumbWidth")
        End Set
    End Property
    Private _ThumbHeigth As Integer = 128
    Public Property ThumbHeight() As Integer
        Get
            Return _ThumbHeigth
        End Get
        Set(ByVal value As Integer)
            _ThumbHeigth = value
            OnPropertyChanged("ThumbHeight")
        End Set
    End Property

    Private _Etikett As String
    Public Property Etikett() As String
        Get
            Return _Etikett
        End Get
        Set(ByVal value As String)
            _Etikett = value
            OnPropertyChanged("Etikett")
        End Set
    End Property

    Overloads Sub clear(ByVal MeinNULLDatumAlsDate As Date)         'CLstart.mycsimple.MeinNULLDatumAlsDate
        ThumbHeight = 128
        ThumbWidth = 128
        thumbfullname = ""
    End Sub

    ''' <summary>
    ''' erstellt aus dem dokumentanteil des Fotos 
    ''' ein neues dokument und 
    ''' liefert es als solches zurück
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Function reducetoDokument() As Dokument
        Dim dok As New Dokument
        Try
            With dok
                .Beschreibung = Beschreibung
                .Checkindatum = Checkindatum
                .DateinameMitExtension = DateinameMitExtension
                .DocID = DocID
                .DokTyp = DokTyp
                .EreignisID = EreignisID
                .ExifDatum = ExifDatum
                .EXIFdir = EXIFdir
                .EXIFhersteller = EXIFhersteller
                .EXIFlat = EXIFlat
                .EXIFlon = EXIFlon
                .Filedatum = Filedatum
                .FullnameCheckout = FullnameCheckout
                .FullnameImArchiv = FullnameImArchiv
                .OriginalFullname = OriginalFullname
                .OriginalName = OriginalName
                .Handlenr = Handlenr
                .Initiale = Initiale
                .istNurVerwandt = istNurVerwandt
                .istVeraltet = istVeraltet
                .dokumentPfad = dokumentPfad
                .revisionssicher = revisionssicher
                .Typ = Typ
                .VorgangsID = VorgangsID
                .kompressed = kompressed
            End With
            Return dok
        Catch ex As Exception
            Return Nothing
        End Try
    End Function
End Class
