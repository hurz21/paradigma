'-------------------------------------------------------------------------------------------------------------
' module:     TX Text Control Words
'
' copyright:  © Text Control GmbH
' author:     T. Kummerow
'-----------------------------------------------------------------------------------------------------------

Imports System
Imports System.Diagnostics
Imports System.Drawing
Imports System.IO
Imports System.Reflection
Imports System.Windows
Imports System.Windows.Input
Imports TXTextControl.DocumentServer.Fields

Partial Public Class MainWindow

	Private Sub MnuItmFieldProps_Click(sender As Object, e As RoutedEventArgs)
		FieldSettings()
	End Sub

	Private Sub MnuItemDeleteField_Click(sender As Object, e As RoutedEventArgs)
		DeleteField()
	End Sub

	Private Sub DeleteField()
		Dim field = m_textControl.ApplicationFields.GetItem()
		If field Is Nothing Then
			Return
		End If
		m_textControl.ApplicationFields.Remove(field)
	End Sub

	Private Sub FieldSettings()
		Dim bRTL As Boolean = Me.FlowDirection = FlowDirection.RightToLeft
		Try
			Dim field = m_textControl.ApplicationFields.GetItem()

			Select Case field.TypeName
				Case MergeField.TYPE_NAME
					Dim mergeField__1 = New MergeField(field)
					mergeField__1.ShowDialog(Me, bRTL)
					Exit Select

				Case DateField.TYPE_NAME
					Dim dateField__2 = New DateField(field)
					dateField__2.ShowDialog(Me, bRTL)
					Exit Select

				Case IncludeText.TYPE_NAME
					Dim includeTextField = New IncludeText(field)
					includeTextField.ShowDialog(Me, bRTL)
					Exit Select

				Case IfField.TYPE_NAME
					Dim ifField__3 = New IfField(field)
					ifField__3.ShowDialog(Me, bRTL)
					Exit Select

				Case NextIfField.TYPE_NAME
					Dim nextIf = New NextIfField(field)
					nextIf.ShowDialog(Me, bRTL)
					Exit Select
			End Select
		Catch
		End Try
	End Sub
End Class
