#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB.LIBDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.

Imports System.Data

Public Class DB_Oracle_sharedfunctions
    Public Shared Function leseStammdaten(ByVal myneREC As IDB_grundfunktionen, ByVal vid As Integer) As Boolean  'myGlobalz.sitzung.VorgangsID
        'myneREC.mydb.Tabelle ="stammdaten"
        myneREC.mydb.SQL = "select * from " & CLstart.myViewsNTabs.tabStammdaten & " where VorgangsID=" & vid
        Dim hinweis As String = myneREC.getDataDT()
        If myneREC.dt.IsNothingOrEmpty Then
            l("Fatal Error ID " & "konnte nicht gefunden werden!" & myneREC.mydb.getDBinfo(""))
            Return False
        Else
            Return True
        End If
        'Return getDT_("", vid, myneREC)
    End Function
    Public Shared Function leseWiedervorlageRecord(ByVal myneREC As IDB_grundfunktionen, ByVal wvlid As Integer) As Boolean
        'myneREC.mydb.Tabelle ="WV"
        'getDT_("id", wvlid, myneREC)
        'Return True 
        myneREC.mydb.SQL = "select * from " & CLstart.myViewsNTabs.tabWV & "  where id=" & wvlid
        Dim hinweis As String = myneREC.getDataDT()
        If myneREC.dt.IsNothingOrEmpty Then
            l("Fatal Error ID " & "konnte nicht gefunden werden!" & myneREC.mydb.getDBinfo(""))
            Return False
        Else
            Return True
        End If
    End Function


    'Public Shared Function getDT_(ByVal idspalte As String, ByVal idwert As Integer, ByVal myneREC As IDB_grundfunktionen) As Boolean   'myGlobalz.sitzung.VorgangREC
    '    Try
    '        If String.IsNullOrEmpty(idspalte$) Then
    '            idspalte$ = "VorgangsID"
    '        End If
    '        myneREC.mydb.SQL =
    '         "select * from " & myneREC.mydb.Tabelle &
    '         " where " & idspalte & "=" & idwert
    '        Dim hinweis As String = myneREC.getDataDT()
    '        nachricht("  getDT_: " & vbCrLf & myneREC.mydb.SQL)
    '        If myneREC.dt.IsNothingOrEmpty Then
    '            l("Fatal Error ID " & "konnte nicht gefunden werden!" & myneREC.mydb.getDBinfo(""))
    '            Return False
    '        End If
    '        Return True
    '    Catch ex As Exception
    '        nachricht("FEhler: getDT_" & vbCrLf & vbCrLf ,ex)
    '        Return False
    '    End Try
    'End Function

    Public Shared Sub hole_AdressKoordinatenExtracted(ByVal halo_id As String)
        myGlobalz.sitzung.postgresREC.mydb.SQL = "select rechts,hoch,fs,strcode,gemeindenr,hausnr,Zusatz from flurkarte.halofs " &
                  " where id = " & halo_id
        Dim hinweis As String = myGlobalz.sitzung.postgresREC.getDataDT()
    End Sub



    Public Shared Sub DBholeHausnrDT()
        myGlobalz.sitzung.postgresREC.mydb.SQL = String.Format("select distinct id ,cast(concat(hausnr,zusatz) as text) as hausnrkombi,hausnr,zusatz " &
                                                           "from flurkarte.halofs where gemeindenr = {0} and strcode = {1} order by  hausnr,zusatz",
                              myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(),
                             myGlobalz.sitzung.aktADR.Gisadresse.strasseCode())
        myGlobalz.sitzung.postgresREC.mydb.SQL = String.Format("select gml_id,hausnrkombi,hausnr,zusatz,rechts,hoch,gemeindetext,gemarkungstext,fs   " &
                                                           "from flurkarte.halofs where gemeindenr = {0} and strcode = {1} order by  hausnr,zusatz",
                              myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(),
                             myGlobalz.sitzung.aktADR.Gisadresse.strasseCode())
        'gml_id,hausnrkombi,hausnr,zusatz  
        nachricht(myGlobalz.sitzung.postgresREC.getDataDT())
    End Sub

    Public Shared Sub holeStrasseDT4Vorgaenge()
        'myGlobalz.sitzung.tempREC.mydb.Tabelle ="paraadresse"
        myGlobalz.sitzung.tempREC.mydb.SQL = "select distinct trim(strassenname)  as sname " &
                                                " from " & CLstart.myViewsNTabs.tabPARAADRESSE & "  where gemeindenr = " & myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig() &
                                                " order by	trim(strassenname) asc"
        Dim resultdt As New System.Data.DataTable
        Dim erfolg As Integer = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.tempREC.mydb.SQL, resultdt)
        myGlobalz.sitzung.tempREC.dt = resultdt.Copy

    End Sub

    Public Shared Sub holeStrasseDTausHalo(buchstabe As String)
        Dim a = myGlobalz.sitzung.postgresREC.mydb.tostring
        If String.IsNullOrEmpty(buchstabe) Then
            myGlobalz.sitzung.postgresREC.mydb.SQL = "select distinct strcode ,sname,'halo' as halo from flurkarte.halofs " &
                " where gemeindenr = " & myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig() &
                " order by	sname asc"
        Else
            myGlobalz.sitzung.postgresREC.mydb.SQL = "select distinct strcode ,sname,'halo' as halo from flurkarte.halofs " &
                                                " where gemeindenr = " & myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig() &
                                                " and lower(sname) like '" & buchstabe.Trim & "%'" &
                                                " order by	sname asc "
        End If
        nachricht(myGlobalz.sitzung.postgresREC.getDataDT())

    End Sub


    Public Shared Sub holeZaehlerDTinvorgaengen()
        myGlobalz.sitzung.tempREC.mydb.SQL = "select distinct zaehler  from " & CLstart.myViewsNTabs.tabPARAFLURSTUECK & " " &
         " where gemcode = " & myGlobalz.sitzung.aktFST.normflst.gemcode &
         " and flur = " & myGlobalz.sitzung.aktFST.normflst.flur &
         " order by zaehler  "
        nachricht(myGlobalz.sitzung.tempREC.getDataDT())
    End Sub

    Public Shared Sub holeZaehlerDT()
        myGlobalz.sitzung.postgresREC.mydb.SQL = "select distinct zaehler from flurkarte.basis_f " &
         " where gemcode = " & myGlobalz.sitzung.aktFST.normflst.gemcode &
         " and flur = " & myGlobalz.sitzung.aktFST.normflst.flur &
         " order by zaehler  "
        nachricht(myGlobalz.sitzung.postgresREC.getDataDT())
    End Sub

    Public Shared Sub holeNennerDTinVorgaengen()
        myGlobalz.sitzung.tempREC.mydb.SQL = "select distinct nenner from " & CLstart.myViewsNTabs.tabPARAFLURSTUECK & " " &
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

    Public Shared Sub holeNennerDT()
        myGlobalz.sitzung.postgresREC.mydb.SQL = "select distinct nenner from flurkarte.basis_f " &
         " where gemcode = " & myGlobalz.sitzung.aktFST.normflst.gemcode &
         " and flur = " & myGlobalz.sitzung.aktFST.normflst.flur &
         " and zaehler = " & myGlobalz.sitzung.aktFST.normflst.zaehler &
         " order by nenner  "
        nachricht(myGlobalz.sitzung.postgresREC.getDataDT())
    End Sub

    Public Shared Sub holeFlureDT()
        myGlobalz.sitzung.postgresREC.mydb.SQL = "select distinct flur from flurkarte.basis_f " &
         " where gemcode = " & myGlobalz.sitzung.aktFST.normflst.gemcode &
         " order by flur "
        nachricht(myGlobalz.sitzung.postgresREC.getDataDT())
    End Sub



    Public Shared Sub hole_FSTKoordinatenExtracted(ByVal FS$)
        myGlobalz.sitzung.postgresREC.mydb.SQL = "select * from flurkarte.basis_f where FS = '" & FS & "'"
        nachricht(myGlobalz.sitzung.postgresREC.getDataDT())
    End Sub

