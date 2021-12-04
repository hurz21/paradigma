'-------------------------------------------------------------------------------------------------------------
'** module:        TX Text Control Words
'**
'** copyright:     © Text Control GmbH
'** author:        T. Kummerow
'**-----------------------------------------------------------------------------------------------------------

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Drawing
Imports System.Text
Imports System.Windows
Imports System.Runtime.CompilerServices
Imports TXTextControl

Partial Public Class MainWindow

	'-------------------------------------------------------------------------------------------------------
	'		** Handles the "ShowMessageBox" event of the file handler.
	'		**-----------------------------------------------------------------------------------------------------

	Private Sub FileHandler_ShowMessageBox(sender As Object, e As FileHandling.ShowMessageBoxEventArgs)
		Dim caption As String = If(e.Caption, ProductName)
		Dim res As MessageBoxResult = MessageBox.Show(Me, e.Text, caption, e.Button.ToWpfButton(), e.Icon.ToWpfImage())
		e.DialogResult = res.ToFileHandlerDialogResult()
	End Sub

	'-------------------------------------------------------------------------------------------------------
	'		** Handles the "DocumentDirtyChanged" event of the file handler.
	'		**-----------------------------------------------------------------------------------------------------

	Private Sub FileHandler_DocumentDirtyChanged(sender As Object, e As FileHandling.DocumentDirtyChangedEventArgs)
		SetWindowTitle(m_fileHandler.DocumentTitle, e.NewValue)
	End Sub

	'-------------------------------------------------------------------------------------------------------
	'		** Handles the "DocumentFileNameChanged" event of the file handler.
	'		**-----------------------------------------------------------------------------------------------------

	Private Sub FileHandler_DocumentFileNameChanged(sender As Object, e As FileHandling.DocumentFileNameChangedEventArgs)
		SetWindowTitle(m_fileHandler.DocumentTitle, m_fileHandler.IsDocumentDirty)
	End Sub

	'-------------------------------------------------------------------------------------------------------
	'		** Handles the "RecentFileListChanged" event of the file handler.
	'		**-----------------------------------------------------------------------------------------------------

	Private Sub FileHandler_RecentFileListChanged(sender As Object, e As EventArgs)
		SetRecentItemsList(m_fileHandler.RecentFiles)
	End Sub

	'-------------------------------------------------------------------------------------------------------
	'		** Handles the "UserInputRequested" event of the file handler.
	'		**-----------------------------------------------------------------------------------------------------

	Private Sub FileHandler_UserInputRequested(sender As Object, e As FileHandling.UserInputRequestedEventArgs)
		Dim dlg = New UserPromptDialog(e.Caption, e.Label, e.Value)
		dlg.FlowDirection = Me.FlowDirection
		dlg.IsPassword = e.IsPasswordRequest
		dlg.Owner = Me
		If dlg.ShowDialog() = True Then
			e.DialogResult = FileHandling.DialogResult.OK
			e.Value = dlg.Value
		End If
	End Sub
	'-------------------------------------------------------------------------------------------------------
	' Handles the "DocumentAccessPermissionsChanged" event of the file handler.
	'-----------------------------------------------------------------------------------------------------

	Private Sub FileHandler_DocumentAccessPermissionsChanged(sender As Object, e As EventArgs)
		' Enable / Disable print buttons / menu items
		If (m_fileHandler.DocumentAccessPermissions And (DocumentAccessPermissions.AllowHighLevelPrinting Or DocumentAccessPermissions.AllowLowLevelPrinting)) <> 0 Then
			m_btnPrint.IsEnabled = True
            'm_appSpltMnuItmPrint.IsEnabled = True
        Else
			m_btnPrint.IsEnabled = True
            'm_appSpltMnuItmPrint.IsEnabled = False
        End If

		If (m_fileHandler.DocumentAccessPermissions And DocumentAccessPermissions.AllowGeneralEditing) <> 0 Then
			m_textControl.EditMode = EditMode.Edit
		ElseIf (m_fileHandler.DocumentAccessPermissions And DocumentAccessPermissions.AllowExtractContents) <> 0 Then
			m_textControl.EditMode = EditMode.ReadAndSelect
		Else
			m_textControl.EditMode = EditMode.[ReadOnly]
		End If

		' ToDo: react to other permission settings?
	End Sub

End Class


'-------------------------------------------------------------------------------------------------------
'	** Extension methods used in this file.
'	**-----------------------------------------------------------------------------------------------------

Module Extensions

	'-------------------------------------------------------------------------------------------------------
	'		** Converts UI-agnostic message box button type to Windows Forms value.
	'		**-----------------------------------------------------------------------------------------------------

	<Extension()>
	Public Function ToWpfButton(button As FileHandling.MessageBoxButton) As MessageBoxButton
		Select Case button
			Case FileHandling.MessageBoxButton.OKCancel
				Return MessageBoxButton.OKCancel

			Case FileHandling.MessageBoxButton.AbortRetryIgnore
				Return MessageBoxButton.YesNoCancel

			Case FileHandling.MessageBoxButton.YesNoCancel
				Return MessageBoxButton.YesNoCancel

			Case FileHandling.MessageBoxButton.YesNo
				Return MessageBoxButton.YesNo

			Case FileHandling.MessageBoxButton.RetryCancel
				Return MessageBoxButton.OKCancel
		End Select
		Return MessageBoxButton.OK
	End Function

	'-------------------------------------------------------------------------------------------------------
	'		** Converts UI-agnostic message box icon type to Windows Forms value.
	'		**-----------------------------------------------------------------------------------------------------

	<Extension()>
	Public Function ToWpfImage(icon As FileHandling.MessageBoxIcon) As MessageBoxImage
		Select Case icon
			Case FileHandling.MessageBoxIcon.[Error]
				Return MessageBoxImage.[Error]

			Case FileHandling.MessageBoxIcon.Question
				Return MessageBoxImage.Question

			Case FileHandling.MessageBoxIcon.Exclamation
				Return MessageBoxImage.Exclamation

			Case FileHandling.MessageBoxIcon.Information
				Return MessageBoxImage.Information
		End Select

		Return MessageBoxImage.None
	End Function

	'-------------------------------------------------------------------------------------------------------
	'		** Converts a Windows Forms dialog result to the UI-agnostic equivalent.
	'		**-----------------------------------------------------------------------------------------------------

	<Extension()>
	Public Function ToFileHandlerDialogResult(res As MessageBoxResult) As FileHandling.DialogResult
		Select Case res
			Case MessageBoxResult.Cancel, MessageBoxResult.None
				Return FileHandling.DialogResult.Cancel

			Case MessageBoxResult.No
				Return FileHandling.DialogResult.No

			Case MessageBoxResult.Yes
				Return FileHandling.DialogResult.Yes
		End Select

		Return FileHandling.DialogResult.OK
	End Function
End Module
