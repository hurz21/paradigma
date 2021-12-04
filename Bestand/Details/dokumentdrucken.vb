Imports System.Drawing.Printing

Namespace nachZielKopieren
    Class dokumentdrucken
        private Sub new

        End Sub
        Private Shared Sub exeExtracted(ByVal dokument As Dokument, ByVal info As ProcessStartInfo)
            Try
                Process.Start(info)
            Catch ex As Exception
                MsgBox("Für diesen Dateityp ist keine Standardanwendung festgelegt. " &
                       "Zum Drucken legen Sie eine Standardanwendung fest!" & Environment.NewLine &
                       Environment.NewLine & "(" &
                       ex.ToString & ")")
                ' Debug.Print(ex.ToString)
                nachricht("fehler 2ind dokumentdrucken--" & dokument.FullnameCheckout)
            End Try
        End Sub
        Shared Sub exe(dokument As Dokument)
            nachricht("dokumentdrucken--")
            nachricht("datei : " & dokument.FullnameCheckout)
            Dim printDoc As String
            Dim info As ProcessStartInfo
            Try

                printDoc = dokument.FullnameCheckout '"L:\websys\mapfiles\cache\kuhn_p.map"
                info = New ProcessStartInfo(printDoc)
                info.Verb = "PrintTo"
                info.CreateNoWindow = True
                info.WindowStyle = ProcessWindowStyle.Hidden
                nachricht("vor try : ")
            Catch
                nachricht("fehler 1ind dokumentdrucken--" & dokument.FullnameCheckout)
            End Try

            exeExtracted(dokument, info)
        End Sub

    End Class
End Namespace
