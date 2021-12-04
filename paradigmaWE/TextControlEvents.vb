'-------------------------------------------------------------------------------------------------------------
' module:     TX Text Control Words
'
' copyright:  © Text Control GmbH
' author:     T. Kummerow
'-----------------------------------------------------------------------------------------------------------

Imports System
Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.Reflection
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports TXTextControl

'----------------------------------------------------------------------------------------------------------
' TextControl event handlers
'--------------------------------------------------------------------------------------------------------

Partial Public Class MainWindow
    Public Property nostart As Boolean = False

#Region "  Event Handlers  "

    Private Sub TextControl_Loaded(sender As Object, e As RoutedEventArgs)
        m_textControl.Focus()

        ' Remove page shadow
        m_textControl.DisplayColors.DarkShadowColor = m_textControl.DisplayColors.DesktopColor
        m_textControl.DisplayColors.LightShadowColor = m_textControl.DisplayColors.DesktopColor

        ' For unknown reasons m_fileHandler.IsDocumentDirty is true at this point
        m_fileHandler.IsDocumentDirty = False

        ' Load file provided as a command line argument
        Dim args As String() = Environment.GetCommandLineArgs()
        'MsgBox("args(2)" & args(2))
        '1 = dateiname
        '2 = extension
        '3 = docid
        '4 = vid
        '5 = Titel
        If args.Length > 1 Then
            m_fileHandler.jfDateityp = args(2)
            m_fileHandler.jfDokumentId = args(3)
            m_fileHandler.jfVorgangsid = args(4)
            m_fileHandler.jfVorgangsTitel = args(5)
            'MsgBox(args(1) & Environment.NewLine &
            '       m_fileHandler.jfDateityp & Environment.NewLine &
            '       m_fileHandler.jfDokumentId & Environment.NewLine &
            '       m_fileHandler.jfVorgangsid & Environment.NewLine &
            '       m_fileHandler.jfVorgangsTitel & Environment.NewLine &
            '       m_fileHandler.jfLocalLocksRoot)
            If isDateiSchonGeoeffnet(m_fileHandler.jfDokumentId, m_fileHandler.jfLocalLocksRoot) Then
                m_fileHandler.jfDokumentId = 0
                MsgBox("Die Datei ist schon geöffnet. Wir schließen Sie diese Instanz wieder, sonst droht Datenverlust!")
                nostart = True
            Else
                lockDateianlegen(m_fileHandler.jfDokumentId, m_fileHandler.jfLocalLocksRoot)
                m_fileHandler.Open(args(1))
                nostart = False
            End If

        End If
    End Sub

    Private Sub lockDateianlegen(jfDokumentId As Integer, jfLocalLocksRoot As String)
        Dim datei As String
        Dim fi As IO.FileInfo
        Try
            datei = jfLocalLocksRoot & "\" & jfDokumentId
            fi = New IO.FileInfo(datei)
            'If fi.Exists Then
            '    fi.Delete()
            '    fi = Nothing
            '    Exit Sub
            'End If


            Dim fs As FileStream = fi.Create()
            ' Modify the file as required, and then close the file.
            fs.Close()
            fs.Dispose()

            fi = Nothing
        Catch ex As Exception
            MsgBox("fehler Integer isDateiSchonGeoeffnet: " & ex.ToString)
        End Try
    End Sub

    Private Function isDateiSchonGeoeffnet(jfDokumentId As Integer, jfLocalLocksRoot As String) As Boolean
        Dim datei As String
        Dim fi As IO.FileInfo
        Try
            datei = jfLocalLocksRoot & "\" & jfDokumentId
            fi = New IO.FileInfo(datei)
            If fi.Exists Then
                fi = Nothing
                Return True
            Else
                fi = Nothing
                Return False
            End If
        Catch ex As Exception
            MsgBox("fehler Integer isDateiSchonGeoeffnet: " & ex.ToString)
        End Try
    End Function

    Private Sub TextControl_InputPositionChanged(sender As Object, e As EventArgs)
        m_ctgTableTools.Visibility = If(m_textControl.Tables.GetItem() IsNot Nothing, Visibility.Visible, Visibility.Collapsed)
    End Sub

    Private Sub TextControl_Changed(sender As Object, e As EventArgs)
        m_fileHandler.IsDocumentDirty = True
    End Sub

    Private Sub TextControl_TextContextMenuOpening(sender As Object, e As TXTextControl.WPF.TextContextMenuEventArgs)
        ' AddFrameContextMenuItems(e.TextContextMenu);

        ' ToDo: implement

        If (e.ContextMenuLocation And ContextMenuLocation.SelectedFrame) <> 0 Then
        End If
        If (e.ContextMenuLocation And ContextMenuLocation.TextField) <> 0 Then
            Dim field = m_textControl.ApplicationFields.GetItem()
            If field Is Nothing Then
                Return
            End If

            Dim cntMnu As ContextMenu = DirectCast(Resources("ContextMenuApplicationFields"), ContextMenu)
            cntMnu.IsOpen = True

            e.Cancel = True
        End If
    End Sub

    '-------------------------------------------------------------------------------------------------------
    ' Checks whether a frame was selected or deselected and changes the visibility of the frame layout tab
    ' accordingly.
    '-----------------------------------------------------------------------------------------------------

    Private Sub TextControl_FrameSelected(sender As Object, e As TXTextControl.FrameEventArgs)
        m_ctgFrameTools.Visibility = Visibility.Visible
    End Sub

    Private Sub TextControl_DrawingActivated(sender As Object, e As TXTextControl.DataVisualization.DrawingEventArgs)
        m_ctgFrameTools.Visibility = Visibility.Visible
    End Sub

    Private Sub TextControl_FrameDeselected(sender As Object, e As TXTextControl.FrameEventArgs)
        If (m_textControl.Frames.GetItem() Is Nothing) AndAlso (m_textControl.Drawings.GetActivatedItem() Is Nothing) Then
            m_ctgFrameTools.Visibility = Visibility.Collapsed
        End If
    End Sub

    Private Sub TextControl_DrawingDeselected(sender As Object, e As TXTextControl.DataVisualization.DrawingEventArgs)
        If (m_textControl.Frames.GetItem() Is Nothing) AndAlso (m_textControl.Drawings.GetActivatedItem() Is Nothing) Then
            m_ctgFrameTools.Visibility = Visibility.Collapsed
        End If
    End Sub

    '-------------------------------------------------------------------------------------------------------
    ' Keyboard shortcuts
    '-----------------------------------------------------------------------------------------------------

    Private Sub TextControl_KeyDown(sender As Object, e As KeyEventArgs)
        Select Case e.Key
            Case Key.Insert
                ' Toggle insertion mode					
                If Keyboard.Modifiers <> 0 Then
                    Exit Select
                End If
                ToggleInsertionMode()
                Exit Select
        End Select
    End Sub

    Private Sub TextControl_Drop(sender As Object, e As DragEventArgs)
        If m_dragDropHandler.CanDrop Then
            Select Case m_dragDropHandler.FileType
                Case FileDragDropHandler.DraggedFileType.Document
                    OpenDroppedDocument()
                    Exit Select

                Case FileDragDropHandler.DraggedFileType.Image
                    InsertDroppedImage(e)
                    Exit Select
            End Select
        End If
    End Sub

    Private Sub TextControl_DragEnter(sender As Object, e As DragEventArgs)
        m_dragDropHandler.Reset()
        m_dragDropHandler.CheckDraggedFiles(DirectCast(e.Data.GetData(DataFormats.FileDrop), String()))
    End Sub

    Private Sub TextControl_DragOver(sender As Object, e As DragEventArgs)
        If m_dragDropHandler.CanDrop Then
            e.Effects = m_dragDropHandler.GetDragDropEffect(e.AllowedEffects)
        End If
    End Sub

    Private Sub SelectAllHandler(sender As [Object], e As ExecutedRoutedEventArgs)
        m_textControl.SelectAll()
    End Sub

    Private Sub FindHandler(sender As [Object], e As ExecutedRoutedEventArgs)
        m_textControl.Find()
    End Sub

    '-------------------------------------------------------------------------------------------------------
    ' Links
    '-----------------------------------------------------------------------------------------------------

    Private Sub TextControl_DocumentLinkClicked(sender As Object, e As TXTextControl.DocumentLinkEventArgs)
        If e.DocumentLink.DocumentTarget Is Nothing Then
            Return
        End If
        e.DocumentLink.DocumentTarget.ScrollTo()
    End Sub

    Private Sub TextControl_HypertextLinkClicked(sender As Object, e As TXTextControl.HypertextLinkEventArgs)
        OpenHyperlink(e.HypertextLink.Target)
    End Sub

