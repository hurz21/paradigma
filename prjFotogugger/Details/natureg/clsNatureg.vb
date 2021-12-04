Imports System.ComponentModel

Public Class clsNatureg
    Implements INotifyPropertyChanged
    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
     Implements INotifyPropertyChanged.PropertyChanged



    Protected Sub OnPropertyChanged(ByVal prop As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub

    Private _MassnahmenNr As String
    Public Property MassnahmenNr() As String
        Get
            Return _MassnahmenNr
        End Get
        Set(ByVal value As String)
            _MassnahmenNr = value
            OnPropertyChanged("MassnahmenNr")
        End Set
    End Property


    Private _timestamp As Date
    Public Property timestamp() As Date
        Get
            Return _timestamp
        End Get
        Set(ByVal value As Date)
            _timestamp = value
            OnPropertyChanged("timestamp")
        End Set
    End Property


    Private _typ As String
    Public Property typ() As String
        Get
            Return _typ
        End Get
        Set(ByVal value As String)
            _typ = value
            OnPropertyChanged("typ")
        End Set
    End Property

    Private _art As String
    Public Property art() As String
        Get
            Return _art
        End Get
        Set(ByVal value As String)
            _art = value
            OnPropertyChanged("art")
        End Set
    End Property
    Private _nummer As String
    Public Property nummer() As String
        Get
            Return _nummer
        End Get
        Set(ByVal value As String)
            _nummer = value
            OnPropertyChanged("nummer")
        End Set
    End Property

    Private _beschreibung As String
    Public Property beschreibung() As String
        Get
            Return _beschreibung
        End Get
        Set(ByVal value As String)
            _beschreibung = value
            OnPropertyChanged("beschreibung")
        End Set
    End Property

    Private _notiz As String
    Public Property notiz() As String
        Get
            Return _notiz
        End Get
        Set(ByVal value As String)
            _notiz = value
            OnPropertyChanged("notiz")
        End Set
    End Property
    Private _Quelle As String
    Public Property Quelle() As String
        Get
            Return _Quelle
        End Get
        Set(ByVal value As String)
            _Quelle = value
            OnPropertyChanged("Quelle")
        End Set
    End Property
    Private newPropertyValue As Integer
    Public Property ID() As Integer
        Get
            Return newPropertyValue
        End Get
        Set(ByVal value As Integer)
            newPropertyValue = value
        End Set
    End Property

    Private _VorgangsID As Integer
    Public Property VorgangsID() As Integer
        Get
            Return _VorgangsID
        End Get
        Set(ByVal value As Integer)
            _VorgangsID = value
        End Set
    End Property

    Sub New()
        clear()
    End Sub
    Sub clear()
        ID = 0
        VorgangsID = 0
        Quelle = ""
        typ = ""
        art = ""
        nummer = ""
        beschreibung = ""
        notiz = ""
        MassnahmenNr = ""
    End Sub
End Class
