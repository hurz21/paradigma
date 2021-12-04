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





#Region "RB_adresse"
    Public Shared Sub hole_AdressKoordinatenExtracted(ByVal halo_id%)

        myGlobalz.sitzung.postgresREC.mydb.SQL = "select rechts,hoch,fs from flurkarte.halofs " &
         " where id = " & halo_id%
        Dim hinweis As String = myGlobalz.sitzung.postgresREC.getDataDT()
    End Sub

    'Public Shared Sub hole_AdressKoordinaten_bynames() 
    '    myGlobalz.sitzung.postgresREC.mydb.SQL = "select rechts,hoch from public.halofs " & _
    '     myGlobalz.sitzung.postgresREC.mydb.Tabelle & _
    '     " where gemeindenr = " & myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig() & _
    '     " and sname = '" & myGlobalz.sitzung.aktADR.Gisadresse.strasseName() & "'" & _
    '     " and hausnr = " & myGlobalz.sitzung.aktADR.Gisadresse.hausNr & _
    '     " and zusatz = '" & myGlobalz.sitzung.aktADR.Gisadresse.hausZusatz & "'"
    '    nachricht(myGlobalz.sitzung.postgresREC.getDataDT())
    'End Sub

    Public Shared Sub DBholeHausnrDT()
        myGlobalz.sitzung.postgresREC.mydb.SQL = String.Format("select distinct id ,cast(concat(hausnr,zusatz) as CHAR) as hausnrkombi from  flurkarte.halofs" &
                                                           " where gemeindenr = {0} and strcode = {1} order by  hausnr,zusatz",
                                                           myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(),
                                                           myGlobalz.sitzung.aktADR.Gisadresse.strasseCode())
        nachricht(myGlobalz.sitzung.postgresREC.getDataDT())
    End Sub

    Public Shared Sub DBholeHausnrDTVorgaenge_alledb()
        'myGlobalz.sitzung.tempREC.mydb.Tabelle ="paraadresse" ' abs(hausnrkombi)in mysql
        If myGlobalz.vorgang_MYDB.dbtyp = "mysql" Then
            myGlobalz.sitzung.tempREC.mydb.SQL =
         String.Format("select distinct id ,hausnrkombi  from " & CLstart.myViewsNTabs.tabPARAADRESSE & "  where gemeindenr = {0} and lower(Strassenname) = '{1}' order by abs(hausnrkombi)",
                       myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(), myGlobalz.sitzung.aktADR.Gisadresse.strasseName().ToLower)
        End If
        If myGlobalz.vorgang_MYDB.dbtyp = "oracle" Then
            myGlobalz.sitzung.tempREC.mydb.SQL =
         String.Format("select distinct id ,hausnrkombi  from " & CLstart.myViewsNTabs.tabPARAADRESSE & "  where gemeindenr = {0} and lower(Strassenname) = '{1}' order by to_number(regexp_substr(hausnrkombi,'^[0-9]+')),to_number(regexp_substr(hausnrkombi,'$[0-9]+'))",
                          myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig(), myGlobalz.sitzung.aktADR.Gisadresse.strasseName().ToLower)
        End If

        ' nachricht(myGlobalz.sitzung.tempREC.getDataDT())

        Dim resultdt As New System.Data.DataTable
        VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.tempREC.mydb.SQL, resultdt)
        myGlobalz.sitzung.tempREC.dt = resultdt.Copy
        'Todo aufbauend darauf die kompletten vorgänge ziehen
    End Sub

    'Public Shared Sub holeStrasseDT4Vorgaenge()
    '    'myGlobalz.sitzung.tempREC.mydb.Tabelle ="paraadresse"
    '    myGlobalz.sitzung.tempREC.mydb.SQL = String.Format("select distinct strassenname  as sname from " & CLstart.myViewsNTabs.tabPARAADRESSE  & "  where gemeindenr = {0} order by	strassenname asc",
    '                                                       myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig())
    '    ' nachricht(myGlobalz.sitzung.tempREC.getDataDT())

    '    Dim resultdt As New System.Data.DataTable
    '    VSTTools.selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.tempREC.mydb.SQL, resultdt)
    '    myGlobalz.sitzung.tempREC.dt = resultdt.Copy
    'End Sub


    'Public Shared Sub holeStrasseDT() 
    '    myGlobalz.sitzung.postgresREC.mydb.SQL = "select distinct strcode ,sname from public.halofs " & _
    '     myGlobalz.sitzung.postgresREC.mydb.Tabelle & _
    '     " where gemeindenr = " & myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNrBig() & _
    '     " order by	sname asc"
    '    nachricht(myGlobalz.sitzung.postgresREC.getDataDT())
    'End Sub

    Public Shared Sub holeZaehlerDTinvorgaengen()
        myGlobalz.sitzung.tempREC.mydb.SQL = "select distinct zaehler  from " & CLstart.myViewsNTabs.tabPARAFLURSTUECK & " " &
         " where gemcode = " & myGlobalz.sitzung.aktFST.normflst.gemcode &
         " and flur = " & myGlobalz.sitzung.aktFST.normflst.flur &
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
        myGlobalz.sitzung.tempREC.mydb.SQL = "select distinct nenner,id  from " & CLstart.myViewsNTabs.tabPARAFLURSTUECK & " " &
         " where gemcode = " & myGlobalz.sitzung.aktFST.normflst.gemcode &
         " and flur = " & myGlobalz.sitzung.aktFST.normflst.flur &
         " and zaehler = " & myGlobalz.sitzung.aktFST.normflst.zaehler &
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

    'Public Shared Function Entkoppelung_Vorgang_Person(ByVal vorgangID%, ByVal personenID as integer) as  Integer
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
    '            nachricht_und_Mbox("Problem beim Abspeichernv:" & myGlobalz.sitzung.tempREC.mydb.SQL)
    '            Return -1
    '        Else
    '            Return CInt(anzahlTreffer)
    '        End If
    '    Catch ex As Exception
    '        nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
    '        Return -2
    '    End Try
    'End Function






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
    '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="Person2vorgang"    ''& " order by ts desc"
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
    '	myGlobalz.sitzung.personenRec.mydb.Tabelle ="personen"
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

    'Public Shared Function Beteiligte_abspeichern_Edit(ByVal pid%, ByVal vid%, ByVal lpers As Person) As Integer
    '    Dim anzahl As Integer
    '    'If myGlobalz.beteiligte_MYDB.dbtyp = "mysql" Then
    '    '    Dim zzz As New clsBeteiligteDBCRUD_MYSQL(clsDBspecMYSQL.getConnection(myGlobalz.beteiligte_MYDB))
    '    '    anzahl = zzz.Beteiligte_abspeichern_EditExtracted(pid%, vid, lpers)
    '    '    zzz.dispose
    '    'End If
    '    If myGlobalz.beteiligte_MYDB.dbtyp = "oracle" Then
    '        Dim zzz As New clsBeteiligteDBCRUD_ORACLE(clsDBspecOracle.getConnection(myGlobalz.beteiligte_MYDB))
    '        anzahl% = zzz.Beteiligte_abspeichern_EditExtracted(pid%, vid, lpers)
    '          zzz.dispose
    '    End If
    '    Return anzahl%
    'End Function

