'Imports MySql.Data.MySqlClient
'Imports System.Data

'Imports System.Data.OracleClient
Imports LibDB
Imports System.Data

Public Class DB_Oracle_sharedfunctions
        Public Shared Function leseStammdaten(ByVal myneREC As IDB_grundfunktionen, ByVal vid As Integer) As Boolean  'myGlobalz.sitzung.VorgangsID
        myneREC.mydb.Tabelle = "stammdaten"
        Return getDT_("", vid, myneREC)
    End Function


    Public Shared Function leseWiedervorlageRecord(ByVal myneREC As IDB_grundfunktionen, ByVal wvlid As Integer) As Boolean
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
        myGlobalz.sitzung.haloREC.mydb.Schema = "halosort"
        myGlobalz.sitzung.haloREC.mydb.Tabelle = "halofs"
        myGlobalz.sitzung.haloREC.mydb.SQL = "select rechts,hoch,fs,strcode,gemeindenr,hausnr,Zusatz from " & _
         myGlobalz.sitzung.haloREC.mydb.Tabelle & _
         " where id = " & halo_id%
        Dim hinweis As String = myGlobalz.sitzung.haloREC.getDataDT()
    End Sub

    Public Shared Sub hole_AdressKoordinaten_bynames()
        myGlobalz.sitzung.haloREC.mydb.Schema = "halosort"
        myGlobalz.sitzung.haloREC.mydb.Tabelle = "halofs"
        myGlobalz.sitzung.haloREC.mydb.SQL = "select rechts,hoch from " & _
         myGlobalz.sitzung.haloREC.mydb.Tabelle & _
         " where gemeindenr = " & myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig() & _
         " and sname = '" & myGlobalz.sitzung.aktADR.Gisadresse.strasseName() & "'" & _
         " and hausnr = " & myGlobalz.sitzung.aktADR.Gisadresse.hausNr & _
         " and zusatz = '" & myGlobalz.sitzung.aktADR.Gisadresse.hausZusatz & "'"
        nachricht(myGlobalz.sitzung.haloREC.getDataDT())
    End Sub

    Public Shared Sub DBholeHausnrDT()
        myGlobalz.sitzung.haloREC.mydb.Schema = "halosort"
        myGlobalz.sitzung.haloREC.mydb.Tabelle = "halofs"
        myGlobalz.sitzung.haloREC.mydb.SQL = String.Format("select distinct id ,cast(concat(hausnr,zusatz) as CHAR) as hausnrkombi " &
                                                           "from {0} where gemeindenr = {1} and strcode = {2} order by  hausnr,zusatz", _
         myGlobalz.sitzung.haloREC.mydb.Tabelle, myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(), myGlobalz.sitzung.aktADR.Gisadresse.strasseCode())
        nachricht(myGlobalz.sitzung.haloREC.getDataDT())
    End Sub

    'Private Shared Sub erstelleSQLfuerhausnr_alledb()
    '    If myGlobalz.vorgang_MYDB.dbtyp = "mysql" Then
    '        myGlobalz.sitzung.tempREC.mydb.SQL = _
    '     String.Format("select distinct hausnrkombi  from {0} where gemeindenr = {1} and lower(Strassenname) = '{2}' order by abs(hausnrkombi)", _
    '                  myGlobalz.sitzung.tempREC.mydb.Tabelle, myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(), myGlobalz.sitzung.aktADR.Gisadresse.strasseName().ToLower)
    '    End If
    '    If myGlobalz.vorgang_MYDB.dbtyp = "oracle" Then
    '        myGlobalz.sitzung.tempREC.mydb.SQL = _
    '     String.Format("select distinct hausnrkombi  from {0} where gemeindenr = {1} and lower(Strassenname) = '{2}'" &
    '                   " order by to_number(regexp_substr(hausnrkombi,'^[0-9]+')),to_number(regexp_substr(hausnrkombi,'$[0-9]+'))", _
    '                    myGlobalz.sitzung.tempREC.mydb.Tabelle, myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(), myGlobalz.sitzung.aktADR.Gisadresse.strasseName().ToLower)
    '    End If
    'End Sub
    Private Shared Sub erstelleSQLfuerhausnr_alledb2(iDB_grundfunktionen As IDB_grundfunktionen)
        If myGlobalz.vorgang_MYDB.dbtyp = "mysql" Then
            iDB_grundfunktionen.mydb.SQL = _
         String.Format("select distinct hausnrkombi  from {0} where gemeindenr = {1} and lower(Strassenname) = '{2}' order by abs(hausnrkombi)", _
                      iDB_grundfunktionen.mydb.Tabelle, myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(),
                      myGlobalz.sitzung.aktADR.Gisadresse.strasseName().ToLower)
        End If
        If myGlobalz.vorgang_MYDB.dbtyp = "oracle" Then
            iDB_grundfunktionen.mydb.SQL = _
         String.Format("select distinct hausnrkombi  from {0} where gemeindenr = {1} and lower(Strassenname) = '{2}'" &
                       " order by to_number(regexp_substr(hausnrkombi,'^[0-9]+')),to_number(regexp_substr(hausnrkombi,'$[0-9]+'))", _
                        iDB_grundfunktionen.mydb.Tabelle, myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(),
                        myGlobalz.sitzung.aktADR.Gisadresse.strasseName().ToLower)
        End If
    End Sub

    Public Shared Sub DBholeHausnrDTVorgaenge_alledb()
        myGlobalz.sitzung.tempREC2.mydb.Tabelle = "paraadresse" ' abs(hausnrkombi)in mysql
        erstelleSQLfuerhausnr_alledb2(myGlobalz.sitzung.tempREC2)

        Dim resultdt As New System.Data.DataTable
        Dim erfolg As Integer = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.tempREC2.mydb.SQL,
                                                                               myGlobalz.sitzung.tempREC2.mydb.Tabelle,
                                                                               resultdt)
        If resultdt IsNot Nothing Then myGlobalz.sitzung.tempREC2.dt = resultdt.Copy
    End Sub

    Public Shared Sub holeStrasseDT4Vorgaenge()
        myGlobalz.sitzung.tempREC.mydb.Tabelle = "paraadresse"
        myGlobalz.sitzung.tempREC.mydb.SQL = String.Format("select distinct trim(strassenname)  as sname from {0} where gemeindenr = {1} order by	trim(strassenname) asc",
                                                           myGlobalz.sitzung.tempREC.mydb.Tabelle, myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig())
        Dim resultdt As New System.Data.DataTable
        Dim erfolg As Integer = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.tempREC.mydb.SQL, myGlobalz.sitzung.tempREC.mydb.Tabelle, resultdt)
        myGlobalz.sitzung.tempREC.dt = resultdt.Copy

    End Sub



    Public Shared Sub holeStrasseDTausHalo(buchstabe As String)
        myGlobalz.sitzung.haloREC.mydb.Schema = "halosort"
        myGlobalz.sitzung.haloREC.mydb.Tabelle = "halofs"

        If String.IsNullOrEmpty(buchstabe) Then
            myGlobalz.sitzung.haloREC.mydb.SQL = "select distinct strcode ,sname,'halo' from " & _
                myGlobalz.sitzung.haloREC.mydb.Tabelle & _
                " where gemeindenr = " & myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig() & _
                " order by	sname asc"
        Else
            myGlobalz.sitzung.haloREC.mydb.SQL = "select distinct strcode ,sname,'halo' from " & _
        myGlobalz.sitzung.haloREC.mydb.Tabelle & _
        " where gemeindenr = " & myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig() & _
        " and lower(sname) like '" & buchstabe.Trim & "%'" & _
        " order by	sname asc "
        End If

        nachricht(myGlobalz.sitzung.haloREC.getDataDT())
    End Sub

    Public Shared Sub holeZaehlerDTinvorgaengen()
        myGlobalz.sitzung.tempREC.mydb.SQL = "select distinct zaehler  from paraflurstueck" & _
         " where gemcode = " & myGlobalz.sitzung.aktFST.normflst.gemcode & _
         " and flur = " & myGlobalz.sitzung.aktFST.normflst.flur & _
         " order by zaehler  "
        nachricht(myGlobalz.sitzung.tempREC.getDataDT())
    End Sub

    Public Shared Sub holeZaehlerDT()
        myGlobalz.sitzung.fstREC.mydb.SQL = "select distinct zaehler from " & _
         myGlobalz.sitzung.fstREC.mydb.Tabelle & _
         " where gemarkung = " & myGlobalz.sitzung.aktFST.normflst.gemcode & _
         " and flur = " & myGlobalz.sitzung.aktFST.normflst.flur & _
         " order by zaehler  "
        nachricht(myGlobalz.sitzung.fstREC.getDataDT())
    End Sub

    Public Shared Sub holeNennerDTinVorgaengen()
        myGlobalz.sitzung.tempREC.mydb.SQL = "select distinct nenner from paraflurstueck" & _
         " where gemcode = " & myGlobalz.sitzung.aktFST.normflst.gemcode & _
         " and flur = " & myGlobalz.sitzung.aktFST.normflst.flur & _
         " and zaehler = " & myGlobalz.sitzung.aktFST.normflst.zaehler & _
         " order by nenner  "
        nachricht(myGlobalz.sitzung.tempREC.getDataDT())
    End Sub

    Public Shared Sub holeSekidListeDTinVorgaengenZuFlurstueckSQL()
        myGlobalz.sitzung.tempREC.mydb.SQL = "select distinct id from paraflurstueck" & _
         " where gemcode = " & myGlobalz.sitzung.aktFST.normflst.gemcode & _
         " and flur = " & myGlobalz.sitzung.aktFST.normflst.flur & _
         " and zaehler = " & myGlobalz.sitzung.aktFST.normflst.zaehler & _
         " and nenner = " & myGlobalz.sitzung.aktFST.normflst.nenner & _
         " order by id  "
        nachricht(myGlobalz.sitzung.tempREC.getDataDT())
    End Sub

    Public Shared Sub holeNennerDT()
        myGlobalz.sitzung.fstREC.mydb.SQL = "select distinct nenner from " & _
         myGlobalz.sitzung.fstREC.mydb.Tabelle & _
         " where gemarkung = " & myGlobalz.sitzung.aktFST.normflst.gemcode & _
         " and flur = " & myGlobalz.sitzung.aktFST.normflst.flur & _
         " and zaehler = " & myGlobalz.sitzung.aktFST.normflst.zaehler & _
         " order by nenner  "
        nachricht(myGlobalz.sitzung.fstREC.getDataDT())
    End Sub

    Public Shared Sub holeFlureDT()
        myGlobalz.sitzung.fstREC.mydb.SQL = "select distinct flur from " & _
         myGlobalz.sitzung.fstREC.mydb.Tabelle & _
         " where gemarkung = " & myGlobalz.sitzung.aktFST.normflst.gemcode & _
         " order by flur "
        nachricht(myGlobalz.sitzung.fstREC.getDataDT())
    End Sub



    'Public Shared Sub holeGemeindeDT()
    '	myGlobalz.sitzung.AlbRec.mydb.Tabelle = "gmde"
    '	myGlobalz.sitzung.AlbRec.mydb.SQL = "select  col3 as Gemeindenr,col7 as Gemeindename from " & _
    '	 myGlobalz.sitzung.AlbRec.mydb.Tabelle & _
    '	 " where col3 like '438%'" & _
    '	 " order by	Gemeindename asc"
    '	Dim hinweis = myGlobalz.sitzung.AlbRec.getDataDT()
    'End Sub

    'Public Shared Sub holeGemarkungsDT()
    '	myGlobalz.sitzung.AlbRec.mydb.Tabelle = "gmkg"
    '	myGlobalz.sitzung.AlbRec.mydb.SQL = "select  col3 as Gemarkungsnr,col4 as Gemarkungsname from " & _
    '	 myGlobalz.sitzung.AlbRec.mydb.Tabelle & _
    '	 " where col8 like '438%'" & _
    '	 " order by	Gemarkungsname asc"
    '	Dim hinweis = myGlobalz.sitzung.AlbRec.getDataDT()
    'End Sub
