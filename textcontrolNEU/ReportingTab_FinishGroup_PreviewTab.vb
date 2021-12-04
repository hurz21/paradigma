'-------------------------------------------------------------------------------------------------------------
' module:        TX Text Control Words
'
' copyright:     © Text Control GmbH
' author:        F. Zenker
'-----------------------------------------------------------------------------------------------------------

Imports Microsoft.Windows.Controls.Ribbon
Imports System.Collections.Generic
Imports System.Windows.Controls
Imports System.Windows.Input
Imports TXTextControl

Partial Public Class MainWindow

#Region "  Private Fields  "

	Private m_lstMergedFiles As IList(Of Byte()) = Nothing

	Private m_iPreviewIndex As Integer = 0
	Private m_nPreviewCount As Integer = 0

#End Region

#Region "  Preview Group  "

	'--------------------------------------------------------------------------------------------
	' ClosePreviewCommand_Executed
	' Close the contextual 'Preview' tab and select the 'Reporting' tab.
	'------------------------------------------------------------------------------------------

	Private Sub ClosePreviewCommand_Executed(sender As Object, e As ExecutedRoutedEventArgs)
		HandleClosePreview()
		m_ribbon.SelectedItem = m_reportingTab
	End Sub

#End Region

#Region "  Navigate Group  "

#Region " First Record Button "

	'--------------------------------------------------------------------------------------------
	' FirstRecordCommand_Executed
	' Load's the first record into the TextControl and updates the enable states of the 'Navigate'
	' gourp's buttons
	'------------------------------------------------------------------------------------------

	Private Sub FirstRecordCommand_Executed(sender As Object, e As ExecutedRoutedEventArgs)
		m_iPreviewIndex = 0
		m_textControl.Load(m_lstMergedFiles(m_iPreviewIndex), BinaryStreamType.InternalUnicodeFormat)
		UpdateNavigateButtons()
	End Sub

	'--------------------------------------------------------------------------------------------
	' FirstRecord_ToolTipOpening
	' Updates the 'First Record' button's tool strip description text with the corresponding 
	' record number.
	'------------------------------------------------------------------------------------------

	Private Sub FirstRecord_ToolTipOpening(sender As Object, e As ToolTipEventArgs)
		SetToolTipDescription(m_btnFirstRecord)
	End Sub

#End Region

#Region " Previous Record Button "

	'--------------------------------------------------------------------------------------------
	' PreviousRecordCommand_Executed
	' Load's the previous record into the TextControl and updates the enable states of the 'Navigate'
	' gourp's buttons
	'------------------------------------------------------------------------------------------

	Private Sub PreviousRecordCommand_Executed(sender As Object, e As ExecutedRoutedEventArgs)
		m_iPreviewIndex -= 1
		m_textControl.Load(m_lstMergedFiles(m_iPreviewIndex), BinaryStreamType.InternalUnicodeFormat)
		UpdateNavigateButtons()
	End Sub

	'--------------------------------------------------------------------------------------------
	' PreviousRecord_ToolTipOpening
	' Updates the 'Previous Record' button's tool strip description text with the corresponding 
	' record number.
	'------------------------------------------------------------------------------------------

	Private Sub PreviousRecord_ToolTipOpening(sender As Object, e As ToolTipEventArgs)
		SetToolTipDescription(m_btnPreviousRecord)
	End Sub

#End Region

#Region " Next Record Button "

	'--------------------------------------------------------------------------------------------
	' NextRecordCommand_Executed
	' Load's the next last into the TextControl and updates the enable states of the 'Navigate'
	' gourp's buttons
	'------------------------------------------------------------------------------------------

	Private Sub NextRecordCommand_Executed(sender As Object, e As ExecutedRoutedEventArgs)
		m_iPreviewIndex += 1
		m_textControl.Load(m_lstMergedFiles(m_iPreviewIndex), BinaryStreamType.InternalUnicodeFormat)
		UpdateNavigateButtons()
	End Sub

	'--------------------------------------------------------------------------------------------
	' NextRecord_ToolTipOpening
	' Updates the 'Next Record' button's tool strip description text with the corresponding 
	' record number.
	'------------------------------------------------------------------------------------------

	Private Sub NextRecord_ToolTipOpening(sender As Object, e As ToolTipEventArgs)
		SetToolTipDescription(m_btnNextRecord)
	End Sub

