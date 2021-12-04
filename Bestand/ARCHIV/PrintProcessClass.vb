'Imports System.Data
'Imports System.Threading

'Class PrintProcessClass
'    Private Sub New()
'    End Sub
'    Private WithEvents myProcess As New Process
'    Private elapsedTime As Integer
'    Private eventHandled As Boolean

'    ' Print a file with any known extension.
'    Function PrintDoc(ByVal fileName As String) As Integer
'        elapsedTime = 0
'        eventHandled = False
'        Try
'            ' Start a process to print a file and raise an event when done.
'            myProcess.StartInfo.FileName = fileName
'            myProcess.StartInfo.Verb = "Edit"
'            myProcess.StartInfo.CreateNoWindow = True
'            myProcess.EnableRaisingEvents = True

'            myProcess = Process.Start("C:\Program Files (x86)\Microsoft Office\Office14\winword.exe ", fileName)
'            myProcess.EnableRaisingEvents = True
'            Return CInt(myProcess.Handle)
'        Catch ex As Exception
'            Console.WriteLine("An error occurred trying to print ""{0}"":" & _
'                vbCrLf & ex.Message, fileName)
'            Return 0
'        End Try
'    End Function

'    Private Sub myProcess_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles myProcess.Disposed
'        MsgBox("2")
'    End Sub

'    Private Sub myProcess_ErrorDataReceived(ByVal sender As Object, ByVal e As System.Diagnostics.DataReceivedEventArgs) Handles myProcess.ErrorDataReceived
'        MsgBox("3")
'    End Sub

'    ' Handle Exited event and display process information.
'    Private Sub myProcess_Exited(ByVal sender As Object, ByVal e As System.EventArgs) Handles myProcess.Exited
'        'ihah
'        'Dim mysend As New Process
'        'mysend = CType(myProcess, Process)

'        eventHandled = True
'        Console.WriteLine("Exit time:    {0}" & vbCrLf & _
'                        "Exit code:    {1}" & vbCrLf &
'                        "Elapsed time: {2} " &
'                        "Verb: {3} ",
'                        myProcess.ExitTime,
'                        myProcess.ExitCode,
'                        myProcess.TotalProcessorTime.Ticks.ToString,
'                        myProcess.StartInfo.Verb.ToString)
'        wurdensDokumenteGeaendert(myGlobalz.sitzung.checkoutDokuList, CInt(myProcess.Handle))


'        '   nachricht("Dokumentwurde geändert?: " & detailsTools.wurdenDokumenteGeaendert(myGlobalz.sitzung.checkoutDokuList).ToString)
'        'gbxDateiInBenutzung.Visibility = Windows.Visibility.Collapsed
'    End Sub

'    Shared Function wurdensDokumenteGeaendert(ByVal checkoutDokuList As List(Of LIBArchiv.Dokument),
'                                             ByVal handlenr As Integer) As Boolean
'        Try
'            nachricht("wurdenDokumenteGeaendert: --------------------------------------")
'            If myGlobalz.sitzung.checkoutDokuList.Count > 0 Then
'                For Each dok As LIBArchiv.Dokument In checkoutDokuList
'                    If handlenr = dok.Handlenr Then
'                        dokuschliessen(checkoutDokuList, dok)
'                        MsgBox("Dokument wurde geschlossen.   " & vbCrLf & vbCrLf & "Name: " & dok.DateinameMitExtension)
'                    End If
'                Next
'                Return False
'            Else
'                Return False
'            End If
'            nachricht("wurdenDokumenteGeaendert: --------------------------------------ende")
'            Return True
'        Catch ex As Exception
'            nachricht("1Fehler inwurdenDokumenteGeaendert: " & ex.ToString)
'        Finally
'            '  checkoutDokuList.Clear()
'        End Try
'    End Function

