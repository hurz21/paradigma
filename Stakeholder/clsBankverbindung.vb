Imports System.ComponentModel
Public Class clsBankverbindung
	 	Implements INotifyPropertyChanged	 
	Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
							 Implements INotifyPropertyChanged.PropertyChanged
	Protected Sub OnPropertyChanged(ByVal prop As String)
		RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
	End Sub
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
	Private _bLZ As String
	Public Property BLZ() As String
		Get
			Return _bLZ
		End Get
		Set(ByVal Value As String)
			_bLZ = Value
				OnPropertyChanged("BLZ")
		End Set
	End Property
	Private _kontoNr As String
	Public Property KontoNr() As String
		Get
			Return _kontoNr
		End Get
		Set(ByVal Value As String)
			_kontoNr = Value
				OnPropertyChanged("KontoNr")
		End Set
	End Property
	Private _titel As String
	Public Property Titel() As String
		Get
			Return _titel
		End Get
		Set(ByVal Value As String)
			_titel = Value
				OnPropertyChanged("Titel")
		End Set
	End Property
	Sub clear
		Titel=""
		KontoNr=""
		BLZ=""
		Name=""
		
 End Sub 
End Class
