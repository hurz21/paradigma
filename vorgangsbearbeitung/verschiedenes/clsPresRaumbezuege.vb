Imports System.ComponentModel
Public Class clsPresRaumbezuege
    'zur presentation in den listen statt datagrids
    Implements INotifyPropertyChanged
    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
     Implements INotifyPropertyChanged.PropertyChanged
    Protected Sub OnPropertyChanged(ByVal prop As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub

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


    Private _FlaecheQm As Double
    Public Property FlaecheQm() As Double
        Get
            Return _FlaecheQm
        End Get
        Set(ByVal value As Double)
            _FlaecheQm = value
            OnPropertyChanged("FlaecheQm")
        End Set
    End Property

    Private _LaengeM As Double
    Public Property LaengeM() As Double
        Get
            Return _LaengeM
        End Get
        Set(ByVal value As Double)
            _LaengeM = value
            OnPropertyChanged("LaengeM")
        End Set
    End Property

    Private _rid As Integer
    Public Property rid() As Integer
        Get
            Return _rid
        End Get
        Set(ByVal value As Integer)
            _rid = value
            OnPropertyChanged("rid")
        End Set
    End Property
    Private _typ As Integer
    Public Property typ() As Integer
        Get
            Return _typ
        End Get
        Set(ByVal value As Integer)
            _typ = value
            OnPropertyChanged("typ")
        End Set
    End Property
    Private _sekid As Integer
    Public Property sekid() As Integer
        Get
            Return _sekid
        End Get
        Set(ByVal value As Integer)
            _sekid = value
            OnPropertyChanged("sekid")
        End Set
    End Property
    Private _Titel As String
    Public Property Titel() As String
        Get
            Return _Titel
        End Get
        Set(ByVal value As String)
            _Titel = value
            OnPropertyChanged("Titel")
        End Set
    End Property
    Private _Abstract As String
    Public Property Abstract() As String
        Get
            Return _Abstract
        End Get
        Set(ByVal value As String)
            _Abstract = value
            OnPropertyChanged("Abstract")
        End Set
    End Property
    Private _Freitext As String
    Public Property Freitext() As String
        Get
            Return _Freitext
        End Get
        Set(ByVal value As String)
            _Freitext = value
            OnPropertyChanged("_Freitext")
        End Set
    End Property


    Private _rechts As Integer
    Public Property rechts() As Integer
        Get
            Return _rechts
        End Get
        Set(ByVal value As Integer)
            _rechts = value
            OnPropertyChanged("rechts")
        End Set
    End Property

    Private _hoch As Integer
    Public Property hoch() As Integer
        Get
            Return _hoch
        End Get
        Set(ByVal value As Integer)
            _hoch = value
            OnPropertyChanged("hoch")
        End Set
    End Property
    Private _xmin As Integer
    Public Property xmin() As Integer
        Get
            Return _xmin
        End Get
        Set(ByVal value As Integer)
            _xmin = value
            OnPropertyChanged("xmin")
        End Set
    End Property
    Private _xmax As Integer
    Public Property xmax() As Integer
        Get
            Return _xmax
        End Get
        Set(ByVal value As Integer)
            _xmax = value
            OnPropertyChanged("xmax")
        End Set
    End Property
    Private _ymin As Integer
    Public Property ymin() As Integer
        Get
            Return _ymin
        End Get
        Set(ByVal value As Integer)
            _ymin = value
            OnPropertyChanged("ymin")
        End Set
    End Property
    Private _ymax As Integer
    Public Property ymax() As Integer
        Get
            Return _ymax
        End Get
        Set(ByVal value As Integer)
            _ymax = value
            OnPropertyChanged("ymax")
        End Set
    End Property

    Private _mapisenabled As Boolean
    Public Property mapisenabled() As Boolean
        Get
            Return _mapisenabled
        End Get
        Set(ByVal value As Boolean)
            _mapisenabled = value
            OnPropertyChanged("mapisenabled")
        End Set
    End Property

End Class
