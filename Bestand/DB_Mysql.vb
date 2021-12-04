Imports MySql.Data.MySqlClient
Imports System.Data



Public Class DB_Mysql
    
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

    Public Shared Function leseStammdaten(ByVal myneREC As IDB_grundfunktionen, ByVal vid as integer) as  Boolean  'myGlobalz.sitzung.VorgangsID
        myneREC.mydb.Tabelle = "stammdaten"
        Return getDT_("", vid, myneREC)
    End Function


    Public Shared Function leseWiedervorlageRecord(ByVal myneREC As IDB_grundfunktionen, ByVal wvlid as integer) as  Boolean
        myneREC.mydb.Tabelle = "WV"
        getDT_("id", wvlid, myneREC)
    End Function

    Public Shared Function getDT_(ByVal idspalte$, ByVal idwert%, ByVal myneREC As IDB_grundfunktionen) As Boolean   'myGlobalz.sitzung.VorgangREC
        Try
            If String.IsNullOrEmpty(idspalte$) Then
                idspalte$ = "VorgangsID"
            End If
            myneREC.mydb.SQL = _
             "select * from " & myneREC.mydb.Tabelle & _
             " where " & idspalte$ & "=" & idwert%
            Dim hinweis As String = myneREC.getDataDT()
            nachricht("  getDT_: " & vbCrLf & myneREC.mydb.SQL)
            If myneREC.dt.IsNothingOrEmpty Then
                My.Log.WriteEntry("Fatal Error ID " & "konnte nicht gefunden werden!" & myneREC.mydb.getDBinfo(""))
                Return False
            End If
            Return True
        Catch ex As Exception
            nachricht("FEhler: getDT_" & vbCrLf & vbCrLf & ex.ToString)
            Return False
        End Try
    End Function