#End Region









    Public Shared Function initBearbeiterLISTEByUserid_DT_ausParadigma_datenbank(ByVal sql As String) As DataTable

        '  Dim lokREC As LibDB.IDB_grundfunktionen
        ' lokREC = CType(myGlobalz.sitzung.BearbeiterREC, clsDBspecMYSQL)
        'myGlobalz.sitzung.BearbeiterREC.mydb.Tabelle =CLstart.myViewsNTabs.tabBearbeiter
        myGlobalz.sitzung.BearbeiterREC.mydb.SQL = sql
        nachricht(myGlobalz.sitzung.BearbeiterREC.getDataDT)
        Return myGlobalz.sitzung.BearbeiterREC.dt
    End Function


    'Shared Function initBearbeiterByUserid_ausParadigmadb(ByRef usi As clsBearbeiter, ByVal spalteKey$, ByVal spaltevalue As String) As Boolean
    '    'Todo Diese funktion sollte auf LINQ umgestellt werden
    '    Dim lokrec As New DataTable
    '    Try
    '        'If spaltevalue$ = "FEINEN_DR" Then
    '        '    spaltevalue$ = "A670024"
    '        'End If
    '        Dim sql$ = String.Format("select * from  " & CLstart.myViewsNTabs.tabBearbeiter & "  where lower({0})='{1}' or kuerzel1='{1}'  or lower(initial_)='{1}'", spalteKey, spaltevalue$.ToLower)
    '        lokrec = initBearbeiterLISTEByUserid_DT_ausParadigma_datenbank(sql).Copy
    '        usi.Namenszusatz = clsDBtools.fieldvalue(lokrec.Rows(0).Item("namenszusatz"))
    '        usi.Name = clsDBtools.fieldvalue(lokrec.Rows(0).Item("name"))
    '        usi.Vorname = clsDBtools.fieldvalue(lokrec.Rows(0).Item("vorname"))
    '        usi.Rang = clsDBtools.fieldvalue(lokrec.Rows(0).Item("rang"))
    '        usi.Raum = clsDBtools.fieldvalue(lokrec.Rows(0).Item("rites"))
    '        usi.STDGRANTS = clsDBtools.fieldvalue(lokrec.Rows(0).Item("STDGRANTS"))
    '        usi.Kontakt.elektr.Telefon1 = clsDBtools.fieldvalue(lokrec.Rows(0).Item("Telefon"))
    '        usi.Kontakt.elektr.Fax1 = clsDBtools.fieldvalue(lokrec.Rows(0).Item("Fax"))
    '        usi.Kuerzel2Stellig = clsDBtools.fieldvalue(lokrec.Rows(0).Item("Kuerzel1"))
    '        usi.Kontakt.elektr.Email = clsDBtools.fieldvalue(lokrec.Rows(0).Item("email"))
    '        usi.Bemerkung = clsDBtools.fieldvalue(lokrec.Rows(0).Item("abteilung"))
    '        usi.Rolle = clsDBtools.fieldvalue(lokrec.Rows(0).Item("rolle"))
    '        usi.ExpandHeaderInSachgebiet = clsDBtools.fieldvalue(lokrec.Rows(0).Item("ExpandHeaderInSachgebiet"))

    '        usi.Initiale = usi.getInitial
    '        Return True
    '    Catch ex As Exception
    '        nachricht_und_Mbox("fehler User ist unbekannt: " & usi.username & vbCrLf & " Bitte informieren sie den Admin." & vbCrLf,ex)
    '        Return False
    '    Finally
    '        lokrec.Dispose()
    '    End Try
    'End Function






End Class