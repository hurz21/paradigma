Imports MySql.Data.MySqlClient
Imports System.Data

Public Class clsVorgangDB_Mysql
      Implements IDisposable
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
    Public MeineDBConnection As New MySqlConnection
    Sub New(ByVal conn As System.Data.Common.DbConnection)
        MeineDBConnection = CType(conn, MySqlConnection)
    End Sub

    Shared Function setSQLbody() As String
        Return " set " & _
         " AZ=@AZ" & _
         ",Sachgebietnr=@Sachgebietnr" & _
         ",Vorgangsnr=@Vorgangsnr " & _
         ",Vorgangsgegenstand=@Vorgangsgegenstand " & _
         ",Sachgebietstext=@Sachgebietstext " & _
         ",istUNB=@istUNB "
    End Function
    Shared Function setSQLParams(ByVal com As MySqlCommand, ByVal vid as integer) as  Boolean
        com.Parameters.AddWithValue("@AZ", myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt)
        com.Parameters.AddWithValue("@Sachgebietnr", myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl)
        com.Parameters.AddWithValue("@Vorgangsnr", myGlobalz.sitzung.aktVorgang.Stammdaten.az.Vorgangsnummer)
        com.Parameters.AddWithValue("@Vorgangsgegenstand", myGlobalz.sitzung.aktVorgang.Stammdaten.az.Prosa)
        com.Parameters.AddWithValue("@Sachgebietstext", myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header)
        com.Parameters.AddWithValue("@istUNB", CBool(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.isUNB()))
        com.Parameters.AddWithValue("@VorgangsID", vid)
    End Function

    'Public Function Edit_speichern_Vorgang(ByVal vid as integer) as  Boolean   'myGlobalz.sitzung.VorgangsID
    '    Dim anzahlTreffer& = 0, hinweis$ = ""
    '    Dim com As MySqlCommand
    '    'todo Vorgangsnr: muss im Betrieb auf integer umgestellt werden
    '    Try
    '        If vid% < 1 Then
    '            nachricht_und_Mbox("FEHLER: Edit_speichern_Vorgang updateid =0. Abbruch")
    '            Return False
    '        End If
    '        myGlobalz.sitzung.VorgangREC.mydb.Tabelle ="Vorgang"
    '        If myGlobalz.sitzung.aktVorgang.Stammdaten.az.anychange Then
    '            myGlobalz.sitzung.VorgangREC.mydb.SQL = _
    '             "update  " & myGlobalz.sitzung.VorgangREC.mydb.Tabelle & _
    '             setSQLbody() & _
    '             "  where VorgangsID=@VorgangsID" '& myGlobalz.sitzung.VorgangsID
    '            myGlobalz.sitzung.VorgangREC.dboeffnen(hinweis$)
    '            com = New MySqlCommand(myGlobalz.sitzung.VorgangREC.mydb.SQL, MeineDBConnection)
    '            setSQLParams(com, vid)

    '            anzahlTreffer& = CInt(com.ExecuteNonQuery)
    '            myGlobalz.sitzung.VorgangREC.dbschliessen(hinweis$)
    '        Else
    '            Return True 'keine änderungen vorhanden
    '        End If
    '        'anzahlTreffer = myGlobalz.sitzung.VorgangREC.sqlexecute(newid, myGlobalz.mylog)
    '        If anzahlTreffer < 1 Then
    '            nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.VorgangREC.mydb.SQL)
    '            Return False
    '        Else
    '            nachricht("Vorgang edit erefolgreich!")
    '            Return True
    '        End If
    '    Catch ex As Exception
    '        nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
    '        Return False
    '    End Try
    'End Function

    Public Function Neu_speichern_Vorgang() As Boolean
        'hier wird die vorgangsnummer angelegt
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As MySqlCommand
        'todo Vorgangsnr: muss im Betrieb auf integer umgestellt werden
        Try
            'myGlobalz.sitzung.VorgangREC.mydb.Tabelle ="Vorgang"
            myGlobalz.sitzung.VorgangREC.mydb.SQL =
             String.Format("insert into " & CLstart.myViewsNTabs.tabVorgang & " {0}", setSQLbody())

            myGlobalz.sitzung.VorgangREC.dboeffnen(hinweis$)
            com = New MySqlCommand(myGlobalz.sitzung.VorgangREC.mydb.SQL, MeineDBConnection)
            setSQLParams(com, 0)
            anzahlTreffer& = CInt(com.ExecuteNonQuery)
            com.CommandText = "Select LAST_INSERT_ID()"
            newid = CLng(com.ExecuteScalar)
            myGlobalz.sitzung.VorgangREC.dbschliessen(hinweis$)
            'anzahlTreffer = myGlobalz.sitzung.VorgangREC.sqlexecute(newid, myGlobalz.mylog)
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox(String.Format("Problem beim abspeichern:{0}", myGlobalz.sitzung.VorgangREC.mydb.SQL))
                Return False
            Else
                myGlobalz.sitzung.aktVorgangsID = CInt(newid)
                nachricht("Neue Paradigmanr: " & newid)
                Return True
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Problem beim Abspeichern des Vorgangs: " & vbCrLf & ex.ToString)
            Return False
        End Try
    End Function




    'Public Shared Sub initvorgangsDT_by_SQLstring(ByVal sql$)
    '    myGlobalz.sitzung.VorgangREC.mydb.Tabelle ="vorgang"
    '    myGlobalz.sitzung.VorgangREC.mydb.SQL = sql$
    '    nachricht(myGlobalz.sitzung.VorgangREC.getDataDT())
    '    If myGlobalz.sitzung.VorgangREC.mycount < 1 Then
    '        nachricht("Keine initvorgangsDT_by_SQLstring gespeichert!")
    '    Else
    '        nachricht(String.Format("{0} VorgangREC vorhanden", myGlobalz.sitzung.VorgangREC.mycount))
    '    End If
    'End Sub

    Public Function Delete_Vorgang(ByVal vid as integer) as  Boolean   'myGlobalz.sitzung.VorgangsID
        Dim anzahlTreffer& = 0, hinweis$ = ""
        Dim com As MySqlCommand
        'todo Vorgangsnr: muss im Betrieb auf integer umgestellt werden
        Try
            If vid% < 1 Then
                nachricht_und_Mbox("FEHLER: Delete_Vorgang updateid =0. Abbruch")
                Return False
            End If
            'myGlobalz.sitzung.VorgangREC.mydb.Tabelle ="Vorgang"

            myGlobalz.sitzung.VorgangREC.mydb.SQL =
             "delete from " & CLstart.myViewsNTabs.tabVorgang &
             "  where VorgangsID=@VorgangsID" '& myGlobalz.sitzung.VorgangsID
            myGlobalz.sitzung.VorgangREC.dboeffnen(hinweis$)
            com = New MySqlCommand(myGlobalz.sitzung.VorgangREC.mydb.SQL, MeineDBConnection)
            setSQLParams(com, vid)

            anzahlTreffer& = CInt(com.ExecuteNonQuery)
            myGlobalz.sitzung.VorgangREC.dbschliessen(hinweis$)

            'anzahlTreffer = myGlobalz.sitzung.VorgangREC.sqlexecute(newid, myGlobalz.mylog)
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.VorgangREC.mydb.SQL)
                Return False
            Else
                nachricht("Vorgang edit erefolgreich!")
                Return True
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
            Return False
        End Try
    End Function

    Public Function holeNeueVorgangsNummer(ByVal sachgebiet as string) as  Integer
        Dim maxnr As Long?
        myGlobalz.sitzung.VorgangREC.mydb.SQL =
         String.Format("SELECT  max(v.vorgangsnr)" +
         " FROM " & CLstart.myViewsNTabs.tabVorgang & " v, " & CLstart.myViewsNTabs.tabStammdaten & " s  " +
         " where v.Sachgebietnr='{0}'" +
         " and s.VorgangsID=v.vorgangsid ", sachgebiet$)
        Dim result$ = ""
        Dim com As New MySqlCommand()
        nachricht("Retcode: " & myGlobalz.sitzung.VorgangREC.dboeffnen(result$).ToString)
        com.Connection = MeineDBConnection 'myGlobalz.sitzung.VorgangREC.myconn
        com.CommandText = myGlobalz.sitzung.VorgangREC.mydb.SQL
        Try
            maxnr = CLng(clsDBtools.fieldvalue(com.ExecuteScalar))
            com.Dispose()
            Return CInt(maxnr + 1)
        Catch mex As MySqlException
            Return 1
            nachricht_und_Mbox(mex.ToString)
        Catch ex As Exception
            'com.Dispose()
            Return 1
            nachricht_und_Mbox(ex.ToString)
        End Try
    End Function
    ''' <summary>
    ''' Das ergebnis liegt auf der Datatable myGlobalz.sitzung.VorgangREC.dt
    ''' </summary>
    ''' <param name="SQL">Vollständiges SQL statement</param>
    ''' <param name="Tabelle">Die tabelle muss auch im sql statement enthalten sein</param>
    ''' <remarks>return anzahl der treffer</remarks>
    Public Function selectFromParadigmaTabelle(ByVal SQL$, ByVal Tabelle$, ByRef resultDT As DataTable) As Integer
        Try
            myGlobalz.sitzung.VorgangREC.mydb.Tabelle = Tabelle$
            myGlobalz.sitzung.VorgangREC.mydb.SQL = SQL$
            nachricht(myGlobalz.sitzung.VorgangREC.getDataDT())
            If myGlobalz.sitzung.VorgangREC.mycount < 1 Then
                nachricht("Keine selectFromParadigmaTabelle gespeichert!")
                resultDT = myGlobalz.sitzung.VorgangREC.dt.Copy
            Else
                nachricht(String.Format("{0} selectFromParadigmaTabelle vorhanden", myGlobalz.sitzung.VorgangREC.mycount))
                resultDT = Nothing
            End If
            Return CInt(myGlobalz.sitzung.VorgangREC.mycount)
        Catch ex As Exception
            nachricht("Fehler in   nachricht: " & ex.ToString)
            Return -1
        End Try
    End Function
End Class
