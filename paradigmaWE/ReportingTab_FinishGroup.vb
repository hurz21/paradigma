'-------------------------------------------------------------------------------------------------------------
' module:        TX Text Control Words
'
' copyright:     © Text Control GmbH
' author:        T. Kummerow,  F. Zenker
'-----------------------------------------------------------------------------------------------------------

Imports Microsoft.Windows.Controls.Ribbon
Imports System
Imports System.Collections.Generic
Imports System.Drawing.Printing
Imports System.Printing
Imports System.Threading
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input
Imports TX_Text_Control_Words.Win32
Imports TXTextControl
Imports Microsoft.Win32
Imports System.Runtime.CompilerServices

Partial Class MainWindow

#Region "  Private fields  "

	Private m_dlgMergeWait As MergeWaitDialog

	Private m_grpFinishMerge As RibbonGroup
	Private m_btnPreview As RibbonButton
	Private m_btnFinishMerge As RibbonSplitButton

	Private m_textControlContent As Byte() = Nothing
	Private m_editMode As EditMode = EditMode.Edit

#End Region

#Region "  Finish Group  "

	'--------------------------------------------------------------------------------------------
	' AddFinishGroup
	' Get the RibbonGroup 'Finish & Merge' from the resources and add this group to the end
	' of the RibbonReportingTab instance.
	'------------------------------------------------------------------------------------------

	Private Sub AddFinishGroup()
		m_grpFinishMerge = DirectCast(Me.Resources("RibbonGroupFinish"), RibbonGroup)
		m_btnPreview = DirectCast(m_grpFinishMerge.Items(0), RibbonButton)
		m_btnFinishMerge = DirectCast(m_grpFinishMerge.Items(1), RibbonSplitButton)
		m_reportingTab.Items.Add(m_grpFinishMerge)
		AddHandler m_reportingTab.DataSourceManager.IsMergingPossibleChanged, AddressOf DataSourceManager_IsMergingPossibleChanged
	End Sub

#Region "  Preview Button  "

	'--------------------------------------------------------------------------------------------
	' PreviewCommand_Executed
	' Opens the 'Limit Preview Data' dialog, shows the contextual 'Preview' tab and loads the
	' first record into the TextControl.
	'------------------------------------------------------------------------------------------

	Private Sub PreviewCommand_Executed(sender As Object, e As ExecutedRoutedEventArgs)
		m_nPreviewCount = MaxPrevRowsDialog.Show(Me)
		If m_nPreviewCount > 0 Then

			m_textControl.Save(m_textControlContent, BinaryStreamType.InternalUnicodeFormat)
			SetLastSelectedMasterTable()

			m_lstMergedFiles = m_reportingTab.DataSourceManager.Merge(m_textControlContent, m_nPreviewCount, m_textControl)
			If m_lstMergedFiles.Count > 0 Then
				m_iPreviewIndex = 0
				m_nPreviewCount = Math.Min(m_nPreviewCount, m_lstMergedFiles.Count)
				m_textControl.Load(m_lstMergedFiles(0), BinaryStreamType.InternalUnicodeFormat)
				m_ctgReportingPreview.Visibility = System.Windows.Visibility.Visible
				m_ribbon.SelectedItem = m_previewTab
				m_editMode = m_textControl.EditMode
				m_textControl.EditMode = EditMode.[ReadOnly]
				m_btnPreview.IsEnabled = False
				UpdateNavigateButtons()
			Else
				m_lstMergedFiles = Nothing
			End If
		End If
	End Sub

#End Region

#Region "  Finish Button  "

	'--------------------------------------------------------------------------------------------
	' DataSourceManager_IsMergingPossibleChanged
	' Enables or disables the 'Finish & Merge' group regarding the 
	' DataSourceManager.IsMergingPossible property value.
	'------------------------------------------------------------------------------------------

	Private Sub DataSourceManager_IsMergingPossibleChanged(sender As Object, e As EventArgs)
		m_grpFinishMerge.IsEnabled = m_reportingTab.DataSourceManager.IsMergingPossible
	End Sub

#Region " Merge into Current Document "

	'--------------------------------------------------------------------------------------------
	' MergeIntoCurrentDocumentCommand_Executed
	' Merges the data into the current document (TextControl).
	'------------------------------------------------------------------------------------------

	Private Sub MergeIntoCurrentDocumentCommand_Executed(sender As Object, e As ExecutedRoutedEventArgs)
		MergeIntoCurrentDocAsync()
	End Sub

