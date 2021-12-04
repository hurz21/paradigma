Imports System.IO
Imports DocumentFormat.OpenXml
Imports DocumentFormat.OpenXml.Packaging
Imports DocumentFormat.OpenXml.Wordprocessing
Imports A = DocumentFormat.OpenXml.Drawing
Imports DW = DocumentFormat.OpenXml.Drawing.Wordprocessing
Imports PIC = DocumentFormat.OpenXml.Drawing.Pictures
Imports System.Linq
Partial Module modAktennotiz
    Private textzumbild As IEnumerable(Of OpenXmlElement)
    Public wordDocument As WordprocessingDocument
    Public mainPart As MainDocumentPart
    Public koerper As Body
    Public para As Paragraph
    'Public parafotos As Paragraph

    'Set the font for a text run.
    Function ErzeugeAktennotizMitBildern(document As String,
                                    jpegListe As String(),
                                    fotountertitel As String(),
                                    anzahlFotosProSeite As Integer,
                                    fotosMitUnterTitel As Boolean,
                                    ByRef mitGisKarte As String,
                                    mitfotos As Boolean,
                                    kopftexte() As String,
                                    vid As String,
                                    scalierfaktor As Double,
                                         zweispaltig As Boolean,
                                         fototitelSize As String,
                                         standardfontsize As String,
                                         mitExtraZeilenumbruch As Boolean,
                                           dateiname() As String) As Boolean
        Dim outfil As IO.FileInfo
        Dim styleid As String = "jft"
        Dim stylename As String = "snJFT"
        Try
            l("ErzeugeAktennotizMitBildern")
            l("mitGisKarte " & mitGisKarte)
            outfil = New IO.FileInfo(document)
            outfil.Delete() 'alte version löschen
            l("alte version gelöscht")
            CreateWordprocessingDocument(document)
            para = koerper.AppendChild(New Paragraph)
            ' Get the paragraph properties element of the paragraph.
            Dim pPr As ParagraphProperties = New ParagraphProperties
            ' Get the Styles part for this document.
            Dim part As StyleDefinitionsPart = wordDocument.MainDocumentPart.StyleDefinitionsPart

            ' If the Styles part does not exist, add it and then add the style.

            part = AddStylesPartToPackage(wordDocument)
            AddNewStyle(part, styleid, stylename)
            ' Set the style of the paragraph.
            pPr.ParagraphStyleId = New ParagraphStyleId With {.Val = styleid}
            '-------------------------


            l("kopf erzeugen")
            Dim dateitemp As String = ""
            machDenKopf(document, kopftexte, vid, standardfontsize, mitExtraZeilenumbruch)
            l("kopf erzeugen fertig")
            Dim formatstring As String
            para = koerper.AppendChild(New Paragraph())
            If mitGisKarte.Trim.Length > 5 Then
                l("mitGisKarte !")
                mitGisKarte = kopierenDerKarte(mitGisKarte, dossierordner)
                InsertAPicture(document, mitGisKarte, scalierfaktor, "bla", fotosMitUnterTitel, "quer",
                               fototitelSize, dateitemp)
            Else
                l("ohne mitGisKarte")
            End If
            If mitfotos Then
                l("mit fotos")
                'write a first paragraph on two columns
                '  Dim parafotos = New Paragraph()
                'Dim zweispaltig As Boolean = False
                If zweispaltig Then
                    Dim paragraphSectionProperties = New SectionProperties()
                    Dim paragraphColumns = New Columns()
                    paragraphColumns.EqualWidth = True
                    paragraphColumns.ColumnCount = 2
                    paragraphSectionProperties.Append(paragraphColumns)
                    koerper.Append(paragraphSectionProperties)
                End If


                For i = 0 To jpegListe.GetUpperBound(0)
                    l(jpegListe(i))
                    If istQuerformat(jpegListe(i)) Then
                        formatstring = "quer"
                    Else
                        formatstring = "hoch"
                    End If
                    'If (i Mod 2 = 0) Then
                    '    mitZeilenumbruch = False
                    'Else
                    '    mitZeilenumbruch = True
                    'End If
                    Dim fi As New IO.FileInfo(jpegListe(i))
                    dateitemp = dateiname(i)
                    l("dateitemp " & dateitemp)
                    If fi.Exists Then
                        InsertAPicture(document, jpegListe(i), scalierfaktor, fotountertitel(i), fotosMitUnterTitel, formatstring,
                                   fototitelSize, dateitemp)
                        '6 = 2 fotos pro seite
                        '4 = 3 fortos for seite
                    Else
                        MsgBox("Foto nicht vorhanden!! ")
                        l(jpegListe(i) & " foto fehlt")
                    End If
                Next

            End If
            'Dim rPr As RunProperties = New RunProperties(New RunFonts With {.Ascii = "Arial"})
            'Dim r As Run = mainPart.Document.Descendants(Of Run).First

            'r.PrependChild(Of RunProperties)(rPr)
            wordDocument.MainDocumentPart.Document.Save()

            koerper = Nothing
            mainPart = Nothing
            wordDocument.Close()
            wordDocument.Dispose()
            l("dokument geschlossen, fertig")
            Return True
        Catch ex As Exception
            l("fehler in ErzeugeAktennotizMitBildern: " & ex.ToString)
            Return False
        End Try
    End Function

    Private Sub machDenKopf(document As String, kopftexte() As String, vid As String, standardfontsize As String, mitExtraZeilenumbruch As Boolean)
        TextAbsatzErzeugen(document, "Aktennotiz", "Arial", "40", True)
        TextAbsatzErzeugen(document, "Erstellt am: " & Now.ToString("dd.MM.yyyy"), "Arial", standardfontsize, False)
        TextAbsatzErzeugen(document, "Datum des Ereignisses: " & kopftexte(4), "Arial", standardfontsize, False)
        TextAbsatzErzeugen(document, "Bearbeiter: " & kopftexte(0), "Arial", standardfontsize, False)
        TextAbsatzErzeugen(document, "Vorgang: " & vid, "Arial", standardfontsize, False)
        TextAbsatzErzeugen(document, "Aktenzeichen: " & kopftexte(1), "Arial", standardfontsize, False)
        TextAbsatzErzeugen(document, "Sachgebiet: " & kopftexte(2), "Arial", standardfontsize, False)
        TextAbsatzErzeugen(document, "Titel: " & kopftexte(3), "Arial", standardfontsize, False)
        TextAbsatzErzeugen(document, " ", "Arial", standardfontsize, False)
        TextAbsatzErzeugen(document, " " & kopftexte(5) & ": " & kopftexte(6), "Arial", standardfontsize, True)

        'der Haupttext ist die notiz, hier zeilenumbruch
        kopftexte(7) = kopftexte(7).Replace(Environment.NewLine, "#") 'wird durch zeilenumbruch ersetzt

        'TextAbsatzErzeugenOhneBreak(document, kopftexte(7), "Arial", CStr(CInt(standardfontsize) + 0), False, mitExtraZeilenumbruch) '0 war 2
        'TextAbsatzErzeugenOhneBreak(document, " ", "Arial", CStr(CInt(standardfontsize) + 0), False, mitExtraZeilenumbruch)
        TextAbsatzErzeugen(document, " " & kopftexte(7), "Arial", standardfontsize, False)
        For i = 0 To 25

            'TextAbsatzErzeugen(document, " " & "              ", "Arial", standardfontsize, False)
        Next
    End Sub

    Private Sub TextAbsatzErzeugenOhneBreak(ByVal filepath As String, txt As String, fontname As String,
                                  fontsize As String, fett As Boolean, mitExtraZeilenumbruch As Boolean)
        Dim run As Run = para.AppendChild(New Run)
        Dim a() As String
        'Dim run As New Run
        Dim rPr As New RunProperties
        Dim runf As New RunFonts
        Dim spacing As New SpacingBetweenLines()
        spacing.After = "0"
        spacing.Before = "0"
        spacing.Line = "0"
        spacing.LineRule = LineSpacingRuleValues.Exact

        Dim pPr As ParagraphProperties = New ParagraphProperties

        ' Get the Styles part for this document.
        Dim part As StyleDefinitionsPart = wordDocument.MainDocumentPart.StyleDefinitionsPart
        Dim styleid As String, stylename As String

        ' If the Styles part does not exist, add it and then add the style.

        '   part = AddStylesPartToPackage(wordDocument)
        AddNewStyle(part, styleid, stylename)
        ' Set the style of the paragraph.
        pPr.ParagraphStyleId = New ParagraphStyleId With {.Val = styleid}


        spacing.After = "0"
        spacing.Before = "0"
        spacing.Line = "0"
        spacing.LineRule = LineSpacingRuleValues.Exact
        ', LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" }
        '  spacing = 1
        runf.Ascii = fontname

        Dim size As New FontSize()
        size.Val = fontsize
        'run.Append(spacing)
        'rPr.Append(spacing)
        para.ParagraphProperties = pPr
        para.Append(spacing)

        rPr.Append(runf)
        rPr.Append(size)

        Dim Bold As Bold = New Bold()
        Bold.Val = OnOffValue.FromBoolean(fett)
        rPr.AppendChild(Bold)

        run.AppendChild(Of RunProperties)(rPr)

        a = txt.Split("#"c)
        For i = 0 To a.GetUpperBound(0)
            run.AppendChild(New Text(a(i)))
            If mitExtraZeilenumbruch Then
                run.AppendChild(New Break())
            End If

        Next
    End Sub

    Private Function istQuerformat(v As String) As Boolean
        Dim img As System.Drawing.Image
        img = System.Drawing.Image.FromFile(v)

        'Richtiges Seitenverhältnis ausrechnen
        If img.Width > img.Height Then
            img.Dispose()
            img = Nothing
            Return True
        Else
            img.Dispose()
            img = Nothing
            Return False
        End If
    End Function

    Public Sub TextAbsatzErzeugen(ByVal filepath As String, txt As String, fontname As String,
                                  fontsize As String, fett As Boolean)
        Dim run As Run = para.AppendChild(New Run)
        Dim a() As String
        'Dim run As New Run
        Dim rPr As New RunProperties
        Dim runf As New RunFonts
        runf.Ascii = fontname
        Dim size As New FontSize()
        size.Val = fontsize

        rPr.Append(runf)
        rPr.Append(size)

        Dim Bold As Bold = New Bold()
        Bold.Val = OnOffValue.FromBoolean(fett)
        rPr.AppendChild(Bold)

        run.AppendChild(Of RunProperties)(rPr)

        a = txt.Split("#"c)
        For i = 0 To a.GetUpperBound(0)
            run.AppendChild(New Text(a(i)))
            run.AppendChild(New Break())
        Next
        ' Close the handle explicitly.
        'wordprocessingDocument.Close()
    End Sub
    Sub l(text As String)
        ' Debug.Print(text)
        logf.WriteLine(text)
        logf.Flush()
    End Sub

    Public Sub InsertAPicture(ByVal document As String, ByVal JPEGfileName As String, scalierfaktor As Double, untertitel As String,
                              fotosMitUnterTitel As Boolean, hochquerformat As String, fototitelSize As String, dateitemp As String)
        Dim imagePart As ImagePart
        l("in InsertAPicture " & dateitemp & " / " & untertitel)
        Try
            'imagePart = mainPart.AddImagePart(ImagePartType.Png)
            l("in InsertAPicture 1")
            If dateitemp.ToLower.EndsWith("png") Then
                imagePart = mainPart.AddImagePart(ImagePartType.Png)
            End If
            If dateitemp.ToLower.EndsWith("bmp") Then
                imagePart = mainPart.AddImagePart(ImagePartType.Bmp)
            End If
            If dateitemp.ToLower.EndsWith("gif") Then
                imagePart = mainPart.AddImagePart(ImagePartType.Gif)
            End If
            'If dateitemp.ToLower.EndsWith("heic") Then
            '    imagePart = mainPart.AddImagePart(ImagePartType.hei)
            'End If
            l("in InsertAPicture 2")
            If dateitemp.ToLower.EndsWith("jpg") Or JPEGfileName.ToLower.EndsWith("jpeg") Then
                imagePart = mainPart.AddImagePart(ImagePartType.Jpeg)
            End If
            l("in InsertAPicture 3")
            If dateitemp.ToLower.EndsWith("tif") Or JPEGfileName.ToLower.EndsWith("tiff") Then
                imagePart = mainPart.AddImagePart(ImagePartType.Tiff)
                l("in InsertAPicture 4")
            End If
            Using stream As New FileStream(JPEGfileName, FileMode.Open)
                imagePart.FeedData(stream)
            End Using
            l("in InsertAPicture 5")
            AddImageToBody(wordDocument, mainPart.GetIdOfPart(imagePart), scalierfaktor, hochquerformat, untertitel,
                           fotosMitUnterTitel, fototitelSize)
            l("in InsertAPicture 6")
        Catch ex As Exception
            l("Fehler in InsertAPicture" & ex.ToString)
        End Try
    End Sub
    Public Sub AddImageToBody(ByVal wordDoc As WordprocessingDocument, ByVal relationshipId As String,
                          scalierfaktor As Double, format As String, untertitel As String,
                              fotosMitUnterTitel As Boolean,
                              fototitelSize As String)
        Dim emux As Long
        Dim emuy As Long
        Try
            If format = "quer" Then
                emux = 990000L
                emuy = 692000L
            Else
                emux = 692000L
                emuy = 990000L
            End If

            'Dim fak As Double = 2.8
            'fak=.5
            'emux = CInt(2077839 * fak)
            'emuy = CInt(3171439 * fak)
            emux = CInt(emux * scalierfaktor)
            emuy = CInt(emuy * scalierfaktor)
            ' Define the reference of the image.
            Dim element = New Drawing(
                                  New DW.Inline(
                              New DW.Extent() With {.Cx = emux, .Cy = emuy},
                              New DW.EffectExtent() With {.LeftEdge = 0L, .TopEdge = 0L, .RightEdge = 0L, .BottomEdge = 0L},
                              New DW.DocProperties() With {.Id = CType(1UI, UInt32Value), .Name = "Picture1"},
                              New DW.NonVisualGraphicFrameDrawingProperties(
                                  New A.GraphicFrameLocks() With {.NoChangeAspect = True}
                                  ),
                              New A.Graphic(New A.GraphicData(
                                            New PIC.Picture(
                                                New PIC.NonVisualPictureProperties(
                                                    New PIC.NonVisualDrawingProperties() With {.Id = 0UI, .Name = "Koala.jpg"},
                                                    New PIC.NonVisualPictureDrawingProperties()
                                                    ),
                                                New PIC.BlipFill(
                                                    New A.Blip(
                                                        New A.BlipExtensionList(
                                                            New A.BlipExtension() With {.Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}"})
                                                        ) With {.Embed = relationshipId, .CompressionState = A.BlipCompressionValues.Print},
                                                    New A.Stretch(
                                                        New A.FillRectangle()
                                                        )
                                                    ),
                                                New PIC.ShapeProperties(
                                                    New A.Transform2D(
                                                        New A.Offset() With {.X = 0L, .Y = 0L},
                                                        New A.Extents() With {.Cx = emux, .Cy = emuy}),
                                                    New A.PresetGeometry(
                                                        New A.AdjustValueList()
                                                        ) With {.Preset = A.ShapeTypeValues.Rectangle}
                                                    )
                                                )
                                            ) With {.Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture"}
                                        )
                                    ) With {.DistanceFromTop = 0UI,
                                            .DistanceFromBottom = 0UI,
                                            .DistanceFromLeft = 0UI,
                                            .DistanceFromRight = 0UI}
                                )



            ' neuemetheode(untertitel, fotosMitUnterTitel, mitZeilenumbruch, element)
            altemetheode(untertitel, fotosMitUnterTitel, element, fototitelSize)
            ' altemetheodeOriginal(untertitel, fotosMitUnterTitel, mitZeilenumbruch, element)
        Catch ex As Exception
            l("fehler in AddImageToBody: " & ex.ToString)
        End Try
    End Sub
    Private Sub altemetheode(untertitel As String, fotosMitUnterTitel As Boolean,
                             element As Drawing, fototitelSize As String)
        'Append the reference To body, the element should be In a Run.
        Dim para As Paragraph = koerper.AppendChild(New Paragraph())
        Dim run As Run = para.AppendChild(New Run())
        Dim rPr As New RunProperties
        Dim runf As New RunFonts
        Dim size As New FontSize()
        Try
            runf.Ascii = "Arial" : size.Val = fototitelSize
            rPr.Append(runf)
            rPr.Append(size)
            run.AppendChild(Of RunProperties)(rPr)
            If fotosMitUnterTitel Then
                run.AppendChild(New Text(untertitel))
            End If
            koerper.AppendChild(New Paragraph(New Run(element)))
        Catch ex As Exception
            l("fehler in AddImageToBody: " & ex.ToString)
        End Try
    End Sub
    'Private Sub altemetheodeOriginal(untertitel As String, fotosMitUnterTitel As Boolean, mitZeilenumbruch As Boolean, element As Drawing)
    '    'Append the reference To body, the element should be In a Run.

    '    Dim para As Paragraph = koerper.AppendChild(New Paragraph())
    '    Dim run As Run = para.AppendChild(New Run())
    '    Dim rPr As New RunProperties
    '    Dim runf As New RunFonts
    '    Dim size As New FontSize()
    '    Try
    '        runf.Ascii = "Arial"
    '        size.Val = "8"

    '        rPr.Append(runf)
    '        rPr.Append(size)
    '        run.AppendChild(Of RunProperties)(rPr)
    '        If fotosMitUnterTitel Then
    '            run.AppendChild(New Text(untertitel))
    '        End If

    '        koerper.AppendChild(New Paragraph(New Run(element)))
    '    Catch ex As Exception
    '        l("fehler in AddImageToBody: " & ex.ToString)
    '    End Try
    'End Sub
    'Private Sub neuemetheode(untertitel As String, fotosMitUnterTitel As Boolean, mitZeilenumbruch As Boolean, element As Drawing)
    '    ' Append the reference to body, the element should be in a Run.


    '    'If mitZeilenumbruch Then
    '    '    Dim para As Paragraph = koerper.AppendChild(New Paragraph()) '' sont 
    '    'Else

    '    'End If

    '    Dim run As Run = para.AppendChild(New Run())
    '    Dim rPr As New RunProperties
    '    Dim runf As New RunFonts
    '    Dim size As New FontSize()
    '    Try
    '        runf.Ascii = "Arial"
    '        size.Val = "8"

    '        rPr.Append(runf)
    '        rPr.Append(size)
    '        '   run.AppendChild(Of RunProperties)(rPr)


    '        If fotosMitUnterTitel Then
    '            'If mitZeilenumbruch Then
    '            '    run.AppendChild(New Text(untertitel))
    '            'Else
    '            run.AppendChild(New Text(untertitel))
    '            'End If

    '        Else
    '            ' run.AppendChild(New Text(" _ "))
    '        End If

    '        'If mitZeilenumbruch Then
    '        ' run.AppendChild(New Run(element))
    '        'Else

    '        'End If
    '        If mitZeilenumbruch Then
    '            koerper.AppendChild(New Paragraph(New Run(element)))
    '        Else
    '            run.AppendChild(element)
    '        End If
    '        ' koerper.AppendChild(New Paragraph(New Run(element)))
    '        'para.AppendChild((New Run(element)))

    '    Catch ex As Exception
    '        l("fehler in AddImageToBody: " & ex.ToString)
    '    End Try
    'End Sub


    Public Sub CreateWordprocessingDocument(ByVal filepath As String)
        Try
            l(" MOD CreateWordprocessingDocument anfang")
            ' Create a document by supplying the filepath.
            wordDocument = WordprocessingDocument.Create(filepath, WordprocessingDocumentType.Document)


            ' Add a main document part. 
            mainPart = wordDocument.AddMainDocumentPart()
            ' Create the document structure and add some text. 
            mainPart.Document = New Document()

            koerper = mainPart.Document.AppendChild(New Body())





            l(" MOD CreateWordprocessingDocument ende")
        Catch ex As Exception
            l("Fehler in CreateWordprocessingDocument: " & ex.ToString())
        End Try
    End Sub
    Sub getParams(commandLine As String, ByRef vid As String, ByRef ereignisid As String,
                  ByRef mitGISkarteFullName As String,
                  ByRef reportModus As String, ByRef bearbeiterid As String)
        Dim a$()
        Dim b$()
        Try
            a = commandLine.Split("/"c)
            b = a(1).Split("#"c)
            vid = b(0).Replace("vid=", "").Trim
            ereignisid = b(1).Replace("eid=", "").Trim
            bearbeiterid = b(4).Replace("bid=", "").Trim
            mitGISkarteFullName = b(2).Trim
            reportModus = b(3).Trim.Replace("modus=", "")
        Catch ex As Exception
            l("fehler in getParams----------  e" & ex.ToString)
        End Try
    End Sub
    Private Function kopierenDerKarte(mitGisKarte As String, dossierordner As String) As String
        Dim kopie As String
        Try
            l("kopierenDerKarte----------  ")
            Dim fi As New IO.FileInfo(mitGisKarte)
            kopie = dossierordner & Now.ToString("yyyyMMddhhmmss_") & "loeschmich.png"
            fi.CopyTo(kopie)
            l("kopieren erfolgreihc nach: " & kopie)
            Return kopie
        Catch ex As Exception
            l("fehler in kopierenDerKarte----------  e" & ex.ToString)
            Return "fehler"
        End Try
    End Function
    ' Create a new style with the specified styleid and stylename and add it to the specified
    ' style definitions part.
    Private Sub AddNewStyle(ByVal styleDefinitionsPart As StyleDefinitionsPart,
                            ByRef styleid As String, ByVal stylename As String)
        ' Get access to the root element of the styles part.
        Dim styles As Styles = styleDefinitionsPart.Styles
        Dim spacing As New SpacingBetweenLines()
        spacing.After = "0"
        spacing.Before = "0"
        spacing.Line = "0"
        spacing.LineRule = LineSpacingRuleValues.Exact
        ' Create a new paragraph style and specify some of the properties.
        Dim style As New Style With {.Type = StyleValues.Paragraph,
                                     .StyleId = styleid,
                                     .CustomStyle = True}
        Dim styleName1 As New StyleName With {.Val = stylename}
        Dim basedOn1 As New BasedOn With {.Val = "Normal"}

        Dim nextParagraphStyle1 As New NextParagraphStyle With {.Val = "Normal"}
        style.Append(spacing)
        style.Append(styleName1)
        style.Append(basedOn1)
        style.Append(nextParagraphStyle1)

        ' Create the StyleRunProperties object and specify some of the run properties.
        Dim styleRunProperties1 As New StyleRunProperties
        Dim bold1 As New Bold
        Dim color1 As New Color With {.ThemeColor = ThemeColorValues.Accent2}
        Dim font1 As New RunFonts With {.Ascii = "Arial"}
        Dim italic1 As New Italic
        ' Specify a 12 point size.
        Dim fontSize1 As New FontSize With {.Val = "14"}
        styleRunProperties1.Append(bold1)
        'styleRunProperties1.Append(color1)
        styleRunProperties1.Append(font1)
        styleRunProperties1.Append(fontSize1)
        'styleRunProperties1.Append(italic1)

        ' Add the run properties to the style.
        style.Append(styleRunProperties1)

        ' Add the style to the styles part.
        styles.Append(style)
    End Sub
    ' Add a StylesDefinitionsPart to the document.  Returns a reference to it.
    Public Function AddStylesPartToPackage(ByVal doc As WordprocessingDocument) _
        As StyleDefinitionsPart
        Dim part As StyleDefinitionsPart
        part = doc.MainDocumentPart.AddNewPart(Of StyleDefinitionsPart)()
        Dim root As New Styles
        root.Save(part)
        Return part
    End Function
End Module
