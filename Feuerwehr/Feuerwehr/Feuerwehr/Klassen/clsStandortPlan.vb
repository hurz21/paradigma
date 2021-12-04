Imports System.ComponentModel
Public Class clsStandortPlan
	Implements INotifyPropertyChanged

	Public anyChange As Boolean = False
	Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
	Implements INotifyPropertyChanged.PropertyChanged
	Protected Sub OnPropertyChanged(ByVal prop As String)
		anyChange = True
		RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
	End Sub



	Public DBplcrud As IFeuerwehrPlanStandort

	Sub New(ByVal eDBplcrud As IFeuerwehrPlanStandort)
		DBplcrud = eDBplcrud
	End Sub

	Private _pt As New myPoint
	Public Property pt() As myPoint
		Get
			Return _pt
		End Get
		Set(ByVal Value As myPoint)
			_pt = Value
			OnPropertyChanged("pt")
		End Set
	End Property
	Private _adr As New clsAdresse
	Public Property adr() As clsAdresse
		Get
			Return _adr
		End Get
		Set(ByVal Value As clsAdresse)
			_adr = Value
			OnPropertyChanged("adr")
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
	Private _hinweis1 As String
	Public Property Hinweis1() As String
		Get
			Return _hinweis1
		End Get
		Set(ByVal Value As String)
			_hinweis1 = Value
			OnPropertyChanged("Hinweis1")
		End Set
	End Property
	Private _hinweis2 As String
	Public Property Hinweis2() As String
		Get
			Return _hinweis2
		End Get
		Set(ByVal Value As String)
			_hinweis2 = Value
			OnPropertyChanged("Hinweis2")
		End Set
	End Property
	Private _rechts As Double
	Public Property Rechts() As Double
		Get
			Return _rechts
		End Get
		Set(ByVal Value As Double)
			_rechts = Value
			OnPropertyChanged("Rechts")
		End Set
	End Property
	Private _hoch As Double
	Public Property Hoch() As Double
		Get
			Return _hoch
		End Get
		Set(ByVal Value As Double)
			_hoch = Value
			OnPropertyChanged("Hoch")
		End Set
	End Property
	Private _relativPfad As String
	Public Property RelativPfad() As String
		Get
			Return _relativPfad
		End Get
		Set(ByVal Value As String)
			_relativPfad = Value
		End Set
	End Property

	Sub New()
		Dim adr As New clsAdresse
		clear()
	End Sub
	Sub clear()
		Rechts = 0
		Hoch = 0
		StammID = 0
		Hinweis1 = ""
		Hinweis2 = ""
		Titel = ""
		RelativPfad = ""
		anyChange = False
		adr.clear()
	End Sub
	Sub speichern(ByVal stammid%)
		If stammid = 0 Then
			DBplcrud.create(Me)
		Else
			DBplcrud.update(Me)
		End If
	End Sub
	Function loeschen(ByVal stammid%) As Integer
		If stammid% < 1 Then
			Return 0
		Else
			Return DBplcrud.delete(Me)
		End If
	End Function
End Class
