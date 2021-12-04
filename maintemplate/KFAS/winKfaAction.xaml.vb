Public Class winKfaAction
    Public Property kfasobj As New clsKfas

    Sub New(_kfasobj As clsKfas)
        InitializeComponent()
        kfasobj = _kfasobj
    End Sub

    Private Sub Cancel_Click_1(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Close()
    End Sub
    Private Sub btnMakeParadigmaVG_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        kfasobj.AS_person = kfatools.getPerson(kfasobj.dict, "as_")
        makeStammdatenObjekt() 'myGlobalz.sitzung.aktVorgang.Stammdaten
        Dim dummy As Boolean
        Dim st As New clsStammTools
        st.speichernAllgemein(dummy, "neu")
        'stammdaten_pruefen
        'stamm_objekt anlegen
        'fall anlegen
        'personen anlegen
        'rb anlegen
        'dokumente anlegen
    End Sub

    Private Sub makeStammdatenObjekt()
        If kfasobj.typ = "KFAS_007" Then
            myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl = "3306"
            myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header = "Baumfällungen" 'kfasobj.kfa_typ_klartext        '
            myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.isUNB = True
        End If

        myGlobalz.sitzung.aktVorgang.Stammdaten.Aufnahmedatum = Now
        myGlobalz.sitzung.aktVorgang.Stammdaten.Eingangsdatum = Now 'ÄÄÄÄ
        myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt = "huraxdaxpaxbeiderhax" 'ÄÄÄÄ

        myGlobalz.sitzung.aktVorgang.Stammdaten.az.Vorgangsnummer = 0
        myGlobalz.sitzung.aktVorgang.Stammdaten.az.Prosa = "keine Prosa"


        Debug.Print(myGlobalz.sitzung.aktBearbeiter.ID.ToString) ' kopiere hauptbearbeiter von aktbearbeiter
        myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.ID = myGlobalz.sitzung.aktBearbeiter.ID 'ÄÄÄÄ
        myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Vorname = myGlobalz.sitzung.aktBearbeiter.Vorname
        myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.username = myGlobalz.sitzung.aktBearbeiter.username
        myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Name = myGlobalz.sitzung.aktBearbeiter.Name
        myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Anrede = myGlobalz.sitzung.aktBearbeiter.Anrede
        myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale = myGlobalz.sitzung.aktBearbeiter.Initiale
        myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter = "testw" 'ÄÄÄÄ

        myGlobalz.sitzung.aktVorgang.Stammdaten.Bemerkung = "Bemerkung testfall" 'ÄÄÄÄ
        myGlobalz.sitzung.aktVorgang.Stammdaten.Beschreibung = "Fällung " 'ÄÄÄÄ

        myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung = Now
        myGlobalz.sitzung.aktVorgang.Stammdaten.erledigt = False
        myGlobalz.sitzung.aktVorgang.Stammdaten.Ortstermin = False
        myGlobalz.sitzung.aktVorgang.Stammdaten.Stellungnahme = False
        myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz = ""
        myGlobalz.sitzung.aktVorgang.Stammdaten.AltAz = ""
        myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ = "" 'ÄÄÄÄ
        myGlobalz.sitzung.aktVorgang.Stammdaten.LastActionHeroe = ""


        '"VORGANGSID", vid%)) 'MYGLObalz.sitzung.VorgangsID)
        '"BEARBEITER", stamm.hauptBearbeiter.Initiale.Trim))
        '"BEARBEITERID", stamm.hauptBearbeiter.ID))
        '"BEMERKUNG", stamm.Bemerkung))
        '"BESCHREIBUNG", stamm.Beschreibung))
        '"EINGANG",
        'kedateMssqlConform(Stamm.Eingangsdatum, myGlobalz.sitzung.VorgangREC.mydb.dbtyp)))
        '"LETZTEBEARBEITUNG", stamm.LetzteBearbeitung))
        '"ERLEDIGT", Convert.ToInt16(stamm.erledigt)))
        '"ORTSTERMIN", Convert.ToInt16(stamm.Ortstermin)))
        '"STELLUNGNAHME", Convert.ToInt16(stamm.Stellungnahme)))
        '"PROBAUGAZ", (stamm.Probaugaz)))
        '"ALTAZ", (stamm.AltAz)))
        '"GEMKRZ", stamm.GemKRZ))
        '"LASTACTIONHEROE", stamm.LastActionHeroe))
        '"AZ2", stamm.az.gesamt))
        '"WEITEREBEARB", stamm.WeitereBearbeiter))

        myGlobalz.sitzung.aktVorgang.Stammdaten.darfNichtVernichtetWerden = True 'ÄÄÄÄ
        myGlobalz.sitzung.aktVorgang.Stammdaten.AbgabeBA = False
        myGlobalz.sitzung.aktVorgang.Stammdaten.hatraumbezug = False 'ÄÄÄÄ
        myGlobalz.sitzung.aktVorgang.Stammdaten.darfNichtVernichtetWerden = True
        myGlobalz.sitzung.aktVorgang.Stammdaten.meinGutachten.existiert = False
        myGlobalz.sitzung.aktVorgang.Stammdaten.meinGutachten.UnterDokumente = False
        myGlobalz.sitzung.aktVorgang.Stammdaten.Standort.RaumNr = "" 'ÄÄÄÄ
        myGlobalz.sitzung.aktVorgang.Stammdaten.Standort.Titel = "" 'ÄÄÄÄ
        myGlobalz.sitzung.aktVorgang.Stammdaten.Paragraf = "" 'ÄÄÄÄ
        myGlobalz.sitzung.aktVorgang.Stammdaten.InterneNr = ""
        myGlobalz.sitzung.aktVorgang.Stammdaten.Paragraf = ""

        '"DARFNICHTVERNICHTETWERDEN", Convert.ToInt16(stamm.darfNichtVernichtetWerden)))
        '"ABGABEBA", Convert.ToInt16(stamm.AbgabeBA)))
        '"HATRAUMBEZUG", Convert.ToInt16(stamm.hatraumbezug)))
        '"GUTACHTENMIT", Convert.ToInt16(stamm.meinGutachten.existiert)))
        '"GUTACHTENDRIN", Convert.ToInt16(stamm.meinGutachten.UnterDokumente)))
        '"STORAUMNR", stamm.Standort.RaumNr))
        '"STOTITEL", stamm.Standort.Titel))
        '"PARAGRAF", stamm.Paragraf))
        '"INTERNENR", stamm.InterneNr)) 
    End Sub

    Private Sub btndeaktiv_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        MsgBox("Baustelle")
    End Sub
End Class
