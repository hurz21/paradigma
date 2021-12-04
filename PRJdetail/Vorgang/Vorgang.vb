Imports System.ComponentModel
Public Class Vorgang
    Implements INotifyPropertyChanged
    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
   Implements INotifyPropertyChanged.PropertyChanged
    Protected Sub OnPropertyChanged(ByVal prop As String)
        anychange = True
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub
    Public anychange As Boolean
    Property istConjectVorgang As Boolean = False ' der Vorgang wird in conject= bauantragonline - bearbeitet

    Private _Stammdaten As New Stamm(CLstart.mycSimple.MeinNULLDatumAlsDate)
    Public Property Stammdaten() As Stamm
        Get
            Return _Stammdaten
        End Get
        Set(ByVal value As Stamm)
            _Stammdaten = value
            OnPropertyChanged("Stamm")
        End Set
    End Property
    Property KostenStatus As New clsKosten
    Public Sub clear()
        Stammdaten.clear()
    End Sub
End Class
