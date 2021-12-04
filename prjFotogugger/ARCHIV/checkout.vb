﻿Public Class checkout
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

    Public Shared Function ArchivDateinameGueltig(ByVal aktdoku As Dokument) As String
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
        aktdoku.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
        If ArchivDateinameGueltig(aktdoku) = "fehler" Then Return "fehler"
        aktdoku.makeFullname_ImArchiv(myGlobalz.Arc.rootDir)
        If aktdoku.FullnameCheckout.IsNothingOrEmpty Then
            Dim ausgabeVerzeichnis As String = ""
            aktdoku.makeFullname_Checkout(lokvorgangsid, myGlobalz.Arc.lokalerCheckoutcache, aktdoku.DocID, ausgabeVerzeichnis)
        End If
        Dim fiold As IO.FileInfo = Nothing

        If bildeFIold(aktdoku, fiold) = "fehler" Then
            Return "fehler"
        End If
        'alt  IO.Directory.CreateDirectory((myGlobalz.Arc.lokalerCheckoutcache & "\" & lokvorgangsid).Replace("\\", "\"))
        Dim dir As String = aktdoku.FullnameCheckout.Replace(aktdoku.DateinameMitExtension, "")
        IO.Directory.CreateDirectory((dir.Replace("\\", "\")))
        Dim lResult As String = rauskopieren(aktdoku, nointeraction, fiold)
        fiold = Nothing
        glob2.MeinGarbage()

        If lResult.StartsWith("fehler") Then
            Return lResult
        Else
            Return aktdoku.FullnameCheckout
        End If
    End Function

    Private Shared Function alteDateiSichernAlsBackup(ByVal finew As IO.FileInfo) As Boolean
         Dim neuername As String
        Try
            nachricht("alteDateiSichernAlsBackup: " & finew.FullName)
            nachricht(finew.FullName)

            neuername = "Backup_" & LIBgemeinsames.clsString.date2string(Now, 1) & "_" & finew.Name
            neuername = finew.FullName.Replace(finew.Name, neuername)

            If finew.Exists Then
                finew.MoveTo(neuername)
                '  finew.Delete()
                l("alteDateiSichernAlsBackup gelöscht.")
            Else
                l("alteDateiSichernAlsBackup existiert nicht.")
            End If
            Return True
        Catch ex As Exception
            nachricht("awarnung alteDateiSichernAlsBackup beim alteDateiSichernAlsBackup der alten datei: " & finew.FullName & " neuer:  " & neuername ,ex)
            Return False
        End Try
    End Function

    Private Shared Function rauskopieren(ByVal aktdoku As Dokument, ByVal nointeraction As Boolean,
                                         ByVal fiold As IO.FileInfo) As String
        Dim result As MessageBoxResult
        Dim finew As IO.FileInfo
        Dim errorindic As Integer = 0
        l("in rauskopieren")
        Try
            FileArchivTools.inputFileReadonlyEntfernen(aktdoku.FullnameCheckout)
            finew = New IO.FileInfo(aktdoku.FullnameCheckout)
            If Not fiold.Exists Then
                errorindic = 1
                nachricht(String.Format("FEHLER Diese Quell-Datei fehlt im Archiv: {0}{1} !", vbCrLf, fiold.FullName))
                finew = Nothing
                Return "fehlerquellefehlt " & fiold.FullName
            End If
            If finew.Exists Then
                errorindic = 2
                'BackupAnlegen(finew, dok)
                If alteDateiSichernAlsBackup(finew) Then
                    errorindic = 3
                    DokArc.copyOrDekompresscopy(fiold, aktdoku.FullnameCheckout, aktdoku.kompressed)
                    nachricht("Dokument wurde erfolgreich ausgecheckt: " & aktdoku.FullnameCheckout)
                    finew = Nothing
                    Return "ok"
                Else
                    nachricht("Dokument wurde NICHT erfolgreich ausgecheckt: " & aktdoku.FullnameCheckout)
                    finew = Nothing
                    Return "fehler alteLokalDateiLiessichNichtLoeschen"
                End If

            Else
                errorindic = 4
                DokArc.copyOrDekompresscopy(fiold, aktdoku.FullnameCheckout, aktdoku.kompressed)
                'fiold.CopyTo(aktdoku.FullnameCheckout, True)
                nachricht("Dokument wurde erfolgreich ausgecheckt" & aktdoku.FullnameCheckout)
                finew = Nothing
                Return "ok"
            End If

        Catch ex As Exception
            nachricht("warnung Dokument wurde NICHT erfolgreich ausgecheckt: " & errorindic & " , " & fiold.FullName, ex)
            If nointeraction Then
                result = MessageBoxResult.Yes
            Else
                nachricht(String.Format("FEHLER Diese Datei existiert schon: {0}{1} Ersetzen ?",
                            myGlobalz.sitzung.aktDokument.DateinameMitExtension, vbCrLf))
                result = MessageBox.Show(String.Format("Diese Datei existiert schon: {0}{1} Ersetzen ?",
                                 myGlobalz.sitzung.aktDokument.DateinameMitExtension, vbCrLf),
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
                            nachricht(String.Format("Fehler beim überschreiben. Die Datei wird ggf. " &
                                                             "von einem anderen Programm benutzt!{0}{1}", vbCrLf, ex2))
                            Return "fehler2 beim auscheken der datei: CopyTo " & aktdoku.FullnameCheckout
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
