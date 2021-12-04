'-------------------------------------------------------------------------------------------------------------
' module:        TX Text Control Words
'
' copyright:     © Text Control GmbH
' author:        T. Kummerow
'-----------------------------------------------------------------------------------------------------------

Imports System
Imports System.Collections.Generic
Imports System.Text
Imports TXTextControl
Imports TXTextControl.WPF
Imports System.Security.Cryptography.X509Certificates
Imports System.Collections.Specialized
Imports System.IO
Imports System.Runtime.CompilerServices

Namespace FileHandling

    '----------------------------------------------------------------------------------------------------------
    ' Encapsulates file handling tasks.
    '--------------------------------------------------------------------------------------------------------

    Public Class FileHandler
        Public Property jfDateityp As String = ".docx"
        Public Property jfVorgangsid As Integer = 0
        Public Property jfVorgangsTitel As String = "JFtitel"
        Public Property jfDokumentId As Integer = 0
        Public Property jfLocalLocksRoot As String = "O:\UMWELT-PARADIGMA\div\user\textcontrolLocks"


#Region "  Private Fields  "

        Private m_textControl As TextControl
        Private m_isDocumentDirty As Boolean = False
        Private m_documentFileName As String = ""
        Private m_recentFiles As StringCollection
        Private m_maxRecentFiles As Integer = 10
        Private m_streamType As StreamType = StreamType.InternalUnicodeFormat
        Private m_docAccPerms As DocumentAccessPermissions
        Private m_pdfUserPwd As String = ""

        Private Const DefaultSaveTypes As StreamType = (StreamType.All And Not (StreamType.XMLFormat Or StreamType.CascadingStylesheet Or StreamType.PlainAnsiText))
        Private Const DefaultLoadTypes As StreamType = StreamType.All And Not StreamType.CascadingStylesheet

#End Region


#Region "  Constructors  "

        '-------------------------------------------------------------------------------------------------------
        ' Constructor.
        '-----------------------------------------------------------------------------------------------------

        Public Sub New(textControl As TextControl)
            m_textControl = textControl
            CssFileName = ""
            CssSaveMode = TXTextControl.CssSaveMode.None
            DocumentFileName = ""
            PDFCertFilePath = ""
            PDFCertPasswd = ""
            PDFUserPassword = ""
            PDFMasterPassword = ""
            m_recentFiles = New StringCollection()
            DocumentAccessPermissions = TXTextControl.DocumentAccessPermissions.AllowAll
            PDFImportSettings = TXTextControl.PDFImportSettings.GenerateTextFrames
        End Sub

#End Region


#Region "  Events  "

        Public Event ShowMessageBox As EventHandler(Of ShowMessageBoxEventArgs)
        Public Event DocumentDirtyChanged As EventHandler(Of DocumentDirtyChangedEventArgs)
        Public Event DocumentFileNameChanged As EventHandler(Of DocumentFileNameChangedEventArgs)
        Public Event RecentFileListChanged As EventHandler
        Public Event UserInputRequested As EventHandler(Of UserInputRequestedEventArgs)
        Public Event DocumentAccessPermissionsChanged As EventHandler

#End Region


