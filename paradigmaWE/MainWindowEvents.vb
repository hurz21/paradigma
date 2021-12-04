Imports Microsoft.Windows.Controls.Ribbon
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Input

'-------------------------------------------------------------------------------------------------------
' MainWindow event handlers
'-----------------------------------------------------------------------------------------------------

Partial Public Class MainWindow

#Region "  Methods  "

#Region "  Event Handlers  "

	Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs)
		SetWindowTitle(m_fileHandler.DocumentTitle)
		LocalizeWindow()
		LoadAppSettings()
	End Sub

	Private Sub MainWindow_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs)
		e.Cancel = Not m_fileHandler.ExitApplication()
		If Not e.Cancel Then
            SaveAppSettings()
            'MsgBox("Remove LockFile" & m_fileHandler.jfDokumentId)
            Remove_LockFile(m_fileHandler.jfLocalLocksRoot, m_fileHandler.jfDokumentId)
        End If
	End Sub

    Private Sub Remove_LockFile(jfLocalLocksRoot As String, jfDokumentId As Integer)
        Dim datei As String
        Dim fi As IO.FileInfo
        Try
            If jfDokumentId < 0 Then Exit Sub
            datei = jfLocalLocksRoot & "\" & jfDokumentId
            fi = New IO.FileInfo(datei)
            fi.Delete()
            fi = Nothing
        Catch ex As Exception
            MsgBox("fehler   Remove_LockFile: " & ex.ToString)
        End Try
    End Sub

    Private Sub HelpHandler(sender As Object, e As ExecutedRoutedEventArgs)
        AboutBox.Show(Me)
    End Sub

    Private Sub Ribbon_Loaded(sender As Object, e As RoutedEventArgs)
		AddFinishGroup()
		AddLoadSampleDbMenuButton()
	End Sub

