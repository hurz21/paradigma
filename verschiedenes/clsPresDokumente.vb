 

Public Class clsPresDokumente
    'zur presentation in den listen statt datagrids
    Inherits LIBArchiv.Dokument
    'Property ausgewaehlt As Boolean = False
    Private _ausgewaehlt As Boolean = False
    Public Property ausgewaehlt() As Boolean
        Get
            Return _ausgewaehlt
        End Get
        Set(ByVal value As Boolean)
            _ausgewaehlt = value
            OnPropertyChanged("ausgewaehlt")
        End Set
    End Property

End Class