#End Region



    Public Shared Sub hole_FSTKoordinatenExtracted(ByVal FS$)
        myGlobalz.sitzung.fstREC.mydb.SQL = "select * from " & myGlobalz.sitzung.fstREC.mydb.Tabelle & _
                                " where FS = '" & FS$ & "'"
        nachricht(myGlobalz.sitzung.fstREC.getDataDT())
        ' rechts,hoch,fs,area,fsgml,zeigtauf,weistauf,gebucht from " & _
    End Sub

#Region "Beteiligte"

    Public Shared Function Entkoppelung_Vorgang_Person(ByVal vorgangID%, ByVal personenID As Integer) As Integer
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
                nachricht_und_Mbox("Problem beim Abspeichern3:" & myGlobalz.sitzung.tempREC.mydb.SQL)
                Return -1
            Else
                Return CInt(anzahlTreffer)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
            Return -2
        End Try
    End Function
 
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
        Dim anzahl%
        If myGlobalz.beteiligte_MYDB.dbtyp = "mysql" Then
            Dim zzz As New clsBeteiligteDBCRUD_MYSQL(clsDBspecMYSQL.getConnection(myGlobalz.beteiligte_MYDB))
            anzahl% = zzz.Beteiligte_abspeichern_EditExtracted(pid%, vid, lpers)
            zzz.Dispose
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
            nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.beteiligteREC.mydb.SQL)
            Return False
        Else
            Return True
        End If
    End Function

