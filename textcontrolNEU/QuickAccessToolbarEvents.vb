'-------------------------------------------------------------------------------------------------------------
'** module:        TX Text Control Words
'**
'** copyright:     © Text Control GmbH
'** author:        T. Kummerow
'**-----------------------------------------------------------------------------------------------------------

Imports Microsoft.Windows.Controls.Ribbon
Imports System
Imports System.Windows.Input

	'----------------------------------------------------------------------------------------------------------
'	** Event handlers for the quick access toolbar
'	**--------------------------------------------------------------------------------------------------------

Partial Class MainWindow

	Public Sub CanUndo(sender As [Object], e As CanExecuteRoutedEventArgs)
		If m_textControl Is Nothing Then
			Return
		End If
		e.CanExecute = m_textControl.CanUndo
	End Sub

	Public Sub CanRedo(sender As [Object], e As CanExecuteRoutedEventArgs)
		If m_textControl Is Nothing Then
			Return
		End If
		e.CanExecute = m_textControl.CanRedo
	End Sub

	Private Sub UndoHandler(sender As [Object], e As ExecutedRoutedEventArgs)
		m_textControl.Undo()
	End Sub

	Private Sub RedoHandler(sender As [Object], e As ExecutedRoutedEventArgs)
		m_textControl.Redo()
	End Sub
End Class
