Module txtTools
    Function checkinausfuehren(txtdok As Dokument) As Boolean

        'ins archiv einchecken mit ereignisID

        'If dateityp = ".doc" Or dateityp = ".docx" Then
        '    neuu._VorlageDateiImArchiv.CopyTo(myglobalz.sitzung.aktDokument.FullnameCheckout)
        'Else
        '    neuu.vorlagendateiInsLokCheckoutDirKopieren(vdatei, myglobalz.sitzung.aktDokument.FullnameCheckout)
        'End If

        Dim NumDir As String = ""
        'Dim NumDir As String = myGlobalz.Arc.getFreshNumDir(myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir)

        If myGlobalz.Arc.checkINDoku(txtdok.FullnameCheckout,
                                     txtdok.Beschreibung,
                                     0,
                                     False,
                                     myGlobalz.sitzung.defineArchivVorgangsDir(myGlobalz.sitzung.aktVorgangsID),
                                     NumDir,
                                     myGlobalz.sitzung.aktVorgangsID,
                                     False, Now, txtdok.DocID,
                                     myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir,
                                     txtdok.newSaveMode, False,
                                     myGlobalz.sitzung.aktDokument.kompressed, myGlobalz.sitzung.aktBearbeiter.ID) Then
            'datei im Archiv öffnen
            Return True
        Else
            Return False
        End If
    End Function

    Friend Function leeresTextFileErzeugen(datei As String) As Boolean
        Dim a As String = "_"
        Try
            My.Computer.FileSystem.WriteAllText(datei, a, False, System.Text.Encoding.UTF8)
            Return True
        Catch ex As Exception
            nachricht("fehler in leeresTextFileErzeugen ", ex)
            Return False
        End Try

    End Function
End Module
