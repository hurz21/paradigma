#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB.LIBDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
'Imports Devart.Data.Oracle
#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports System.Data

Public Class clsVorgangDB_Oracle

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
    Public MeineDBConnection As New OracleConnection
    Sub New(ByVal conn As System.Data.Common.DbConnection)
        MeineDBConnection = CType(conn, OracleConnection)
    End Sub
    Shared Function setSQLbody() As String
        Return " set " &
         " AZ=:AZ" &
         ",SACHGEBIETNR=:SACHGEBIETNR" &
         ",VORGANGSNR=:VORGANGSNR " &
         ",VORGANGSGEGENSTAND=:VORGANGSGEGENSTAND " &
         ",SACHGEBIETSTEXT=:SACHGEBIETSTEXT " &
         ",ISTUNB=:ISTUNB "
    End Function
    Shared Function setSQLParams(ByVal com As OracleCommand, ByVal vid As Integer) As Boolean
        com.Parameters.AddWithValue(":AZ", myglobalz.sitzung.aktVorgang.Stammdaten.az.gesamt)
        com.Parameters.AddWithValue(":SACHGEBIETNR", myglobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl)
        com.Parameters.AddWithValue(":VORGANGSNR", myglobalz.sitzung.aktVorgang.Stammdaten.az.Vorgangsnummer)
        com.Parameters.AddWithValue(":VORGANGSGEGENSTAND", myglobalz.sitzung.aktVorgang.Stammdaten.az.Prosa)
        com.Parameters.AddWithValue(":SACHGEBIETSTEXT", myglobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header)
        com.Parameters.AddWithValue(":ISTUNB", CBool(myglobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.isUNB()))
        '  com.Parameters.AddWithValue(":VORGANGSID", vid)
        Return True
    End Function

    'Public Function Edit_speichern_Vorgang(ByVal vid As Integer) As Boolean   'myGlobalz.sitzung.VorgangsID
    '    Dim anzahlTreffer& = 0, hinweis$ = ""
    '    Dim com As OracleCommand
    '    'todo Vorgangsnr: muss im Betrieb auf integer umgestellt werden
    '    Try
    '        If vid% < 1 Then
    '            nachricht_und_Mbox("FEHLER: Edit_speichern_Vorgang updateid =0. Abbruch")
    '            Return False
    '        End If
    '        myglobalz.sitzung.VorgangREC.mydb.Tabelle ="Vorgang"
    '        If myglobalz.sitzung.aktVorgang.Stammdaten.az.anychange Then
    '            myglobalz.sitzung.VorgangREC.mydb.SQL =
    '             "UPDATE  " & myglobalz.sitzung.VorgangREC.mydb.Tabelle &
    '             setSQLbody() &
    '             "  WHERE VORGANGSID=:VORGANGSID" '& MYGLOBALZ.SITZUNG.VORGANGSID
    '            MeineDBConnection.Open()
    '            com = New OracleCommand(myglobalz.sitzung.VorgangREC.mydb.SQL, MeineDBConnection)
    '            setSQLParams(com, vid)
    '            com.Parameters.AddWithValue(":VORGANGSID", vid)

    '            anzahlTreffer& = CInt(com.ExecuteNonQuery)
    '            MeineDBConnection.Close()
    '        Else
    '            Return True 'keine änderungen vorhanden
    '        End If
    '        'anzahlTreffer = myGlobalz.sitzung.VorgangREC.sqlexecute(newid, myGlobalz.mylog)
    '        If anzahlTreffer < 1 Then
    '            nachricht_und_Mbox("Problem beim Abspeichern:" & myglobalz.sitzung.VorgangREC.mydb.SQL)
    '            Return False
    '        Else
    '            nachricht("Vorgang edit erefolgreich!")
    '            Return True
    '        End If
    '    Catch ex As Exception
    '        nachricht_und_Mbox("Fehler beim Abspeichern: " ,ex)
    '        Return False
    '    End Try
    'End Function

    'Public Function Neu_speichern_Vorgang() As Boolean
    '    'hier wird die vorgangsnummer angelegt
    '    Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
    '    Dim com As OracleCommand
    '    'todo Vorgangsnr: muss im Betrieb auf integer umgestellt werden
    '    Try
    '        myglobalz.sitzung.VorgangREC.mydb.Tabelle ="Vorgang"

    '        Dim SQLupdate$ =
    '          String.Format("INSERT INTO {0} (AZ,SACHGEBIETNR,VORGANGSNR,VORGANGSGEGENSTAND,SACHGEBIETSTEXT,ISTUNB) " +
    '                                " VALUES (:AZ,:SACHGEBIETNR,:VORGANGSNR,:VORGANGSGEGENSTAND,:SACHGEBIETSTEXT,:ISTUNB)",
    '                                  myglobalz.sitzung.VorgangREC.mydb.Tabelle)
    '        SQLupdate$ = SQLupdate$ & " RETURNING VORGANGSID INTO :R1"

    '        nachricht("nach setSQLbody : " & SQLupdate)
    '        MeineDBConnection.Open()
    '        nachricht("nach dboeffnen  ")

    '        com = New OracleCommand(SQLupdate$, MeineDBConnection)
    '        nachricht("vor setParams  ")
    '        setSQLParams(com, 0)


    '        newid = clsOracleIns.GetNewid(com, SQLupdate)
    '        MeineDBConnection.Close()



    '        If newid < 1 Then
    '            nachricht_und_Mbox(String.Format("Problem beim abspeichern:{0}", myglobalz.sitzung.VorgangREC.mydb.SQL))
    '            Return False
    '        Else
    '            myglobalz.sitzung.aktVorgangsID = CInt(newid)
    '            nachricht("Neue Paradigmanr: " & newid)
    '            Return True
    '        End If
    '    Catch ex As Exception
    '        nachricht_und_Mbox("Problem beim Abspeichern des Vorgangs: " & vbCrLf ,ex)
    '        Return False
    '    End Try
    'End Function



    Public Shared Function leseAktenzeichen(ByVal vorgangsid%, ByVal dbrec As IDB_grundfunktionen) As Boolean 'myGlobalz.sitzung.VorgangsID	 ,myGlobalz.sitzung.VorgangREC
        'dbrec.mydb.Tabelle ="Vorgang"
        'Return DB_Oracle_sharedfunctions.getDT_("",vorgangsid, dbrec)
        dbrec.mydb.SQL = "select * from " & CLstart.myViewsNTabs.tabVorgang & " where vorgangsid=" & vorgangsid
        Dim hinweis As String = dbrec.getDataDT()
        If dbrec.dt.IsNothingOrEmpty Then
            l("Fatal Error ID " & "konnte nicht gefunden werden!" & dbrec.mydb.getDBinfo(""))
            Return False
        Else
            Return True
        End If
    End Function

    'Public Shared Sub initvorgangsDT_by_SQLstring(ByVal sql$)
    '    myGlobalz.sitzung.VorgangREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
    '    myGlobalz.sitzung.VorgangREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
    '    myGlobalz.sitzung.VorgangREC.mydb.Tabelle ="vorgang"
    '    myGlobalz.sitzung.VorgangREC.mydb.SQL = sql$
    '    nachricht(myGlobalz.sitzung.VorgangREC.getDataDT())
    '    If myGlobalz.sitzung.VorgangREC.mycount < 1 Then
    '        nachricht("Keine raumbezugsRec gespeichert!")
    '    Else
    '        nachricht(String.Format("{0} VorgangREC vorhanden", myGlobalz.sitzung.VorgangREC.mycount))
    '    End If
    'End Sub



    Public Function holeNeueVorgangsNummer(ByVal sachgebiet As String) As Integer
        Dim maxnr As Long?
        myGlobalz.sitzung.VorgangREC.mydb.SQL =
         String.Format("SELECT  max(v.vorgangsnr)" +
         " FROM " & CLstart.myViewsNTabs.tabVorgang & " v, " & CLstart.myViewsNTabs.tabStammdaten & " s  " +
         " where v.Sachgebietnr='{0}'" +
         " and s.VorgangsID=v.vorgangsid ", sachgebiet$)
        Dim result$ = ""
        Dim com As New OracleCommand()
        MeineDBConnection.Close()
        com.Connection = MeineDBConnection 'myGlobalz.sitzung.VorgangREC.myconn
        com.CommandText = myglobalz.sitzung.VorgangREC.mydb.SQL
        Try
            maxnr = CLng(clsDBtools.fieldvalue(com.ExecuteScalar))
            com.Dispose()
            Return CInt(maxnr + 1)
        Catch mex As OracleException
            Return 1
            nachricht_und_Mbox(mex.ToString)
        Catch ex As Exception
            'com.Dispose()
            Return 1
            nachricht_und_Mbox(ex.ToString)
        Finally
            MeineDBConnection.Close()
        End Try
    End Function


End Class
