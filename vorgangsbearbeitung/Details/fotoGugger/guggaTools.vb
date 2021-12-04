'Imports System.ComponentModel
'Imports System.IO
'Imports System.Data

'Module guggaTools
'Property maxWidth As Integer = 128
'Property maxHeight As Integer = 90
'Public Sub kopiereDokMetadaten(ByVal zieldok As Dokument, ByVal quelldok As Dokument)
'    With quelldok
'        zieldok.Filedatum = .Filedatum
'        zieldok.DateinameMitExtension = .DateinameMitExtension
'        zieldok.Beschreibung = .Beschreibung
'        zieldok.ExifDatum = .ExifDatum
'        zieldok.Checkindatum = .Checkindatum
'        zieldok.EXIFhersteller = .EXIFhersteller
'        zieldok.Handlenr = .Handlenr
'        zieldok.EXIFlat = .EXIFlat
'        zieldok.EXIFdir = .EXIFdir
'        zieldok.EXIFlon = .EXIFlon
'        zieldok.DocID = .DocID
'    End With
'End Sub

'Public Sub initHandleCursor(ByVal collFotos As List(Of clsFotoDokument), aktfoto As Dokument)
'    Dim cnt As Integer = 0
'    For Each adok As Dokument In collFotos
'        If adok.DateinameMitExtension = aktfoto.DateinameMitExtension Then
'            Debug.Print("")
'        End If
'        adok.Handlenr = cnt
'        cnt += 1
'    Next
'End Sub

'Public Sub initSetEtikett(ByVal collFotos As List(Of clsFotoDokument))
'    For Each adok As clsFotoDokument In collFotos
'        adok.Etikett = adok.Beschreibung & Environment.NewLine &
'            adok.ExifDatum & Environment.NewLine &
'            adok.DateinameMitExtension
'    Next
'End Sub



'Private Function GetCheckoutSubdir() As String
'    Dim subdir As String
'    subdir = clstart.myc.collFotos.Item(0).FullnameCheckout
'    Dim testfi As New IO.FileInfo(clstart.myc.collFotos.Item(0).FullnameCheckout)
'    subdir = testfi.DirectoryName & "\thumbs\"
'    Return subdir
'End Function

'Public Sub thumbnails4Liste(ByVal generate As Boolean, thumbNailWidth As Integer, thumbNailHeight As Integer)
'    Dim subdir As String = GetCheckoutSubdir()
'    Dim ThumbNailFile As String
'    For Each dok As clsFotoDokument In clstart.myc.collFotos
'        Dim source As New FileInfo(dok.FullnameImArchiv)
'        ThumbNailFile = subdir & source.Name
'        Dim ThumbNailFileTest As New FileInfo(ThumbNailFile)
'        If Not ThumbNailFileTest.Exists Then
'            If generate Then
'                IO.Directory.CreateDirectory(subdir)
'                createMiniThumb(dok.FullnameImArchiv, ThumbNailFile, thumbNailWidth, thumbNailHeight)
'            End If
'        End If
'        dok.thumbfullname = ThumbNailFile
'    Next
'End Sub
''' <summary>
''' Erstellt aus einem Bild ein Mini Abbild (Thumbnail)
''' </summary>
''' <param name="strOriginalImage"></param>
''' Hier wird er Speicherort und der Name des Originalbildes angegeben
''' "C:\Temp\original.png"
''' 
''' <param name="strnewThumbnailImage"></param>
''' Hier bitte den Speicherort, den Namen und die Erweiterung des neuen
''' thumbs angeben ("C:\Temp\mini.bmp")
''' 
''' 
''' <returns></returns>
''' <remarks></remarks>
''' 
'Function createMiniThumb(ByVal strOriginalImage As String, ByVal strnewThumbnailImage As String, thumbNailWidth As Integer, thumbNailHeight As Integer) As Boolean
'    Try
'        Dim wSize, hSize As Integer
'        Dim objImage As System.Drawing.Image
'        Dim objFileStream As New IO.FileStream(strOriginalImage, IO.FileMode.Open, FileAccess.Read)
'        Dim fakeCall As System.Drawing.Image.GetThumbnailImageAbort
'        'Original einlesen
'        objImage = System.Drawing.Image.FromStream(objFileStream)
'        fakeCall = New System.Drawing.Image.GetThumbnailImageAbort(AddressOf FakePreviewCallback)
'        calcThumbsSize(objImage.Width, objImage.Height, thumbNailWidth, thumbNailHeight, wSize, hSize)
'        'Groesse festlegen
'        objImage = objImage.GetThumbnailImage(wSize, hSize, fakeCall, IntPtr.Zero)
'        'Speichern
'        objImage.Save(strnewThumbnailImage, System.Drawing.Imaging.ImageFormat.Jpeg)
'        objFileStream.Close()
'        'Alles wieder freigeben
'        objFileStream = Nothing
'        objImage = Nothing
'        fakeCall = Nothing
'        GC.Collect()
'        Return True
'    Catch ex As Exception
'        Return False
'    End Try
'End Function

'Function FakePreviewCallback() As Boolean
'    Return False
'End Function

'Public Sub btnStandardviewer_ClickExtracted(ByVal Gesamtcursor As Integer)
'    checkout.checkout(clstart.myc.collFotos.Item(Gesamtcursor), myGlobalz.sitzung.aktVorgangsID)
'    glob2.OpenDocument(clstart.myc.collFotos.Item(Gesamtcursor).FullnameCheckout)
'End Sub
'Public Function getCursor4foto(ByVal cAND As Dokument, ByVal collFotos As List(Of clsFotoDokument)) As Integer
'    ' Dim cnt As Integer = 0
'    For Each adok As Dokument In collFotos
'        If adok.DateinameMitExtension.ToLower = cAND.DateinameMitExtension.ToLower Then
'            Return adok.Handlenr
'        End If
'        ' adok.Handlenr = cnt
'        ' cnt += 1
'    Next
'End Function

'Private Sub calcThumbsSize(ByVal pixelWidth As Integer,
'                            ByVal pixelHeight As Integer,
'                            ByVal thumbNailWidth As Integer,
'                            ByVal thumbNailHeight As Integer,
'                            ByRef thumbWidth As Integer,
'                            ByRef thumbHeight As Integer)
'    'isportrait ?
'    Dim ratio As Double
'    If isLandscape(pixelWidth, pixelHeight) Then
'        thumbWidth = thumbNailWidth
'        ratio = pixelHeight / pixelWidth
'        thumbHeight = CInt((ratio) * thumbNailWidth)
'    Else
'        thumbHeight = thumbNailHeight
'        ratio = pixelWidth / pixelHeight
'        thumbWidth = CInt((ratio) * thumbNailHeight)
'    End If
'End Sub

'Private Function isLandscape(ByVal pWidth As Integer, ByVal pHeight As Integer) As Boolean
'    If pWidth < 0 Or pHeight < 0 Then
'        Return Nothing
'    End If
'    If pWidth >= pHeight Then
'        Return True
'    Else
'        Return False
'    End If
'End Function
''' <summary>
''' benötigt das vollständige myGlobalz.sitzung.aktDokument
''' </summary>
''' <returns></returns>
''' <remarks></remarks>
'Public Function KoordinatenGgfLoeschen(ByVal aktfoto As Dokument) As Boolean
'    If jawirklich() Then
'        Return koordinatenloeschen(aktfoto)
'        Return True
'    Else
'        Return False
'    End If
'End Function

