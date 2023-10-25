Imports System.Data

Public Class clsTools


    Sub initDB()
        'ParaRec wird in module 1  als sqls definiert
        ParaRec.mydb = New clsDatenbankZugriff
#If DEBUG Then
        ParaRec.mydb.Host = "kh-w-sql02"
        ParaRec.mydb.Schema = "Paradigma"
        ParaRec.mydb.ServiceName = ""
        ParaRec.mydb.Tabelle = "stakeholder"
        ParaRec.mydb.username = "sgis"
        ParaRec.mydb.password = "WinterErschranzt.74"
        ParaRec.mydb.dbtyp = "sqls"
#Else
        ParaRec.mydb.Host = "kh-w-sql02"
        ParaRec.mydb.Schema = "Paradigma"
        ParaRec.mydb.ServiceName = ""
        ParaRec.mydb.Tabelle = "stakeholder"
        ParaRec.mydb.username = "sgis"
        ParaRec.mydb.password = "WinterErschranzt.74"
        ParaRec.mydb.dbtyp = "sqls"
        'ParaRec.mydb.Host = "ora-clu-vip-004"
        '       ParaRec.mydb.Schema = "paradigma"
        '       ParaRec.mydb.ServiceName = "paradigma.kreis-of.local"
        '       ParaRec.mydb.Tabelle = "stakeholder"
        '       ParaRec.mydb.username = "paradigma"
        '       ParaRec.mydb.password = "luftikus12"
        '       ParaRec.mydb.dbtyp = "oracle"