#Region "Beteiligte"



    Public Shared Function leseEinzelPersonViaPersonenID() As Boolean
        Dim hinweis As String
        'myGlobalz.sitzung.beteiligteREC.mydb.Tabelle ="personen"
        myGlobalz.sitzung.beteiligteREC.mydb.SQL = "SELECT * FROM personen " &
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
#Disable Warning BC42024 ' Unused local variable: 'personenid'.
        Dim personenid As Integer
#Enable Warning BC42024 ' Unused local variable: 'personenid'.
        Dim querie As String


        clsBeteiligteBUSI.werteDBsicherMachenBeteiligte(lpers)
        clsSqlparam.paramListe.Clear()
        'populateParamListeEreignis(zielvorgangsid, ereignis, clsSqlparam.paramListe)
        'clsSqlparam.paramListe.Add(New clsSqlparam("eid", 0))
        querie = "update  " & myGlobalz.sitzung.beteiligteREC.mydb.Tabelle &
            " SET NACHNAME=@NACHNAME" &
         ",VORNAME=@VORNAME" &
         ",BEMERKUNG=@BEMERKUNG " &
         ",NAMENSZUSATZ=@NAMENSZUSATZ " &
         ",ANREDE=@ANREDE " &
         ",QUELLE=@QUELLE " &
         ",GEMEINDENAME=@GEMEINDENAME " &
         ",STRASSE=@STRASSE " &
         ",HAUSNR=@HAUSNR " &
         ",PLZ=@PLZ" &
         ",POSTFACH=@POSTFACH" &
         ",POSTFACHPLZ=@POSTFACHPLZ" &
         ",FFTELEFON1=@FFTELEFON1 " &
         ",FFTELEFON2=@FFTELEFON2 " &
         ",FFFAX1=@FFFAX1 " &
         ",FFFAX2=@FFFAX2 " &
         ",FFMOBILFON=@FFMOBILFON " &
         ",FFEMAIL=@FFEMAIL " &
         ",FFHOMEPAGE=@FFHOMEPAGE " &
         ",GESELLFUNKTION=@GESELLFUNKTION " &
         ",ORGNAME=@ORGNAME" &
         ",ORGZUSATZ=@ORGZUSATZ" &
         ",ORGTYP1=@ORGTYP1 " &
         ",ORGTYP2=@ORGTYP2 " &
         ",ORGEIGENTUEMER=@ORGEIGENTUEMER " &
         ",ROLLE=@ROLLE " &
         ",KASSENKONTO=@KASSENKONTO " &
         ",BEZIRK=@BEZIRK " &
         ",LASTCHANGE=@LASTCHANGE " &
         ",PERSONENVORLAGE=@PERSONENVORLAGE " &
         ",VERTRETENDURCH=@VERTRETENDURCH " &
         ",VORGANGSID=@VORGANGSID " &
             " where PERSONENID=@PERSONENID"

        clsBeteiligteBUSI.populateBeteiligte(vid, lpers)
        clsSqlparam.paramListe.Add(New clsSqlparam("PERSONENID", pid))
        anzahl% = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, False, "PERSONENID")

        Return anzahl%
    End Function

