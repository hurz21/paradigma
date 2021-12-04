Module Module1
    public sw As IO.StreamWriter
    'Public myrec As New clsDBspecOracle
    'Public Property ParaRec As New clsDBspecOracle
    Public Property ParaRec As New clsDBspecMSSQL
    'Public Property HALOREC As New clsDBspecMYSQL
    Public Property HALOREC As New clsDBspecPostgres
    'Public Sub main()
    '    Dim mainw As New MainWindow
    '    mainw.ShowDialog()
    'End Sub

    Public aktPerson As New Person


    Private _aktADR As ParaAdresse
    Public Property aktADR() As ParaAdresse
        Get
            Return _aktADR
        End Get
        Set(ByVal Value As ParaAdresse)
            _aktADR = Value
            ' OnPropertyChanged("aktADR")
        End Set
    End Property


    Public appdir As String = IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "paradigma")
    Sub l(ByVal text$)
        Debug.Print(text)
        sw.WriteLine(text)
    End Sub

    Function getPLZfromGemeinde(ByVal gemeindename As String) As String
        Using neuadr As New clsAdress(gemeindename)
            If clsGemarkungsParams.liegtGemeindeImKreisOffenbach(gemeindename) Then
                Dim test As String = neuadr.gemparms.gemeindetext2PLZ(gemeindename)
                If test = "0" Then
                    Return "0"
                Else
                    Return test
                End If
            Else
                Return "0"
            End If
        End Using
        Return "0"
    End Function
End Module
