Namespace StammToolsNs
    Public Class setWindowTitel

        Public Shared Function exe(ByVal _modus As String, ByVal Formname As String) As String
            Dim lokname As String = "Formular: " & Formname$ & " - "
            Dim aktion As String = ""
            If String.IsNullOrEmpty(_modus) Then
                Return "Modus unbekannt"
                Exit Function
            End If
            If _modus = "neu" Then aktion = " neu anlegen, "
            If _modus = "edit" Then aktion = " einsehen / ändern, "
            Return lokname & aktion$ & " Vorgang: " & myGlobalz.sitzung.aktVorgangsID & ", " & myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt & " "
            Return "Fehler in setWindowTitel"
        End Function
    End Class

    Public Class istAzNachAltemAktenplan
        Public Shared Function exe(ByVal cand as string) as  Boolean 'myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl
            If String.IsNullOrEmpty(cand) Then Return False
            Return cand$.ToString.Length <> 4
        End Function
    End Class

    Public Class stammObjektKopieren
        Public Shared Sub exe(ByVal quelle As Stamm, ByVal ziel As Stamm) 'lokalestamm, myGlobalz.sitzung.Vorgang.Stammdaten
            'ziel = CType(quelle.Clone, Stamm)'zu flach
            ziel.AltAz = quelle.AltAz
            ziel.ArchivSubdir = quelle.ArchivSubdir
            ziel.Aufnahmedatum = quelle.Aufnahmedatum
            ziel.hatraumbezug = quelle.hatraumbezug
            ziel.az.gesamt = quelle.az.gesamt
            ziel.az.Prosa = quelle.az.Prosa
            ziel.az.sachgebiet = quelle.az.sachgebiet
            ziel.az.schreiber = quelle.az.schreiber
            ziel.az.stamm = quelle.az.stamm
            ziel.az.verfasser = quelle.az.verfasser
            ziel.az.Vorgangsnummer = quelle.az.Vorgangsnummer
            ziel.az.anychange = quelle.az.anychange
            ziel.hauptBearbeiter = quelle.hauptBearbeiter
            ziel.Bemerkung = quelle.Bemerkung
            ziel.Beschreibung = quelle.Beschreibung
            ziel.darfNichtVernichtetWerden = quelle.darfNichtVernichtetWerden
            ziel.AbgabeBA = quelle.AbgabeBA
            ziel.Eingangsdatum = quelle.Eingangsdatum
            ziel.GemKRZ = quelle.GemKRZ
            ziel.ID = quelle.ID
            ziel.LastActionHeroe = quelle.LastActionHeroe
            ziel.LetzteBearbeitung = quelle.LetzteBearbeitung
            ziel.Ortstermin = quelle.Ortstermin
            ziel.Probaugaz = quelle.Probaugaz
            ' ziel.Standort = quelle.Standort
            ziel.erledigt = quelle.erledigt
            ziel.Stellungnahme = quelle.Stellungnahme
            ziel.WeitereBearbeiter = quelle.WeitereBearbeiter
            ziel.Standort.RaumNr = quelle.Standort.RaumNr
            ziel.Standort.Titel = quelle.Standort.Titel
            ziel.meinGutachten.existiert = quelle.meinGutachten.existiert
            ziel.meinGutachten.UnterDokumente = quelle.meinGutachten.UnterDokumente
            ziel.AbgabeBA = quelle.AbgabeBA
            ziel.Paragraf = quelle.Paragraf
            ziel.InterneNr = quelle.InterneNr
        End Sub
    End Class

End Namespace