'    Shared Function dokuschliessen(ByVal checkoutDokuList As List(Of LIBArchiv.Dokument),
'                                   ByVal dok As LIBArchiv.Dokument) As Boolean
'        Dim CO_test, AR_test As IO.FileInfo
'        'Dim AR_datumzeit As Date
'        'Dim CO_datumzeit As Date
'        'todo alledoppelten einträge in der liste entfernen
'        'es wurden dokus ausgecheckt!
'        'filedatum prüfen ob neuer als in der list
'        'checkoutnamen bilden
'        CO_test = New IO.FileInfo(dok.FullnameCheckout)
'        AR_test = New IO.FileInfo(dok.FullnameImArchiv)
'        'AR_datumzeit = CO_test.LastWriteTime
'        'CO_datumzeit = AR_test.LastWriteTime
'        nachricht(dok.OriginalName)
'        nachricht("Alt: " & AR_test.LastWriteTime.ToString)
'        nachricht("Neu: " & CO_test.LastWriteTime.ToString)
'        If dok.revisionssicher Then
'            'If aktdoku.revisionssicher Then 
'            nachricht("Dokument ist revisionssicher. sie können die Änderungen also nicht direkt ins archiv übernhemen.")
'            MessageBox.Show(myGlobalz.Infotext_revisionssicherheit, dok.DateinameMitExtension, MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.OK)
'            Return False
'        End If
'        If AR_test.LastWriteTime < CO_test.LastWriteTime Then
'            'Dim mesres As New MessageBoxResult
'            Dim mesres As MessageBoxResult = GetMesres(dok)
'            If mesres = MessageBoxResult.Yes Then
'                'Ins Archiv übernhemen
'                'todo frag ob als neue version ins Archiv übernehmen
'                If CO_test.Exists Then CO_test.CopyTo(dok.FullnameImArchiv, True)
'                CLstart.myc.aLog.komponente = "Dokumente"
'                CLstart.myc.aLog.aktion = myGlobalz.sitzung.aktDokument.DocID & " " &
'                                        myGlobalz.sitzung.aktDokument.DateinameMitExtension & ": geaendert" : CLstart.myc.aLog.log()
'                myGlobalz.sitzung.aktDokument.Filedatum = Now
'                Dim result As Integer = DokArcTools.dokUpdate.execute(myGlobalz.sitzung.aktDokument.DocID, myGlobalz.sitzung.aktDokument.istVeraltet, myGlobalz.sitzung.aktDokument.Beschreibung,
'                                                                      myGlobalz.sitzung.aktDokument.revisionssicher, myGlobalz.sitzung.aktDokument.Filedatum, myGlobalz.sitzung.aktDokument.EXIFlat, myGlobalz.sitzung.aktDokument.EXIFlon)
'                'todo loggen: datei geändert

'            End If
'        End If
'        DateiLoeschenFallsExistiert(CO_test, dok)
'        checkoutDokuList.Remove(dok)
'    End Function

'    Private Shared Function GetMesres(ByVal dok As LIBArchiv.Dokument) As MessageBoxResult
'        Dim mesres As MessageBoxResult
'        mesres = MessageBox.Show("Sie haben das Dokument >" & dok.DateinameMitExtension & "< geändert!" & Environment.NewLine &
'                        " " & Environment.NewLine &
'                        "Soll die geänderte Datei ins Archiv übernommen werden ?" & Environment.NewLine &
'                        " " & Environment.NewLine &
'                        "  Ja  - ins Archiv übernehmen " & Environment.NewLine &
'                        " Nein - geänderte Datei löschen!" & Environment.NewLine & Environment.NewLine &
'                        "  " & Environment.NewLine & Environment.NewLine &
'                        "(Hinweis: Bitte schließen Sie in jedem Falle alle Officedokumente bevor Sie fortfahren! )" & Environment.NewLine & Environment.NewLine,
'                        "Archiv: Ein Dokument wurde geändert!",
'                        MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
'        Return mesres
'    End Function


'    Private Shared Sub DateiLoeschenFallsExistiert(ByVal CO_test As IO.FileInfo, ByVal dok As LIBArchiv.Dokument)
'        Try
'            If CO_test.Exists Then CO_test.Delete() 'In jedem Falle das Dokument löschen

'        Catch ex As Exception
'            If Not CO_test.Extension.ToLower.Contains("jpg") Then
'                MsgBox("Sie haben die Office-Datei '" & dok.DateinameMitExtension & "' noch geöffnet. " & Environment.NewLine &
'                   "Bitte schließen Sie die Datei. " & Environment.NewLine &
'                   "Drücken sie erst dann auf 'OK' !", MsgBoxStyle.OkOnly, "Office-Dokument schließen!")
'                CO_test.Delete() 'In jedem Falle das Dokument löschen
'            End If

'        End Try
'    End Sub


'    'Shared Sub starteProgramm(ByVal args() As String)

'    '    ' Verify that an argument has been entered.
'    '    If args.Length <= 0 Then
'    '        Console.WriteLine("Enter a file name.")
'    '        Return
'    '    End If

'    '    ' Create the process and print the document.
'    '    Dim myProcess As New PrintProcessClass
'    '    myProcess.PrintDoc(args(0))
'    'End Sub

'    Private Sub myProcess_OutputDataReceived(ByVal sender As Object, ByVal e As System.Diagnostics.DataReceivedEventArgs) Handles myProcess.OutputDataReceived
'        MsgBox("1")
'    End Sub
'End Class