#End If


        HALOREC.mydb = New clsDatenbankZugriff
        HALOREC.mydb.Host = "w2gis02"
        HALOREC.mydb.Schema = "postgis20"
        HALOREC.mydb.Tabelle = "public.halofs"
        HALOREC.mydb.username = "postgres"
        HALOREC.mydb.password = "lkof4"
    End Sub

    Shared Sub schliessenButton_einschalten(ByVal btn As Button)
        If Not btn Is Nothing Then
            btn.IsEnabled = True
            btn.Visibility = Visibility.Visible
        End If
    End Sub

    Public Shared Sub istTextzulang(ByVal maxlen%, ByVal tb As TextBox)
        Try
            If tb Is Nothing Then Exit Sub
            If tb.Text.Length > maxlen% Then
                MessageBox.Show("Der Text ist zu lang: " & vbCrLf &
                 tb.Text.Length & " statt maximal " & maxlen & " Zeichen." & vbCrLf _
                 & "Der Text wird am Ende abgeschnitten!", "Eingabe zu lang", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK)
                tb.Text = tb.Text.Substring(0, maxlen - 1)
            End If
        Catch ex As Exception
            l(ex.ToString)
        End Try
    End Sub

    Public Shared Sub PersonUebernehmen(ByVal item As DataRowView, ByVal aktperson As Person)
        Try
            With aktperson
                .clear()
                .Name = CStr(clsDBtools.fieldvalue(item("NachName"))).ToString
                .Vorname = CStr(clsDBtools.fieldvalue(item("Vorname"))).ToString
                .Bemerkung = CStr(clsDBtools.fieldvalue(item("Bemerkung"))).ToString
                .Namenszusatz = CStr(clsDBtools.fieldvalue(item("Namenszusatz"))).ToString()
                .Anrede = CStr(clsDBtools.fieldvalue(item("Anrede"))).ToString()
                .Bezirk = CStr(clsDBtools.fieldvalue(item("Bezirk"))).ToString()
                .Rolle = CStr(clsDBtools.fieldvalue(item("Rolle"))).ToString()
                .Kontakt.clear()
                .Kontakt.GesellFunktion = CStr(clsDBtools.fieldvalue(item("GesellFunktion"))).ToString()
                .Kontakt.Bemerkung = "Quelle: VorgangsDB"
                .Kontakt.Anschrift.Gemeindename = CStr(clsDBtools.fieldvalue(item("Gemeindename"))).ToString()
                .Kontakt.Anschrift.Strasse = CStr(clsDBtools.fieldvalue(item("Strasse"))).ToString()
                .Kontakt.Anschrift.Hausnr = CStr(clsDBtools.fieldvalue(item("Hausnr"))).ToString()
                .Kontakt.Anschrift.PLZ = (CStr(clsDBtools.fieldvalue(item("PLZ"))).ToString())
                .Kontakt.elektr.Telefon1 = (CStr(clsDBtools.fieldvalue(item("fftelefon1"))).ToString())
                .Kontakt.elektr.Telefon2 = (CStr(clsDBtools.fieldvalue(item("fftelefon2"))).ToString())
                .Kontakt.elektr.Fax1 = (CStr(clsDBtools.fieldvalue(item("fffax1"))).ToString())
                .Kontakt.elektr.Fax2 = (CStr(clsDBtools.fieldvalue(item("fffax2"))).ToString())

                .Kontakt.elektr.MobilFon = (CStr(clsDBtools.fieldvalue(item("FFMobilFon"))).ToString())
                .Kontakt.elektr.Homepage = (CStr(clsDBtools.fieldvalue(item("FFHomepage"))).ToString())

                .Kontakt.elektr.Email = (CStr(clsDBtools.fieldvalue(item("ffemail"))).ToString())
                .Kontakt.Org.Name = (CStr(clsDBtools.fieldvalue(item("orgname"))).ToString())
                .Kontakt.Org.Zusatz = (CStr(clsDBtools.fieldvalue(item("orgzusatz"))).ToString())

                .changed_Anschrift = True
                .PersonenID = CInt(clsDBtools.fieldvalue(item("personenid")))
                .Kassenkonto = (CStr(clsDBtools.fieldvalue(item("KASSENKONTO"))).ToString())
            End With
        Catch ex As Exception
            MsgBox("Fehler bei der Übernahme von Daten aus der Vorgangsdatenbank!")
        End Try
    End Sub

    Public Shared Sub holeStrasseDT()
        'HALOREC.mydb.Schema = "halosort"
        'HALOREC.mydb.Tabelle = "halofs"
        HALOREC.mydb.SQL = "select distinct strcode ,trim(sname) as sname from " &
                              HALOREC.mydb.Tabelle &
                           " where gemeindenr = " & aktADR.Gisadresse.gemeindeNrBig() &
                           " order by trim(sname) asc"
        l(HALOREC.getDataDT())
    End Sub

    Public Shared Sub DBholeHausnrDT()
        'HALOREC.mydb.Schema = "halosort"
        'HALOREC.mydb.Tabelle = "halofs"
        'HALOREC.mydb.SQL = String.Format("select distinct id ,cast(concat(hausnr,zusatz) as CHAR) as hausnrkombi , hausnr,zusatz from {0} where gemeindenr = {1} and strcode = {2} order by  hausnr,zusatz",
        HALOREC.mydb.SQL = String.Format("select   distinct id,hausnrkombi from public.halofs where gemeindenr = {0} and strcode = {1}  order by hausnrkombi",
      aktADR.Gisadresse.gemeindeNrBig(), aktADR.Gisadresse.strasseCode())
        l(HALOREC.getDataDT())
    End Sub

    Shared Function personenKopieVon(quellPerson As Person) As Person
        Dim neup As New Person
        neup.Anrede = quellPerson.Anrede
        neup.Bemerkung = quellPerson.Bemerkung
        neup.Bezirk = quellPerson.Bezirk
        neup.Kassenkonto = quellPerson.Kassenkonto
        neup.Kontakt = quellPerson.Kontakt
        neup.Name = quellPerson.Name
        neup.Namenszusatz = quellPerson.Namenszusatz
        neup.Quelle = quellPerson.Quelle
        neup.Rolle = quellPerson.Rolle
        neup.Status = quellPerson.Status
        neup.Vorname = quellPerson.Vorname

        neup.Kontakt.Anschrift.Bemerkung = quellPerson.Kontakt.Anschrift.Bemerkung
        neup.Kontakt.Anschrift.Gemeindename = quellPerson.Kontakt.Anschrift.Gemeindename
        neup.Kontakt.Anschrift.Hausnr = quellPerson.Kontakt.Anschrift.Hausnr
        neup.Kontakt.Anschrift.PLZ = quellPerson.Kontakt.Anschrift.PLZ
        neup.Kontakt.Anschrift.Postfach = quellPerson.Kontakt.Anschrift.Postfach
        neup.Kontakt.Anschrift.PostfachPLZ = quellPerson.Kontakt.Anschrift.PostfachPLZ
        neup.Kontakt.Anschrift.Quelle = quellPerson.Kontakt.Anschrift.Quelle
        neup.Kontakt.Anschrift.Strasse = quellPerson.Kontakt.Anschrift.Strasse

        neup.Kontakt.Bankkonto.BLZ = quellPerson.Kontakt.Bankkonto.BLZ
        neup.Kontakt.Bankkonto.KontoNr = quellPerson.Kontakt.Bankkonto.KontoNr
        neup.Kontakt.Bankkonto.Name = quellPerson.Kontakt.Bankkonto.Name
        neup.Kontakt.Bankkonto.Titel = quellPerson.Kontakt.Bankkonto.Titel

        neup.Kontakt.BankkontoID = quellPerson.Kontakt.BankkontoID
        neup.Kontakt.Bemerkung = quellPerson.Kontakt.Bemerkung

        neup.Kontakt.elektr.Email = quellPerson.Kontakt.elektr.Email
        neup.Kontakt.elektr.Fax1 = quellPerson.Kontakt.elektr.Fax1
        neup.Kontakt.elektr.Fax2 = quellPerson.Kontakt.elektr.Fax2
        neup.Kontakt.elektr.Homepage = quellPerson.Kontakt.elektr.Homepage
        neup.Kontakt.elektr.MobilFon = quellPerson.Kontakt.elektr.MobilFon
        neup.Kontakt.elektr.Quelle = quellPerson.Kontakt.elektr.Quelle
        neup.Kontakt.elektr.Telefon1 = quellPerson.Kontakt.elektr.Telefon1
        neup.Kontakt.elektr.Telefon2 = quellPerson.Kontakt.elektr.Telefon2

        neup.Kontakt.GesellFunktion = quellPerson.Kontakt.GesellFunktion
        neup.Kontakt.Org.Name = quellPerson.Kontakt.Org.Name
        neup.Kontakt.Org.Eigentuemer = quellPerson.Kontakt.Org.Eigentuemer
        neup.Kontakt.Org.Bemerkung = quellPerson.Kontakt.Org.Bemerkung
        neup.Kontakt.Org.Typ1 = quellPerson.Kontakt.Org.Typ1
        neup.Kontakt.Org.Typ2 = quellPerson.Kontakt.Org.Typ2
        neup.Kontakt.Org.Zusatz = quellPerson.Kontakt.Org.Zusatz

        neup.Kontakt.KontaktID = quellPerson.Kontakt.KontaktID
        neup.Kontakt.OrgID = quellPerson.Kontakt.OrgID
        neup.Kontakt.Quelle = quellPerson.Kontakt.Quelle





        Return neup
    End Function

End Class
