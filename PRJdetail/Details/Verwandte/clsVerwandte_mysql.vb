﻿Imports MySql.Data.MySqlClient

Public Class clsVerwandte_mysql
       Implements IDisposable
 
    Public MeineDBConnection As New MySqlConnection
    Sub New(ByVal conn As System.Data.Common.DbConnection)
        MeineDBConnection = CType(conn, MySqlConnection)
    End Sub
    'Public Function Koppelung_Vorgang_Fremdvorgang(ByVal vorgangID%, ByVal fremdvorgangsid%, ByVal Titel as string) as  Integer
    '    Dim anzahlTreffer&
    '    Dim newid& = -1
    '    Try
    '        myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, clsDBspecMYSQL)
    '        myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
    '        myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
    '        myGlobalz.sitzung.tempREC.mydb.Tabelle ="vorgang2fremdvorgang"
    '        myGlobalz.sitzung.tempREC.mydb.SQL = _
    '         "insert into " & myGlobalz.sitzung.tempREC.mydb.Tabelle & " set " & _
    '         " Vorgangsid=" & vorgangID% & _
    '         ",fremdvorgangsid=" & fremdvorgangsid% & _
    '         ",Titel='" & Titel & "'"
    '        anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ', myGlobalz.mylog)
    '        If anzahlTreffer < 1 Then
    '            nachricht_und_Mbox("Problem beim Koppelung_Vorgang_Fremdvorgang:" & myGlobalz.sitzung.tempREC.mydb.SQL)
    '            Return -1
    '        Else
    '            Return CInt(newid)
    '        End If
    '    Catch ex As Exception
    '        nachricht_und_Mbox("Problem beim Koppelung_Vorgang_Fremdvorgang: " ,ex)
    '        Return -2
    '    End Try
    'End Function

    'Public Function erzeugeVerwandtenlistezuVorgang(ByVal sql as string) as  Boolean
    '    dim hinweis as string 
    '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="vorgang2fremdvorgang"
    '    myGlobalz.sitzung.tempREC.mydb.SQL = sql$
    '    hinweis = myGlobalz.sitzung.tempREC.getDataDT()
    '    If myGlobalz.sitzung.tempREC.mycount < 1 Then
    '        nachricht("Keine beteiligte gespeichert!")
    '        Return False
    '    Else
    '        nachricht(String.Format("{0} beteiligte vorhanden", myGlobalz.sitzung.tempREC.mycount))
    '        Return True
    '    End If
    'End Function



#Region "IDisposable Support"
    Private disposedValue As Boolean' So ermitteln Sie überflüssige Aufrufe
    Protected     Overridable     Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                MeineDBConnection.Dispose
            End If
        End If
        Me.disposedValue = True
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
