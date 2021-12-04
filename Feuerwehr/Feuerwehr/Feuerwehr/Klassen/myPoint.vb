Imports System.ComponentModel

''' <summary>
''' x,y, as double
''' </summary>
''' <remarks></remarks>
Public Class myPoint
	Implements INotifyPropertyChanged
	Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
	Implements INotifyPropertyChanged.PropertyChanged
	Protected Sub OnPropertyChanged(ByVal prop As String)
		RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
	End Sub
	Private _x As Double
	Public Property X() As Double
		Get
			Return _x
		End Get
		Set(ByVal Value As Double)
			_x = Value
			OnPropertyChanged("X")
		End Set
	End Property
	Private _y As Double
	Public Property Y() As Double
		Get
			Return _y
		End Get
		Set(ByVal Value As Double)
			_y = Value
			OnPropertyChanged("Y")
		End Set
	End Property
	Overrides Function toString() As String
		Return X & ", " & Y
	End Function
	Sub New()
		X = 0
		Y = 0
	End Sub
End Class
