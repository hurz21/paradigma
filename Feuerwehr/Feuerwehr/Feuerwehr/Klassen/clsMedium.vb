Imports System.ComponentModel
Public Class clsMedium
	Implements INotifyPropertyChanged
	Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
	Implements INotifyPropertyChanged.PropertyChanged
	Protected Sub OnPropertyChanged(ByVal prop As String)
		RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
	End Sub

	Public DBmediumcrud As IMediumCrud

	Sub New(ByVal eDBplcrud As IMediumCrud)
		DBmediumcrud = eDBplcrud
	End Sub



	Private _iD As Integer
	Public Property ID() As Integer
		Get
			Return _iD
		End Get
		Set(ByVal Value As Integer)
			_iD = Value
			OnPropertyChanged("ID")
		End Set
	End Property
	Private _stammID As Integer
	Public Property StammID() As Integer
		Get
			Return _stammID
		End Get
		Set(ByVal Value As Integer)
			_stammID = Value
			OnPropertyChanged("StammID")
		End Set
	End Property
	Private _laufnr As Integer
	Public Property laufnr() As Integer
		Get
			Return _laufnr
		End Get
		Set(ByVal Value As Integer)
			_laufnr = Value
			OnPropertyChanged("laufnr")
		End Set
	End Property
	Private _dateiname As String
	Public Property Dateiname() As String
		Get
			Return _dateiname
		End Get
		Set(ByVal Value As String)
			_dateiname = Value
			OnPropertyChanged("Dateiname")
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
	Private _relativpfad As String
	Public Property Relativpfad() As String
		Get
			Return _relativpfad
		End Get
		Set(ByVal Value As String)
			_relativpfad = Value
			OnPropertyChanged("Relativpfad")
		End Set
	End Property
	Private _archiv_FullName As String
	Public Property Archiv_FullName() As String
		Get
			Return _archiv_FullName
		End Get
		Set(ByVal Value As String)
			_archiv_FullName = Value
			OnPropertyChanged("Archiv_FullName")
		End Set
	End Property


	Sub clear()
		ID = 0
		StammID = 0
		laufnr = 0
		Titel = ""
		Dateiname = ""
		Relativpfad = ""
		Archiv_FullName = ""
	End Sub

	Sub speichern(ByVal mediumID%)
		If mediumID% = 0 Then
			DBmediumcrud.Medium_create(Me)
		Else
			DBmediumcrud.Medium_update(Me)
		End If
	End Sub
	Function loeschen(ByVal mediumID%) As Integer
		If mediumID% < 1 Then
			Return 0
		Else
			Return DBmediumcrud.Medium_delete(Me)
		End If
	End Function
End Class