'Private Function jawirklich() As Boolean
'    Dim res As New MessageBoxResult
'    res = MessageBox.Show("Möchten Sie die Koordinate wirklich löschen ? " & vbCrLf & "  ", "Koordinaten es Fotos löschen' ?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Cancel)
'    Return If(res = MessageBoxResult.No, False, True)
'End Function



'Private Function koordinatenloeschen(ByVal aktfoto As Dokument) As Boolean
'    'Koordinaten in dokumente löschen
'    Try
'        aktfoto.EXIFlat = "0#0#0#"
'        aktfoto.EXIFlon = "0#0#0#"
'        speichernFotoDetails(aktfoto)
'        'objekt muss als Raumbezug gelöscht werden
'        'also 1. in tabelle Parafoto löschen
'        'und  2. in tabelle Raumbezug  löschen
'        Dim itest As Integer
'        Dim sekid% = DokArcTools.getID4Foto.execute(aktfoto.DocID)
'        nachricht("ARCHIV: getID4Foto: >0 ist erfolg: " & sekid%)
'        If sekid > 0 Then
'            itest = RBtoolsns.RBFotoLoeschen_alleDB.execute(aktfoto.DocID)
'            nachricht("ARCHIV: RB_FOTO_loeschen:  " & itest%)
'            itest = RBtoolsns.Raumbezug_loeschen_bySEKid_alleDB.execute(sekid, "5")
'            nachricht("ARCHIV: RB_loeschenBySekIDTyp:  : " & itest%)
'        End If
'        nachricht("foto wurde als raumbzug gelöscht wurde gelöscht. Bitte führen Sie einen Refresh durch.")
'        Return True
'    Catch ex As Exception
'        nachricht("fehler in koordinatenloeschen: " & ex.ToString)
'        Return False
'    End Try
'End Function

'Public Sub speichernFotoDetails(ByVal aktfoto As clsFotoDokument)
'    With aktfoto
'        Dim result As Integer = DokArcTools.dokUpdate.execute(.DocID,
'                                                                .istVeraltet,
'                                                                .Beschreibung,
'                                                                .revisionssicher,
'                                                                .Filedatum,
'                                                                .EXIFlat,
'                                                                .EXIFlon)
'    End With
'End Sub

'Sub speichernDokudetail(ByVal aktfoto As clsFotoDokument)



'End Sub

'Public Sub speichernFotoDetails(ByVal aktfoto As Dokument)

'    Dim result As Integer = DokArcTools.dokUpdate.execute(aktfoto.DocID, aktfoto.istVeraltet, aktfoto.Beschreibung,
'                                                     aktfoto.revisionssicher, aktfoto.Filedatum,
'                                                     aktfoto.EXIFlat,
'                                                     aktfoto.EXIFlon)
'End Sub

'End Module
