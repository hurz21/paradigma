Module drucktools
    Function mehlen(ByVal nachrichttext As String) As String
        myglobalz.sitzung.SendMail.An = "m.kroemmelbein@kreis-offenbach.de;m.thieme@kreis-offenbach.de;dr.j.feinen@kreis-offenbach.de"
        myglobalz.sitzung.SendMail.Message = nachrichttext
        myglobalz.sitzung.SendMail.Betreff = "Ausdruck des Paradigma-Vorgangs " & myglobalz.sitzung.aktVorgangsID
        myglobalz.sitzung.SendMail.Message = myglobalz.sitzung.SendMail.Message.Replace(vbCrLf, "<br>")
        myglobalz.sitzung.SendMail.Anhang = CLstart.myc.kartengen.gifKartenDateiFullName
        nachricht("vor dem ersten mailraus")
        Dim outlookAnzeigen As Boolean = False
        Dim erfolg As String = clsMailsenden.mailrausOutlook(myglobalz.sitzung.SendMail.An, myglobalz.sitzung.SendMail.Betreff, myglobalz.sitzung.SendMail.Message,
                                                             myglobalz.sitzung.SendMail.Anhang, myglobalz.sitzung.SendMail.CC, myglobalz.anhangtrenner, outlookAnzeigen)
        Return erfolg
    End Function

    Function istBilddateiSchonda(ByVal pdateiname1 As String) As Boolean
        Dim fil As New IO.FileInfo(pdateiname1)
        Dim fila As Boolean = fil.Exists
        fil = Nothing
        Return fila
    End Function

End Module