#Region "RB_adresse"
    Public Shared Sub hole_AdressKoordinatenExtracted(ByVal halo_id%)
        
        myGlobalz.sitzung.postgresREC.mydb.SQL = "select rechts,hoch,fs from public.halofs " &  
         " where id = " & halo_id%
        Dim hinweis As String = myGlobalz.sitzung.postgresREC.getDataDT()
    End Sub

    Public Shared Sub hole_AdressKoordinaten_bynames() 
        myGlobalz.sitzung.postgresREC.mydb.SQL = "select rechts,hoch from public.halofs " & _
         myGlobalz.sitzung.postgresREC.mydb.Tabelle & _
         " where gemeindenr = " & myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig() & _
         " and sname = '" & myGlobalz.sitzung.aktADR.Gisadresse.strasseName() & "'" & _
         " and hausnr = " & myGlobalz.sitzung.aktADR.Gisadresse.hausNr & _
         " and zusatz = '" & myGlobalz.sitzung.aktADR.Gisadresse.hausZusatz & "'"
        nachricht(myGlobalz.sitzung.postgresREC.getDataDT())
    End Sub

    Public Shared Sub DBholeHausnrDT() 
        myGlobalz.sitzung.postgresREC.mydb.SQL = String.Format("select distinct id ,cast(concat(hausnr,zusatz) as CHAR) as hausnrkombi from  public.halofs" &
                                                           " where gemeindenr = {0} and strcode = {1} order by  hausnr,zusatz", 
                                                           myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(), 
                                                           myGlobalz.sitzung.aktADR.Gisadresse.strasseCode())
        nachricht(myGlobalz.sitzung.postgresREC.getDataDT())
    End Sub

    Public Shared Sub DBholeHausnrDTVorgaenge_alledb()
        myGlobalz.sitzung.tempREC.mydb.Tabelle = "paraadresse" ' abs(hausnrkombi)in mysql
        If myGlobalz.vorgang_MYDB.dbtyp = "mysql" Then
            myGlobalz.sitzung.tempREC.mydb.SQL = _
         String.Format("select distinct id ,hausnrkombi  from {0} where gemeindenr = {1} and lower(Strassenname) = '{2}' order by abs(hausnrkombi)", _
                      myGlobalz.sitzung.tempREC.mydb.Tabelle, myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(), myGlobalz.sitzung.aktADR.Gisadresse.strasseName().ToLower)
        End If
        If myGlobalz.vorgang_MYDB.dbtyp = "oracle" Then
            myGlobalz.sitzung.tempREC.mydb.SQL = _
         String.Format("select distinct id ,hausnrkombi  from {0} where gemeindenr = {1} and lower(Strassenname) = '{2}' order by to_number(regexp_substr(hausnrkombi,'^[0-9]+')),to_number(regexp_substr(hausnrkombi,'$[0-9]+'))", _
                        myGlobalz.sitzung.tempREC.mydb.Tabelle, myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(), myGlobalz.sitzung.aktADR.Gisadresse.strasseName().ToLower)
        End If

        ' nachricht(myGlobalz.sitzung.tempREC.getDataDT())

        Dim resultdt As New System.Data.DataTable
        VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.tempREC.mydb.SQL, myGlobalz.sitzung.tempREC.mydb.Tabelle, resultdt)
        myGlobalz.sitzung.tempREC.dt = resultdt.Copy
        'Todo aufbauend darauf die kompletten vorgänge ziehen
    End Sub

    Public Shared Sub holeStrasseDT4Vorgaenge()
        myGlobalz.sitzung.tempREC.mydb.Tabelle = "paraadresse"
        myGlobalz.sitzung.tempREC.mydb.SQL = String.Format("select distinct strassenname  as sname from {0} where gemeindenr = {1} order by	strassenname asc",
                                                           myGlobalz.sitzung.tempREC.mydb.Tabelle, myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig())
        ' nachricht(myGlobalz.sitzung.tempREC.getDataDT())

        Dim resultdt As New System.Data.DataTable
        VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.tempREC.mydb.SQL, myGlobalz.sitzung.tempREC.mydb.Tabelle, resultdt)
        myGlobalz.sitzung.tempREC.dt = resultdt.Copy
    End Sub


    Public Shared Sub holeStrasseDT() 
        myGlobalz.sitzung.postgresREC.mydb.SQL = "select distinct strcode ,sname from public.halofs " & _
         myGlobalz.sitzung.postgresREC.mydb.Tabelle & _
         " where gemeindenr = " & myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig() & _
         " order by	sname asc"
        nachricht(myGlobalz.sitzung.postgresREC.getDataDT())
    End Sub

    Public Shared Sub holeZaehlerDTinvorgaengen()
        myGlobalz.sitzung.tempREC.mydb.SQL = "select distinct zaehler  from paraflurstueck" & _
         " where gemcode = " & myGlobalz.sitzung.aktFST.normflst.gemcode & _
         " and flur = " & myGlobalz.sitzung.aktFST.normflst.flur & _
         " order by zaehler  "
        nachricht(myGlobalz.sitzung.tempREC.getDataDT())
    End Sub

    Public Shared Sub holeZaehlerDT()
        myGlobalz.sitzung.postgresREC.mydb.SQL = "select distinct zaehler  from   flurkarte.basis_f " &  
         " where gemarkung = " & myGlobalz.sitzung.aktFST.normflst.gemcode & _
         " and flur = " & myGlobalz.sitzung.aktFST.normflst.flur & _
         " order by zaehler  "
        nachricht(myGlobalz.sitzung.postgresREC.getDataDT())
    End Sub
    Public Shared Sub holeNennerDTinVorgaengen()
        myGlobalz.sitzung.tempREC.mydb.SQL = "select distinct nenner,id  from paraflurstueck" & _
         " where gemcode = " & myGlobalz.sitzung.aktFST.normflst.gemcode & _
         " and flur = " & myGlobalz.sitzung.aktFST.normflst.flur & _
         " and zaehler = " & myGlobalz.sitzung.aktFST.normflst.zaehler & _
         " order by nenner  "
        nachricht(myGlobalz.sitzung.tempREC.getDataDT())
    End Sub

    Public Shared Sub holeNennerDT()
        myGlobalz.sitzung.postgresREC.mydb.SQL = "select distinct nenner  from   flurkarte.basis_f " &  
         " where gemarkung = " & myGlobalz.sitzung.aktFST.normflst.gemcode & _
         " and flur = " & myGlobalz.sitzung.aktFST.normflst.flur & _
         " and zaehler = " & myGlobalz.sitzung.aktFST.normflst.zaehler & _
         " order by nenner  "
        nachricht(myGlobalz.sitzung.postgresREC.getDataDT())
    End Sub

    Public Shared Sub holeFlureDT()
        myGlobalz.sitzung.postgresREC.mydb.SQL = "select distinct flur  from   flurkarte.basis_f " &  
         " where gemarkung = " & myGlobalz.sitzung.aktFST.normflst.gemcode & _
         " order by flur "
        nachricht(myGlobalz.sitzung.postgresREC.getDataDT())
    End Sub

 
 
#End Region



    Public Shared Sub hole_FSTKoordinatenExtracted(ByVal FS$)
        myGlobalz.sitzung.postgresREC.mydb.SQL = "select rechts,hoch,fs from  flurkarte.basis_f " &  
                                " where FS = '" & FS$ & "'"
        nachricht(myGlobalz.sitzung.postgresREC.getDataDT())
    End Sub

