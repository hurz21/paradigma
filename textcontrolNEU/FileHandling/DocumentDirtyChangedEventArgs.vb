'-------------------------------------------------------------------------------------------------------------
'** module:        TX Text Control Words
'**
'** copyright:     © Text Control GmbH
'** author:        T. Kummerow
'**-----------------------------------------------------------------------------------------------------------

Imports System
Imports System.Collections.Generic
Imports System.Text

Namespace FileHandling

	Public Class DocumentDirtyChangedEventArgs
		Inherits EventArgs

		Public Sub New(newValue__1 As Boolean)
			NewValue = newValue__1
		End Sub

		Public Property NewValue() As Boolean
			Get
				Return m_NewValue
			End Get
			Private Set(value As Boolean)
				m_NewValue = value
			End Set
		End Property
		Private m_NewValue As Boolean
	End Class
End Namespace
