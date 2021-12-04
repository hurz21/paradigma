Module modNachricht
    Public Sub l(text As String)
        nachricht(text)
        If text.ToLower.Contains("starten des servers fehlgeschlagen") And
           text.ToLower.Contains("meinsendmail") Then
            MsgBox("Outlook ist nicht korrekt gestartet worden. Bitte starten Sie Outlook vollständig!")
        End If
    End Sub
    Public Sub l(ByVal text As String, ByVal ex As System.Exception)
        Dim anhang As String = ""
        text = text & ToLogString(ex, text)
        myGlobalz.sitzung.nachrichtenText = text
        My.Log.WriteEntry(text)
        mitFehlerMail(text, anhang)
    End Sub
    Friend Function getfilesAusDir(verz As String) As String()
        Dim filesAusDir As String()
        'Dim syn As String = verz
        Try
            filesAusDir = IO.Directory.GetFiles(verz)
            If filesAusDir Is Nothing Then
                Return Nothing
            End If
            Return filesAusDir
        Catch ex As Exception
            l("Fehler in getfilesAusDir " ,ex)
            Return Nothing
        End Try
    End Function

    Private Function genScreenshotAndSaveLocal() As String
        Dim anhang As String
        Dim screenshot As System.Drawing.Bitmap
        Dim graph As System.Drawing.Graphics
        Try
            anhang = IO.Path.Combine(System.IO.Path.GetTempPath, "dump.png")
            screenshot = New System.Drawing.Bitmap(My.Computer.Screen.WorkingArea.Width,
                                                   My.Computer.Screen.WorkingArea.Height,
                                                   System.Drawing.Imaging.PixelFormat.Format32bppPArgb)
            graph = System.Drawing.Graphics.FromImage(screenshot)
            '  graph.CopyFromScreen(bounds.X, bounds.Y, 0, 0, bounds.Size, CopyPixelOperation.SourceCopy)
            graph.CopyFromScreen(New System.Drawing.Point(0, 0), New System.Drawing.Point(0, 0), _
               New System.Drawing.Size(My.Computer.Screen.WorkingArea.Width, _
                My.Computer.Screen.WorkingArea.Height))
            graph.Save()
            screenshot.Save(anhang, System.Drawing.Imaging.ImageFormat.Png)
            Return anhang
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Private Sub mitFehlerMail(ByVal text As String, ByVal anhang As String)
        'If myGlobalz.zuhause Then Exit Sub
        If text.ToLower.StartsWith("fehler") Or text.ToLower.StartsWith("error") Or
            text.ToLower.StartsWith("warnung") Or text.ToLower.StartsWith("problem") Then
            My.Log.WriteEntry("!!!!  Ein Fehler/Warnung ist aufgetreten !!!! ----------------------------")
            If myGlobalz.zuhause Then Exit Sub
            CLstart.MeinLogging.LoggingEnde(logfile, myGlobalz.LOGFILEKOPIE)
            If initP.getValue("Haupt.miterroremail") = "nomail" Then
                Exit Sub
            End If
            Dim keyword As String
            If text.ToLower.StartsWith("warnung") Then
                keyword = "warnung"
                anhang = ""
            Else
                keyword = "Fehler"
                anhang = genScreenshotAndSaveLocal()
            End If
            anhang = anhang & "," & myGlobalz.LOGFILEKOPIE
            Dim test As Boolean = clsMailsenden.mailrausSMTP("dr.j.feinen@kreis-offenbach.de", "dr.j.feinen@kreis-offenbach.de",
                                                              keyword & " in Paradigma, Vorgang: " & myGlobalz.sitzung.aktVorgangsID &
                                                              ", Initial: " & myGlobalz.sitzung.aktBearbeiter.Initiale,
                                                              text.Replace(vbCrLf, "<br>"),
                                                              anhang, False, "", "", "")
            My.Log.WriteEntry("----------------------------")
        End If
    End Sub

    Public Sub nachricht(ByVal text As String)
        Dim anhang As String = ""
        If myGlobalz.zuhause Then Exit Sub
        Try
            If myGlobalz.minErrorMessages Then
                If Not (text.ToLower.StartsWith("fehler") Or text.ToLower.StartsWith("warnung")) Then
                    Exit Sub
                End If
            End If
            myGlobalz.sitzung.nachrichtenText = text
            My.Log.WriteEntry(text)
            mitFehlerMail(text, anhang)
        Catch ex As Exception

        End Try
    End Sub

    Public Sub nachricht(ByVal text As String, ByVal ex As System.Exception)
        Dim anhang As String = ""
        text = text.Replace("DefaultSource	Information	0	", "")
        text = text & ToLogString(ex, text)
        myGlobalz.sitzung.nachrichtenText = text
        My.Log.WriteEntry(text)
        mitFehlerMail(text, anhang)
    End Sub



    Public Sub nachricht_und_Mbox(ByVal text As String)
        Dim anhang As String = ""
        Try
            myGlobalz.sitzung.nachrichtenText = text
            My.Log.WriteEntry(text)
            MessageBox.Show(text)
            mitFehlerMail(text, anhang)
        Catch ex As Exception

        End Try
    End Sub

    Public Sub nachricht_und_Mbox(ByVal text As String, ByVal ex As System.Exception)
        Dim anhang As String = ""
        text = ToLogString(ex, text)
        myGlobalz.sitzung.nachrichtenText = text
        My.Log.WriteEntry(text)
        MessageBox.Show(text)
        mitFehlerMail(text, anhang)
    End Sub
End Module
