Public Class clsStammTools
    Sub New()

    End Sub
    Function speichernAllgemein(DialogResult As Boolean, lokmodus As String) As Boolean

        If Not persistiereVorgangStammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten, lokmodus) Then
            MsgBox("Problem beim Abspeichern der Stammdaten")
            DialogResult = False
            Return False
        End If
        If lokmodus = "neu" Then
            DialogResult = True 'regelt den direktaufruf des 
            CLstart.HistoryKookie.schreibeVerlaufsCookie.exe(myGlobalz.sitzung.aktVorgangsID.ToString,
                                                   myGlobalz.sitzung.aktVorgang.Stammdaten.Beschreibung,
                                                   myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt,
                                                   myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz,
                                                   myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ)
            '     HistoryKookie.schreibeVerlaufsCookie.exe(auswahlid, beschreibung, az2,myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz, myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ)

        Else
            DialogResult = False
        End If
        Return True
    End Function
    Private Function persistiereVorgangStammdaten(ByVal _meinstamm As Stamm, lokmodus As String) As Boolean 'tbStammbeschreibung.Text, tbBeschreibung_nummer.Text

        nachricht("Eingabe ist ok")
        If lokmodus = "neu" Then
            glob2.NEU_VorgangStamm_2DBOk()  'abspeichern in der db
            myGlobalz.sitzung.modus = "edit"
            Return True
        Else
            glob2.EDIT_VorgangStamm_2DBOk()  'abspeichern in der db
            myGlobalz.sitzung.modus = "edit"

            Return True
        End If
        Return False
    End Function
End Class