#Region "  Public Methods  "

        '-------------------------------------------------------------------------------------------------------
        ' Use this method to open a file from the list of recent files.
        '-----------------------------------------------------------------------------------------------------

        Public Sub OpenRecentFile(fileName As String)
            ' Check if file exists
            If File.Exists(fileName) Then
                Open(fileName)
            Else
                Dim e = New ShowMessageBoxEventArgs(My.Resources.MSG_REMOVE_FILE_FROM_LIST, MessageBoxButton.YesNo, MessageBoxIcon.Question)
                OnShowMessageBox(e)
                If e.DialogResult = DialogResult.Yes Then
                    RemoveRecentFile(fileName)
                End If
            End If
        End Sub

        '-------------------------------------------------------------------------------------------------------
        ' Opens a file or shows a file open dialog.
        '-----------------------------------------------------------------------------------------------------

        Public Sub Open(Optional fileName As String = "", Optional bKeepPdfSecSettings As Boolean = False)
            If Not bKeepPdfSecSettings AndAlso Not HandleUnsavedChanges() Then
                Exit Sub
            End If

            ' Store PDF security settings beforehand
            Dim pdfSecStorage = New With {
                .PDFCertFilePath = PDFCertFilePath,
                .PDFCertPasswd = PDFCertPasswd,
                .PDFMasterPassword = PDFMasterPassword,
                .PDFUserPassword = PDFUserPassword,
                .PDFSignature = PDFSignature,
                .DocumentAccessPermissions = DocumentAccessPermissions
            }

            ' Clear pdf security settings
            If Not bKeepPdfSecSettings Then
                PDFCertFilePath = ""
                PDFCertPasswd = ""
                PDFMasterPassword = ""
                PDFUserPassword = ""
                DocumentAccessPermissions = TXTextControl.DocumentAccessPermissions.AllowAll
                PDFSignature = Nothing
            End If

            Dim ls = New LoadSettings() With {
                .ApplicationFieldFormat = ApplicationFieldFormat.MSWord,
                .LoadSubTextParts = True,
                .PDFImportSettings = Me.PDFImportSettings,
                .UserPassword = Me.PDFUserPassword,
                .MasterPassword = Me.PDFMasterPassword,
                .ReportingMergeBlockFormat = ReportingMergeBlockFormat.SubTextParts
            }

            Try
                If String.IsNullOrEmpty(fileName) Then
                    m_textControl.Load(DefaultLoadTypes, ls)
                Else
                    ' First, check if file exists
                    If Not File.Exists(fileName) Then
                        Dim e = New ShowMessageBoxEventArgs(String.Format(My.Resources.MSG_FILE_DOES_NOT_EXIST, fileName), MessageBoxButton.OK, MessageBoxIcon.[Error])
                        OnShowMessageBox(e)
                        Exit Sub
                    End If
                    Dim ext As String = Path.GetExtension(fileName)
                    'MsgBox("fileName in open " & fileName)
                    ext = jfDateityp
                    Dim streamType As StreamType = ToTXStreamType(fileName)
                    If streamType = DirectCast(-1, StreamType) Then
                        OnShowMessageBox(New ShowMessageBoxEventArgs(String.Format(My.Resources.MSG_UNKNOWN_FILE_TYPE, ext), MessageBoxButton.OK, MessageBoxIcon.Information))
                        Return
                    End If

                    ' Try to load file
                    m_textControl.Load(fileName, streamType, ls)

                    ' Ask for master / owner password if permissions are resticted
                    If ls.DocumentAccessPermissions <> TXTextControl.DocumentAccessPermissions.AllowAll Then
                        If RetrieveMasterPasswort() Then
                            If String.IsNullOrEmpty(fileName) Then
                                fileName = ls.LoadedFile
                            End If
                            Open(fileName, True)
                            ' Open again with master password set
                            Return
                        End If
                    End If
                End If
            Catch exc As FilterException
                If String.IsNullOrEmpty(fileName) Then
                    fileName = ls.LoadedFile
                End If
                HandleFilterException(exc, fileName)
                Return
            Catch exc As LicenseLevelException
                OnShowMessageBox(New ShowMessageBoxEventArgs(exc.Message, MessageBoxButton.OK, MessageBoxIcon.[Error]))
                Return
            Catch exc As MergeBlockConversionException
                HandleMergeBlockConversionException(exc)
            Catch exc As Exception
                OnShowMessageBox(New ShowMessageBoxEventArgs(exc.Message, MessageBoxButton.OK, MessageBoxIcon.[Error]))
                Return
            End Try

            ' If LoadSettings.LoadedFile is set, a file was successfully loaded.
            If Not String.IsNullOrEmpty(ls.LoadedFile) Then
                DocumentFileName = ls.LoadedFile
                IsDocTargetBased = (ls.ConvertedMergeBlocks > 0)
                m_streamType = ls.LoadedStreamType
                IsDocumentDirty = False
                Me.DocumentAccessPermissions = ls.DocumentAccessPermissions
                CssFileName = ls.CssFileName
                CssSaveMode = TXTextControl.CssSaveMode.None
                AddRecentFile(ls.LoadedFile)
            ElseIf Not bKeepPdfSecSettings Then
                ' If pdf security settings were reset but no file was loaded (e. g. because of the 
                ' user pressing cancel in the file open dialog), revert pdf security settings here
                PDFCertFilePath = pdfSecStorage.PDFCertFilePath
                PDFCertPasswd = pdfSecStorage.PDFCertPasswd
                PDFMasterPassword = pdfSecStorage.PDFMasterPassword
                PDFUserPassword = pdfSecStorage.PDFUserPassword
                PDFSignature = pdfSecStorage.PDFSignature
                DocumentAccessPermissions = pdfSecStorage.DocumentAccessPermissions
            End If
        End Sub

        '-------------------------------------------------------------------------------------------------------
        ' Saves the current document.
        '-----------------------------------------------------------------------------------------------------

        Public Sub Save()
            MessageBox.Show("Speichern nicht möglich. Diese Anwendung dient nur der Ansicht und dem Herauskopieren!") : Exit Sub
            Dim blockFormat As ReportingMergeBlockFormat
            If Not TryDetermineMergeBlockSaveFormat(blockFormat) Then
                Return
            End If

            Dim saveSettings = New SaveSettings() With {
                .CssFileName = Me.CssFileName,
                .CssSaveMode = Me.CssSaveMode,
                .UserPassword = Me.PDFUserPassword,
                .MasterPassword = Me.PDFMasterPassword,
                .DocumentAccessPermissions = Me.DocumentAccessPermissions,
                .ReportingMergeBlockFormat = blockFormat
            }
            If PDFSignature IsNot Nothing Then
                saveSettings.DigitalSignature = PDFSignature
            End If

            Try
                If Not String.IsNullOrEmpty(m_documentFileName) Then
                    ' Save with current name and type
                    m_textControl.Save(m_documentFileName, m_streamType, saveSettings)
                Else
                    ' Save As...
                    m_textControl.Save(DefaultSaveTypes, saveSettings)
                End If
            Catch exc As Exception
                OnShowMessageBox(New ShowMessageBoxEventArgs(exc.Message, MessageBoxButton.OK, MessageBoxIcon.[Error]))
            End Try

            ' If saveSettings.SavedFile is set, a file was successfully saved.
            If Not String.IsNullOrEmpty(saveSettings.SavedFile) Then
                DocumentFileName = saveSettings.SavedFile
                m_streamType = saveSettings.SavedStreamType
                IsDocumentDirty = False
                AddRecentFile(m_documentFileName)
            End If
        End Sub

        '-------------------------------------------------------------------------------------------------------
        ' Shows a file save as dialog.
        '-----------------------------------------------------------------------------------------------------

        Public Sub SaveAs(Optional streamType As System.Nullable(Of StreamType) = Nothing)
            MessageBox.Show("Speichern nicht möglich. Diese Anwendung dient nur der Ansicht und dem Herauskopieren!") : Exit Sub
            Dim blockFormat As ReportingMergeBlockFormat
            If Not TryDetermineMergeBlockSaveFormat(blockFormat) Then
                Return
            End If

            Dim saveSettings = New SaveSettings() With {
                .CssFileName = Me.CssFileName,
                .CssSaveMode = Me.CssSaveMode,
                .UserPassword = Me.PDFUserPassword,
                .MasterPassword = Me.PDFMasterPassword,
                .DocumentAccessPermissions = Me.DocumentAccessPermissions,
                .ReportingMergeBlockFormat = blockFormat
            }
            If PDFSignature IsNot Nothing Then
                saveSettings.DigitalSignature = PDFSignature
            End If

            streamType = If(streamType, DefaultSaveTypes)
            Try
                m_textControl.Save(streamType.Value, saveSettings)
            Catch exc As Exception
                OnShowMessageBox(New ShowMessageBoxEventArgs(exc.Message, MessageBoxButton.OK, MessageBoxIcon.[Error]))
            End Try

            ' If SaveSettings.SavedFile is set, a file was successfully saved.
            If Not String.IsNullOrEmpty(saveSettings.SavedFile) Then
                DocumentFileName = saveSettings.SavedFile
                m_streamType = saveSettings.SavedStreamType
                IsDocumentDirty = False
                AddRecentFile(m_documentFileName)
            End If
        End Sub

        '-------------------------------------------------------------------------------------------------------
        ' Creates an empty document. Shows a confirmation dialog if there are
        ' unsaved changes.
        '
        ' Returns:
        '		True if the document content was reset. False otherwise.
        '-----------------------------------------------------------------------------------------------------

        Public Function [New]() As Boolean
            If Not HandleUnsavedChanges() Then
                Return False
            End If
            m_textControl.ResetContents()
            IsDocumentDirty = False
            DocumentFileName = ""
            IsDocTargetBased = False
            PDFUserPassword = ""
            PDFMasterPassword = ""
            PDFCertFilePath = ""
            PDFCertPasswd = ""
            PDFSignature = Nothing
            DocumentAccessPermissions = TXTextControl.DocumentAccessPermissions.AllowAll
            Return True
        End Function

        '-------------------------------------------------------------------------------------------------------
        ' Shows a confirmation dialog if unsaved changes exist.
        '
        ' Returns:
        '		True if application is allowed to close. False otherwise.
        '-----------------------------------------------------------------------------------------------------

        Public Function ExitApplication() As Boolean
            Return HandleUnsavedChanges()
        End Function

        '-------------------------------------------------------------------------------------------------------
        ' Asks the user what to do with unsaved changes if there are any.
        '
        ' Returns:
        '		Either there were no unsaved changes or unsaved changes were handled successfully.
        '-----------------------------------------------------------------------------------------------------

        Public Function HandleUnsavedChanges() As Boolean
            If IsDocumentDirty Then
                ' If there are unsaved changes, make the caller show a message box
                Dim msg As String = String.Format(My.Resources.SAVE_CHANGES_TO, DocumentTitle)
                Dim args = New ShowMessageBoxEventArgs(msg, MessageBoxButton.YesNoCancel, MessageBoxIcon.Question)
                OnShowMessageBox(args)
                Select Case args.DialogResult
                    Case DialogResult.Cancel
                        Return False

                    Case DialogResult.Yes
                        Save()
                        If String.IsNullOrEmpty(m_documentFileName) Then
                            Return False
                        End If
                        Exit Select
                End Select
            End If
            Return True
        End Function

        '-------------------------------------------------------------------------------------------------------
        ' Removes a file from the list of recent files and fires the RecentFileListChanged event.
        '-----------------------------------------------------------------------------------------------------

        Friend Sub RemoveRecentFile(path As String)
            Dim nFiles As Integer = m_recentFiles.Count
            m_recentFiles.Remove(path)
            If m_recentFiles.Count < nFiles Then
                OnRecentFileListChanged()
            End If
        End Sub

