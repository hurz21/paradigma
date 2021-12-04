Imports Microsoft.Windows.Controls.Ribbon
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
''' Interaction logic for UserPromptDialog.xaml
''' </summary>
Partial Public Class UserPromptDialog
	Inherits Window

#Region "  Constructors  "

	Public Sub New(caption As String, label As String, value As String)
		InitializeComponent()
		LocalizeDialog()

		m_lblInput.Content = If(label, "")
		m_txtInput.Text = If(value, "")
	End Sub

#End Region


#Region "  Methods  "

	Private Sub LocalizeDialog()
		m_btnOK.Content = My.Resources.BTN_OK.ConvertAccelerators()
		m_btnCancel.Content = My.Resources.BTN_CANCEL.ConvertAccelerators()
	End Sub

#End Region


#Region "  Properties  "

	Public Property IsPassword() As Boolean
		Get
			Return m_passwordBox.Visibility = Visibility.Visible
		End Get
		Set(value As Boolean)
			If Value Then
				m_passwordBox.Visibility = Visibility.Visible
				m_passwordBox.Password = m_txtInput.Text
				m_txtInput.Visibility = Visibility.Collapsed
			Else
				m_txtInput.Visibility = Visibility.Visible
				m_txtInput.Text = m_passwordBox.Password
				m_passwordBox.Visibility = Visibility.Collapsed
			End If
		End Set
	End Property

	Public ReadOnly Property Value() As String
		Get
			Return If(IsPassword, m_passwordBox.Password, m_txtInput.Text)
		End Get
	End Property

#End Region


#Region "  Event Handlers  "

	Private Sub BtnOK_Click(sender As Object, e As RoutedEventArgs)
		Me.DialogResult = True
		Me.Close()
	End Sub

#End Region
End Class
