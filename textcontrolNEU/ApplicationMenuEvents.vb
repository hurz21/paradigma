Imports Microsoft.Windows.Controls.Ribbon
Imports System
Imports System.Collections.Specialized
Imports System.Diagnostics
Imports System.IO
Imports System.Printing
Imports System.Reflection
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports TXTextControl

'-------------------------------------------------------------------------------------------------------
'	** Event handlers for the application menu.
'	**-----------------------------------------------------------------------------------------------------

Partial Public Class MainWindow

	Private Sub FileNewHandler(sender As Object, e As ExecutedRoutedEventArgs)
		m_fileHandler.[New]()
	End Sub

	Private Sub CloseHandler(sender As Object, e As ExecutedRoutedEventArgs)
		Me.Close()
	End Sub

	Public Sub CanSave(sender As Object, e As CanExecuteRoutedEventArgs)
		If m_textControl Is Nothing Then
			Return
		End If
		e.CanExecute = m_fileHandler.IsDocumentDirty
	End Sub

	Private Sub FileOpenHandler(sender As Object, e As ExecutedRoutedEventArgs)
		m_fileHandler.Open()
	End Sub

	Private Sub OpenSampleTemplateCommand_Executed(sender As Object, e As ExecutedRoutedEventArgs)
		Dim fileName As String = ""
		Dim tmplType As SampleTemplateType
		Dim exePath As String = Assembly.GetEntryAssembly().Location
		Dim dir As String = Path.GetDirectoryName(exePath)

		Try
			tmplType = DirectCast(e.Parameter, SampleTemplateType)
		Catch
			Return
		End Try

		Select Case tmplType
			Case SampleTemplateType.Invoice
				fileName = dir & "\..\invoice.docx"
				Exit Select

			Case SampleTemplateType.PackingList
				fileName = dir & "\..\shippinglabel.docx"
				Exit Select

			Case SampleTemplateType.ShippingLabel
				fileName = dir & "\..\packinglist.docx"
				Exit Select
		End Select
		If String.IsNullOrEmpty(fileName) Then
			Return
		End If

		' Start new demo instance
		Dim process = New Process()
		process.StartInfo.FileName = exePath
		process.StartInfo.Arguments = """" & fileName & """"
		process.Start()
	End Sub

	Public Sub TextControlExists(sender As Object, e As CanExecuteRoutedEventArgs)
		e.CanExecute = (m_textControl IsNot Nothing)
	End Sub

	Private m_bInSaveAsHandler As Boolean = False
	Private Sub FileSaveAsHandler(sender As Object, e As ExecutedRoutedEventArgs)
		MessageBox.Show("im saveas handler")

		Dim streamType As TXTextControl.StreamType = TXTextControl.StreamType.RichTextFormat

		If m_bInSaveAsHandler Then
			Return
		End If
		m_bInSaveAsHandler = True

		Dim param As String = TryCast(e.Parameter, String)
		If param Is Nothing Then
			m_fileHandler.SaveAs()
			m_bInSaveAsHandler = False
			Return
		End If

		Select Case param
			Case "doc"
				streamType = TXTextControl.StreamType.MSWord
				Exit Select

			Case "docx"
				streamType = TXTextControl.StreamType.WordprocessingML
				Exit Select

			Case "rtf"
				streamType = TXTextControl.StreamType.RichTextFormat
				Exit Select

			Case "pdf"
				streamType = TXTextControl.StreamType.AdobePDF
				Exit Select

			Case "html"
				streamType = TXTextControl.StreamType.HTMLFormat
				Exit Select
		End Select

		m_fileHandler.SaveAs(streamType)
		m_bInSaveAsHandler = False
	End Sub

	Private Sub FileSaveHandler(sender As Object, e As ExecutedRoutedEventArgs)
		m_fileHandler.Save()
	End Sub

	' Unfortunately this boolean variable is necessary due to a
	' bug in RibbonApplicationSplitMenuItem leading to the associated command being called twice.
	Private m_bInPrintHandler As Boolean = False

	Private Sub PrintHandler(sender As Object, e As ExecutedRoutedEventArgs)
		If m_bInPrintHandler Then
			Return
		End If
		' Check document permissions and abort if necessary
		If (m_fileHandler.DocumentAccessPermissions And (DocumentAccessPermissions.AllowHighLevelPrinting Or DocumentAccessPermissions.AllowLowLevelPrinting)) = 0 Then
			Return
		End If
		m_bInPrintHandler = True
		m_textControl.Print((ProductName & " – ") + m_fileHandler.DocumentTitle, True)
		m_bInPrintHandler = False
	End Sub


	Private Sub QuickPrintHandler(sender As Object, e As ExecutedRoutedEventArgs)
		m_textControl.Print((ProductName & " – ") + m_fileHandler.DocumentTitle, New PageRange(1, m_textControl.Pages), 1, Collation.Collated)
	End Sub

	Private Sub OptionsHandler(sender As Object, e As System.Windows.Input.ExecutedRoutedEventArgs)
		Dim dlg = New OptionsDialog(m_textControl, m_fileHandler)
		dlg.FlowDirection = Me.FlowDirection
		dlg.Owner = Me
		dlg.ShowDialog()
	End Sub

	Private Sub RecentFiles_SelectionChanged(sender As Object, e As RoutedPropertyChangedEventArgs(Of Object))
		Dim itm = DirectCast(e.NewValue, RibbonGalleryItem)
		Dim path As String = DirectCast(itm.Tag, String)
		m_fileHandler.OpenRecentFile(path)
	End Sub
End Class
