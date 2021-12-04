Public Class userEditHelper
    Friend Shared Function save(bea As clsBearbeiter) As Integer
        Dim querie As String
        Dim result As Integer
        clsSqlparam.paramListe.Clear()

        querie = "update " & CLstart.myViewsNTabs.tabBearbeiter & " set " &
                " telefon=@telefon" &
                ",fax=@fax " &
                ",kuerzel1=@kuerzel1 " &
                ",email=@email " &
                ",rites=@rites " &
                ",anrede=@anrede " &
                " where BEARBEITERID=@BEARBEITERID"
        clsSqlparam.paramListe.Add(New clsSqlparam("telefon", bea.Kontakt.elektr.Telefon1))
        clsSqlparam.paramListe.Add(New clsSqlparam("fax", bea.Kontakt.elektr.Fax1))
        clsSqlparam.paramListe.Add(New clsSqlparam("email", bea.Kontakt.elektr.Email))
        clsSqlparam.paramListe.Add(New clsSqlparam("kuerzel1", bea.Kuerzel2Stellig))
        clsSqlparam.paramListe.Add(New clsSqlparam("rites", bea.Raum))
        clsSqlparam.paramListe.Add(New clsSqlparam("BEARBEITERID", bea.ID))
        clsSqlparam.paramListe.Add(New clsSqlparam("ANREDE", bea.Anrede))

        result = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, False, "ID")
        Return result
    End Function
End Class
