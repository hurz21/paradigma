Imports Microsoft.Win32
Imports System
Imports System.Security.Cryptography.X509Certificates
'-------------------------------------------------------------------------------------------------------------
'** module:        TX Text Control Words
'**
'** copyright:     © Text Control GmbH
'** author:        T. Kummerow
'**-----------------------------------------------------------------------------------------------------------

Imports System.Text.RegularExpressions
Imports System.Windows
Imports System.Windows.Controls
Imports System.Runtime.CompilerServices
Imports TXTextControl
Imports TXTextControl.WPF

''' <summary>
''' Interaction logic for OptionsDialog.xaml
''' </summary>
Partial Public Class OptionsDialog
	Inherits Window

#Region "  Private Fields  "

	Private m_textControl As TextControl
	Private m_fileHandler As FileHandling.FileHandler

#End Region


#Region "  Constructors  "

	Public Sub New(textControl As TextControl, fileHandler As FileHandling.FileHandler)
		InitializeComponent()
		LocalizeDialog()
		Me.m_textControl = textControl
		Me.m_fileHandler = fileHandler
	End Sub

#End Region


#Region "  Methods  "

	Private Sub LocalizeDialog()
		' Window title
		Title = My.Resources.OPT_DLG_TITLE.ConvertAccelerators()

		' Button texts
		m_btnOK.Content = My.Resources.BTN_OK.ConvertAccelerators()
		m_btnCancel.Content = My.Resources.BTN_CANCEL.ConvertAccelerators()

		' Tab pages
		m_tabPageHTML.Header = My.Resources.OPT_DLG_TAB_HTMLOPTS.ConvertAccelerators()
		m_tabPagePDFSecurity.Header = My.Resources.OPT_DLG_TAB_PDFSEC.ConvertAccelerators()
		m_tabPagePDFExport.Header = My.Resources.OPT_DLG_TAB_PDFEXP.ConvertAccelerators()
		m_tabPagePDFImport.Header = My.Resources.OPT_DLG_TAB_PDFIMP.ConvertAccelerators()

		' HTML options
		m_grpCSS.Header = My.Resources.OPT_DLG_CSS_GRP_SAVEOPTS.ConvertAccelerators()
		m_optNoStylesheet.Content = My.Resources.OPT_DLG_CSS_NONE.ConvertAccelerators()
		m_optInlineStylesheet.Content = My.Resources.OPT_DLG_CSS_INLINE.ConvertAccelerators()
		m_optSaveStylesheetInSeperateFile.Content = My.Resources.OPT_DLG_CSS_SEPARATE_FILE.ConvertAccelerators()
		m_optSaveButDoNotOverwriteExistingFile.Content = My.Resources.OPT_DLG_CSS_SAVE_NOT_OVR.ConvertAccelerators()
		m_lblStylesheetFile.Content = My.Resources.OPT_DLG_CSS_LBL_FILE.ConvertAccelerators()

		' PDF security
		m_grpPDFDigSig.Header = My.Resources.OPT_DLG_PDFSEC_GRP_DIG_SIG.ConvertAccelerators()
		m_grpPDFDocPwd.Header = My.Resources.OPT_DLG_PDFSEC_GRP_DOC_PWD.ConvertAccelerators()
		m_grpPDFPermissions.Header = My.Resources.OPT_DLG_PDFSEC_GRP_PERM.ConvertAccelerators()
		m_lblCertFile.Content = My.Resources.OPT_DLG_PDFSEC_LBL_CERT_FILE.ConvertAccelerators()
		m_lblCertPwd.Content = My.Resources.OPT_DLG_PDFSEC_LBL_CERT_PWD.ConvertAccelerators()
		m_chkUserPassword.Content = My.Resources.OPT_DLG_PDFSEC_REQ_DOC_PWD.ConvertAccelerators()
		m_lblDocumentPassword.Content = My.Resources.OPT_DLG_PDFSEC_LBL_DOC_PWD.ConvertAccelerators()
		m_chkMasterPassword.Content = My.Resources.OPT_DLG_PDFSEC_REQ_MSTR_PWD.ConvertAccelerators()
		m_lblMasterPwd.Content = My.Resources.OPT_DLG_PDFSEC_LBL_PERM_PWD.ConvertAccelerators()
		m_lblAllowPrint.Content = My.Resources.OPT_DLG_PDFSEC_LBL_ALLOW_PRNT.ConvertAccelerators()
		m_lblAllowChng.Content = My.Resources.OPT_DLG_PDFSEC_LBL_ALLOW_CHNG.ConvertAccelerators()
		m_chkAllowExtractContents.Content = My.Resources.OPT_DLG_PDFSEC_ALLOW_EXTR_CNT.ConvertAccelerators()
		m_chkAllowContentAccessibility.Content = My.Resources.OPT_DLG_PDFSEC_ALLOW_ACCESSIBILITY.ConvertAccelerators()

		' PDF import
		m_grpPDFImport.Header = My.Resources.OPT_DLG_PDFIMP_GRP_OPTS.ConvertAccelerators()
		m_lblPDFImportMode.Content = My.Resources.OPT_DLG_PDFIMP_LBL_MODE.ConvertAccelerators()

		' PDF export
		m_grpPDFExport.Header = My.Resources.OPT_DLG_PDFEXP_GRP_OPTS.ConvertAccelerators()
		m_chkPDFEmbeddableFontsOnly.Content = My.Resources.OPT_DLG_PDFEXP_ENABLE_PDFA.ConvertAccelerators()
	End Sub

	Private Function ValidateDialogContent() As Boolean
		If m_chkMasterPassword.IsChecked.Value AndAlso m_txtMasterPassword.Password.Length = 0 Then
			MessageBox.Show(My.Resources.OPT_DLG_PDFSEC_ERR_NO_MASTER_PWD, MainWindow.ProductName)
			Return False
		End If
		If m_chkUserPassword.IsChecked.Value AndAlso m_txtUserPassword.Password.Length = 0 Then
			MessageBox.Show(My.Resources.OPT_DLG_PDFSEC_ERR_NO_DOC_PWD, MainWindow.ProductName)
			Return False
		End If
		If m_chkMasterPassword.IsChecked.Value AndAlso m_chkUserPassword.IsChecked.Value AndAlso m_txtMasterPassword.Password = m_txtUserPassword.Password Then
			MessageBox.Show(My.Resources.OPT_DLG_PDFSEC_ERR_SAME_PWD, MainWindow.ProductName)
			Return False
		End If
		Return True
	End Function

#Region "  Event Handlers  "

	Private Sub BtnOK_Click(sender As Object, e As RoutedEventArgs)
		If Not ValidateDialogContent() Then
			Return
		End If

		' Digital signature
		If m_txtCertFile.Text <> "" Then
			Try
				m_txtCertFile.Text = m_txtCertFile.Text.Trim()
				Dim cert = New X509Certificate2(m_txtCertFile.Text, m_txtCertPwd.Password)
				m_fileHandler.PDFSignature = New DigitalSignature(cert, Nothing)
				m_fileHandler.PDFCertFilePath = m_txtCertFile.Text
				m_fileHandler.PDFCertPasswd = m_txtCertPwd.Password
			Catch exc As Exception
				MessageBox.Show(String.Format(My.Resources.EXC_PDF_SIGNATURE, exc.Message), MainWindow.ProductName, MessageBoxButton.OK, MessageBoxImage.[Error])
				Return
			End Try
		End If

		m_fileHandler.PDFUserPassword = m_txtUserPassword.Password
		m_fileHandler.PDFMasterPassword = m_txtMasterPassword.Password
		m_fileHandler.CssFileName = m_txtStylesheetFile.Text

		If m_optNoStylesheet.IsChecked.Value Then
			m_fileHandler.CssSaveMode = TXTextControl.CssSaveMode.None
		ElseIf m_optInlineStylesheet.IsChecked.Value Then
			m_fileHandler.CssSaveMode = TXTextControl.CssSaveMode.Inline
		ElseIf m_optSaveStylesheetInSeperateFile.IsChecked.Value Then
			m_fileHandler.CssSaveMode = TXTextControl.CssSaveMode.OverwriteFile
		Else
			m_fileHandler.CssSaveMode = TXTextControl.CssSaveMode.CreateFile
		End If

		Dim uFlags As UInteger = 0

		' Printing combo box
		If m_cbPrinting.SelectedIndex = 2 Then
			uFlags += CUInt(TXTextControl.DocumentAccessPermissions.AllowHighLevelPrinting)
		ElseIf m_cbPrinting.SelectedIndex = 1 Then
			uFlags += CUInt(TXTextControl.DocumentAccessPermissions.AllowLowLevelPrinting)
		End If

		' Changes Allowed combo box
		If m_cbChangesAllowed.SelectedIndex = 4 Then
			uFlags += CUInt(TXTextControl.DocumentAccessPermissions.AllowAuthoring) + CUInt(TXTextControl.DocumentAccessPermissions.AllowDocumentAssembly) + CUInt(TXTextControl.DocumentAccessPermissions.AllowGeneralEditing)
		ElseIf m_cbChangesAllowed.SelectedIndex = 3 Then
			uFlags += CUInt(TXTextControl.DocumentAccessPermissions.AllowAuthoring)
		ElseIf m_cbChangesAllowed.SelectedIndex = 2 Then
			uFlags += CUInt(TXTextControl.DocumentAccessPermissions.AllowAuthoringFields)
		ElseIf m_cbChangesAllowed.SelectedIndex = 1 Then
			uFlags += CUInt(TXTextControl.DocumentAccessPermissions.AllowDocumentAssembly)
		End If

		' Remaining 2 checkboxes
		If m_chkAllowContentAccessibility.IsChecked.Value Then
			uFlags += CUInt(TXTextControl.DocumentAccessPermissions.AllowContentAccessibility)
		End If
		If m_chkAllowExtractContents.IsChecked.Value Then
			uFlags += CUInt(TXTextControl.DocumentAccessPermissions.AllowExtractContents)
		End If

		m_fileHandler.DocumentAccessPermissions = CType(uFlags, TXTextControl.DocumentAccessPermissions)

		' Set PDFImportSettings
		Select Case m_cbPDFImportMode.SelectedIndex
			Case 0
				m_fileHandler.PDFImportSettings = TXTextControl.PDFImportSettings.GenerateLines
				Exit Select

			Case 1
				m_fileHandler.PDFImportSettings = TXTextControl.PDFImportSettings.GenerateParagraphs
				Exit Select

			Case 2
				m_fileHandler.PDFImportSettings = TXTextControl.PDFImportSettings.GenerateTextFrames
				Exit Select
		End Select

		' PDF/A setting
		m_textControl.FontSettings.EmbeddableFontsOnly = m_chkPDFEmbeddableFontsOnly.IsChecked.Value

		' Close the dialog
		Close()
	End Sub

	Private Sub OptNoStylesheet_Checked(sender As Object, e As RoutedEventArgs)
		m_txtStylesheetFile.IsEnabled = False
		m_lblStylesheetFile.IsEnabled = False
	End Sub

	Private Sub OptInlineStylesheet_Checked(sender As Object, e As RoutedEventArgs)
		m_txtStylesheetFile.IsEnabled = True
		m_lblStylesheetFile.IsEnabled = True
	End Sub

	Private Sub OptSaveStylesheetInSeperateFile_Checked(sender As Object, e As RoutedEventArgs)
		m_txtStylesheetFile.IsEnabled = True
		m_lblStylesheetFile.IsEnabled = True
	End Sub

	Private Sub OptSaveButDoNotOverwriteExistingFile_Checked(sender As Object, e As RoutedEventArgs)
		m_txtStylesheetFile.IsEnabled = True
		m_lblStylesheetFile.IsEnabled = True
	End Sub

	Private Sub BtnBrowseCertFile_Click(sender As Object, e As RoutedEventArgs)
		Dim ofd As New OpenFileDialog()
		ofd.CheckPathExists = True
		ofd.Filter = "Personal Information Exchange File (*.pfx)|*.pfx"
		ofd.ValidateNames = True
		If ofd.ShowDialog(Me) <> True Then
			Return
		End If
		m_txtCertFile.Text = ofd.FileName
	End Sub

	Private Sub ChkUserPassword_Checked(sender As Object, e As RoutedEventArgs)
		m_txtUserPassword.IsEnabled = True
		m_chkPDFEmbeddableFontsOnly.IsEnabled = False
	End Sub

	Private Sub ChkUserPassword_Unchecked(sender As Object, e As RoutedEventArgs)
		m_txtUserPassword.Password = ""
		m_txtUserPassword.IsEnabled = False
		m_chkPDFEmbeddableFontsOnly.IsEnabled = True
	End Sub

	Private Sub ChkMasterPassword_Checked(sender As Object, e As RoutedEventArgs)
		m_txtMasterPassword.IsEnabled = True
		m_cbPrinting.IsEnabled = True
		m_cbChangesAllowed.IsEnabled = True
		m_chkAllowContentAccessibility.IsEnabled = True
		m_chkAllowExtractContents.IsEnabled = True
		m_chkPDFEmbeddableFontsOnly.IsEnabled = False
	End Sub

	Private Sub ChkMasterPassword_Unchecked(sender As Object, e As RoutedEventArgs)
		m_txtMasterPassword.Password = ""
		m_txtMasterPassword.IsEnabled = False
		m_cbPrinting.IsEnabled = False
		m_cbChangesAllowed.IsEnabled = False
		m_chkAllowContentAccessibility.IsEnabled = False
		m_chkAllowExtractContents.IsEnabled = False
		m_chkPDFEmbeddableFontsOnly.IsEnabled = True
	End Sub

	Private Sub ChkPDFEmbeddableFontsOnly_Checked(sender As Object, e As RoutedEventArgs)
		m_grpPDFDocPwd.IsEnabled = False
		m_grpPDFPermissions.IsEnabled = False
	End Sub

	Private Sub ChkPDFEmbeddableFontsOnly_Unchecked(sender As Object, e As RoutedEventArgs)
		m_grpPDFDocPwd.IsEnabled = True
		m_grpPDFPermissions.IsEnabled = True
	End Sub

	Private Sub OptionsDialog_Loaded(sender As Object, e As RoutedEventArgs)
		m_txtStylesheetFile.Text = m_fileHandler.CssFileName

		Select Case m_fileHandler.CssSaveMode
			Case TXTextControl.CssSaveMode.None
				m_optNoStylesheet.IsChecked = True
				Exit Select

			Case TXTextControl.CssSaveMode.Inline
				m_optInlineStylesheet.IsChecked = True
				Exit Select

			Case TXTextControl.CssSaveMode.OverwriteFile
				m_optSaveStylesheetInSeperateFile.IsChecked = True
				Exit Select

			Case TXTextControl.CssSaveMode.CreateFile
				m_optSaveButDoNotOverwriteExistingFile.IsChecked = True
				Exit Select
		End Select

		m_txtUserPassword.Password = m_fileHandler.PDFUserPassword
		m_chkUserPassword.IsChecked = (m_fileHandler.PDFUserPassword.Length > 0)
		m_txtUserPassword.IsEnabled = m_chkUserPassword.IsChecked.Value

		m_txtMasterPassword.Password = m_fileHandler.PDFMasterPassword
		m_chkMasterPassword.IsChecked = (m_fileHandler.PDFMasterPassword.Length > 0)
		m_txtMasterPassword.IsEnabled = m_chkMasterPassword.IsChecked.Value

		m_txtCertFile.Text = m_fileHandler.PDFCertFilePath
		m_txtCertPwd.Password = m_fileHandler.PDFCertPasswd

		' PDF import combo box
		m_cbPDFImportMode.Items.Clear()
		m_cbPDFImportMode.Items.Add(My.Resources.OPT_DLG_PDFIMP_MODE_PLAIN)
		m_cbPDFImportMode.Items.Add(My.Resources.OPT_DLG_PDFIMP_MODE_PAR)
		m_cbPDFImportMode.Items.Add(My.Resources.OPT_DLG_PDFIMP_MODE_FRAMES)

		Select Case m_fileHandler.PDFImportSettings
			Case TXTextControl.PDFImportSettings.GenerateLines
				m_cbPDFImportMode.SelectedIndex = 0
				Exit Select

			Case TXTextControl.PDFImportSettings.GenerateParagraphs
				m_cbPDFImportMode.SelectedIndex = 1
				Exit Select

			Case TXTextControl.PDFImportSettings.GenerateTextFrames
				m_cbPDFImportMode.SelectedIndex = 2
				Exit Select
		End Select

		m_chkPDFEmbeddableFontsOnly.IsChecked = m_textControl.FontSettings.EmbeddableFontsOnly

		' Printing combo box
		m_cbPrinting.Items.Clear()
		m_cbPrinting.Items.Add(My.Resources.OPT_DLG_PDFSEC_PRNTMODE_NONE)
		m_cbPrinting.Items.Add(My.Resources.OPT_DLG_PDFSEC_PRNTMODE_LOWRES)
		m_cbPrinting.Items.Add(My.Resources.OPT_DLG_PDFSEC_PRNTMODE_HIRES)

		Dim flags As UInteger = CUInt(m_fileHandler.DocumentAccessPermissions)

		If (flags And CUInt(TXTextControl.DocumentAccessPermissions.AllowHighLevelPrinting)) <> 0 Then
			m_cbPrinting.SelectedIndex = 2
		ElseIf (flags And CUInt(TXTextControl.DocumentAccessPermissions.AllowLowLevelPrinting)) <> 0 Then
			m_cbPrinting.SelectedIndex = 1
		Else
			m_cbPrinting.SelectedIndex = 0
		End If

		m_cbPrinting.IsEnabled = m_chkMasterPassword.IsChecked.Value

		' Changes Allowed combo box
		m_cbChangesAllowed.Items.Clear()
		m_cbChangesAllowed.Items.Add(My.Resources.OPT_DLG_PDFSEC_CHNGALLOW_NONE)
		m_cbChangesAllowed.Items.Add(My.Resources.OPT_DLG_PDFSEC_CHNGALLOW_INSDEL)
		m_cbChangesAllowed.Items.Add(My.Resources.OPT_DLG_PDFSEC_CHNGALLOW_FORMFLDS)
		m_cbChangesAllowed.Items.Add(My.Resources.OPT_DLG_PDFSEC_CHNGALLOW_COMMENTS)
		m_cbChangesAllowed.Items.Add(My.Resources.OPT_DLG_PDFSEC_CHNGALLOW_ANY)

		If ((flags And CUInt(TXTextControl.DocumentAccessPermissions.AllowAuthoring)) <> 0) AndAlso ((flags And CUInt(TXTextControl.DocumentAccessPermissions.AllowDocumentAssembly)) <> 0) AndAlso ((flags And CUInt(TXTextControl.DocumentAccessPermissions.AllowGeneralEditing)) <> 0) Then
			m_cbChangesAllowed.SelectedIndex = 4
		ElseIf (flags And CUInt(TXTextControl.DocumentAccessPermissions.AllowAuthoring)) <> 0 Then
			m_cbChangesAllowed.SelectedIndex = 3
		ElseIf (flags And CUInt(TXTextControl.DocumentAccessPermissions.AllowAuthoringFields)) <> 0 Then
			m_cbChangesAllowed.SelectedIndex = 2
		ElseIf (flags And CUInt(TXTextControl.DocumentAccessPermissions.AllowDocumentAssembly)) <> 0 Then
			m_cbChangesAllowed.SelectedIndex = 1
		Else
			m_cbChangesAllowed.SelectedIndex = 0
		End If

		m_cbChangesAllowed.IsEnabled = m_chkMasterPassword.IsChecked.Value

		' Remaining checkboxes
		m_chkAllowContentAccessibility.IsChecked = ((flags And CUInt(TXTextControl.DocumentAccessPermissions.AllowContentAccessibility)) <> 0)
		m_chkAllowExtractContents.IsChecked = ((flags And CUInt(TXTextControl.DocumentAccessPermissions.AllowExtractContents)) <> 0)

		m_chkAllowContentAccessibility.IsEnabled = m_chkMasterPassword.IsChecked.Value
		m_chkAllowExtractContents.IsEnabled = m_chkMasterPassword.IsChecked.Value
	End Sub

#End Region

#End Region
End Class

#Region "  Extensions  "

'-------------------------------------------------------------------------------------------------------------
'	** Extensions used in this file.
'	**-----------------------------------------------------------------------------------------------------------

Module Extensions2

	'----------------------------------------------------------------------------------------------------------
	'		** Converts WinForms accelerators in a string to underscores.
	'		**--------------------------------------------------------------------------------------------------------

	<Extension()>
	Public Function ConvertAccelerators(text As String) As String
		Return Regex.Replace(text, "&(.?)", "_$1")
	End Function

End Module

#End Region
