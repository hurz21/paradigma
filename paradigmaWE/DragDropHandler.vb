'-------------------------------------------------------------------------------------------------------------
'** module:        TX Text Control Words
'** description:   This file contains a class handling file drag & drop operations.
'**
'** copyright:     © Text Control GmbH
'** author:        T. Kummerow
'**-----------------------------------------------------------------------------------------------------------

Imports System
Imports System.IO
Imports System.Windows

	Public Enum ImageType
		UNKNOWN

		bmp
		jpg
		gif
		tif
		png
		emf
		wmf
	End Enum

	'----------------------------------------------------------------------------------------------------------
'	** Handles file drag &amp; drop operations.
'	**--------------------------------------------------------------------------------------------------------

	Class FileDragDropHandler

		#Region "  Enumerations  "

		Public Enum DraggedFileType
			UNKNOWN
			Document
			Image
		End Enum

		#End Region

		#Region "  Class Properties  "

		'-------------------------------------------------------------------------------------------------------
'		** Dragged / dropped file type. (Image, document etc.)
'		**-----------------------------------------------------------------------------------------------------

		Public Property FileType() As DraggedFileType
			Get
				Return m_FileType
			End Get
			Private Set
				m_FileType = Value
			End Set
		End Property
		Private m_FileType As DraggedFileType

		'-------------------------------------------------------------------------------------------------------
'		** Dragged image type.
'		**-----------------------------------------------------------------------------------------------------

		Public Property ImageType() As ImageType
			Get
				Return m_ImageType
			End Get
			Private Set
				m_ImageType = Value
			End Set
		End Property
		Private m_ImageType As ImageType

		' Gets the name of the file handled through this drag&drop handler:
		Public Property FileName() As String
			Get
				Return m_FileName
			End Get
			Private Set
				m_FileName = Value
			End Set
		End Property
		Private m_FileName As String

		' Gets the TXTextControl Streamtype of the file handled through this drag&drop handler:
		Public Property StreamType() As TXTextControl.StreamType
			Get
				Return m_StreamType
			End Get
			Private Set
				m_StreamType = Value
			End Set
		End Property
		Private m_StreamType As TXTextControl.StreamType

		' Gets a value indicating whether something can be dropped:
		Public Property CanDrop() As Boolean
			Get
				Return m_CanDrop
			End Get
			Private Set
				m_CanDrop = Value
			End Set
		End Property
		Private m_CanDrop As Boolean

		#End Region

		#Region "  Methods  "

		' Resets the internal state of the drag & drop handler:
		Public Sub Reset()
			FileName = String.Empty
			StreamType = 0
			CanDrop = False
			FileType = DraggedFileType.UNKNOWN
			ImageType = ImageType.UNKNOWN
		End Sub

		Public Sub CheckDraggedFiles(fileList As String())
			Reset()

			If fileList IsNot Nothing Then
				' Get first parameter from the list and check if it is a supported file type
				FileName = fileList(0)
            'MsgBox("2 " & "" & ", " & FileName)
            Select Case Path.GetExtension(FileName).ToLower()
					Case ".rtf"
						FileType = DraggedFileType.Document
						StreamType = TXTextControl.StreamType.RichTextFormat
						Exit Select

					Case ".htm", ".html"
						FileType = DraggedFileType.Document
						StreamType = TXTextControl.StreamType.HTMLFormat
						Exit Select

					Case ".doc"
						FileType = DraggedFileType.Document
						StreamType = TXTextControl.StreamType.MSWord
						Exit Select

					Case ".docx"
						FileType = DraggedFileType.Document
						StreamType = TXTextControl.StreamType.WordprocessingML
						Exit Select

					Case ".pdf"
						FileType = DraggedFileType.Document
						StreamType = TXTextControl.StreamType.AdobePDF
						Exit Select

					Case ".xml"
						FileType = DraggedFileType.Document
						StreamType = TXTextControl.StreamType.XMLFormat
						Exit Select

					Case ".txt"
						FileType = DraggedFileType.Document
						StreamType = TXTextControl.StreamType.PlainText
						Exit Select

					Case ".tx"
						FileType = DraggedFileType.Document
						StreamType = TXTextControl.StreamType.InternalUnicodeFormat
						Exit Select

					Case ".jpeg", ".jpg"
						FileType = DraggedFileType.Image
						ImageType = ImageType.jpg
						Exit Select

					Case ".tif"
						FileType = DraggedFileType.Image
						ImageType = ImageType.tif
						Exit Select

					Case ".bmp"
						FileType = DraggedFileType.Image
						ImageType = ImageType.bmp
						Exit Select

					Case ".gif"
						FileType = DraggedFileType.Image
						ImageType = ImageType.gif
						Exit Select

					Case ".png"
						FileType = DraggedFileType.Image
						ImageType = ImageType.png
						Exit Select

					Case ".wmf"
						FileType = DraggedFileType.Image
						ImageType = ImageType.wmf
						Exit Select

					Case ".emf"
						FileType = DraggedFileType.Image
						ImageType = ImageType.emf
						Exit Select
					Case Else
                    'MsgBox("3 " & "UNKNOWN" & ", " & FileName)
                    FileType = DraggedFileType.UNKNOWN
						ImageType = ImageType.UNKNOWN
						FileName = [String].Empty
						Exit Select
				End Select

				If FileType <> DraggedFileType.UNKNOWN Then
					CanDrop = True
				End If
			End If
		End Sub

		' Calculates a drag&drop effect depending on the allowed effects:
		Public Function GetDragDropEffect(allowedEffects As DragDropEffects) As DragDropEffects
			If (allowedEffects And DragDropEffects.Copy) = DragDropEffects.Copy Then
				Return DragDropEffects.Copy
			ElseIf (allowedEffects And DragDropEffects.Move) = DragDropEffects.Move Then
				Return DragDropEffects.Move
			Else
				Return DragDropEffects.None
			End If
		End Function

		#End Region

	End Class

