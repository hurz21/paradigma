'-------------------------------------------------------------------------------------------------------------
'** module:        TX Text Control Words
'**
'** copyright:     © Text Control GmbH
'** author:        T. Kummerow
'**-----------------------------------------------------------------------------------------------------------

Imports Microsoft.Windows.Controls.Ribbon
Imports System
Imports System.IO
Imports System.Reflection
Imports System.Windows
Imports System.Windows.Input
Imports TXTextControl.WPF

'----------------------------------------------------------------------------------------------------------
' Adding the "Load sample DB" buton to the load db menu on the reporting tab
'--------------------------------------------------------------------------------------------------------

Partial Class MainWindow

#Region "  Button Creation  "

	Private Sub AddLoadSampleDbMenuButton()
		Dim mnuItm = DirectCast(Me.Resources("MnuItmLoadSampleDb"), RibbonMenuItem)
		mnuItm.Header = My.Resources.OPEN_SAMPLE_DB_MENU_BTN_TEXT
		mnuItm.ToolTip = My.Resources.OPEN_SAMPLE_DB_MENU_BTN_TOOLTIP
		Dim btn = DirectCast(m_reportingTab.FindName(RibbonReportingTab.RibbonItem.TXITEM_DataSource.ToString()), RibbonSplitButton)
		btn.Items.Insert(3, mnuItm)
	End Sub

#End Region


#Region "  Event Handlers  "

	Private Sub LoadSampleDatabaseCommand_Executed(sender As Object, e As ExecutedRoutedEventArgs)
		Dim dir As String = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
		Dim fileName As String = dir & "\..\sample_db.xml"
		Try
			m_reportingTab.DataSourceManager.LoadXmlFile(fileName)
		Catch exc As Exception
			MessageBox.Show(exc.Message, ProductName, MessageBoxButton.OK, MessageBoxImage.[Error])
		End Try
	End Sub

#End Region
End Class
