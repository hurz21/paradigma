Imports MySql.Data.MySqlClient
Imports System.Data

Public Class clsVorgangDB_Mysql
    Implements IDisposable
#Region "IDisposable Support"
    Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe
    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                MeineDBConnection.Dispose()
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
        Return " set " &
         " AZ=@AZ" &
         ",Sachgebietnr=@Sachgebietnr" &
         ",Vorgangsnr=@Vorgangsnr " &
         ",Vorgangsgegenstand=@Vorgangsgegenstand " &
         ",Sachgebietstext=@Sachgebietstext " &
         ",istUNB=@istUNB "
    End Function
    Shared Function setSQLParams(ByVal com As MySqlCommand, ByVal vid As Integer) As Boolean
        com.Parameters.AddWithValue("@AZ", myglobalz.sitzung.aktVorgang.Stammdaten.az.gesamt)
        com.Parameters.AddWithValue("@Sachgebietnr", myglobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl)
        com.Parameters.AddWithValue("@Vorgangsnr", myglobalz.sitzung.aktVorgang.Stammdaten.az.Vorgangsnummer)
        com.Parameters.AddWithValue("@Vorgangsgegenstand", myglobalz.sitzung.aktVorgang.Stammdaten.az.Prosa)
        com.Parameters.AddWithValue("@Sachgebietstext", myglobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header)
        com.Parameters.AddWithValue("@istUNB", CBool(myglobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.isUNB()))
        com.Parameters.AddWithValue("@VorgangsID", vid)
        Return True
    End Function

    Public Function Edit_speichern_Vorgang(ByVal vid As Integer) As Boolean   'myGlobalz.sitzung.VorgangsID
        Dim anzahlTreffer& = 0, hinweis$ = ""
        Dim com As MySqlCommand
        'todo Vorgangsnr: muss im Betrieb auf integer umgestellt werden
        Try
            If vid% < 1 Then
                nachricht_und_Mbox("FEHLER: Edit_speichern_Vorgang updateid =0. Abbruch")
                Return False
            End If
            'myglobalz.sitzung.VorgangREC.mydb.Tabelle ="Vorgang"
            If myglobalz.sitzung.aktVorgang.Stammdaten.az.anychange Then
                myGlobalz.sitzung.VorgangREC.mydb.SQL =
                 "update " & CLstart.myViewsNTabs.tabVorgang & " " & setSQLbody() & "  where VorgangsID=@VorgangsID" '& myGlobalz.sitzung.VorgangsID
                myGlobalz.sitzung.VorgangREC.dboeffnen(hinweis$)
                com = New MySqlCommand(myglobalz.sitzung.VorgangREC.mydb.SQL, MeineDBConnection)
                setSQLParams(com, vid)

                anzahlTreffer& = CInt(com.ExecuteNonQuery)
                myglobalz.sitzung.VorgangREC.dbschliessen(hinweis$)
            Else
                Return True 'keine änderungen vorhanden
            End If
            'anzahlTreffer = myGlobalz.sitzung.VorgangREC.sqlexecute(newid, myGlobalz.mylog)
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichern:" & myglobalz.sitzung.VorgangREC.mydb.SQL)
                Return False
            Else
                nachricht("Vorgang edit erefolgreich!")
                Return True
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Abspeichern: " ,ex)
            Return False
        End Try
    End Function

    Public Function Neu_speichern_Vorgang() As Boolean
        'hier wird die vorgangsnummer angelegt
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As MySqlCommand
        'todo Vorgangsnr: muss im Betrieb auf integer umgestellt werden
        Try
            'myglobalz.sitzung.VorgangREC.mydb.Tabelle ="Vorgang"
            myGlobalz.sitzung.VorgangREC.mydb.SQL =
             String.Format("insert into " & CLstart.myViewsNTabs.tabVorgang & " {0}", setSQLbody())

            myGlobalz.sitzung.VorgangREC.dboeffnen(hinweis$)
            com = New MySqlCommand(myglobalz.sitzung.VorgangREC.mydb.SQL, MeineDBConnection)
            setSQLParams(com, 0)
            anzahlTreffer& = CInt(com.ExecuteNonQuery)
            com.CommandText = "Select LAST_INSERT_ID()"
            newid = CLng(com.ExecuteScalar)
            myglobalz.sitzung.VorgangREC.dbschliessen(hinweis$)
            'anzahlTreffer = myGlobalz.sitzung.VorgangREC.sqlexecute(newid, myGlobalz.mylog)
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox(String.Format("Problem beim abspeichern:{0}", myglobalz.sitzung.VorgangREC.mydb.SQL))
                Return False
            Else
                myglobalz.sitzung.aktVorgangsID = CInt(newid)
                nachricht("Neue Paradigmanr: " & newid)
                Return True
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Problem beim Abspeichern des Vorgangs: " & vbCrLf ,ex)
            Return False
        End Try
    End Function


    Public Function holeNeueVorgangsNummer(ByVal sachgebiet As String) As Integer
        Dim maxnr As Long?
        myGlobalz.sitzung.VorgangREC.mydb.SQL =
         String.Format("SELECT  max(v.vorgangsnr)" +
                             " FROM " & CLstart.myViewsNTabs.tabVorgang & " v, " & CLstart.myViewsNTabs.tabStammdaten & " s  " +
                             " where v.Sachgebietnr='{0}'" +
                             " and s.VorgangsID=v.vorgangsid ", sachgebiet$)
        Dim result$ = ""
        Dim com As New MySqlCommand()
        nachricht("Retcode: " & myglobalz.sitzung.VorgangREC.dboeffnen(result$).ToString)
        com.Connection = MeineDBConnection 'myGlobalz.sitzung.VorgangREC.myconn
        com.CommandText = myglobalz.sitzung.VorgangREC.mydb.SQL
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

End Class