#Region "Beteiligte"

    Public Shared Function Entkoppelung_Vorgang_Person(ByVal vorgangID%, ByVal personenID as integer) as  Integer
        Dim anzahlTreffer&
        Dim newid& = -1
        Try
            myGlobalz.sitzung.tempREC = CType(CType(myGlobalz.sitzung.VorgangREC, clsDBspecMYSQL), IDB_grundfunktionen)
            myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
            myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "Person2vorgang"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             "delete from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & " where " & _
             "     Vorgangsid=" & vorgangID% & _
             " and PersonenID=" & personenID%
            anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ', myGlobalz.mylog)
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichernv:" & myGlobalz.sitzung.tempREC.mydb.SQL)
                Return -1
            Else
                Return CInt(anzahlTreffer)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
            Return -2
        End Try
    End Function






    'Private Shared Sub personenRECRollenspaltefuellen(ByVal personenID_DT As DataTable)
    '    Try
    '        'Feld Bemerkung noch nicht implementiert
    '        Dim personenIDinPREC% = 0, personenIDinKoppelung% = 0
    '        For Each row As DataRow In myGlobalz.sitzung.personenRec.dt.Rows
    '            personenIDinPREC% = CInt(row.Item("PersonenID"))
    '            If personenIDinPREC > 0 Then
    '                For Each row2 In personenID_DT.AsEnumerable
    '                    If CInt(row2.Item("PersonenID")) = personenIDinPREC Then
    '                        If IsDBNull(row2.Item("Rolle")) Then
    '                            row.Item("Rolle") = "Privat"
    '                        Else
    '                            row.Item("Rolle") = CStr(row2.Item("Rolle"))
    '                        End If
    '                        Exit For
    '                    End If
    '                Next
    '            End If
    '        Next
    '    Catch ex As Exception
    '        nachricht_und_Mbox("" & ex.ToString)
    '    End Try
    'End Sub



    'Private Shared Function ZuerstDiePersonenIDsholen(ByVal personenid_dt As DataTable) As Boolean
    '    dim hinweis as string 
    '    myGlobalz.sitzung.tempREC.mydb.MySQLServer = myGlobalz.sitzung.VorgangREC.mydb.MySQLServer
    '    myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
    '    myGlobalz.sitzung.tempREC.mydb.Tabelle = "Person2vorgang"    ''& " order by ts desc"
    '    myGlobalz.sitzung.tempREC.mydb.SQL = _
    '     String.Format("SELECT * FROM " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
    '     " where VorgangsID=" & myGlobalz.sitzung.VorgangsID)
    '    hinweis = myGlobalz.sitzung.tempREC.getDataDT()
    '    If myGlobalz.sitzung.tempREC.mycount < 1 Then
    '        nachricht("Keine Ereignisse gespeichert!")
    '        Return False
    '    Else
    '        nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.tempREC.mycount))
    '        Return True
    '    End If
    'End Function



    'Private Shared Function erzeugePersonenListezuVorgang(ByVal summe as string) as  Boolean
    '	dim hinweis as string 
    '	myGlobalz.sitzung.personenRec.mydb.Tabelle = "personen"
    '	myGlobalz.sitzung.personenRec.mydb.SQL = summe$
    '	hinweis = myGlobalz.sitzung.personenRec.getDataDT()
    '	If myGlobalz.sitzung.personenRec.mycount < 1 Then
    '		nachricht("Keine Ereignisse gespeichert!")
    '		Return False
    '	Else
    '		nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.personenRec.mycount))
    '		Return True
    '	End If
    'End Function

    Public Shared Function leseEinzelPersonViaPersonenID() As Boolean
        Dim hinweis As String
        myGlobalz.sitzung.beteiligteREC.mydb.Tabelle = "personen"
        myGlobalz.sitzung.beteiligteREC.mydb.SQL = "SELECT * FROM " & myGlobalz.sitzung.beteiligteREC.mydb.Tabelle & _
         " where PersonenID=" & myGlobalz.sitzung.aktPerson.PersonenID
        hinweis = myGlobalz.sitzung.beteiligteREC.getDataDT()
        If myGlobalz.sitzung.beteiligteREC.mycount < 1 Then
            nachricht("Keine Personen gespeichert!")
            Return False
        Else
            nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.beteiligteREC.mycount))
            Return True
        End If
    End Function

    Public Shared Function Beteiligte_abspeichern_Edit(ByVal pid%, ByVal vid%, ByVal lpers As Person) As Integer
        Dim anzahl As integer
        If myGlobalz.beteiligte_MYDB.dbtyp = "mysql" Then
            Dim zzz As New clsBeteiligteDBCRUD_MYSQL(clsDBspecMYSQL.getConnection(myGlobalz.beteiligte_MYDB))
            anzahl = zzz.Beteiligte_abspeichern_EditExtracted(pid%, vid, lpers)
            zzz.dispose
        End If
        If myGlobalz.beteiligte_MYDB.dbtyp = "oracle" Then
            Dim zzz As New clsBeteiligteDBCRUD_ORACLE(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.beteiligte_MYDB))
            anzahl% = zzz.Beteiligte_abspeichern_EditExtracted(pid%, vid, lpers)
              zzz.dispose
        End If
        Return anzahl%
    End Function

