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
Imports System.Windows.Interop
Imports System.Windows.Media
Imports System.Windows.Media.Imaging
Imports System.Windows.Shapes
Imports TX_Text_Control_Words.Win32

''' <summary>
''' Interaction logic for MergeWaitDialog.xaml
''' </summary>
Partial Friend Class MergeWaitDialog
	Inherits Window

	Private m_bMayClose As Boolean = False

	Public Sub New()
		InitializeComponent()
	End Sub

	Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
		' Remove close button
		Dim hwnd = (New WindowInteropHelper(Me)).Handle
		PInvoke.SetWindowLong(hwnd, CInt(GWL.STYLE), CInt(PInvoke.GetWindowLong(hwnd, CInt(GWL.STYLE)) And Not CInt(WS.SYSMENU)))
	End Sub

	Public Sub CloseDialog()
		m_bMayClose = True
		Close()
	End Sub

	Private Sub Window_Closing(sender As Object, e As System.ComponentModel.CancelEventArgs)
		If Not m_bMayClose Then
			e.Cancel = True
		Else
			m_bMayClose = False
		End If
	End Sub
End Class
