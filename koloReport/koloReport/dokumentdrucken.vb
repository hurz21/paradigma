Imports System.Drawing.Printing

Namespace nachZielKopieren
    Class dokumentdrucken
        Private Sub New()

        End Sub
        Private Shared Sub exeExtracted(ByVal dokument As String, ByVal info As ProcessStartInfo)
            Try
                l("dokumentdruckenexeExtracted---------------------")
                Process.Start(info)
            Catch ex As Exception
                MsgBox("Für diesen Dateityp ist keine Standardanwendung festgelegt. " &
                       "Zum Drucken legen Sie eine Standardanwendung fest!" & Environment.NewLine &
                       Environment.NewLine & "(" &
                       ex.ToString & ")")
                ' Debug.Print(ex.ToString)
                l("fehler 2ind dokumentdrucken--" & dokument)
            End Try
        End Sub
        Shared Sub exe(dokument As String)
            l("dokumentdrucken--")
            l("datei : " & dokument)
            Dim printDoc As String
            Dim info As ProcessStartInfo = Nothing
            Try

                printDoc = dokument '"L:\websys\mapfiles\cache\kuhn_p.map"
                info = New ProcessStartInfo(printDoc)
                info.Verb = "PrintTo"
                info.CreateNoWindow = True
                info.WindowStyle = ProcessWindowStyle.Hidden
                l("vor try : ")
            Catch
                l("fehler 1ind dokumentdrucken--" & dokument)
            End Try

            exeExtracted(dokument, info)
        End Sub

    End Class
End Namespace
