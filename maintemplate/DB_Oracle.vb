'Imports MySql.Data.MySqlClient
'Imports System.Data

'Imports System.Data.OracleClient
Imports LibDB
Imports System.Data

Public Class DB_Oracle_sharedfunctions
        Public Shared Function leseStammdaten(ByVal myneREC As IDB_grundfunktionen, ByVal vid As Integer) As Boolean  'myGlobalz.sitzung.VorgangsID
        'myneREC.mydb.Tabelle ="stammdaten"
        'Return getDT_("", vid, myneREC)

        'myneREC.mydb.SQL = "select * fromstammdaten where VorgangsID=" & vid
        myneREC.mydb.SQL = "select * from " & CLstart.myViewsNTabs.tabStammdaten & " where VorgangsID=" & vid

        Dim hinweis As String = myneREC.getDataDT()
        If myneREC.dt.IsNothingOrEmpty Then
            l("Fatal Error ID " & "konnte nicht gefunden werden!" & myneREC.mydb.getDBinfo(""))
            Return False
        Else
            Return True
        End If

    End Function


    Public Shared Function leseWiedervorlageRecord(ByVal myneREC As IDB_grundfunktionen, ByVal wvlid As Integer) As Boolean
        'myneREC.mydb.Tabelle ="WV"
        'getDT_("id", wvlid, myneREC)

        myneREC.mydb.SQL = "select * from " & CLstart.myViewsNTabs.tabWV & "  where id=" & wvlid
        Dim hinweis As String = myneREC.getDataDT()
        If myneREC.dt.IsNothingOrEmpty Then
            l("Fatal Error ID " & "konnte nicht gefunden werden!" & myneREC.mydb.getDBinfo(""))
            Return False
        Else
            Return True
        End If
    End Function
    'Public Shared Function getDT_(ByVal idspalte$, ByVal idwert%, ByVal myneREC As IDB_grundfunktionen) As Boolean   'myGlobalz.sitzung.VorgangREC
    '    Try
    '        If String.IsNullOrEmpty(idspalte$) Then
    '            idspalte$ = "VorgangsID"
    '        End If
    '        myneREC.mydb.SQL = _
    '         "select * from " & myneREC.mydb.Tabelle & _
    '         " where " & idspalte$ & "=" & idwert%
    '        Dim hinweis As String = myneREC.getDataDT()
    '        nachricht("  getDT_: " & vbCrLf & myneREC.mydb.SQL)
    '        If myneREC.dt.IsNothingOrEmpty Then
    '            My.Log.WriteEntry("Fatal Error ID " & "konnte nicht gefunden werden!" & myneREC.mydb.getDBinfo(""))
    '            Return False
    '        End If
    '        Return True
    '    Catch ex As Exception
    '        nachricht("FEhler: getDT_" & vbCrLf & vbCrLf & ex.ToString)
    '        Return False
    '    End Try
    'End Function



