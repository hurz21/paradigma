Imports System.Data

Public Class clsEigentuemerschnell
    Public Property eigentschnellKonn As New Devart.Common.DbConnectionStringBuilder
    Public Property EigentuemerSchnellDB As OracleConnection = New OracleConnection()

    Public Sub oeffneConnectionEigentuemer()
        Try
            EigentuemerSchnellDB = New OracleConnection("Data Source=(DESCRIPTION=" &
                                   "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" & CType(initP.getValue("EigentuemerSchnellDB.MySQLServer"), String) &
                                   ")(PORT=1521)))" &
                                   "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=" & CType(initP.getValue("EigentuemerSchnellDB.ServiceName"), String) &
                                   ")));" &
                                   "User Id=" & CType(initP.getValue("EigentuemerSchnellDB.username"), String) &
                                   ";Password=" & CType(initP.getValue("EigentuemerSchnellDB.password"), String) &
                                   ";direct=yes;")

            EigentuemerSchnellDB.Open()
        Catch ex As Exception
            nachricht("Fehler in oeffneConnectionEigentuemer: ", ex)
        End Try
    End Sub
    Public Function getEigentuemerdataALT(ByVal fs As String,
                                        ByRef kurzinfo As String,
                                        ByRef nameundadresse As String,
                                        ByRef _mycount As Integer,
                                        ByRef dt As DataTable) As Boolean
        Dim com As OracleCommand
        Try
            com = New OracleCommand("select * from fs2eigentuemer where fs='" & fs & "'", EigentuemerSchnellDB)
            Dim da As New OracleDataAdapter(com)
            'da.MissingSchemaAction = MissingSchemaAction.AddWithKey
            dt = New DataTable
            _mycount = da.Fill(dt)
            If _mycount < 1 Then
                Return False
            End If
            kurzinfo = CStr(clsDBtools.fieldvalue(dt.Rows(0).Item("TOOLTIP")))
            nameundadresse = CStr(clsDBtools.fieldvalue(dt.Rows(0).Item("NAMENADRESSEN")))
            Return True
        Catch ex As Exception
            nachricht("fehler in getEigentuemerdata: ", ex)
            Return False
        End Try
    End Function

    Public Function getEigentuemerdata(ByVal fs As String,
                                      ByRef kurzinfo As String,
                                      ByRef nameundadresse As String,
                                      ByRef _mycount As Integer,
                                      ByRef dt As DataTable) As Boolean
        Dim hinweis As String = ""
        myGlobalz.sitzung.tempREC.mydb.SQL = "select * from paradigma.dbo.alkis_fs2eigentuemer where fs='" & fs & "'"
        myGlobalz.sitzung.tempREC.dt = getDT4Query(myGlobalz.sitzung.tempREC.mydb.SQL, myGlobalz.sitzung.tempREC, hinweis)
        _mycount = myGlobalz.sitzung.tempREC.dt.Rows.Count
        If _mycount < 1 Then
            Return False
        End If
        kurzinfo = CStr(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("TOOLTIP")))
        nameundadresse = CStr(clsDBtools.fieldvalue(myGlobalz.sitzung.tempREC.dt.Rows(0).Item("NAMENADRESSEN")))
        Return True
    End Function


End Class