#End Region


#Region "  Public Properties  "

        '-------------------------------------------------------------------------------------------------------
        ' Returns if there are unsaved changes.
        '-----------------------------------------------------------------------------------------------------

        Public Property IsDocumentDirty() As Boolean
            Get
                Return m_isDocumentDirty
            End Get
            Set(value As Boolean)
                SetDocumentDirty(value)
            End Set
        End Property

        Public Property CssFileName() As String
            Get
                Return m_CssFileName
            End Get
            Set(value As String)
                m_CssFileName = value
            End Set
        End Property
        Private m_CssFileName As String
        Public Property CssSaveMode() As CssSaveMode
            Get
                Return m_CssSaveMode
            End Get
            Set(value As CssSaveMode)
                m_CssSaveMode = value
            End Set
        End Property
        Private m_CssSaveMode As CssSaveMode

        Public Property PDFUserPassword() As String
            Get
                Return m_pdfUserPwd
            End Get
            Set(value As String)
                m_pdfUserPwd = value
            End Set
        End Property

        Public Property PDFMasterPassword() As String
            Get
                Return m_PDFMasterPassword
            End Get
            Set(value As String)
                m_PDFMasterPassword = value
            End Set
        End Property
        Private m_PDFMasterPassword As String

        Public Property DocumentAccessPermissions() As DocumentAccessPermissions
            Get
                Return m_docAccPerms
            End Get
            Set(value As DocumentAccessPermissions)
                Dim valOld As DocumentAccessPermissions = m_docAccPerms
                m_docAccPerms = value
                If value <> valOld Then
                    OnDocumentAccessPermissionsChanged()
                End If
            End Set
        End Property

        Public Property PDFImportSettings() As PDFImportSettings
            Get
                Return m_PDFImportSettings
            End Get
            Set(value As PDFImportSettings)
                m_PDFImportSettings = value
            End Set
        End Property
        Private m_PDFImportSettings As PDFImportSettings
        Public Property PDFCertPasswd() As String
            Get
                Return m_PDFCertPasswd
            End Get
            Set(value As String)
                m_PDFCertPasswd = value
            End Set
        End Property
        Private m_PDFCertPasswd As String
        Public Property PDFCertFilePath() As String
            Get
                Return m_PDFCertFilePath
            End Get
            Set(value As String)
                m_PDFCertFilePath = value
            End Set
        End Property
        Private m_PDFCertFilePath As String
        Public Property PDFSignature() As DigitalSignature
            Get
                Return m_PDFSignature
            End Get
            Set(value As DigitalSignature)
                m_PDFSignature = value
            End Set
        End Property
        Private m_PDFSignature As DigitalSignature
        Public Property IsDocTargetBased() As Boolean
            Get
                Return m_IsDocTargetBased
            End Get
            Private Set(value As Boolean)
                m_IsDocTargetBased = value
            End Set
        End Property
        Private m_IsDocTargetBased As Boolean

        '-------------------------------------------------------------------------------------------------------
        ' Maximum number of possible items in the recent files list.
        '-----------------------------------------------------------------------------------------------------

        Public Property MaxRecentFiles() As Integer
            Get
                Return m_maxRecentFiles
            End Get
            Set(value As Integer)
                If value < 1 Then
                    Throw New IndexOutOfRangeException()
                End If
                m_maxRecentFiles = value
                Dim count As Integer = m_recentFiles.Count
                TrimRecentFilesList()
                If m_recentFiles.Count < count Then
                    OnRecentFileListChanged()
                End If
            End Set
        End Property

        '-------------------------------------------------------------------------------------------------------
        ' List of recently opened files.
        '-----------------------------------------------------------------------------------------------------

        Public Property RecentFiles() As StringCollection
            Get
                Return m_recentFiles
            End Get
            Set(value As StringCollection)
                m_recentFiles = If(value, New StringCollection())
                TrimRecentFilesList()
                OnRecentFileListChanged()
            End Set
        End Property

        '-------------------------------------------------------------------------------------------------------
        ' The document file name. Is never null.
        '-----------------------------------------------------------------------------------------------------

        Public Property DocumentFileName() As String
            Get
                Return m_documentFileName
            End Get
            Private Set(value As String)
                value = If(value, "")
                Dim oldValue As String = m_documentFileName
                m_documentFileName = value
                If value <> oldValue Then
                    OnDocumentFileNameChanged(value)
                End If
            End Set
        End Property

        '-------------------------------------------------------------------------------------------------------
        ' Returns a string which can be used as the document title.
        '-----------------------------------------------------------------------------------------------------

        Public ReadOnly Property DocumentTitle() As String
            Get
                Return If(String.IsNullOrEmpty(m_documentFileName), My.Resources.DOC_TITLE_UNTITLED, Path.GetFileName(m_documentFileName))
            End Get
        End Property

