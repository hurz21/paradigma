Imports System.Collections.Specialized
Imports System.Printing
Imports System.Windows.Controls
Imports System.Windows.Input
Imports System.Linq
Imports System.IO
Imports Microsoft.Windows.Controls.Ribbon


'-------------------------------------------------------------------------------------------------------
' Interaction logic for the application menu.
'-----------------------------------------------------------------------------------------------------

' Needed for click events on sample template buttons
Public Enum SampleTemplateType
	Invoice
	PackingList
	ShippingLabel
End Enum

Partial Public Class MainWindow

	'-------------------------------------------------------------------------------------------------------
	' Populates the application menu.
	'-----------------------------------------------------------------------------------------------------

	Private Sub InitializeAppMenu()
		' Localize texts
		LocalizeAppMenu()
	End Sub

	'-------------------------------------------------------------------------------------------------------
	' sets recent document list.
	'-----------------------------------------------------------------------------------------------------

	Private Sub SetRecentItemsList(recentFiles As StringCollection)
        'm_rgcRecentFiles.Items.Clear()
        'For Each fileName As String In recentFiles
        '	m_rgcRecentFiles.Items.Add(New RibbonGalleryItem() With { _
        '		.Content = Path.GetFileName(fileName), _
        '		.Tag = fileName _
        '	})
        'Next
    End Sub

	'-------------------------------------------------------------------------------------------------------
	' Localizes application menu texts.
	'-----------------------------------------------------------------------------------------------------

	Private Sub LocalizeAppMenu()
        'm_appMenu.ToolTipTitle = My.Resources.APP_MENU_TOOLTIP_TITLE
        'm_appMenu.ToolTipDescription = My.Resources.APP_MENU_TOOLTIP_DESC
        'm_appMnuItmNew.Header = My.Resources.APP_MENU_NEW
        'm_appMnuItmNew.ToolTipTitle = My.Resources.APP_MENU_NEW_TOOLTIP
        'm_appMnuItmOpen.Header = My.Resources.APP_MENU_OPEN
        'm_appMnuItmOpen.ToolTipTitle = My.Resources.APP_MENU_OPEN_TOOLTIP
        'LocalizeOpenSampleTemplateButton()
        'm_appMnuItmSave.Header = My.Resources.APP_MENU_SAVE
        'm_appMnuItmSave.ToolTipTitle = My.Resources.APP_MENU_SAVE_TOOLTIP
        'm_appMnuItmSaveAs.Header = My.Resources.APP_MENU_SAVE_AS
        'm_appMnuItmSaveAs.ToolTipTitle = My.Resources.APP_MENU_SAVE_AS_TOOLTIP
        LocalizeSaveAsMenu()
        'm_appSpltMnuItmPrint.Header = My.Resources.APP_MENU_PRINT
        'm_appSpltMnuItmPrint.ToolTipTitle = My.Resources.APP_MENU_PRINT_TOOLTIP
        'm_appMnuItmPrint.Header = My.Resources.APP_MENU_PRINT
        'm_appMnuItmPrint.ToolTip = My.Resources.APP_MENU_PRINT_TOOLTIP
        'm_appMnuItmQuickPrint.Header = My.Resources.APP_MENU_PRINT_QUICK
        'm_appMnuItmQuickPrint.ToolTip = My.Resources.APP_MENU_PRINT_QUICK_TOOLTIP
        'm_btnClose.Label = My.Resources.APP_MENU_EXIT
        'm_btnClose.ToolTip = My.Resources.APP_MENU_EXIT_TOOLTIP
        'm_btnOptions.Label = My.Resources.APP_MENU_OPTIONS
        'm_btnOptions.ToolTip = My.Resources.APP_MENU_OPTIONS_TOOLTIP

        'm_rgcRecentFiles.Header = My.Resources.APP_MENU_RECENT_ITEMS_HEADER
    End Sub

	'-------------------------------------------------------------------------------------------------------
	' Localizes "Open Sample Template" button
	'-----------------------------------------------------------------------------------------------------

	Private Sub LocalizeOpenSampleTemplateButton()
        ' Open Sample Template button
        'm_appMnuItmOpenTmpl.Header = My.Resources.APP_MENU_OPEN_SAMPLE
        'm_appMnuItmOpenTmpl.ToolTip = My.Resources.APP_MENU_OPEN_SAMPLE_TOOLTIP
        'm_mnuItm_OpenSampleInvoice.Header = My.Resources.APP_MENU_OPEN_SAMPLE_INVOICE
        'm_mnuItm_OpenSampleInvoice.ToolTip = My.Resources.APP_MENU_OPEN_SAMPLE_INVOICE
        'm_mnuItm_OpenSamplePackList.Header = My.Resources.APP_MENU_OPEN_SAMPLE_PACK_LST
        'm_mnuItm_OpenSamplePackList.ToolTip = My.Resources.APP_MENU_OPEN_SAMPLE_PACK_LST
        'm_mnuItm_OpenSampleShipLabel.Header = My.Resources.APP_MENU_OPEN_SAMPLE_SHIP_LBL
        'm_mnuItm_OpenSampleShipLabel.ToolTip = My.Resources.APP_MENU_OPEN_SAMPLE_SHIP_LBL
    End Sub

	'-------------------------------------------------------------------------------------------------------
	' Localizes "save as..." menu
	'-----------------------------------------------------------------------------------------------------

	Private Sub LocalizeSaveAsMenu()
        'm_appMnuItmSaveAsDoc.Header = My.Resources.APP_MENU_SAVE_AS_DOC
        'm_appMnuItmSaveAsDoc.ToolTip = My.Resources.APP_MENU_SAVE_AS_DOC_TOOLTIP
        'm_appMnuItmSaveAsDocx.Header = My.Resources.APP_MENU_SAVE_AS_DOCX
        'm_appMnuItmSaveAsDocx.ToolTip = My.Resources.APP_MENU_SAVE_AS_DOCX_TOOLTIP
        'm_appMnuItmSaveAsRtf.Header = My.Resources.APP_MENU_SAVE_AS_RTF
        'm_appMnuItmSaveAsRtf.ToolTip = My.Resources.APP_MENU_SAVE_AS_RTF_TOOLTIP
        'm_appMnuItmSaveAsPdf.Header = My.Resources.APP_MENU_SAVE_AS_PDF
        'm_appMnuItmSaveAsPdf.ToolTip = My.Resources.APP_MENU_SAVE_AS_PDF_TOOLTIP
        'm_appMnuItmSaveAsHtml.Header = My.Resources.APP_MENU_SAVE_AS_HTML
        'm_appMnuItmSaveAsHtml.ToolTip = My.Resources.APP_MENU_SAVE_AS_HTML_TOOLTIP
        'm_appMnuItmSaveAsOther.Header = My.Resources.APP_MENU_SAVE_AS_OTHER
        'm_appMnuItmSaveAsOther.ToolTip = My.Resources.APP_MENU_SAVE_AS_OTHER_TOOLTIP
    End Sub
End Class
