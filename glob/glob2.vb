Imports System.Data
Public Class glob2
    Public Shared Function setzeHintergrundTextInMiniMapExtracted(ByVal instring As String) As String
        instring = instring.Replace(";", "")
        instring = instring.Replace("lubi", "Luftbild ")
        instring = instring.Replace("realshapeopak", "Stadtplan")
        instring = instring.Replace("tk5", "Flurkarte ")
        instring = instring.Replace("tk", "Flurkarte ")
        instring = instring.Replace("osm", "OpenstreetMap")
        instring = instring.Replace("flurscan", "TK5fd 1994")
        instring = instring.Replace("nullover", "Kein Hintergrund")
        Return instring
    End Function

    Shared Sub editVorgang(ByVal vorgangsnr As Integer)

        glob2.holeDetailform(vorgangsnr)
        My.Log.WriteEntry("editVorgang")
    End Sub

    Public Shared Function EDIT_VorgangStamm_2DBOk() As Boolean
        nachricht("in EDIT_VorgangStamm_2DBOk--------------------------------------")
        If Not STammdatenInordnung() Then
            MsgBox("Die Stammdaten sind nicht in Ordnung. Bitte starten Sie Paradigma neu und laden Sie den Vorgang erneut! (Das Problem wird in Kürze beseitigt.)")
            Exit Function
        End If
        Dim erfolg As Boolean = VSTTools.SpeichernVorgang_alleDB.exe(myGlobalz.sitzung.aktVorgangsID)
        nachricht("in EDIT_VorgangStamm_2DBOk 1 ")
        If erfolg Then
            nachricht("in EDIT_VorgangStamm_2DBOk 2 ")
            erfolg = VSTTools.editStammdaten_alleDB.exe(myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktVorgang.Stammdaten)
            nachricht("in EDIT_VorgangStamm_2DBOk 3 ")
            If erfolg Then ' LIBstammdatenCRUD.clsStammCRUD_Mysql.EDIT_speichern_stammdaten(myGlobalz.sitzung.VorgangsID, myGlobalz.sitzung.VorgangREC, myGlobalz.sitzung.Vorgang.Stammdaten) Then
                nachricht("Daten wurden vollständig gespeichert")       ', "Speichern", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                Return True
            Else
                nachricht("Problem: EDIT_speichern_stammdaten")
            End If
        End If
        nachricht("in EDIT_VorgangStamm_2DBOk 4 ")
        Return False
    End Function


    Public Shared Function NEU_VorgangStamm_2DBOk() As Boolean
        If Not STammdatenInordnung() Then
            MsgBox("Die Stammdaten sind nicht in Ordnung. Bitte starten Sie Paradigma neu und laden Sie den Vorgang erneut! (Bitte melden Sie das Problem dem Admin.)")
            Exit Function
        End If
        Dim erfolg As Boolean = VSTTools.EinfuegeVorgang_AlleDB.exe()
        If erfolg Then
            '-----------------------------
            myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir = myGlobalz.sitzung.defineArchivVorgangsDir(myGlobalz.sitzung.aktVorgangsID) 'archicsubdirfeststellen()
            erfolg = myGlobalz.sitzung.aktVorgang.Stammdaten.createArchivsubdir(myGlobalz.Arc.rootDir.ToString, myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir)
            nachricht(If(Not erfolg, "Fehler beim erzeugen des createArchivsubdir", "createArchivsubdir erfolgreich"))
            VorgangsIdInsAzEinfuegen()
            '-----------------------------
            erfolg = VSTTools.NEU_StammSpeichern_alleDB.exe(Now)
            If erfolg Then
                nachricht("Die Daten wurden vollständig gespeichert")
                'in der neuen version muss hier nochmal das az mit der Partadigmanr gespecihert werden
                Return True
            End If
        End If
        nachricht("Die Daten wurden NICHT vollständig gespeichert!!!")
        Return False
    End Function


    Shared Function DokuEincheckFormAufrufen(ByRef filenames As String(), ByVal ereignisID As Integer, ByVal initalDir As String) As Boolean
        Dim dcc As New winDokumenteEinchecken(filenames, ereignisID, initalDir, "")
        dcc.ShowDialog()
        Dim result As Boolean = CBool(dcc.DialogResult)
        Return result
    End Function

    Private Shared Function bestimmeInitialdir() As String
        Return initP.getValue("Haupt.gesamterSchriftverkehr")
    End Function

    Public Shared Function FktDokumentehinzu(ByVal ereignisID As Integer) As Boolean
        Dim filenames As String() = Nothing
        Dim initalDir As String = bestimmeInitialdir()
        nachricht("USERAKTION:  dokumente hinzufügen")
        Return DokuEincheckFormAufrufen(filenames, ereignisID, initalDir)
    End Function

    Public Shared Function DokumentehinzuDragDrop(ByVal ereignisID As Integer, ByVal filenames() As String) As Boolean
        If filenames Is Nothing Then Return False
        Dim dasaktdir = ""
        If filenames.Count > 0 Then
            DokuEincheckFormAufrufen(filenames, ereignisID, dasaktdir)
            Return True
        Else
            nachricht_und_Mbox("Die Auswahl ist leer")
            Return False
        End If
    End Function

    Public Shared Function DokumenteAuswaehlen(ByRef filenames() As String, ByRef initalDir As String) As Boolean
        nachricht("DokumenteAuswaehlen -----------------------------------------")
        Dim ofd As New Microsoft.Win32.OpenFileDialog() With {.Title = "Bitte wählen Sie die Datei(en) aus!", _
         .InitialDirectory = initalDir, _
         .Multiselect = True}
        If Not ofd.ShowDialog Then
            nachricht_und_Mbox("Es wurde keine Auswahl getroffen!")
            Return False
        End If
        filenames = ofd.FileNames
        Return True
    End Function

    Public Shared Function Archiv_eingang(ByVal fileliste As String(),
                                            ByVal beschreibung As String,
                                            ByVal ereignisID As Integer,
                                            ByVal Dokumente_moven As Boolean,
                                            ByVal fotoZuRaumbezug As Boolean,
                                            dateidatum As Date) As Boolean
        Dim NumDir As String
        Try
            NumDir = myGlobalz.Arc.getFreshNumDir(myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir)
            For Each datei As String In fileliste
                Dim schlagworte As String = beschreibung '& " " & FileArchivTools.buildSchlagworteAusDateiname(datei)
                myGlobalz.Arc.checkINDoku(datei,
                                            schlagworte,
                                            ereignisID,
                                            Dokumente_moven,
                                            "",
                                            NumDir,
                                            myGlobalz.sitzung.aktVorgangsID,
                                            fotoZuRaumbezug, 
                                            dateidatum, 
                                            myGlobalz.sitzung.aktDokument.DocID,
                                            myGlobalz.sitzung.aktVorgang.Stammdaten.ArchivSubdir)
                clstart.myc.aLog.wer = myGlobalz.sitzung.aktBearbeiter.Initiale
                clstart.myc.aLog.vorgang = myGlobalz.sitzung.aktVorgangsID.ToString
                clstart.myc.aLog.komponente = "detail"
                clstart.myc.aLog.aktion = "dokument eingefügt: " & datei
                clstart.myc.aLog.log()
            Next
            Return True
        Catch ex As Exception
            nachricht_und_Mbox("Archiv_eingang schwerer Fehler: " & ex.ToString)
            Return False
        End Try
    End Function

    Public Shared Sub zeigeWiedervorlageAdminTabelle()
        Dim wvtool As New clsWVTOOLS
        wvtool.getWiedervorlageDT("alle")
        Dim wiedervorlagewin As New Window_Wiedervorlage
        wiedervorlagewin.ShowDialog()
    End Sub

    Public Shared Sub zeigeWiedervorlageakutAdminTabelle()
        clsWVTOOLS.getWiedervorlageAkut("alle")
        Dim wiedervorlagewin As New Window_Wiedervorlage
        wiedervorlagewin.ShowDialog()
    End Sub

    Public Shared Sub zeigeWiedervorlageTabelle()
        Dim wvtool As New clsWVTOOLS
        wvtool.getWiedervorlageDT(myGlobalz.sitzung.aktBearbeiter.Initiale)
        Dim wiedervorlagewin As New Window_Wiedervorlage
        wiedervorlagewin.ShowDialog()
    End Sub

    Public Shared Sub holeDetailform(ByVal vorgangsnr As Integer)
        GC.Collect()

        If Not istSchonEinVorgangGeoeffnet() Then
            myGlobalz.sitzung.aktVorgangsID = vorgangsnr
            myGlobalz.sitzung.modus = "edit"
            myGlobalz.sitzung.w_detail = New Window_Detail
            myGlobalz.sitzung.w_detail.Show()

            'folgendes entfällt wg. der umstellung auf .show
            'If myGlobalz.w_detail.retcode = "zurBestandsUebersicht" Then
            '    'bestand aufrufen  
            '    clsStartup.FormularBestandStammdaten(False)
            'End If
            'If myGlobalz.w_detail.retcode = "zuNeuerVorganganlegen" Then
            '    clsStartup.NeuerVorgang2()
            'End If
            'If myGlobalz.w_detail.retcode = "zugotoVorgangsnummer" Then
            '    Dim az As String = ""
            '    Dim header As String = ""
            '    clsStartup.suchenNachVorgaengen(az, header)
            'End If
        Else
            MessageBox.Show("Es ist bereits ein Vorgang geöffnet! Es kann immer nur EIN Vorgang geöffnet werden. Abbruch!", "Vorgang öffnen", MessageBoxButton.OK, MessageBoxImage.Exclamation)
        End If
    End Sub

    Private Shared Function istSchonEinVorgangGeoeffnet() As Boolean
        'If myGlobalz.w_detail Is Nothing Then
        '    ' noch kein vorgang geöffnet
        '    Return False
        'Else
        '    'es IST ein vorgang geöffnet
        '    Return True
        'End If
        If myGlobalz.einVorgangistgeoeffnet Then
            ' noch kein vorgang geöffnet
            Return True
        Else
            'es IST ein vorgang geöffnet
            Return False
        End If
    End Function
    Shared Sub schliessenButton_einschalten(ByVal btn As Button)
        If Not btn Is Nothing Then
            btn.IsEnabled = True
            btn.Visibility = Visibility.Visible
        End If
    End Sub
    ''' <summary>
    ''' bezieht sich auf datarowview also auf das angeklickte item im datagrid
    ''' </summary>
    ''' <param name="item"></param>
    ''' <param name="rb"></param>
    ''' <remarks></remarks>
    Public Shared Sub raumbezugsDataRowView2OBJ(ByVal item As DataRowView, ByVal rb As iRaumbezug)
        With rb
            .id = CInt(item("RAUMBEZUGSID"))
            .name = CStr(clsDBtools.fieldvalue(item("TITEL")))
            .SekID = CInt(item("SEKID"))
            .abstract = CStr(item("ABSTRACT"))
            .punkt.X = CDbl(item("RECHTS"))
            .punkt.Y = CDbl(item("HOCH"))
            .box.xl = CDbl(item("XMIN"))
            .box.xh = CDbl(item("XMAX"))
            .box.yl = CDbl(item("YMIN"))
            .box.yh = CDbl(item("YMAX"))
            .Status = CInt(item("STATUS"))
            .Freitext = CStr(clsDBtools.fieldvalue(item("FREITEXT")))
            .typ = CType(item("TYP"), RaumbezugsTyp)
            .isMapEnabled = CBool(clsDBtools.fieldvalue(item("ISMAPENABLED")))
            .LAENGEM=CDbl(item("LAENGEM"))
            .FLAECHEQM = CDbl(item("FLAECHEQM"))
        End With
    End Sub

    Shared Sub raumbezugsDataRow2OBJ(ByVal item As DataRow, ByVal rb As iRaumbezug)
        With rb
            .id = CInt(item("RAUMBEZUGSID"))
            .name = CStr(clsDBtools.fieldvalue(item("TITEL")))
            .SekID = CInt(item("SEKID"))
            .abstract = CStr(item("ABSTRACT"))
            .punkt.X = CDbl(item("RECHTS"))
            .punkt.Y = CDbl(item("HOCH"))
            .box.xl = CDbl(item("XMIN"))
            .box.xh = CDbl(item("XMAX"))
            .box.yl = CDbl(item("YMIN"))
            .box.yh = CDbl(item("YMAX"))
            .Status = CInt(item("STATUS"))
            ' .Freitext = CStr(item("FREITEXT"))
            .Freitext = CStr(clsDBtools.fieldvalue(item("FREITEXT")))
            .isMapEnabled = CBool(clsDBtools.fieldvalue(item("ISMAPENABLED")))
        End With
    End Sub


    Public Shared Sub zum_dgAdresse_Editmode()
        'daten aus raumbezugtabelle laden
        'daten aus adressdate satz laden
        Dim sekid$ = CStr(myGlobalz.sitzung.aktADR.SekID)
        'DBraumbezug_Mysql.RB_Adresse_holen_by_ID(sekid$)
        RBtoolsns.RB_Adresse_holen_by_ID_alleDB.exe(CInt(sekid))
        If AdressTools.DTaufAdressObjektabbilden(myGlobalz.sitzung.tempREC.dt, myGlobalz.sitzung.aktADR) Then
            AdressFormaufrufen()
        Else
            nachricht_und_Mbox("Die Maske kann nicht aufgerufen werden. Es gab einen Fehler. Bitte beim Admin melden")
        End If
    End Sub

    Public Shared Sub AdressFormaufrufen()
        Dim wadress As New Window_RB_Adresse
        wadress.ShowDialog()
    End Sub


    Public Shared Sub setzeZugriffsrechte()
        Dim winzu As New winzugriff("kollegen")
        winzu.ShowDialog()
    End Sub

    Public Shared Sub Adresse_Neu(ByVal Radius As Double)
        Dim adresseID As Integer
        adresseID = RBtoolsns.AdresseNeuSpeichern_alleDB.execute
        If adresseID > 0 Then
            myGlobalz.sitzung.aktADR.SekID = adresseID
            myGlobalz.sitzung.aktADR.defineAbstract()
            DBraumbezug_Mysql.defineBBOX(Radius, myGlobalz.sitzung.aktADR)
            myGlobalz.sitzung.aktADR.Typ = RaumbezugsTyp.Adresse
            Dim raumbezugsID% = RBtoolsns.Raumbezug_abspeichern_Neu_alleDB.execute(myGlobalz.sitzung.aktADR)

            myGlobalz.sitzung.aktADR.RaumbezugsID = raumbezugsID
            Dim koppelungsID4% = RBtoolsns.Koppelung_Raumbezug_Vorgang_alleDB.execute(CInt(myGlobalz.sitzung.aktADR.RaumbezugsID), myGlobalz.sitzung.aktVorgangsID, 0)
            My.Log.WriteEntry(" Koppelung_Raumbezug_Vorgang:" & koppelungsID4% & " ")
        Else
            nachricht("Problem beim Abspeicherne!")
        End If
    End Sub

    Public Shared Function isfreieTexteingabeOK() As Boolean
        Dim res As New MessageBoxResult
        res = MessageBox.Show("Bei freier Texteingabe können die Objekte über Google verortet werden." & vbCrLf &
                              ">>> Das ist ganz einfach !!! <<<" & vbCrLf &
                              "- Geben Sie die Adresse frei ein." & vbCrLf &
                              "- Drücken Sie dann anschließend auf den ROTEN Knopf <GoogleKoordinaten>. " & vbCrLf &
                              "- Speichern: Fertig !",
                              "Freie Texteingabe", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK)
        'If res = MessageBoxResult.No Then
        '    Return False
        'End If
        Return True
    End Function
    Public Shared Function istEntkopplungAllerDokusErnstGemeint() As Boolean
        Dim res As New MessageBoxResult
        res = MessageBox.Show("Möchten Sie wirklich alle Dokumente von diesem Ereignis entkoppeln ?" & vbCrLf & _
         " ", "Alle Dokumente von diesem Ereignis entkoppeln", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
        If res = MessageBoxResult.No Then
            Return False
        End If
        Return True
    End Function
    Public Shared Function istloeschenErnstgemeint() As Boolean
        Dim res As New MessageBoxResult
        res = MessageBox.Show("Möchten Sie das Objekt wirklich löschen ?" & vbCrLf & _
         " ", "Objekt löschen", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
        If res = MessageBoxResult.No Then
            Return False
        End If
        Return True
    End Function

    Shared Function getMailuserid() As String
        Dim mailuserid$ = Microsoft.VisualBasic.Interaction.InputBox( _
        "Bitte geben Sie hier Ihre Notes / Outlook - Userid ein:" & vbCrLf & _
        "  " & vbCrLf, "Mail Userid eingeben", "@kreis-offenbach.de")
        If String.IsNullOrEmpty(mailuserid$) Then
            MessageBox.Show("Sie haben keine Eingabe gemacht bitte wenden Sie sich an den Admin!")
        End If
        Return mailuserid$
    End Function


    Shared Function getMailpassword() As String
        Dim mailpw$ = Microsoft.VisualBasic.Interaction.InputBox( _
        "Bitte geben Sie hier Ihr Notes / Outlook - Passwort ein:" & vbCrLf & _
        "  " & vbCrLf, "Mail Passwort eingeben", "@kreis-offenbach.de")
        If String.IsNullOrEmpty(mailpw$) Then
            MessageBox.Show("Sie haben keine Eingabe gemacht bitte wenden Sie sich an den Admin!")
        End If
        Return mailpw$
    End Function

    Shared Sub genmailXML()


          
        Try
            'Dim inifile$ = System.Environment.GetEnvironmentVariable("APPDATA") & "\Paradigma"
            Dim inifile$ = myGlobalz.appdataDir
            If Not IO.Directory.Exists(inifile$) Then IO.Directory.CreateDirectory(inifile$)
            inifile$ = inifile$ & "\mail.xml"
            Dim test As New IO.FileInfo(inifile)
            If Not test.Exists Then
                Dim sr As New IO.StreamWriter(inifile)
                sr.WriteLine("<?xml version=" & Chr(34) & "1.0" & Chr(34) & " encoding=" & Chr(34) & "iso-8859-2" & Chr(34) & "?>")
                sr.WriteLine("  <ini>")
                sr.WriteLine("       <INTERNET				")
                sr.WriteLine("        mailserver=" & Chr(34) & CType(clstart.mycsimple.iniDict("Beide.Mailserverintranet_standard"), String)  & Chr(34))
                sr.WriteLine("         userid = " & Chr(34) & "@kreis-offenbach.de" & Chr(34))
                sr.WriteLine("        userpw = " & Chr(34) & Chr(34))
                sr.WriteLine(" >     <!--bla-->")
                sr.WriteLine("  </INTERNET>")
                sr.WriteLine("       <INTRANET				")
                sr.WriteLine("        mailserver=" & Chr(34) & CType(clstart.mycsimple.iniDict("Beide.Mailserverintranet_standard"), String)  & Chr(34))
                sr.WriteLine("         userid = " & Chr(34) & getMailuserid() & Chr(34))
                sr.WriteLine("        userpw = " & Chr(34) & getMailpassword() & Chr(34))
                sr.WriteLine(" >     <!--bla-->")
                sr.WriteLine("  </INTRANET>")
                sr.WriteLine(" </ini> ")
                sr.Flush()
                sr.Dispose()
            End If
        Catch ex As Exception
            nachricht("FEhler: in genmailXML: " & ex.ToString)
        End Try
    End Sub




    Private Shared Function getEmailStandardBody(ByVal mailkonto_intranet$, ByVal BodyInsert As String) As Text.StringBuilder
        Try
            Dim sb As New Text.StringBuilder
            sb.Append("Sehr geehrte Damen und Herren," & vbCrLf & vbCrLf & vbCrLf)
            sb.Append(BodyInsert & vbCrLf)
            sb.Append(vbCrLf & vbCrLf & "Mit freundlichen Grüßen" & vbCrLf & vbCrLf)
            sb.Append(myGlobalz.sitzung.aktBearbeiter.Vorname & " " & myGlobalz.sitzung.aktBearbeiter.Name & vbCrLf & vbCrLf)
            sb.Append(vbCrLf)
            sb.Append("Kreis Offenbach" & vbCrLf)
            sb.Append("Fachdienst Umwelt" & vbCrLf)
            sb.Append(myGlobalz.sitzung.aktBearbeiter.Bemerkung & vbCrLf)
            sb.Append("Werner Hilpert Str. 1" & vbCrLf)
            sb.Append("63128 Dietzenbach" & vbCrLf)
            sb.Append("Fon: 06074 8180 " & myGlobalz.sitzung.aktBearbeiter.Kontakt.elektr.Telefon1 & vbCrLf)
            sb.Append("Fax: 06074 8180 4910" & vbCrLf)
            sb.Append("http://www.kreis-offenbach.de" & vbCrLf)
            sb.Append("email: " & mailkonto_intranet$ & vbCrLf)
            Return sb
        Catch ex As Exception
            nachricht_und_Mbox("Fehler in getEmailStandardBody" & vbCrLf & ex.ToString)
            Return Nothing
        End Try
    End Function


    Private Shared Function getEmailBauaufsichtBody(ByVal mailkonto_intranet$, ByVal BodyInsert$, ByVal BAname As String) As Text.StringBuilder
        Try
            Dim sb As New Text.StringBuilder
            sb.Append("Sehr geehrte " & BAname & "," & vbCrLf & vbCrLf & vbCrLf)
            sb.Append(BodyInsert & vbCrLf)
            sb.Append(vbCrLf & vbCrLf & "Mit freundlichen Grüßen" & vbCrLf & vbCrLf)
            sb.Append(myGlobalz.sitzung.aktBearbeiter.Vorname & " " & myGlobalz.sitzung.aktBearbeiter.Name & vbCrLf & vbCrLf)
            sb.Append(vbCrLf)
            sb.Append("Kreis Offenbach" & vbCrLf)
            sb.Append("Fachdienst Umwelt" & vbCrLf)
            sb.Append(myGlobalz.sitzung.aktBearbeiter.Bemerkung & vbCrLf)
            sb.Append("Werner Hilpert Str. 1" & vbCrLf)
            sb.Append("63128 Dietzenbach" & vbCrLf)
            sb.Append("Fon: 06074 8180 " & myGlobalz.sitzung.aktBearbeiter.Kontakt.elektr.Telefon1 & vbCrLf)
            sb.Append("Fax: 06074 8180 4910" & vbCrLf)
            sb.Append("http://www.kreis-offenbach.de" & vbCrLf)
            sb.Append("email: " & mailkonto_intranet$ & vbCrLf)
            Return sb
        Catch ex As Exception
            nachricht_und_Mbox("Fehler in getEmailStandardBody" & vbCrLf & ex.ToString)
            Return Nothing
        End Try
    End Function

    'Private Shared Function GetProaz() As String
    '    Dim proaz$
    '    Try
    '        If Not String.IsNullOrEmpty(myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz.Trim) Then
    '            proaz$ = myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz.Trim
    '        Else
    '            proaz = ""
    '        End If
    '    Catch ex As Exception
    '        nachricht_und_Mbox("Fehler in getEmailStandardBody" & vbCrLf & ex.ToString)
    '        Return " Fehler in GetProaz"
    '    End Try
    '    Return proaz
    'End Function

    'Private Shared Function getEmailAZ$()
    '    Dim az$ = ": " & myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt '& " / " & _
    '    Dim proaz$ = ""
    '    az = az.Substring(0, az.Length - 5)
    '    proaz = GetProaz()
    '    az = az & " // " & proaz
    '    Return az
    'End Function

    Private Shared Sub Betreffbilden(ByVal az As String, ByVal Betreffergaenzung As String, ByVal anBauaufsicht As Boolean)
        If anBauaufsicht Then
            myGlobalz.sitzung.SendMail.Betreff = "Az. " & az & " / " & myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz 'getEmailAZ() & " " & 
        Else
            myGlobalz.sitzung.SendMail.Betreff = "Az. " & az
        End If
    End Sub


    Public Shared function EmailFormOEffnen(ByVal an As string, 
                                        ByVal Betreffergaenzung As string, 
                                        ByVal BodyInsert As string, 
                                        ByVal anhang As string, 
                                        ByVal BearbeiterEmail As String, 
                                        ByVal anBauaufsicht As Boolean) As boolean
        'email form öffen
        'text vordefinieren
        Dim dummy$ = "", hinweis$ = "", mailkonto_intranet$ = ""
        If anBauaufsicht Then
            Betreffbilden(clsVorlagedokumente.getAktenzeichenOhneProsa(), Betreffergaenzung, anBauaufsicht)
        Else
            Betreffbilden(myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt, Betreffergaenzung, anBauaufsicht)
        End If
        myGlobalz.sitzung.SendMail.An = an 'myGlobalz.sitzung.aktPerson.Kontakt.elektr.Email


        Dim sb As Text.StringBuilder
        If anBauaufsicht Then
            Dim dmm As String = ""
            Dim BAname As String = clsVorlagedokumente.SpezialPersonBilden("Bauaufsicht", dmm, dmm, dmm)
            sb = getEmailBauaufsichtBody(BearbeiterEmail, BodyInsert$, BAname)
        Else
            sb = getEmailStandardBody(BearbeiterEmail, BodyInsert$)
        End If


        myGlobalz.sitzung.SendMail.Message = sb.ToString
        anhang = clsString.removeLeadingChar(anhang, myGlobalz.anhangtrenner)
        Dim winemail As New Window_email_sofort(anhang$)
        winemail.ShowDialog()
        Return winemail.abbruch
    End function

    Shared Function istLoeschenErlaubt(ByVal loeschObjektQuelle As String) As Boolean
        Try
            If loeschObjektQuelle$ Is Nothing Then
                Return True
            End If
            ' myGlobalz.sitzung.aktPerson.Quelle
            If loeschObjektQuelle$.ToLower = myGlobalz.sitzung.aktBearbeiter.Initiale.ToLower Then
                Return True
            End If
            If myGlobalz.sitzung.aktBearbeiter.Rang = "admin" Then
                Return True
            End If
            Return False
        Catch ex As Exception
            Return False
        End Try
    End Function

    Shared Function getPLZfromGemeinde(ByVal gemeindename As String) As String 'von integer auf string geändert wg. ddr und urbanke
        Using neuadr As New clsAdress(gemeindename)
            If clsGemarkungsParams.liegtGemeindeImKreisOffenbach(gemeindename) Then
                Dim test As String = neuadr.gemparms.gemeindetext2PLZ(gemeindename)
                If test = "0" Then
                    Return "0"
                Else
                    Return test
                End If
            Else
                Return "0"
            End If
        End Using
        Return "0"
    End Function



    Shared Function Email_verschicken(ByVal anhangtrenner As String, ByVal outlookAnzeigen As Boolean) As String
        If Not myGlobalz.sitzung.SendMail.isReady = "" Then
            nachricht("sendmail ready ? " & myGlobalz.sitzung.SendMail.isReady)
            Return "Sendmail ist nicht bereit. " & myGlobalz.sitzung.SendMail.isReady
        End If
        myGlobalz.sitzung.SendMail.Message = myGlobalz.sitzung.SendMail.Message.Replace(vbCrLf, "<br>")
        nachricht("vor dem ersten mailraus")

        Dim erfolg As String = clsMailsenden.mailrausOutlook(myGlobalz.sitzung.SendMail.An,
                                                             myGlobalz.sitzung.SendMail.Betreff,
                                                             myGlobalz.sitzung.SendMail.Message,
                                                             myGlobalz.sitzung.SendMail.Anhang,
                                                             myGlobalz.sitzung.SendMail.CC,
                                                             anhangtrenner,
                                                             outlookAnzeigen)
        Return erfolg

    End Function


    'Public Shared Sub VorzimmerHinweis()
    '    If myGlobalz.sitzung.Bearbeiter.Initiale.ToUpper <> myGlobalz.weyers Then
    '        If myGlobalz.sitzung.aktPerson.Quelle = myGlobalz.weyers Then
    '            MessageBox.Show("Es handelt sich hier um Stammdaten aus dem Vorzimmer!!!" & vbCrLf & _
    '             "Sie sollten diese Daten nicht eigenständig ändern. " & vbCrLf & _
    '             "Bitte nehmen Sie Rücksprqache mit Frau Weyers.")
    '        End If
    '    End If
    'End Sub
    ''' <summary>
    ''' Startet eine bestimmte Anwendung mit dem angegebenen Dokument
    ''' </summary>
    ''' <param name="ProgramFile">Dateiname der Anwendung</param>
    ''' <param name="DocumentFile">Dokument-Dateiname</param>
    ''' <returns>True, wenn die Anwendung gestartet werden konnte, andernfalls False.</returns>
    Public Shared Function OpenDocument(ByVal ProgramFile As String, _
     ByVal DocumentFile As String) As Boolean
        Try
            Dim pInfo As New Diagnostics.ProcessStartInfo
            With pInfo
                ' Anwendung, die gestartet werden soll
                .FileName = ProgramFile
                ' Parameter (Dokument)
                .Arguments = Chr(34) & DocumentFile & Chr(34)
                ' Anwendung starten
                .Verb = "open"
            End With
            Process.Start(pInfo)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    ''' <summary>
    ''' Öffnet das Dokument mit der im System festgelegten Standard-Anwendung
    ''' </summary>
    ''' <param name="DocumentFile">Dokument-Dateiname</param>
    ''' <returns>True, wenn das Dokument geöffnet werden konnte, andernfalls False.</returns>
    Public Shared Function OpenDocument(ByVal DocumentFile As String) As Boolean
        Try
            nachricht("OpenDocument DocumentFile:" & vbCrLf & DocumentFile)
            If DocumentFile.IsNothingOrEmpty Then
                MsgBox("Der Dateiname ist leer ")
                Return False
            End If
            Dim pInfo As New Diagnostics.ProcessStartInfo
            Dim test As New IO.FileInfo(DocumentFile)
            If Not test.Exists Then
                MessageBox.Show("Die Datei existiert nicht. " & test.Name)
                nachricht("FEHLER:	 Die Datei existiert nicht: " & test.FullName)
                Return False
            End If
            With pInfo
                ' Dokument	
                .FileName = DocumentFile
                ' verknüpfte Anwendung starten
                .Verb = "open"
            End With
            Process.Start(pInfo)
            nachricht("OpenDocument erfolgreich: ")
            Return True
        Catch ex As Exception
            nachricht("OpenDocument FEHLER: " & vbCrLf & ex.ToString)
            Return False
        End Try
    End Function


    Shared Sub vorgangLoeschen()
        nachricht("vorgangLoeschen todo")
        '1. Dokus löschen
        '2. Raumbezüge
        '3. Beteiligte entkoppeln
        '4. Ereignisse entkoppeln
        '5. Stammdaten löschen
        '6. Vorgang löschen
    End Sub
    Shared Function klammerraus(ByVal candidat As String) As String
        If candidat.Contains("(") Then
            Dim pos% = candidat.IndexOf("(")
            candidat = candidat.Substring(0, pos - 1)
        End If
        Return candidat
    End Function


    Public Shared Function NeueBeteiligteAbspeichern(ByVal zielvid%, ByVal aperson As Person) As Boolean ', myGlobalz.sitzung.aktPerson
        Dim PersonenID% = clsBeteiligteBUSI.BeteiligteAbspeichernNeu_AlleDB(zielvid%, aperson)
        If PersonenID > 0 Then
            myGlobalz.sitzung.aktPerson.PersonenID = PersonenID
            myGlobalz.sitzung.aktPerson.Status = 0
            ' Koppelung_BeteiligteVorgang(zielvid)
            Return True
        Else
            nachricht("Problem beim NeueBeteiligteAbspeichern!")
            Return False
        End If
    End Function

    Public Shared Sub Koppelung_BeteiligteVorgang(ByVal zielvid%)
        myGlobalz.sitzung.aktPerson.Status = 1
        Dim koppid% = clsBeteiligteBUSI.BeteiligteKopplungVorgang_alledb(zielvid%, myGlobalz.sitzung.aktPerson.PersonenID, myGlobalz.sitzung.aktPerson.Status)

        '  Dim koppID% = clsBeteiligteBUSI.Koppelung_Beteiligte_Vorgang(myGlobalz.sitzung.aktPerson.PersonenID, myGlobalz.sitzung.VorgangsID, myGlobalz.sitzung.aktPerson.Status)
        If koppid < 1 Then
            nachricht_und_Mbox("Koppelung_Vorgang_Person Fehler beim koppeln: " & zielvid% & myGlobalz.sitzung.aktPerson.PersonenID)
        End If
    End Sub

    Public Shared Sub istTextzulang(ByVal maxlen%, ByVal tb As TextBox)
        Try
            If tb Is Nothing Then Exit Sub
            If tb.Text.Length > maxlen% Then
                MessageBox.Show("Der Text ist zu lang: " & vbCrLf & _
                 tb.Text.Length & " statt maximal " & maxlen & " Zeichen." & vbCrLf _
                 & "Der Text wird am Ende abgeschnitten!", "Eingabe zu lang", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK)
                tb.Text = tb.Text.Substring(0, maxlen - 1)
            End If
        Catch ex As Exception
            nachricht_und_Mbox(ex.ToString)
        End Try
    End Sub
    Public Shared Function pruefeObZahl(ByVal cnt As TextBox) As Boolean
        Try
            If Not IsNumeric(cnt.Text) Then
                MessageBox.Show("Es ist hier eine Zahl gefordert. Texte sind ungültig!")
                Return False
            Else
                Return True
            End If
        Catch ex As Exception
            nachricht_und_Mbox(ex.ToString)
        End Try
    End Function

    Public Shared Sub ZahlungFormoeffen()
        Dim wz As New WINzahlungdetail("neu")
        wz.ShowDialog()
    End Sub


    Public Shared Sub initTemprecAusVorgangRecMysql()
        myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, clsDBspecMYSQL)
        myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
        myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
    End Sub
    Public Shared Sub initTemprecAusVorgangRecOracle()
        myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, LIBoracle.clsDBspecOracle)
        myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
        myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
        myGlobalz.sitzung.tempREC2 = CType(myGlobalz.sitzung.VorgangREC, LIBoracle.clsDBspecOracle)
        myGlobalz.sitzung.tempREC2.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
        myGlobalz.sitzung.tempREC2.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
    End Sub

    Shared Sub holeGemeindeDT(ByVal gemeindeDict As Dictionary(Of String, String))
        Dim gemparms As New clsGemarkungsParams
        gemparms.init()
        Dim a = From item In gemparms.parms Where item.gemeindenr < 14 Select item.gemeindenr, item.gemeindetext Order By gemeindetext Distinct
        If a.ToArray.Length > 0 Then
            For Each kkk In a.ToArray
                gemeindeDict.Add(kkk.gemeindetext, kkk.gemeindenr.ToString)
            Next
        End If
    End Sub

    Public Shared Function istKontaktAdresseFuerGoogleBrauchbar() As Boolean
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktPerson.Kontakt.Anschrift.Gemeindename) Then Return False
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktPerson.Kontakt.Anschrift.Strasse) Then Return False
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktPerson.Kontakt.Anschrift.Hausnr) Then Return False
        Return True
    End Function

    Shared Sub hole_AdressKoordinaten(ByVal halo_id%)
        Try
            DB_Oracle_sharedfunctions.hole_AdressKoordinatenExtracted(halo_id%)
            If Not myGlobalz.sitzung.haloREC.dt.Rows.IsNothingOrEmpty Then
                With myGlobalz.sitzung.haloREC.dt.Rows(0)
                    myGlobalz.sitzung.aktADR.punkt.X = CDbl(.Item(0))
                    myGlobalz.sitzung.aktADR.punkt.Y = CDbl(.Item(1))
                    myGlobalz.sitzung.aktADR.FS = CStr(.Item(2))
                    myGlobalz.sitzung.aktADR.Gisadresse.strasseCode = CInt(.Item("strcode"))
                    myGlobalz.sitzung.aktADR.Gisadresse.gemeindeNr = CInt(.Item("gemeindenr"))
                    myGlobalz.sitzung.aktADR.Gisadresse.hausNr = CInt(clsDBtools.fieldvalue(.Item("hausNr")))
                    myGlobalz.sitzung.aktADR.Gisadresse.hausZusatz = CStr(clsDBtools.fieldvalue(.Item("Zusatz")))
                End With
            Else
                myGlobalz.sitzung.aktADR.punkt.X = 0
                myGlobalz.sitzung.aktADR.punkt.Y = 0
                myGlobalz.sitzung.aktADR.FS = ""
            End If
        Catch ex As Exception
            myGlobalz.sitzung.aktADR.punkt.X = 0
            myGlobalz.sitzung.aktADR.punkt.Y = 0
            myGlobalz.sitzung.aktADR.FS = ""
            nachricht_und_Mbox("Fehler bei der Ermittlung der Koordinaten! (=:0)")
        End Try
    End Sub

    Shared Sub initCMBParagraf(ByVal dumm As System.Windows.Window)
        Dim existing As XmlDataProvider = TryCast(dumm.Resources("XMLSourceComboBoxParagraf"), XmlDataProvider)
        existing.Source = New Uri(myGlobalz.appdataDir & "\config\Combos\paragraf.xml")
    End Sub

    Shared Sub initGemKRZCombo(ByVal dumm As System.Windows.Window)
        Dim existing As XmlDataProvider = TryCast(dumm.Resources("XMLSourceComboBoxGemKRZ"), XmlDataProvider)
        existing.Source = New Uri(myGlobalz.appdataDir & "\config\Combos\GemKRZn.xml")
    End Sub
    Shared Sub initRaumNrCombo(ByVal dumm As System.Windows.Window)
        Dim existing As XmlDataProvider = TryCast(dumm.Resources("XMLSourceComboBoxRaumNr"), XmlDataProvider)
        existing.Source = New Uri(myGlobalz.appdataDir & "\config\Combos\RaumNr.xml")
    End Sub



    Public Shared Sub generateLayerWithShapes()
        nachricht("generateAndSaveSerialShapeInDb---------------------------------------------")
        Try
            Dim rumpf As String
            rumpf = initP.getValue("GisServer.URLlayer2shpfile")
            rumpf &= myGlobalz.sitzung.aktBearbeiter.username
            rumpf &= "&vid=" & myGlobalz.sitzung.aktVorgangsID
            rumpf &= "&modus=einzeln"
            nachricht("url: " & rumpf)
            webmapBrows(rumpf)
        Catch ex As Exception
            nachricht("fehler in: generateAndSaveSerialShapeInDb: " & ex.ToString)
        End Try
    End Sub


    Public Shared Sub generateAndSaveSerialShapeInDb()
        nachricht("generateAndSaveSerialShapeInDb---------------------------------------------")
        Try
            Dim rumpf As String
            rumpf = initP.getValue("GisServer.URLserialserver")
            rumpf &= myGlobalz.sitzung.aktBearbeiter.Initiale
            rumpf &= "&vid=" & myGlobalz.sitzung.aktVorgangsID
            rumpf &= "&rid=" & CInt(myGlobalz.sitzung.aktFST.RaumbezugsID)
            rumpf &= "&gemcode=" & myGlobalz.sitzung.aktFST.normflst.gemcode
            rumpf &= "&FS=" & myGlobalz.sitzung.aktFST.normflst.FS
            nachricht("url: " & rumpf)
            webmapBrows(rumpf)
        Catch ex As Exception
            nachricht("fehler in: generateAndSaveSerialShapeInDb: " & ex.ToString)
        End Try
    End Sub
    Shared Sub webmapBrows(ByVal aufruf As String)
        nachricht("webmapBrows --------------------------")
        Dim ww As New WINwebrowser
        ww.Show()
        ww.wb2.Navigate(New Uri(aufruf, UriKind.RelativeOrAbsolute))
        ' Process.Start(aufruf)
        ww.Close()

        ww = Nothing
        nachricht("webmapBrows ------------ende--------------")
        'Dim dand As New LIBgoogle.clsGoogleCoordinates
        'nachricht(dand.getREsp(aufruf))
    End Sub

    Shared Function ggfTerminNachOutlookUebernehmen(ByVal zielvorgangsid As Integer, ByVal ereig As clsEreignis, ByVal nachOutlookUebernehmen As Boolean) As Boolean
        If Not nachOutlookUebernehmen Then Exit Function
        nachricht("ggfTerminNachOutlookUebernehmen -------------------------------------")
        Dim start As New Date(Year(ereig.Datum), Month(ereig.Datum), Day(ereig.Datum), 11, 0, 0, 0) '1100Uhr
        Dim neuu As New LIBOutlook2.MeinOutlook
        Dim erfolg As Boolean = neuu.OutlookTermin("AD " & myGlobalz.sitzung.aktBearbeiter.Name &
                                                    ", Vorgang:" &
                                                    zielvorgangsid & ", " &
                                                    ereig.Art & ". " & ereig.Beschreibung,
                                                    myGlobalz.sitzung.aktVorgang.Stammdaten.Beschreibung & " " &
                                                    ereig.Beschreibung,
                                                    start,
                                                    60,
                                                    True, False, False,
                                                    False)
        If erfolg Then
            MsgBox("Der Termin wurde in 'Dienstgang Umwelt' als ganztägiges Ereignis angelegt.")
        Else
            MsgBox("Der Termin konnte nicht angelegt werden.")
        End If
        neuu = Nothing
        Return erfolg
    End Function

    Public Shared Sub neueKarteerstellen()
        nachricht("Kartenmodul:" & initP.getValue("ExterneAnwendungen.APPLICATION_KARTE"))
        Dim test As New IO.FileInfo(initP.getValue("ExterneAnwendungen.APPLICATION_KARTE"))
        If test.Exists Then
            Process.Start(initP.getValue("ExterneAnwendungen.APPLICATION_KARTE"))
        Else
            nachricht_und_Mbox("Das Kartenmodul ist nicht installiert!")
        End If
    End Sub

    Private Shared Sub VorgangsIdInsAzEinfuegen()
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt) Then
            nachricht_und_Mbox("Fehler beim Einfügen der Paradigmanummer in das Aktenzeichen: " & "Aktenzeichengesamt ist leer. Bitte informieren Sie umgehend den Admin!")
            Exit Sub
        End If
        If myGlobalz.sitzung.aktVorgangsID < 1 Then
            nachricht_und_Mbox("Fehler beim Einfügen der Paradigmanummer in das Aktenzeichen: " & "Paradigmanummer ist leer. Bitte informieren Sie umgehend den Admin!")
            Exit Sub
        End If
        Try
            myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt = myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt.Replace("XXXXX", myGlobalz.sitzung.aktVorgangsID.ToString)
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Einfügen der Paradigmanummer in das Aktenzeichen:  Bitte informieren Sie umgehend den Admin! " & ex.ToString)
        End Try
    End Sub

    Shared Sub holeGemarkungsDT(ByVal gemarkungsDict As Dictionary(Of String, String))
        Dim gemparms As New clsGemarkungsParams
        gemparms.init()
        Dim a = From item In gemparms.parms
                Where item.gemcode < 761 And
                        item.gemcode > 725 And
                        item.gemeindenr < 14
                Select item.gemcode, item.gemarkungstext
                Order By gemarkungstext Distinct
        If a.ToArray.Length > 0 Then
            For Each kkk In a.ToArray
                gemarkungsDict.Add(kkk.gemarkungstext, kkk.gemcode.ToString)
            Next
        End If
    End Sub

    Private Shared Function STammdatenInordnung() As Boolean
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt) Then Return False
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl) Then Return False
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Header) Then Return False
        Return True
    End Function

    Shared Function userIstinGastModus() As Boolean
        If myGlobalz.sitzung.aktBearbeiter.username.ToLower.StartsWith("gast_") Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Shared Function ConvertRtf2Doc(ByRef filenameImLokalenCache As String) As String
        Dim lw As LIBwordvorlage.WordReplaceTextmarken
        lw = New LIBwordvorlage.WordReplaceTextmarken()
        Dim docFile As String = lw.rtf2doc(filenameImLokalenCache)
        lw = Nothing
        Return docFile
    End Function

    Public Shared Function sendjobExtracted(url As String, enc As System.Text.Encoding, zeitInMS As Integer) As String
        Try
            l("sendjobExtracted -----------------------")
            l("url-: " & url)
            Dim myWebRequest As System.Net.HttpWebRequest = DirectCast(System.Net.HttpWebRequest.Create(url), System.Net.HttpWebRequest)
            myWebRequest.Method = "GET"
            myWebRequest.Timeout = zeitInMS
            Dim myWebResponse As System.Net.HttpWebResponse = DirectCast(myWebRequest.GetResponse(), System.Net.HttpWebResponse)
            Dim myWebSource As New System.IO.StreamReader(myWebResponse.GetResponseStream(), enc)
            Dim myPageSource As String = String.Empty
            myPageSource = myWebSource.ReadToEnd()
            myWebResponse.Close()
            l("ergebnis:" & myPageSource)
            l("sendjobExtracted -----------------------Ende")
            Return myPageSource
        Catch ex As Exception
            l("Fehler in sendjobExtracted: " & ex.ToString)
            '  MsgBox("Fehler beim Abspeichern der Dokumente. Der DMS-Server (w2gis02) ist nicht aktiv. Bitte Admin benachrichtigen!") 
            Return "Fehler - server busy?"
        End Try
    End Function
        Public Shared  Function webLinkedit(modus As string) As Boolean
        Dim wvw As New winWebLink(modus)
        wvw.ShowDialog()
        Return True
    End Function
End Class

