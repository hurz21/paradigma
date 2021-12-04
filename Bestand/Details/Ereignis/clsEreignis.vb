Imports System.ComponentModel
Public Class clsEreignis
    Implements INotifyPropertyChanged
    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
     Implements INotifyPropertyChanged.PropertyChanged
    Protected Sub OnPropertyChanged(ByVal prop As String)
        anychange = True
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub
    Public anychange As Boolean
    Public Property istRTF As Boolean = False

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

    Private _art As String
    Public Property Art() As String
        Get
            Return _art
        End Get
        Set(ByVal Value As String)
            _art = Value
            OnPropertyChanged("Art")
        End Set
    End Property

    Private _richtung As String
    Public Property Richtung() As String
        Get
            Return _richtung
        End Get
        Set(ByVal Value As String)
            _richtung = Value
            OnPropertyChanged("Richtung")
        End Set
    End Property
    Private _datum As Date
    Public Property Datum() As Date
        Get
            Return _datum
        End Get
        Set(ByVal Value As Date)
            _datum = Value
            OnPropertyChanged("Datum")
        End Set
    End Property
    Private _beschreibung As String
    Public Property Beschreibung() As String
        Get
            Return _beschreibung
        End Get
        Set(ByVal Value As String)
            _beschreibung = Value
            OnPropertyChanged("Beschreibung")
        End Set
    End Property
    ''' <summary>
    ''' speichert auch die wiedervorlageID
    ''' </summary>
    ''' <remarks></remarks>					
    Private _dokumentID As Integer
    Public Property DokumentID() As Integer
        Get
            Return _dokumentID
        End Get
        Set(ByVal Value As Integer)
            _dokumentID = Value
            OnPropertyChanged("DokumentID")
        End Set
    End Property
    Private _notiz As String
    Public Property Notiz() As String
        Get
            Return _notiz
        End Get
        Set(ByVal Value As String)
            _notiz = Value
            OnPropertyChanged("Notiz")
        End Set
    End Property
    Public Function clearValues() As Boolean
        Notiz = ""
        Art = ""
        Datum = CLstart.mycsimple.MeinNULLDatumAlsDate
        Beschreibung = ""
        DokumentID = 0
        Richtung = "Ausgang"
        Quelle = ""
        istRTF = False
        '	Datum = Nothing 'CLstart.mycsimple.MeinNULLDatumAlsDate
        'Art=""	 weil sonst die info im detail nicht mehr auftaucht!
        ID = 0
    End Function
    Private _quelle As String
    Public Property Quelle() As String
        Get
            Return _quelle
        End Get
        Set(ByVal Value As String)
            _quelle = Value
            OnPropertyChanged("Quelle")
        End Set
    End Property


End Class
Public Class clsEreignisDok
    Inherits clsEreignis
    Property EreignisDokTyp As Integer = 1
    Property dokBeschreibung As string
    '0=ereignis
    '1=Dokument
        Property revisionssicher As Integer
    Property dok As New Dokument
End Class