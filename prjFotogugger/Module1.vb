﻿#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB.LIBDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports System.Data
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Security.Cryptography


Module meineExtensionsIsnullorempty
    'Public Function getDT4Query(sql As String, ByVal myneREC As IDB_grundfunktionen, ByRef hinweis As String) As DataTable
    '    'Dim dt As DataTable 
    '    Try
    '        l("getDT4Query---------------------- anfang")
    '        myneREC.mydb.SQL = sql
    '        nachricht("getDT4Query: " & vbCrLf & myneREC.mydb.SQL)
    '        hinweis = myneREC.getDataDT()
    '        nachricht("  hinweis: " & hinweis)
    '        If myneREC.dt.IsNothingOrEmpty Then
    '            If myneREC.mydb.SQL.ToLower.Trim.StartsWith("delete") Or
    '                myneREC.mydb.SQL.ToLower.Trim.StartsWith("update") Then
    '                'dt darf leer sein
    '                If hinweis.ToLower.Contains("fehler") Then
    '                    ' Return -1
    '                End If
    '            Else
    '                l("Fatal Error ID " & "konnte nicht gefunden werden!" & myneREC.mydb.getDBinfo(""))
    '            End If
    '            Return myneREC.dt
    '        End If
    '        ' dt = myneREC.dt
    '        Return myneREC.dt
    '        l("getDT4Query---------------------- ende")
    '    Catch ex As Exception
    '        l("Fehler in getDT4Query: " ,ex)
    '        Return Nothing
    '    End Try
    'End Function
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

    ''' <summary> 
    ''' <para>Creates a log-string from the Exception.</para>
    ''' <para>The result includes the stacktrace, innerexception et cetera, separated by <seealso cref="Environment.NewLine"/>.</para>
    ''' </summary>
    ''' <param name="ex">The exception to create the string from.</param>
    ''' <param name="additionalMessage">Additional message to place at the top of the string, maybe be empty or null.</param>
    ''' <returns></returns>
    <System.Runtime.CompilerServices.Extension()>
    Public Function ToLogString(ByVal ex As Exception, ByVal additionalMessage As String) As String
        Dim msg As New StringBuilder()

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
