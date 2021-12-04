Imports System.ComponentModel
Public Class clsLayerListItem
    Implements INotifyPropertyChanged
    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
     Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub OnPropertyChanged(ByVal prop As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub
     
    Private _dokuTitel As String
    Public Property dokuTitel() As String
        Get
            Return _dokuTitel
        End Get
        Set(ByVal value As String)
            OnPropertyChanged("dokuTitel")
            _dokuTitel = value
        End Set
    End Property

    Private _id As Integer
    Public Property Id() As Integer
        Get
            Return _id
        End Get
        Set(ByVal Value As Integer)
            _id = Value
            OnPropertyChanged("Id")
        End Set
    End Property
    Private _name As String
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal Value As String)
            _name = Value
            OnPropertyChanged("Name")
        End Set
    End Property

    Private _isChecked As Boolean
    Public Property IsChecked() As Boolean
        Get
            Return _isChecked
        End Get
        Set(ByVal Value As Boolean)
            _isChecked = Value
            OnPropertyChanged("IsChecked")
        End Set
    End Property

    Private _istAktiveEbene As Boolean
    Public Property istAktiveEbene() As Boolean
        Get
            Return _istAktiveEbene
        End Get
        Set(ByVal value As Boolean)
            _istAktiveEbene = value
            OnPropertyChanged("istAktiveEbene")
        End Set
    End Property

    Private _istAktivierbar As Boolean
    Public Property istAktivierbar() As Boolean
        Get
            Return _istAktivierbar
        End Get
        Set(ByVal value As Boolean)
            _istAktivierbar = value
            OnPropertyChanged("istAktivierbar")
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

End Class
