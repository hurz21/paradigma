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

Namespace FileHandling

	Public Class DocumentFileNameChangedEventArgs
		Inherits EventArgs

		Public Sub New(newName__1 As String)
			NewName = newName__1
		End Sub

		Public Property NewName() As String
			Get
				Return m_NewName
			End Get
			Private Set(value As String)
				m_NewName = value
			End Set
		End Property
		Private m_NewName As String
	End Class
End Namespace
