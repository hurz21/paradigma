Imports System.ComponentModel
Public Class SendEmailTexte
	Implements INotifyPropertyChanged
    Public anychange As Boolean
	Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
	Implements INotifyPropertyChanged.PropertyChanged
	Protected Sub OnPropertyChanged(ByVal prop As String)
		anychange = True
		RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
	End Sub

	Private _an As String = ""
	Public Property An() As String
		Get
			Return _an
		End Get
		Set(ByVal Value As String)
			_an = Value
			OnPropertyChanged("An")
		End Set
	End Property
	Private _cC As String = ""
	Public Property CC() As String
		Get
			Return _cC
		End Get
		Set(ByVal Value As String)
			_cC = Value
			OnPropertyChanged("CC")
		End Set
	End Property

	Private _betreff As String = ""
	Public Property Betreff() As String
		Get
			Return _betreff
		End Get
		Set(ByVal Value As String)
			_betreff = Value
			OnPropertyChanged("Betreff")
		End Set
    End Property

	Private _message As String = ""
	Public Property Message() As String
		Get
			Return _message
		End Get
		Set(ByVal Value As String)
			_message = Value
			OnPropertyChanged("Message")
		End Set
	End Property
	Private _anhang As String = ""
	Public Property Anhang() As String
		Get
			Return _anhang
		End Get
		Set(ByVal Value As String)
			_anhang = Value
			OnPropertyChanged("Anhang")
		End Set
	End Property
	Public Sub clear()
		An = ""
		CC=""
		Betreff = ""
		Anhang = ""
		Message = ""
	End Sub
	Sub New()
		clear()
	End Sub
	Public Function isReady() As String
		If Not An.Contains("@") Then Return "Sie müssen eine gültige Email-Adresse eingeben."
		If String.IsNullOrEmpty(Betreff) Then Return "Sie müssen ein Betreff eingeben."
		If String.IsNullOrEmpty(Message) Then Return "Sie müssen eine Nachricht eingeben."
		Return ""
	End Function
End Class
