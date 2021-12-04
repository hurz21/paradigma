Imports System.ComponentModel
Namespace CLstart
    Public Class clsProjektAllgemein
        Implements INotifyPropertyChanged
        Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged

        Protected Sub OnPropertyChanged(ByVal prop As String)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
        End Sub

        Sub New(ByVal m_vorgangsid As Integer)
            vorgangsid = m_vorgangsid
        End Sub

        Property refnr As String = ""

        Private _TS As Date
        Public Property TS() As Date
            Get
                Return _TS
            End Get
            Set(ByVal value As Date)
                _TS = value
                OnPropertyChanged("TS")
            End Set
        End Property



        Private _Quelle As String = ""
        Public Property Quelle() As String
            Get
                Return _Quelle
            End Get
            Set(ByVal value As String)
                _Quelle = value
                OnPropertyChanged("Quelle")
            End Set
        End Property


        Private _WiedervorlageID As Integer = 0
        Public Property WiedervorlageID() As Integer
            Get
                Return _WiedervorlageID
            End Get
            Set(ByVal value As Integer)
                _WiedervorlageID = value
            End Set
        End Property

        Private _gemeinde As String = ""
        Public Property Gemeinde() As String
            Get
                Return _gemeinde
            End Get
            Set(ByVal value As String)
                _gemeinde = value
                OnPropertyChanged("Gemeinde")
            End Set
        End Property

        Private _id As Integer = 0
        Public Property id() As Integer
            Get
                Return _id%
            End Get
            Set(ByVal value As Integer)
                _id% = value
                OnPropertyChanged("id")
            End Set
        End Property

        Private _vorgangsid As Integer
        Public Property vorgangsid() As Integer
            Get
                Return _vorgangsid%
            End Get
            Set(ByVal value As Integer)
                _vorgangsid% = value
                OnPropertyChanged("vorgangsid")
            End Set
        End Property

        Private _Kategorie2 As String = ""
        Public Property Kategorie2() As String
            Get
                Return _Kategorie2
            End Get
            Set(ByVal value As String)
                _Kategorie2 = value
                OnPropertyChanged("Kategorie2")
            End Set
        End Property

        Private _Kategorie1 As String = ""
        Public Property Kategorie1() As String
            Get
                Return _Kategorie1
            End Get
            Set(ByVal value As String)
                _Kategorie1 = value
                OnPropertyChanged("Kategorie1")
            End Set
        End Property


        Private _BezeichnungLang As String = ""
        Public Property BezeichnungLang() As String
            Get
                Return _BezeichnungLang
            End Get
            Set(ByVal value As String)
                _BezeichnungLang = value
                OnPropertyChanged("BezeichnungLang")
            End Set
        End Property


        Private _BezeichnungKurz As String = ""
        Public Property BezeichnungKurz() As String
            Get
                Return _BezeichnungKurz
            End Get
            Set(ByVal value As String)
                _BezeichnungKurz = value
                OnPropertyChanged("BezeichnungKurz")
            End Set
        End Property


        Private _von As Date
        Public Property von() As Date
            Get
                Return _von
            End Get
            Set(ByVal value As Date)
                _von = value
                OnPropertyChanged("von")
            End Set
        End Property

        Private _bis As Date
        Public Property bis() As Date
            Get
                Return _von
            End Get
            Set(ByVal value As Date)
                _von = value
                OnPropertyChanged("bis")
            End Set
        End Property


        Public Function bildeTextFuerDetails() As String
            Dim t$ = Kategorie1 & ": " & BezeichnungKurz & " " & BezeichnungLang
            If t.Trim = ":" Then t = ""
            Return t
        End Function

        Public Function bildeTooltipFuerDetails() As String
            Dim t$ = Kategorie1.trim & ": " & BezeichnungKurz.trim & " " & BezeichnungLang .Trim & " " & Gemeinde.trim & vbCrLf & "Angelegt von: " & Quelle & ", bis: " & bis.ToString
            Return t
        End Function

        Sub clear()
            BezeichnungKurz = ""
            BezeichnungLang = ""
            Kategorie1 = ""
            Kategorie2 = ""
            Gemeinde = ""
            Quelle = ""
            WiedervorlageID = 0
            id = 0
            vorgangsid = 0
            TS = Nothing
            von = Nothing
            bis = Nothing
            refnr = ""

        End Sub

    End Class
End Namespace