#End Region

#Region " Last Record Button "

	'--------------------------------------------------------------------------------------------
	' LastRecordCommand_Executed
	' Load's the previous last into the TextControl and updates the enable states of the 'Navigate'
	' gourp's buttons
	'------------------------------------------------------------------------------------------

	Private Sub LastRecordCommand_Executed(sender As Object, e As ExecutedRoutedEventArgs)
		m_iPreviewIndex = m_nPreviewCount - 1
		m_textControl.Load(m_lstMergedFiles(m_iPreviewIndex), BinaryStreamType.InternalUnicodeFormat)
		UpdateNavigateButtons()
	End Sub

	'--------------------------------------------------------------------------------------------
	' LastRecord_ToolTipOpening
	' Updates the 'Last Record' button's tool strip description text with the corresponding 
	' record number.
	'------------------------------------------------------------------------------------------

	Private Sub LastRecord_ToolTipOpening(sender As Object, e As ToolTipEventArgs)
		SetToolTipDescription(m_btnLastRecord)
	End Sub

#End Region

	'--------------------------------------------------------------------------------------------
	' LastRecord_ToolTip_Opening
	' Updates the button's tool strip description text of a specific navigate button with the 
	' corresponding record number.
	'------------------------------------------------------------------------------------------

	Private Sub SetToolTipDescription(navigateButton As RibbonButton)
		Dim iDataSetNumber As Integer = 0
		Select Case navigateButton.Name
			Case "m_btnFirstRecord"
				iDataSetNumber = 0
				Exit Select
			Case "m_btnPreviousRecord"
				iDataSetNumber = m_iPreviewIndex - 1
				Exit Select
			Case "m_btnNextRecord"
				iDataSetNumber = m_iPreviewIndex + 1
				Exit Select
			Case "m_btnLastRecord"
				iDataSetNumber = m_nPreviewCount - 1
				Exit Select

		End Select
		navigateButton.ToolTipDescription = String.Format(My.Resources.GO_TO_RECORD_TOOLTIP, (iDataSetNumber + 1))
	End Sub

	'--------------------------------------------------------------------------------------------
	' UpdateNavigateButtons
	' Updates the 'Navigate' button's enable state.
	'------------------------------------------------------------------------------------------

	Private Sub UpdateNavigateButtons()
		m_ctgTableTools.Visibility = System.Windows.Visibility.Collapsed
		m_btnFirstRecord.IsEnabled = (m_iPreviewIndex > 0)
		m_btnPreviousRecord.IsEnabled = (m_iPreviewIndex > 0)
		m_btnNextRecord.IsEnabled = (m_iPreviewIndex < m_nPreviewCount - 1)
		m_btnLastRecord.IsEnabled = (m_iPreviewIndex < m_nPreviewCount - 1)
	End Sub


#End Region

	'--------------------------------------------------------------------------------------------
	' Ribbon_SelectionChanged
	' If the contextual 'Preview' tab is deselected, all preview settings are reset.
	'------------------------------------------------------------------------------------------

	Private Sub Ribbon_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
		If m_ctgReportingPreview.Visibility = System.Windows.Visibility.Visible AndAlso m_ribbon.SelectedItem IsNot m_previewTab Then
			HandleClosePreview()
		End If
	End Sub

	'--------------------------------------------------------------------------------------------
	' HandleClosePreview
	' Resets the TextControl's edit mode and content. Furthermore the contextual 'Preview' tab 
	' is hidden and the 'Reporting' tab's 'Preview' button is enabled.
	'------------------------------------------------------------------------------------------

	Private Sub HandleClosePreview()
		m_textControl.EditMode = m_editMode
		If m_textControlContent IsNot Nothing Then
			m_textControl.Load(m_textControlContent, BinaryStreamType.InternalUnicodeFormat)
		End If
		m_ctgReportingPreview.Visibility = System.Windows.Visibility.Collapsed
		m_btnPreview.IsEnabled = True
	End Sub
End Class
