
Namespace nachZielKopieren
    Class DolumentImArchivKopieren
        Private Sub New()

        End Sub
        Private Shared Function GetBeschreibung(ByVal dokument As Dokument) As String
            Dim a As String = "Kopie von: " & dokument.DateinameMitExtension & " (" & dokument.Beschreibung & ")"
            If a.Length > 940 Then
                a = a.Substring(0, 940)
            End If
            Return a
        End Function
        Private Shared Sub setDokumentTypLength(ByRef dokument As Dokument)
            Try
                If dokument Is Nothing Then
                    nachricht("fehler in DolumentImArchivKopieren dokument is nothing: " & "/" & dokument.Typ)

                    Exit Sub
                End If
                If dokument.Typ.Length > 4 Then
                    dokument.Typ = dokument.Typ.Substring(0, 3)
                End If
            Catch ex As Exception
                nachricht("fehler in DolumentImArchivKopieren setDokumentTypLength: " & ex.ToString & "/" & dokument.Typ)
            End Try
        End Sub
        Shared Function exe(dokument As Dokument) As Boolean
            'kopie unter neunem namen im checkoutdir anlegen
            'kopie Einchecken
            'refreshen
            Dim neuername As String
            Dim erfolg As Boolean
            Try
                neuername = getNeuerName(dokument)
                If String.IsNullOrEmpty(neuername) Then
                    Return False
                End If
                myGlobalz.sitzung.aktDokument.newSaveMode = True
                erfolg = UmkopiereDokumentInCheckout(dokument, neuername)
                If erfolg Then
                    dokument.Beschreibung = GetBeschreibung(dokument)
                    setDokumentTypLength(dokument)

                    erfolg = myGlobalz.Arc.checkINDoku(neuername,
                                                       dokument.Beschreibung,
                                                       0,
                                                       False,
                                                       "",
                                                       myGlobalz.sitzung.defineArchivVorgangsDir(myGlobalz.sitzung.aktVorgangsID),
                                                       myGlobalz.sitzung.aktVorgangsID,
                                                       False,
                                                       Now,
                                                       myGlobalz.sitzung.aktDokument.DocID,
                                                       myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir,
                                                      myGlobalz.sitzung.aktDokument.newSaveMode, False,
                                     myGlobalz.sitzung.aktDokument.kompressed, myGlobalz.sitzung.aktBearbeiter.ID)
                    Return erfolg
                Else
                    Return False
                End If
            Catch ex As Exception
                nachricht("fehler in DolumentImArchivKopieren: ", ex)
                Return False
            End Try
        End Function

        Private Shared Function UmkopiereDokumentInCheckout(dokument As Dokument, ByRef neuername As String) As Boolean
            nachricht("UnbenenneDokumentInCheckout ---------------- ")
            nachricht("altername:" & dokument.DateinameMitExtension)
            nachricht("neuername:" & neuername)
            Try
                Dim fi As New IO.FileInfo(dokument.FullnameCheckout)
                neuername = IO.Path.Combine(fi.DirectoryName, neuername)
                nachricht("neuername:" & neuername)
                fi.CopyTo(neuername, True)
                nachricht("UnbenenneDokumentInCheckout: ok")
                fi = Nothing
                Return True
            Catch ex As Exception
                nachricht("fehler in UnbenenneDokumentInCheckout: ", ex)
                Return False
            End Try
        End Function

        Private Shared Function getNeuerName(ByVal dokument As Dokument) As String
            Dim a As String = Microsoft.VisualBasic.Interaction.InputBox(glob2.getMsgboxText("neuenDateinamenAngeben", New List(Of String)(New String() {})),
                                                      "Neuen Dateinamen definieren",
                                                      "KOPIE_" & dokument.DateinameMitExtension)
            Try
                If String.IsNullOrEmpty(a) Then
                    MsgBox("Keine Eingabe, Abbruch", MsgBoxStyle.Critical, "Eingabe ist leer")
                    Return ""
                End If
                If LIBgemeinsames.clsString.enthaeltUnerlaubteZeichen(a) Then
                    MsgBox("Der Dateiname enthält unerlaubte Zeichen (z.B. < > ?  : | \ / *) !!! (Abbruch)", MsgBoxStyle.Critical, "Unerlaubte Zeichen")
                    Return ""
                End If
                If a.ToLower.Trim = dokument.DateinameMitExtension.ToLower.Trim Then
                    MsgBox("Sie müssen einen neuen Namen vergeben !!! (Abbruch)", MsgBoxStyle.Critical, "Alter Name = Neuer Name")
                    Return ""
                End If
                If Not a.ToLower.Trim.Contains(".") Then
                    MsgBox("Sie müssen einen Namen mit gültiger Dokumenttyperweiterung vergeben !!! (Abbruch)", MsgBoxStyle.Critical, "Dokumenttyperweiterung fehlt")
                    Return ""
                End If
                Return a
            Catch ex As Exception
                nachricht("fehler in getNeuerName: ", ex)
                Return ""
            End Try
        End Function
    End Class

    Class AlsPDFAkopieren
        Shared Function exe(dokument As Dokument, isConject As Boolean, conjectname As String) As Boolean
            Dim zielPDFdatei As String
            Dim erfolg As Boolean
            l("AlsPDFAkopieren ")
            Try
                dokument.getDokTyp()
                If dokument.DokTyp = DokumentenTyp.MSG Or dokument.DokTyp = DokumentenTyp.EML Then

                    MsgBox("Keine Emails bitte!")
                    Return False
                End If
                If Not istWordDatei(dokument) Then Return False
                If isConject Then
                    Dim ausgabeVerzeichnis As String = ""
                    dokument.makeFullname_Checkout(dokument.VorgangsID, myGlobalz.Arc.lokalerCheckoutcache, dokument.DocID, ausgabeVerzeichnis)
                    'neuername = getNeuerPDFNameConject(dokument, conjectname)
                    zielPDFdatei = IO.Path.Combine(ausgabeVerzeichnis, conjectname)

                Else
                    Dim ausgabeVerzeichnis As String = ""
                    dokument.makeFullname_Checkout(dokument.VorgangsID, myGlobalz.Arc.lokalerCheckoutcache, dokument.DocID, ausgabeVerzeichnis)
                    zielPDFdatei = getNeuerPDFName(dokument)
                End If
                If String.IsNullOrEmpty(zielPDFdatei) Then
                    Return False
                End If
                myGlobalz.sitzung.aktDokument.newSaveMode = True
                l("AlsPDFAkopieren dateiloeschen " & zielPDFdatei)
                DokArc.dateiloeschen(zielPDFdatei)
                'If dokument.revisionssicher then
                '    nachricht("fehler: AlsPDFAkopieren: Dokument ist revisionssicher und kann daher nicht geändert werden!")
                '    Return false
                'End If
                erfolg = erzeugePDFA(dokument, zielPDFdatei)
                l("AlsPDFAkopieren erzeugePDFA von " & dokument.FullnameCheckout)
                l("AlsPDFAkopieren erzeugePDFA nach " & zielPDFdatei)

                If erfolg Then
                    l("AlsPDFAkopieren erzeugePDFA erfolg ")

                    dokument.Beschreibung = GetBeschreibungPDFA(dokument)
                    setDokumentTypLengthPDFA(dokument)
                    erfolg = myGlobalz.Arc.checkINDoku(zielPDFdatei,
                                                       dokument.Beschreibung,
                                                       0,
                                                       False,
                                                       "",
                                                        myGlobalz.sitzung.defineArchivVorgangsDir(myGlobalz.sitzung.aktVorgangsID),
                                                       myGlobalz.sitzung.aktVorgangsID,
                                                       False,
                                                       Now,
                                                       myGlobalz.sitzung.aktDokument.DocID,
                                                       myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir,
                                                      myGlobalz.sitzung.aktDokument.newSaveMode, False,
                                     myGlobalz.sitzung.aktDokument.kompressed, myGlobalz.sitzung.aktBearbeiter.ID)

                    Return erfolg
                Else
                    l("AlsPDFAkopieren erzeugePDFA kein erfolg ")
                    Return False
                End If
            Catch ex As Exception
                nachricht("fehler in DolumentImArchivKopieren: ", ex)
                Return False
            End Try
        End Function

        'Private Shared Function getNeuerPDFNameConject(dokument As Dokument, conjectname As String) As String
        '    Dim fullPathName As String


        '    Dim neuerConjectName As String

        '    'neuerConjectName = makeNewConjectFileName(dokument, conjectname As String) & ".pdf"
        '    neuerConjectName = conjectname & ".pdf"
        '    fullPathName = IO.Path.Combine(ausgabeVerzeichnis, neuerConjectName)
        '    'fi = Nothing
        '    'If nn.ToLower.EndsWith(".doc") Then
        '    '    Return nn.ToLower.Replace(".doc", ".pdf")
        '    'End If
        '    'If nn.ToLower.EndsWith(".docx") Then
        '    '    Return nn.ToLower.Replace(".docx", ".pdf")
        '    'End If
        '    'If nn.ToLower.EndsWith(".rtf") Then
        '    '    Return nn.ToLower.Replace(".rtf", ".pdf")
        '    'End If
        '    'If nn.ToLower.EndsWith(".pdf") Then
        '    '    Return nn
        '    'End If
        '    Return fullPathName
        'End Function

        Private Shared Function makeNewConjectFileName(dokument As Dokument, topTitel As String, abteilung As String) As String
            Dim temp As String = ""
            Try
                l(" MOD makeNewConjectFileName anfang")
                temp = topTitel '"Stellungnahme_FD_Umwelt_"
                temp = temp & abteilung & "_vom_"
                'Select Case myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl
                '    Case "3311"
                '        temp = temp & "UNB_vom_"
                '    Case "5332"
                '        temp = temp & "Immissionsschutz_vom_"
                '    Case "4150"
                '        temp = temp & "UWBB_vom_"
                '    Case Else
                '        temp = temp & "UWBB_vom_"
                'End Select
                temp = temp & dokument.getTimestamp()

                l(" MOD makeNewConjectFileName ende")
                Return temp
            Catch ex As Exception
                l("Fehler in makeNewConjectFileName: ", ex)
                Return temp
            End Try
        End Function

        Private Shared Function getNeuerPDFName(dokument As Dokument) As String
            Dim nn As String

            Dim fi As New IO.FileInfo(dokument.FullnameCheckout)
            nn = fi.FullName
            fi = Nothing
            If nn.ToLower.EndsWith(".doc") Then
                Return nn.ToLower.Replace(".doc", ".pdf")
            End If
            If nn.ToLower.EndsWith(".docx") Then
                Return nn.ToLower.Replace(".docx", ".pdf")
            End If
            If nn.ToLower.EndsWith(".rtf") Then
                Return nn.ToLower.Replace(".rtf", ".pdf")
            End If
            Return ""
        End Function

        Private Shared Function erzeugePDFA(dokument As Dokument, zielDatei As String) As Boolean
            'Dim lw = New WordReplaceTextmarken()
            FileArchivTools.inputFileReadonlyEntfernen(dokument.FullnameCheckout)
            If wordInterop.dok2pdfA(dokument.FullnameCheckout, zielDatei) Then
                'lw = Nothing
                Return True
            End If
            'lw = Nothing
            Return False
        End Function

        Private Shared Function GetBeschreibungPDFA(dokument As Dokument) As String
            Return "PDF/A-Kopie"
        End Function

        Private Shared Sub setDokumentTypLengthPDFA(dokument As Dokument)
            '   Throw New NotImplementedException
        End Sub

    End Class
End Namespace
