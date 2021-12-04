'-------------------------------------------------------------------------------------------------------------
' module:        TX Text Control Words
'
' copyright:     © Text Control GmbH
' author:        T. Kummerow
'-----------------------------------------------------------------------------------------------------------

Imports System
Imports System.Collections.Generic
Imports System.Linq
Imports System.Text
Imports System.Runtime.InteropServices
Imports System.Windows
Imports System.Windows.Interop

Namespace Win32

	NotInheritable Class FolderBrowserDialog
		Private Sub New()
		End Sub

		Private Shared _initialPath As String

		Private Shared Function OnBrowseEvent(hWnd As IntPtr, msg As Integer, lp As IntPtr, lpData As IntPtr) As Integer
			Dim pathPtr As IntPtr

			Select Case CType(msg, BFFM)
				Case BFFM.INITIALIZED
					' Required to set initialPath               
					' Use BFFM_SETSELECTIONW if passing a Unicode string, i.e. native CLR Strings.
					PInvoke.SendMessage(New HandleRef(Nothing, hWnd), CInt(BFFM.SETSELECTIONW), 1, _initialPath)
					Exit Select

				Case BFFM.SELCHANGED
					pathPtr = Marshal.AllocHGlobal(CInt(1024 * Marshal.SystemDefaultCharSize))
					If PInvoke.SHGetPathFromIDList(lp, pathPtr) Then
						PInvoke.SendMessage(New HandleRef(Nothing, hWnd), CUInt(BFFM.SETSTATUSTEXTW), 0, pathPtr)
					End If
					Marshal.FreeHGlobal(pathPtr)
					Exit Select
			End Select

			Return 0
		End Function

		Public Shared Function ShowDialog(caption As String, initialPath As String, owner As Window) As String
			_initialPath = initialPath
			Dim sb = New StringBuilder()
			Dim bufferAddress As IntPtr = Marshal.AllocHGlobal(1024)
			Dim pidl As IntPtr = IntPtr.Zero
			Dim wih = New WindowInteropHelper(owner)

			Dim bi = New BrowseInfo()
			bi.dlgOwner = wih.Handle
			bi.displayName = initialPath
			bi.title = caption
			bi.Flags = CUInt(BIF.NEWDIALOGSTYLE Or BIF.SHAREABLE)
			bi.fn = New BrowseCallBackProc(AddressOf OnBrowseEvent)

			Try
				pidl = PInvoke.SHBrowseForFolder(bi)
				If Not PInvoke.SHGetPathFromIDList(pidl, bufferAddress) Then
					Return Nothing
				End If
				sb.Append(Marshal.PtrToStringAuto(bufferAddress))
			Finally
				' Caller is responsible for freeing this memory.
				Marshal.FreeCoTaskMem(pidl)
			End Try

			Marshal.FreeHGlobal(bufferAddress)
			Return sb.ToString()
		End Function
	End Class
End Namespace
