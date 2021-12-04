Imports System.Data

Public Class clsCSVausgaben
    Implements IDisposable
    Public Shared Property csvdir As String = "\csv"
    Private sw As IO.StreamWriter
    Private exportFileFuerEigentuemer As String
    Private vid As Integer
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
    Public Property prefix() As String

    Private Shared Function GetTimestamp() As String
        Return Now.Year & Now.Day & Now.Hour & Now.Minute & Now.Second
    End Function
      Sub New(ByVal _prefix As String, ByVal _tab As DataTable, ByVal _vid As Integer )
        prefix = _prefix
        tab = _tab
        vid = _vid
        Dim timestamp As String = GetTimestamp()
        Try
            IO.Directory.CreateDirectory(clstart.mycsimple.Paradigma_local_root & csvdir)
        Catch ex As Exception

        End Try
     
        exportfile = String.Format("{0}\{1}_{2}_{3}_{4}.csv", clstart.mycsimple.Paradigma_local_root & csvdir, prefix, _vid, prefix, timestamp)
        exportFileFuerEigentuemer = String.Format("{0}\{1}_{2}.csv", clstart.mycsimple.Paradigma_local_root & csvdir, prefix, prefix)
        delim = ";"c
    End Sub
          Sub New(ByVal _prefix As String, ByVal _tab As DataTable, ByVal _vid As Integer,exportFileFullName As string)
        prefix = _prefix
        tab = _tab
        vid = _vid
        Dim timestamp As String = GetTimestamp()
        Try
            IO.Directory.CreateDirectory(clstart.mycsimple.Paradigma_local_root & csvdir)
        Catch ex As Exception

        End Try
     
        exportfile = exportFileFullName  
        exportFileFuerEigentuemer = exportFileFullName  
        delim = ";"c
    End Sub

    Function ausgeben() As String
        Try
            sw = New IO.StreamWriter(exportfile, False, CLstart.mycsimple.enc)
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
                Case "FlurstueckeFuerEigentuemer"
                    FlurstueckeFuerEigentuemer(vid)
                Case Else
                    nachricht("FEhler: diesen typ gibt es nicht!")
            End Select
            sw.Close()
            sw.Dispose()
            Return exportfile
        Catch ex As Exception
            nachricht_und_Mbox("Fehler bei der Excelausgabe, ausgeben:" & vbCrLf & ex.ToString)
            Return ""
        End Try
    End Function

    Public Sub Vorgaenge()
        Try
            sw.WriteLine( _
                        "EINGANG" & delim & _
                        "SACHGEBIETNR" & delim & _
                        "VORGANGSID" & delim & _
                        "GEMKRZ" & delim & _
                        "BESCHREIBUNG" & delim & _
                        "LETZTEBEARBEITUNG" & delim & _
                        "ZULETZT" & delim & _
                        "ORTSTERMIN" & delim & _
                        "STELLUNGNAHME" & delim & _
                        "ERLEDIGT" & delim & _
                        "AZ" & delim & _
                        "BEARBEITER" & delim & _
                        "PARAGRAF" & delim & _
                        "PROBAUGAZ" & delim & _
                        "ABGABEBA" & delim
               )
            For Each p As DataRow In tab.AsEnumerable   'myGlobalz.sitzung.EreignisseRec.dt.AsEnumerable
                sw.WriteLine( _
                            ohneSemikolon(p.Item("EINGANG").ToString) & delim & _
                            ohneSemikolon(p.Item("SACHGEBIETNR").ToString) & delim & _
                            ohneSemikolon(p.Item("VORGANGSID").ToString) & delim & _
                            ohneSemikolon(p.Item("GEMKRZ").ToString) & delim & _
                            ohneSemikolon(p.Item("BESCHREIBUNG").ToString) & delim & _
                            ohneSemikolon(p.Item("LETZTEBEARBEITUNG").ToString) & delim & _
                            ohneSemikolon(p.Item("LASTACTIONHEROE").ToString) & delim & _
                            ohneSemikolon(p.Item("ORTSTERMIN").ToString) & delim & _
                            ohneSemikolon(p.Item("STELLUNGNAHME").ToString) & delim & _
                            ohneSemikolon(p.Item("ERLEDIGT").ToString) & delim & _
                            ohneSemikolon(p.Item("AZ2").ToString) & delim & _
                            ohneSemikolon(p.Item("BEARBEITER").ToString) & delim & _
                            ohneSemikolon(p.Item("PARAGRAF").ToString) & delim & _
                            ohneSemikolon(p.Item("PROBAUGAZ").ToString) & delim & _
                            ohneSemikolon(p.Item("ABGABEBA").ToString) & delim)
            Next
        Catch ex As Exception
            nachricht_und_Mbox("Fehler bei der Excelausgabe" & vbCrLf & ex.ToString)
        End Try
    End Sub

    Public Sub VorgaengeALT()
        Try
            sw.WriteLine( _
                        "ERLEDIGT" & delim & _
                        "VORGANGSID" & delim & _
                        "ORTSTERMIN" & delim & _
                        "STELLUNGNAHME" & delim & _
                        "ABGABEBA" & delim & _
                        "GEMKRZ" & delim & _
                        "SACHGEBIETNR" & delim & _
                        "AZ" & delim & _
                        "BESCHREIBUNG" & delim & _
                        "BEARBEITER" & delim & _
                        "LETZTEBEARBEITUNG" & delim & _
                        "SACHGEBIETNR" & delim & _
                        "ZULETZT" & delim & _
                        "EINGANG" & delim & _
                        "PARAGRAF" & delim & _
                        "PROBAUGAZ" & delim
               )
            For Each p As DataRow In tab.AsEnumerable   'myGlobalz.sitzung.EreignisseRec.dt.AsEnumerable
                sw.WriteLine( _
                            ohneSemikolon(p.Item("ERLEDIGT").ToString) & delim & _
                            ohneSemikolon(p.Item("VORGANGSID").ToString) & delim & _
                            ohneSemikolon(p.Item("ORTSTERMIN").ToString) & delim & _
                            ohneSemikolon(p.Item("STELLUNGNAHME").ToString) & delim & _
                            ohneSemikolon(p.Item("ABGABEBA").ToString) & delim & _
                            ohneSemikolon(p.Item("GEMKRZ").ToString) & delim & _
                            ohneSemikolon(p.Item("SACHGEBIETNR").ToString) & delim & _
                            ohneSemikolon(p.Item("AZ2").ToString) & delim & _
                            ohneSemikolon(p.Item("BESCHREIBUNG").ToString) & delim & _
                            ohneSemikolon(p.Item("Bearbeiter").ToString) & delim & _
                            ohneSemikolon(p.Item("LETZTEBEARBEITUNG").ToString) & delim & _
                            ohneSemikolon(p.Item("Sachgebietnr").ToString) & delim & _
                            ohneSemikolon(p.Item("LASTACTIONHEROE").ToString) & delim & _
                            ohneSemikolon(p.Item("EINGANG").ToString) & delim & _
                            ohneSemikolon(p.Item("PARAGRAF").ToString) & delim & _
                            ohneSemikolon(p.Item("PROBAUGAZ").ToString) & delim)
            Next
        Catch ex As Exception
            nachricht_und_Mbox("Fehler bei der Excelausgabe" & vbCrLf & ex.ToString)
        End Try
    End Sub
    Public Sub Wiedervorlagen()
        Try
            sw.WriteLine( _
            "Fällig am" & delim & _
            "Az" & delim & _
            "Todo" & delim & _
            "Bemerkung" & delim & _
            "Wartenauf" & delim & _
            "Beschreibung" & delim & _
            "erledigt" & delim
               )
            For Each p As DataRow In tab.AsEnumerable   'myGlobalz.sitzung.EreignisseRec.dt.AsEnumerable
                sw.WriteLine( _
                ohneSemikolon(p.Item("Datum").ToString) & delim & _
               ohneSemikolon(p.Item("az").ToString) & delim & _
               ohneSemikolon(p.Item("Todo").ToString) & delim & _
              ohneSemikolon(p.Item("Bemerkung").ToString) & delim & _
             ohneSemikolon(p.Item("Wartenauf").ToString) & delim & _
              ohneSemikolon(p.Item("Beschreibung").ToString) & delim & _
              ohneSemikolon(p.Item("erledigt").ToString) & delim
                 )
            Next
        Catch ex As Exception
            nachricht_und_Mbox("Fehler bei der Excelausgabe" & vbCrLf & ex.ToString)
        End Try
    End Sub
    Public Sub Beteiligte()
        Try
            sw.WriteLine( _
               "Rolle" & delim & _
               "Anrede" & delim & _
               "Namenszusatz" & delim & _
               "Vorname" & delim & _
               "Name" & delim & _
               "Gemeindename" & delim & _
               "Strasse" & delim & _
               "FFEmail" & delim & _
               "FFTelefon1" & delim & _
               "Orgname" & delim & _
               "orgzusatz".ToString & delim _
                )
            For Each p As DataRow In tab.AsEnumerable   'myGlobalz.sitzung.EreignisseRec.dt.AsEnumerable
                sw.WriteLine( _
                 ohneSemikolon(p.Item("Rolle").ToString) & delim & _
                ohneSemikolon(p.Item("Anrede").ToString) & delim & _
               ohneSemikolon(p.Item("Namenszusatz").ToString) & delim & _
               ohneSemikolon(p.Item("Vorname").ToString) & delim & _
               ohneSemikolon(p.Item("NachName").ToString) & delim & _
               ohneSemikolon(p.Item("Gemeindename").ToString) & delim & _
               ohneSemikolon(p.Item("Strasse").ToString) & delim & _
                ohneSemikolon(p.Item("FFEmail").ToString) & delim & _
                ohneSemikolon(p.Item("FFTelefon1").ToString) & delim & _
                ohneSemikolon(p.Item("Orgname").ToString) & delim & _
               ohneSemikolon(p.Item("orgzusatz").ToString) & delim _
                 )
            Next
        Catch ex As Exception
            nachricht_und_Mbox("Fehler bei der Excelausgabe" & vbCrLf & ex.ToString)
        End Try
    End Sub

    Public Sub Ereignisse()
        Try
            sw.WriteLine( _
                "Datum" & delim & _
                "Art" & delim & _
                "Beschreibung" & delim
                 )
            For Each p As DataRow In tab.AsEnumerable   'myGlobalz.sitzung.EreignisseRec.dt.AsEnumerable
                sw.WriteLine( _
                 ohneSemikolon(p.Item("Datum").ToString) & delim & _
                 ohneSemikolon(p.Item("Art").ToString) & delim & _
                ohneSemikolon(p.Item("Beschreibung").ToString) & delim
                 )
            Next
        Catch ex As Exception
            nachricht_und_Mbox("Fehler bei der Excelausgabe" & vbCrLf & ex.ToString)
        End Try
    End Sub

    Public Sub Dokumente()
        Try
            sw.WriteLine( _
                "Filedatum" & delim & _
                "Typ " & delim &
                "Dateinameext" & delim &
                "Beschreibung" & delim
                )
            For Each p As DataRow In tab.AsEnumerable   'myGlobalz.sitzung.EreignisseRec.dt.AsEnumerable
                sw.WriteLine( _
                 ohneSemikolon(p.Item("Filedatum").ToString) & delim & _
               ohneSemikolon(p.Item("Typ").ToString) & delim & _
              ohneSemikolon(p.Item("Dateinameext").ToString) & delim & _
              ohneSemikolon(p.Item("Beschreibung").ToString) & delim
                 )
            Next
        Catch ex As Exception
            nachricht_und_Mbox("Fehler bei der Excelausgabe" & vbCrLf & ex.ToString)
        End Try
    End Sub

    Public Sub Raumbezuege()
        Try
            sw.WriteLine( _
               "Typ" & delim &
               "Titel" & delim &
                   "Abstract" & delim &
                       "qm" & delim &
               "freitext" & delim
                )
            If tab IsNot Nothing AndAlso tab.AsEnumerable IsNot Nothing AndAlso tab.AsEnumerable.Count > 0 Then
                For Each p As DataRow In tab.AsEnumerable   'myGlobalz.sitzung.EreignisseRec.dt.AsEnumerable
                    sw.WriteLine( _
                    ohneSemikolon(p.Item("Typ").ToString) & delim &
                    ohneSemikolon(p.Item("Titel").ToString) & delim &
                    ohneSemikolon(p.Item("Abstract").ToString) & delim &
                    ohneSemikolon(p.Item("flaecheqm").ToString) & delim &
                    ohneSemikolon(p.Item("FREITEXT").ToString) & delim
                     )
                Next
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler bei der Excelausgabe" & vbCrLf & ex.ToString)
        End Try
    End Sub

    Public Sub Zahlungen()
        Try
            sw.WriteLine( _
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
            For Each p As DataRow In tab.AsEnumerable   'myGlobalz.sitzung.EreignisseRec.dt.AsEnumerable
                sw.WriteLine( _
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
            nachricht_und_Mbox("Fehler bei der Excelausgabe" & vbCrLf & ex.ToString)
        End Try
    End Sub

    Private Function ohneSemikolon(ByRef p1 As String) As String
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
            nachricht("Fehler in ohneSemikolon: " & ex.ToString)
            Return ""
        End Try
    End Function

    Public Sub FlurstueckeFuerEigentuemer(ByVal vid As Integer)
        Dim cnt As Integer
        Try
            Dim swe = New IO.StreamWriter(exportFileFuerEigentuemer, False, CLstart.mycsimple.enc)
            myGlobalz.sitzung.raumbezugsRec.mydb.SQL = "select * from paraflurstueck " &
                "where id in (select sekid from pf_sekid2vid where vorgangsid=" & vid & ")" &
                 " order by gemcode,flur,zaehler,nenner"
            Dim hinweis As String = myGlobalz.sitzung.raumbezugsRec.getDataDT()
            swe.WriteLine( _
                        "gemcode" & delim &
                        "flur" & delim &
                        "zaehler" & delim &
                        "nenner" & delim &
                        "fs" & delim
           )
            tab = myGlobalz.sitzung.raumbezugsRec.dt
            For i = 0 To myGlobalz.sitzung.raumbezugsRec.dt.Rows.Count - 1
                '       If tab IsNot Nothing AndAlso tab.AsEnumerable IsNot Nothing AndAlso tab.AsEnumerable.Count > 0 Then
                ' For Each p As DataRow In myGlobalz.sitzung.raumbezugsRec.dt.AsEnumerable   'myGlobalz.sitzung.EreignisseRec.dt.AsEnumerable
                swe.WriteLine( _
                ohneSemikolon(myGlobalz.sitzung.raumbezugsRec.dt.Rows(i).Item("gemcode").ToString) & delim &
                ohneSemikolon(myGlobalz.sitzung.raumbezugsRec.dt.Rows(i).Item("flur").ToString) & delim &
                ohneSemikolon(myGlobalz.sitzung.raumbezugsRec.dt.Rows(i).Item("zaehler").ToString) & delim &
                ohneSemikolon(myGlobalz.sitzung.raumbezugsRec.dt.Rows(i).Item("nenner").ToString) & delim &
                ohneSemikolon(myGlobalz.sitzung.raumbezugsRec.dt.Rows(i).Item("fs").ToString) & delim)
                cnt += 1
                'Next
                '  End If
            Next
          '  swe.Close()
            swe.Dispose()
        Catch ex As Exception
            nachricht_und_Mbox("Fehler bei der Excelausgabe" & vbCrLf & ex.ToString)
        End Try
    End Sub
 
 #Region "IDisposable Support"
    Private disposedValue As Boolean' So ermitteln Sie überflüssige Aufrufe
    Protected     Overridable     Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
             tab = Nothing
                sw.Dispose
            End If
        End If
        Me.disposedValue = True
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
End Class
