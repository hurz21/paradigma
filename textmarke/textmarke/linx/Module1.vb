Imports System.Data

Module Module1
    'Sub l(kkk As String)

    'End Sub
    'Sub l(kkk As String, ex As System.Exception)

    'End Sub
    <Runtime.CompilerServices.Extension()>
    Public Function IsNothingOrEmpty(ByRef dt As DataTable) As Boolean
        Dim result As Boolean = (dt Is Nothing)
        If Not result Then result = dt.Rows.Count = 0
        Return result
    End Function
    <Runtime.CompilerServices.Extension()>
    Public Function IsNothingOrEmpty(ByRef text As String) As Boolean
        Return String.IsNullOrEmpty(text)

    End Function
    <Runtime.CompilerServices.Extension()>
    Public Function IsNothingOrEmpty(ByRef icoll As ICollection) As Boolean
        Return icoll Is Nothing Or icoll.Count = 0
    End Function

    <System.Runtime.CompilerServices.Extension()>
    Public Function ToLogString(ByVal ex As Exception, ByVal additionalMessage As String) As String
        Dim msg As New Text.StringBuilder()

        If Not String.IsNullOrEmpty(additionalMessage) Then
            msg.Append(additionalMessage)
            msg.Append(Environment.NewLine)
        End If

        If ex IsNot Nothing Then
            Try
                Dim orgEx As Exception = ex
                msg.Append("Exception:")
                msg.Append(Environment.NewLine)
                While orgEx IsNot Nothing
                    msg.Append("Message: " & orgEx.Message)
                    msg.Append(Environment.NewLine)
                    orgEx = orgEx.InnerException
                End While

                If ex.Data IsNot Nothing Then
                    For Each i As Object In ex.Data
                        msg.Append("Data :")
                        msg.Append(i.ToString())
                        msg.Append(Environment.NewLine)
                    Next
                End If

                If ex.StackTrace IsNot Nothing Then
                    msg.Append("StackTrace:")
                    msg.Append(Environment.NewLine)
                    msg.Append(ex.StackTrace.ToString())
                    msg.Append(Environment.NewLine)
                End If

                If ex.Source IsNot Nothing Then
                    msg.Append("Source:")
                    msg.Append(Environment.NewLine)
                    msg.Append(ex.Source)
                    msg.Append(Environment.NewLine)
                End If

                If ex.TargetSite IsNot Nothing Then
                    msg.Append("TargetSite:")
                    msg.Append(Environment.NewLine)
                    msg.Append(ex.TargetSite.ToString())
                    msg.Append(Environment.NewLine)
                End If

                Dim baseException As Exception = ex.GetBaseException()
                If baseException IsNot Nothing Then
                    msg.Append("BaseException:")
                    msg.Append(Environment.NewLine)
                    msg.Append(ex.GetBaseException())
                End If
            Finally
            End Try
        End If
        Return msg.ToString()
    End Function
End Module
