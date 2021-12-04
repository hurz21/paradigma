Imports System.Data
Imports System.Data.Common


Public Interface IDB_grundfunktionen

    ' Property myconn As Object

    Function dboeffnen(ByRef resultstring$) As Integer
    Function dbschliessen(ByRef resultstring$) As Integer
    Function doConnection(ByRef hinweis$) As Boolean ' As System.Data.Common.DbConnection
    Function getDataDT() As String
    Function sqlexecute(ByRef newID&) As Long
    Property mycount() As Long
    Property dt() As DataTable
    Property mydb() As clsDatenbankZugriff
    '  Property myconn() As DbConnection
End Interface