#End Region

#Region " Merge into Single File "

	'--------------------------------------------------------------------------------------------
	' MergeIntoSingleFileCommand_Executed
	' On clicking one of the 'Merge into Single File' menu button's items the data is merged 
	' into and and saved as a single file 
	'------------------------------------------------------------------------------------------

	Private Sub MergeIntoSingleFileCommand_Executed(sender As Object, e As ExecutedRoutedEventArgs)
		Dim fileExt As String = TryCast(e.Parameter, String)
		If fileExt IsNot Nothing Then
			Dim streamType As StreamType = fileExt.ToTxStreamType()
			MergeIntoSingleFileAsync(streamType)
		End If
	End Sub

#End Region

#Region " Merge into Multiple Files "

	'--------------------------------------------------------------------------------------------
	' MergeIntoSeparateFilesCommand_Executed
	' On clicking one of the Merge into Multiple Files' menu button's items the data is merged 
	' into and and saved as separate files.
	'------------------------------------------------------------------------------------------

	Private Sub MergeIntoSeparateFilesCommand_Executed(sender As Object, e As ExecutedRoutedEventArgs)
		Dim fileExt As String = TryCast(e.Parameter, String)
		If fileExt IsNot Nothing Then
			Dim streamType As StreamType = fileExt.ToTxStreamType()
			MergeIntoSeparateFilesAsync(streamType)
		End If
	End Sub

#End Region

#Region " Print Document "

	'--------------------------------------------------------------------------------------------
	' MergePrintCommand_Executed
	' Merges the data and prints the result.
	'------------------------------------------------------------------------------------------

	Private Sub MergePrintCommand_Executed(sender As Object, e As ExecutedRoutedEventArgs)
		MergePrintAsync()
	End Sub

#End Region

#End Region