#Region "RB_adresse"
    Public Shared Sub hole_AdressKoordinatenExtracted(ByVal halo_id As integer)
        myGlobalz.sitzung.postgresREC.mydb.SQL = "select rechts,hoch,fs,strcode,gemeindenr,hausnr,Zusatz from flurkarte.halofs " &
         " where id = " & halo_id
        Dim hinweis As String = myGlobalz.sitzung.postgresREC.getDataDT()
    End Sub

    Public Shared Sub hole_AdressKoordinaten_bynames()
        myGlobalz.sitzung.postgresREC.mydb.SQL = "select rechts,hoch from flurkarte.halofs " &
         " where gemeindenr = " & myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig() &
         " and sname = '" & myGlobalz.sitzung.aktADR.Gisadresse.strasseName() & "'" &
         " and hausnr = " & myGlobalz.sitzung.aktADR.Gisadresse.hausNr &
         " and zusatz = '" & myGlobalz.sitzung.aktADR.Gisadresse.hausZusatz & "'"
        nachricht(myGlobalz.sitzung.postgresREC.getDataDT())
    End Sub

    Public Shared Sub DBholeHausnrDT()
        myGlobalz.sitzung.postgresREC.mydb.SQL = String.Format("select distinct id ,cast(concat(hausnr,zusatz) as text) as hausnrkombi " &
                                                           "from flurkarte.halofs  where gemeindenr = {0} and strcode = {1} order by  hausnr,zusatz",
                                                           myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(), myGlobalz.sitzung.aktADR.Gisadresse.strasseCode())
        nachricht(myGlobalz.sitzung.postgresREC.getDataDT())
    End Sub
     
    Private Shared Sub erstelleSQLfuerhausnr_alledb2(iDB_grundfunktionen As IDB_grundfunktionen)
        If myglobalz.vorgang_MYDB.dbtyp = "mysql" Then
            iDB_grundfunktionen.mydb.SQL =
         String.Format("select distinct HAUSNRKOMBI  from " & CLstart.myViewsNTabs.tabPARAADRESSE & "  where gemeindenr = {0} and lower(Strassenname) = '{1}' order by abs(hausnrkombi)",
                        myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(),
                      myGlobalz.sitzung.aktADR.Gisadresse.strasseName().ToLower)


        End If
        If myglobalz.vorgang_MYDB.dbtyp = "sqls" Then
            iDB_grundfunktionen.mydb.SQL =
         String.Format("select distinct HAUSNRKOMBI  from " & CLstart.myViewsNTabs.tabPARAADRESSE & "  where gemeindenr = {0} and lower(Strassenname) = '{1}' order by hausnrkombi",
                       myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(),
                      myGlobalz.sitzung.aktADR.Gisadresse.strasseName().ToLower)


        End If
        If myGlobalz.vorgang_MYDB.dbtyp = "oracle" Then
            iDB_grundfunktionen.mydb.SQL =
         String.Format("select distinct HAUSNRKOMBI  from " & CLstart.myViewsNTabs.tabPARAADRESSE & "  where gemeindenr = {0} and lower(Strassenname) = '{1}'" &
                       " order by to_number(regexp_substr(hausnrkombi,'^[0-9]+')),to_number(regexp_substr(hausnrkombi,'$[0-9]+'))",
                    myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(),
                        myGlobalz.sitzung.aktADR.Gisadresse.strasseName().ToLower)
        End If
    End Sub

    Public Shared Sub DBholeHausnrDTVorgaenge_alledb()
        'myGlobalz.sitzung.tempREC.mydb.Tabelle ="paraadresse" ' abs(hausnrkombi)in mysql
        erstelleSQLfuerhausnr_alledb2(myglobalz.sitzung.tempREC)

        Dim resultdt As New System.Data.DataTable
        Dim erfolg As Integer = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.tempREC.mydb.SQL,
                                                                               resultdt)
        If resultdt IsNot Nothing Then myGlobalz.sitzung.tempREC.dt = resultdt.Copy
    End Sub

    Public Shared Sub holeStrasseDT4Vorgaenge()
        'myglobalz.sitzung.tempREC.mydb.Tabelle ="paraadresse"
        'oracle version sql-------------
        myGlobalz.sitzung.tempREC.mydb.SQL =
            String.Format("select distinct trim(strassenname)  as SNAME from " & CLstart.myViewsNTabs.tabPARAADRESSE & "  where gemeindenr = {0} order by trim(strassenname) asc",
                                                        myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig())
        'mssql version sql-------------
        myGlobalz.sitzung.tempREC.mydb.SQL =
            String.Format("select distinct   ltrim(rTrim(strassenname))  as SNAME from " & CLstart.myViewsNTabs.tabPARAADRESSE & "  where gemeindenr = {0} order by   ltrim(rTrim(strassenname)) asc",
                                                           myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig())
        Dim resultdt As New System.Data.DataTable
        Dim erfolg As Integer = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.tempREC.mydb.SQL, resultdt)
        myGlobalz.sitzung.tempREC.dt = resultdt.Copy

    End Sub



    Public Shared Sub holeStrasseDTausHalo(buchstabe As String) 
        If String.IsNullOrEmpty(buchstabe) Then
            myGlobalz.sitzung.postgresREC.mydb.SQL = "select distinct strcode ,sname,'halo' from flurkarte.halofs " &
                " where gemeindenr = " & myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig() &
                " order by	sname asc"
        Else
            myGlobalz.sitzung.postgresREC.mydb.SQL = "select distinct strcode ,sname,'halo' from flurkarte.halofs " &
        " where gemeindenr = " & myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig() &
        " and lower(sname) like '" & buchstabe.Trim & "%'" &
        " order by	sname asc "
        End If

        nachricht(myGlobalz.sitzung.postgresREC.getDataDT())
    End Sub

    Public Shared Sub holeZaehlerDTinvorgaengen()
        myGlobalz.sitzung.tempREC.mydb.SQL = "select distinct ZAEHLER  from " & CLstart.myViewsNTabs.tabPARAFLURSTUECK & " " &
         " where gemcode = " & myGlobalz.sitzung.aktFST.normflst.gemcode &
         " and flur = " & myGlobalz.sitzung.aktFST.normflst.flur &
         " order by zaehler  "
        nachricht(myGlobalz.sitzung.tempREC.getDataDT())
    End Sub

    'Public Shared Sub holeZaehlerDT()
    '    myglobalz.sitzung.postgresREC.mydb.SQL = "select distinct ZAEHLER from flurkarte.basis_f " &
    '     myglobalz.sitzung.postgresREC.mydb.Tabelle &
    '     " where gemarkung = " & myglobalz.sitzung.aktFST.normflst.gemcode &
    '     " and flur = " & myglobalz.sitzung.aktFST.normflst.flur &
    '     " order by zaehler  "
    '    nachricht(myGlobalz.sitzung.postgresREC.getDataDT())
    'End Sub

    Public Shared Sub holeNennerDTinVorgaengen()
        myGlobalz.sitzung.tempREC.mydb.SQL = "select distinct NENNER from " & CLstart.myViewsNTabs.tabPARAFLURSTUECK & " " &
         " where gemcode = " & myGlobalz.sitzung.aktFST.normflst.gemcode &
         " and flur = " & myGlobalz.sitzung.aktFST.normflst.flur &
         " and zaehler = " & myGlobalz.sitzung.aktFST.normflst.zaehler &
         " order by nenner  "
        nachricht(myGlobalz.sitzung.tempREC.getDataDT())
    End Sub

    Public Shared Sub holeSekidListeDTinVorgaengenZuFlurstueckSQL()
        myGlobalz.sitzung.tempREC.mydb.SQL = "select distinct id from " & CLstart.myViewsNTabs.tabPARAFLURSTUECK & " " &
         " where gemcode = " & myGlobalz.sitzung.aktFST.normflst.gemcode &
         " and flur = " & myGlobalz.sitzung.aktFST.normflst.flur &
         " and zaehler = " & myGlobalz.sitzung.aktFST.normflst.zaehler &
         " and nenner = " & myGlobalz.sitzung.aktFST.normflst.nenner &
         " order by id  "
        nachricht(myGlobalz.sitzung.tempREC.getDataDT())
    End Sub

    Public Shared Sub holeSekidListeDTinVorgaengenZuZaehlerSQL()
        myGlobalz.sitzung.tempREC.mydb.SQL = "select distinct id from " & CLstart.myViewsNTabs.tabPARAFLURSTUECK & " " &
         " where gemcode = " & myGlobalz.sitzung.aktFST.normflst.gemcode &
         " and flur = " & myGlobalz.sitzung.aktFST.normflst.flur &
         " and zaehler = " & myGlobalz.sitzung.aktFST.normflst.zaehler &
         " order by id  "
        nachricht(myglobalz.sitzung.tempREC.getDataDT())
    End Sub

    Public Shared Sub holeSekidListeDTinVorgaengenZuFlurSQL()
        myGlobalz.sitzung.tempREC.mydb.SQL = "select distinct id from " & CLstart.myViewsNTabs.tabPARAFLURSTUECK & " " &
         " where gemcode = " & myGlobalz.sitzung.aktFST.normflst.gemcode &
         " and flur = " & myGlobalz.sitzung.aktFST.normflst.flur &
         " order by id  "
        nachricht(myglobalz.sitzung.tempREC.getDataDT())
    End Sub

    Public Shared Sub holeNennerDT()
        myGlobalz.sitzung.postgresREC.mydb.SQL = "select distinct nenner from  flurkarte.basis_f " &  
         " where gemarkung = " & myGlobalz.sitzung.aktFST.normflst.gemcode & _
         " and flur = " & myGlobalz.sitzung.aktFST.normflst.flur & _
         " and zaehler = " & myGlobalz.sitzung.aktFST.normflst.zaehler & _
         " order by nenner  "
        nachricht(myGlobalz.sitzung.postgresREC.getDataDT())
    End Sub

    Public Shared Sub holeFlureDT()
        myGlobalz.sitzung.postgresREC.mydb.SQL = "select distinct flur from   flurkarte.basis_f " &  
         " where gemarkung = " & myGlobalz.sitzung.aktFST.normflst.gemcode & _
         " order by flur "
        nachricht(myGlobalz.sitzung.postgresREC.getDataDT())
    End Sub



    
