'-------------------------------------------------------------------------------------------------------------
' module:        TX Text Control Words
'
' copyright:     © Text Control GmbH
' author:        T. Kummerow
'-----------------------------------------------------------------------------------------------------------

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Runtime.InteropServices
Imports System.Text

Namespace Win32

#Region "  Enumerations  "

	''' <summary>
	''' <see cref="ChooseColorInput"/> struct flags.
	''' </summary>
	<Flags> _
	Enum CC As UInteger
		RGBINIT = &H1
		FULLOPEN = &H2
		PREVENTFULLOPEN = &H4
		ENABLEHOOK = &H10
		ENABLETEMPLATE = &H20
		ENABLETEMPLATEHANDLE = &H40
		SOLIDCOLOR = &H80
		ANYCOLOR = &H100
	End Enum

	Enum WM As UInteger
		USER = &H400
	End Enum

	<Flags> _
	Enum WS As UInteger
		SYSMENU = &H80000
	End Enum

	Enum GWL
		STYLE = -16
	End Enum

	''' <summary>
	''' Constants for sending and receiving messages in BrowseCallBackProc
	''' </summary>
	Enum BFFM As UInteger
		INITIALIZED = 1
		SELCHANGED = 2
		VALIDATEFAILEDA = 3
		VALIDATEFAILEDW = 4
		IUNKNOWN = 5
		' provides IUnknown to client. lParam: IUnknown*
		SETSTATUSTEXTA = WM.USER + 100
		ENABLEOK = WM.USER + 101
		SETSELECTIONA = WM.USER + 102
		SETSELECTIONW = WM.USER + 103
		SETSTATUSTEXTW = WM.USER + 104
		SETOKTEXT = WM.USER + 105
		' Unicode only
		SETEXPANDED = WM.USER + 106
		' Unicode only
	End Enum

	' Browsing for directory.
	<Flags> _
	Enum BIF As UInteger
		RETURNONLYFSDIRS = &H1
		' For finding a folder to start document searching
		DONTGOBELOWDOMAIN = &H2
		' For starting the Find Computer
		STATUSTEXT = &H4
		' Top of the dialog has 2 lines of text for BROWSEINFO.lpszTitle and one line if
		' this flag is set.  Passing the message BFFM_SETSTATUSTEXTA to the hwnd can set the
		' rest of the text.  This is not used with BIF_USENEWUI and BROWSEINFO.lpszTitle gets
		' all three lines of text.
		RETURNFSANCESTORS = &H8
		EDITBOX = &H10
		' Add an editbox to the dialog
		VALIDATE = &H20
		' insist on valid result (or CANCEL)
		NEWDIALOGSTYLE = &H40
		' Use the new dialog layout with the ability to resize
		' Caller needs to call OleInitialize() before using this API
		USENEWUI = NEWDIALOGSTYLE Or EDITBOX

		BROWSEINCLUDEURLS = &H80
		' Allow URLs to be displayed or entered. (Requires BIF_USENEWUI)
		UAHINT = &H100
		' Add a UA hint to the dialog, in place of the edit box. May not be combined with BIF_EDITBOX
		NONEWFOLDERBUTTON = &H200
		' Do not add the "New Folder" button to the dialog.  Only applicable with BIF_NEWDIALOGSTYLE.
		NOTRANSLATETARGETS = &H400
		' don't traverse target as shortcut
		BROWSEFORCOMPUTER = &H1000
		' Browsing for Computers.
		BROWSEFORPRINTER = &H2000
		' Browsing for Printers
		BROWSEINCLUDEFILES = &H4000
		' Browsing for Everything
		SHAREABLE = &H8000
		' sharable resources displayed (remote shares, requires BIF_USENEWUI)
	End Enum

#End Region


#Region "  Structs  "

	'
	'   typedef struct {
	'      DWORD        lStructSize;
	'      HWND         hwndOwner;
	'      HWND         hInstance;
	'      COLORREF     rgbResult;
	'      COLORREF     *lpCustColors;
	'      DWORD        Flags;
	'      LPARAM       lCustData;
	'      LPCCHOOKPROC lpfnHook;
	'      LPCTSTR      lpTemplateName;
	'   } CHOOSECOLOR, *LPCHOOSECOLOR;
	'   

	''' <summary>
	''' Contains information the ChooseColor function uses to initialize 
	''' the Color dialog box. After the user closes the dialog box, the 
	''' system returns information about the user's selection in this 
	''' structure.
	''' </summary>
	<StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)> _
	Class ChooseColorInput
		Public structSize As Integer = Marshal.SizeOf(GetType(ChooseColorInput))
		Public dlgOwner As IntPtr = IntPtr.Zero
		Public instance As IntPtr = IntPtr.Zero
		Public rgbResult As Integer = 0
		Public custColors As IntPtr = IntPtr.Zero
		Public Flags As Integer = 0
		Public custData As IntPtr = IntPtr.Zero
		Public fnHook As IntPtr = IntPtr.Zero
		Public templateName As String = Nothing
	End Class

	Delegate Function BrowseCallBackProc(hwnd As IntPtr, msg As Integer, lp As IntPtr, wp As IntPtr) As Integer

	<StructLayout(LayoutKind.Sequential, CharSet:=CharSet.Auto)> _
	Class BrowseInfo
		Public dlgOwner As IntPtr = IntPtr.Zero
		Public pidlRoot As IntPtr = IntPtr.Zero
		Public displayName As String = Nothing
		Public title As String = Nothing
		Public Flags As UInteger = 0
		Public fn As BrowseCallBackProc = Nothing
		Public lParam As IntPtr = IntPtr.Zero
		Public iImage As Integer = 0
	End Class

#End Region


	''' <summary>
	''' Contains imported win32 functions
	''' </summary>
	NotInheritable Class PInvoke

		''' <summary>
		''' Creates a Color dialog box that enables the user to select a color.
		''' </summary>
		<DllImport("Comdlg32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
		Public Shared Function ChooseColor(<[In], Out> cc As ChooseColorInput) As Integer
		End Function

		<DllImport("shell32.dll", SetLastError:=True, CharSet:=CharSet.Auto)> _
		Public Shared Function SHBrowseForFolder(<[In], Out> bi As BrowseInfo) As IntPtr
		End Function

		' Note that the BROWSEINFO object's pszDisplayName only gives you the name of the folder.
		' To get the actual path, you need to parse the returned PIDL
		<DllImport("shell32.dll", CharSet:=CharSet.Auto)> _
		Public Shared Function SHGetPathFromIDList(pidl As IntPtr, pszPath As IntPtr) As Boolean
		End Function

		<DllImport("user32.dll", PreserveSig:=True, CharSet:=CharSet.Auto)> _
		Public Shared Function SendMessage(hWnd As HandleRef, Msg As UInteger, wParam As Long, lParam As IntPtr) As IntPtr
		End Function

		<DllImport("user32.dll", CharSet:=CharSet.Auto)> _
		Public Shared Function SendMessage(hWnd As HandleRef, msg As Integer, wParam As Integer, lParam As String) As IntPtr
		End Function

		<DllImport("user32.dll", SetLastError:=True)> _
		Public Shared Function GetWindowLong(hWnd As IntPtr, nIndex As Integer) As Integer
		End Function

		<DllImport("user32.dll")> _
		Public Shared Function SetWindowLong(hWnd As IntPtr, nIndex As Integer, dwNewLong As Integer) As Integer
		End Function

	End Class

End Namespace