#Region "  Document Processing  "

	'-----------------------------------------------------------------------------------------
	' Merge into current document:
	'---------------------------------------------------------------------------------------


	Private Sub MergeIntoCurrentDocAsync()
		If Not m_fileHandler.HandleUnsavedChanges() Then
			Return
		End If
		ThreadPool.QueueUserWorkItem(AddressOf MergeIntoCurrentDocCallback)
	End Sub

	Private Sub MergeIntoCurrentDocCallback(state As Object)
		Me.Dispatcher.BeginInvoke(DirectCast(AddressOf ShowMergeWaitDialog, Action))
		MergeIntoCurrentDocument()
		Me.Dispatcher.BeginInvoke(DirectCast(AddressOf CloseMergeWaitDialog, Action))
	End Sub

	Private Sub MergeIntoCurrentDocument()
		Dim dm = m_reportingTab.DataSourceManager
		Dim document As Byte() = Nothing
		m_textControl.Save(document, BinaryStreamType.InternalUnicodeFormat)
		Dim mergeResult As IList(Of Byte()) = dm.Merge(document, m_textControl)

		If mergeResult.Count = 0 Then
			MessageBox.Show(My.Resources.MERGE_NO_MERGE_RESULTS, ProductName, MessageBoxButton.OK, MessageBoxImage.Information)
			Return
		End If
		Me.Dispatcher.Invoke(DirectCast(AddressOf m_textControl.ResetContents, Action))
		For Each doc As Byte() In mergeResult
			m_textControl.Append(doc, BinaryStreamType.InternalUnicodeFormat, AppendSettings.StartWithNewSection)
		Next
	End Sub

	'-----------------------------------------------------------------------------------------
	' Merge into separate files:
	'---------------------------------------------------------------------------------------


	' Small internal class used to pass information to one of the async callback methods.
	Private NotInheritable Class MergeIntoSeparateFilesInfo
		Public Sub New(streamType As TXTextControl.StreamType, dirName As String)
			Me.StreamType = streamType
			Me.DirectoryName = dirName
		End Sub
		Public Property StreamType() As StreamType
			Get
				Return m_StreamType
			End Get
			Private Set(value As StreamType)
				m_StreamType = Value
			End Set
		End Property
		Private m_StreamType As StreamType
		Public Property DirectoryName() As String
			Get
				Return m_DirectoryName
			End Get
			Private Set(value As String)
				m_DirectoryName = Value
			End Set
		End Property
		Private m_DirectoryName As String
	End Class

	Private Sub MergeIntoSeparateFilesAsync(streamType As StreamType)
		Dim strFolder As String = FolderBrowserDialog.ShowDialog("Browse for Folder", "", Me)
		If String.IsNullOrEmpty(strFolder) Then
			Return
		End If
		Dim mergeInfo = New MergeIntoSeparateFilesInfo(streamType, strFolder)
		ThreadPool.QueueUserWorkItem(AddressOf MergeIntoSeparateFilesCallback, mergeInfo)
	End Sub

	Private Sub MergeIntoSeparateFilesCallback(state As Object)
		Dim mergeInfo = TryCast(state, MergeIntoSeparateFilesInfo)
		If mergeInfo Is Nothing Then
			Return
		End If

		Me.Dispatcher.BeginInvoke(DirectCast(AddressOf ShowMergeWaitDialog, Action))
		MergeIntoSeparateFiles(mergeInfo.StreamType, mergeInfo.DirectoryName)
		Me.Dispatcher.BeginInvoke(DirectCast(AddressOf CloseMergeWaitDialog, Action))
	End Sub

	Private Sub MergeIntoSeparateFiles(streamType As StreamType, dirName As String)
		Dim dm = m_reportingTab.DataSourceManager

		Dim document As Byte() = Nothing
		m_textControl.Save(document, BinaryStreamType.InternalUnicodeFormat)
		Dim mergeResult As IEnumerable(Of Byte()) = dm.Merge(document, m_textControl)

		Using txTmp = New ServerTextControl()
			txTmp.Create()
			Dim nDataRow As Integer = 0
			Dim fileNamePrefix As String = String.Format("\MergedDocument_{0:yy-MM-dd}_{0:HH-mm-ss}_", DateTime.Now), fileExt As String = streamType.ToFileExt()
			For Each doc As Byte() In mergeResult
				txTmp.Load(doc, BinaryStreamType.InternalUnicodeFormat)
				Dim strFileName As String = String.Format("{0}{1}{2}{3}", dirName, fileNamePrefix, String.Format("{0:00000}", System.Math.Max(System.Threading.Interlocked.Increment(nDataRow), nDataRow - 1)), fileExt)
				txTmp.Save(strFileName, streamType)
			Next
		End Using
	End Sub

	'-----------------------------------------------------------------------------------------
	' Merge into single file:
	'---------------------------------------------------------------------------------------


	' Small internal class used to pass information to one of the async callback methods.
	Private NotInheritable Class MergeIntoSingleFileInfo
		Public Sub New(streamType As TXTextControl.StreamType, fileName As String)
			Me.StreamType = streamType
			Me.FileName = fileName
		End Sub
		Public Property StreamType() As StreamType
			Get
				Return m_StreamType
			End Get
			Private Set(value As StreamType)
				m_StreamType = Value
			End Set
		End Property
		Private m_StreamType As StreamType
		Public Property FileName() As String
			Get
				Return m_FileName
			End Get
			Private Set(value As String)
				m_FileName = Value
			End Set
		End Property
		Private m_FileName As String
	End Class

	Private Sub MergeIntoSingleFileAsync(streamType As StreamType)
		Dim dlg = New SaveFileDialog() With { _
			.Filter = streamType.ToFilterString(), _
			.OverwritePrompt = True, _
			.AddExtension = True _
		}
		If dlg.ShowDialog(Me) = True Then
			Dim mergeInfo = New MergeIntoSingleFileInfo(streamType, dlg.FileName)
			ThreadPool.QueueUserWorkItem(AddressOf MergeIntoSingleFileCallback, mergeInfo)
		End If
	End Sub

	Private Sub MergeIntoSingleFileCallback(state As Object)
		Dim mergeInfo = TryCast(state, MergeIntoSingleFileInfo)
		If mergeInfo Is Nothing Then
			Return
		End If

		Me.Dispatcher.BeginInvoke(DirectCast(AddressOf ShowMergeWaitDialog, Action))
		MergeIntoSingleFile(mergeInfo.StreamType, mergeInfo.FileName)
		Me.Dispatcher.BeginInvoke(DirectCast(AddressOf CloseMergeWaitDialog, Action))
	End Sub

	Private Sub MergeIntoSingleFile(streamType As StreamType, fileName As String)
		Dim dm = m_reportingTab.DataSourceManager

		Dim document As Byte() = Nothing
		m_textControl.Save(document, BinaryStreamType.InternalUnicodeFormat)
		Dim mergeResult As IEnumerable(Of Byte()) = dm.Merge(document, m_textControl)

		Using txTmp = New ServerTextControl()
			txTmp.Create()
			For Each doc As Byte() In mergeResult
				txTmp.Append(doc, BinaryStreamType.InternalUnicodeFormat, AppendSettings.StartWithNewSection)
			Next
			txTmp.Save(fileName, streamType)
		End Using
	End Sub

	'-----------------------------------------------------------------------------------------
	' Merge print:
	'---------------------------------------------------------------------------------------


	Private Sub MergePrintAsync()
		Dim printDoc = New PrintDocument()
		Dim printDlg = New PrintDialog() With { _
			.UserPageRangeEnabled = False _
		}
		If printDlg.ShowDialog() = True Then
			Dim bCollate As Boolean = If((printDlg.PrintTicket.Collation IsNot Nothing), printDlg.PrintTicket.Collation = Collation.Collated, False)
			Dim nCopyCount As Short = CShort(If(printDlg.PrintTicket.CopyCount, 1))
			printDoc.DocumentName = "Merged Document"
			printDoc.PrinterSettings = New PrinterSettings() With { _
				.PrinterName = printDlg.PrintQueue.FullName, _
				.Copies = nCopyCount, _
				.Collate = bCollate _
			}
			ThreadPool.QueueUserWorkItem(AddressOf MergePrintCallback, printDoc)
		End If
	End Sub

	Private Sub MergePrintCallback(state As Object)
		Dim printDoc = TryCast(state, PrintDocument)
		If printDoc Is Nothing Then
			Return
		End If
		Me.Dispatcher.BeginInvoke(DirectCast(AddressOf ShowMergeWaitDialog, Action))
		MergePrint(printDoc)
		Me.Dispatcher.BeginInvoke(DirectCast(AddressOf CloseMergeWaitDialog, Action))
	End Sub

	Private Sub MergePrint(printDoc As PrintDocument)
		Dim dm = m_reportingTab.DataSourceManager

		Dim document As Byte() = Nothing
		m_textControl.Save(document, BinaryStreamType.InternalUnicodeFormat)
		Dim mergeResult As IEnumerable(Of Byte()) = dm.Merge(document, m_textControl)
		Using txTmp = New ServerTextControl()
			txTmp.Create()
			For Each doc As Byte() In mergeResult
				txTmp.Append(doc, BinaryStreamType.InternalUnicodeFormat, AppendSettings.StartWithNewSection)
			Next
			txTmp.Print(printDoc)
		End Using
	End Sub

