Imports System.Data
Imports textmarke

Namespace LIBcsvAusgabe

    Public Class clsCSVausgaben
        Implements IDisposable
        Public Shared Property csvSubDir As String = "\csv"
        Private sw As IO.StreamWriter
        Private exportFileFuerEigentuemer As String
        Private vid As Integer
        Property _CLstart_mycSimple_Paradigma_local_root As String
        Property _CLstart_mycSimple_enc As System.Text.Encoding
        Sub nachricht(text As String)

            My.Log.WriteEntry(text)
        End Sub

        Public Sub nachricht(ByVal text As String, ByVal ex As System.Exception)
            Dim anhang As String = ""
            text = text & ToLogString(ex, text)
            'myGlobalz.sitzung.nachrichtenText = text
            My.Log.WriteEntry(text)
            'mitFehlerMail(text, anhang)
        End Sub




        Sub start()
            If Not String.IsNullOrEmpty(exportfile) Then
                System.Diagnostics.Process.Start(exportfile)
            Else
                nachricht_und_Mbox("Exportfile nicht vorhanden: " & exportfile.ToString)
            End If
        End Sub
        Public Property delim() As Char
        Public Property exportfile() As String
        Public Property tab As DataTable
        Public Property dokuslist As New List(Of clsPresDokumente)
        Public Property prefix() As String

        Private Shared Function GetTimestamp() As String
            Return Now.Year & Now.Day & Now.Hour & Now.Minute & Now.Second
        End Function

        Sub New(ByVal _prefix As String,
                        ByVal _doks As List(Of clsPresDokumente),
                        ByVal _vid As Integer,
                        exportFileFullName As String,
                        CLstart_mycSimple_Paradigma_local_root As String,
                        CLstart_mycSimple_enc As System.Text.Encoding)
            _CLstart_mycSimple_enc = CLstart_mycSimple_enc
            _CLstart_mycSimple_Paradigma_local_root = CLstart_mycSimple_Paradigma_local_root
            prefix = _prefix
            dokuslist = _doks
            vid = _vid
            Dim timestamp As String = GetTimestamp()
            Try
                IO.Directory.CreateDirectory(_CLstart_mycSimple_Paradigma_local_root & csvSubDir)
            Catch ex As Exception
            End Try
            If String.IsNullOrEmpty(exportFileFullName.Trim) Then
                exportfile = String.Format("{0}\{1}_{2}_{3}_{4}.csv", _CLstart_mycSimple_Paradigma_local_root & csvSubDir, prefix, _vid, prefix, timestamp)
                exportFileFuerEigentuemer = String.Format("{0}\{1}.csv", _CLstart_mycSimple_Paradigma_local_root & csvSubDir, prefix, prefix)
            Else
                exportfile = exportFileFullName
                exportFileFuerEigentuemer = exportFileFullName
            End If
            delim = ";"c
        End Sub
        Sub New(ByVal _prefix As String,
                        ByVal _tab As DataTable,
                        ByVal _vid As Integer,
                        exportFileFullName As String,
                        CLstart_mycSimple_Paradigma_local_root As String,
                        CLstart_mycSimple_enc As System.Text.Encoding)
            _CLstart_mycSimple_enc = CLstart_mycSimple_enc
            _CLstart_mycSimple_Paradigma_local_root = CLstart_mycSimple_Paradigma_local_root
            prefix = _prefix
            tab = _tab
            vid = _vid
            Dim timestamp As String = GetTimestamp()
            Try
                IO.Directory.CreateDirectory(_CLstart_mycSimple_Paradigma_local_root & csvSubDir)
            Catch ex As Exception
            End Try
            If String.IsNullOrEmpty(exportFileFullName.Trim) Then
                exportfile = String.Format("{0}\{1}_{2}_{3}_{4}.csv", _CLstart_mycSimple_Paradigma_local_root & csvSubDir, prefix, _vid, prefix, timestamp)
                exportFileFuerEigentuemer = String.Format("{0}\{1}.csv", _CLstart_mycSimple_Paradigma_local_root & csvSubDir, prefix, prefix)
            Else
                exportfile = exportFileFullName
                exportFileFuerEigentuemer = exportFileFullName
            End If
            delim = ";"c
        End Sub

        'Function CscDateiAusgeben(Optional myGlobalz_sitzung_raumbezugsRec As DataTable = Nothing) As String 'myGlobalz.sitzung.raumbezugsRec
        Function CscDateiAusgeben() As String 'myGlobalz.sitzung.raumbezugsRec
            Try
                sw = New IO.StreamWriter(exportfile, False, _CLstart_mycSimple_enc)
                Select Case prefix
                    Case "Ereignisse"
                        Ereignisse()
                    Case "Beteiligte"
                        Beteiligte()
                    Case "Dokumente"
                        Dokumente()
                    Case "Raumbezuege"
                        Raumbezuege()
                    Case "Zahlungen"
                        Zahlungen()
                    Case "Wiedervorlagen"
                        Wiedervorlagen()
                    Case "Vorgaenge"
                        Vorgaenge()

                'Case "FlurstueckeFuerEigentuemer"
                '    FlurstueckeFuerEigentuemer(vid, myGlobalz_sitzung_raumbezugsRec)
                    Case "beliebig"
                        beliebig(vid)
                    Case Else
                        nachricht("FEhler: diesen typ gibt es nicht!")
                End Select
                sw.Close()
                sw.Dispose()
                Return exportfile
            Catch ex As Exception
                nachricht_und_Mbox("Fehler bei der Excelausgabef, ausgeben:" & vbCrLf & ex.ToString)
                Return ""
            End Try
        End Function

        Public Sub Vorgaenge()
            Try
                sw.WriteLine(
                        "EINGANG" & delim &
                        "SACHGEBIETNR" & delim &
                        "VORGANGSID" & delim &
                        "GEMKRZ" & delim &
                        "BESCHREIBUNG" & delim &
                        "LETZTEBEARBEITUNG" & delim &
                        "ZULETZT" & delim &
                        "ORTSTERMIN" & delim &
                        "STELLUNGNAHME" & delim &
                        "ERLEDIGT" & delim &
                        "AZ" & delim &
                        "BEARBEITER" & delim &
                        "PARAGRAF" & delim &
                        "PROBAUGAZ" & delim &
                        "ABGABEBA" & delim
               )
                For Each p As DataRow In tab.AsEnumerable
                    sw.WriteLine(
                            ohneSemikolon(p.Item("EINGANG").ToString) & delim &
                            ohneSemikolon(p.Item("SACHGEBIETNR").ToString) & delim &
                            ohneSemikolon(p.Item("VORGANGSID").ToString) & delim &
                            ohneSemikolon(p.Item("GEMKRZ").ToString) & delim &
                            ohneSemikolon(p.Item("BESCHREIBUNG").ToString) & delim &
                            ohneSemikolon(p.Item("LETZTEBEARBEITUNG").ToString) & delim &
                            ohneSemikolon(p.Item("LASTACTIONHEROE").ToString) & delim &
                            ohneSemikolon(p.Item("ORTSTERMIN").ToString) & delim &
                            ohneSemikolon(p.Item("STELLUNGNAHME").ToString) & delim &
                            ohneSemikolon(p.Item("ERLEDIGT").ToString) & delim &
                            ohneSemikolon(p.Item("AZ2").ToString) & delim &
                            ohneSemikolon(p.Item("BEARBEITER").ToString) & delim &
                            ohneSemikolon(p.Item("PARAGRAF").ToString) & delim &
                            ohneSemikolon(p.Item("PROBAUGAZ").ToString) & delim &
                            ohneSemikolon(p.Item("ABGABEBA").ToString) & delim)
                Next
            Catch ex As Exception
                nachricht_und_Mbox("Fehler bei der Excelausgabeg" & vbCrLf & ex.ToString)
            End Try
        End Sub

        Public Sub VorgaengeALT()
            Try
                sw.WriteLine(
                        "ERLEDIGT" & delim &
                        "VORGANGSID" & delim &
                        "ORTSTERMIN" & delim &
                        "STELLUNGNAHME" & delim &
                        "ABGABEBA" & delim &
                        "GEMKRZ" & delim &
                        "SACHGEBIETNR" & delim &
                        "AZ" & delim &
                        "BESCHREIBUNG" & delim &
                        "BEARBEITER" & delim &
                        "LETZTEBEARBEITUNG" & delim &
                        "SACHGEBIETNR" & delim &
                        "ZULETZT" & delim &
                        "EINGANG" & delim &
                        "PARAGRAF" & delim &
                        "PROBAUGAZ" & delim
               )
                For Each p As DataRow In tab.AsEnumerable
                    sw.WriteLine(
                            ohneSemikolon(p.Item("ERLEDIGT").ToString) & delim &
                            ohneSemikolon(p.Item("VORGANGSID").ToString) & delim &
                            ohneSemikolon(p.Item("ORTSTERMIN").ToString) & delim &
                            ohneSemikolon(p.Item("STELLUNGNAHME").ToString) & delim &
                            ohneSemikolon(p.Item("ABGABEBA").ToString) & delim &
                            ohneSemikolon(p.Item("GEMKRZ").ToString) & delim &
                            ohneSemikolon(p.Item("SACHGEBIETNR").ToString) & delim &
                            ohneSemikolon(p.Item("AZ2").ToString) & delim &
                            ohneSemikolon(p.Item("BESCHREIBUNG").ToString) & delim &
                            ohneSemikolon(p.Item("Bearbeiter").ToString) & delim &
                            ohneSemikolon(p.Item("LETZTEBEARBEITUNG").ToString) & delim &
                            ohneSemikolon(p.Item("Sachgebietnr").ToString) & delim &
                            ohneSemikolon(p.Item("LASTACTIONHEROE").ToString) & delim &
                            ohneSemikolon(p.Item("EINGANG").ToString) & delim &
                            ohneSemikolon(p.Item("PARAGRAF").ToString) & delim &
                            ohneSemikolon(p.Item("PROBAUGAZ").ToString) & delim)
                Next
            Catch ex As Exception
                l("Fehler bei der Excelausgabeh" & vbCrLf, ex)
            End Try
        End Sub
        Public Sub Wiedervorlagen()
            Try
                sw.WriteLine(
            "Fällig am" & delim &
            "Az" & delim &
            "Todo" & delim &
            "Bemerkung" & delim &
            "Wartenauf" & delim &
            "Beschreibung" & delim &
            "erledigt" & delim &
             "gemkrz"
               )
                For Each p As DataRow In tab.AsEnumerable
                    sw.WriteLine(
                ohneSemikolon(p.Item("Datum").ToString) & delim &
               ohneSemikolon(p.Item("az2").ToString) & delim &
               ohneSemikolon(p.Item("Todo").ToString) & delim &
              ohneSemikolon(p.Item("Bemerkung").ToString) & delim &
             ohneSemikolon(p.Item("Wartenauf").ToString) & delim &
              ohneSemikolon(p.Item("Beschreibung").ToString) & delim &
              ohneSemikolon(p.Item("erledigt").ToString) & delim &
                 ohneSemikolon(p.Item("gemkrz").ToString) & delim
                 )
                Next
            Catch ex As Exception
                l("Fehler bei der Excelausgabei" & vbCrLf, ex)
            End Try
        End Sub
        Public Sub Beteiligte()
            Try
                sw.WriteLine(
               "Rolle" & delim &
               "Anrede" & delim &
               "Namenszusatz" & delim &
               "Vorname" & delim &
               "Name" & delim &
               "Gemeindename" & delim &
               "Strasse" & delim &
               "FFEmail" & delim &
               "FFTelefon1" & delim &
               "Orgname" & delim &
               "orgzusatz".ToString & delim &
               "plz".ToString & delim &
               "hausnr".ToString & delim
                )
                For Each p As DataRow In tab.AsEnumerable
                    sw.WriteLine(
                 ohneSemikolon(p.Item("Rolle").ToString) & delim &
                ohneSemikolon(p.Item("Anrede").ToString) & delim &
               ohneSemikolon(p.Item("Namenszusatz").ToString) & delim &
               ohneSemikolon(p.Item("Vorname").ToString) & delim &
               ohneSemikolon(p.Item("NachName").ToString) & delim &
               ohneSemikolon(p.Item("Gemeindename").ToString) & delim &
               ohneSemikolon(p.Item("Strasse").ToString) & delim &
                ohneSemikolon(p.Item("FFEmail").ToString) & delim &
                ohneSemikolon(p.Item("FFTelefon1").ToString) & delim &
                ohneSemikolon(p.Item("Orgname").ToString) & delim &
               ohneSemikolon(p.Item("orgzusatz").ToString) & delim &
                  ohneSemikolon(p.Item("plz").ToString) & delim &
                     ohneSemikolon(p.Item("hausnr").ToString) & delim
                 )
                Next
            Catch ex As Exception
                l("Fehler bei der Excelausgabej" & vbCrLf, ex)
            End Try
        End Sub

        Public Sub Ereignisse()
            Try
                sw.WriteLine(
                "Datum" & delim &
                "Art" & delim &
                "Beschreibung" & delim
                 )
                For Each p As DataRow In tab.AsEnumerable
                    sw.WriteLine(
                 ohneSemikolon(p.Item("Datum").ToString) & delim &
                 ohneSemikolon(p.Item("Art").ToString) & delim &
                ohneSemikolon(p.Item("Beschreibung").ToString) & delim
                 )
                Next
            Catch ex As Exception
                l("Fehler bei der Excelausgabea" & vbCrLf, ex)
            End Try
        End Sub

        Public Sub Dokumente()
            Try
                sw.WriteLine(
                "Verw." & delim &
                "Filedatum" & delim &
                "Typ " & delim &
                "Dateinameext" & delim &
                "Beschreibung" & delim
                )
                For Each p As Dokument In dokuslist
                    sw.WriteLine(
                                ohneSemikolon(p.istNurVerwandt.ToString) & delim &
                                ohneSemikolon(p.Filedatum.ToString) & delim &
                                ohneSemikolon(p.Typ.ToString) & delim &
                                ohneSemikolon(p.DateinameMitExtension.ToString) & delim &
                                ohneSemikolon(p.Beschreibung.ToString) & delim
                 )
                Next
            Catch ex As Exception
                l("Fehler bei der Excelausgabeb" & vbCrLf, ex)
            End Try
        End Sub

        Public Sub Raumbezuege()
            Try
                sw.WriteLine(
               "Typ" & delim &
               "Titel" & delim &
                   "Abstract" & delim &
                       "qm" & delim &
               "freitext" & delim
                )
                If tab IsNot Nothing AndAlso tab.AsEnumerable IsNot Nothing AndAlso tab.AsEnumerable.Count > 0 Then
                    For Each p As DataRow In tab.AsEnumerable
                        sw.WriteLine(
                    ohneSemikolon(p.Item("Typ").ToString) & delim &
                    ohneSemikolon(p.Item("Titel").ToString) & delim &
                    ohneSemikolon(p.Item("Abstract").ToString) & delim &
                    ohneSemikolon(p.Item("flaecheqm").ToString) & delim &
                    ohneSemikolon(p.Item("FREITEXT").ToString) & delim
                     )
                    Next
                End If
            Catch ex As Exception
                l("Fehler bei der Excelausgabec" & vbCrLf, ex)
            End Try
        End Sub

        Public Sub Zahlungen()
            Try
                sw.WriteLine(
                    "Sachgebietsnr" & delim &
                    "Vorgangsid" & delim &
                    "Aktenzeichen" & delim &
                    "Bearbeiterinitial" & delim &
                    "Typ" & delim &
                    "Betrag" & delim &
                    "Zahler" & delim &
                    "Richtung" & delim &
                    "HHST" & delim &
                    "verschicktam" & delim
                 )
                For Each p As DataRow In tab.AsEnumerable
                    sw.WriteLine(
                            ohneSemikolon(p.Item("Sachgebietsnr").ToString) & delim &
                            ohneSemikolon(p.Item("Vorgangsid").ToString) & delim &
                            ohneSemikolon(p.Item("Aktenzeichen").ToString) & delim &
                            ohneSemikolon(p.Item("Bearbeiterinitial").ToString) & delim &
                            ohneSemikolon(p.Item("Typ").ToString) & delim &
                            ohneSemikolon(p.Item("Betrag").ToString) & delim &
                            ohneSemikolon(p.Item("Zahler").ToString) & delim &
                            ohneSemikolon(p.Item("Richtung").ToString) & delim &
                            ohneSemikolon(p.Item("HHST").ToString) & delim &
                            ohneSemikolon(p.Item("verschicktam").ToString) & delim
                 )
                Next
            Catch ex As Exception
                l("Fehler bei der Excelausgabed" & vbCrLf, ex)
            End Try
        End Sub

        Public Function ohneSemikolon(ByRef p1 As String) As String
            Try
                If String.IsNullOrEmpty(p1) Then
                    Return ""
                End If
                Dim temp$ = p1
                temp = temp.Trim
                temp = temp.Replace(";", "_")
                temp = temp.Replace(vbCrLf, "")
                Return temp
            Catch ex As Exception
                nachricht("Fehler in ohneSemikolon: ", ex)
                Return ""
            End Try
        End Function



#Region "IDisposable Support"
        Private disposedValue As Boolean ' So ermitteln Sie überflüssige Aufrufe
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    tab = Nothing
                    sw.Dispose()
                End If
            End If
            Me.disposedValue = True
        End Sub
        Public Sub Dispose() Implements IDisposable.Dispose
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

        Private Sub beliebig(vid As Integer)
            Try
                For i = 0 To tab.Columns.Count - 1
                    sw.Write(ohneSemikolon(tab.Columns(i).ColumnName.ToString) & delim)
                Next
                sw.WriteLine(delim)
                For Each p As DataRow In tab.AsEnumerable   'myGlobalz.sitzung.EreignisseRec.dt.AsEnumerable
                    For i = 0 To tab.Columns.Count - 1
                        sw.Write(ohneSemikolon(p.Item(i).ToString) & delim)
                    Next
                    sw.WriteLine(delim)
                Next
            Catch ex As Exception
                l("Fehler bei der Excelausgabeg" & vbCrLf, ex)
            End Try
        End Sub

        Private Sub nachricht_und_Mbox(text As String)

        End Sub

    End Class
End Namespace