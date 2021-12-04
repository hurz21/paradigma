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
        Return True
    End Function
    Public Shared Function getDT_(ByVal idspalte As String, ByVal idwert As Integer, ByVal myneREC As IDB_grundfunktionen) As Boolean   'myGlobalz.sitzung.VorgangREC
        Try
            If String.IsNullOrEmpty(idspalte$) Then
                idspalte$ = "VorgangsID"
            End If
            myneREC.mydb.SQL =
             "select * from " & myneREC.mydb.Tabelle &
             " where " & idspalte & "=" & idwert
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

    Public Shared Sub hole_AdressKoordinatenExtracted(ByVal halo_id%)
        myglobalz.sitzung.postgresREC.mydb.SQL = "select rechts,hoch,fs,strcode,gemeindenr,hausnr,Zusatz from public.halofs " &
                  " where id = " & halo_id%
        Dim hinweis As String = myglobalz.sitzung.postgresREC.getDataDT()
    End Sub



    Public Shared Sub DBholeHausnrDT()
        myglobalz.sitzung.postgresREC.mydb.SQL = String.Format("select distinct id ,cast(concat(hausnr,zusatz) as text) as hausnrkombi,hausnr,zusatz " &
                                                           "from public.halofs where gemeindenr = {0} and strcode = {1} order by  hausnr,zusatz",
                              myglobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(),
                             myglobalz.sitzung.aktADR.Gisadresse.strasseCode())
        nachricht(myglobalz.sitzung.postgresREC.getDataDT())
    End Sub


    Private Shared Sub erstelleSQLfuerhausnr_alledb2(iDB_grundfunktionen As IDB_grundfunktionen)
        If myglobalz.vorgang_MYDB.dbtyp = "mysql" Then
            iDB_grundfunktionen.mydb.SQL =
         String.Format("select distinct hausnrkombi  from {0} where gemeindenr = {1} and lower(Strassenname) = '{2}' order by abs(hausnrkombi)",
                      iDB_grundfunktionen.mydb.Tabelle, myglobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(),
                      myglobalz.sitzung.aktADR.Gisadresse.strasseName().ToLower)
        End If
        If myglobalz.vorgang_MYDB.dbtyp = "oracle" Then
            iDB_grundfunktionen.mydb.SQL =
         String.Format("select distinct hausnrkombi  from {0} where gemeindenr = {1} and lower(Strassenname) = '{2}'" &
                       " order by to_number(regexp_substr(hausnrkombi,'^[0-9]+')),to_number(regexp_substr(hausnrkombi,'$[0-9]+'))",
                        iDB_grundfunktionen.mydb.Tabelle, myglobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(),
                        myglobalz.sitzung.aktADR.Gisadresse.strasseName().ToLower)
        End If
    End Sub

    Public Shared Sub DBholeHausnrDTVorgaenge_alledb()
        myglobalz.sitzung.tempREC2.mydb.Tabelle = "paraadresse" ' abs(hausnrkombi)in mysql
        erstelleSQLfuerhausnr_alledb2(myglobalz.sitzung.tempREC2)

        Dim resultdt As New System.Data.DataTable
        Dim erfolg As Integer = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myglobalz.sitzung.tempREC2.mydb.SQL,
                                                                               myglobalz.sitzung.tempREC2.mydb.Tabelle,
                                                                               resultdt)
        If resultdt IsNot Nothing Then myglobalz.sitzung.tempREC2.dt = resultdt.Copy
    End Sub

    Public Shared Sub holeStrasseDT4Vorgaenge()
        myglobalz.sitzung.tempREC.mydb.Tabelle = "paraadresse"
        myglobalz.sitzung.tempREC.mydb.SQL = String.Format("select distinct trim(strassenname)  as sname from {0} where gemeindenr = {1} order by	trim(strassenname) asc",
                                                           myglobalz.sitzung.tempREC.mydb.Tabelle, myglobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig())
        Dim resultdt As New System.Data.DataTable
        Dim erfolg As Integer = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myglobalz.sitzung.tempREC.mydb.SQL, myglobalz.sitzung.tempREC.mydb.Tabelle, resultdt)
        myglobalz.sitzung.tempREC.dt = resultdt.Copy

    End Sub

    Public Shared Sub holeStrasseDTausHalo(buchstabe As String)
        Dim a = myglobalz.sitzung.postgresREC.mydb.tostring
        If String.IsNullOrEmpty(buchstabe) Then
            myglobalz.sitzung.postgresREC.mydb.SQL = "select distinct strcode ,sname,'halo' as halo from public.halofs " &
                " where gemeindenr = " & myglobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig() &
                " order by	sname asc"
        Else
            myglobalz.sitzung.postgresREC.mydb.SQL = "select distinct strcode ,sname,'halo' as halo from public.halofs " &
                                                " where gemeindenr = " & myglobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig() &
                                                " and lower(sname) like '" & buchstabe.Trim & "%'" &
                                                " order by	sname asc "
        End If
        nachricht(myglobalz.sitzung.postgresREC.getDataDT())

    End Sub


    Public Shared Sub holeZaehlerDTinvorgaengen()
        myglobalz.sitzung.tempREC.mydb.SQL = "select distinct zaehler  from paraflurstueck" &
         " where gemcode = " & myglobalz.sitzung.aktFST.normflst.gemcode &
         " and flur = " & myglobalz.sitzung.aktFST.normflst.flur &
         " order by zaehler  "
        nachricht(myglobalz.sitzung.tempREC.getDataDT())
    End Sub

    Public Shared Sub holeZaehlerDT()
        myglobalz.sitzung.postgresREC.mydb.SQL = "select distinct zaehler from flurkarte.basis_f " &
         " where gemcode = " & myglobalz.sitzung.aktFST.normflst.gemcode &
         " and flur = " & myglobalz.sitzung.aktFST.normflst.flur &
         " order by zaehler  "
        nachricht(myglobalz.sitzung.postgresREC.getDataDT())
    End Sub

    Public Shared Sub holeNennerDTinVorgaengen()
        myglobalz.sitzung.tempREC.mydb.SQL = "select distinct nenner from paraflurstueck" &
         " where gemcode = " & myglobalz.sitzung.aktFST.normflst.gemcode &
         " and flur = " & myglobalz.sitzung.aktFST.normflst.flur &
         " and zaehler = " & myglobalz.sitzung.aktFST.normflst.zaehler &
         " order by nenner  "
        nachricht(myglobalz.sitzung.tempREC.getDataDT())
    End Sub

    Public Shared Sub holeSekidListeDTinVorgaengenZuFlurstueckSQL()
        myglobalz.sitzung.tempREC.mydb.SQL = "select distinct id from paraflurstueck" &
         " where gemcode = " & myglobalz.sitzung.aktFST.normflst.gemcode &
         " and flur = " & myglobalz.sitzung.aktFST.normflst.flur &
         " and zaehler = " & myglobalz.sitzung.aktFST.normflst.zaehler &
         " and nenner = " & myglobalz.sitzung.aktFST.normflst.nenner &
         " order by id  "
        nachricht(myglobalz.sitzung.tempREC.getDataDT())
    End Sub

    Public Shared Sub holeNennerDT()
        myglobalz.sitzung.postgresREC.mydb.SQL = "select distinct nenner from flurkarte.basis_f " &
         " where gemcode = " & myglobalz.sitzung.aktFST.normflst.gemcode &
         " and flur = " & myglobalz.sitzung.aktFST.normflst.flur &
         " and zaehler = " & myglobalz.sitzung.aktFST.normflst.zaehler &
         " order by nenner  "
        nachricht(myglobalz.sitzung.postgresREC.getDataDT())
    End Sub

    Public Shared Sub holeFlureDT()
        myglobalz.sitzung.postgresREC.mydb.SQL = "select distinct flur from flurkarte.basis_f " &
         " where gemcode = " & myglobalz.sitzung.aktFST.normflst.gemcode &
         " order by flur "
        nachricht(myglobalz.sitzung.postgresREC.getDataDT())
    End Sub



    Public Shared Sub hole_FSTKoordinatenExtracted(ByVal FS$)
        myglobalz.sitzung.postgresREC.mydb.SQL = "select * from flurkarte.basis_f where FS = '" & FS & "'"
        nachricht(myglobalz.sitzung.postgresREC.getDataDT())
    End Sub

