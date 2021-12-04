Imports System.Data

Public Class detail_dokuauswahl

    Shared Function dokuDTnachObj(dataTable As DataTable) As List(Of clsPresDokumente)
        Dim dok As clsPresDokumente
        Dim doklist As New List(Of clsPresDokumente)
        For Each item As DataRow In dataTable.AsEnumerable
            dok = New clsPresDokumente
            DokArc.DokumentDatarow2Obj(item, dok)
            doklist.Add(dok)
        Next
        Return doklist
    End Function

    Shared Function AlleMarkiertenDokumenteLoeschen(dlist As List(Of clsPresDokumente)) As Integer
        Dim icount As Integer = 0
        For Each dockument As clsPresDokumente In dlist
            If dockument.ausgewaehlt And (Not (dockument.istNurVerwandt)) And (Not (dockument.revisionssicher)) Then
                'detailsTools.LoescheDokument(dockument)
                nachricht("USERAKTION: ausgewähltes dokument löschen: " & dockument.DateinameMitExtension)
                dockument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache)
                checkout.checkout(dockument, myGlobalz.sitzung.aktVorgangsID) 'checkout findet IMMER statt
                DokArc.aktDokumentLoschen(dockument)
                DokArc.ausCheckoutlisteEntfernen(dockument, myGlobalz.sitzung.checkoutDokuList)
                nachricht("USERAKTION: doku löschen: " & dockument.DateinameMitExtension)
                clstart.myc.aLog.wer = myGlobalz.sitzung.aktBearbeiter.Initiale
                clstart.myc.aLog.vorgang = myGlobalz.sitzung.aktVorgangsID.ToString
                clstart.myc.aLog.komponente = "detail"
                clstart.myc.aLog.aktion = "dokument löschen: " & dockument.DateinameMitExtension
                clstart.myc.aLog.log()
                icount += 1
            End If
        Next
        Return icount
    End Function

    Shared Function AlleMarkiertenDokumentenKopieren(dlist As List(Of clsPresDokumente), quellVid As Integer, ByVal zielvorgang As Vorgang) As Integer
        '  Dim bresult As Boolean = DokArcTools.dokusVonVorgangHolen.execute(quellVorgangsID, "beides") ' nach myGlobalz.Arc.ArcRec
        Dim kopiert As Integer = 0
        Dim kopierteEreignisse As Integer = 0
        Dim NumDir As String = ""
        ' NumDir = myGlobalz.Arc.getFreshNumDir(myGlobalz.sitzung.defineArchivVorgangsDir(CInt(zielvorgang.Stammdaten.ID)))
        If zielvorgang.Stammdaten.ArchivSubdir.IsNothingOrEmpty Then
            zielvorgang.Stammdaten.ArchivSubdir = myGlobalz.sitzung.defineArchivVorgangsDir(CInt(zielvorgang.Stammdaten.ID))
        End If
        NumDir = myGlobalz.Arc.getFreshNumDir(zielvorgang.Stammdaten.ArchivSubdir)
        For Each dockument As clsPresDokumente In Psession.presDokus
            If dockument.ausgewaehlt Then
                dockument.VorgangsID = quellVid
                dockument.makeFullname_Checkout(quellVid, myGlobalz.Arc.lokalerCheckoutcache)
                checkout.checkout(dockument, quellVid)   'checkout findet IMMER statt
                If detailsTools.dokumentschonImVorgangvorhanden(dockument, CInt(zielvorgang.Stammdaten.ID)) Then
                    Continue For
                End If
                Dim datei As String = dockument.FullnameCheckout
                Dim Beschreibung As String = dockument.Beschreibung
                Dim ereignisid As Integer = DokArcOracle.getEreignisID4DokId(dockument.DocID)
                myGlobalz.sitzung.aktDokument.clear(CLstart.mycsimple.MeinNULLDatumAlsDate)
                'myGlobalz.sitzung.aktEreignis.ID ist unbekannt
                If ereignisid > 0 Then
                    'zu diesem Dokument gibt es ein Ereignis
                    Dim erfolg As Boolean = clsEreignisTools.Ereigniskopieren(ereignisid, CInt(zielvorgang.Stammdaten.ID), False)
                    dockument.EreignisID = myGlobalz.sitzung.aktEreignis.ID
                    kopierteEreignisse += 1
                End If
                Dim erfolgreich As Boolean = myGlobalz.Arc.checkINDoku(datei,
                                                                dockument.EreignisID,
                                                                Beschreibung,
                                                                CInt(zielvorgang.Stammdaten.ID),
                                                                False,
                                                                NumDir,
                                                                dockument.Filedatum,
                                                                dockument.DocID,
                                                                zielvorgang.Stammdaten.ArchivSubdir)
                If erfolgreich Then
                    nachricht("Checkin  erfolgreich: " & datei)
                    kopiert += 1
                Else
                    nachricht_und_Mbox("Checkin nicht erfolgreich: " & datei)
                End If
                clstart.myc.aLog.wer = myGlobalz.sitzung.aktBearbeiter.Initiale
                clstart.myc.aLog.vorgang = myGlobalz.sitzung.aktVorgangsID.ToString
                clstart.myc.aLog.komponente = "detail"
                clstart.myc.aLog.aktion = "dokument in anderen Vorgang (" & zielvorgang.Stammdaten.ID & ") kopieren: " &
                    dockument.DateinameMitExtension
                clstart.myc.aLog.log()
            End If
        Next
        nachricht("Kopieren von Dokumenten: Es wurden " & kopiert & " Dokumente kopiert")
        Return kopiert
    End Function


    Shared Function initZielvorgang(ByRef zielvorgang As Vorgang) As Boolean
        Return clsVorgangCTRL.leseVorgangvonDBaufObjekt(CInt(zielvorgang.Stammdaten.ID), zielvorgang.Stammdaten, myGlobalz.sitzung.VorgangREC)
    End Function
    Public Shared ReadOnly Property ZielvorgangsidInput() As String
        Get
            Dim a$ = Microsoft.VisualBasic.Interaction.InputBox("Bitte geben Sie hier die ParadigmaNr des Ziel-Vorgangs ein:" & vbCrLf & _
                                                      "  " & vbCrLf,
                                                      "Ziel definieren",
                                                      "")
            Return a$
        End Get
    End Property

    Public Shared Function zielvorgangsidIstInOrdnung(ByRef zielid As String) As Boolean
        zielid$ = ZielvorgangsidInput
        If String.IsNullOrEmpty(zielid) Then Return False
        If Not IsNumeric(zielid) Then Return False
        Return True
    End Function

    Public Shared Function zielvorgangsidistOK(ByRef zielid As String) As Boolean
        If Not zielvorgangsidIstInOrdnung(zielid) Then
            MessageBox.Show("Sie haben keine Eingabe gemacht - Die Aktion wird abgebrochen!", "Keine Eingabe - Abbruch", MessageBoxButton.OK)
            Return False
        End If
        Return True
    End Function
    ''' <summary>
    ''' 
    ''' </summary>
    ''' 
    ''' <returns> anzahl der kopierten Dokumente</returns>
    ''' <returns>-1  zielvorgangsnummer ist nicht ok</returns>
    ''' <returns>-2  zielvorgang existiert nicht</returns>
    ''' <remarks></remarks>
    Public Shared Function Dokus_MehrfachKopieren() As Integer
        Dim zielid As String = "" '25017
        Dim icount As Integer = 0
        Dim zielvorgang As New Vorgang
        If zielvorgangsidistOK(zielid) Then
            zielvorgang.Stammdaten.ID = CLng(zielid)
            Dim erfolg As Boolean = detail_dokuauswahl.initZielvorgang(zielvorgang)
            If erfolg Then
                icount = detail_dokuauswahl.AlleMarkiertenDokumentenKopieren(Psession.presDokus, myGlobalz.sitzung.aktVorgangsID, zielvorgang)
            Else
                Return -2 ' zielvorgang existiert nicht
            End If
        Else
            Return -1 ' zielvorgangsnummer ist nicht ok
        End If
        zielvorgang = Nothing
        Return icount
    End Function


    Shared Function AlleMarkiertenDokumenteRevisionssichern(list As List(Of clsPresDokumente)) As Integer
        Dim icount As Integer = 0
        For Each dockument As clsPresDokumente In list
            If dockument.ausgewaehlt And (Not (dockument.istNurVerwandt)) Then
                nachricht("USERAKTION: ausgewähltes dokument revisionssichern: " & dockument.DateinameMitExtension)
                If dockument.revisionssicher = True Then
                    MsgBox(dockument.DateinameMitExtension & "=> war bereits revisionssicher!")
                Else
                    dockument.revisionssicher = True
                    Dim result As Integer = DokArcTools.dokUpdate.execute(dockument.DocID,
                                                          dockument.istVeraltet,
                                                          dockument.Beschreibung,
                                                          dockument.revisionssicher,
                                                          dockument.Filedatum,
                                                          dockument.EXIFlat,
                                                          dockument.EXIFlon)
                    If result > 0 Then
                        nachricht("USERAKTION: doku revisionsgesichert: " & dockument.DateinameMitExtension)
                        clstart.myc.aLog.wer = myGlobalz.sitzung.aktBearbeiter.Initiale
                        clstart.myc.aLog.vorgang = myGlobalz.sitzung.aktVorgangsID.ToString
                        clstart.myc.aLog.komponente = "detail"
                        clstart.myc.aLog.aktion = "dokument revisionsgesichert: " & dockument.DateinameMitExtension
                        clstart.myc.aLog.log()
                        icount += 1
                    Else
                        MsgBox("Speichern nicht erfolgreich. Formular schließen")
                    End If
                End If
            End If
        Next
        Return icount
    End Function



    Private Shared Sub anhangstringBilden(ByRef anhangstring As String, ByRef icount As Integer, dlist As List(Of clsPresDokumente))
        If dlist Is Nothing OrElse dlist.Count < 1 Then Exit Sub
        Try
            For Each dockument As clsPresDokumente In dlist
                If dockument.ausgewaehlt And (Not dockument.istNurVerwandt) Then
                    dockument.makeFullname_Checkout(myGlobalz.sitzung.aktVorgangsID, myGlobalz.Arc.lokalerCheckoutcache)
                    checkout.checkout(dockument, myGlobalz.sitzung.aktVorgangsID)
                    anhangstring = anhangstring & myGlobalz.anhangtrenner & dockument.FullnameCheckout
                    icount += 1
                    clstart.myc.aLog.wer = myGlobalz.sitzung.aktBearbeiter.Initiale
                    clstart.myc.aLog.vorgang = myGlobalz.sitzung.aktVorgangsID.ToString
                    clstart.myc.aLog.komponente = "detail"
                    clstart.myc.aLog.aktion = "dokument gemailt: " & dockument.DateinameMitExtension
                    clstart.myc.aLog.log()
                End If
            Next
        Catch ex As Exception
            nachricht("fehler in anhangstringBilden: " & ex.ToString)
        End Try
    End Sub

    Shared Function Dokus_MehrfachMailen() As Integer
        Dim anhangstring As String = ""
        Dim icount As Integer = 0
        Try
            anhangstringBilden(anhangstring, icount, Psession.presDokus)
            anhangstringBilden(anhangstring, icount, Psession.presFotos)
            If anhangstring.IsNothingOrEmpty Then
                MsgBox("Es wurden keine Dokumente ausgewählt!")
            Else
                Dim abbruch As Boolean = glob2.EmailFormOEffnen(myGlobalz.sitzung.aktPerson.Kontakt.elektr.Email, "", "",
                                     anhangstring,
                                     myGlobalz.sitzung.aktBearbeiter.Kontakt.elektr.Email,
                                     False)
                If abbruch Then
                    Return 0
                End If
            End If
            Return icount
        Catch ex As Exception
            nachricht("fehler in Dokus_MehrfachMailen: " & ex.ToString)
        End Try
    End Function
    Shared Function anzahlAusgewaehlt(dlist As List(Of clsPresDokumente)) As Integer
        Dim icount As Integer = 0
        Try
            For Each dockument As clsPresDokumente In dlist
                If dockument.ausgewaehlt Then
                    icount += 1
                End If
            Next
            Return icount
        Catch ex As Exception
            nachricht("fehler in anzahlAusgewaehlt: " & ex.ToString)
            Return 0
        End Try
    End Function

    Shared Function getAnzahlAusgewaehlterDokumente() As Integer
        Dim icount As Integer = 0
        icount += detail_dokuauswahl.anzahlAusgewaehlt(Psession.presDokus)
        icount += detail_dokuauswahl.anzahlAusgewaehlt(Psession.presFotos)
        Return icount
    End Function

    Private Shared Function ausgewDokusDemAktEreignisHinzufuegenExtracted(ByVal eid As Integer, ByVal dockument As clsPresDokumente) As Integer
        Try
            If eid > 0 Then
                nachricht("in		checkIN_Database KopplungMitEreignis-----------------------------------------------------")
                Dim koppereignis As Integer = DokArcTools.KopplungMitEreignis.execute(dockument.DocID, eid)
                If koppereignis > 0 Then
                    nachricht("in		checkIN_Database KopplungMitEreignis erfolgreich")
                Else
                    nachricht("in		checkIN_Database KopplungMitEreignis NICHT erfolgreich")
                End If
                Return koppereignis
            End If
        Catch ex As Exception
            nachricht("Fehler in kopplenMitEreignis: ", ex)
        End Try
    End Function

    Shared Function ausgewDokusDemAktEreignisHinzufuegen(eid As Integer, dlist As List(Of clsPresDokumente)) As String
        Dim icount As Integer = 0
        Dim hinweis As String = ""
        Try
            For Each dockument As clsPresDokumente In dlist
                If dockument.ausgewaehlt Then
                    If Not (dockument.istNurVerwandt) Then
                        If dokumentHatSchonEreigniskopplung(dockument) Then
                            hinweis = hinweis & "-> Dokument ist schon an ein anderes Ereignis gekoppelt: " & dockument.DateinameMitExtension & Environment.NewLine
                        Else
                            If ausgewDokusDemAktEreignisHinzufuegenExtracted(eid, dockument) > 0 Then
                                hinweis = hinweis & "hinzugefügt: " & dockument.DateinameMitExtension & Environment.NewLine
                                icount += 1
                            End If
                        End If
                    Else
                        hinweis = hinweis & "-> Dokument ist verwandt und kann daher nicht angekoppelt werden: " & dockument.DateinameMitExtension & Environment.NewLine
                    End If
                End If
            Next
            Return hinweis
        Catch ex As Exception
            nachricht("fehler in anzahlAusgewaehlt: " & ex.ToString)
            Return "-1"
        End Try

    End Function

    Private Shared Function dokumentHatSchonEreigniskopplung(dockument As clsPresDokumente) As Boolean
        Try
            myGlobalz.Arc.ArcRec.mydb.SQL = "select id from ereignis2dok where dokumentid=" & dockument.DocID
            Dim hinweis As String = myGlobalz.Arc.ArcRec.getDataDT()
            If myGlobalz.Arc.ArcRec.dt.Rows.Count > 0 Then
                Return True
            Else
                Return False
            End If
        Catch ex As Exception
            nachricht("fehler in dokumentHatSchonEreigniskopplung: " & ex.ToString)
            Return False
        End Try
    End Function
    Shared Function dokListenMergen(doks As List(Of clsPresDokumente), fotos As List(Of clsPresDokumente)) As List(Of clsPresDokumente)
        Dim mergeListe As New List(Of clsPresDokumente)
        Try
            For Each dockument As clsPresDokumente In doks
                mergeListe.Add(dockument)
            Next
            For Each dockument As clsPresDokumente In fotos
                mergeListe.Add(dockument)
            Next
            Return mergeListe
        Catch ex As Exception
            nachricht("fehler in dokListenMergen: " & ex.ToString)
            Return Nothing
        End Try
    End Function

    Public Shared Function fotodokumentNachPresDokumentKonvertieren(ByVal fto As clsFotoDokument) As clsPresDokumente
        Dim newdok As clsPresDokumente
        newdok = New clsPresDokumente
        Try
            newdok.ausgewaehlt = fto.ausgewaehlt
            newdok.Beschreibung = fto.Beschreibung
            newdok.Checkindatum = fto.Checkindatum
            newdok.DateinameMitExtension = fto.DateinameMitExtension
            newdok.DocID = fto.DocID
            newdok.DokTyp = fto.DokTyp
            newdok.dokumentPfad = fto.dokumentPfad
            newdok.EreignisID = fto.EreignisID
            newdok.ExifDatum = fto.ExifDatum
            newdok.EXIFdir = fto.EXIFdir
            newdok.EXIFhersteller = fto.EXIFhersteller
            newdok.EXIFlat = fto.EXIFlat
            newdok.EXIFlon = fto.EXIFlon
            newdok.Filedatum = fto.Filedatum
            newdok.FullnameCheckout = fto.FullnameCheckout
            newdok.FullnameImArchiv = fto.FullnameImArchiv
            newdok.Handlenr = fto.Handlenr
            newdok.Initiale = fto.Initiale
            newdok.istNurVerwandt = fto.istNurVerwandt
            newdok.VorgangsID = fto.VorgangsID
            newdok.revisionssicher = fto.revisionssicher
            newdok.Typ = fto.Typ
            Return newdok
        Catch ex As Exception
            nachricht("Fehler in fotodokumentNachPresDokumentKonvertieren: ", ex)
            Return Nothing
        End Try
    End Function
End Class
