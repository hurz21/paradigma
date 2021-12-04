''' <summary>
''' 
''' </summary>
''' <remarks></remarks>
Public Class ActionLog
    Public ReadOnly outfile As String
    Public _logfile As String
    Private delim As String = ";"
    'Private Property sw As IO.StreamWriter
    Public Property wer As String
    Public Property vorgang As String 'vid 
    Public Property komponente As String '& verlauf
    Public Property aktion As String 'dok(id) XXX gelöscht

    Sub New(ByVal _ofile As String)
        Try

            'sw = New IO.StreamWriter()
            outfile = _ofile
            _logfile = _ofile
            'sw.AutoFlush = True
        Catch ex As Exception
            nachricht("FEhler in actionlog.New: " & ex.ToString)
        End Try
    End Sub

    Function buildRecord() As String
        Dim summe As String = wer & delim & Now & delim & vorgang & delim & komponente & delim & aktion & delim
        Return summe
    End Function

    Public Sub log()
        Try
            Using sw As New IO.StreamWriter(_logfile, True, myGlobalz.enc)
                sw.WriteLine(buildRecord)
            End Using
        Catch ex As Exception
            nachricht("FEhler!!!! in actionlog.log: ggf ist archiv nciht mehr im Zugriff" & ex.ToString)
        End Try
    End Sub
    Public Sub endlog()
        Try
            'sw.Close()
            'sw.Dispose()
        Catch ex As Exception
            nachricht("FEhler in actionlog.endlog: " & ex.ToString)
        End Try
    End Sub

    Sub copyToServer(ByVal serverdir As String) 'myGlobalz.ActionLogDir
        Dim testt As New IO.FileInfo(outfile)
        Try
            testt.CopyTo(serverdir & testt.Name)
        Catch ex As Exception
            nachricht("FEhler in actionlog.copytoserver: " & ex.ToString)
        End Try
    End Sub

End Class