#End Region


    Shared Function suchePersonNachFilterDT_LikeRolle(ByVal filter As String) As DataTable
        'myGlobalz.sitzung.tempREC.mydb.Tabelle ="personen"
        myGlobalz.sitzung.tempREC.mydb.SQL =
         "SELECT * FROM personen " &
         " where lower(gesellfunktion) like '%" & filter.ToLower & "%'" &
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
        'myGlobalz.sitzung.tempREC.mydb.Tabelle ="stakeholder" 
        If myGlobalz.sitzung.aktBearbeiter.istUser_admin_oder_vorzimmer Then
            myGlobalz.sitzung.tempREC.mydb.SQL =
                           "SELECT * FROM  " & CLstart.myViewsNTabs.tabSTAKEHOLDER & " as stakeholder " &
                           " where lower(nachName) like '%" & filter.ToLower & "%'" &
                           " order by nachName,vorname"
        Else
            myGlobalz.sitzung.tempREC.mydb.SQL =
                           "SELECT * FROM  " & CLstart.myViewsNTabs.tabSTAKEHOLDER & " as stakeholder " &
                           " where lower(nachName) like '%" & filter.ToLower & "%'" &
                           " and ROLLE<>'Mitarbeiter/in'" &
                           " order by nachName,vorname"
        End If
        'ROLLE='Mitarbeiter/in'
        nachricht(myGlobalz.sitzung.tempREC.getDataDT())
        If myGlobalz.sitzung.tempREC.mycount < 1 Then
            nachricht("Keine Personen gespeichert!")
            Return Nothing
        Else
            nachricht(String.Format("{0} Personen vorhanden", myGlobalz.sitzung.tempREC.mycount))
            Return myGlobalz.sitzung.tempREC.dt
        End If
    End Function


    'Public Shared Function adressliste4AdressIDListe(ByVal sql As String) As Boolean

    '    'myGlobalz.sitzung.tempREC.mydb.Tabelle ="paraadresse"
    '    myGlobalz.sitzung.tempREC.mydb.SQL = sql$
    '    nachricht(myGlobalz.sitzung.tempREC.getDataDT())
    '    If myGlobalz.sitzung.tempREC.mycount < 1 Then
    '        nachricht("adressliste4AdressIDListe!" & sql$)
    '        Return False
    '    Else
    '        nachricht(String.Format("{0} adressliste4AdressIDListe vorhanden", myGlobalz.sitzung.tempREC.mycount))
    '        Return True
    '    End If
    'End Function

    'Public Shared Function adressIDlisteVonPersonErstellen(ByVal personenID As Integer) As Boolean
    '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="person2adresse"
    '    myGlobalz.sitzung.tempREC.mydb.SQL = "SELECT * FROM " & myGlobalz.sitzung.tempREC.mydb.Tabelle &
    '     " where personenid =" & personenID%
    '    nachricht(myGlobalz.sitzung.tempREC.getDataDT())
    '    If myGlobalz.sitzung.tempREC.mycount < 1 Then
    '        nachricht("Keine adressen zur Personen gespeichert!" & personenID%)
    '        Return False
    '    Else
    '        nachricht(String.Format("{0} adresslisteVonPersonErstellen vorhanden", myGlobalz.sitzung.tempREC.mycount))
    '        Return True
    '    End If
    'End Function

    Shared Function suchePersonNachFilterDT_inProbaugDB(ByVal filterName$, ByVal FilterOrt As String) As DataTable
        Dim hinweis As String, ortfilter As String
        '   " `ID` AS `PersonenID` " & _
        If String.IsNullOrEmpty(FilterOrt) Then
            ortfilter$ = ""
        Else
            ortfilter$ = " and (FELD22='" & FilterOrt & "') "
        End If
        If myGlobalz.sitzung.probaugREC.mydb.dbtyp = "mysql" Then
            myGlobalz.sitzung.probaugREC.mydb.SQL = "select distinct " &
         " `AS_nachname` AS `NACHNAME` " &
         ",`AS_vorname` AS `VORNAME`" &
         ",`AS_Titel` AS `NAMENSZUSATZ` " &
         ",`AS_Ort` AS `AS_Ort` " &
         ",`AS_Strasse` AS `AS_Strasse` " &
         ",`AS_PLZ` AS `AS_PLZ` " &
         ",`AS_Hausnummer` AS `AS_Hausnummer` " &
         ",`FS` " &
         ", CONCAT(CAST(Jahr as CHAR),'_',CAST(laufnr as CHAR),'_',vorhaben1,'_',vorhaben2,'_',vorhabensmerkmal) AS `Bemerkung` " &
         " from `antraege`   " +
         " where (upper(AS_nachname) like '%" & filterName.ToUpper & "%'" &
         " or  upper(AS_vorname) like '%" & filterName.ToUpper & "%')" &
         ortfilter &
          "  order by bemerkung desc limit 100"
            '"  order by name,vorname,as_ort limit 500"
            '				 ", CONCAT(CAST(Jahr as CHAR),'_',CAST(laufnr as CHAR),'_',vorhaben1,'_',vorhaben2,'_',vorhabensmerkmal) AS `Bemerkung` " & _
        End If
        If myGlobalz.sitzung.probaugREC.mydb.dbtyp = "oracle" Then
            myGlobalz.sitzung.probaugREC.mydb.SQL = "select distinct " &
         "  trim(FELD20)  AS  NACHNAME  " &
         ", trim(FELD19)  AS  VORNAME " &
         ", trim(FELD17)  AS  NAMENSZUSATZ  " &
         ", trim(FELD24)  AS  GEMEINDENAME  " &
         ", trim(FELD21)  AS  STRASSE  " &
         ", trim(FELD23)  AS  PLZ  " &
         ", trim(FELD22)  AS  HAUSNR  " &
         ", trim(FELD2) ||  ' ' || trim(FELD1) || ' ' || trim(FELD3) || ' ' || trim(FELD4) || ' '|| trim(FELD5) || ' ' || trim(FELD10) as   BEMERKUNG  " &
         " from   " & myGlobalz.probaug_MYDB.Tabelle & "    " +
         " where (upper(trim(feld20)) like '%" & filterName.ToUpper & "%'" &
         " or  upper(trim(feld19)) like '%" & filterName.ToUpper & "%')" &
         " and (" &
            " upper(trim(feld1)) = '" & Format(DateTime.Now.AddYears(-1), "yyyy") & "' or " &
            " upper(trim(feld1)) = '" & Format(Now, "yyyy") & "'   " &
            " ) " &
         ortfilter &
          "  order by BEMERKUNG desc"
        End If
        If myGlobalz.sitzung.probaugREC.mydb.dbtyp = "sqls" Then
            myGlobalz.sitzung.probaugREC.mydb.SQL = "select distinct " &
             "  RTRIM(LTRIM(FELD20))  AS  NACHNAME  " &
             ", RTRIM(LTRIM(FELD19))  AS  VORNAME " &
             ", RTRIM(LTRIM(FELD17))  AS  NAMENSZUSATZ  " &
             ", RTRIM(LTRIM(FELD24))  AS  GEMEINDENAME  " &
             ", RTRIM(LTRIM(FELD21))  AS  STRASSE  " &
             ", RTRIM(LTRIM(FELD23))  AS  PLZ  " &
             ", RTRIM(LTRIM(FELD22))  AS  HAUSNR  " &
             ", concat(RTRIM(LTRIM(FELD2)) ,  ' ' , RTRIM(LTRIM(FELD1)) , ' ' , RTRIM(LTRIM(FELD3)) , ' ' , RTRIM(LTRIM(FELD4)) , ' ', RTRIM(LTRIM(FELD5)) , ' ' , RTRIM(LTRIM(FELD10))) as   BEMERKUNG  " &
             " from   " & myGlobalz.probaug_MYDB.Tabelle & "    " +
             " where (upper(RTRIM(LTRIM(feld20))) like '%" & filterName.ToUpper & "%'" &
             " or  upper(RTRIM(LTRIM(feld19))) like '%" & filterName.ToUpper & "%')" &
             " and (" &
                " upper(RTRIM(LTRIM(feld1))) = '" & Format(DateTime.Now.AddYears(-1), "yyyy") & "' or " &
                " upper(RTRIM(LTRIM(feld1))) = '" & Format(Now, "yyyy") & "'   " &
                " ) " &
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
        '  myGlobalz.sitzung.tempREC.mydb.Tabelle =myGlobalz.sitzung.tempREC.mydb.Tabelle
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
            myGlobalz.sitzung.tempREC.mydb.SQL =
             "select * from " & CLstart.myViewsNTabs.tabbankverbindung & "   where BankkontoID=" & BankkontoID%
            Dim hinweis As String = myGlobalz.sitzung.tempREC.getDataDT()
            If myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
                nachricht_und_Mbox("Problem beim initBankkontoDT:" & myGlobalz.sitzung.tempREC.mydb.SQL)
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Abspeichern: " & vbCrLf ,ex)
            Return False
        End Try
    End Function

    Shared Function suchePersonNachRolleDT_inVorgangsDB(ByVal filter As String) As DataTable
        '   dim hinweis as string 
        filter = filter.ToLower
        myGlobalz.sitzung.vorgangsbeteiligteAuswahlREC.mydb.SQL =
         "select * from " & CLstart.myViewsNTabs.tabSTAKEHOLDER & " as stakeholder " &
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
         "select distinct anrede,namenszusatz,nachname,vorname,plz,gemeindename,Strasse,Hausnr,orgname,kassenkonto," &
                        "orgzusatz,FFTelefon1,FFTelefon2,FFFax1,FFFax2,FFMobilfon,FFhomepage,FFemail, gesellfunktion,bezirk,personenvorlage from " &
         " " & CLstart.myViewsNTabs.tabBeteiligte & " " &
         " where upper(nachname) like '%" & filter.ToUpper & "%'" &
         " or  upper(vorname) like '%" & filter.ToUpper & "%'" &
         " or  upper(orgname) like '%" & filter.ToUpper & "%'" &
         " or  upper(orgzusatz) like '%" & filter.ToUpper & "%'" &
         " order by nachname,vorname,gemeindename,personenvorlage desc"
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





    Shared Sub holeStrasseDTausLageschluessel(buchstabe As String)

        'myGlobalz.sitzung.postgresREC.mydb.Tabelle ="lageschluessel"

        If String.IsNullOrEmpty(buchstabe) Then
            myGlobalz.sitzung.postgresREC.mydb.SQL = "select distinct lage as strcode,bezeichnung as sname,'halo' as lage from  flurkarte.lageschluessel " &
                " where gemeinde = " & myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNr() &
                " order by	bezeichnung asc"
        Else
            myGlobalz.sitzung.postgresREC.mydb.SQL = "select distinct lage as strcode,bezeichnung as sname,'halo' from flurkarte.lageschluessel " &
            " where gemeinde = " & myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNr() &
            " and lower(bezeichnung) like '" & buchstabe.ToLower & "%'" &
            " order by	bezeichnung asc"
        End If
        nachricht(myGlobalz.sitzung.postgresREC.getDataDT())
    End Sub
End Class