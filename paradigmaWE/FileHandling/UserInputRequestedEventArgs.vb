'-------------------------------------------------------------------------------------------------------------
' module:        TX Text Control Words
'
' copyright:     © Text Control GmbH
' author:        T. Kummerow
'-----------------------------------------------------------------------------------------------------------

Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Linq
Imports System.Text

Namespace FileHandling

#Region "  Enumerations  "

	Public Enum UserInputRequestReason
		Unknown
		PdfUserPassword
		PdfMasterPassword
	End Enum

#End Region

	Public Class UserInputRequestedEventArgs
		Inherits EventArgs

		Public Sub New(current As String, caption__1 As String, label__2 As String, Optional isPasswordRequest__3 As Boolean = False, Optional reason As UserInputRequestReason = UserInputRequestReason.Unknown)

			Value = current
			Caption = caption__1
			Label = label__2
			IsPasswordRequest = isPasswordRequest__3
			DialogResult = FileHandling.DialogResult.Cancel
		End Sub

		Public Property Value() As String
			Get
				Return m_Value
			End Get
			Set(value As String)
				m_Value = Value
			End Set
		End Property
		Private m_Value As String
		Public Property Caption() As String
			Get
				Return m_Caption
			End Get
			Private Set(value As String)
				m_Caption = Value
			End Set
		End Property
		Private m_Caption As String
		Public Property Label() As String
			Get
				Return m_Label
			End Get
			Private Set(value As String)
				m_Label = Value
			End Set
		End Property
		Private m_Label As String
		Public Property IsPasswordRequest() As Boolean
			Get
				Return m_IsPasswordRequest
			End Get
			Private Set(value As Boolean)
				m_IsPasswordRequest = Value
			End Set
		End Property
		Private m_IsPasswordRequest As Boolean
		Public Property Reason() As UserInputRequestReason
			Get
				Return m_Reason
			End Get
			Private Set(value As UserInputRequestReason)
				m_Reason = Value
			End Set
		End Property
		Private m_Reason As UserInputRequestReason
		Public Property DialogResult() As FileHandling.DialogResult
			Get
				Return m_DialogResult
			End Get
			Set(value As FileHandling.DialogResult)
				m_DialogResult = Value
			End Set
		End Property
		Private m_DialogResult As FileHandling.DialogResult
	End Class
End Namespace
