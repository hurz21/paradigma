Imports System
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Data
Imports System.Globalization
Imports System.Linq
Imports System.Reflection
Imports System.Threading
Imports System.Windows


'-------------------------------------------------------------------------------------------------------
'	** Interaction logic for App.xaml
'	**-----------------------------------------------------------------------------------------------------

Partial Public Class App
	Inherits Application

	Private Sub Application_Startup(sender As Object, e As StartupEventArgs)
		' Show main window
		Dim mainWindow = New MainWindow()
		Application.Current.MainWindow = mainWindow
        mainWindow.Show()
        If mainWindow.nostart Then
            mainWindow.Close()
        End If
    End Sub

	Private Sub Application_DispatcherUnhandledException(sender As Object, e As System.Windows.Threading.DispatcherUnhandledExceptionEventArgs)
		' Catch unhandled exceptions and show a message box
		Dim strProductName As String = DirectCast(Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), GetType(AssemblyProductAttribute)), AssemblyProductAttribute).Product
		MessageBox.Show(e.Exception.Message, strProductName, MessageBoxButton.OK, MessageBoxImage.[Error])
		e.Handled = True
	End Sub
End Class
