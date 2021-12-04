'-------------------------------------------------------------------------------------------------------------
'** module:        TX Text Control Words
'**
'** copyright:     © Text Control GmbH
'** author:        T. Kummerow
'**-----------------------------------------------------------------------------------------------------------

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Input
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Shapes

''' <summary>
''' Interaction logic for MaxPrevRowsDialog.xaml
''' </summary>
Partial Public Class MaxPrevRowsDialog
	Inherits Window

	Private Sub New()
		InitializeComponent()
	End Sub

	Friend Shared Shadows Function Show(owner As Window) As Integer
		Dim dlg = New MaxPrevRowsDialog()
		dlg.FlowDirection = owner.FlowDirection
		dlg.Owner = owner
		If dlg.ShowDialog() = True Then
			Dim value As Integer = CInt(dlg._spinnerMax.Value)
			My.Settings.MergePreviewMaxRows = value
			Return value
		End If

		Return 0
	End Function

	Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
		_spinnerMax.Value = My.Settings.MergePreviewMaxRows
	End Sub

	Private Sub BtnOK_Click(sender As Object, e As RoutedEventArgs)
		DialogResult = True
		Close()
	End Sub
End Class
