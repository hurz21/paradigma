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
Imports System.Windows.Navigation
Imports System.Windows.Shapes

''' <summary>
''' Interaction logic for MainWindow.xaml
''' </summary>
Partial Public Class MainWindow
	Inherits RibbonWindow

#Region "  Private Fields  "

	Private m_fileHandler As FileHandling.FileHandler
	Private m_dragDropHandler As FileDragDropHandler
    Public Property jfDateityp As String = ".docx"
#End Region


#Region "  Constructors  "

    Public Sub New()
		InitializeComponent()
		InitializeAppMenu()

		' File handling
		m_fileHandler = New FileHandling.FileHandler(m_textControl)
		m_fileHandler.MaxRecentFiles = My.Settings.RecentFilesMaxItemCount
		AddHandler m_fileHandler.ShowMessageBox, AddressOf FileHandler_ShowMessageBox
		AddHandler m_fileHandler.DocumentDirtyChanged, AddressOf FileHandler_DocumentDirtyChanged
		AddHandler m_fileHandler.DocumentFileNameChanged, AddressOf FileHandler_DocumentFileNameChanged
		AddHandler m_fileHandler.RecentFileListChanged, AddressOf FileHandler_RecentFileListChanged
		AddHandler m_fileHandler.UserInputRequested, AddressOf FileHandler_UserInputRequested
		AddHandler m_fileHandler.DocumentAccessPermissionsChanged, AddressOf FileHandler_DocumentAccessPermissionsChanged

		' Drag drop handling
		m_dragDropHandler = New FileDragDropHandler()

		' Set ruler- and statusbar background colors
		Dim col = Color.FromArgb(255, 245, 246, 247)
		m_rulerBarHor.DisplayColors.GradientBackColor = col
		m_rulerBarHor.DisplayColors.BackColor = col
		m_rulerBarVert.DisplayColors.GradientBackColor = col
		m_rulerBarVert.DisplayColors.BackColor = col
		SetStatusBarColor(Color.FromArgb(255, 43, 86, 154))
	End Sub

#End Region


#Region "  Properties  "

	Friend Shared ReadOnly Property ProductName() As String
		Get
			Return AssemblyAttributes.AssemblyProduct
		End Get
	End Property

#End Region


#Region "  Methods  "

	'-------------------------------------------------------------------------------------------------------
	' Loads application and user settings.
	'-----------------------------------------------------------------------------------------------------

	Private Sub LoadAppSettings()
		' Take over initial resizing
		Me.WindowStartupLocation = WindowStartupLocation.Manual

		' Resize form
		Me.Width = My.Settings.LastWindowSize.Width
		Me.Height = My.Settings.LastWindowSize.Height
		Me.Left = My.Settings.LastWindowPos.X
		Me.Top = My.Settings.LastWindowPos.Y
		Me.WindowState = My.Settings.LastWindowState

		' Recent items
		m_fileHandler.RecentFiles = My.Settings.RecentFiles
	End Sub

	'-------------------------------------------------------------------------------------------------------
	' Saves application and user settings.
	'-----------------------------------------------------------------------------------------------------

	Private Sub SaveAppSettings()
		My.Settings.LastWindowPos = New System.Windows.Point(Me.RestoreBounds.Left, Me.RestoreBounds.Top)
		My.Settings.LastWindowSize = New System.Windows.Size(Me.RestoreBounds.Width, Me.RestoreBounds.Height)
		My.Settings.RecentFiles = m_fileHandler.RecentFiles
		My.Settings.Save()
	End Sub

	'-------------------------------------------------------------------------------------------------------
	' Sets the window title according to the current document file name
	' and the "dirty" state of the document.
	'-----------------------------------------------------------------------------------------------------

	Private Sub SetWindowTitle(documentTitle As String, Optional isDocumentDirty As Boolean = False)
		Dim asterisk As String = If(isDocumentDirty, "*", "")
		Dim title__1 As String = String.Format("{0}{1} - {2}", documentTitle, asterisk, ProductName)
        Title = title__1
        Title = "Paradigma-WordEditor, Vorgang:" & m_fileHandler.jfVorgangsid & ", " & m_fileHandler.jfVorgangsTitel & " (" & m_fileHandler.jfDokumentId & ")"
    End Sub

	Private Sub SetStatusBarColor(col As Color)
		m_statusBar.DisplayColors.BackColorBottom = col
		m_statusBar.DisplayColors.BackColorMiddle = col
		m_statusBar.DisplayColors.BackColorTop = col
		m_statusBar.DisplayColors.FrameColor = col
		m_statusBar.DisplayColors.SeparatorColorLight = col
		m_statusBar.DisplayColors.ForeColor = Colors.White
		m_statusBar.DisplayColors.SeparatorColorDark = Colors.White
	End Sub

#End Region
End Class
