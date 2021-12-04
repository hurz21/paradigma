Imports System.Data
#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB.LIBDB
Imports paradigmaDetail
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Public Class clsBeteiligteBUSI
    'Private Shared Function viaKopplung_VorgangID_zu_BeteiligteID_OHNEKOPPLUNG(ByVal vorgangsid As String) As Boolean
    '    Dim hinweis As String
    '    myglobalz.sitzung.tempREC.mydb.Host = myglobalz.sitzung.VorgangREC.mydb.Host
    '    myglobalz.sitzung.tempREC.mydb.Schema = myglobalz.sitzung.VorgangREC.mydb.Schema
    '    myglobalz.sitzung.tempREC.mydb.Tabelle ="Beteiligte"     ''& " order by ts desc"
    '    myglobalz.sitzung.tempREC.mydb.SQL =
    '                                    "SELECT * FROM " & myglobalz.sitzung.tempREC.mydb.Tabelle &
    '                                    " where VorgangsID=" & vorgangsid
    '    hinweis = myglobalz.sitzung.tempREC.getDataDT()
    '    If myglobalz.sitzung.tempREC.mycount < 1 Then
    '        nachricht("Keine Beteiligten gespeichert!")
    '        Return False
    '    Else
    '        nachricht(String.Format("{0} Ereignisse vorhanden", myglobalz.sitzung.tempREC.mycount))
    '        Return True
    '    End If
    'End Function


    'Public Shared Function BeteiligtelinksKopieren(ByVal zielvid%, ByVal instring As String) As Boolean
    '    nachricht("in BeteiligtelinksKopieren ------------------------------------") 'BeteiligtelinksKopieren
    '    Dim newid%
    '    Dim erfolgreich As Boolean = False
    '    Dim a() As String = instring.Split(","c)
    '    Try
    '        For i = 0 To a.GetUpperBound(0) - 1
    '            newid = BeteiligteKopplungVorgang_alledb(zielvid, CInt(a(i)), 1)
    '            If newid < 1 Then
    '                nachricht_und_Mbox("Kopie konnte nicht angelegt werden: ")
    '                erfolgreich = False
    '            Else
    '                nachricht("Kopie konnte angelegt werden: ")
    '                erfolgreich = True
    '            End If
    '        Next
    '        Return erfolgreich
    '    Catch ex As Exception
    '        nachricht_und_Mbox("Fehler bei BeteiligtelinksKopieren: " ,ex)
    '        nachricht_und_Mbox("Fehler bei BeteiligtelinksKopieren: ")
    '        Return True
    '    End Try
    'End Function

    Friend Shared Function hatBankverbindung(personenID As Integer) As String
        Dim iban As String
        iban = bvTools.bv3PersonenID(personenID)
        If iban.IsNothingOrEmpty Then
            Return ""
        Else
            Return iban
        End If

    End Function

    Public Shared Function Anschrift_Text_erzeugen(ByVal ap As Person) As String 'myGlobalz.sitzung.aktPerson
        Try
            Dim sb As New Text.StringBuilder
            If Not String.IsNullOrEmpty(ap.Kontakt.Org.Name) Then sb.Append(ap.Kontakt.Org.Name & vbCrLf)
            If Not String.IsNullOrEmpty(ap.Kontakt.Org.Zusatz) Then sb.Append(ap.Kontakt.Org.Zusatz & vbCrLf)
            Dim ared As String

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
            nachricht("Fehler: Anschrift_Text_erzeugen: ", ex)
            Return " -------------------- "
        End Try
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="item">eingang</param>
    ''' <param name="aktper">ausgang</param>
    ''' <remarks>overloaded!!!</remarks>
    'Public Shared Sub BeteiligtenRec2Obj(ByVal item As DataRow, ByVal aktper As Person)
    '    Try
    '        With aktper
    '            .PersonenID = CInt(clsDBtools.fieldvalue(item("PERSONENID")))
    '            .Status = CInt(clsDBtools.fieldvalue(item("STATUS")))
    '            .Name = CStr(clsDBtools.fieldvalue(item("NACHNAME")))
    '            .Vorname = CStr(clsDBtools.fieldvalue(item("VORNAME")))
    '            .Bemerkung = CStr(clsDBtools.fieldvalue(item("BEMERKUNG")))
    '            .Namenszusatz = CStr(clsDBtools.fieldvalue(item("NAMENSZUSATZ")))
    '            '	MYGLOBALZ.SITZUNG.AKTPERSON.FDKURZ = CStr(clsDBtools.fieldvalue(item("FDKURZ")))
    '            .Anrede = CStr(clsDBtools.fieldvalue(item("ANREDE")))
    '            .Kontakt.Anschrift.Gemeindename = CStr(clsDBtools.fieldvalue(item("GEMEINDENAME")))
    '            .Kontakt.Anschrift.Strasse = CStr(clsDBtools.fieldvalue(item("STRASSE")))
    '            .Kontakt.Anschrift.Hausnr = CStr(clsDBtools.fieldvalue(item("HAUSNR")))
    '            .Kontakt.Anschrift.PLZ = (clsDBtools.fieldvalue(item("PLZ")))
    '            .Kontakt.Anschrift.Postfach = CStr(clsDBtools.fieldvalue(item("POSTFACH")))
    '            .Kontakt.Anschrift.PostfachPLZ = clsDBtools.fieldvalue((item("POSTFACHPLZ")))
    '            .Kontakt.Org.Name = CStr(clsDBtools.fieldvalue(item("ORGNAME")))
    '            .Kontakt.Org.Zusatz = CStr(clsDBtools.fieldvalue(item("ORGZUSATZ")))
    '            .Kontakt.Org.Typ1 = CStr(clsDBtools.fieldvalue(item("ORGTYP1")))
    '            .Kontakt.Org.Typ2 = CStr(clsDBtools.fieldvalue(item("ORGTYP2")))
    '            .Kontakt.Org.Eigentuemer = CStr(clsDBtools.fieldvalue(item("ORGEIGENTUEMER")))
    '            .Kontakt.GesellFunktion = CStr(clsDBtools.fieldvalue(item("GESELLFUNKTION")))
    '            .Kontakt.elektr.Telefon1 = CStr(clsDBtools.fieldvalue(item("FFTELEFON1")))
    '            .Kontakt.elektr.Telefon2 = CStr(clsDBtools.fieldvalue(item("FFTELEFON2")))
    '            .Kontakt.elektr.Fax1 = CStr(clsDBtools.fieldvalue(item("FFFAX1")))
    '            .Kontakt.elektr.Fax2 = CStr(clsDBtools.fieldvalue(item("FFFAX2")))
    '            .Kontakt.elektr.MobilFon = CStr(clsDBtools.fieldvalue(item("FFMOBILFON")))
    '            .Kontakt.elektr.Email = CStr(clsDBtools.fieldvalue(item("FFEMAIL")))
    '            .Kontakt.elektr.Homepage = CStr(clsDBtools.fieldvalue(item("FFHOMEPAGE")))
    '            .PersonenVorlage = CInt(clsDBtools.fieldvalue(item("PERSONENVORLAGE")))
    '            .VERTRETENDURCH = CStr(clsDBtools.fieldvalue(item("VERTRETENDURCH")))
    '            '.Kontakt.Bankkonto.BankName = CStr(clsDBtools.fieldvalue(item("BankName")))
    '            '.Kontakt.Bankkonto.BIC = CStr(clsDBtools.fieldvalue(item("BIC")))
    '            '.Kontakt.Bankkonto.IBAN = CStr(clsDBtools.fieldvalue(item("IBAN")))
    '            '.Kontakt.Bankkonto.Titel = CStr(clsDBtools.fieldvalue(item("BVTITEL")))
    '            Try
    '                .Kassenkonto = CStr(clsDBtools.fieldvalue(item("KASSENKONTO")))
    '            Catch ex As Exception

    '            End Try

    '            .Rolle = CStr(clsDBtools.fieldvalue(item("ROLLE")))
    '            .Bezirk = CStr(clsDBtools.fieldvalue(item("BEZIRK")))
    '            .lastchange = CDate(clsDBtools.fieldvalueDate(item("lastchange")))
    '        End With
    '    Catch ex As Exception
    '        nachricht_und_Mbox("2 fehler in BeteiligtenRec2Obj(Datarow): ggf. fehlt die rolle:" ,ex)
    '    End Try
    'End Sub
    ''' <summary>
    ''' 
    ''' </summary>
    ''' <param name="item">eingang datarowview</param>
    ''' <param name="aktper">ausgang ein objekt der marke person</param>
    ''' <remarks>overloaded!!</remarks>
    'Public Shared Sub BeteiligtenRec2Obj(ByVal item As DataRowView, ByVal aktper As Person)
    '    Try
    '        With aktper
    '            .PersonenID = CInt(clsDBtools.fieldvalue(item("PERSONENID")))
    '            .Status = CInt(clsDBtools.fieldvalue(item("STATUS")))
    '            .Name = CStr(clsDBtools.fieldvalue(item("NACHNAME")))
    '            .Vorname = CStr(clsDBtools.fieldvalue(item("VORNAME")))
    '            .Bemerkung = CStr(clsDBtools.fieldvalue(item("BEMERKUNG")))
    '            .Namenszusatz = CStr(clsDBtools.fieldvalue(item("NAMENSZUSATZ")))
    '            '	MYGLOBALZ.SITZUNG.AKTPERSON.FDKURZ = CStr(clsDBtools.fieldvalue(item("FDKURZ")))
    '            .Anrede = CStr(clsDBtools.fieldvalue(item("ANREDE")))
    '            .Kontakt.Anschrift.Gemeindename = CStr(clsDBtools.fieldvalue(item("GEMEINDENAME")))
    '            .Kontakt.Anschrift.Strasse = CStr(clsDBtools.fieldvalue(item("STRASSE")))
    '            .Kontakt.Anschrift.Hausnr = CStr(clsDBtools.fieldvalue(item("HAUSNR")))
    '            .Kontakt.Anschrift.PLZ = (clsDBtools.fieldvalue(item("PLZ"))) 'ddr
    '            .Kontakt.Anschrift.Postfach = CStr(clsDBtools.fieldvalue(item("POSTFACH")))
    '            .Kontakt.Anschrift.PostfachPLZ = clsDBtools.fieldvalue((item("POSTFACHPLZ")))
    '            .Kontakt.Org.Name = CStr(clsDBtools.fieldvalue(item("ORGNAME")))
    '            .Kontakt.Org.Zusatz = CStr(clsDBtools.fieldvalue(item("ORGZUSATZ")))
    '            .Kontakt.Org.Typ1 = CStr(clsDBtools.fieldvalue(item("ORGTYP1")))
    '            .Kontakt.Org.Typ2 = CStr(clsDBtools.fieldvalue(item("ORGTYP2")))
    '            .Kontakt.Org.Eigentuemer = CStr(clsDBtools.fieldvalue(item("ORGEIGENTUEMER")))
    '            .Kontakt.GesellFunktion = CStr(clsDBtools.fieldvalue(item("GESELLFUNKTION")))
    '            .Kontakt.elektr.Telefon1 = CStr(clsDBtools.fieldvalue(item("FFTELEFON1")))
    '            .Kontakt.elektr.Telefon2 = CStr(clsDBtools.fieldvalue(item("FFTELEFON2")))
    '            .Kontakt.elektr.Fax1 = CStr(clsDBtools.fieldvalue(item("FFFAX1")))
    '            .Kontakt.elektr.Fax2 = CStr(clsDBtools.fieldvalue(item("FFFAX2")))
    '            .Kontakt.elektr.MobilFon = CStr(clsDBtools.fieldvalue(item("FFMOBILFON")))
    '            .Kontakt.elektr.Email = CStr(clsDBtools.fieldvalue(item("FFEMAIL")))
    '            .Kontakt.elektr.Homepage = CStr(clsDBtools.fieldvalue(item("FFHOMEPAGE")))
    '            .PersonenVorlage = CInt(clsDBtools.fieldvalue(item("PERSONENVORLAGE")))
    '            .VERTRETENDURCH = CStr(clsDBtools.fieldvalue(item("VERTRETENDURCH")))
    '            '.Kontakt.Bankkonto.BankName = CStr(clsDBtools.fieldvalue(item("bvname")))
    '            ' .Kontakt.Bankkonto.Bic = CStr(clsDBtools.fieldvalue(item("blz")))
    '            '.Kontakt.Bankkonto.IBAN = CStr(clsDBtools.fieldvalue(item("IBAN")))
    '            '.Kontakt.Bankkonto.Titel = CStr(clsDBtools.fieldvalue(item("BVTITEL")))
    '            Try
    '                .Kassenkonto = CStr(clsDBtools.fieldvalue(item("KASSENKONTO")))
    '            Catch ex As Exception

    '            End Try

    '            .Rolle = CStr(clsDBtools.fieldvalue(item("ROLLE")))
    '            .Bezirk = CStr(clsDBtools.fieldvalue(item("BEZIRK")))
    '            .Quelle = CStr(clsDBtools.fieldvalue(item("QUELLE")))
    '            .lastchange = CDate(clsDBtools.fieldvalueDate(item("lastchange")))
    '        End With
    '    Catch ex As Exception
    '        nachricht_und_Mbox("1 fehler in BeteiligtenRec2Obj: ggf. fehlt die rolle:" ,ex)
    '    End Try
    'End Sub



    Private Shared Function ausTabelleBeteiligte(ByVal vid%, ByVal idb As IDB_grundfunktionen) As DataTable ' myGlobalz.sitzung.beteiligteREC
        Dim BeteiligteImVorgang As DataTable
        clsBeteiligteBUSI.initBeteiligteDatatable(vid, idb) 'myGlobalz.sitzung.beteiligteREC wird aktualisert
        BeteiligteImVorgang = idb.dt.Copy

        clsDBtools.SpalteZuDatatableHinzufuegen(BeteiligteImVorgang, "STATUS", "System.Int16")
        clsDBtools.SpalteInitialisieren(BeteiligteImVorgang, "STATUS", 0)
        Return BeteiligteImVorgang
    End Function


    Public Shared Async Function refreshBeteiligteListe_dt_erzeugenundMergen(ByVal vid%) As System.Threading.Tasks.Task(Of Boolean)
        Dim BeteiligteImVorgang As New DataTable 'bekommen den status=0
        BeteiligteImVorgang = ausTabelleBeteiligte(vid, myGlobalz.sitzung.beteiligteREC)
        clsDBtools.SpalteZuDatatableHinzufuegen(myGlobalz.sitzung.beteiligteREC.dt, "STATUS", "System.Int16")
        If vid = myGlobalz.sitzung.aktVorgangsID Then
            clsDBtools.SpalteInitialisieren(myGlobalz.sitzung.beteiligteREC.dt, "STATUS", 0)
        Else
            clsDBtools.SpalteInitialisieren(myGlobalz.sitzung.beteiligteREC.dt, "STATUS", 1)
        End If
        BeteiligteImVorgang.Dispose()
        Return True
    End Function

    ''' <summary>
    ''' myGlobalz.sitzung.personenRec wird gefüllt
    ''' </summary>
    ''' <param name="sql"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function erzeugeBeteiligtenListezuVorgang(ByVal sql As String, ByVal idb As IDB_grundfunktionen) As Boolean '  myGlobalz.sitzung.beteiligteREC
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

    Public Shared Sub holeBeteiligte()
        'myglobalz.sitzung.tempREC.mydb.Tabelle ="beteiligte" 
        myGlobalz.sitzung.tempREC.mydb.SQL =
                  "SELECT concat(NACHNAME,', ',' ',ORGNAME,', ',GEMEINDENAME,'; ',' ',' KASSENKONTO: ',KASSENKONTO)  AS ABSTRACT FROM  " & CLstart.myViewsNTabs.tabBeteiligte & " " &
                           "  where vorgangsid=" & myGlobalz.sitzung.aktVorgangsID & " order by	abstract  asc"
        nachricht(myGlobalz.sitzung.tempREC.getDataDT())
    End Sub
    ''' <summary>
    ''' personenRec wird aktualisert
    ''' </summary>
    ''' <param name="vid"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function initBeteiligteDatatable(ByVal vid As Integer, ByVal idb As IDB_grundfunktionen) As Boolean   ' myGlobalz.sitzung.beteiligteREC
        If clsBeteiligteBUSI.erzeugeBeteiligtenListezuVorgang("select * from  " & CLstart.myViewsNTabs.tabBeteiligte & " where vorgangsid=" & vid & " order by nachname", idb) Then                'myGlobalz.sitzung.personenRec
            Return True
        Else
            nachricht("Es konnten keine Personen zu diesem Vorgang gefunden werden!")
            Return False
        End If
        Return True
    End Function



    Public Shared Function BeteiligteAbspeichernNeu_AlleDB(ByVal vid As Integer, ByVal lpers As Person) As Integer
        Dim personenid As Integer
        Dim querie As String
        werteDBsicherMachenBeteiligte(lpers)
        clsSqlparam.paramListe.Clear()
        'populateParamListeEreignis(zielvorgangsid, ereignis, clsSqlparam.paramListe)
        'clsSqlparam.paramListe.Add(New clsSqlparam("eid", 0))
        querie = "INSERT INTO " & CLstart.myViewsNTabs.tabBeteiligte & " (VORGANGSID,NACHNAME,VORNAME,BEMERKUNG,NAMENSZUSATZ,ANREDE,QUELLE,GEMEINDENAME,STRASSE,HAUSNR,POSTFACH,POSTFACHPLZ,FFTELEFON1,FFTELEFON2,FFFAX1," +
                        "FFFAX2,FFMOBILFON,FFEMAIL,FFHOMEPAGE,GESELLFUNKTION,ORGNAME,ORGZUSATZ,ORGTYP1,ORGTYP2,ORGEIGENTUEMER,ROLLE,KASSENKONTO,PLZ,BEZIRK,LASTCHANGE,PERSONENVORLAGE,VERTRETENDURCH) " +
                        " VALUES (@VORGANGSID,@NACHNAME,@VORNAME,@BEMERKUNG,@NAMENSZUSATZ,@ANREDE,@QUELLE,@GEMEINDENAME,@STRASSE,@HAUSNR,@POSTFACH,@POSTFACHPLZ,@FFTELEFON1,@FFTELEFON2,@FFFAX1," +
                        "@FFFAX2,@FFMOBILFON,@FFEMAIL,@FFHOMEPAGE,@GESELLFUNKTION,@ORGNAME,@ORGZUSATZ,@ORGTYP1,@ORGTYP2,@ORGEIGENTUEMER,@ROLLE,@KASSENKONTO,@PLZ,@BEZIRK,@LASTCHANGE,@PERSONENVORLAGE,@VERTRETENDURCH)"

        populateBeteiligte(vid, lpers)

        personenid% = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "PERSONENID")


        Return personenid
    End Function

    Shared Sub populateBeteiligte(vid As Integer, lpers As Person)
        With lpers
            clsSqlparam.paramListe.Add(New clsSqlparam("NACHNAME", .Name))
            clsSqlparam.paramListe.Add(New clsSqlparam("VORNAME", .Vorname))
            clsSqlparam.paramListe.Add(New clsSqlparam("BEMERKUNG", .Bemerkung.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("NAMENSZUSATZ", .Namenszusatz.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("ANREDE", .Anrede.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("QUELLE", .Quelle.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("GEMEINDENAME", .Kontakt.Anschrift.Gemeindename.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("STRASSE", .Kontakt.Anschrift.Strasse.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("HAUSNR", .Kontakt.Anschrift.Hausnr.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("PLZ", .Kontakt.Anschrift.PLZ.ToString.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("POSTFACH", .Kontakt.Anschrift.Postfach.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("POSTFACHPLZ", .Kontakt.Anschrift.PostfachPLZ.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("FFTELEFON1", .Kontakt.elektr.Telefon1.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("FFTELEFON2", .Kontakt.elektr.Telefon2.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("FFFAX1", .Kontakt.elektr.Fax1.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("FFFAX2", .Kontakt.elektr.Fax2.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("FFMOBILFON", .Kontakt.elektr.MobilFon.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("FFEMAIL", .Kontakt.elektr.Email.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("FFHOMEPAGE", .Kontakt.elektr.Homepage.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("GESELLFUNKTION", .Kontakt.GesellFunktion.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("ORGNAME", .Kontakt.Org.Name.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("ORGZUSATZ", .Kontakt.Org.Zusatz.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("ORGTYP1", .Kontakt.Org.Typ1.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("ORGTYP2", .Kontakt.Org.Typ2.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("ORGEIGENTUEMER", .Kontakt.Org.Eigentuemer.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("ROLLE", .Rolle.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("BEZIRK", .Bezirk.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("KASSENKONTO", .Kassenkonto.Trim))
            clsSqlparam.paramListe.Add(New clsSqlparam("LASTCHANGE",
                                                                                clsDBtools.makedateMssqlConform(Now, myGlobalz.sitzung.VorgangREC.mydb.dbtyp)))

            clsSqlparam.paramListe.Add(New clsSqlparam("PERSONENVORLAGE", .PersonenVorlage))
            clsSqlparam.paramListe.Add(New clsSqlparam("VERTRETENDURCH", .VERTRETENDURCH))
        End With
        'clsSqlparam.paramListe.Add(New clsSqlparam("KASSENKONTO", lpers.Kassenkonto))
        clsSqlparam.paramListe.Add(New clsSqlparam("VORGANGSID", vid))
    End Sub

    Shared Sub werteDBsicherMachenBeteiligte(lpers As Person)
        If String.IsNullOrEmpty(lpers.Kontakt.Anschrift.PostfachPLZ) Then lpers.Kontakt.Anschrift.PostfachPLZ = ""
        If String.IsNullOrEmpty(lpers.Name) Then lpers.Name = " "
        If String.IsNullOrEmpty(lpers.Vorname) Then lpers.Vorname = " "
        If String.IsNullOrEmpty(lpers.Bezirk) Then lpers.Bezirk = " "
        If String.IsNullOrEmpty(lpers.Quelle) Then lpers.Quelle = myGlobalz.sitzung.aktBearbeiter.Initiale

        If String.IsNullOrEmpty(lpers.Bemerkung.Trim) Then
            If lpers.Kontakt.elektr.Telefon1.Length > 240 Then
                lpers.Bemerkung = lpers.Kontakt.elektr.Telefon1.Substring(0, 240)
            Else
                lpers.Bemerkung = lpers.Kontakt.elektr.Telefon1
            End If

        End If
        If lpers.Namenszusatz.Length > 46 Then
            lpers.Namenszusatz = lpers.Namenszusatz.Substring(0, 45)
        End If

        If lpers.Kontakt.elektr.Telefon1.Length > 99 Then
            lpers.Kontakt.elektr.Telefon1 = lpers.Kontakt.elektr.Telefon1.Substring(0, 98)
        End If
    End Sub

    Public Shared Function Beteiliten_loeschen_AlleDB(ByVal pid As Integer) As Integer 'myGlobalz.sitzung.aktPerson.PersonenID
        Dim hinweis As String = ""
        myGlobalz.sitzung.VorgangREC.mydb.SQL = "delete from  " & CLstart.myViewsNTabs.tabBeteiligte & " where PersonenID=" & pid
        myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC, hinweis)
        Return 1
    End Function
    Public Shared Function personAusVorgangEntfernen(ByVal personenid%, ByVal vorgangsid%, ByVal personenStatus As Integer) As Boolean 'myGlobalz.sitzung.aktPerson.PersonenID,myGlobalz.sitzung.VorgangsID,myGlobalz.sitzung.aktPerson.Status
        Dim hinweis As String = ""
        If CInt(personenStatus%) = 0 Then
            Dim erfolg% = clsBeteiligteBUSI.Beteiliten_loeschen_AlleDB(personenid)
            If erfolg < 1 Then
                nachricht_und_Mbox(String.Format("Problem beim Entkoppeln von Vorgang und Person: {0} {1}", vorgangsid, personenid))
                Return False
            Else
                Return True
            End If
        Else
            'Entkoppeln durch löschen aus der kopplungstabelle
            'ist gedacht für die verwandten vorgänge(status <>1)

            myGlobalz.sitzung.VorgangREC.mydb.SQL = "delete from " & CLstart.myViewsNTabs.tabKoppBeteiligteVorgang & "" &
                 " where  BeteiligteID=" & personenid &
                 " and VorgangsID=" & vorgangsid
            myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myGlobalz.sitzung.VorgangREC.mydb.SQL, myGlobalz.sitzung.VorgangREC, hinweis)
            Return True
        End If
    End Function

    Shared Sub personZuZielvorgangKopieren(ByVal personenID As Integer, ByVal status As Integer, ByVal Quellvorgangsid As Integer, ByVal Zielvorgangsid As Integer)
        glob2.NeueBeteiligteAbspeichern(Zielvorgangsid, myGlobalz.sitzung.aktPerson)
    End Sub

    Shared Sub Anschrift_generieren()
        Dim text As String = clsBeteiligteBUSI.Anschrift_Text_erzeugen(myGlobalz.sitzung.aktPerson)
        Clipboard.Clear()
        Clipboard.SetText(text)
        MsgBox(text$ &
               vbCrLf & vbCrLf &
               "Die Anschrift befindet sich nun in Ihrer Windows-Zwischenablage. " & vbCrLf &
               "Sie können Sie mit Strg-v in Ihr Dokument einfügen." & vbCrLf)
    End Sub

    Friend Shared Function ConvertDatatable2Personenliste(dt As DataTable) As List(Of Person)
        Dim tListe As New List(Of Person)
        Dim temp As New Person
        Dim item As DataRow
        Try
            l(" MOD ConvertDatatable2Peron anfang")
            For i = 0 To dt.Rows.Count - 1
                item = CType(dt.Rows(i), DataRow)
                temp = convertItem2person(item)
                tListe.Add(temp)
            Next
            l(" MOD ConvertDatatable2Peron ende")
            Return tListe
        Catch ex As Exception
            l("Fehler in ConvertDatatable2Peron: ", ex)
            Return tListe
        End Try
    End Function

    Shared Function convertItem2person(item As DataRow) As Person
        Dim aktper As New Person
        Try
            l(" MOD convertItem2person anfang")
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
                .PersonenVorlage = CInt(clsDBtools.fieldvalue(item("PERSONENVORLAGE")))
                .VERTRETENDURCH = CStr(clsDBtools.fieldvalue(item("VERTRETENDURCH")))
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
            l(" MOD convertItem2person ende")
            Return aktper
        Catch ex As Exception
            l("Fehler in convertItem2person: ", ex)
            Return aktper
        End Try
    End Function
End Class
