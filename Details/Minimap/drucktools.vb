Module drucktools
    Function mehlen(ByVal nachrichttext As String) As String
        myGlobalz.sitzung.SendMail.An = "m.kroemmelbein@kreis-offenbach.de;m.thieme@kreis-offenbach.de;dr.j.feinen@kreis-offenbach.de"
        myGlobalz.sitzung.SendMail.Message = nachrichttext
        myGlobalz.sitzung.SendMail.Betreff = "Ausdruck des Paradigma-Vorgangs " & myGlobalz.sitzung.aktVorgangsID
        myGlobalz.sitzung.SendMail.Message = myGlobalz.sitzung.SendMail.Message.Replace(vbCrLf, "<br>")
        myGlobalz.sitzung.SendMail.Anhang = clstart.myc.kartengen.gifKartenDateiFullName
     nachricht("vor dem ersten mailraus")
        Dim outlookAnzeigen As Boolean = False
        Dim erfolg As String = clsMailsenden.mailrausOutlook(myGlobalz.sitzung.SendMail.An, myGlobalz.sitzung.SendMail.Betreff, myGlobalz.sitzung.SendMail.Message,
                                                             myGlobalz.sitzung.SendMail.Anhang, myGlobalz.sitzung.SendMail.CC, myGlobalz.anhangtrenner, outlookAnzeigen)
        Return erfolg
    End Function

    Function istBilddateiSchonda(ByVal pdateiname1 As String) As Boolean
        Dim fil As New IO.FileInfo(pdateiname1)
        Return fil.Exists
    End Function

End Module
