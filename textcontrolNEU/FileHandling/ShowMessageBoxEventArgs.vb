'-------------------------------------------------------------------------------------------------------------
'** module:        TX Text Control Words
'**
'** copyright:     © Text Control GmbH
'** author:        T. Kummerow
'**-----------------------------------------------------------------------------------------------------------

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports System.ComponentModel

Namespace FileHandling

#Region "  Enumerations  "

	Public Enum MessageBoxButton
		OK
		OKCancel
		AbortRetryIgnore
		YesNoCancel
		YesNo
		RetryCancel
	End Enum

	Public Enum MessageBoxIcon
		None
		[Error]
		Question
		Exclamation
		Information
	End Enum

	Public Enum DialogResult
		OK
		Cancel
		Yes
		No
	End Enum

#End Region

	Public Class ShowMessageBoxEventArgs
		Inherits EventArgs

#Region "  Constructors  "

		Public Sub New(text__1 As String, caption__2 As String, button__3 As MessageBoxButton, icon__4 As MessageBoxIcon)
			Text = text__1
			Caption = caption__2
			Button = button__3
			Icon = icon__4
			Me.DialogResult = DialogResult.Cancel
		End Sub

		Public Sub New(text As String)
			Me.New(text, Nothing, MessageBoxButton.OK, MessageBoxIcon.None)
		End Sub

		Public Sub New(text As String, caption As String)
			Me.New(text, caption, MessageBoxButton.OK, MessageBoxIcon.None)
		End Sub

		Public Sub New(text As String, button As MessageBoxButton)
			Me.New(text, Nothing, button, MessageBoxIcon.None)
		End Sub

		Public Sub New(text As String, button As MessageBoxButton, icon As MessageBoxIcon)
			Me.New(text, Nothing, button, icon)
		End Sub

#End Region


#Region "  Public Properties  "

		Public Property DialogResult() As DialogResult
			Get
				Return m_DialogResult
			End Get
			Set(value As DialogResult)
				m_DialogResult = value
			End Set
		End Property
		Private m_DialogResult As DialogResult

		'-------------------------------------------------------------------------------------------------------
'		** The MessageBox button type.
'		**-----------------------------------------------------------------------------------------------------

		Public Property Button() As MessageBoxButton
			Get
				Return m_Button
			End Get
			Private Set(value As MessageBoxButton)
				m_Button = value
			End Set
		End Property
		Private m_Button As MessageBoxButton

		'-------------------------------------------------------------------------------------------------------
'		** The MessageBox icon type.
'		**-----------------------------------------------------------------------------------------------------

		Public Property Icon() As MessageBoxIcon
			Get
				Return m_Icon
			End Get
			Private Set(value As MessageBoxIcon)
				m_Icon = value
			End Set
		End Property
		Private m_Icon As MessageBoxIcon

		'-------------------------------------------------------------------------------------------------------
'		** The MessageBox text.
'		**-----------------------------------------------------------------------------------------------------

		Public Property Text() As String
			Get
				Return m_Text
			End Get
			Private Set(value As String)
				m_Text = value
			End Set
		End Property
		Private m_Text As String

		'-------------------------------------------------------------------------------------------------------
'		** The MessageBox caption. Is null if not set.
'		**-----------------------------------------------------------------------------------------------------

		Public Property Caption() As String
			Get
				Return m_Caption
			End Get
			Private Set(value As String)
				m_Caption = value
			End Set
		End Property
		Private m_Caption As String

#End Region
	End Class
End Namespace
