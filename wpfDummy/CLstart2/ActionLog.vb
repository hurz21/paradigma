Namespace CLstart
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

        Shared Sub nachricht(text As String)
            My.Log.WriteEntry(text)
        End Sub
        Public Sub nachricht(ByVal text As String, ByVal ex As System.Exception)
            Dim anhang As String = ""
            text = text & ToLogString(ex, text)
            'myGlobalz.sitzung.nachrichtenText = text
            My.Log.WriteEntry(text)
            'mitFehlerMail(text, anhang)
        End Sub
        Sub New(ByVal _ofile As String, verzeichnis As String)
            Try

                'sw = New IO.StreamWriter()
                outfile = _ofile
                _logfile = _ofile
                createActiondir(verzeichnis)
            Catch ex As Exception
                nachricht("FEhler in actionlog.New: " ,ex)
            End Try
        End Sub

        Private Sub createActiondir(verz As String)
            Try
                IO.Directory.CreateDirectory(verz)
            Catch ex As Exception

            End Try
        End Sub

        Function buildRecord() As String
            Dim summe As String = wer & delim & Now & delim & vorgang & delim & komponente & delim & aktion & delim
            Return summe
        End Function

        Public Sub log()
            Try
                Using sw As New IO.StreamWriter(_logfile, True, mycSimple.enc)
                    sw.WriteLine(buildRecord)
                End Using
            Catch ex As Exception
                nachricht("FEhler!!!! in actionlog.log: ggf ist archiv nciht mehr im Zugriff" ,ex)
            End Try
        End Sub
        Public Sub endlog()
            Try
            Catch ex As Exception
                nachricht("FEhler in actionlog.endlog: " ,ex)
            End Try
        End Sub

        Sub copyToServer(ByVal serverdir As String) 'myGlobalz.ActionLogDir      
            Try
                Dim testt As New IO.FileInfo(outfile)
                testt.CopyTo(serverdir & testt.Name)
                testt = Nothing
            Catch ex As Exception
                nachricht("FEhler in actionlog.copytoserver: " ,ex)
            End Try
        End Sub

    End Class
End Namespace