#End Region

	Private Sub LocalizeWindow()
		' Status bar
		m_statusBar.LineText = My.Resources.STATUSBAR_LINE
		m_statusBar.PageText = My.Resources.STATUSBAR_PAGE
		m_statusBar.ColumnText = My.Resources.STATUSBAR_COLUMN
		m_statusBar.SectionText = My.Resources.STATUSBAR_SECTION

		' Set individual text to contextual ribbon tabs so the text in the 
		' contextual tab group header is completely visible
		m_tableLayoutTab.Header = My.Resources.RIBBON_TAB_TABLE_FORMAT_HEADER
		m_frameLayoutTab.Header = My.Resources.RIBBON_TAB_FRAME_FORMAT_HEADER

		' Contextual Tab Group headers
		m_ctgTableTools.Header = My.Resources.CONT_TAB_GRP_TBL_TOOLS
		m_tableLayoutTab.ContextualTabGroupHeader = My.Resources.CONT_TAB_GRP_TBL_TOOLS
		m_ctgFrameTools.Header = My.Resources.CONT_TAB_GRP_FRAME_TOOLS
		m_frameLayoutTab.ContextualTabGroupHeader = My.Resources.CONT_TAB_GRP_FRAME_TOOLS
		m_ctgReportingPreview.Header = m_reportingTab.Header
		m_previewTab.ContextualTabGroupHeader = m_reportingTab.Header

		' "Finish and Merge" group
		Dim grpFinishMerge = DirectCast(Me.Resources("RibbonGroupFinish"), RibbonGroup)
		grpFinishMerge.Header = My.Resources.RIBBON_GROUP_FINISH.ConvertAccelerators()
		Dim btnPreview = DirectCast(grpFinishMerge.Items(0), RibbonButton)
		btnPreview.Label = My.Resources.BTN_PREVIEW.ConvertAccelerators()
		btnPreview.ToolTipTitle = btnPreview.Label
		btnPreview.ToolTipDescription = My.Resources.BTN_PREVIEW_TOOLTIP
		Dim btnFinishMerge = DirectCast(grpFinishMerge.Items(1), RibbonSplitButton)
		btnFinishMerge.Label = My.Resources.BTN_FINISH_AND_MERGE.ConvertAccelerators()
		btnFinishMerge.ToolTipTitle = btnFinishMerge.Label
		btnFinishMerge.ToolTipDescription = My.Resources.BTN_FINISH_AND_MERGE_TOOLTIP
		Dim mnuMergeIntoCurrent = DirectCast(btnFinishMerge.Items(0), RibbonMenuItem)
		mnuMergeIntoCurrent.Header = My.Resources.BTN_MERGE_INTO_CURRENT
		mnuMergeIntoCurrent.ToolTipTitle = mnuMergeIntoCurrent.Header.ToString()
		mnuMergeIntoCurrent.ToolTipDescription = My.Resources.BTN_FINISH_AND_MERGE_TOOLTIP
		Dim mnuMergeIntoSingle = DirectCast(btnFinishMerge.Items(1), RibbonMenuItem)
		mnuMergeIntoSingle.Header = My.Resources.BTN_MERGE_INTO_SINGLE_FILE
		mnuMergeIntoSingle.ToolTipTitle = mnuMergeIntoSingle.Header.ToString()
		mnuMergeIntoSingle.ToolTipDescription = My.Resources.BTN_MERGE_INTO_SINGLE_FILE_TOOLTIP
		Dim mnuMergeIntoIdividual = DirectCast(btnFinishMerge.Items(2), RibbonMenuItem)
		mnuMergeIntoIdividual.Header = My.Resources.BTN_MERGE_INTO_MULTIPLE_FILES
		mnuMergeIntoIdividual.ToolTipTitle = mnuMergeIntoIdividual.Header.ToString()
		mnuMergeIntoIdividual.ToolTipDescription = My.Resources.BTN_MERGE_INTO_MULTIPLE_FILES_TOOLTIP
		Dim mnuMergePrint = DirectCast(btnFinishMerge.Items(3), RibbonMenuItem)
		mnuMergePrint.Header = My.Resources.BTN_MERGE_PRINT
		mnuMergePrint.ToolTipTitle = mnuMergePrint.Header.ToString()
		mnuMergePrint.ToolTipDescription = My.Resources.BTN_MERGE_PRINT_TOOLTIP

		' Preview tab
		m_previewTab.Header = My.Resources.RIBBON_TAB_PREVIEW_HEADER
		m_grpPreview.Header = My.Resources.RIBBON_GROUP_PREVIEW
		m_btnClosePreview.Label = My.Resources.BTN_CLOSE_PREVIEW
		m_btnClosePreview.ToolTipTitle = m_btnClosePreview.Label
		m_btnClosePreview.ToolTipDescription = My.Resources.BTN_CLOSE_PREVIEW_TOOLTIP
		m_grpNavigate.Header = My.Resources.RIBBON_GROUP_NAVIGATE
		m_btnFirstRecord.Label = My.Resources.BTN_FIRST_RECORD
		m_btnFirstRecord.ToolTipTitle = m_btnFirstRecord.Label
		m_btnPreviousRecord.Label = My.Resources.BTN_PREVIOUS_RECORD
		m_btnPreviousRecord.ToolTipTitle = m_btnPreviousRecord.Label
		m_btnNextRecord.Label = My.Resources.BTN_NEXT_RECORD
		m_btnNextRecord.ToolTipTitle = m_btnNextRecord.Label
		m_btnLastRecord.Label = My.Resources.BTN_LAST_RECORD
		m_btnLastRecord.ToolTipTitle = m_btnLastRecord.Label
		' Applicationfield context menu
		Dim mnuAppfield = DirectCast(Me.Resources("ContextMenuApplicationFields"), ContextMenu)
		Dim mnuProperties = DirectCast(mnuAppfield.Items(0), MenuItem)
		mnuProperties.Header = My.Resources.FIELD_NAV_BTN_PROPERTIES.ConvertAccelerators()
		Dim mnuDelete = DirectCast(mnuAppfield.Items(1), MenuItem)
		mnuDelete.Header = My.Resources.FIELD_NAV_BTN_REMOVE.ConvertAccelerators()
	End Sub

#End Region
End Class
