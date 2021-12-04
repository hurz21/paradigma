Module dokumenteTools
    Public Sub presDokusAusgewaehltMarkieren(valju As Boolean, dlist As List(Of clsPresDokumente))
        If dlist Is Nothing Then Exit Sub
        For Each ele As clsPresDokumente In dlist
            ele.ausgewaehlt = valju
        Next
    End Sub

    Public Function alleDokusUndFotosLoeschen(modus As String) As Integer
        nachricht("USERAKTION: alle dokumente und fotos löschen")
        Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(CStr(myGlobalz.sitzung.aktVorgangsID), modus, True, 0) ' nach myGlobalz.Arc.ArcRec
        If bresult Then
            Psession.presDokus = detail_dokuauswahl.dokuDTnachObj(myGlobalz.Arc.ArcRec.dt.Copy)
            presDokusAusgewaehltMarkieren(True, Psession.presDokus)
            Dim vorheranzahl As Integer = Psession.presDokus.Count
            Dim icount As Integer = Dokus_MehrfachLoeschen()
            MessageBox.Show("Es wurden " & icount & " von " & vorheranzahl & " Dokumenten gelöscht.")
            Return icount
        End If
        Return 0
    End Function

    Public Function Dokus_MehrfachLoeschen() As Integer
        Dim messi As New MessageBoxResult
        If Psession.presDokus.Count > 0 Then
            messi = MessageBox.Show("Objekte wirklich löschen ?" & vbCrLf,
                                    "Dokumente löschen ?",
                                     MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
            If messi = MessageBoxResult.Yes Then
                Dim icount As Integer
                icount = detail_dokuauswahl.AlleMarkiertenDokumenteLoeschen(Psession.presDokus)
                nachricht(icount & " Objekte gelöscht")
                nachricht("USERAKTION: " & icount & " dokus wurden gelöscht")
                Return icount
            Else
                Return 0
            End If
        Else
            Return 0
        End If
    End Function

    Public Function Beteiligte_MehrfachLoeschen() As Integer
        Dim messi As New MessageBoxResult
        If Psession.presBeteiligte.Count > 0 Then
            messi = MessageBox.Show("Objekte wirklich löschen ?" & vbCrLf,
                                    "Beteiligte löschen ?",
                                     MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
            If messi = MessageBoxResult.Yes Then
                Dim icount As Integer
                'icount = detail_dokuauswahl.AlleMarkiertenDokumenteLoeschen(Psession.presDokus)
                icount = detail_dokuauswahl.AlleMarkiertenBeteiligteLoeschen(Psession.presBeteiligte)
                nachricht(icount & " Objekte gelöscht")
                nachricht("USERAKTION: " & icount & " Beteiligte wurden gelöscht")
                Return icount
            Else
                Return 0
            End If
        Else
            Return 0
        End If
    End Function
    Public Sub alleDokuUndFotosZuFremdvorgangKopieren(altemarkierungen As List(Of Integer))
        nachricht("USERAKTION: alle dokumente und fotos zu anderem vorgang kopieren")
        Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(CStr(myGlobalz.sitzung.aktVorgangsID), "beides", True, 0) ' nach myGlobalz.Arc.ArcRec
        If bresult Then
            Psession.presDokus = detail_dokuauswahl.dokuDTnachObj(myGlobalz.Arc.ArcRec.dt.Copy)
            presDokusAusgewaehltMarkieren(True, Psession.presDokus)
            Dim icount As Integer = detail_dokuauswahl.Dokus_MehrfachKopieren(Psession.presDokus, altemarkierungen)
            MessageBox.Show("Es wurden " & icount & " von " & Psession.presDokus.Count & " Dokumenten kopiert.")
        End If
        'If detailsTools.zielvorgangsidistOK(zielid) Then
        ' detailsTools.AlleDokumentenKopieren(myGlobalz.sitzung.aktVorgangsID, CInt(zielid)) ' myGlobalz.sitzung.beteiligteREC.dt
        'End If
    End Sub
    Public Sub alleDokusRevisionssicherMachen()
        nachricht("USERAKTION: alle dokumente revisionssicher speichern")
        Dim icount As Integer = clsEreignisTools.AlleDokumentenRevisionssicherMachen(myGlobalz.sitzung.aktVorgangsID, "")
        MessageBox.Show("Es wurden " & icount & " von " & Psession.presDokus.Count & " Dokumenten revisionsgesichert.")
    End Sub
    Sub alleWordDokusNachPdfaKopieren()
        nachricht("USERAKTION: alle WordDokusNachPdfaKopieren")
        Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(CStr(myGlobalz.sitzung.aktVorgangsID), "beides", True, 0)
        If bresult Then
            Psession.presDokus = detail_dokuauswahl.dokuDTnachObj(myGlobalz.Arc.ArcRec.dt.Copy)

            Psession.presDokus = reduceToWorddoks(Psession.presDokus)

            PdfaKopieAnlegen(Psession.presDokus)
            'presDokusAusgewaehltMarkieren(True, Psession.presDokus)
            'Dim icount As Integer = detail_dokuauswahl.Dokus_MehrfachKopieren()
            ' MessageBox.Show("Es wurden " & icount & " von " & Psession.presDokus.Count & " Dokumenten kopiert.")
        End If
    End Sub

    Private Function reduceToWorddoks(list As List(Of clsPresDokumente)) As List(Of clsPresDokumente)
        Dim wordlist As New List(Of clsPresDokumente)
        For Each dok As clsPresDokumente In list
            dok.getDokTyp()
            If (dok.DokTyp = DokumentenTyp.RTF Or
                dok.DokTyp = DokumentenTyp.DOC) Then
                wordlist.Add(dok)
            End If
        Next
        Return wordlist
    End Function

    Private Sub PdfaKopieAnlegen(list As List(Of clsPresDokumente))
        Dim diag As New winPDFA(list, myGlobalz.Arc.lokalerCheckoutcache)
        diag.ShowDialog

        'Dim erfolg As Boolean
        'For Each dok As clsPresDokumente In list
        '    myGlobalz.Arc.einzeldokument_auschecken(myGlobalz.Arc.lokalerCheckoutcache & dok.VorgangsID, dok)
        '    erfolg = nachZielKopieren.AlsPDFAkopieren.exe(dok)
        '    If Not erfolg Then
        '        MsgBox("PDFA-Konvertierung gescheitert: " & dok.DateinameMitExtension)
        '    End If
        'Next
    End Sub
    Public Sub presBeteiligtenAusgewaehltMarkieren(valju As Boolean, dlist As List(Of Person))
        If dlist Is Nothing Then Exit Sub
        For Each ele As Person In dlist
            ele.ausgewaehlt = valju
        Next
    End Sub
End Module