#End Region


#Region "  Helpers  "

	'--------------------------------------------------------------------------------------------
	' ShowMergeWaitDialog
	' Shows the 'Merge Wait Dialog'
	'------------------------------------------------------------------------------------------*/
	Private Sub ShowMergeWaitDialog()
		If m_dlgMergeWait IsNot Nothing Then
			Try
				m_dlgMergeWait.CloseDialog()
			Catch
			End Try
			m_dlgMergeWait = Nothing
		End If
		m_dlgMergeWait = New MergeWaitDialog()
		m_dlgMergeWait.Owner = Me
		m_dlgMergeWait.ShowDialog()
	End Sub

	'--------------------------------------------------------------------------------------------
	' CloseMergeWaitDialog
	' Closes the 'Merge Wait Dialog'
	'------------------------------------------------------------------------------------------*/
	Private Sub CloseMergeWaitDialog()
		If m_dlgMergeWait IsNot Nothing Then
			Try
				m_dlgMergeWait.CloseDialog()
				m_dlgMergeWait = Nothing
			Catch
			End Try
		End If
	End Sub

	'--------------------------------------------------------------------------------------------
	' SetLastSelectedMasterTable
	' Resets the master table to that data table info which is currently represented by the 
	' Select Master Table' menu button's checked drop down item. 
	'------------------------------------------------------------------------------------------

	Private Sub SetLastSelectedMasterTable()
		Dim rmbtnTXITEM_SelectMasterTable As RibbonMenuButton = TryCast(m_reportingTab.FindName(TXTextControl.WPF.RibbonReportingTab.RibbonItem.TXITEM_SelectMasterTable.ToString()), RibbonMenuButton)
		For Each dropDownItem As Object In rmbtnTXITEM_SelectMasterTable.Items
			Dim rglGallery As RibbonGallery = TryCast(dropDownItem, RibbonGallery)
			If rglGallery IsNot Nothing Then
				For Each galleryItem As Object In rglGallery.Items
					Dim rmiTable As RibbonMenuItem = TryCast(galleryItem, RibbonMenuItem)
					If rmiTable IsNot Nothing AndAlso rmiTable.IsChecked Then
						m_reportingTab.DataSourceManager.MasterDataTableInfo = TryCast(rmiTable.Tag, TXTextControl.DocumentServer.DataSources.DataTableInfo)
						Return
					End If
				Next
			End If
		Next
	End Sub
