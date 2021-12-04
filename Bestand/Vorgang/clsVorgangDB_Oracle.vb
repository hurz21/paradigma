'Imports System.Data.OracleClient
Imports LibDB
Imports System.Data

Public Class clsVorgangDB_Oracle
    
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
    Public MeineDBConnection As New OracleConnection
    Sub New(ByVal conn As System.Data.Common.DbConnection)
        MeineDBConnection = CType(conn, OracleConnection)
    End Sub
    Shared Function setSQLbody() As String
        Return " set " & _
         " AZ=:AZ" & _
         ",SACHGEBIETNR=:SACHGEBIETNR" & _
         ",VORGANGSNR=:VORGANGSNR " & _
         ",VORGANGSGEGENSTAND=:VORGANGSGEGENSTAND " & _
         ",SACHGEBIETSTEXT=:SACHGEBIETSTEXT " & _
         ",ISTUNB=:ISTUNB "
    End Function
    Shared Function setSQLParams(ByVal com As OracleCommand, ByVal vid As Integer) As Boolean
        com.Parameters.AddWithValue(":AZ", myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt)
        com.Parameters.AddWithValue(":SACHGEBIETNR", myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl)
        com.Parameters.AddWithValue(":VORGANGSNR", myGlobalz.sitzung.aktVorgang.Stammdaten.az.Vorgangsnummer)
        com.Parameters.AddWithValue(":VORGANGSGEGENSTAND", myGlobalz.sitzung.aktVorgang.Stammdaten.az.Prosa)
        com.Parameters.AddWithValue(":SACHGEBIETSTEXT", myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header)
        com.Parameters.AddWithValue(":ISTUNB", CBool(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.isUNB()))
        '  com.Parameters.AddWithValue(":VORGANGSID", vid)
    End Function

    Public Function Edit_speichern_Vorgang(ByVal vid As Integer) As Boolean   'myGlobalz.sitzung.VorgangsID
        Dim anzahlTreffer& = 0, hinweis$ = ""
        Dim com As OracleCommand
        'todo Vorgangsnr: muss im Betrieb auf integer umgestellt werden
        Try
            If vid% < 1 Then
                nachricht_und_Mbox("FEHLER: Edit_speichern_Vorgang updateid =0. Abbruch")
                Return False
            End If
            myGlobalz.sitzung.VorgangREC.mydb.Tabelle = "Vorgang"
            If myGlobalz.sitzung.aktVorgang.Stammdaten.az.anychange Then
                myGlobalz.sitzung.VorgangREC.mydb.SQL = _
                 "UPDATE  " & myGlobalz.sitzung.VorgangREC.mydb.Tabelle & _
                 setSQLbody() & _
                 "  WHERE VORGANGSID=:VORGANGSID" '& MYGLOBALZ.SITZUNG.VORGANGSID
                MeineDBConnection.Open()
                com = New OracleCommand(myGlobalz.sitzung.VorgangREC.mydb.SQL, MeineDBConnection)
                setSQLParams(com, vid)
                com.Parameters.AddWithValue(":VORGANGSID", vid)

                anzahlTreffer& = CInt(com.ExecuteNonQuery)
                MeineDBConnection.Close()
            Else
                Return True 'keine änderungen vorhanden
            End If
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

    Public Function Neu_speichern_Vorgang() As Boolean
        'hier wird die vorgangsnummer angelegt
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As OracleCommand
        'todo Vorgangsnr: muss im Betrieb auf integer umgestellt werden
        Try
            myGlobalz.sitzung.VorgangREC.mydb.Tabelle = "Vorgang"

            Dim SQLupdate$ =
              String.Format("INSERT INTO {0} (AZ,SACHGEBIETNR,VORGANGSNR,VORGANGSGEGENSTAND,SACHGEBIETSTEXT,ISTUNB) " +
                                    " VALUES (:AZ,:SACHGEBIETNR,:VORGANGSNR,:VORGANGSGEGENSTAND,:SACHGEBIETSTEXT,:ISTUNB)",
                                      myGlobalz.sitzung.VorgangREC.mydb.Tabelle)
            SQLupdate$ = SQLupdate$ & " RETURNING VORGANGSID INTO :R1"

            nachricht("nach setSQLbody : " & SQLupdate)
            MeineDBConnection.Open()
            nachricht("nach dboeffnen  ")

            com = New OracleCommand(SQLupdate$, MeineDBConnection)
            nachricht("vor setParams  ")
            setSQLParams(com, 0)


            newid = LIBoracle.clsOracleIns.GetNewid(com, SQLupdate)
            MeineDBConnection.Close()



            If newid < 1 Then
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



    Public Shared Function leseAktenzeichen(ByVal vorgangsid%, ByVal dbrec As IDB_grundfunktionen) As Boolean 'myGlobalz.sitzung.VorgangsID	 ,myGlobalz.sitzung.VorgangREC
        dbrec.mydb.Tabelle = "Vorgang"
        Return DB_Oracle_sharedfunctions.getDT_("", vorgangsid, dbrec)
    End Function

    'Public Shared Sub initvorgangsDT_by_SQLstring(ByVal sql$)
    '    myGlobalz.sitzung.VorgangREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
    '    myGlobalz.sitzung.VorgangREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
    '    myGlobalz.sitzung.VorgangREC.mydb.Tabelle = "vorgang"
    '    myGlobalz.sitzung.VorgangREC.mydb.SQL = sql$
    '    nachricht(myGlobalz.sitzung.VorgangREC.getDataDT())
    '    If myGlobalz.sitzung.VorgangREC.mycount < 1 Then
    '        nachricht("Keine raumbezugsRec gespeichert!")
    '    Else
    '        nachricht(String.Format("{0} VorgangREC vorhanden", myGlobalz.sitzung.VorgangREC.mycount))
    '    End If
    'End Sub

    Public Function Delete_Vorgang(ByVal vid As Integer) As Boolean   'myGlobalz.sitzung.VorgangsID
        Dim anzahlTreffer& = 0, hinweis$ = ""
        Dim com As OracleCommand
        'todo Vorgangsnr: muss im Betrieb auf integer umgestellt werden
        Try
            If vid% < 1 Then
                nachricht_und_Mbox("FEHLER:Delete_Vorgang  updateid =0. Abbruch")
                Return False
            End If
            myGlobalz.sitzung.VorgangREC.mydb.Tabelle = "Vorgang"

            myGlobalz.sitzung.VorgangREC.mydb.SQL = _
             "DELETE FROM  " & myGlobalz.sitzung.VorgangREC.mydb.Tabelle & _
             "  WHERE VORGANGSID=:VORGANGSID" '& MYGLOBALZ.SITZUNG.VORGANGSID
            MeineDBConnection.Open()
            com = New OracleCommand(myGlobalz.sitzung.VorgangREC.mydb.SQL, MeineDBConnection)
            '  setSQLParams(com, vid)
            com.Parameters.AddWithValue(":VORGANGSID", vid)

            anzahlTreffer& = CInt(com.ExecuteNonQuery)
            MeineDBConnection.Close()

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

    Public Function holeNeueVorgangsNummer(ByVal sachgebiet As String) As Integer
        Dim maxnr As Long?
        myGlobalz.sitzung.VorgangREC.mydb.SQL = _
         String.Format("SELECT  max(v.vorgangsnr)" + _
         " FROM vorgang v,  stammdaten s  " + _
         " where v.Sachgebietnr='{0}'" + _
         " and s.VorgangsID=v.vorgangsid ", sachgebiet$)
        Dim result$ = ""
        Dim com As New OracleCommand()
        MeineDBConnection.Close()
        com.Connection = MeineDBConnection 'myGlobalz.sitzung.VorgangREC.myconn
        com.CommandText = myGlobalz.sitzung.VorgangREC.mydb.SQL
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

    ''' <summary>
    ''' Das ergebnis liegt auf der Datatable myGlobalz.sitzung.VorgangREC.dt
    ''' </summary>
    ''' <param name="SQL">Vollständiges SQL statement</param>
    ''' <param name="Tabelle">Die tabelle muss auch im sql statement enthalten sein</param>
    ''' <remarks>return anzahl der treffer</remarks>
    Public Function selectFromParadigmaTabelle(ByVal SQL As String,
                                               ByVal Tabelle As String,
                                               ByRef resultDT As DataTable) As Integer
        Try
            myGlobalz.sitzung.VorgangREC.mydb.Tabelle = Tabelle$
            myGlobalz.sitzung.VorgangREC.mydb.SQL = SQL
            nachricht(myGlobalz.sitzung.VorgangREC.getDataDT())
            If myGlobalz.sitzung.VorgangREC.mycount < 1 Then
                nachricht("Keine selectFromParadigmaTabelle gespeichert!")
                resultDT = Nothing
            Else
                nachricht(String.Format("{0} selectFromParadigmaTabelle vorhanden", myGlobalz.sitzung.VorgangREC.mycount))
                resultDT = myGlobalz.sitzung.VorgangREC.dt.Copy
            End If
            Return CInt(myGlobalz.sitzung.VorgangREC.mycount)
        Catch ex As Exception
            nachricht("Fehler in   nachricht: " & ex.ToString)
            Return -1
        End Try
    End Function
End Class
