Imports System.ComponentModel
Public Class clsMinimapLayer
    Implements INotifyPropertyChanged
    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
                     Implements INotifyPropertyChanged.PropertyChanged

    Protected Sub OnPropertyChanged(ByVal prop As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub

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

    Private _Featureclass As String
    Public Property Featureclass() As String
        Get
            Return _Featureclass
        End Get
        Set(ByVal value As String)
            _Featureclass = value
            OnPropertyChanged("Featureclass")
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
