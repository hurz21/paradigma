'Imports System.ComponentModel
'Public Class Status
'    Implements INotifyPropertyChanged
'    Implements ICloneable
'    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
'     Implements INotifyPropertyChanged.PropertyChanged
'    Protected Sub OnPropertyChanged(ByVal prop As String)
'        anychange = True
'        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
'    End Sub
'    Public anychange As Boolean

'    Private _erledigt As Boolean
'    Public Property erledigt() As Boolean
'        Get
'            Return _erledigt
'        End Get
'        Set(ByVal Value As Boolean)
'            _erledigt = Value
'            OnPropertyChanged("erledigt")
'        End Set
'    End Property
'    Sub clear()
'        erledigt = False
'    End Sub
'    Public Function Clone() As Object Implements System.ICloneable.Clone
'        Return MemberwiseClone()
'    End Function
'End Class
