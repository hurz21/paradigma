
Public Class GEMKRZenenViewModel
    Inherits ViewModelBase
    Public anychange As Boolean = False
    Private _cmdAddCommand As ICommand
    Private _cmdRemoveCommand As ICommand
    Private _cmdSaveCommand As ICommand

    Private _objVariable As GEMKRZ
    Private _GEMKRZen As GEMKRZen
    Dim _selectedVariable As GEMKRZ

    Public Property Selection() As GEMKRZ
        Get
            Return _selectedVariable
        End Get
        Set(ByVal value As GEMKRZ)
            If value Is _selectedVariable Then
                Return
            End If
            _selectedVariable = value
            MyBase.OnPropertyChanged("Selection")
        End Set
    End Property

    Public Property GEMKRZen As GEMKRZen
        Get
            Return _GEMKRZen
        End Get
        Set(ByVal value As GEMKRZen)
            Me._GEMKRZen = value
            OnPropertyChanged("GEMKRZen")
        End Set
    End Property

    Public Property Variable() As GEMKRZ
        Get
            Return _objVariable
        End Get
        Set(ByVal Value As GEMKRZ)
            _objVariable = Value
            MyBase.OnPropertyChanged("GEMKRZ")
        End Set
    End Property



    Public Property Name() As String
        Get
            Return _objVariable.Name
        End Get
        Set(ByVal Value As String)
            _objVariable.Name = Value
            MyBase.OnPropertyChanged("GEMKRZ")
        End Set
    End Property

    Public Property ID() As String
        Get
            Return _objVariable.ID
        End Get
        Set(ByVal Value As String)
            _objVariable.ID = Value
            MyBase.OnPropertyChanged("GEMKRZ")
        End Set
    End Property


    Public Property GEME() As String
        Get
            Return _objVariable.GEME
        End Get
        Set(ByVal Value As String)
            _objVariable.GEME = Value
            MyBase.OnPropertyChanged("GEMKRZ")
        End Set
    End Property

    Public Property GEMA() As String
        Get
            Return _objVariable.GEMA
        End Get
        Set(ByVal Value As String)
            _objVariable.GEMA = Value
            MyBase.OnPropertyChanged("GEMKRZ")
        End Set
    End Property


    Public Sub New()
        Me._GEMKRZen = GEMKRZen.LoadVariablen()
    End Sub

    Public Sub New(ByVal VariableCollection As GEMKRZen)
        Me._GEMKRZen = VariableCollection
    End Sub

    Public Sub New(ByVal objVariable As GEMKRZ)
        _objVariable = objVariable
    End Sub

    Public ReadOnly Property AddCommand() As ICommand
        Get
            If _cmdAddCommand Is Nothing Then
                _cmdAddCommand = New RelayCommand(AddressOf AddExecute, AddressOf CanAddExecute)
            End If
            Return _cmdAddCommand
        End Get
    End Property

    Public ReadOnly Property RemoveCommand() As ICommand
        Get
            If _cmdRemoveCommand Is Nothing Then
                _cmdRemoveCommand = New RelayCommand(AddressOf Remove, AddressOf CanRemove)
            End If
            Return _cmdRemoveCommand
        End Get
    End Property



    Public ReadOnly Property SaveCommand() As ICommand
        Get
            If _cmdSaveCommand Is Nothing Then
                _cmdSaveCommand = New RelayCommand(AddressOf Save, AddressOf CanSave)
            End If
            Return _cmdSaveCommand
        End Get
    End Property

    'Restituisce sempre True perchè si può sempre
    'aggiungere un nuovo elemento
    Private Function CanAddExecute(ByVal param As Object) As Boolean
        Return True
    End Function

    Private Sub AddExecute(ByVal param As Object)
        Dim cust As New GEMKRZ
        anychange = True
        Me.GEMKRZen.Add(cust)
    End Sub

    Private Function CanRemove(ByVal param As Object) As Boolean
        Return Me.Selection IsNot Nothing
    End Function


    Private Sub Remove(ByVal param As Object)
        Dim mesres As New MessageBoxResult
        mesres = MessageBox.Show("Vorsicht: Entfernen Sie keine Einträge die bereits in Benutzung sind. Damit zerstören Sie den Bezug zur Datenbank!!!" &
                                 Environment.NewLine &
                                 "Funktion verlassen = Abbruch?", "Vorsicht",
                                MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.Cancel)
        If mesres = MessageBoxResult.Cancel Then Exit Sub
        anychange = True
        Me.GEMKRZen.Remove(Me.Selection)
    End Sub

    Private Function CanSave(ByVal param As Object) As Boolean
        Return True
    End Function

    Private Sub Save(ByVal param As Object)
        Dim doc = <?xml version="1.0"?>
                  <GEMKRZen>
                      <%= From cust In Me.GEMKRZen
                          Select <GEMKRZ Name=<%= cust.Name %>
                                     ID=<%= cust.ID %>
                                     GEME=<%= cust.GEME %>
                                     GEMA=<%= cust.GEMA %>
                                 />
                      %>
                  </GEMKRZen>
        doc.Save(Application.fullpath)
        anychange = False
    End Sub

End Class