#End Region
#Region "Workflow"

    Public Shared Sub initWorkflowDatatable_beteiligte(ByRef hinweis$)
        myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
        myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
        myGlobalz.sitzung.tempREC.mydb.Tabelle = "wfbeteiligte" ''& " order by ts desc"
        myGlobalz.sitzung.tempREC.mydb.SQL = _
         "SELECT wfbeteiligteID as ID,rolle as name FROM " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
         " WHERE SACHGEBIETSNR=" & myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl.ToString & _
         "  order by reihenfolge"
        hinweis = myGlobalz.sitzung.tempREC.getDataDT()
        If myGlobalz.sitzung.tempREC.mycount < 1 Then
            ' nachricht_und_Mbox("Fehler: Die Datenbank ist nicht ansprechbar1")
            nachricht("Keine Ereignisse gespeichert a!")
        Else
            nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.EreignisseRec.mycount))
        End If
    End Sub

    Public Shared Sub initWorkflowDatatable_verlauf()
        myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
        myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
        myGlobalz.sitzung.tempREC.mydb.Tabelle = "workflow" ''& " order by ts desc"
        myGlobalz.sitzung.tempREC.mydb.SQL = _
         "SELECT workflowid as ID,aktion as name FROM " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
         " WHERE SACHGEBIETSNR=" & myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl.ToString & _
         "  order by reihenfolge"
        nachricht("hinweis:" & myGlobalz.sitzung.tempREC.getDataDT())
        If myGlobalz.sitzung.tempREC.mycount < 1 Then
            ' nachricht_und_Mbox("Fehler: Die Datenbank ist nicht ansprechbar1")
            nachricht("Keine Ereignisse gespeichert z!")
        Else
            nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.EreignisseRec.mycount))
        End If
    End Sub

    Public Shared Sub initWorkflowDatatable()
        myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
        myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
        myGlobalz.sitzung.tempREC.mydb.Tabelle = "workflow" ''& " order by ts desc"
        myGlobalz.sitzung.tempREC.mydb.SQL = _
         "SELECT * FROM " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
         " WHERE SACHGEBIETSNR=" & myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl.ToString & _
         "  order by reihenfolge"
        nachricht("hinweis:" & myGlobalz.sitzung.tempREC.getDataDT())
        If myGlobalz.sitzung.tempREC.mycount < 1 Then
            ' nachricht_und_Mbox("Fehler: Die Datenbank ist nicht ansprechbar1")
            nachricht("Keine Ereignisse gespeichert b!")
        Else
            nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.EreignisseRec.mycount))
        End If
    End Sub

    Public Shared Function updateWorkflowtable(ByVal WorkflowID%, ByVal erledigt As Integer) As Boolean
        If WorkflowID < 1 Then
            nachricht_und_Mbox("FEHLER: updateWorkflowtable updateid =0. Abbruch")
            Return False
        End If
        myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
        myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
        myGlobalz.sitzung.tempREC.mydb.Tabelle = "workflow" ''& " order by ts desc"
        myGlobalz.sitzung.tempREC.mydb.SQL = _
         "update " & myGlobalz.sitzung.tempREC.mydb.Tabelle & "  set " & _
         " erledigt=" & erledigt.ToString & _
         " where  WorkflowID=" & WorkflowID
        Dim newid As Long
        Dim anzahlTreffer As Long = myGlobalz.sitzung.tempREC.sqlexecute(newid) ', myGlobalz.mylog)
        If anzahlTreffer < 1 Then
            nachricht_und_Mbox("Problem beim Abspeichernw:" & myGlobalz.sitzung.beteiligteREC.mydb.SQL)
            Return False
        Else
            Return True
        End If
    End Function

