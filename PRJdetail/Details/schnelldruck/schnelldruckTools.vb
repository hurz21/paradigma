Public Class schnelldruckTools
    Public Shared Property Result As String = ""
    Shared Function hatMSGFiles(dlist As List(Of clsPresDokumente)) As Boolean
        Try
            l(" MOD ---------------------- anfang")
            For Each dockument As clsPresDokumente In dlist
                If dockument.ausgewaehlt Then
                    If dockument.DokTyp = DokumentenTyp.MSG Or dockument.DokTyp = DokumentenTyp.EML Then
                        Return True
                    End If
                End If
            Next
            l(" MOD ---------------------- ende")
            Return False
        Catch ex As Exception
            l("Fehler in MOD: ", ex)
            Return False
        End Try
    End Function

    Friend Shared Function reduziereAufAusgwaehlte(dlist As List(Of clsPresDokumente)) As List(Of clsPresDokumente)
        Dim dd As New List(Of clsPresDokumente)
        Try
            l(" MOD reduziereAufAusgwaehlte anfang")
            For Each dockument As clsPresDokumente In dlist
                If dockument.ausgewaehlt Then
                    dd.Add(dockument)
                End If
            Next
            l(" MOD reduziereAufAusgwaehlte ende")
            Return dd
        Catch ex As Exception
            l("Fehler in reduziereAufAusgwaehlte: " & ex.ToString())
            Return dd
        End Try

    End Function
    Friend Shared Function getInfoDokues(druckdokumente As List(Of clsPresDokumente)) As String
        Dim dd As String = ""
        Try
            l(" MOD getInfoDokues anfang")
            For Each dockument As clsPresDokumente In druckdokumente

                dd = dd & " " & dockument.DateinameMitExtension & Environment.NewLine
            Next
            l(" MOD getInfoDokues ende")
            Return dd
        Catch ex As Exception
            l("Fehler in getInfoDokues: " & ex.ToString())
            Return dd
        End Try
    End Function

    Shared Function sollMSGGedrucktWerden(msgFilesDrucken As Boolean) As Boolean
        Dim mres As MessageBoxResult = MessageBox.Show("Bei MSG-Dateien (=> Outlook - Emails) werden der Anhang oder eingefügte Bilder NICHT mitgedruckt." & Environment.NewLine &
                        "" & Environment.NewLine &
                        "Möchten sie die MSG trotzdem mitdrucken ?" & Environment.NewLine &
                        " " & Environment.NewLine &
                        " Ja / Nein" & Environment.NewLine &
                        " " & Environment.NewLine,
                        "Wichtiger Hinweis",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Exclamation)
        If mres = MessageBoxResult.Yes Then
            msgFilesDrucken = True
        End If

        Return msgFilesDrucken
    End Function


End Class