#End Region


#Region "  Protected Methods  "

        '-------------------------------------------------------------------------------------------------------
        ' Fires the ShowMessageBox event.
        '-----------------------------------------------------------------------------------------------------

        Protected Overridable Sub OnShowMessageBox(e As ShowMessageBoxEventArgs)
            RaiseEvent ShowMessageBox(Me, e)
        End Sub

        '-------------------------------------------------------------------------------------------------------
        ' Fires the "DocumentDirtyChanged" event.
        '-----------------------------------------------------------------------------------------------------

        Protected Overridable Sub OnDocumentDirtyChanged(newValue As Boolean)
            RaiseEvent DocumentDirtyChanged(Me, New DocumentDirtyChangedEventArgs(newValue))
        End Sub

        '-------------------------------------------------------------------------------------------------------
        ' Fires the "DocumentFileNameChanged" event.
        '-----------------------------------------------------------------------------------------------------

        Protected Overridable Sub OnDocumentFileNameChanged(newName As String)
            RaiseEvent DocumentFileNameChanged(Me, New DocumentFileNameChangedEventArgs(newName))
        End Sub

        '-------------------------------------------------------------------------------------------------------
        ' Fires the "RecentFileListChanged" event.
        '-----------------------------------------------------------------------------------------------------

        Protected Overridable Sub OnRecentFileListChanged()
            RaiseEvent RecentFileListChanged(Me, EventArgs.Empty)
        End Sub

        '-------------------------------------------------------------------------------------------------------
        ' Fires the "UserInputRequested" event.
        '-----------------------------------------------------------------------------------------------------

        Protected Overridable Sub OnUserInputRequested(e As UserInputRequestedEventArgs)
            RaiseEvent UserInputRequested(Me, e)
        End Sub

        '-------------------------------------------------------------------------------------------------------
        ' Fires the "DocumentAccessPermissionsChanged" event.
        '-----------------------------------------------------------------------------------------------------

        Protected Overridable Sub OnDocumentAccessPermissionsChanged()
            RaiseEvent DocumentAccessPermissionsChanged(Me, EventArgs.Empty)
        End Sub