#End Region



    Public Shared Sub hole_FSTKoordinatenExtracted(ByVal FS$)
        myGlobalz.sitzung.postgresREC.mydb.SQL = "select * from   flurkarte.basis_f "   & 
                                " where FS = '" & FS$ & "'"
        nachricht(myGlobalz.sitzung.postgresREC.getDataDT())
        ' rechts,hoch,fs,area,fsgml,zeigtauf,weistauf,gebucht from " & _
    End Sub

#Region "Beteiligte"

    'Public Shared Function Entkoppelung_Vorgang_Person(ByVal vorgangID%, ByVal personenID As Integer) As Integer
    '    Dim anzahlTreffer&
    '    Dim newid& = -1
    '    Try
    '        myGlobalz.sitzung.tempREC = CType(CType(myGlobalz.sitzung.VorgangREC, clsDBspecMYSQL), IDB_grundfunktionen)
    '        myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
    '        myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
    '        myGlobalz.sitzung.tempREC.mydb.Tabelle ="Person2vorgang"
    '        myGlobalz.sitzung.tempREC.mydb.SQL = _
    '         "delete from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & " where " & _
    '         "     Vorgangsid=" & vorgangID% & _
    '         " and PersonenID=" & personenID%
    '        anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ', myGlobalz.mylog)
    '        If anzahlTreffer < 1 Then
    '            nachricht_und_Mbox("Problem beim Abspeichern3:" & myGlobalz.sitzung.tempREC.mydb.SQL)
    '            Return -1
    '        Else
    '            Return CInt(anzahlTreffer)
    '        End If
    '    Catch ex As Exception
    '        nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
    '        Return -2
    '    End Try
    'End Function

    Public Shared Function leseEinzelPersonViaPersonenID() As Boolean
        Dim hinweis As String
        'myGlobalz.sitzung.beteiligteREC.mydb.Tabelle ="personen"
        myGlobalz.sitzung.beteiligteREC.mydb.SQL = "SELECT * FROM personen where PersonenID=" & myGlobalz.sitzung.aktPerson.PersonenID
        hinweis = myGlobalz.sitzung.beteiligteREC.getDataDT()
        If myGlobalz.sitzung.beteiligteREC.mycount < 1 Then
            nachricht("Keine Personen gespeichert!")
            Return False
        Else
            nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.beteiligteREC.mycount))
            Return True
        End If
    End Function