#End Region

#End Region
End Class

#Region "  Extensions  "

'--------------------------------------------------------------------------------------------
' Extensions used in this code file
'------------------------------------------------------------------------------------------

Module Extensions__2
	'-----------------------------------------------------------------------------------------
	' Converts a file extension with or without dot (e. g. ".docx" or "HTML") to a TX
	' stream type if possible.
	'---------------------------------------------------------------------------------------

	<Extension> _
	Public Function ToTxStreamType(fileExt As String) As StreamType
		If fileExt.StartsWith(".") Then
			fileExt = fileExt.Substring(1)
		End If
		Select Case fileExt.ToLower()
			Case "pdf"
				Return StreamType.AdobePDF

			Case "docx"
				Return StreamType.WordprocessingML

			Case "htm", "html"
				Return StreamType.HTMLFormat

			Case "tx"
				Return StreamType.InternalUnicodeFormat

			Case "doc"
				Return StreamType.MSWord

			Case "rtf"
				Return StreamType.RichTextFormat

			Case "txt", "text"
				Return StreamType.PlainText
		End Select

		' Default to DOCX
		Return StreamType.WordprocessingML
	End Function

	'-----------------------------------------------------------------------------------------
	' Converts a TX stream type to a save file dialog filter string (e. g. 
	' "Word Document (*.docx)|*.docx") if possible.
	'---------------------------------------------------------------------------------------

	<Extension> _
	Public Function ToFilterString(streamType__1 As StreamType) As String
		Select Case streamType__1
			Case StreamType.AdobePDF, StreamType.AdobePDFA
				Return My.Resources.APP_MENU_SAVE_AS_PDF + "|*.pdf"

			Case StreamType.HTMLFormat
				Return My.Resources.APP_MENU_SAVE_AS_HTML + "|*.htm;*.html"

			Case StreamType.InternalFormat, StreamType.InternalUnicodeFormat
				Return My.Resources.APP_MENU_SAVE_AS_TX + "|*.tx"

			Case StreamType.MSWord
				Return My.Resources.APP_MENU_SAVE_AS_DOC + "|*.doc"

			Case StreamType.PlainAnsiText, StreamType.PlainText
				Return My.Resources.APP_MENU_SAVE_AS_TXT + "|*.txt"

			Case StreamType.RichTextFormat
				Return My.Resources.APP_MENU_SAVE_AS_RTF + "|*.rtf"

			Case StreamType.WordprocessingML
				Return My.Resources.APP_MENU_SAVE_AS_DOCX + "|*.docx"
		End Select

		' Default to DOCX
		Return My.Resources.APP_MENU_SAVE_AS_DOCX + "|*.docx"
	End Function

	'-----------------------------------------------------------------------------------------
	' Converts a TX stream type to a file extension (e. g. ".docx" or ".html")
	'---------------------------------------------------------------------------------------

	<Extension> _
	Public Function ToFileExt(streamType__1 As StreamType) As String
		Select Case streamType__1
			Case StreamType.AdobePDF, StreamType.AdobePDFA
				Return ".pdf"

			Case StreamType.HTMLFormat
				Return ".html"

			Case StreamType.InternalFormat, StreamType.InternalUnicodeFormat
				Return ".tx"

			Case StreamType.MSWord
				Return ".doc"

			Case StreamType.PlainAnsiText, StreamType.PlainText
				Return ".txt"

			Case StreamType.RichTextFormat
				Return ".rtf"

			Case StreamType.WordprocessingML
				Return ".docx"

			Case StreamType.XMLFormat
				Return ".xml"
		End Select

		Return ""
	End Function
End Module

#End Region
