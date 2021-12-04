Module meinClipboard
    Function getContentFromZwischenablage() As Boolean
        Dim grabpicture As System.Drawing.Image
        Dim fi As IO.FileInfo
        Dim ausgabeVerzeichnis As String = ""
        Try
            myglobalz.sitzung.aktDokument.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
            myGlobalz.sitzung.aktDokument.DateinameMitExtension = "VonZwischenablage.png"

            myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktDokument.DocID, ausgabeVerzeichnis)
            fi = New IO.FileInfo(myglobalz.sitzung.aktDokument.FullnameCheckout)
            IO.Directory.CreateDirectory(fi.Directory.ToString)

            grabpicture = getPictureFromCB()
            If grabpicture IsNot Nothing Then
                alteDateiLoeschen()
                Dim erfolg As Boolean
                erfolg = BildTemporaerSpeichern(grabpicture, myglobalz.sitzung.aktDokument.FullnameCheckout)
                If erfolg Then
                    If Not myglobalz.sitzung.aktDokument.FullnameCheckout.IsNothingOrEmpty Then
                        If insarchivUebernehmen(myglobalz.sitzung.aktDokument.FullnameCheckout, "") Then
                            fi = Nothing
                            Return True
                        End If
                    End If
                End If
            Else
                Debug.Print("text")
                Dim text As String = getTextFromCB()
                If Not text.IsNothingOrEmpty Then
                    'datei erzeugen
                    myglobalz.sitzung.aktDokument.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
                    myGlobalz.sitzung.aktDokument.DateinameMitExtension = "AusZwischenablage.txt" ' LIBgemeinsames.clsString.kuerzeTextauf(text,50) & ".txt"

                    myGlobalz.sitzung.aktDokument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache, myGlobalz.sitzung.aktDokument.DocID, ausgabeVerzeichnis)
                    fi = New IO.FileInfo(myglobalz.sitzung.aktDokument.FullnameCheckout)
                    IO.Directory.CreateDirectory(fi.Directory.ToString)
                    Using raus As New IO.StreamWriter(myglobalz.sitzung.aktDokument.FullnameCheckout)
                        raus.WriteLine(text)
                    End Using
                    If meinClipboard.insarchivUebernehmen(myglobalz.sitzung.aktDokument.FullnameCheckout, "") Then
                        fi = Nothing
                        Return True
                    End If
                End If
            End If
            fi = Nothing
            Return False
        Catch ex As Exception
            nachricht("fehler in getContentFromZwischenablage: " ,ex)
            Return False
        End Try
    End Function

    Private Sub alteDateiLoeschen()
        Try
            Dim fi As New IO.FileInfo(myglobalz.sitzung.aktDokument.FullnameCheckout)
            If fi.Exists Then
                fi.Delete()
            End If
            fi = Nothing
        Catch ex As Exception
            nachricht("fehler in alteDateiLoeschen: " ,ex)
        End Try
    End Sub

    Public Function getPictureFromCB() As System.Drawing.Image
        Try
            If My.Computer.Clipboard.ContainsImage() Then
                Dim grabpicture As System.Drawing.Image
                grabpicture = My.Computer.Clipboard.GetImage()
                Return grabpicture
            End If
            Return Nothing
        Catch ex As Exception
            nachricht("fehler in getPictureFromCB: " & vbCrLf & vbCrLf ,ex)
            Return Nothing
        End Try
    End Function

    Public Function BildTemporaerSpeichern(grabpicture As System.Drawing.Image, dateiname As String) As Boolean
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
            nachricht("fehler in BildTemporaerSpeichern: " & vbCrLf & vbCrLf ,ex)
            Return False
        End Try
    End Function

    Public Function insarchivUebernehmen(tempFile As String, DateiMetaData As String, Optional eid As Integer = 0) As Boolean
        Try
            Dim filenames As String() = Nothing
            Dim initalDir As String = bestimmeInitialdir(tempFile)
            Dim dcc As New winDokumenteEinchecken(filenames, eid, initalDir, tempFile, DateiMetaData, False)
            dcc.ShowDialog()
            glob2.MeinGarbage()
            Dim result As Boolean = CBool(dcc.DialogResult)
            Return result
        Catch ex As Exception
            nachricht("fehler in insarchivUebernehmen: " ,ex)
            Return False
        End Try
    End Function

    Private Function bestimmeInitialdir(tempFile As String) As String
        Dim fi As New IO.FileInfo(tempFile)
        Dim drname As String = fi.DirectoryName
        fi = Nothing
        Return drname
    End Function

    Public Function getTextFromCB() As String
        Try
            If My.Computer.Clipboard.ContainsText() Then
                Return My.Computer.Clipboard.GetText
            End If
            Return Nothing
        Catch ex As Exception
            nachricht("fehler in getTextFromCB: " & vbCrLf & vbCrLf ,ex)
            Return Nothing
        End Try
    End Function



End Module