#End Region

    Shared Function suchePersonNachFilterDT_Like(ByVal filter As String) As DataTable
        'myGlobalz.sitzung.tempREC.mydb.Tabelle ="stakeholder"
        myGlobalz.sitzung.tempREC.mydb.SQL =
         "SELECT * FROM stakeholder " &
         " where lower(nachName) like '%" & filter.ToLower & "%'" &
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
        'myGlobalz.sitzung.tempREC.mydb.Tabelle ="paraadresse"
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
        'myGlobalz.sitzung.tempREC.mydb.Tabelle ="person2adresse"
        myGlobalz.sitzung.tempREC.mydb.SQL = "SELECT * FROM person2adresse " &
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

 

 




 
 

    Shared Function initBankkontoDT(ByVal BankkontoID As Integer) As Boolean
        If BankkontoID% < 1 Then
            My.Log.WriteEntry("	initBankkontoDT: ist ist kleiner 1")
            Return False
        End If
        Try
            'myGlobalz.sitzung.tempREC.mydb.Tabelle ="bankverbindung"
            myGlobalz.sitzung.tempREC.mydb.SQL = "select * from " & CLstart.myViewsNTabs.tabbankverbindung & "  where BankkontoID=" & BankkontoID%
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
        'myglobalz.sitzung.tempREC.mydb.Tabelle ="paraadresse"
        '  myGlobalz.sitzung.tempREC.mydb.SQL = "select * from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & " "

        If myglobalz.sitzung.tempREC.mydb.dbtyp = "oracle" Then
            myGlobalz.sitzung.tempREC.mydb.SQL = String.Format("select ad.gemeindetext,ad.strassenname,ad.hausnrkombi,v.beschreibung,v.vorgangsid," &
                                                    " v.az2, v.eingang, v.letztebearbeitung, v.lastactionheroe, v.id,v.altaz,v.probaugaz,v.sachgebietnr, " &
                                                    " v.stellungnahme,v.ortstermin,v.erledigt " &
                                                    " from " & CLstart.myViewsNTabs.tabPARAADRESSE & "  ad ,(" & CLstart.myViewsNTabs.view_vsk_d & ") v,pa_sekid2vid k " &
                                                    " where gemeindenr = {0}  and lower(Strassenname) = '{1}'  " &
                                                    " and ad.id=k.sekid " &
                                                    " and k.vorgangsid=v.vorgangsid " &
                                                    " order by to_number(regexp_substr(hausnrkombi,'^[0-9]+')),to_number(regexp_substr(hausnrkombi,'$[0-9]+'))",
                            myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(),
                            myGlobalz.sitzung.aktADR.Gisadresse.strasseName().ToLower)
        End If
        If myglobalz.sitzung.tempREC.mydb.dbtyp = "sqls" Then
            myGlobalz.sitzung.tempREC.mydb.SQL = String.Format("SELECT AD.GEMEINDETEXT,AD.STRASSENNAME,AD.HAUSNRKOMBI,V.BESCHREIBUNG,V.VORGANGSID," &
                                                    " V.AZ2, V.EINGANG, V.LETZTEBEARBEITUNG, V.LASTACTIONHEROE, V.ID,V.ALTAZ,V.PROBAUGAZ,V.SACHGEBIETNR, " &
                                                    " V.STELLUNGNAHME,V.ORTSTERMIN,V.ERLEDIGT " &
                                                    " FROM " & CLstart.myViewsNTabs.tabPARAADRESSE & "  AD ,(" & CLstart.myViewsNTabs.view_vsk_d & ") V,PA_SEKID2VID K " &
                                                    " WHERE GEMEINDENR = {0}  AND LOWER(STRASSENNAME) = '{1}'  " &
                                                    " AND AD.ID=K.SEKID " &
                                                    " AND K.VORGANGSID=V.VORGANGSID " &
                                                    "  ",
                            myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(),
                            myGlobalz.sitzung.aktADR.Gisadresse.strasseName().ToLower)
        End If

        Dim resultdt As New System.Data.DataTable
        Dim erfolg As Integer = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.tempREC.mydb.SQL,
                                                                               resultdt)
        If resultdt IsNot Nothing Then myGlobalz.sitzung.tempREC.dt = resultdt.Copy
    End Sub

    Shared Sub DBholeAdressenFuerDatagridHNR()
        'myglobalz.sitzung.tempREC.mydb.Tabelle ="paraadresse"
        If myglobalz.sitzung.tempREC.mydb.dbtyp = "oracle" Then
            myGlobalz.sitzung.tempREC.mydb.SQL = String.Format("select ad.gemeindetext,ad.strassenname,ad.hausnrkombi,v.beschreibung,v.vorgangsid," &
                                                    " v.az2, v.eingang, v.letztebearbeitung, v.lastactionheroe, v.id,v.altaz,v.probaugaz,v.sachgebietnr, " &
                                                    " v.stellungnahme,v.ortstermin,v.erledigt " &
                                                    " from " & CLstart.myViewsNTabs.tabPARAADRESSE & "  ad ,(" & CLstart.myViewsNTabs.view_vsk_d & ") v,pa_sekid2vid k " &
                                                    " where gemeindenr = {0}  and lower(Strassenname) = '{1}'    and lower(hausnrkombi) = '{2}'  " &
                                                    " and ad.id=k.sekid " &
                                                    " and k.vorgangsid=v.vorgangsid " &
                                                    " order by to_number(regexp_substr(hausnrkombi,'^[0-9]+')),to_number(regexp_substr(hausnrkombi,'$[0-9]+'))",
                            myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(),
                            myGlobalz.sitzung.aktADR.Gisadresse.strasseName().ToLower,
                            myGlobalz.sitzung.aktADR.Gisadresse.HausKombi.ToLower)
        End If
        If myglobalz.sitzung.tempREC.mydb.dbtyp = "sqls" Then
            myGlobalz.sitzung.tempREC.mydb.SQL = String.Format("SELECT AD.GEMEINDETEXT,AD.STRASSENNAME,AD.HAUSNRKOMBI,V.BESCHREIBUNG,V.VORGANGSID," &
                                                    " V.AZ2, V.EINGANG, V.LETZTEBEARBEITUNG, V.LASTACTIONHEROE, V.ID,V.ALTAZ,V.PROBAUGAZ,V.SACHGEBIETNR, " &
                                                    " V.STELLUNGNAHME,V.ORTSTERMIN,V.ERLEDIGT " &
                                                    " FROM " & CLstart.myViewsNTabs.tabPARAADRESSE & "  AD ,(" & CLstart.myViewsNTabs.view_vsk_d & ") V,PA_SEKID2VID K " &
                                                    " where gemeindenr = {0}  and lower(Strassenname) = '{1}'    and lower(hausnrkombi) = '{2}'  " &
                                                    " and ad.id=k.sekid " &
                                                    " and k.vorgangsid=v.vorgangsid " &
                                                    " order by  hausnrkombi ",
                            myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(),
                            myGlobalz.sitzung.aktADR.Gisadresse.strasseName().ToLower,
                            myGlobalz.sitzung.aktADR.Gisadresse.HausKombi.ToLower)
        End If
        Dim resultdt As New System.Data.DataTable
        Dim erfolg As Integer = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.tempREC.mydb.SQL,
                                                                               resultdt)
        If resultdt IsNot Nothing Then myglobalz.sitzung.tempREC.dt = resultdt.Copy
    End Sub

    Shared Sub holeStrasseDTausLageschluessel(buchstabe As String)
        'myGlobalz.sitzung.postgresREC.mydb.Schema = "halosort"
        'myGlobalz.sitzung.postgresREC.mydb.Tabelle ="lageschluessel"

        If String.IsNullOrEmpty(buchstabe) Then
            myGlobalz.sitzung.postgresREC.mydb.SQL = "select distinct schluesselGesamt as strcode,bezeichnung as sname,'lage' from  flurkarte.lageschluessel " &
                " where gemeinde = " & myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNr() &
                " order by	bezeichnung asc"
        Else
            myGlobalz.sitzung.postgresREC.mydb.SQL = "select distinct schluesselGesamt as strcode,bezeichnung as sname,'lage' from flurkarte.lageschluessel" &
            " where gemeinde = " & myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNr() &
            " and bezeichnung like '" & buchstabe & "%'" &
            " order by	bezeichnung asc"
        End If

        nachricht(myGlobalz.sitzung.postgresREC.getDataDT())
    End Sub



End Class