#End Region

    Shared Function suchePersonNachFilterDT_Like(ByVal filter As String) As DataTable
        myGlobalz.sitzung.tempREC.mydb.Tabelle = "personen"
        myGlobalz.sitzung.tempREC.mydb.SQL = _
         "SELECT * FROM " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
         " where Name like '%" & filter & "%'" & _
         " order by name,vorname"
        nachricht(myGlobalz.sitzung.tempREC.getDataDT())
        If myGlobalz.sitzung.tempREC.mycount < 1 Then
            nachricht("Keine Personen gespeichert!")
            Return Nothing
        Else
            nachricht(String.Format("{0} Personen vorhanden", myGlobalz.sitzung.tempREC.mycount))
            Return myGlobalz.sitzung.tempREC.dt
        End If
    End Function

    'Public Shared Function UNION_SQL_erzeugen(ByVal anyDT As DataTable, ByVal tabellenname$, ByVal ausgabespaltenNr%, ByVal idspalte as string) as  String
    '    Try
    '        Dim summe$ = ""
    '        If anyDT.Rows.Count = 1 Then
    '            summe$ = summe$ & "SELECT * FROM " & tabellenname$ & _
    '             " where " & idspalte$ & "=" & anyDT.Rows(0).Item(ausgabespaltenNr).ToString & ""
    '        Else
    '            For i = 0 To anyDT.Rows.Count - 1
    '                summe$ = summe$ & "(SELECT * FROM " & tabellenname$ & _
    '                 " where " & idspalte$ & "=" & anyDT.Rows(i).Item(ausgabespaltenNr).ToString & ")"
    '                If i < anyDT.Rows.Count - 1 Then
    '                    summe$ = summe$ & " union "
    '                End If
    '            Next
    '        End If
    '        Return summe$
    '    Catch ex As Exception
    '        Return "-1"
    '    End Try
    'End Function

    'Public Shared Function UNION_SQL_VST_erzeugen(ByVal anyDT As DataTable, ByVal ausgabespaltenNr%, ByVal idspalte as string) as  String
    '    Try
    '        'SELECT * FROM vorgang v,stammdaten  s where v.vorgangsid=s.vorgangsid  and v.vorgangsid=1356
    '        Dim summe$ = ""
    '        If anyDT.Rows.Count = 1 Then
    '            summe$ = summe$ & "SELECT * FROM  vorgang v,stammdaten  s where   v.vorgangsid=s.vorgangsid  and " & _
    '             idspalte$ & "=" & anyDT.Rows(0).Item(ausgabespaltenNr).ToString & ""
    '        Else
    '            For i = 0 To anyDT.Rows.Count - 1
    '                summe$ = summe$ & "SELECT * FROM  vorgang v,stammdaten  s where   v.vorgangsid=s.vorgangsid  and " & _
    '             idspalte$ & "=" & anyDT.Rows(i).Item(ausgabespaltenNr).ToString & ""
    '                If i < anyDT.Rows.Count - 1 Then
    '                    summe$ = summe$ & " union "
    '                End If
    '            Next
    '        End If
    '        Return summe$
    '    Catch ex As Exception
    '        Return "-1"
    '    End Try
    'End Function

    'Public Shared Function initAdressen4personDatatable(ByVal personenID As Integer) As Boolean
    '    'koppelungstabelle abfragen> adressid-liste
    '    If adressIDlisteVonPersonErstellen(personenID%) Then
    '        myGlobalz.sitzung.AdressDT = myGlobalz.sitzung.tempREC.dt.Copy
    '        myGlobalz.sitzung.tempREC.mydb.Tabelle = "paraadresse"
    '        Dim SQL$ = clsDBtools.UNION_SQL_erzeugenInn(myGlobalz.sitzung.AdressDT, myGlobalz.sitzung.tempREC.mydb.Tabelle, 1, "id")
    '        My.Log.WriteEntry("UNION: " & SQL)
    '        adressliste4AdressIDListe(SQL$)
    '        myGlobalz.sitzung.AdressDT = myGlobalz.sitzung.tempREC.dt.Copy
    '        Return True
    '    Else
    '        ' nachricht_und_Mbox("Für personid: " & personenID% & " gibts keine Adressen")
    '        Return False
    '    End If
    'End Function

    Public Shared Function adressliste4AdressIDListe(ByVal sql As String) As Boolean

        myGlobalz.sitzung.tempREC.mydb.Tabelle = "paraadresse"
        myGlobalz.sitzung.tempREC.mydb.SQL = sql$
        nachricht(myGlobalz.sitzung.tempREC.getDataDT())
        If myGlobalz.sitzung.tempREC.mycount < 1 Then
            nachricht("adressliste4AdressIDListe!" & sql$)
            Return False
        Else
            nachricht(String.Format("{0} adressliste4AdressIDListe vorhanden", myGlobalz.sitzung.tempREC.mycount))
            Return True
        End If
    End Function

    Public Shared Function adressIDlisteVonPersonErstellen(ByVal personenID As Integer) As Boolean
        myGlobalz.sitzung.tempREC.mydb.Tabelle = "person2adresse"
        myGlobalz.sitzung.tempREC.mydb.SQL = "SELECT * FROM " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
         " where personenid =" & personenID%
        nachricht(myGlobalz.sitzung.tempREC.getDataDT())
        If myGlobalz.sitzung.tempREC.mycount < 1 Then
            nachricht("Keine adressen zur Personen gespeichert!" & personenID%)
            Return False
        Else
            nachricht(String.Format("{0} adresslisteVonPersonErstellen vorhanden", myGlobalz.sitzung.tempREC.mycount))
            Return True
        End If
    End Function

    Shared Function suchePersonNachFilterDT_inProbaugDB(ByVal filterName$, ByVal FilterOrt As String) As DataTable
        Dim hinweis As String, ortfilter$
        '   " `ID` AS `PersonenID` " & _
        If String.IsNullOrEmpty(FilterOrt) Then
            ortfilter$ = ""
        Else
            ortfilter$ = " and (AS_Ort='" & FilterOrt & "') "
        End If
        If myGlobalz.sitzung.probaugREC.mydb.dbtyp = "mysql" Then
            myGlobalz.sitzung.probaugREC.mydb.SQL = "select distinct " & _
         " `AS_nachname` AS `Name` " & _
         ",`AS_vorname` AS `Vorname`" & _
         ",`AS_Titel` AS `Namenszusatz` " & _
         ",`AS_Ort` AS `AS_Ort` " & _
         ",`AS_Strasse` AS `AS_Strasse` " & _
         ",`AS_PLZ` AS `AS_PLZ` " & _
         ",`AS_Hausnummer` AS `AS_Hausnummer` " & _
         ",`FS` " & _
         ", CONCAT(CAST(Jahr as CHAR),'_',CAST(laufnr as CHAR),'_',vorhaben1,'_',vorhaben2,'_',vorhabensmerkmal) AS `Bemerkung` " & _
         " from `antraege`   " + _
         " where (upper(AS_nachname) like '%" & filterName.ToUpper & "%'" & _
         " or  upper(AS_vorname) like '%" & filterName.ToUpper & "%')" & _
         ortfilter &
          "  order by bemerkung desc limit 100"
            '"  order by name,vorname,as_ort limit 500"
            '				 ", CONCAT(CAST(Jahr as CHAR),'_',CAST(laufnr as CHAR),'_',vorhaben1,'_',vorhaben2,'_',vorhabensmerkmal) AS `Bemerkung` " & _
        End If
        If myGlobalz.sitzung.probaugREC.mydb.dbtyp = "oracle" Then
            myGlobalz.sitzung.probaugREC.mydb.SQL = "select distinct " & _
         "  AS_nachname  AS  Name  " & _
         ", AS_vorname  AS  Vorname " & _
         ", AS_Titel  AS  Namenszusatz  " & _
         ", AS_Ort  AS  AS_Ort  " & _
         ", AS_Strasse  AS  AS_Strasse  " & _
         ", AS_PLZ  AS  AS_PLZ  " & _
         ", AS_Hausnummer  AS  AS_Hausnummer  " & _
         ", FS  " & _
         ", CONCAT(CAST(Jahr as CHAR),'_',CAST(laufnr as CHAR),'_',vorhaben1,'_',vorhaben2,'_',vorhabensmerkmal) AS  Bemerkung  " & _
         " from  antraege    " + _
         " where (upper(AS_nachname) like '%" & filterName.ToUpper & "%'" & _
         " or  upper(AS_vorname) like '%" & filterName.ToUpper & "%')" & _
         ortfilter &
          "  order by bemerkung desc"
            '"  order by name,vorname,as_ort limit 500"
            '				 ", CONCAT(CAST(Jahr as CHAR),'_',CAST(laufnr as CHAR),'_',vorhaben1,'_',vorhaben2,'_',vorhabensmerkmal) AS `Bemerkung` " & _
        End If
        hinweis = myGlobalz.sitzung.probaugREC.getDataDT()
        If myGlobalz.sitzung.probaugREC.mycount < 1 Then
            nachricht("Keine Personen gespeichert!")
            Return Nothing
        Else
            nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.probaugREC.mycount))
            Return myGlobalz.sitzung.probaugREC.dt
        End If
    End Function

    Public Shared Function initKollegenDT() As DataTable
        Dim hinweis As String
        myGlobalz.sitzung.tempREC.mydb.Schema = "paradigma"
        myGlobalz.sitzung.tempREC.mydb.Tabelle = "bearbeiter"
        myGlobalz.sitzung.tempREC.mydb.SQL = "SELECT id as Id,initial_ as Titel, Name FROM " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
         " order by name"
        hinweis = myGlobalz.sitzung.tempREC.getDataDT()
        If myGlobalz.sitzung.tempREC.mycount < 1 Then
            nachricht("Keine Personen gespeichert!")
            Return Nothing
        Else
            nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.tempREC.mycount))
            Return myGlobalz.sitzung.tempREC.dt
        End If
    End Function
    Shared Function suchePersonNachFilterDT_istgleich(ByVal sqlstring As String) As DataTable
        Dim hinweis As String
        '  myGlobalz.sitzung.tempREC.mydb.Tabelle = myGlobalz.sitzung.tempREC.mydb.Tabelle
        myGlobalz.sitzung.tempREC.mydb.SQL = sqlstring$
        hinweis = myGlobalz.sitzung.tempREC.getDataDT()
        If myGlobalz.sitzung.tempREC.mycount < 1 Then
            nachricht("Keine Personen gespeichert!")
            Return Nothing
        Else
            nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.tempREC.mycount))
            Return myGlobalz.sitzung.tempREC.dt
        End If
    End Function
 

  
 


    Public Shared Function initBearbeiterLISTEByUserid_DT_ausParadigma_datenbank(ByVal sql As String) As DataTable

        '  Dim lokREC As LibDB.IDB_grundfunktionen
        ' lokREC = CType(myGlobalz.sitzung.BearbeiterREC, clsDBspecMYSQL)
        myGlobalz.sitzung.BearbeiterREC.mydb.Tabelle = "bearbeiter"
        myGlobalz.sitzung.BearbeiterREC.mydb.SQL = sql
        nachricht(myGlobalz.sitzung.BearbeiterREC.getDataDT)
        Return myGlobalz.sitzung.BearbeiterREC.dt
    End Function


    Shared Function initBearbeiterByUserid_ausParadigmadb(ByRef usi As clsBearbeiter, ByVal spalteKey$, ByVal spaltevalue As String) As Boolean
        'Todo Diese funktion sollte auf LINQ umgestellt werden
        Dim lokrec As New DataTable
        Try
            If spaltevalue$ = "FEINEN_DR" Then
                spaltevalue$ = "A670024"
            End If
            Dim sql$ = String.Format("select * from  bearbeiter where lower({0})='{1}' or kuerzel1='{1}'  or lower(initial_)='{1}'", spalteKey, spaltevalue$.ToLower)
            lokrec = initBearbeiterLISTEByUserid_DT_ausParadigma_datenbank(sql).Copy
            usi.Namenszusatz = clsDBtools.fieldvalue(lokrec.Rows(0).Item("namenszusatz"))
            usi.Name = clsDBtools.fieldvalue(lokrec.Rows(0).Item("name"))
            usi.Vorname = clsDBtools.fieldvalue(lokrec.Rows(0).Item("vorname"))
            usi.Rang = clsDBtools.fieldvalue(lokrec.Rows(0).Item("rang"))
            usi.Rites = clsDBtools.fieldvalue(lokrec.Rows(0).Item("rites"))
            usi.STDGRANTS = clsDBtools.fieldvalue(lokrec.Rows(0).Item("STDGRANTS"))
            usi.Kontakt.elektr.Telefon1 = clsDBtools.fieldvalue(lokrec.Rows(0).Item("Telefon"))
            usi.Kontakt.elektr.Fax1 = clsDBtools.fieldvalue(lokrec.Rows(0).Item("Fax"))
            usi.Kuerzel2Stellig = clsDBtools.fieldvalue(lokrec.Rows(0).Item("Kuerzel1"))
            usi.Kontakt.elektr.Email = clsDBtools.fieldvalue(lokrec.Rows(0).Item("email"))
            usi.Bemerkung = clsDBtools.fieldvalue(lokrec.Rows(0).Item("abteilung"))
            usi.Rolle = clsDBtools.fieldvalue(lokrec.Rows(0).Item("rolle"))
            usi.ExpandHeaderInSachgebiet = clsDBtools.fieldvalue(lokrec.Rows(0).Item("ExpandHeaderInSachgebiet"))

            usi.Initiale = usi.getInitial
            Return True
        Catch ex As Exception
            nachricht_und_Mbox("fehler User ist unbekannt: " & usi.username & vbCrLf & " Bitte informieren sie den Admin." & vbCrLf,ex)
            Return False
        Finally
            lokrec.Dispose()
        End Try
    End Function

  

    Public Shared Function getOrglisteDT() As Integer
        Try
            myGlobalz.sitzung.kontaktdatenREC.mydb.Tabelle = "Beteiligte"
            myGlobalz.sitzung.kontaktdatenREC.mydb.SQL = _
             "select  distinct orgname,orgzusatz,orgtyp1,orgtyp2,Gemeindename,strasse,hausnr,fftelefon1 from " & myGlobalz.sitzung.kontaktdatenREC.mydb.Tabelle &
             " where orgname<>'Kreis Offenbach' " &
                   " order by orgName,orgzusatz"
            Dim hinweis As String = myGlobalz.sitzung.kontaktdatenREC.getDataDT()
            If myGlobalz.sitzung.kontaktdatenREC.dt.IsNothingOrEmpty Then
                nachricht_und_Mbox("Problem beim Abspeicherny: initOrgDT:" & myGlobalz.sitzung.kontaktdatenREC.mydb.SQL)
                Return 0
            Else
                Return myGlobalz.sitzung.kontaktdatenREC.dt.Rows.Count
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Problem beim Abspeichernz:" & vbCrLf & ex.ToString)
            Return -1
        End Try
    End Function


    Shared Function initAnschriftDT(ByVal AnschriftID As Integer) As Boolean
        Try
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "Anschrift"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             "select * from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
             " where AnschriftID=" & AnschriftID%
            Dim hinweis As String = myGlobalz.sitzung.tempREC.getDataDT()
            If myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
                nachricht_und_Mbox("Problem beim Abspeichern1:Problem beim initAnschriftDT:" & myGlobalz.sitzung.tempREC.mydb.SQL)
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Problem beim Abspeichern2:Problem beim initAnschriftDT: " & vbCrLf & ex.ToString)
            Return False
        End Try
    End Function

    Shared Function initOrgDT(ByVal OrgID As Integer) As Boolean
        If OrgID < 1 Then
            My.Log.WriteEntry("	initOrgDT: ist ist kleiner 1")
            Return False
        End If
        Try
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "Organisation"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             "select * from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
             " where OrgID=" & OrgID%
            nachricht("hinweis$ = " & myGlobalz.sitzung.tempREC.getDataDT())
            If myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
                nachricht_und_Mbox("Problem beim initOrgDT:" & myGlobalz.sitzung.tempREC.mydb.SQL)
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Abspeichern: " & vbCrLf & ex.ToString)
            Return False
        End Try
    End Function

    Shared Function initBankkontoDT(ByVal BankkontoID As Integer) As Boolean
        If BankkontoID% < 1 Then
            My.Log.WriteEntry("	initBankkontoDT: ist ist kleiner 1")
            Return False
        End If
        Try
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "bankverbindung"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             "select * from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
             " where BankkontoID=" & BankkontoID%
            Dim hinweis As String = myGlobalz.sitzung.tempREC.getDataDT()
            If myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
                nachricht_und_Mbox("Problem beim initBankkontoDT:" & myGlobalz.sitzung.tempREC.mydb.SQL)
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Abspeichern: " & vbCrLf & ex.ToString)
            Return False
        End Try
    End Function

    Shared Function suchePersonNachFilterDT_inVorgangsDB(ByVal filter As String) As DataTable
        '   dim hinweis as string 
        filter = filter.ToLower
        myGlobalz.sitzung.vorgangsbeteiligteAuswahlREC.mydb.SQL =
         "select distinct anrede,namenszusatz,nachname,vorname,plz,gemeindename,Strasse,Hausnr,orgname," &
                        "orgzusatz,FFTelefon1,FFTelefon2,FFFax1,FFFax2,FFMobilfon,FFhomepage,FFemail, gesellfunktion from " &
         "beteiligte " & _
         " where upper(nachname) like '%" & filter.ToUpper & "%'" & _
         " or  upper(vorname) like '%" & filter.ToUpper & "%'" & _
         " or  upper(orgname) like '%" & filter.ToUpper & "%'" & _
         " or  upper(orgzusatz) like '%" & filter.ToUpper & "%'" & _
         " order by nachname,vorname,gemeindename"
        myGlobalz.sitzung.vorgangsbeteiligteAuswahlREC.mydb.SQL = myGlobalz.sitzung.vorgangsbeteiligteAuswahlREC.mydb.SQL.ToUpper
        nachricht("hinweis = " & myGlobalz.sitzung.vorgangsbeteiligteAuswahlREC.getDataDT())
        If myGlobalz.sitzung.vorgangsbeteiligteAuswahlREC.mycount < 1 Then
            nachricht("Keine Personen gespeichert!")
            Return Nothing
        Else
            nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.vorgangsbeteiligteAuswahlREC.mycount))
            Return myGlobalz.sitzung.vorgangsbeteiligteAuswahlREC.dt
        End If
    End Function

    '    Public Shared Function SpalteZuDatatableHinzufuegen(ByVal meinelokaleDT As DataTable, ByVal Spaltenname$, ByVal SpaltenTyp as string) as  Boolean '"System.Int16"
    '        Try
    '            For Each col As DataColumn In meinelokaleDT.Columns
    '                If col.ColumnName = Spaltenname$ Then
    '                    GoTo NICHTNEUANLEGEN
    '                End If
    '            Next
    '            meinelokaleDT.Columns.Add(Spaltenname$, System.Type.GetType(SpaltenTyp$))
    '            Return True
    'NICHTNEUANLEGEN:
    '            meinelokaleDT.Columns(Spaltenname$).ReadOnly = False
    '            Return True
    '        Catch ex As Exception
    '            MsgBox("Fehler in  SpalteHinzufuegen: " & ex.ToString)
    '            Return False
    '        End Try
    '    End Function

    '    Public Shared Sub SpalteInitialisieren(ByVal lokdt As DataTable, ByVal Spaltenname$, ByVal Wert%)
    '        For Each row As DataRow In lokdt.Rows
    '            row.Item(Spaltenname) = Wert
    '        Next
    '    End Sub


End Class