#Region "Beteiligte"



    Public Shared Function leseEinzelPersonViaPersonenID() As Boolean
        Dim hinweis As String
        myglobalz.sitzung.beteiligteREC.mydb.Tabelle = "personen"
        myglobalz.sitzung.beteiligteREC.mydb.SQL = "SELECT * FROM " & myglobalz.sitzung.beteiligteREC.mydb.Tabelle &
         " where PersonenID=" & myglobalz.sitzung.aktPerson.PersonenID
        hinweis = myglobalz.sitzung.beteiligteREC.getDataDT()
        If myglobalz.sitzung.beteiligteREC.mycount < 1 Then
            nachricht("Keine Personen gespeichert!")
            Return False
        Else
            nachricht(String.Format("{0} Ereignisse vorhanden", myglobalz.sitzung.beteiligteREC.mycount))
            Return True
        End If
    End Function

    Public Shared Function Beteiligte_abspeichern_Edit(ByVal pid%, ByVal vid%, ByVal lpers As Person) As Integer
        Dim anzahl%
        If myglobalz.beteiligte_MYDB.dbtyp = "mysql" Then
            Dim zzz As New clsBeteiligteDBCRUD_MYSQL(clsDBspecMYSQL.getConnection(myglobalz.beteiligte_MYDB))
            anzahl% = zzz.Beteiligte_abspeichern_EditExtracted(pid%, vid, lpers)
            zzz.Dispose()
        End If
        If myglobalz.beteiligte_MYDB.dbtyp = "oracle" Then
            Dim zzz As New clsBeteiligteDBCRUD_ORACLE(clsDBspecOracle.getConnection(myglobalz.beteiligte_MYDB))
            anzahl% = zzz.Beteiligte_abspeichern_EditExtracted(pid%, vid, lpers)
            zzz.Dispose()
        End If
        Return anzahl%
    End Function

