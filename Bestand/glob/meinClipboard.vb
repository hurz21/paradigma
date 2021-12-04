Module meinClipboard
    Function getContentFromZwischenablage() As Boolean
        Dim grabpicture As System.Drawing.Image
        Dim fi As  IO.FileInfo
        Try
            myGlobalz.sitzung.aktDokument.clear(CLstart.mycsimple.MeinNULLDatumAlsDate)
            myGlobalz.sitzung.aktDokument.DateinameMitExtension = "VonZwischenablage.png"
            myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache)
                 fi= New IO.FileInfo(myGlobalz.sitzung.aktDokument.FullnameCheckout)
                    IO.Directory.CreateDirectory(fi.Directory.ToString)

            grabpicture = getPictureFromCB()
            If grabpicture IsNot Nothing Then
                alteDateiLoeschen()
                Dim erfolg As Boolean
                erfolg = BildTemporaerSpeichern(grabpicture, myGlobalz.sitzung.aktDokument.FullnameCheckout)
                If erfolg Then
                    If Not myGlobalz.sitzung.aktDokument.FullnameCheckout.IsNothingOrEmpty Then
                        If insarchivUebernehmen(myGlobalz.sitzung.aktDokument.FullnameCheckout) Then
                            Return True
                        End If
                    End If
                End If
            Else
                Debug.Print("text")
                Dim text As String = getTextFromCB()
                If Not text.IsNothingOrEmpty Then
                    'datei erzeugen
                    myGlobalz.sitzung.aktDokument.clear(CLstart.mycsimple.MeinNULLDatumAlsDate)
                    myGlobalz.sitzung.aktDokument.DateinameMitExtension = "AusZwischenablage.txt" ' clsString.kuerzeTextauf(text,50) & ".txt"
                    myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache)
                     fi=New IO.FileInfo(myGlobalz.sitzung.aktDokument.FullnameCheckout)
                    IO.Directory.CreateDirectory(fi.Directory.ToString)
                    Using raus As New IO.StreamWriter(myGlobalz.sitzung.aktDokument.FullnameCheckout)
                        raus.WriteLine(text)
                    End Using
                    If meinClipboard.insarchivUebernehmen(myGlobalz.sitzung.aktDokument.FullnameCheckout) Then
                        Return True
                    End If
                End If
            End If
            Return False
        Catch ex As Exception
            nachricht("fehler in getContentFromZwischenablage: " & ex.ToString)
            Return False
        End Try
    End Function

    Private Sub alteDateiLoeschen()
        Try
            Dim fi As New IO.FileInfo(myGlobalz.sitzung.aktDokument.FullnameCheckout)
            If fi.Exists Then
                fi.Delete()
            End If
            fi = Nothing
        Catch ex As Exception
            nachricht("fehler in alteDateiLoeschen: " & ex.ToString)
        End Try
    End Sub

    Private Function getPictureFromCB() As System.Drawing.Image
        Try
            If My.Computer.Clipboard.ContainsImage() Then
                Dim grabpicture As System.Drawing.Image
                grabpicture = My.Computer.Clipboard.GetImage()
                Return grabpicture
            End If
            Return Nothing
        Catch ex As Exception
            nachricht("fehler in getPictureFromCB: " & vbCrLf & vbCrLf & ex.ToString)
            Return Nothing
        End Try
    End Function

    Private Function BildTemporaerSpeichern(grabpicture As System.Drawing.Image, dateiname As String) As Boolean
        Try
            Dim NewBitmap As New System.Drawing.Bitmap(grabpicture)
            'Create a new Graphic Object
            Dim Graphic As System.Drawing.Graphics = System.Drawing.Graphics.FromImage(NewBitmap)
            'Save new Image
            NewBitmap.Save(dateiname, System.Drawing.Imaging.ImageFormat.Png)
            Graphic.Dispose()
            NewBitmap.Dispose()
            Return True
        Catch ex As Exception
            nachricht("fehler in BildTemporaerSpeichern: " & vbCrLf & vbCrLf & ex.ToString)
            Return False
        End Try
    End Function

    Public Function insarchivUebernehmen(tempFile As String) As Boolean
        Try
            Dim filenames As String() = Nothing
            Dim initalDir As String = bestimmeInitialdir(tempFile)
            Dim dcc As New winDokumenteEinchecken(filenames, 0, initalDir, tempFile)
            dcc.ShowDialog()
            Dim result As Boolean = CBool(dcc.DialogResult)
            Return result
        Catch ex As Exception
            nachricht("fehler in insarchivUebernehmen: " & ex.ToString)
            Return False
        End Try
    End Function

    Private Function bestimmeInitialdir(tempFile As String) As String
        Dim fi As New IO.FileInfo(tempFile)
        Return fi.DirectoryName
    End Function

    Private Function getTextFromCB() As String
        Try
            If My.Computer.Clipboard.ContainsText() Then
                Return My.Computer.Clipboard.GetText
            End If
            Return Nothing
        Catch ex As Exception
            nachricht("fehler in getTextFromCB: " & vbCrLf & vbCrLf & ex.ToString)
            Return Nothing
        End Try
    End Function



End Module
