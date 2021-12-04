Imports MySql.Data.MySqlClient
Public Class clsUmkreisDB_Mysql
        Implements IDisposable
    Public MeineDBConnection As New MySqlConnection
    Sub New(ByVal conn As System.Data.Common.DbConnection)
        MeineDBConnection = CType(conn, MySqlConnection)
    End Sub


    Public Function RB_Umkreis_abspeichern_Neu() As Integer
        'Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        'Dim com As MySqlCommand
        'Try
        '    glob2.initTemprecAusVorgangRecMysql()
        '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="Paraumkreis"
        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
        '         "insert into " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
        '     setsqlbodyUmkreisRB()

        '    myGlobalz.sitzung.tempREC.dboeffnen(hinweis$)
        '    com = New MySqlCommand(myGlobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection)
        '    setSQLparamsUmkreisRB(com, 0)

        '    anzahlTreffer& = CInt(com.ExecuteNonQuery)
        '    com.CommandText = "Select LAST_INSERT_ID()"
        '    newid = CLng(com.ExecuteScalar)
        '    myGlobalz.sitzung.tempREC.dbschliessen(hinweis$)

        '    'anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid)
        '    If anzahlTreffer < 1 Then
        '        nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
        '        Return -1
        '    Else
        '        Return CInt(newid)
        '    End If
        'Catch ex As Exception
        '    nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
        '    Return -2
        'End Try
    End Function

    Shared Function setsqlbodyUmkreisRB() As String
        Return " set " & _
         " radiusM=@radiusM" & _
         ",Beschreibung=@Beschreibung"
    End Function

    Shared Sub setSQLparamsUmkreisRB(ByVal com As MySqlCommand, ByVal sekid%)
        '	com = New MySqlCommand(myGlobalz.sitzung.tempREC.mydb.SQL, myGlobalz.sitzung.tempREC.myconn)
        com.Parameters.AddWithValue("@radiusM", myGlobalz.sitzung.aktPMU.Radius)
        com.Parameters.AddWithValue("@Beschreibung", myGlobalz.sitzung.aktPMU.Name)
        com.Parameters.AddWithValue("@ID", sekid)
    End Sub
    Public Function RB_Umkreis_abspeichern_Edit(ByVal sekid as integer) as  Integer
        'Dim anzahlTreffer& = 0, hinweis$ = ""
        'Dim com As MySqlCommand
        'Try
        '    If sekid < 1 Then
        '        nachricht_und_Mbox("fehler updateid<1)")
        '        Return 0
        '    End If
        '    glob2.initTemprecAusVorgangRecMysql()
        '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="Paraumkreis"
        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
        '     "update " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
        '     setsqlbodyUmkreisRB() &
        '      " where id=@ID"

        '    myGlobalz.sitzung.tempREC.dboeffnen(hinweis$)
        '    com = New MySqlCommand(myGlobalz.sitzung.tempREC.mydb.SQL, MeineDBConnection)
        '    setSQLparamsUmkreisRB(com, sekid)

        '    anzahlTreffer& = CInt(com.ExecuteNonQuery)

        '    myGlobalz.sitzung.tempREC.dbschliessen(hinweis$)

        '    'anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid)
        '    If anzahlTreffer < 1 Then
        '        nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.tempREC.mydb.SQL)
        '        Return -1
        '    Else
        '        Return CInt(anzahlTreffer&)
        '    End If
        'Catch ex As Exception
        '    nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
        '    Return -2
        'End Try
    End Function
    Public Function RB_Umkreis_loeschen(ByVal umkreisid As Integer) As Integer
        'Dim anzahlTreffer&, newid&
        'Try
        '    glob2.initTemprecAusVorgangRecMysql()
        '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="Paraumkreis"
        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
        '     "delete from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
        '     " where id=" & umkreisid.ToString
        '    anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ')
        '    If anzahlTreffer < 1 Then
        '        nachricht_und_Mbox("Problem beim löschen:" & myGlobalz.sitzung.tempREC.mydb.SQL)
        '        Return -1
        '    Else
        '        Return CInt(anzahlTreffer)
        '    End If
        'Catch ex As Exception
        '    nachricht_und_Mbox("Problem beim löschen: " & ex.ToString)
        '    Return -2
        'End Try
    End Function
    Public Sub Umkreis_undVorgang_Entkoppeln(ByVal erfolg%)
        'erfolg = DBactionParadigma.Entkoppelung_Raumbezug_Vorgang(CInt(myGlobalz.sitzung.aktPMU.RaumbezugsID), myGlobalz.sitzung.VorgangsID)
        'erfolg = RBtoolsns.Entkoppelung_Raumbezug_Vorgang_alleDB.exe(CInt(myGlobalz.sitzung.aktPMU.RaumbezugsID), myGlobalz.sitzung.aktVorgangsID)
        'If erfolg > 0 Then
        '    My.Log.WriteEntry("Adresse wurde erfolgreich gelöscht")
        'Else
        '    My.Log.WriteEntry("Adresse wurde erfolgreich gelöscht")
        '    nachricht_und_Mbox("Problem beim Löschen des Raumbezugs aus dem Vorgang. Abbruch.")
        'End If
    End Sub

    Public Function RB_ParaUmkreis_holen(ByVal sekid as string) as  Boolean
        'dim hinweis as string 
        'Try
        '    glob2.initTemprecAusVorgangRecMysql()
        '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="paraumkreis"
        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
        '     "select * from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
        '     " where id=" & sekid$
        '    hinweis = myGlobalz.sitzung.tempREC.getDataDT()
        '    Return True
        'Catch ex As Exception
        '    nachricht_und_Mbox("Problem beim RB_Paraumkreis_holen: " & ex.ToString)
        '    Return False
        'End Try
    End Function
 
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
