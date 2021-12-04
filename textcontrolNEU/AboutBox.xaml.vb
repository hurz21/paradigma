'-------------------------------------------------------------------------------------------------------------
'** module:        TX Text Control Words
'** description:   This file contains the “About Box” form.
'**
'** copyright:     © Text Control GmbH
'** author:        T. Kummerow
'**-----------------------------------------------------------------------------------------------------------

Imports System
Imports System.Diagnostics
Imports System.Windows
Imports System.Windows.Navigation

''' <summary>
''' Interaction logic for AboutBox.xaml
''' </summary>
Partial Public Class AboutBox
	Inherits Window

	Public Overloads Shared Function Show(owner As Window) As System.Nullable(Of Boolean)
		Dim dlg = New AboutBox()
		dlg.Owner = owner
		Return dlg.ShowDialog()
	End Function

	Private Sub New()
		InitializeComponent()

		Title = [String].Format("About {0}", AssemblyAttributes.AssemblyProduct)
		_lblSubTitle.Content = (If(AssemblyAttributes.Is64BitAssembly, "64-bit", "32-bit")) & " Edition"
		_lblProductName.Content = MainWindow.ProductName
		_lblVersion.Content = [String].Format("Version {0}.{1}", AssemblyAttributes.AssemblyVersion.Major.ToString(), AssemblyAttributes.AssemblyVersion.Minor.ToString())
		_lblCopyright.Content = AssemblyAttributes.AssemblyCopyright
	End Sub

	Private Sub Hyperlink_RequestNavigate(sender As Object, e As RequestNavigateEventArgs)
		Process.Start(e.Uri.ToString())
		e.Handled = True
	End Sub

	Private Sub BtnClose_Click(sender As Object, e As RoutedEventArgs)
		Close()
	End Sub
End Class
