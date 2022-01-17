Imports System
Namespace CLstart
    Public Class ZeigeraufXMLinitdatei
        Shared Sub nachricht(text As String)
            My.Log.WriteEntry(text)
        End Sub
        Public Shared Sub nachricht(ByVal text As String, ByVal ex As System.Exception)
            Dim anhang As String = ""
            text = text & ToLogString(ex, text)
            'myGlobalz.sitzung.nachrichtenText = text
            My.Log.WriteEntry(text)
            'mitFehlerMail(text, anhang)
        End Sub
        Shared Function [get](zeigerdatei As String, standardXMLpath As String) As String 'myGlobalz.DMSLaufWerkBuchstabe
            Dim XMLdatei As String = readFromZeigerDatei(zeigerdatei)
            ' Dim defaultXML As String =myGlobalz.DMSLaufWerkBuchstabe & "\paradigmacache\backup\archiv\1\0\1\paradigma.xml"
            'Dim defaultXML As String
            ' defaultXML = DMStools._DMSLaufWerkBuchstabe & "\paradigmaArchiv\div\xml\paradigma_2017.xml"
            'defaultXML = DMStools._DMSLaufWerkBuchstabe & "\paradigmaArchiv\div\xml\paradigma_2017_test.xml"
            If XMLdatei.Contains("fehler") Then
                nachricht("Zeigerdatei: verwende default: " & standardXMLpath)
                Return standardXMLpath
            Else
                nachricht("XMLdatei:   " & XMLdatei)
                Return XMLdatei
            End If
        End Function

        Private Shared Function readFromZeigerDatei(zeigerdatei As String) As String
            Dim XMLdatei = ""
            Try
                Dim fi As New System.IO.FileInfo(zeigerdatei)
                If fi.Exists Then
                    Using fineu As New IO.StreamReader(zeigerdatei, System.Text.Encoding.GetEncoding(1252))
                        XMLdatei = fineu.ReadLine
                    End Using
                Else
                    nachricht("zeigerdatei (" & zeigerdatei & ") nicht gefunden - verwende default!)")
                End If
                fi = Nothing
                If String.IsNullOrEmpty(XMLdatei) Then
                    Return "fehler"
                Else
                    nachricht("zeigerdatei ist: " & zeigerdatei)
                    Return XMLdatei
                End If
            Catch ex As Exception
                nachricht("Fehler in readFromZeigerDatei: ", ex)
                Return "fehler"
            End Try
        End Function
    End Class
    Public Class wintools2
        Public Shared Sub umgebungstest()
            '-db-läuft?
            '-archiv-vorhanden (exchange vorhanden?)
            '-deploy-vorhanden
            '-netzwerk -allgemein vorhanden
            '-config.xml vorhanden
        End Sub
    End Class
    Public Class MeinLogging
        Public Shared Sub LoggingEnde(qfile As String, LOGFILEKOPIE As String) 'myGlobalz.LOGFILEKOPIE
            Try
                My.Log.WriteEntry("Programm wird beendet: " & Now.ToString)
                My.Log.WriteEntry("myGlobalz.LOGFILEKOPIE: " & LOGFILEKOPIE)
                My.Log.WriteEntry("My.Log.DefaultFileLogWriter.FullLogFileName: " & qfile)
                My.Log.DefaultFileLogWriter.Flush()
                Dim quelle As New IO.FileInfo(qfile)
                Dim ziel As New IO.FileInfo(LOGFILEKOPIE)
                nachricht("Kopie: LoggingEnde, Quelldatei:  " & quelle.FullName)
                nachricht("Kopie: LoggingEnde, Zieldatei:  " & LOGFILEKOPIE)
                If quelle.Exists Then
                    IO.Directory.CreateDirectory(ziel.DirectoryName)
                    quelle.CopyTo(LOGFILEKOPIE, True)
                Else
                    nachricht("FEHLER: LoggingEnde, Quelldatei existiert nicht: " & quelle.FullName)
                End If
                quelle = Nothing
            Catch ex As Exception
                nachricht("FEHLER: LoggingEnde, Quelldatei lies sich nicht Kopieren nach: " & LOGFILEKOPIE)
            End Try
        End Sub

        Public Shared Sub LoggingInit(modulname As String, username As String, ByRef qfile As String)
            Try
                With My.Log.DefaultFileLogWriter
                    .BaseFileName = "Paradigma_" & modulname & "_" & username
                    .AutoFlush = False
                    .Append = False
                End With
                qfile = My.Log.DefaultFileLogWriter.FullLogFileName
                My.Log.WriteEntry("My.Log.DefaultFileLogWriter: " & My.Log.DefaultFileLogWriter.FullLogFileName)
                My.Log.WriteEntry("startlog: " & Now.ToString)
            Catch ex As Exception
                nachricht("Fehler in " & System.Reflection.MethodBase.GetCurrentMethod().Name & ": ", ex)
            End Try
        End Sub
        Public Shared Sub nachricht(text As String)
            My.Log.WriteEntry(text)
        End Sub
        Public Shared Sub nachricht(ByVal text As String, ByVal ex As System.Exception)
            Dim anhang As String = ""
            text = text & ToLogString(ex, text)
            'myGlobalz.sitzung.nachrichtenText = text
            My.Log.WriteEntry(text)
            'mitFehlerMail(text, anhang)
        End Sub
    End Class
End Namespace
