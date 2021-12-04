Imports System.ComponentModel



Public Class AktenStandort
    Implements INotifyPropertyChanged
    Implements ICloneable

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
     Implements INotifyPropertyChanged.PropertyChanged
    Protected Sub OnPropertyChanged(ByVal prop As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub
    Public Function Clone() As Object Implements System.ICloneable.Clone
        Return MemberwiseClone()
    End Function

    Private _RaumNr As String
    Public Property RaumNr() As String
        Get
            Return _RaumNr
        End Get
        Set(ByVal value As String)
            _RaumNr = value
            OnPropertyChanged("RaumNr")
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
    Sub clear()
        RaumNr = "unbekannt"
        Titel = ""
    End Sub
    Sub New()
        clear()
    End Sub

End Class
Public Class Gutachten
    Implements INotifyPropertyChanged
    Implements ICloneable

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
     Implements INotifyPropertyChanged.PropertyChanged
    Protected Sub OnPropertyChanged(ByVal prop As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub
    Public Function Clone() As Object Implements System.ICloneable.Clone
        Return MemberwiseClone()
    End Function

    Private _existiert As Boolean
    Public Property existiert() As Boolean
        Get
            Return _existiert
        End Get
        Set(ByVal value As Boolean)
            _existiert = value
            OnPropertyChanged("existiert")
        End Set
    End Property


    Private _UnterDokumente As Boolean
    Public Property UnterDokumente() As Boolean
        Get
            Return _UnterDokumente
        End Get
        Set(ByVal value As Boolean)
            _UnterDokumente = value
            OnPropertyChanged("UnterDokumente")
        End Set
    End Property
    Sub clear()
        UnterDokumente = False
        existiert = False
    End Sub
    Sub New()
        clear()
    End Sub

End Class