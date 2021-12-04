'Imports System.ComponentModel
'Public Class LetzteBearbeitung
'    Implements INotifyPropertyChanged
'    Implements ICloneable
'  Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
' Implements INotifyPropertyChanged.PropertyChanged
'  Protected Sub OnPropertyChanged(ByVal prop As String)
'    anychange = True
'    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
'  End Sub
'    Public anychange As Boolean
'  Private _datum As Date
'  Public Property datum() As Date
'    Get
'      Return _datum
'    End Get
'    Set(ByVal Value As Date)
'      _datum = Value
'      OnPropertyChanged("datum")
'    End Set
'    End Property

'    Public Function Clone() As Object Implements System.ICloneable.Clone
'        Return MemberwiseClone()
'    End Function
'End Class