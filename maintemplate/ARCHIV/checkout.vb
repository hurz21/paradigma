Public Class checkout
    Private Shared Function bildeFIold(ByVal aktdoku As Dokument, ByRef fiold As IO.FileInfo) As String
        Try
            fiold = New IO.FileInfo(aktdoku.FullnameImArchiv)
            Return String.Empty
        Catch ex As Exception
            Dim hinweis As String = "Fehler: Die Datei " & aktdoku.DateinameMitExtension & " konnte nicht gefunden werden. " &
                                    "1evtl. gibt es ein Problem mit dem Namen! Datei wird ignoriert."
            nachricht(hinweis & " // " & aktdoku.FullnameImArchiv)
            MsgBox(hinweis)
            Return "fehler"
        End Try
    End Function

    Private Shared Function ArchivDateinameGueltig(ByVal aktdoku As Dokument) As String
        If String.IsNullOrEmpty(aktdoku.FullnameImArchiv) Then
            MessageBox.Show("Der Archiv-Dateiname ist leer: " & aktdoku.FullnameImArchiv)
            nachricht("Fehler in checkout: " & "Der Archiv-Dateiname ist leer: " & aktdoku.FullnameImArchiv)
            Return "fehler"
        End If
        Return String.Empty
    End Function
    Public Shared Function checkout(ByVal aktdoku As Dokument, ByVal lokvorgangsid As Integer) As String 'myGlobalz.sitzung.VorgangsID
        nachricht("Archiv: checkout: ----------------------------- eingang")
        Dim nointeraction As Boolean = True
        aktdoku.makeFullname_ImArchiv(myglobalz.Arc.rootDir)
        If ArchivDateinameGueltig(aktdoku) = "fehler" Then Return "fehler"
        aktdoku.makeFullname_ImArchiv(myglobalz.Arc.rootDir)
        If aktdoku.FullnameCheckout.IsNothingOrEmpty Then
            Dim ausgabeverzeichnis As String = ""
            myGlobalz.sitzung.aktDokument.makeFullname_Checkout(lokvorgangsid, myGlobalz.Arc.lokalerCheckoutcache, 0, ausgabeverzeichnis)
        End If
        Dim fiold As IO.FileInfo
        If bildeFIold(aktdoku, fiold) = "fehler" Then Return "fehler"
        IO.Directory.CreateDirectory((myglobalz.Arc.lokalerCheckoutcache & "\" & lokvorgangsid).Replace("\\", "\"))
        Dim lResult As String = rauskopieren(aktdoku, nointeraction, fiold)
        If lResult.StartsWith("fehler") Then
            Return lResult
        Else
            Return aktdoku.FullnameCheckout
        End If
    End Function

    Private Shared Sub alteDateiLoeschen(ByVal finew As IO.FileInfo)
        Try
            finew.Delete()
        Catch ex As Exception
            nachricht("fehler beim löschen der alten datei: " & finew.FullName)
        End Try
    End Sub

    Private Shared Function rauskopieren(ByVal aktdoku As Dokument, ByVal nointeraction As Boolean, ByVal fiold As IO.FileInfo) As String
        Dim result As MessageBoxResult
        Dim finew As IO.FileInfo
        Try
            finew = New IO.FileInfo(aktdoku.FullnameCheckout)
            If Not fiold.Exists Then
                nachricht(String.Format("FEHLER Diese Quell-Datei fehlt im Archiv: {0}{1} !", vbCrLf, fiold.FullName))
                Return "fehlerquellefehlt " & fiold.FullName
            End If
            If Not finew.Exists Then
                alteDateiLoeschen(finew)
                fiold.CopyTo(aktdoku.FullnameCheckout, True)
                nachricht("Dokument wurde erfolgreich ausgecheckt")
                Return "ok"
            Else
                fiold.CopyTo(aktdoku.FullnameCheckout, True)
                nachricht("Dokument wurde erfolgreich ausgecheckt")
                Return "ok"
            End If

        Catch ex As Exception
            nachricht("fehler Dokument wurde NICHT erfolgreich ausgecheckt: " & fiold.FullName, ex)
            If nointeraction Then
                result = MessageBoxResult.Yes
            Else
                nachricht(String.Format("FEHLER Diese Datei existiert schon: {0}{1} Ersetzen ?",
                            myglobalz.sitzung.aktDokument.DateinameMitExtension, vbCrLf))
                result = MessageBox.Show(String.Format("Diese Datei existiert schon: {0}{1} Ersetzen ?",
                                 myglobalz.sitzung.aktDokument.DateinameMitExtension, vbCrLf),
                                 "Auschecken von Dokumenten",
                                 MessageBoxButton.YesNo,
                                 MessageBoxImage.Question,
                                 MessageBoxResult.OK)
            End If

            If result = MessageBoxResult.Yes Then
                If fiold.Exists Then
                    Try
                        fiold.CopyTo(aktdoku.FullnameCheckout, True)
                        nachricht("Dokument wurde erfolgreich ausgecheckt")
                        Return "ok"
                    Catch ex2 As Exception
                        If nointeraction Then
                            Return "fehler1 beim auscheken der datei: " & aktdoku.FullnameCheckout
                        Else
                            nachricht_und_Mbox(String.Format("Fehler beim überschreiben. Die Datei wird ggf. von einem anderen Programm benutzt!{0}{1}", vbCrLf, ex2))
                            Return "fehler2 beim auscheken der datei: " & aktdoku.FullnameCheckout
                        End If
                    End Try
                Else
                    nachricht("Datei existiert nicht im Archiv!")
                End If
            End If
        End Try
        Return "fehler beim auscheken der datei: " & aktdoku.FullnameCheckout
    End Function
End Class
