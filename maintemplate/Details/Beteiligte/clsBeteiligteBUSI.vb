Imports System.Data

Public Class clsBeteiligteBUSI
    Public Shared Function verlinkenVonBeteiligten(ByVal quellvid%, ByVal zielVID as integer) as  Boolean
        Dim instring1 As String = ""
        Dim instring2 As String = ""

        nachricht("in verlinkenVonBeteiligten ------------------------------------")
        Dim BeteiligteLinksholen_erfolgreich As Boolean = viaKopplung_VorgangID_zu_BeteiligteID_OHNEKOPPLUNG(quellvid%.ToString) ' nach myGlobalz.sitzung.tempREC

        instring1 = clsDBtools.bildINstringSpaltenname(myGlobalz.sitzung.tempREC.dt, "personenid")
        BeteiligteLinksholen_erfolgreich = viaKopplung_VorgangID_zu_BeteiligteID(quellvid%.ToString, myGlobalz.sitzung.tempREC)
        instring2 = clsDBtools.bildINstringSpaltenname(myGlobalz.sitzung.tempREC.dt, "BeteiligteID")
        If Not String.IsNullOrEmpty(instring1) Then
            instring1 = instring1 & ","
        End If
        instring1 = instring1 & instring2

        If Not BeteiligteLinksholen_erfolgreich Then
            If instring1.Length > 2 Then
                BeteiligteLinksholen_erfolgreich = True
            End If
        End If

        If BeteiligteLinksholen_erfolgreich Then
            '  If myGlobalz.sitzung.tempREC.mycount > 0 Then
            nachricht("Es wird kopiert")
            BeteiligtelinksKopieren(zielVID, instring1)
            Return True
            'Else
            '    nachricht("Es wird nicht kopiert weil keine treffer")
            '    Return False
            'End If
        Else
            nachricht("FEHLER: verlinkenVonBeteiligten nicht erfolgreich!")
            Return False
        End If
    End Function

 

    ''' <summary>
    '''  myGlobalz.sitzung.tempREC wird gefüllt
    ''' </summary>
    ''' <param name="vorgangsid"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function viaKopplung_VorgangID_zu_BeteiligteID(ByVal vorgangsid As String, ByVal Ziel_tidb As IDB_grundfunktionen) As Boolean '  myGlobalz.sitzung.tempREC
        Dim hinweis As String
        Ziel_tidb.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
        Ziel_tidb.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
        'Ziel_tidb.mydb.Tabelle ="Beteiligte2vorgang"     ''& " order by ts desc"
        Ziel_tidb.mydb.SQL = "SELECT * FROM " & CLstart.myViewsNTabs.tabKoppBeteiligteVorgang & "" &
                                        " where VorgangsID=" & vorgangsid
        hinweis = Ziel_tidb.getDataDT()
        If Ziel_tidb.mycount < 1 Then
            nachricht("Keine Beteiligten gespeichert!")
            Return False
        Else
            nachricht(String.Format("{0} Ereignisse vorhanden", Ziel_tidb.mycount))
            Return True
        End If
    End Function

    Private Shared Function viaKopplung_VorgangID_zu_BeteiligteID_OHNEKOPPLUNG(ByVal vorgangsid As String) As Boolean
        Dim hinweis As String
        myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
        myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
        'myGlobalz.sitzung.tempREC.mydb.Tabelle ="Beteiligte"     ''& " order by ts desc"
        myGlobalz.sitzung.tempREC.mydb.SQL =
                                        "SELECT * FROM  " & CLstart.myViewsNTabs.tabBeteiligte & " " &
                                        " where VorgangsID=" & vorgangsid$
        hinweis = myGlobalz.sitzung.tempREC.getDataDT()
        If myGlobalz.sitzung.tempREC.mycount < 1 Then
            nachricht("Keine Beteiligten gespeichert!")
            Return False
        Else
            nachricht(String.Format("{0} Ereignisse vorhanden", myGlobalz.sitzung.tempREC.mycount))
            Return True
        End If
    End Function


    Public Shared Function BeteiligtelinksKopieren(ByVal zielvid%, ByVal instring As String) As Boolean
        nachricht("in BeteiligtelinksKopieren ------------------------------------") 'BeteiligtelinksKopieren

        Dim newid%
        Dim erfolgreich As Boolean = False
        Dim a() As String = instring.Split(","c)
        Try
            For i = 0 To a.GetUpperBound(0) - 1
                newid = BeteiligteKopplungVorgang_alledb(zielvid, CInt(a(i)), 1)
                If newid < 1 Then
                    nachricht_und_Mbox("Kopie konnte nicht angelegt werden: ")
                    erfolgreich = False
                Else
                    nachricht("Kopie konnte angelegt werden: ")
                    erfolgreich = True
                End If
            Next
            Return erfolgreich
        Catch ex As Exception
            nachricht_und_Mbox("Fehler bei BeteiligtelinksKopieren: " & ex.ToString)
            nachricht_und_Mbox("Fehler bei BeteiligtelinksKopieren: ")
            Return True
        End Try
    End Function


    Public Shared Function Anschrift_Text_erzeugen(ByVal ap As Person) As String 'myGlobalz.sitzung.aktPerson
        Try
            Dim sb As New Text.StringBuilder
            If Not String.IsNullOrEmpty(ap.Kontakt.Org.Name) Then sb.Append(ap.Kontakt.Org.Name & vbCrLf)
            If Not String.IsNullOrEmpty(ap.Kontakt.Org.Zusatz) Then sb.Append(ap.Kontakt.Org.Zusatz & vbCrLf)

            Dim ared$ = "Herr"
            If Not String.IsNullOrEmpty(ap.Anrede.Trim) Then
                ared = ap.Anrede & " "
            Else
                ared = ""
            End If
            sb.Append(ared) 

            If Not String.IsNullOrEmpty(ap.Namenszusatz.Trim) Then sb.Append(ap.Namenszusatz & " ")
            If Not String.IsNullOrEmpty(ap.Vorname.Trim) Then sb.Append(ap.Vorname & " ")
            If Not String.IsNullOrEmpty(ap.Name.Trim) Then sb.Append(ap.Name & vbCrLf)
            If Not String.IsNullOrEmpty(ap.VERTRETENDURCH.Trim) Then sb.Append(ap.VERTRETENDURCH & vbCrLf)

            If Not String.IsNullOrEmpty(ap.Kontakt.Anschrift.Strasse.Trim) Then sb.Append(ap.Kontakt.Anschrift.Strasse.Trim & " ")
            If Not String.IsNullOrEmpty(ap.Kontakt.Anschrift.Hausnr.Trim) Then sb.Append(ap.Kontakt.Anschrift.Hausnr.Trim & " " & vbCrLf)

            If String.IsNullOrEmpty(ap.Kontakt.Anschrift.Postfach.Trim) Then
            Else
                sb.Append("Postfach " & (ap.Kontakt.Anschrift.Postfach & vbCrLf))
            End If
            If String.IsNullOrEmpty(ap.Kontakt.Anschrift.PostfachPLZ.Trim) Then
            Else
                sb.Append("PostfachPLZ " & (ap.Kontakt.Anschrift.PostfachPLZ & vbCrLf))
            End If
            sb.Append(ap.Kontakt.Anschrift.PLZ & " ")
            sb.Append(ap.Kontakt.Anschrift.Gemeindename & " " & vbCrLf)
            sb.Append("")
            sb.Append("")
            Return sb.ToString.Replace("  ", " ")
        Catch ex As Exception
            nachricht("Fehler: Anschrift_Text_erzeugen: " & ex.ToString)
            Return " -------------------- "
        End Try
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="item">eingang</param>
    ''' <param name="aktper">ausgang</param>
    ''' <remarks>overloaded!!!</remarks>
    Public Shared Sub BeteiligtenRec2Obj(ByVal item As DataRow, ByVal aktper As Person)
        Try
            With aktper
                .PersonenID = CInt(clsDBtools.fieldvalue(item("PERSONENID")))
                .Status = CInt(clsDBtools.fieldvalue(item("STATUS")))
                .Name = CStr(clsDBtools.fieldvalue(item("NACHNAME")))
                .Vorname = CStr(clsDBtools.fieldvalue(item("VORNAME")))
                .Bemerkung = CStr(clsDBtools.fieldvalue(item("BEMERKUNG")))
                .Namenszusatz = CStr(clsDBtools.fieldvalue(item("NAMENSZUSATZ")))
                '	MYGLOBALZ.SITZUNG.AKTPERSON.FDKURZ = CStr(clsDBtools.fieldvalue(item("FDKURZ")))
                .Anrede = CStr(clsDBtools.fieldvalue(item("ANREDE")))
                .Kontakt.Anschrift.Gemeindename = CStr(clsDBtools.fieldvalue(item("GEMEINDENAME")))
                .Kontakt.Anschrift.Strasse = CStr(clsDBtools.fieldvalue(item("STRASSE")))
                .Kontakt.Anschrift.Hausnr = CStr(clsDBtools.fieldvalue(item("HAUSNR")))
                .Kontakt.Anschrift.PLZ = (clsDBtools.fieldvalue(item("PLZ")))
                .Kontakt.Anschrift.Postfach = CStr(clsDBtools.fieldvalue(item("POSTFACH")))
                .Kontakt.Anschrift.PostfachPLZ = clsDBtools.fieldvalue((item("POSTFACHPLZ")))
                .Kontakt.Org.Name = CStr(clsDBtools.fieldvalue(item("ORGNAME")))
                .Kontakt.Org.Zusatz = CStr(clsDBtools.fieldvalue(item("ORGZUSATZ")))
                .Kontakt.Org.Typ1 = CStr(clsDBtools.fieldvalue(item("ORGTYP1")))
                .Kontakt.Org.Typ2 = CStr(clsDBtools.fieldvalue(item("ORGTYP2")))
                .Kontakt.Org.Eigentuemer = CStr(clsDBtools.fieldvalue(item("ORGEIGENTUEMER")))
                .Kontakt.GesellFunktion = CStr(clsDBtools.fieldvalue(item("GESELLFUNKTION")))
                .Kontakt.elektr.Telefon1 = CStr(clsDBtools.fieldvalue(item("FFTELEFON1")))
                .Kontakt.elektr.Telefon2 = CStr(clsDBtools.fieldvalue(item("FFTELEFON2")))
                .Kontakt.elektr.Fax1 = CStr(clsDBtools.fieldvalue(item("FFFAX1")))
                .Kontakt.elektr.Fax2 = CStr(clsDBtools.fieldvalue(item("FFFAX2")))
                .Kontakt.elektr.MobilFon = CStr(clsDBtools.fieldvalue(item("FFMOBILFON")))
                .Kontakt.elektr.Email = CStr(clsDBtools.fieldvalue(item("FFEMAIL")))
                .Kontakt.elektr.Homepage = CStr(clsDBtools.fieldvalue(item("FFHOMEPAGE")))
                '.Kontakt.Bankkonto.BankName = CStr(clsDBtools.fieldvalue(item("BankName")))
                '.Kontakt.Bankkonto.BIC = CStr(clsDBtools.fieldvalue(item("BIC")))
                '.Kontakt.Bankkonto.IBAN = CStr(clsDBtools.fieldvalue(item("IBAN")))
                '.Kontakt.Bankkonto.Titel = CStr(clsDBtools.fieldvalue(item("BVTITEL")))
                Try
                    .Kassenkonto = CStr(clsDBtools.fieldvalue(item("KASSENKONTO")))
                Catch ex As Exception

                End Try

                .Rolle = CStr(clsDBtools.fieldvalue(item("ROLLE")))
                .Bezirk = CStr(clsDBtools.fieldvalue(item("BEZIRK")))
                .lastchange = CDate(clsDBtools.fieldvalueDate(item("lastchange")))
            End With
        Catch ex As Exception
            nachricht_und_Mbox("2 fehler in BeteiligtenRec2Obj(Datarow): ggf. fehlt die rolle:" & ex.ToString)
        End Try
    End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="item">eingang datarowview</param>
    ''' <param name="aktper">ausgang ein objekt der marke person</param>
    ''' <remarks>overloaded!!</remarks>
    Public Shared Sub BeteiligtenRec2Obj(ByVal item As DataRowView, ByVal aktper As Person)
        Try
            With aktper
                .PersonenID = CInt(clsDBtools.fieldvalue(item("PERSONENID")))
                .Status = CInt(clsDBtools.fieldvalue(item("STATUS")))
                .Name = CStr(clsDBtools.fieldvalue(item("NACHNAME")))
                .Vorname = CStr(clsDBtools.fieldvalue(item("VORNAME")))
                .Bemerkung = CStr(clsDBtools.fieldvalue(item("BEMERKUNG")))
                .Namenszusatz = CStr(clsDBtools.fieldvalue(item("NAMENSZUSATZ")))
                '	MYGLOBALZ.SITZUNG.AKTPERSON.FDKURZ = CStr(clsDBtools.fieldvalue(item("FDKURZ")))
                .Anrede = CStr(clsDBtools.fieldvalue(item("ANREDE")))
                .Kontakt.Anschrift.Gemeindename = CStr(clsDBtools.fieldvalue(item("GEMEINDENAME")))
                .Kontakt.Anschrift.Strasse = CStr(clsDBtools.fieldvalue(item("STRASSE")))
                .Kontakt.Anschrift.Hausnr = CStr(clsDBtools.fieldvalue(item("HAUSNR")))
                .Kontakt.Anschrift.PLZ = (clsDBtools.fieldvalue(item("PLZ"))) 'ddr
                .Kontakt.Anschrift.Postfach = CStr(clsDBtools.fieldvalue(item("POSTFACH")))
                .Kontakt.Anschrift.PostfachPLZ = clsDBtools.fieldvalue((item("POSTFACHPLZ")))
                .Kontakt.Org.Name = CStr(clsDBtools.fieldvalue(item("ORGNAME")))
                .Kontakt.Org.Zusatz = CStr(clsDBtools.fieldvalue(item("ORGZUSATZ")))
                .Kontakt.Org.Typ1 = CStr(clsDBtools.fieldvalue(item("ORGTYP1")))
                .Kontakt.Org.Typ2 = CStr(clsDBtools.fieldvalue(item("ORGTYP2")))
                .Kontakt.Org.Eigentuemer = CStr(clsDBtools.fieldvalue(item("ORGEIGENTUEMER")))
                .Kontakt.GesellFunktion = CStr(clsDBtools.fieldvalue(item("GESELLFUNKTION")))
                .Kontakt.elektr.Telefon1 = CStr(clsDBtools.fieldvalue(item("FFTELEFON1")))
                .Kontakt.elektr.Telefon2 = CStr(clsDBtools.fieldvalue(item("FFTELEFON2")))
                .Kontakt.elektr.Fax1 = CStr(clsDBtools.fieldvalue(item("FFFAX1")))
                .Kontakt.elektr.Fax2 = CStr(clsDBtools.fieldvalue(item("FFFAX2")))
                .Kontakt.elektr.MobilFon = CStr(clsDBtools.fieldvalue(item("FFMOBILFON")))
                .Kontakt.elektr.Email = CStr(clsDBtools.fieldvalue(item("FFEMAIL")))
                .Kontakt.elektr.Homepage = CStr(clsDBtools.fieldvalue(item("FFHOMEPAGE")))
                '.Kontakt.Bankkonto.BankName = CStr(clsDBtools.fieldvalue(item("bvname")))
               ' .Kontakt.Bankkonto.Bic = CStr(clsDBtools.fieldvalue(item("blz")))
                '.Kontakt.Bankkonto.IBAN = CStr(clsDBtools.fieldvalue(item("IBAN")))
                '.Kontakt.Bankkonto.Titel = CStr(clsDBtools.fieldvalue(item("BVTITEL")))
                Try
                    .Kassenkonto = CStr(clsDBtools.fieldvalue(item("KASSENKONTO")))
                Catch ex As Exception

                End Try

                .Rolle = CStr(clsDBtools.fieldvalue(item("ROLLE")))
                .Bezirk = CStr(clsDBtools.fieldvalue(item("BEZIRK")))
                .Quelle = CStr(clsDBtools.fieldvalue(item("QUELLE")))
                .lastchange = CDate(clsDBtools.fieldvalueDate(item("lastchange")))
            End With
        Catch ex As Exception
            nachricht_und_Mbox("1 fehler in BeteiligtenRec2Obj: ggf. fehlt die rolle:" & ex.ToString)
        End Try
    End Sub
 


    Private Shared Function ausTabelleBeteiligte(ByVal vid%, ByVal idb As IDB_grundfunktionen) As DataTable ' myGlobalz.sitzung.beteiligteREC
        Dim BeteiligteImVorgang As DataTable
        clsBeteiligteBUSI.initBeteiligteDatatable(vid, idb) 'myGlobalz.sitzung.beteiligteREC wird aktualisert
        BeteiligteImVorgang = idb.dt.Copy

        clsDBtools.SpalteZuDatatableHinzufuegen(BeteiligteImVorgang, "STATUS", "System.Int16")
        clsDBtools.SpalteInitialisieren(BeteiligteImVorgang, "STATUS", 0)
        Return BeteiligteImVorgang
    End Function
 

    Public Shared Sub refreshBeteiligteListe_dt_erzeugenundMergen(ByVal vid%)
        Dim BeteiligteImVorgang As New DataTable 'bekommen den status=0
        BeteiligteImVorgang = ausTabelleBeteiligte(vid, myGlobalz.sitzung.beteiligteREC)
        clsDBtools.SpalteZuDatatableHinzufuegen(myGlobalz.sitzung.beteiligteREC.dt, "STATUS", "System.Int16")
        If vid = myGlobalz.sitzung.aktVorgangsID Then
            clsDBtools.SpalteInitialisieren(myGlobalz.sitzung.beteiligteREC.dt, "STATUS", 0)
        Else
            clsDBtools.SpalteInitialisieren(myGlobalz.sitzung.beteiligteREC.dt, "STATUS", 1)
        End If
        BeteiligteImVorgang.Dispose()
    End Sub

    ''' <summary>
    ''' myGlobalz.sitzung.personenRec wird gefüllt
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function erzeugeBeteiligtenListezuVorgang(ByVal sql As string, ByVal idb As IDB_grundfunktionen) As Boolean '  myGlobalz.sitzung.beteiligteREC
        Dim hinweis As String
        'idb.mydb.Tabelle ="beteiligte"
        idb.mydb.SQL = sql
        hinweis = idb.getDataDT()
        If idb.mycount < 1 Then
            nachricht("Keine beteiligte gespeichert!")
            Return False
        Else
            nachricht(String.Format("{0} beteiligte vorhanden", idb.mycount))
            Return True
        End If
    End Function


    ''' <summary>
    ''' personenRec wird aktualisert
    ''' </summary>
    ''' <param name="vid"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function initBeteiligteDatatable(ByVal vid As Integer, ByVal idb As IDB_grundfunktionen) As Boolean   ' myGlobalz.sitzung.beteiligteREC
        If clsBeteiligteBUSI.erzeugeBeteiligtenListezuVorgang("select * from  " & CLstart.myViewsNTabs.tabBeteiligte & " where vorgangsid=" & vid, idb) Then                'myGlobalz.sitzung.personenRec
            Return True
        Else
            nachricht("Es konnten keine Personen zu diesem Vorgang gefunden werden!")
            Return False
        End If
        Return True
    End Function

    Public Shared Function BeteiligteKopplungVorgang_alledb(ByVal zielvid%, ByVal aktRID%, ByVal status As Integer) As Integer
        'Dim newid%
        'If (myGlobalz.beteiligte_MYDB.dbtyp = "mysql") Then
        '    newid% = clsBeteiligteDBCRUD_MYSQL.Koppelung_Beteiligte_Vorgang(aktRID, zielvid, status)
        'End If
        'If (myGlobalz.beteiligte_MYDB.dbtyp = "oracle") Then
        '    Dim zzz As New clsBeteiligteDBCRUD_ORACLE(clsDBspecOracle.getConnection(myGlobalz.sitzung.VorgangREC.mydb))
        '    newid% = zzz.Koppelung_Beteiligte_Vorgang(aktRID, zielvid, status)
        '    ' result = zzz.loescheDokument(dokid)
        '    zzz.Dispose()
        'End If
        'Return newid
    End Function

    Public Shared Function BeteiligteAbspeichernNeu_AlleDB(ByVal vid%, ByVal lpers As Person) As Integer
        'Dim personenid%
        'If (myGlobalz.beteiligte_MYDB.dbtyp = "mysql") Then
        '    Dim zzz As New clsBeteiligteDBCRUD_MYSQL(clsDBspecMYSQL.getConnection(myGlobalz.beteiligte_MYDB))
        '    personenid% = zzz.Beteiligte_abspeichern_Neu(vid, lpers)
        '    zzz.Dispose()
        'End If
        'If (myGlobalz.beteiligte_MYDB.dbtyp = "oracle") Then
        '    Dim zzz As New clsBeteiligteDBCRUD_ORACLE(clsDBspecOracle.getConnection(myGlobalz.beteiligte_MYDB))
        '    personenid% = zzz.Beteiligte_abspeichern_Neu(vid, lpers)
        '    zzz.Dispose()
        'End If
        'Return personenid
    End Function


    'Public Shared Function Beteiliten_loeschen_AlleDB(ByVal pid As Integer) As Integer 'myGlobalz.sitzung.aktPerson.PersonenID
    '    Dim personenid%
    '    If (myGlobalz.beteiligte_MYDB.dbtyp = "mysql") Then
    '        Dim zzz As New clsBeteiligteDBCRUD_MYSQL(clsDBspecMYSQL.getConnection(myGlobalz.beteiligte_MYDB))
    '        personenid% = zzz.Beteiliten_loeschen(pid)
    '        zzz.Dispose()
    '    End If
    '    If (myGlobalz.beteiligte_MYDB.dbtyp = "oracle") Then
    '        Dim zzz As New clsBeteiligteDBCRUD_ORACLE(clsDBspecOracle.getConnection(myGlobalz.beteiligte_MYDB))
    '        personenid% = zzz.Beteiliten_loeschen(pid)
    '           zzz.Dispose
    '    End If
    '    Return personenid
    'End Function
    'Dim erfolg% = DBactionParadigma.Beteiliten_loeschen(myGlobalz.sitzung.aktPerson.PersonenID)
    'Public Shared Function personAusVorgangEntfernen(ByVal personenid%, ByVal vorgangsid%, ByVal personenStatus as integer) as  Boolean 'myGlobalz.sitzung.aktPerson.PersonenID,myGlobalz.sitzung.VorgangsID,myGlobalz.sitzung.aktPerson.Status
    '    If CInt(personenStatus%) = 0 Then
    '        Dim erfolg% = clsBeteiligteBUSI.Beteiliten_loeschen_AlleDB(personenid)
    '        If erfolg < 1 Then
    '            nachricht_und_Mbox(String.Format("Problem beim Entkoppeln von Vorgang und Person: {0} {1}", vorgangsid, personenid))
    '            Return False
    '        Else
    '            Return True
    '        End If
    '    Else
    '        'Entkoppeln durch löschen aus der kopplungstabelle
    '        'ist gedacht für die verwandten vorgänge(status <>1)
    '        If (myGlobalz.beteiligte_MYDB.dbtyp = "mysql") Then
    '            nachricht(clsBeteiligteDBCRUD_MYSQL.Entkoppelung_Beteiligte_Vorgang(personenid, vorgangsid).ToString)
    '        End If
    '        If (myGlobalz.beteiligte_MYDB.dbtyp = "oracle") Then
    '            nachricht(clsBeteiligteDBCRUD_ORACLE.Entkoppelung_Beteiligte_Vorgang(personenid, vorgangsid).ToString)
    '        End If
    '    End If
    'End Function

    Shared Sub personZuZielvorgangKopieren(ByVal personenID As Integer, ByVal status As Integer, ByVal Quellvorgangsid As Integer, ByVal Zielvorgangsid As Integer)
        glob2.NeueBeteiligteAbspeichern(Zielvorgangsid, myGlobalz.sitzung.aktPerson)
    End Sub

    Shared Sub Anschrift_generieren()
        Dim text As String = clsBeteiligteBUSI.Anschrift_Text_erzeugen(myGlobalz.sitzung.aktPerson)
        Clipboard.Clear()
        Clipboard.SetText(text)
        MsgBox(text$ &
               vbCrLf & vbCrLf &
               "Die Anschrift befindet sich nun in Ihrer Windows-Zwischenablage. " & vbCrLf & _
               "Sie können Sie mit Strg-v in Ihr Dokument einfügen." & vbCrLf)
    End Sub
End Class
