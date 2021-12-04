Imports Microsoft.SqlServer
Imports System.Data
Imports System.Data.SqlClient

Public Class clsSqlparam
    Public Shared paramListe As New List(Of clsSqlparam)
    Property name As String
    Property obj As Object
    Sub New(_name As String, _obj As Object)
        name = _name
        obj = _obj
    End Sub
    Shared Sub korrigiereParam(dbtyp As String, paramliste As List(Of clsSqlparam))
        For i = 0 To paramliste.Count - 1
            If dbtyp = "oracle" Then paramliste(i).name = ":" & paramliste(i).name.ToUpper
            If dbtyp = "sqls" Then paramliste(i).name = "@" & paramliste(i).name.ToUpper
            If dbtyp = "mysql" Then paramliste(i).name = "@" & paramliste(i).name
            If dbtyp = "postgres" Then paramliste(i).name = ":" & paramliste(i).name
        Next
    End Sub
End Class

Public Class clsMSSQL
    Sub l(t As String)

    End Sub
    Sub nachricht(t As String)

    End Sub
    Public Function manipquerie(querie As String,
                        slqparamlist As List(Of clsSqlparam),
                        ReturnIdentity As Boolean, returnColumn As String) As Integer
        l("Neu_speichern_sqls manip-----------------------------------------------------")
        Dim anzahlTreffer& = 0, newid& = 0
        Dim com As SqlCommand : Dim myconn As New SqlConnection
        'Dim DBDA As SqlDataAdapter
        'Dim DBDT As DataTable
        Try
            querie = querie.ToUpper
            If ReturnIdentity Then
                querie = querie & ";SELECT CAST(scope_identity() AS int);"
            End If
            l("nach setSQLbody : " & querie)
            clsSqlparam.korrigiereParam("sqls", slqparamlist)
            'Dim retcode = dboeffnen(hinweis)
            myconn = getMSSQLCon("msql01", "Paradigma", "sgis", "WinterErschranzt.74")
            myconn.Open()
            l("nach dboeffnen  ")
            com = New SqlCommand(querie, myconn)
            nachricht("vor setParams  ")
            Dim dval As Object
            Dim strDval As String
            For i = 0 To slqparamlist.Count - 1
                dval = slqparamlist(i).obj
                strDval = dval.ToString
                If slqparamlist(i).obj.ToString.StartsWith("1754-01-01") Or
                    slqparamlist(i).obj.ToString.StartsWith("01.01.1754") Or
                    strDval.StartsWith("01.01.0001") Then
                    dval = DBNull.Value
                End If
                com.Parameters.AddWithValue(slqparamlist(i).name, dval)
            Next
            slqparamlist.Clear()
            If ReturnIdentity Then
                Dim kobjssss = com.ExecuteScalar
                If kobjssss Is Nothing Then
                    newid = 0
                Else
                    newid = CLng(kobjssss.ToString)
                End If
                myconn.Close()
                If newid < 1 Then
                    l("Problem beim Abspeichern : " & querie)
                    Return -1
                Else
                    Return CInt(newid)
                End If
            Else
                anzahlTreffer& = CInt(com.ExecuteNonQuery) 'ret number of affe rows
                myconn.Close()
                If anzahlTreffer < 1 Then
                    nachricht("Problem beim Abspeichern:" & querie)
                    Return -1
                Else
                    Return CInt(anzahlTreffer)
                End If
            End If
        Catch ex As Exception
            l("Fehler beim Abspeichern: " & ex.ToString)
            Return -2
        End Try
    End Function
    Function getDT(sql As String, myoracle As SqlClient.SqlConnection) As DataTable
        Dim dt As New DataTable
        Try
            Dim com As New SqlClient.SqlCommand(sql, myoracle)
            Dim da As New SqlClient.SqlDataAdapter(com)
            Dim mycount As Integer
            'MsgBox("vorfill")
            'myoracle.Open()
            mycount = da.Fill(dt)
            com.Dispose()
            da.Dispose()
            myoracle.Close()
            myoracle.Dispose()
            Return dt
        Catch oex As SqlClient.SqlException
            '  nachricht("Fehler in GetNewid&:" & oex.ToString & " / " & sql)
            MsgBox(oex.ToString)
            Return Nothing
        Catch ex As Exception
            '  nachricht("Fehler in GetNewid&:" & ex.ToString & " / " & sql)
            MsgBox(ex.ToString)
            Return Nothing
        Finally
            myoracle.Close()
        End Try
    End Function
    Function getMSSQLCon(host As String, schema As String, user As String, pw As String) As SqlClient.SqlConnection
        Dim myoracle As SqlClient.SqlConnection
        'Dim host, datenbank, schema, tabelle, dbuser, dbpw, dbport As String
        'host = "msql01" : schema = "Paradigma" : dbuser = "sgis" : dbpw = " WinterErschranzt.74"
        Dim conbuil As New SqlClient.SqlConnectionStringBuilder
        Dim v = "Data Source=" & host & ";User ID=" & user & ";Password=" & pw & ";" +
                "Initial Catalog=" & schema & ";"
        myoracle = New SqlClient.SqlConnection(v)
        Return myoracle
    End Function
End Class