#End Region


#Region "  Private Methods  "

        Private Sub SetDocumentDirty(value As Boolean)
            Dim oldValue As Boolean = m_isDocumentDirty
            m_isDocumentDirty = value
            If value <> oldValue Then
                OnDocumentDirtyChanged(value)
            End If
        End Sub

        '-------------------------------------------------------------------------------------------------------
        ' Adds a recent file to the list of recent files.
        '
        ' Params:
        '		fileName:	The full file path.
        '-----------------------------------------------------------------------------------------------------

        Private Sub AddRecentFile(fileName As String)
            ' First, remove the file name if it already exists
            For i As Integer = m_recentFiles.Count - 1 To 0 Step -1
                If m_recentFiles(i).ToLower() = fileName.ToLower() Then
                    m_recentFiles.RemoveAt(i)
                    Exit For
                End If
            Next

            ' Add file name
            m_recentFiles.Insert(0, fileName)
            TrimRecentFilesList()

            ' Fire event
            OnRecentFileListChanged()
        End Sub

        '-------------------------------------------------------------------------------------------------------
        ' Makes sure m_recentFiles contains at most m_maxRecentFiles items.
        '-----------------------------------------------------------------------------------------------------

        Private Sub TrimRecentFilesList()
            While m_recentFiles.Count > m_maxRecentFiles
                m_recentFiles.RemoveAt(m_recentFiles.Count - 1)
            End While
        End Sub

        '-------------------------------------------------------------------------------------------------------
        ' Handles possible filter exception reasons.
        '-----------------------------------------------------------------------------------------------------

        Private Sub HandleFilterException(exc As FilterException, fileName As String)
            Select Case exc.Reason
                Case FilterException.FilterError.InvalidPassword
                    Dim args = New UserInputRequestedEventArgs(PDFUserPassword, My.Resources.USR_INP_PASSWORD_TITLE, My.Resources.USR_INP_PASSWORD_LABEL, True, UserInputRequestReason.PdfUserPassword)
                    OnUserInputRequested(args)
                    Select Case args.DialogResult
                        Case DialogResult.OK
                            PDFUserPassword = If(args.Value, "")
                            Open(fileName, True)    ' Try to open file again with user password set
                            Return

                        Case DialogResult.Cancel
                            Return ' Do nothing.
                    End Select
                    Exit Select
            End Select
            OnShowMessageBox(New ShowMessageBoxEventArgs(exc.Message, MessageBoxButton.OK, MessageBoxIcon.[Error]))
        End Sub

        '-------------------------------------------------------------------------------------------------------
        ' Handles merge block conversion exception
        '-----------------------------------------------------------------------------------------------------

        Private Sub HandleMergeBlockConversionException(exc As MergeBlockConversionException)
            Dim blockList As String = String.Join(vbCr & vbLf, exc.BlockNamesUnconverted.ToArray())
            Dim strMsg As String = String.Format(My.Resources.EXC_MERGE_BLOCK_CONVERSION, blockList)
            Dim e = New ShowMessageBoxEventArgs(strMsg, MessageBoxButton.OK, MessageBoxIcon.Information)
            OnShowMessageBox(e)
        End Sub

        '-------------------------------------------------------------------------------------------------------
        ' Shows a password input box asking the user for the master password
        '-----------------------------------------------------------------------------------------------------

        Private Function RetrieveMasterPasswort() As Boolean
            Dim args = New UserInputRequestedEventArgs("", My.Resources.USR_INP_MASTER_PASSWORD_TITLE, My.Resources.USR_INP_MASTER_PASSWORD_LABEL, True, UserInputRequestReason.PdfMasterPassword)
            OnUserInputRequested(args)
            Select Case args.DialogResult
                Case DialogResult.OK
                    PDFMasterPassword = args.Value
                    Return True
                Case Else

                    PDFMasterPassword = ""
                    Exit Select
            End Select
            Return False
        End Function

        '-------------------------------------------------------------------------------------------------------
        ' Shows a message box letting the user decide if the document should be saved with
        ' SubTextPart or DocumentTarget based merge blocks.
        '
        ' Parameters: 
        '		streamType	Document type (Not all document types require confirmation)
        '
        ' Returns:
        '		True if user pressed yes, false if user pressed no, null if user pressed cancel.
        '-----------------------------------------------------------------------------------------------------

        Friend Function ConfirmSaveSubTextPartBlocks(Optional streamType__1 As StreamType = DirectCast(0, StreamType)) As System.Nullable(Of Boolean)
            ' Never convert to subtextparts in case of formats not supporting them
            If (streamType__1 And (StreamType.InternalFormat Or StreamType.InternalUnicodeFormat Or StreamType.WordprocessingML Or StreamType.RichTextFormat Or StreamType.MSWord)) = 0 Then
                Return False
            End If

            ' User interaction
            Dim args = New ShowMessageBoxEventArgs(My.Resources.MSG_CONFIRM_UPDATE_MERGE_BLOCK_TYPE, MessageBoxButton.YesNoCancel, MessageBoxIcon.Question)
            OnShowMessageBox(args)

            Select Case args.DialogResult
                Case DialogResult.Yes
                    Return True

                Case DialogResult.No
                    Return False

                Case DialogResult.Cancel
                    Return Nothing
            End Select
            Return False
        End Function

        '-------------------------------------------------------------------------------------------------------
        ' Trys to determine the merge block save format (document Target based or sub text part based)
        ' by taking into account the block format in the original document and by showing a yes / no 
        ' message box.
        '
        ' Returns:
        '		false		Abort file open process.
        '		true		Found out block format successfully.
        '-----------------------------------------------------------------------------------------------------

        Private Function TryDetermineMergeBlockSaveFormat(ByRef blockFormat As ReportingMergeBlockFormat) As Boolean
            blockFormat = ReportingMergeBlockFormat.SubTextParts
            ' Check if subtextpart based merge blocks should be 
            ' converted back to "old style" merge blocks
            If IsDocTargetBased Then
                Dim bSubTextParts As System.Nullable(Of Boolean) = ConfirmSaveSubTextPartBlocks(m_streamType)
                If bSubTextParts = False Then
                    blockFormat = ReportingMergeBlockFormat.DocumentTargets
                ElseIf bSubTextParts Is Nothing Then
                    Return False
                End If
            End If
            Return True
        End Function
        Private Shared Function InlineAssignHelper(Of T)(ByRef target As T, value As T) As T
            target = value
            Return value
        End Function

