Imports System.Data
Imports System.Drawing.Imaging

Public Class clsBerichte
    Private myCanvas As Canvas
    Private dierange As clsRange ' clstart.myc.kartengen.aktMap.aktrange
    Private ableitungskreis As New clsAbleitungskreis

    Sub New(ByVal _myCanvas As Canvas, ByVal _dierange As clsRange, ByVal _ableitungskreis As clsAbleitungskreis)
        myCanvas = _myCanvas
        dierange = _dierange
        ableitungskreis = _ableitungskreis
    End Sub

    Public Shared Sub fotobucherstellen(ByVal myCanvas As Canvas,
                                        ByVal nurFotos As Boolean,
                                        dliste As List(Of clsPresDokumente),
                                        renge As LibGISmapgenerator.clsRange)
        MessageBox.Show("Es wird eine temporäre Word-Datei erstellt!. Diese Datei dient nur dem Ausdrucken, und ist nur temporär gültig. " & vbCrLf &
                        "Sie wird NICHT in das Archiv aufgenommen, sondern nach Gebrauch VERNICHTET!" & vbCrLf &
                        "Die folgende Aktion kann einige Minuten dauern. " & vbCrLf &
                        "Während dieser Zeit ist das Programm blockiert. " & vbCrLf &
                        "Bitte haben Sie etwas Geduld. Warten Sie bis die Worddatei fertig ist und automatisch geöffnet wird." & vbCrLf,
                        "Dossier erstellen", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)
        Dim a As New clsBerichte(myCanvas, renge, Nothing)
        If detail_dokuauswahl.anzahlAusgewaehlt(dliste) < 1 Then
            MessageBox.Show("Sie müssen erst Fotos auswählen. Nutzen Sie hierfür die Mehrfachauswahl.")
            Exit Sub
        Else
            a.erstellekurzdossier(nurFotos, dliste)
        End If
    End Sub

    Public Shared Sub erstelleKartendruck(ByVal myCanvas As Canvas, ByVal ableitungskreis As clsAbleitungskreis)
        'MessageBox.Show("Es wird eine Word-Datei erstellt!. Die folgende Aktion kann einige Sekunden dauern. " & vbCrLf &
        '                        "Während dieser Zeit ist das Programm blockiert. " & vbCrLf &
        '                        "Bitte haben Sie etwas Geduld. Warten Sie bis die Worddatei fertig ist und automatisch geöffnet wird." & vbCrLf,
        '                        "Karte drucken", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)
        Dim a As New clsBerichte(myCanvas, clstart.myc.kartengen.aktMap.aktrange, ableitungskreis)
        a.Kartendruck_do()
    End Sub

    Sub Kartendruck_do()
        Dim rtb2 As New RichTextBox
        Dim flw2 As New FlowDocument
        Dim paraHeader = New Paragraph()
        Dim titel$ = "Druck " & Now.Date
        Dim wasserzeichen As String = "Kreis Offenbach - Paradigma. " & myGlobalz.sitzung.aktBearbeiter.Name & ". " & Now.ToString
        raumbezugsbildeinfuegen(flw2, myCanvas, wasserzeichen, 0.8, ableitungskreis)
        'paraHeader.FontWeight = FontWeights.Bold
        'paraHeader.Inlines.Add(New Run(wasserzeichen))
        ''  Dim test = New RotateTransform(50)
        'flw2.Blocks.Add(paraHeader)
        rtb2.Document = flw2
        Dim ausgabeDIR$ = clstart.mycsimple.getParadigma_archiv_temp() & "" & myGlobalz.sitzung.aktVorgangsID
        VerzeichnisAnlegen(ausgabeDIR)
        Dim ausgabedatei$ = ausgabeDIR & "\" & "Kartendruck_" & Now.Year & Now.Month & Now.Minute & Now.Millisecond & ".rtf"
        Dim filename$ = schreibeInTIFDatei(rtb2, ausgabedatei$)
        If Not filename.StartsWith("Fehler") Then
            glob2.OpenDocument(filename)
        End If
    End Sub

    Sub erstellekurzdossier(ByVal nurFotos As Boolean,
                             dliste As List(Of clsPresDokumente))
        Dim rtb2 As New RichTextBox
        Dim flw2 As New FlowDocument
        Dim paraHeader = New Paragraph()
        Dim titel$ = "Kurzdossier"
        clsAktennotiz.NurDerHeader(flw2, paraHeader, titel)
        If nurFotos Then

        Else
            vorgaengeKurzListe(flw2, paraHeader)
            beteiligteKurzListe(flw2, paraHeader)
            raumbezuegeKurzListe(flw2, paraHeader)
            raumbezugsbildeinfuegen(flw2, myCanvas, "", 0.8, ableitungskreis)
            dokumenteKurzListe(flw2, paraHeader)
            trennseite(flw2, paraHeader, "Details")

        End If

        dokumenteDias(flw2, paraHeader, dliste)
        rtb2.Document = flw2
        Dim ausgabeDIR$ = clstart.mycsimple.getParadigma_archiv_temp() & "" & myGlobalz.sitzung.aktVorgangsID
        VerzeichnisAnlegen(ausgabeDIR)
        Dim ausgabedatei$ = ausgabeDIR & "\" & "Kurzdossier_" & Now.Year & Now.Month & Now.Minute & Now.Millisecond & ".rtf"
        Dim filename$ = schreibeInTIFDatei(rtb2, ausgabedatei$)
        If Not filename.StartsWith("Fehler") Then
            glob2.OpenDocument(filename)
        End If
    End Sub

    Private Shared Sub VerzeichnisAnlegen(ByVal at$)
        IO.Directory.CreateDirectory(at$)
    End Sub

    Public Shared Function schreibeInTIFDatei(ByVal rtb2 As RichTextBox, ByVal dateiname As String) As String
        Try
            Dim fs As IO.FileStream
            fs = New IO.FileStream(dateiname$, IO.FileMode.Create)
            Dim tr As New TextRange(rtb2.Document.ContentStart, rtb2.Document.ContentEnd)
            tr.Save(fs, DataFormats.Rtf)
            rtb2.Selection.Save(fs, DataFormats.Rtf)
            fs.Close()
            Return dateiname$
        Catch ex As Exception
            Return "Fehler: " & ex.ToString
        End Try
    End Function

    Private Sub vorgaengeKurzListe(ByVal flw2 As FlowDocument, ByVal paraHeader As System.Windows.Documents.Paragraph)
        paraHeader = New Paragraph()
        paraHeader.FontSize = 20
        paraHeader.FontWeight = FontWeights.Bold
        paraHeader.Inlines.Add(New Run("a) Verlauf"))
        paraHeader.Inlines.Add(New LineBreak())
        flw2.Blocks.Add(paraHeader)
        For Each drow As DataRow In myGlobalz.sitzung.EreignisseRec.dt.AsEnumerable
            paraHeader = New Paragraph()
            paraHeader.FontSize = 12
            paraHeader.FontWeight = FontWeights.Normal
            'paraHeader.Inlines.Add(New Run((Format(CDate(drow.Item("datum")), "dd.MM.yyyy")) & ": "))
            paraHeader.Inlines.Add(New Run((CDate(drow.Item("datum")).ToString("d")) & ": "))

            flw2.Blocks.Add(paraHeader)

            paraHeader.FontSize = 12
            paraHeader.FontWeight = FontWeights.Bold
            paraHeader.Inlines.Add(New Run(drow.Item("Art").ToString & " "))
            flw2.Blocks.Add(paraHeader)

            paraHeader.FontSize = 12
            paraHeader.FontWeight = FontWeights.Normal
            paraHeader.Inlines.Add(New Run(drow.Item("Beschreibung").ToString))
            paraHeader.Inlines.Add(New LineBreak())
            flw2.Blocks.Add(paraHeader)
        Next
    End Sub

    Private Sub beteiligteKurzListe(ByVal flw2 As FlowDocument, ByVal paraHeader As System.Windows.Documents.Paragraph)
        paraHeader = New Paragraph()
        paraHeader.FontSize = 20
        paraHeader.FontWeight = FontWeights.Bold
        paraHeader.Inlines.Add(New Run("b) Beteiligte"))
        paraHeader.Inlines.Add(New LineBreak())
        flw2.Blocks.Add(paraHeader)
        For Each drow As DataRow In myGlobalz.sitzung.beteiligteREC.dt.AsEnumerable

            paraHeader = New Paragraph()


            'paraHeader.FontSize = 12
            'paraHeader.FontWeight = FontWeights.Normal
            'paraHeader.Inlines.Add(New Run(drow.Item("anrede").ToString & ", "))
            'flw2.Blocks.Add(paraHeader)

            paraHeader.FontSize = 12
            paraHeader.FontWeight = FontWeights.Normal
            paraHeader.Inlines.Add(New Run(drow.Item("rolle").ToString & ", "))
            'flw2.Blocks.Add(paraHeader)

            paraHeader.FontSize = 12
            paraHeader.FontWeight = FontWeights.Bold
            paraHeader.Inlines.Add(New Run(drow.Item("nachname").ToString & ", "))
            'flw2.Blocks.Add(paraHeader)

            paraHeader.FontSize = 12
            paraHeader.FontWeight = FontWeights.Normal
            paraHeader.Inlines.Add(New Run(drow.Item("Vorname").ToString & ", "))
            'flw2.Blocks.Add(paraHeader)

            paraHeader.FontSize = 12
            paraHeader.FontWeight = FontWeights.Normal
            paraHeader.Inlines.Add(New Run(drow.Item("PLZ").ToString & ", "))
            'flw2.Blocks.Add(paraHeader)

            paraHeader.FontSize = 12
            paraHeader.FontWeight = FontWeights.Normal
            paraHeader.Inlines.Add(New Run(drow.Item("gemeindename").ToString & ", "))
            'flw2.Blocks.Add(paraHeader)

            paraHeader.FontSize = 12
            paraHeader.FontWeight = FontWeights.Normal
            paraHeader.Inlines.Add(New Run(drow.Item("strasse").ToString & ", "))
            'flw2.Blocks.Add(paraHeader)


            paraHeader.FontSize = 12
            paraHeader.FontWeight = FontWeights.Normal
            paraHeader.Inlines.Add(New Run(drow.Item("hausnr").ToString & ", "))
            'flw2.Blocks.Add(paraHeader) 

            paraHeader.FontSize = 12
            paraHeader.FontWeight = FontWeights.Normal
            paraHeader.Inlines.Add(New Run(drow.Item("FFemail").ToString & ", "))
            'flw2.Blocks.Add(paraHeader)

            paraHeader.FontSize = 12
            paraHeader.FontWeight = FontWeights.Normal
            paraHeader.Inlines.Add(New Run(drow.Item("FFtelefon1").ToString & ", "))
            'flw2.Blocks.Add(paraHeader)

            paraHeader.FontSize = 12
            paraHeader.FontWeight = FontWeights.Normal
            paraHeader.Inlines.Add(New Run(drow.Item("Orgname").ToString & ", "))
            'flw2.Blocks.Add(paraHeader)

            paraHeader.FontSize = 12
            paraHeader.FontWeight = FontWeights.Normal
            paraHeader.Inlines.Add(New Run(drow.Item("Orgzusatz").ToString & ", "))
            'flw2.Blocks.Add(paraHeader)

            paraHeader.FontSize = 12
            paraHeader.FontWeight = FontWeights.Normal
            paraHeader.Inlines.Add(New Run(drow.Item("Orgtyp1").ToString))
            paraHeader.Inlines.Add(New LineBreak())
            flw2.Blocks.Add(paraHeader)
        Next
    End Sub
    Private Sub raumbezuegeKurzListe(ByVal flw2 As FlowDocument, ByVal paraHeader As System.Windows.Documents.Paragraph)
        paraHeader = New Paragraph()
        paraHeader.FontSize = 20
        paraHeader.FontWeight = FontWeights.Bold
        paraHeader.Inlines.Add(New Run("c) Raumbezüge"))
        paraHeader.Inlines.Add(New LineBreak())
        flw2.Blocks.Add(paraHeader)
        If myGlobalz.sitzung.raumbezugsRec.dt Is Nothing Then
            paraHeader = New Paragraph()
            paraHeader.FontSize = 12
            paraHeader.FontWeight = FontWeights.Normal
            'paraHeader.Inlines.Add(New Run((Format(CDate(drow.Item("datum")), "dd.MM.yyyy")) & ": "))
            paraHeader.Inlines.Add(New Run("Es liegen keine Raumbezüge vor!"))
            paraHeader.Inlines.Add(New LineBreak())
        Else
            For Each drow As DataRow In myGlobalz.sitzung.raumbezugsRec.dt.AsEnumerable
                paraHeader = New Paragraph()
                paraHeader.FontSize = 12
                paraHeader.FontWeight = FontWeights.Normal
                'paraHeader.Inlines.Add(New Run((Format(CDate(drow.Item("datum")), "dd.MM.yyyy")) & ": "))
                paraHeader.Inlines.Add(New Run(((drow.Item("typ")).ToString()) & ": "))

                flw2.Blocks.Add(paraHeader)

                paraHeader.FontSize = 12
                paraHeader.FontWeight = FontWeights.Bold
                paraHeader.Inlines.Add(New Run(drow.Item("titel").ToString.Trim & " "))
                flw2.Blocks.Add(paraHeader)

                paraHeader.FontSize = 12
                paraHeader.FontWeight = FontWeights.Normal
                paraHeader.Inlines.Add(New Run(drow.Item("abstract").ToString.Trim))
                paraHeader.Inlines.Add(New LineBreak())
                flw2.Blocks.Add(paraHeader)
            Next
        End If
    End Sub
    Private Sub dokumenteKurzListe(ByVal flw2 As FlowDocument, ByVal paraHeader As System.Windows.Documents.Paragraph)
        paraHeader = New Paragraph()
        paraHeader.FontSize = 20
        paraHeader.FontWeight = FontWeights.Bold
        paraHeader.Inlines.Add(New Run("d) Dokumente"))
        paraHeader.Inlines.Add(New LineBreak())
        flw2.Blocks.Add(paraHeader)
        If myGlobalz.Arc.vorgangDocDt Is Nothing Then
            Exit Sub
        End If
        For Each drow As DataRow In myGlobalz.Arc.vorgangDocDt.AsEnumerable
            paraHeader = New Paragraph()
            paraHeader.FontSize = 12
            paraHeader.FontWeight = FontWeights.Normal
            'paraHeader.Inlines.Add(New Run((Format(CDate(drow.Item("datum")), "dd.MM.yyyy")) & ": "))
            paraHeader.Inlines.Add(New Run(((drow.Item("Typ")).ToString()) & ": "))

            flw2.Blocks.Add(paraHeader)

            paraHeader.FontSize = 12
            paraHeader.FontWeight = FontWeights.Bold
            paraHeader.Inlines.Add(New Run(drow.Item("Dateinameext").ToString.Trim & " "))
            flw2.Blocks.Add(paraHeader)

            paraHeader.FontSize = 12
            paraHeader.FontWeight = FontWeights.Normal
            paraHeader.Inlines.Add(New Run(drow.Item("Beschreibung").ToString.Trim))
            paraHeader.Inlines.Add(New LineBreak())
            flw2.Blocks.Add(paraHeader)
        Next
    End Sub

    ''' <summary>
    ''' 
    ''' 
    ''' </summary>
    ''' <param name="flw2"></param>
    ''' <param name="myCanvas"></param>
    ''' <param name="labeltext"> der einzufuegende text</param>
    ''' <param name="faktor">proportionale scalierung des bildes</param>
    ''' <remarks></remarks>
    Sub raumbezugsbildeinfuegen(ByVal flw2 As FlowDocument, ByVal myCanvas As Canvas, ByVal labeltext As String, ByVal faktor As Single, ByVal ableitungskreis As clsAbleitungskreis)
        Try
            Dim zieldatei As String = clstart.mycsimple.getParadigma_archiv_temp() & "test3.jpg"
            Dim neudatei As String = clstart.mycsimple.getParadigma_archiv_temp() & "neu.jpg"
            Dim obenlinks As New System.Drawing.Point(90, 0)
            UEkarteMitellipssenErstellen(zieldatei$, myCanvas, ableitungskreis)
            'text einfuegen
            textInBitmap.TextOnImage(zieldatei, neudatei, labeltext, ImageFormat.Jpeg, New System.Drawing.Font("Times", 10), System.Drawing.Color.Black, obenlinks)

            Dim image As New Image
            Dim bimg As BitmapImage = New BitmapImage()
            bimg.BeginInit()
            bimg.UriSource = New Uri(neudatei, UriKind.Absolute)
            bimg.Rotation = Rotation.Rotate270

            bimg.EndInit()

            image.Width = bimg.Width * faktor
            image.Height = bimg.Height * faktor
            image.Source = bimg

            flw2.Blocks.Add(New BlockUIContainer(image))
        Catch ex As Exception
            MsgBox("raumbezugsbildeinfuegen: " & ex.ToString)
        End Try
    End Sub

    Private Sub UEkarteMitellipssenErstellen(ByVal zieldatei$, ByVal myCanvas As Canvas, ByVal ableitungskreis As clsAbleitungskreis)
        Dim tempdatei$ = clstart.mycsimple.getParadigma_archiv_temp() & "\test2.jpg"
        Dim test As New IO.FileInfo(clstart.myc.kartengen.gifKartenDateiFullName)
        If Not test.Exists Then
            MessageBox.Show("Das Kartenbild der Minimap kann nicht gefunden werden! Abbruch!")
            Exit Sub
        Else
            erstellebilddatei(zieldatei, tempdatei, myCanvas, clstart.myc.kartengen.gifKartenDateiFullName, ableitungskreis)
        End If
    End Sub

    Sub erstellebilddatei(ByVal zieldatei$, ByVal tempdatei$, ByVal myCanvas As Canvas, ByVal kartenbild As String, ByVal ableitungskreis As clsAbleitungskreis)
        'von gif nach jpg umwandeln
        Dim bmp As New WriteableBitmap(New BitmapImage(New Uri(kartenbild$, UriKind.Absolute)))
        Bitmapspeichern(tempdatei$, bmp)

        bmp = New WriteableBitmap(New BitmapImage(New Uri((tempdatei$), UriKind.Absolute)))
        refreshEllipsen(myCanvas, dierange, bmp)
        Bitmapspeichern(zieldatei, bmp)
    End Sub

    Public Sub refreshEllipsen(ByVal myCanvas As Canvas, ByVal dierange As clsRange, ByVal bmp As WriteableBitmap)
        Dim tag As String
        Dim radius As Integer = 10
        Dim aktpoint As New myPoint
        Dim kreiscanvas As New clsCanvas
        Dim fuellfarbe = New SolidColorBrush(Color.FromRgb(255, 11, 11))
        Dim randfarbe = New SolidColorBrush(Color.FromRgb(255, 11, 11))
        kreiscanvas.w = CLng(myCanvas.Width)
        kreiscanvas.h = CLng(myCanvas.Height)
        If keineRaumbezugeVorhanden() Then
            nachricht("keine ellipsendaten")
        Else
            Try
                Dim zaehler As Integer = 0
                For Each rau As DataRow In myGlobalz.sitzung.raumbezugsRec.dt.AsEnumerable
                    tag = Convert.ToString(rau.Item("titel")) & " " & Convert.ToString(rau.Item("abstract"))
                    If Convert.ToInt16(rau.Item("typ")) = RaumbezugsTyp.Foto Or
                      Convert.ToInt16(rau.Item("typ")) = RaumbezugsTyp.Adresse Or
                      Convert.ToInt16(rau.Item("typ")) = RaumbezugsTyp.Punkt Then
                        aktpoint.X = Convert.ToDouble(rau.Item("rechts"))
                        aktpoint.Y = Convert.ToDouble(rau.Item("hoch"))
                        If aktpoint.X > 3000 And aktpoint.Y > 3000 Then
                            male(aktpoint, radius, dierange, kreiscanvas, bmp, fuellfarbe, randfarbe)
                        End If
                    End If
                    If Convert.ToInt16(rau.Item("typ")) = RaumbezugsTyp.Polygon Or
                       Convert.ToInt16(rau.Item("typ")) = RaumbezugsTyp.Flurstueck Then
                        myGlobalz.sitzung.aktPolygon.RaumbezugsID = CLng(Convert.ToString(rau.Item("RaumbezugsID")))

                        'If myGlobalz.sitzung.aktPolygon.RaumbezugsID > 0 Then
                        '    PolygonTools.RB_ParaPolygon_holen(myGlobalz.sitzung.aktPolygon.RaumbezugsID)
                        '    PolygonTools.DTaufPolygonObjektabbilden(myGlobalz.sitzung.tempREC.dt)
                        '    malepolygon(myGlobalz.sitzung.aktPolygon, radius, dierange, kreiscanvas, bmp, fuellfarbe, randfarbe)

                        '    'addPolygonFromDB(myGlobalz.sitzung.aktPolygon, tag, dierange, myCanvas, kreiscanvas,
                        '    '                 myFillColorBrush, myStrokeColorBrush, "", zindex, zaehler)
                        '    zaehler += 1
                        'Else
                        '    nachricht("Fehler: das polygon hat sekid=0")
                        'End If
                    End If
                Next
            Catch ex As Exception
                nachricht_und_Mbox("fehler in refreshEllipsen a: " & ex.ToString)
            End Try
        End If
        If AbleitungskreisVorhanden() Then
            'aktpoint.X = ableitungskreis.punktUTM.X
            'aktpoint.Y = ableitungskreis.punktUTM.Y
            'radius = CInt(ableitungskreis.radius * 2)
            'fuellfarbe = Nothing
            'randfarbe = New SolidColorBrush(Color.FromRgb(&HA, &H4B, &HF0))
            'male(aktpoint, radius, dierange, kreiscanvas, bmp, fuellfarbe, randfarbe)
        End If
    End Sub

    Private Shared Function keineRaumbezugeVorhanden() As Boolean
        Return myGlobalz.sitzung.raumbezugsRec.dt Is Nothing OrElse
                    myGlobalz.sitzung.raumbezugsRec.dt.IsNothingOrEmpty
    End Function

    Sub male(ByVal aktpoint As myPoint,
                         ByRef radius As Integer,
                         ByVal dierange As clsRange,
                         ByVal kreiscanvas As clsCanvas,
                         ByVal bmp As WriteableBitmap,
                         ByVal fuellFarbe As SolidColorBrush,
                         ByVal randFarbe As SolidColorBrush)
        Dim neupoint As New myPoint
        Try
            If radius < 1 Then radius = 1
            'radius wurde nicht benutzt, dafür wurde einfach 5 gesetzt
            neupoint = clsMiniMapTools.punktvonGKnachCanvasUmrechnen(aktpoint, dierange, kreiscanvas)
            setzeEllipseinBit(neupoint.X, neupoint.Y, bmp, radius, fuellFarbe, randFarbe)
        Catch ex As Exception
            nachricht("Fehler in clsberichte male: " & ex.ToString)
        End Try
    End Sub

    Private Sub setzeEllipseinBit(ByVal x As Double,
                                    ByVal y As Double,
                                    ByRef bmp2 As WriteableBitmap,
                                    ByVal radius%,
                                    ByVal fuellFarbe As SolidColorBrush,
                                    ByVal randFarbe As SolidColorBrush)
        Dim targetrect As Int32Rect
        Dim newbmp As RenderTargetBitmap
        Dim dv As DrawingVisual
        Dim pennn As System.Windows.Media.Pen
        pennn = GetPennn(fuellFarbe, randFarbe)
        Try
            'korrigieren
            targetrect = New Int32Rect(CInt(x - radius), CInt(y - radius), radius * 2, radius * 2)
            newbmp = New RenderTargetBitmap(radius * 2, radius * 2, 96, 96, PixelFormats.Pbgra32)
            dv = New DrawingVisual
            Dim dc = dv.RenderOpen()
            Dim ibr As New ImageBrush(New CroppedBitmap(bmp2, targetrect))
            dc.DrawRectangle(ibr, Nothing, New Rect(0, 0, radius * 2, radius * 2))
            dc.DrawEllipse(fuellFarbe, pennn, New Point(radius, radius), radius, radius)
            dc.Close()
            newbmp.Render(dv)
            'neue Bitmap in die alte Bitmap kopieren
            Dim pixeldata(CInt((radius * 2) ^ 2 * 4)) As Byte
            newbmp.CopyPixels(pixeldata, radius * 2 * 4, 0)
            bmp2.WritePixels(targetrect, pixeldata, radius * 2 * 4, 0)
        Catch ex As Exception
            nachricht("Fehler in setzeEllipseinBit: " & ex.ToString)
        End Try
    End Sub

    Private Shared Function GetPennn(ByVal fuellFarbe As SolidColorBrush, ByVal randFarbe As SolidColorBrush) As System.Windows.Media.Pen
        Dim pennn As Pen
        If randFarbe Is Nothing Then
            pennn = New System.Windows.Media.Pen(fuellFarbe, 1)
        Else
            pennn = New System.Windows.Media.Pen(randFarbe, 1)
        End If
        Return pennn
    End Function

    Public Sub Bitmapspeichern(ByVal Filename As String, ByVal bmp2 As WriteableBitmap)
        Dim fswrite As New IO.FileStream(Filename, IO.FileMode.Create)
        Dim tif As New JpegBitmapEncoder
        tif.Frames.Add(BitmapFrame.Create(bmp2))
        tif.Save(fswrite)
        fswrite.Dispose()
        tif = Nothing
    End Sub

    Public Sub dokumenteDias(ByVal flw2 As FlowDocument,
                             ByVal paraHeader As System.Windows.Documents.Paragraph,
                             dliste As List(Of clsPresDokumente))
        'dokus auschecken
        myGlobalz.Arc.AllesAuscheckenVorgangOBJ(True, True, dliste)
        Dim zielVerzeichnis As String = clstart.mycsimple.getParadigma_checkout() & "" & myGlobalz.sitzung.aktVorgangsID
        Dim ziel As String = ""
        '   For Each dok As DataRow In myGlobalz.Arc.ArcRec.dt.AsEnumerable
        For Each dok As clsPresDokumente In dliste
            If Not dok.ausgewaehlt Then Continue For
            If Not dok.getDokTyp = DokumentenTyp.JPG Then Continue For
            Try
                ziel = String.Format("{0}\{1}", zielVerzeichnis, dok.DateinameMitExtension)
                nachricht("vor fziel: ")
                Dim FIziel As New IO.FileInfo(ziel)
                If Not FIziel.Exists Then
                    nachricht("Zieldatei existiert nicht: " & ziel)
                End If
                If FIziel.Exists Then
                    Try
                        diaanlegen(ziel, flw2, paraHeader, dok.DateinameMitExtension & "," & dok.ExifDatum.ToString & "," &
                                   dok.Beschreibung)
                    Catch ex As Exception
                        nachricht("Problem beim Kopieren von:  " & dok.DateinameMitExtension & "," &
                                  dok.ExifDatum.ToString & "," & dok.Beschreibung)
                    End Try
                Else
                    nachricht("Datei existiert schon!" & ziel)
                End If
            Catch ex As Exception
                nachricht_und_Mbox("2Problem beim Auschecken von:  " & ziel & vbCrLf & ex.ToString)
            End Try
        Next
    End Sub

    Sub diaanlegen(ByVal fotodatei As String,
                   ByVal flw2 As FlowDocument,
                   ByVal paraHeader As System.Windows.Documents.Paragraph,
                   ByVal Titel As String)
        Dim image As New Image
        image.Width = 200
        image.Height = 400
        image.Stretch = Stretch.UniformToFill

        Dim bimg As BitmapImage = New BitmapImage()
        bimg.BeginInit()
        bimg.UriSource = New Uri(fotodatei, UriKind.Absolute)
        bimg.DecodePixelWidth = 600

        bimg.EndInit()
        image.Source = bimg
        flw2.Blocks.Add(New BlockUIContainer(image))

        paraHeader = New Paragraph()
        paraHeader.FontSize = 8
        paraHeader.FontWeight = FontWeights.Normal
        paraHeader.Inlines.Add(New Run((Titel)))
        flw2.Blocks.Add(paraHeader)
        image = Nothing
        bimg = Nothing
    End Sub

    Private Sub trennseite(ByVal flw2 As FlowDocument, ByVal paraHeader As System.Windows.Documents.Paragraph, ByVal ueberschrift As String)
        paraHeader = New Paragraph()
        paraHeader.FontSize = 28
        paraHeader.FontWeight = FontWeights.Bold
        paraHeader.Inlines.Add(New Run(ueberschrift))
        '   paraHeader.Inlines.Add(New newPage())
        flw2.Blocks.Add(paraHeader)
        flw2.Blocks.Add(paraHeader)
        flw2.Blocks.Add(paraHeader)
    End Sub

    Private Function AbleitungskreisVorhanden() As Boolean
        If ableitungskreis Is Nothing Then
            Return False
        End If
        If ableitungskreis.punktUTM.X > 10 Then
            Return True
        Else
            Return False
        End If
    End Function

    'Private Sub malepolygon(clsParapolygon As clsParapolygon, radius As Integer, dierange As clsRange,
    '                        kreiscanvas As clsCanvas,
    '                        bmp As WriteableBitmap,
    '                        fuellfarbe As SolidColorBrush,
    '                        randfarbe As SolidColorBrush)
    '    'punkte konvertieren
    '    Dim punktarrayGK(), neupointsCanvas() As myPoint
    '    Dim myPointCollection As New PointCollection

    '    punktarrayGK = clsMiniMapTools.zerlegeInPunkte(clsParapolygon.GKstring)
    '    If punktarrayGK IsNot Nothing Then
    '        neupointsCanvas = clsMiniMapTools.polygonNachCanvasUmrechnen(punktarrayGK, dierange, kreiscanvas)

    '        Dim x, y As Double
    '        For i = 0 To neupointsCanvas.GetUpperBound(0)
    '            x = CInt(neupointsCanvas(i).X)
    '            y = CInt(neupointsCanvas(i).Y)
    '            myPointCollection.Add(New Point(x, y))
    '        Next
    '        setzePolygoninBit(myPointCollection, bmp, radius, fuellfarbe, randfarbe)
    '        '   drawPolygon2Canvas(tag, name, zaehler, myPointCollection, myCanvas, zindex)
    '        punktarrayGK = Nothing
    '        neupointsCanvas = Nothing
    '    End If
    'End Sub

    'Private Sub setzePolygoninBit(myPointCollection As PointCollection,
    '                              bmp2 As WriteableBitmap, radius As Integer, fuellfarbe As SolidColorBrush,
    '                              randfarbe As SolidColorBrush)
    '    ' Dim targetrect As Int32Rect
    '    Dim newbmp As RenderTargetBitmap
    '    Dim dv As DrawingVisual
    '    Dim pennn As System.Windows.Media.Pen
    '    Dim myPolygon As New Polygon
    '    Dim myBrush As SolidColorBrush = New SolidColorBrush(Color.FromArgb(20, 0, 100, 250)) 'transparenz ist der erste wert
    '    pennn = GetPennn(fuellfarbe, randfarbe)
    '    Try
    '        'korrigieren
    '        '  targetrect = New Int32Rect(CInt(x - radius), CInt(y - radius), radius * 2, radius * 2)
    '        Dim geomentry As New StreamGeometry
    '        geomentry.FillRule = FillRule.EvenOdd

    '        Using ctx As StreamGeometryContext = Geometry.Open()
    '            ctx.BeginFigure(New Point(10, 100), True, True)
    '            ctx.PolyLineTo(myPointCollection, True, True)
    '        End Using

    '        newbmp = New RenderTargetBitmap(radius * 2, radius * 2, 96, 96, PixelFormats.Pbgra32)
    '        dv = New DrawingVisual
    '        Dim dc = dv.RenderOpen()
    '        'Dim ibr As New ImageBrush(New CroppedBitmap(bmp2, targetrect))
    '        'dc.DrawRectangle(ibr, Nothing, New Rect(0, 0, radius * 2, radius * 2))
    '        dc.DrawGeometry(myBrush, pennn, geomentry)
    '        ' dc.DrawEllipse(fuellfarbe, pennn, New Point(radius, radius), radius, radius)
    '        dc.Close()
    '        newbmp.Render(dv)
    '        'neue Bitmap in die alte Bitmap kopieren
    '        Dim pixeldata(CInt((radius * 2) ^ 2 * 4)) As Byte
    '        newbmp.CopyPixels(pixeldata, radius * 2 * 4, 0)
    '        ' bmp2.WritePixels(targetrect, pixeldata, radius * 2 * 4, 0)
    '    Catch ex As Exception
    '        nachricht("Fehler in setzeEllipseinBit: " & ex.ToString)
    '    End Try
    'End Sub


End Class