#End Region


    Shared Function suchePersonNachFilterDT_LikeRolle(ByVal filter As String) As DataTable
        myglobalz.sitzung.tempREC.mydb.Tabelle = "personen"
        myglobalz.sitzung.tempREC.mydb.SQL =
         "SELECT * FROM " & myglobalz.sitzung.tempREC.mydb.Tabelle &
         " where lower(gesellfunktion) like '%" & filter.ToLower & "%'" &
         " order by name,vorname"
        nachricht(myglobalz.sitzung.tempREC.getDataDT())
        If myglobalz.sitzung.tempREC.mycount < 1 Then
            nachricht("Keine Personen gespeichert!")
            Return Nothing
        Else
            nachricht(String.Format("{0} Personen vorhanden", myglobalz.sitzung.tempREC.mycount))
            Return myglobalz.sitzung.tempREC.dt
        End If
    End Function

    Shared Function suchePersonNachFilterDT_Like(ByVal filter As String) As DataTable
        myglobalz.sitzung.tempREC.mydb.Tabelle = "stakeholder"
        myglobalz.sitzung.tempREC.mydb.SQL =
         "SELECT * FROM " & myglobalz.sitzung.tempREC.mydb.Tabelle &
         " where lower(nachName) like '%" & filter.ToLower & "%'" &
         " order by nachName,vorname"
        nachricht(myglobalz.sitzung.tempREC.getDataDT())
        If myglobalz.sitzung.tempREC.mycount < 1 Then
            nachricht("Keine Personen gespeichert!")
            Return Nothing
        Else
            nachricht(String.Format("{0} Personen vorhanden", myglobalz.sitzung.tempREC.mycount))
            Return myglobalz.sitzung.tempREC.dt
        End If
    End Function


    Public Shared Function adressliste4AdressIDListe(ByVal sql As String) As Boolean

        myglobalz.sitzung.tempREC.mydb.Tabelle = "paraadresse"
        myglobalz.sitzung.tempREC.mydb.SQL = sql$
        nachricht(myglobalz.sitzung.tempREC.getDataDT())
        If myglobalz.sitzung.tempREC.mycount < 1 Then
            nachricht("adressliste4AdressIDListe!" & sql$)
            Return False
        Else
            nachricht(String.Format("{0} adressliste4AdressIDListe vorhanden", myglobalz.sitzung.tempREC.mycount))
            Return True
        End If
    End Function

    Public Shared Function adressIDlisteVonPersonErstellen(ByVal personenID As Integer) As Boolean
        myglobalz.sitzung.tempREC.mydb.Tabelle = "person2adresse"
        myglobalz.sitzung.tempREC.mydb.SQL = "SELECT * FROM " & myglobalz.sitzung.tempREC.mydb.Tabelle &
         " where personenid =" & personenID%
        nachricht(myglobalz.sitzung.tempREC.getDataDT())
        If myglobalz.sitzung.tempREC.mycount < 1 Then
            nachricht("Keine adressen zur Personen gespeichert!" & personenID%)
            Return False
        Else
            nachricht(String.Format("{0} adresslisteVonPersonErstellen vorhanden", myglobalz.sitzung.tempREC.mycount))
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
        If myglobalz.sitzung.probaugREC.mydb.dbtyp = "mysql" Then
            myglobalz.sitzung.probaugREC.mydb.SQL = "select distinct " &
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
        If myglobalz.sitzung.probaugREC.mydb.dbtyp = "oracle" Then
            myglobalz.sitzung.probaugREC.mydb.SQL = "select distinct " &
         "  trim(FELD20)  AS  NACHNAME  " &
         ", trim(FELD19)  AS  VORNAME " &
         ", trim(FELD17)  AS  NAMENSZUSATZ  " &
         ", trim(FELD24)  AS  GEMEINDENAME  " &
         ", trim(FELD21)  AS  STRASSE  " &
         ", trim(FELD23)  AS  PLZ  " &
         ", trim(FELD22)  AS  HAUSNR  " &
         ", trim(FELD2) ||  ' ' || trim(FELD1) || ' ' || trim(FELD3) || ' ' || trim(FELD4) || ' '|| trim(FELD5) || ' ' || trim(FELD10) as   BEMERKUNG  " &
         " from   " & myglobalz.probaug_MYDB.Tabelle & "    " +
         " where (upper(trim(feld20)) like '%" & filterName.ToUpper & "%'" &
         " or  upper(trim(feld19)) like '%" & filterName.ToUpper & "%')" &
         " and (" &
            " upper(trim(feld1)) = '2010' or " &
            " upper(trim(feld1)) = '2011' or " &
            " upper(trim(feld1)) = '2012' or " &
            " upper(trim(feld1)) = '2013' or " &
            " upper(trim(feld1)) = '2014' or " &
            " upper(trim(feld1)) = '2015' or " &
            " upper(trim(feld1)) = '2016') " &
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
        hinweis = myglobalz.sitzung.probaugREC.getDataDT()
        If myglobalz.sitzung.probaugREC.mycount < 1 Then
            nachricht("Keine Personen gespeichert!")
            Return Nothing
        Else
            nachricht(String.Format("{0} Ereignisse vorhanden", myglobalz.sitzung.probaugREC.mycount))
            Return myglobalz.sitzung.probaugREC.dt
        End If
    End Function


    Shared Function suchePersonNachFilterDT_istgleich(ByVal sqlstring As String) As DataTable
        Dim hinweis As String
        '  myGlobalz.sitzung.tempREC.mydb.Tabelle = myGlobalz.sitzung.tempREC.mydb.Tabelle
        myglobalz.sitzung.tempREC.mydb.SQL = sqlstring$
        hinweis = myglobalz.sitzung.tempREC.getDataDT()
        If myglobalz.sitzung.tempREC.mycount < 1 Then
            nachricht("Keine Personen gespeichert!")
            Return Nothing
        Else
            nachricht(String.Format("{0} Ereignisse vorhanden", myglobalz.sitzung.tempREC.mycount))
            Return myglobalz.sitzung.tempREC.dt
        End If
    End Function













    Shared Function initBankkontoDT(ByVal BankkontoID As Integer) As Boolean
        If BankkontoID% < 1 Then
            My.Log.WriteEntry("	initBankkontoDT: ist ist kleiner 1")
            Return False
        End If
        Try
            myglobalz.sitzung.tempREC.mydb.Tabelle = "bankverbindung"
            myglobalz.sitzung.tempREC.mydb.SQL =
             "select * from " & myglobalz.sitzung.tempREC.mydb.Tabelle &
             " where BankkontoID=" & BankkontoID%
            Dim hinweis As String = myglobalz.sitzung.tempREC.getDataDT()
            If myglobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
                nachricht_und_Mbox("Problem beim initBankkontoDT:" & myglobalz.sitzung.tempREC.mydb.SQL)
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
        myglobalz.sitzung.vorgangsbeteiligteAuswahlREC.mydb.SQL =
         "select * from " &
         " stakeholder " &
         " where upper(gesellfunktion) like '%" & filter.ToUpper & "%'" &
         " or upper(rolle) like '%" & filter.ToUpper & "%'" &
         " order by nachname,vorname,gemeindename,gesellfunktion"
        myglobalz.sitzung.vorgangsbeteiligteAuswahlREC.mydb.SQL = myglobalz.sitzung.vorgangsbeteiligteAuswahlREC.mydb.SQL.ToUpper
        nachricht("hinweis = " & myglobalz.sitzung.vorgangsbeteiligteAuswahlREC.getDataDT())
        If myglobalz.sitzung.vorgangsbeteiligteAuswahlREC.mycount < 1 Then
            nachricht("Keine Personen gespeichert!")
            Return Nothing
        Else
            nachricht(String.Format("{0} Ereignisse vorhanden", myglobalz.sitzung.vorgangsbeteiligteAuswahlREC.mycount))
            Return myglobalz.sitzung.vorgangsbeteiligteAuswahlREC.dt
        End If
    End Function


    Shared Function suchePersonNachFilterDT_inVorgangsDB(ByVal filter As String) As DataTable
        '   dim hinweis as string 
        filter = filter.ToLower
        myglobalz.sitzung.vorgangsbeteiligteAuswahlREC.mydb.SQL =
         "select distinct anrede,namenszusatz,nachname,vorname,plz,gemeindename,Strasse,Hausnr,orgname,kassenkonto," &
                        "orgzusatz,FFTelefon1,FFTelefon2,FFFax1,FFFax2,FFMobilfon,FFhomepage,FFemail, gesellfunktion,bezirk,personenvorlage from " &
         "beteiligte " &
         " where upper(nachname) like '%" & filter.ToUpper & "%'" &
         " or  upper(vorname) like '%" & filter.ToUpper & "%'" &
         " or  upper(orgname) like '%" & filter.ToUpper & "%'" &
         " or  upper(orgzusatz) like '%" & filter.ToUpper & "%'" &
         " order by nachname,vorname,gemeindename,personenvorlage desc"
        myglobalz.sitzung.vorgangsbeteiligteAuswahlREC.mydb.SQL = myglobalz.sitzung.vorgangsbeteiligteAuswahlREC.mydb.SQL.ToUpper
        nachricht("hinweis = " & myglobalz.sitzung.vorgangsbeteiligteAuswahlREC.getDataDT())
        If myglobalz.sitzung.vorgangsbeteiligteAuswahlREC.mycount < 1 Then
            nachricht("Keine Personen gespeichert!")
            Return Nothing
        Else
            nachricht(String.Format("{0} Ereignisse vorhanden", myglobalz.sitzung.vorgangsbeteiligteAuswahlREC.mycount))
            Return myglobalz.sitzung.vorgangsbeteiligteAuswahlREC.dt
        End If
    End Function

    Shared Sub DBholeAdressenFuerDatagridStrasse()
        myglobalz.sitzung.tempREC2.mydb.Tabelle = "paraadresse"
        '  myGlobalz.sitzung.tempREC.mydb.SQL = "select * from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & " "

        If myglobalz.sitzung.tempREC2.mydb.dbtyp = "oracle" Then
            myglobalz.sitzung.tempREC2.mydb.SQL = String.Format("select ad.gemeindetext,ad.strassenname,ad.hausnrkombi,v.beschreibung,v.vorgangsid," &
                                                    " v.az2, v.eingang, v.letztebearbeitung, v.lastactionheroe, v.id,v.altaz,v.probaugaz,v.sachgebietnr, " &
                                                    " v.stellungnahme,v.ortstermin,v.erledigt " &
                                                    " from paraadresse ad ,vsk_d v,pa_sekid2vid k " &
                                                    " where gemeindenr = {0}  and lower(Strassenname) = '{1}'  " &
                                                    " and ad.id=k.sekid " &
                                                    " and k.vorgangsid=v.vorgangsid " &
                                                    " order by to_number(regexp_substr(hausnrkombi,'^[0-9]+')),to_number(regexp_substr(hausnrkombi,'$[0-9]+'))",
                            myglobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(),
                            myglobalz.sitzung.aktADR.Gisadresse.strasseName().ToLower)
        End If

        Dim resultdt As New System.Data.DataTable
        Dim erfolg As Integer = VSTTools.selectFromParadigmaTabelle_alleDB.exe(myglobalz.sitzung.tempREC2.mydb.SQL,
                                                                               myglobalz.sitzung.tempREC2.mydb.Tabelle,
                                                                               resultdt)
        If resultdt IsNot Nothing Then myglobalz.sitzung.tempREC2.dt = resultdt.Copy
    End Sub



    Shared Sub holeStrasseDTausLageschluessel(buchstabe As String)

        myglobalz.sitzung.postgresREC.mydb.Tabelle = "lageschluessel"

        If String.IsNullOrEmpty(buchstabe) Then
            myglobalz.sitzung.postgresREC.mydb.SQL = "select distinct lage as strcode,bezeichnung as sname,'halo' as lage from  public." &
                                  myglobalz.sitzung.postgresREC.mydb.Tabelle &
                " where gemeinde = " & myglobalz.sitzung.aktADR.Gisadresse.gemeindeNr() &
                " order by	bezeichnung asc"
        Else
            myglobalz.sitzung.postgresREC.mydb.SQL = "select distinct lage as strcode,bezeichnung as sname,'halo' from public." &
                              myglobalz.sitzung.postgresREC.mydb.Tabelle &
            " where gemeinde = " & myglobalz.sitzung.aktADR.Gisadresse.gemeindeNr() &
            " and lower(bezeichnung) like '" & buchstabe.ToLower & "%'" &
            " order by	bezeichnung asc"
        End If
        nachricht(myglobalz.sitzung.postgresREC.getDataDT())
    End Sub
End Class