#End Region
    End Class

    '----------------------------------------------------------------------------------------------------------
    ' Extension methods used in this file.
    '--------------------------------------------------------------------------------------------------------

    Module Extensions

        <Extension>
        Public Function ToTXStreamType(fdatei As String) As StreamType
            Dim result As StreamType = StreamType.MSWord
            Dim fs As FileStream
            Dim reader As BinaryReader
            Dim Data As Byte()
            'MsgBox(fdatei)
            Try
                fs = New FileStream(fdatei, FileMode.Open)
                reader = New BinaryReader(fs)
                Data = reader.ReadBytes(2)
                'MsgBox("res1 : " & Data(0))
                If Data(0) = 123 And Data(1) = 92 Then
                    result = StreamType.RichTextFormat
                End If
                If Data(0) = 80 And Data(1) = 75 Then
                    result = StreamType.WordprocessingML
                End If
                'MsgBox("res: " & result.ToString)
                reader.Close()
                '  reader.Dispose()
                fs.Close()
                fs.Dispose()
                Return result
            Catch ex As Exception
                MsgBox("fehler in ToTXStreamType: " & ex.ToString)
                Return result
            End Try
            Return result
        End Function
        'Public Function ToTXStreamType(fileExt As String) As StreamType
        '	Select Case fileExt.ToLower()
        '		Case ".rtf"
        '			Return StreamType.RichTextFormat

        '		Case ".htm", ".html"
        '			Return StreamType.HTMLFormat

        '		Case ".tx"
        '			Return StreamType.InternalUnicodeFormat

        '		Case ".doc"
        '			Return StreamType.MSWord

        '		Case ".docx"
        '			Return StreamType.WordprocessingML

        '		Case ".pdf"
        '			Return StreamType.AdobePDF

        '		Case ".txt"
        '			Return StreamType.PlainText
        '	End Select
        '	Return DirectCast(-1, StreamType)
        'End Function
    End Module
End Namespace
