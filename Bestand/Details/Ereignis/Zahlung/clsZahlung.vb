Imports System.ComponentModel
Public Class clsZahlung
    Implements INotifyPropertyChanged

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
     Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub OnPropertyChanged(ByVal prop As String)
        anychange = True
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub
    Public anychange As Boolean

    Private _zahler As String
    Public Property Zahler() As String
        Get
            Return _zahler
        End Get
        Set(ByVal Value As String)
            _zahler = Value
            OnPropertyChanged("Zahler")
        End Set
    End Property


    Private _Notiz As String
    Public Property Notiz() As String
        Get
            Return _Notiz
        End Get
        Set(ByVal Value As String)
            _Notiz = Value
            OnPropertyChanged("Notiz")
        End Set
    End Property

    Private _sachgebietNr As String
    Public Property SachgebietNr() As String
        Get
            Return _sachgebietNr
        End Get
        Set(ByVal Value As String)
            _sachgebietNr = Value
            OnPropertyChanged("SachgebietNr")
        End Set
    End Property
    Private _vorgangsNR As Integer
    Public Property VorgangsNR() As Integer
        Get
            Return _vorgangsNR
        End Get
        Set(ByVal Value As Integer)
            _vorgangsNR = Value
            OnPropertyChanged("VorgangsNR")
        End Set
    End Property
    Private _aZ As String
    Public Property AZ() As String
        Get
            Return _aZ
        End Get
        Set(ByVal Value As String)
            _aZ = Value
            OnPropertyChanged("AZ")
        End Set
    End Property
    Private _initiale As String
    Public Property Initiale() As String
        Get
            Return _initiale
        End Get
        Set(ByVal Value As String)
            _initiale = Value
            OnPropertyChanged("Initiale")
        End Set
    End Property
    Private _typ As String
    Public Property Typ() As String
        Get
            Return _typ
        End Get
        Set(ByVal Value As String)
            _typ = Value
            OnPropertyChanged("Typ")
        End Set
    End Property
    Private _eingang As Boolean
    Public Property Eingang() As Boolean
        Get
            Return _eingang
        End Get
        Set(ByVal Value As Boolean)
            _eingang = Value
            OnPropertyChanged("Richtung")
        End Set
    End Property
    Private _verschicktAm As DateTime
    Public Property VerschicktAm() As DateTime
        Get
            Return _verschicktAm
        End Get
        Set(ByVal Value As DateTime)
            _verschicktAm = Value
            OnPropertyChanged("VerschicktAm")
        End Set
    End Property

    Private _AngeordnetAm As DateTime
    Public Property AngeordnetAm() As DateTime
        Get
            Return _AngeordnetAm
        End Get
        Set(ByVal value As DateTime)
            _AngeordnetAm = value
            OnPropertyChanged("AngeordnetAm")
        End Set
    End Property

    Private _eingangAm As DateTime
    Public Property EingangAm() As DateTime
        Get
            Return _eingangAm
        End Get
        Set(ByVal Value As DateTime)
            _eingangAm = Value
            OnPropertyChanged("EingangAm")
        End Set
    End Property
    Private _betrag As Double
    Public Property Betrag() As Double
        Get
            Return _betrag
        End Get
        Set(ByVal Value As Double)
            _betrag = Value
            OnPropertyChanged("Betrag")
        End Set
    End Property
    Public Property ZahlungsID() As Integer
    Private _hausHaltsstelle As String
    Public Property HausHaltsstelle() As String
        Get
            Return _hausHaltsstelle
        End Get
        Set(ByVal Value As String)
            _hausHaltsstelle = Value
            OnPropertyChanged("HausHaltsstelle")
        End Set
    End Property
    Private _istAnordnungbestellt As Boolean
    Public Property istAnordnungbestellt() As Boolean
        Get
            Return _istAnordnungbestellt
        End Get
        Set(ByVal Value As Boolean)
            _istAnordnungbestellt = Value
            OnPropertyChanged("istAnordnungbestellt")
        End Set
    End Property
    Private _istAngeordnet As Boolean
    Public Property istAngeordnet() As Boolean
        Get
            Return _istAngeordnet
        End Get
        Set(ByVal Value As Boolean)
            _istAngeordnet = Value
            OnPropertyChanged("istAngeordnet")
        End Set
    End Property

    Sub clear()
        Betrag = 0
        Notiz = ""
        SachgebietNr = ""
        VorgangsNR = 0
        AZ = ""
        Initiale = ""
        Typ = ""
        Eingang = True     'Eingang
        VerschicktAm = CDate("0001-01-01 01:01:01")
        EingangAm = CDate("0001-01-01 01:01:01")
        AngeordnetAm= CDate("0001-01-01 01:01:01")
        Zahler = ""
        ZahlungsID = 0
        istAngeordnet = False
        istAnordnungbestellt = False
        HausHaltsstelle = ""
        Betrag = 0
    End Sub
End Class
