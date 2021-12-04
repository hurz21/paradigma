Imports System.Data.SqlClient
'https://dba.stackexchange.com/questions/134129/transaction-in-a-stored-procedure
Module Module1
    Dim cn As SqlClient.SqlConnection
    Sub Main()

        ' GetConnection is a method that creates and return the '
        ' SqlConnection used here according to your connection string'
        Using cn = GetConnection()
            cn.Open()

            ' Create the command with the sproc name and add the parameter required'
            Dim cmd As SqlCommand = New SqlCommand("spGetAllBearbeiter", cn)
            cmd.CommandType = CommandType.StoredProcedure
            cmd.Parameters.AddWithValue("@NACHNAME", "Weyers")
            'cmd.Parameters.AddWithValue("@VORNAME", "")
            'cmd.Parameters.Add("@VORNAME", System.Data.SqlDbType.VarChar).Direction =
            '    System.Data.ParameterDirection.ReturnValue
            ' Ask the command to create an SqlDataReader on the result of the sproc'

            'cmd.ExecuteNonQuery()
            'Dim vorname = cmd.Parameters("@VORNAME").Value


            Using r = cmd.ExecuteReader()

                ' If the SqlDataReader.Read returns true then there is a customer with that ID'
                If r.Read() Then

                    ' Get the first and second field frm the reader'
                    Dim test As String = r.GetString(0)
                    Dim test2 As Integer = r.GetInt32(1)
                    'lblAddress.Text = r.GetString(1)
                End If
            End Using
        End Using
    End Sub

    Private Function GetConnection() As SqlClient.SqlConnection
        Dim cc As New SqlClient.SqlConnection
        cc.ConnectionString = "Server =MIFCOM\SQLEXPRESS;Database=GISTEST;User Id=sgislkof;Password =lkof4;"
        Return cc
    End Function

End Module


'USE [GISTest]
'GO
'/****** Object:  StoredProcedure [dbo].[spGetAllBearbeiter]    Script Date: 18.04.2019 07:30:22 ******/
'Set ANSI_NULLS On
'GO
'Set QUOTED_IDENTIFIER On
'GO
'ALTER procedure [dbo].[spGetAllBearbeiter]
'@NACHNAME varchar(45) 

'as
'begin
'Set nocount On;
'Select Case VORNAME,BEARBEITERID from BEARBEITER
'where NACHNAME = @NACHNAME

'End

'exec dbo.spGetAllBearbeiter
