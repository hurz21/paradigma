
'Namespace nachZielKopieren
'    Class DolumentImArchivKopieren
'        'Private Sub New()

''End Sub
'Private Shared Function GetBeschreibung(ByVal dokument As Dokument) As String
'    Dim a As String = "Kopie von: " & dokument.DateinameMitExtension & " (" & dokument.Beschreibung & ")"
'    If a.Length > 940 Then
'        a = a.Substring(0, 940)
'    End If
'    Return a
'End Function
'Private Shared Sub setDokumentTypLength(ByRef dokument As Dokument)
'    Try
'        If dokument.Typ.Length > 4 Then
'            dokument.Typ = dokument.Typ.Substring(0, 3)
'        End If
'    Catch ex As Exception
'        nachricht("fehler in DolumentImArchivKopieren setDokumentTypLength: " & ex.ToString & "/" & dokument.Typ)
'    End Try
'End Sub
'Shared Function exe(dokument As Dokument) As Boolean
'    'kopie unter neunem namen im checkoutdir anlegen
'    'kopie Einchecken
'    'refreshen
'    Dim neuername As String
'    Dim erfolg As Boolean
'    Try
'        neuername = getNeuerName(dokument)
'        If String.IsNullOrEmpty(neuername) Then
'            Return False
'        End If
'        erfolg = UmkopiereDokumentInCheckout(dokument, neuername)
'        If erfolg Then
'            dokument.Beschreibung = GetBeschreibung(dokument)
'            setDokumentTypLength(dokument)

'            erfolg = myGlobalz.Arc.checkINDoku(neuername,
'                                               dokument.Beschreibung,
'                                               0,
'                                               False,
'                                               "",
'                                               myGlobalz.sitzung.defineArchivVorgangsDir(myGlobalz.sitzung.aktVorgangsID),
'                                               myGlobalz.sitzung.aktVorgangsID,
'                                               False,
'                                               Now,
'                                               myGlobalz.sitzung.aktDokument.DocID,
'                                               myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir)
'            Return erfolg
'        Else
'            Return False
'        End If
'    Catch ex As Exception
'        nachricht("fehler in DolumentImArchivKopieren: " & ex.ToString)
'        Return False
'    End Try
'End Function

'Private Shared Function UmkopiereDokumentInCheckout(dokument As Dokument, ByRef neuername As String) As Boolean
'    nachricht("UnbenenneDokumentInCheckout ---------------- ")
'    nachricht("altername:" & dokument.DateinameMitExtension)
'    nachricht("neuername:" & neuername)
'    Try
'        Dim fi As New IO.FileInfo(dokument.FullnameCheckout)
'        neuername = IO.Path.Combine(fi.DirectoryName, neuername)
'        nachricht("neuername:" & neuername)
'        fi.CopyTo(neuername, True)
'        nachricht("UnbenenneDokumentInCheckout: ok")
'        Return True
'    Catch ex As Exception
'        nachricht("fehler in UnbenenneDokumentInCheckout: " & ex.ToString)
'        Return False
'    End Try
'End Function

'        Private Shared Function getNeuerName(ByVal dokument As Dokument) As String
'            Dim a As String = Microsoft.VisualBasic.Interaction.InputBox(
'                                                      "Bitte geben Sie einen neuen Namen für die Datei an:" & vbCrLf & _
'                                                      "  (bitte vermeiden Sie Umlaute und Leerzeichen) " & vbCrLf & vbCrLf,
'                                                      "Neuen Dateinamen definieren",
'                                                      "KOPIE_" & dokument.DateinameMitExtension)
'            Try
'                If String.IsNullOrEmpty(a) Then
'                    MsgBox("Keine Eingabe, Abbruch", MsgBoxStyle.Critical, "Eingabe ist leer")
'                    Return ""
'                End If
'                If LIBgemeinsames.clsString.enthaeltUnerlaubteZeichen(a) Then
'                    MsgBox("Der Dateiname enthält unerlaubte Zeichen (z.B. < > ?  : | \ / *) !!! (Abbruch)", MsgBoxStyle.Critical, "Unerlaubte Zeichen")
'                    Return ""
'                End If
'                If a.ToLower.Trim = dokument.DateinameMitExtension.ToLower.Trim Then
'                    MsgBox("Sie müssen einen neuen Namen vergeben !!! (Abbruch)", MsgBoxStyle.Critical, "Alter Name = Neuer Name")
'                    Return ""
'                End If
'                If Not a.ToLower.Trim.Contains(".") Then
'                    MsgBox("Sie müssen einen Namen mit gültiger Dokumenttyperweiterung vergeben !!! (Abbruch)", MsgBoxStyle.Critical, "Dokumenttyperweiterung fehlt")
'                    Return ""
'                End If
'                Return a
'            Catch ex As Exception
'                nachricht("fehler in getNeuerName: " & ex.ToString)
'                Return ""
'            End Try
'        End Function
'    End Class

'End Namespace
