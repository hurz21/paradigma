Imports System.ComponentModel

Public Class ParaLine
  Implements iRaumbezug
  Implements ICloneable

  Implements INotifyPropertyChanged

  Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
   Implements INotifyPropertyChanged.PropertyChanged
  Protected Sub OnPropertyChanged(ByVal prop As String)
    anychange = True
    RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
  End Sub
    Public anychange As Boolean
     Public Property mitEtikett As Boolean = False Implements iRaumbezug.MITETIKETT
    
    Private _FlaecheQm As Double
    Public Property FlaecheQm() As Double Implements iRaumbezug.FLAECHEQM
        Get
            Return _FlaecheQm
        End Get
        Set(ByVal value As Double)
            _FlaecheQm = value
            OnPropertyChanged("FlaecheQm")
        End Set
    End Property

    Private _LaengeM As Double
    Public Property LaengeM() As Double Implements iRaumbezug.LAENGEM
        Get
            Return _LaengeM
        End Get
        Set(ByVal value As Double)
            _LaengeM = value
            OnPropertyChanged("LaengeM")
        End Set
    End Property
    Private _isMapEnabled As Boolean
    Public Property isMapEnabled() As Boolean Implements iRaumbezug.isMapEnabled
        Get
            Return _isMapEnabled
        End Get
        Set(ByVal value As Boolean)
            _isMapEnabled = value
            OnPropertyChanged("isMapEnabled")
        End Set
    End Property

  Public Function punktisvalid() As Boolean Implements iRaumbezug.PunktIsValid
    If punkt.X < 10000 Then Return False
    If punkt.Y < 10000 Then Return False
    Return True
  End Function
    Private _Freitext As String
    Public Property Freitext() As String Implements iRaumbezug.Freitext
        Get
            Return _Freitext
        End Get
        Set(ByVal value As String)
            _Freitext = value
            OnPropertyChanged("Freitext")
        End Set
    End Property
    Private _status As Integer
    Public Property Status As Integer Implements iRaumbezug.Status
        Get
            Return _status
        End Get
        Set(ByVal Value As Integer)
            _status = Value
            OnPropertyChanged("Status")
        End Set
    End Property

  Private _sekID As Long
  Public Property SekID() As Long Implements iRaumbezug.SekID
    Get
      Return _sekID
    End Get
    Set(ByVal Value As Long)
      _sekID = Value
      OnPropertyChanged("SekID")
    End Set
  End Property

  Private _abstract As String
  Public Property abstract() As String Implements iRaumbezug.abstract
    Get
      Return _abstract
    End Get
    Set(ByVal Value As String)
      _abstract = Value
      OnPropertyChanged("abstract")
    End Set
  End Property
  Private _id As Long
  Public Property id() As Long Implements iRaumbezug.id
    Get
      Return _id
    End Get
    Set(ByVal value As Long)
      _id = value
    End Set
  End Property
  Private _box As New clsRange
  Public Property box() As clsRange Implements iRaumbezug.box
    Get
      Return _box
    End Get
    Set(ByVal value As clsRange)
      _box = value
      OnPropertyChanged("box")
    End Set
  End Property
  Private _name As String
  Public Property name() As String Implements iRaumbezug.name
    Get
      Return _name
    End Get
    Set(ByVal value As String)
      _name = value
    End Set
  End Property
  Private _punkt As New myPoint
  Public Property punkt() As myPoint Implements iRaumbezug.punkt
    Get
      Return _punkt
    End Get
    Set(ByVal value As myPoint)
      _punkt = value
    End Set
  End Property
  Private _typ As RaumbezugsTyp
  Public Property typ() As RaumbezugsTyp Implements iRaumbezug.typ
    Get
      Return _typ
    End Get
    Set(ByVal value As RaumbezugsTyp)
      _typ = value
      OnPropertyChanged("Typ")
    End Set
  End Property
  Public Function Clone() As Object Implements System.ICloneable.Clone
    Return MemberwiseClone()
  End Function
  Private _normflst As New clsFlurstueck("")
  Public Property normflst() As clsFlurstueck
    Get
      Return _normflst
    End Get
    Set(ByVal value As clsFlurstueck)
      _normflst = value
    End Set
  End Property
End Class
