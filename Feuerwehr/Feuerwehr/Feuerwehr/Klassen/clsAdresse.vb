Imports System.ComponentModel
Public Class clsAdresse
	Implements INotifyPropertyChanged

	Public anyChange As Boolean = False
	Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
	Implements INotifyPropertyChanged.PropertyChanged
	Protected Sub OnPropertyChanged(ByVal prop As String)
		anyChange = True
		RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
	End Sub

	Private _adressID As Integer
	Public Property AdressID() As Integer
		Get
			Return _adressID
		End Get
		Set(ByVal Value As Integer)
			_adressID = Value
		End Set
	End Property


	Private _gemeindename As String
	Public Property Gemeindename() As String
		Get
			Return _gemeindename
		End Get
		Set(ByVal Value As String)
			_gemeindename = Value
			OnPropertyChanged("Gemeindename")
		End Set
	End Property
	Private _gemnrbig As Integer
	Public Property gemnrbig() As Integer
		Get
			Return _gemnrbig
		End Get
		Set(ByVal Value As Integer)
			_gemnrbig = Value
		End Set
	End Property
    Private _strassencode As String
    Public Property Strassencode() As String
        Get
            Return _strassencode
        End Get
        Set(ByVal Value As String)
            _strassencode = Value
        End Set
    End Property
	
	
	Private _strassenname As String
	Public Property Strassenname() As String
		Get
			Return _strassenname
		End Get
		Set(ByVal Value As String)
			_strassenname = Value
			OnPropertyChanged("Strassenname")
		End Set
	End Property
	Private _hausnrKombi As String
	Public Property HausnrKombi() As String
		Get
			Return _hausnrKombi
		End Get
		Set(ByVal Value As String)
			_hausnrKombi = Value
			OnPropertyChanged("HausnrKombi")
		End Set
	End Property
	Sub New()
		anyChange = False
	End Sub
	Sub clear()
		Gemeindename = ""
		Strassenname = ""
		HausnrKombi = ""
		gemnrbig = 0
        Strassencode = "0"""
		anyChange = False
	End Sub
End Class