#End Region


#Region "  Helpers  "

    Private Sub ToggleInsertionMode()
        m_textControl.InsertionMode = If(m_textControl.InsertionMode = TXTextControl.InsertionMode.Insert, TXTextControl.InsertionMode.Overwrite, TXTextControl.InsertionMode.Insert)
    End Sub

    '-------------------------------------------------------------------------------------------------------
    ' Opens a link to an extrernal file either in a new TX Words instance or using the program which
    ' is registered for the file type.
    '-----------------------------------------------------------------------------------------------------

    Private Sub OpenHyperlink(strTarget As String)
        If strTarget = "" Then
            Return
        End If

        Try
            Dim uriTarget As New Uri(strTarget, UriKind.RelativeOrAbsolute)
            If Not uriTarget.IsAbsoluteUri Then
                Throw New Exception(My.Resources.EXC_ONLY_ABS_PATH_SUPORTED)
            End If

            If uriTarget.IsFile Then
                ' Remove any fragment.
                ' uriTarget.GetLeftPart(UriPartial.Path) has no effect because the .NET Uri class
                ' does not work correct with file URIs containing any query or fragment part.

                strTarget = uriTarget.LocalPath
                Dim nPos As Integer = strTarget.IndexOf("#")
                If nPos <> -1 Then
                    strTarget = strTarget.Substring(0, nPos)
                End If
            ElseIf uriTarget.Scheme <> Uri.UriSchemeHttp AndAlso uriTarget.Scheme <> Uri.UriSchemeHttps Then
                strTarget = uriTarget.GetLeftPart(UriPartial.Path)
            End If

            If uriTarget.IsFile AndAlso IsMyFile(strTarget) Then
                OpenFileInNewInstance(strTarget)
            Else
                System.Diagnostics.Process.Start(strTarget)
            End If
        Catch ex As Exception
            Dim msg As String = ex.Message
            If Not msg.EndsWith(".") Then
                msg += "."
            End If
            MessageBox.Show(My.Resources.MSG_COULD_NOT_OPEN_LINK + " " & msg, "Hyperlink", MessageBoxButton.OK, MessageBoxImage.[Error])
        End Try
    End Sub

    Private Sub OpenFileInNewInstance(strTarget As String)
        ' Check if file exists and show message box if not
        If Not File.Exists(strTarget) Then
            MessageBox.Show(String.Format(My.Resources.MSG_FILE_DOES_NOT_EXIST, strTarget), "Hyperlink", MessageBoxButton.OK, MessageBoxImage.[Error])
            Return
        End If

        ' Get running demo's exe path
        Dim exePath As String = Assembly.GetEntryAssembly().Location

        ' Start new demo instance
        Dim process = New Process()
        process.StartInfo.FileName = exePath
        process.StartInfo.Arguments = """" & strTarget & """"
        process.Start()
    End Sub

    '-------------------------------------------------------------------------------------------------------
    ' Checks if file type is rtf, doc, docx or tx
    '-----------------------------------------------------------------------------------------------------

    Private Function IsMyFile(strTarget As String) As Boolean
        Dim strExt As String = Path.GetExtension(strTarget).ToLower()
        'MsgBox("4 " & strExt & ", " & strTarget)
        Select Case strExt
            Case ".rtf", ".doc", ".docx", ".tx"
                Return True
        End Select

        Return False
    End Function

    Private Sub OpenDroppedDocument()
        m_fileHandler.Open(m_dragDropHandler.FileName)
    End Sub

    Private Sub InsertDroppedImage(e As DragEventArgs)
        Try
            ' Get pixel position of mouse cursor inside Text Control
            Dim posCursor As System.Windows.Point = e.GetPosition(m_textControl)
            Dim pos As New System.Drawing.Point(CInt(posCursor.X), CInt(posCursor.Y))

            ' Get bounding rectangle of the first character of the paragraph
            ' the image was dropped over
            Dim par As TXTextControl.Paragraph = m_textControl.Paragraphs.GetItem(pos)
            Dim charParStart As TXTextControl.TextChar = m_textControl.TextChars(par.Start)
            Dim rPar As Rectangle = If((charParStart IsNot Nothing), charParStart.Bounds, New Rectangle())

            ' Get bounding rectangle of the character the image was dropped over
            Dim txChar As TXTextControl.TextChar = m_textControl.TextChars.GetItem(pos, True)
            Dim rChar As Rectangle = If((txChar IsNot Nothing), txChar.Bounds, New Rectangle())

            ' Calculate image position relative to paragraph position
            Dim posImg = New System.Drawing.Point(rChar.Left - rPar.Left + rChar.Width, rChar.Top - rPar.Top)

            ' Insert image anchored to paragraph
            Dim txImg = New TXTextControl.Image() With {
                .FileName = m_dragDropHandler.FileName
            }
            m_textControl.Images.Add(txImg, posImg, par.Start, TXTextControl.ImageInsertionMode.DisplaceText)
        Catch exc As Exception
            MessageBox.Show(exc.Message, ProductName, MessageBoxButton.OK, MessageBoxImage.[Error])
        End Try
    End Sub

#End Region
End Class
