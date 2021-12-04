Imports System.Data

Public Class clsADtools
    Public Shared Sub item2objAD(ByVal item As DataRowView, ByRef aktperson As Person)
        nachricht("  item2objAD:--------------------------------")
        Try
            aktperson.clear()
            '' '' % = CInt(item(0).ToString()) 
            '' ''aktperson.Namenszusatz = item("Titel").ToString()
            aktperson.Kassenkonto = item("physicalDeliveryOfficeName").ToString()       '
            aktperson.Name = item("sn").ToString()
            aktperson.Vorname = item("givenName").ToString()
            aktperson.Kontakt.elektr.Telefon1 = item("telephoneNumber").ToString()
            aktperson.Kontakt.elektr.Email = item("mail").ToString()  '
            aktperson.Kontakt.Bemerkung = item("department").ToString()    '
            aktperson.Kontakt.Org.Name = item("company").ToString() '
            aktperson.Kontakt.Anschrift.Strasse = item("streetAddress").ToString
            aktperson.Kontakt.Bemerkung = item("userPrincipalName").ToString() ' 
        Catch ex As System.Exception
            nachricht("Fehler in item2objAD:" ,ex)
        End Try
    End Sub

    Shared Sub renameADColumns(ByRef dt As DataTable)
        dt.Columns("givenName").ColumnName = "VORNAME"
        dt.Columns("sn").ColumnName = "NACHNAME"
        dt.Columns("mail").ColumnName = "FFEMAIL"
        dt.Columns("company").ColumnName = "ORGNAME"
        dt.Columns("telephoneNumber").ColumnName = "FFTELEFON1"
        dt.Columns("streetAddress").ColumnName = "STRASSE"
        dt.Columns("sAMAccountName").ColumnName = "userid"
        dt.Columns("department").ColumnName = "ROLLE"
    End Sub

    Shared Sub PersonAusFachdienstITDBUebernehmen(ByVal item As DataRowView)
        Try
            With myGlobalz.sitzung.aktPerson
                .clear()
                .Name = CStr(clsDBtools.fieldvalue(item("NACHNAME"))).ToString
                .Bezirk = " "
                .Vorname = CStr(clsDBtools.fieldvalue(item("VORNAME"))).ToString
                '  .Bemerkung = CStr(clsDBtools.fieldvalue(item("Kassenkonto"))).ToString
                .Namenszusatz = "" ' 
                .Anrede = "" 'CStr(clsDBtools.fieldvalue(item("Anrede"))).ToString()
                .Kontakt.clear()
                '.Kontakt.GesellFunktion = CStr(clsDBtools.fieldvalue(item("GesellFunktion"))).ToString()
                '.Kontakt.Bemerkung = "Quelle: VorgangsDB"
                '.Kontakt.Anschrift.Gemeindename = CStr(clsDBtools.fieldvalue(item("Gemeindename"))).ToString()
                .Kontakt.Anschrift.Strasse = CStr(clsDBtools.fieldvalue(item("STRASSE"))).ToString()
                '.Kontakt.Anschrift.Hausnr = CStr(clsDBtools.fieldvalue(item("Hausnr"))).ToString()
                '.Kontakt.Anschrift.PLZ = CInt(CStr(clsDBtools.fieldvalue(item("PLZ"))).ToString())
                .Kontakt.elektr.Telefon1 = (CStr(clsDBtools.fieldvalue(item("FFTELEFON1"))).ToString())
                '.Kontakt.elektr.Telefon2 = (CStr(clsDBtools.fieldvalue(item("fftelefon2"))).ToString())
                '.Kontakt.elektr.Fax1 = (CStr(clsDBtools.fieldvalue(item("fffax1"))).ToString())
                '.Kontakt.elektr.Fax2 = (CStr(clsDBtools.fieldvalue(item("fffax2"))).ToString())

                '.Kontakt.elektr.MobilFon = (CStr(clsDBtools.fieldvalue(item("FFMobilFon"))).ToString())
                '.Kontakt.elektr.Homepage = (CStr(clsDBtools.fieldvalue(item("FFHomepage"))).ToString())

                .Kontakt.elektr.Email = (CStr(clsDBtools.fieldvalue(item("FFEMAIL"))).ToString())
                .Kontakt.Org.Name = (CStr(clsDBtools.fieldvalue(item("ORGNAME"))).ToString())
                .Rolle = (CStr(clsDBtools.fieldvalue(item("Rolle"))).ToString())
                .Kontakt.Org.Zusatz = "" '(CStr(clsDBtools.fieldvalue(item("orgzusatz"))).ToString())

                .changed_Anschrift = True
            End With
        Catch ex As Exception
            MsgBox("Fehler bei der Übernahme von Daten aus der Vorgangsdatenbank! in PersonAusVorgangsDBUebernehmen" & ex.ToString)
        End Try
    End Sub
End Class