#End Region

    Shared Function suchePersonNachFilterDT_LikeRolle(ByVal filter As String) As DataTable
        myGlobalz.sitzung.tempREC.mydb.Tabelle = "personen"
        myGlobalz.sitzung.tempREC.mydb.SQL = _
         "SELECT * FROM " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
         " where lower(gesellfunktion) like '%" & filter.ToLower & "%'" & _
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

    Shared Function suchePersonNachFilterDT_Like(ByVal filter As String) As DataTable
        myGlobalz.sitzung.tempREC.mydb.Tabelle = "stakeholder"
        myGlobalz.sitzung.tempREC.mydb.SQL = _
         "SELECT * FROM " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
         " where lower(nachName) like '%" & filter.ToLower & "%'" & _
         " order by nachName,vorname"
        nachricht(myGlobalz.sitzung.tempREC.getDataDT())
        If myGlobalz.sitzung.tempREC.mycount < 1 Then
            nachricht("Keine Personen gespeichert!")
            Return Nothing
        Else
            nachricht(String.Format("{0} Personen vorhanden", myGlobalz.sitzung.tempREC.mycount))
            Return myGlobalz.sitzung.tempREC.dt
        End If
    End Function
  

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
        Dim hinweis As String, ortfilter As String
        '   " `ID` AS `PersonenID` " & _
        If String.IsNullOrEmpty(FilterOrt) Then
            ortfilter$ = ""
        Else
            ortfilter$ = " and (FELD22='" & FilterOrt & "') "
        End If
        If myGlobalz.sitzung.probaugREC.mydb.dbtyp = "mysql" Then
            myGlobalz.sitzung.probaugREC.mydb.SQL = "select distinct " & _
         " `AS_nachname` AS `NACHNAME` " & _
         ",`AS_vorname` AS `VORNAME`" & _
         ",`AS_Titel` AS `NAMENSZUSATZ` " & _
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
         "  trim(FELD20)  AS  NACHNAME  " & _
         ", trim(FELD19)  AS  VORNAME " & _
         ", trim(FELD17)  AS  NAMENSZUSATZ  " & _
         ", trim(FELD24)  AS  GEMEINDENAME  " & _
         ", trim(FELD21)  AS  STRASSE  " & _
         ", trim(FELD23)  AS  PLZ  " & _
         ", trim(FELD22)  AS  HAUSNR  " & _
         ", trim(FELD2) ||  ' ' || trim(FELD1) || ' ' || trim(FELD3) || ' ' || trim(FELD4) || ' '|| trim(FELD5) || ' ' || trim(FELD10) as   BEMERKUNG  " & _
         " from   " & myGlobalz.probaug_MYDB.Tabelle & "    " + _
         " where (upper(trim(feld20)) like '%" & filterName.ToUpper & "%'" & _
         " or  upper(trim(feld19)) like '%" & filterName.ToUpper & "%')" & _
         " and (" &
            " upper(trim(feld1)) = '2010' or " &
            " upper(trim(feld1)) = '2011' or " &
            " upper(trim(feld1)) = '2012' or " &
            " upper(trim(feld1)) = '2013' or " &
            " upper(trim(feld1)) = '2014' or " &
            " upper(trim(feld1)) = '2015' or " &
            " upper(trim(feld1)) = '2016') " & _
         ortfilter &
          "  order by BEMERKUNG desc"
            '"  order by name,vorname,as_ort limit 500"
            '				 ", CONCAT(CAST(Jahr as CHAR),'_',CAST(laufnr as CHAR),'_',vorhaben1,'_',vorhaben2,'_',vorhabensmerkmal) AS `Bemerkung` " & _
            '    myGlobalz.sitzung.probaugREC.mydb.SQL = "select * " & _
            '    " from  " & myGlobalz.probaug_MYDB.Tabelle & "    " + _
            '" where (upper(feld17) like '%" & filterName.ToUpper & "%'" & _
            '" or  upper(feld18) like '%" & filterName.ToUpper & "%')" & _
            '  " and ( upper(feld1) = '2011' or  upper(feld1) = '2010') " & _
            'ortfilter &
            ' "  order by feld17,feld18 desc"
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

 

    Public Shared Function KontaktIDlisteVonPersonErstellen(ByVal personenID As Integer) As Boolean
        Dim hinweis As String
        myGlobalz.sitzung.tempREC.mydb.Tabelle = "person2kontakt"
        myGlobalz.sitzung.tempREC.mydb.SQL = "SELECT * FROM " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
         " where personenid =" & personenID%
        hinweis = myGlobalz.sitzung.tempREC.getDataDT()
        If myGlobalz.sitzung.tempREC.mycount < 1 Then
            nachricht("Keine adressen zur Personen gespeichert!" & personenID%)
            Return False
        Else
            nachricht(String.Format("{0} adresslisteVonPersonErstellen vorhanden", myGlobalz.sitzung.tempREC.mycount))
            Return True
        End If
    End Function

    Public Shared Function kontaktliste4KontaktIDListe(ByVal sql As String) As Boolean
        Dim hinweis As String
        myGlobalz.sitzung.tempREC.mydb.Tabelle = "Kontaktdaten"
        myGlobalz.sitzung.tempREC.mydb.SQL = sql$
        hinweis = myGlobalz.sitzung.tempREC.getDataDT()
        If myGlobalz.sitzung.tempREC.mycount < 1 Then
            nachricht("kontaktliste4KontaktIDListe!" & sql$)
            Return False
        Else
            nachricht(String.Format("{0} kontaktliste4KontaktIDListe vorhanden", myGlobalz.sitzung.tempREC.mycount))
            Return True
        End If
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

    Shared Function suchePersonNachRolleDT_inVorgangsDB(ByVal filter As String) As DataTable
        '   dim hinweis as string 
        filter = filter.ToLower
        myGlobalz.sitzung.vorgangsbeteiligteAuswahlREC.mydb.SQL =
         "select * from " &
         " stakeholder " & _
         " where upper(gesellfunktion) like '%" & filter.ToUpper & "%'" &
         " or upper(rolle) like '%" & filter.ToUpper & "%'" &
         " order by nachname,vorname,gemeindename,gesellfunktion"
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


    Shared Function suchePersonNachFilterDT_inVorgangsDB(ByVal filter As String) As DataTable
        '   dim hinweis as string 
        filter = filter.ToLower
        myGlobalz.sitzung.vorgangsbeteiligteAuswahlREC.mydb.SQL =
         "select distinct anrede,namenszusatz,nachname,vorname,plz,gemeindename,Strasse,Hausnr,orgname," &
                        "orgzusatz,FFTelefon1,FFTelefon2,FFFax1,FFFax2,FFMobilfon,FFhomepage,FFemail, gesellfunktion,bezirk from " &
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

    Shared Sub DBholeAdressenFuerDatagridStrasse()
        myGlobalz.sitzung.tempREC2.mydb.Tabelle = "paraadresse"
        '  myGlobalz.sitzung.tempREC.mydb.SQL = "select * from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & " "

        If myGlobalz.sitzung.tempREC2.mydb.dbtyp = "oracle" Then
            myGlobalz.sitzung.tempREC2.mydb.SQL = String.Format("select ad.gemeindetext,ad.strassenname,ad.hausnrkombi,v.beschreibung,v.vorgangsid," &
                                                    " v.az2, v.eingang, v.letztebearbeitung, v.lastactionheroe, v.id,v.altaz,v.probaugaz,v.sachgebietnr, " &
                                                    " v.stellungnahme,v.ortstermin,v.erledigt " &
                                                    " from paraadresse ad ,vsk_d v,pa_sekid2vid k " &
                                                    " where gemeindenr = {0}  and lower(Strassenname) = '{1}'  " &
                                                    " and ad.id=k.sekid " &
                                                    " and k.vorgangsid=v.vorgangsid " &
                                                    " order by to_number(regexp_substr(hausnrkombi,'^[0-9]+')),to_number(regexp_substr(hausnrkombi,'$[0-9]+'))",
                            myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(),
                            myGlobalz.sitzung.aktADR.Gisadresse.strasseName().ToLower)
        End If

        Dim resultdt As New System.Data.DataTable
        Dim erfolg As Integer = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.tempREC2.mydb.SQL,
                                                                               myGlobalz.sitzung.tempREC2.mydb.Tabelle,
                                                                               resultdt)
        If resultdt IsNot Nothing Then myGlobalz.sitzung.tempREC2.dt = resultdt.Copy
    End Sub

    Shared Sub DBholeAdressenFuerDatagridHNR()
        myGlobalz.sitzung.tempREC2.mydb.Tabelle = "paraadresse"
        If myGlobalz.sitzung.tempREC2.mydb.dbtyp = "oracle" Then
            myGlobalz.sitzung.tempREC2.mydb.SQL = String.Format("select ad.gemeindetext,ad.strassenname,ad.hausnrkombi,v.beschreibung,v.vorgangsid," &
                                                    " v.az2, v.eingang, v.letztebearbeitung, v.lastactionheroe, v.id,v.altaz,v.probaugaz,v.sachgebietnr, " &
                                                    " v.stellungnahme,v.ortstermin,v.erledigt " &
                                                    " from paraadresse ad ,vsk_d v,pa_sekid2vid k " &
                                                    " where gemeindenr = {0}  and lower(Strassenname) = '{1}'    and lower(hausnrkombi) = '{2}'  " &
                                                    " and ad.id=k.sekid " &
                                                    " and k.vorgangsid=v.vorgangsid " &
                                                    " order by to_number(regexp_substr(hausnrkombi,'^[0-9]+')),to_number(regexp_substr(hausnrkombi,'$[0-9]+'))",
                            myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(),
                            myGlobalz.sitzung.aktADR.Gisadresse.strasseName().ToLower,
                            myGlobalz.sitzung.aktADR.Gisadresse.HausKombi.ToLower)
        End If

        Dim resultdt As New System.Data.DataTable
        Dim erfolg As Integer = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.tempREC2.mydb.SQL,
                                                                               myGlobalz.sitzung.tempREC2.mydb.Tabelle,
                                                                               resultdt)
        If resultdt IsNot Nothing Then myGlobalz.sitzung.fstREC.dt = resultdt.Copy
    End Sub

    Shared Sub holeStrasseDTausLageschluessel(buchstabe As String)
        myGlobalz.sitzung.haloREC.mydb.Schema = "halosort"
        myGlobalz.sitzung.haloREC.mydb.Tabelle = "lageschluessel"

        If String.IsNullOrEmpty(buchstabe) Then
            myGlobalz.sitzung.haloREC.mydb.SQL = "select distinct schluesselGesamt as strcode,bezeichnung as sname,'lage' from  halosort." &
                                  myGlobalz.sitzung.haloREC.mydb.Tabelle & _
                " where gemeinde = " & myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNr() & _
                " order by	bezeichnung asc"
        Else
            myGlobalz.sitzung.haloREC.mydb.SQL = "select distinct schluesselGesamt as strcode,bezeichnung as sname,'lage' from halosort." &
                              myGlobalz.sitzung.haloREC.mydb.Tabelle & _
            " where gemeinde = " & myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNr() & _
            " and bezeichnung like '" & buchstabe & "%'" &
            " order by	bezeichnung asc"
        End If

        nachricht(myGlobalz.sitzung.haloREC.getDataDT())
    End Sub



End Class