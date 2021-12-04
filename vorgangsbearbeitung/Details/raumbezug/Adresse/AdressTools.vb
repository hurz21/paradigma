Imports System.Data

Public Class AdressTools
    Shared Function setzeeNeuesGemKRZ(ByVal sitzung As paradigma.Psession) As Boolean
        If String.IsNullOrEmpty(sitzung.aktVorgang.Stammdaten.GemKRZ.Trim) Then
            If Not String.IsNullOrEmpty(sitzung.aktFST.normflst.gemarkungstext) Then
                sitzung.aktVorgang.Stammdaten.GemKRZ = (String.Format("{0}-{1}", sitzung.aktFST.normflst.gemeindename.Substring(0, 2),
                                                                   sitzung.aktFST.normflst.gemarkungstext.Substring(0, 2))).ToUpper
            End If
            If Not String.IsNullOrEmpty(sitzung.aktADR.Gisadresse.gemeindeName) Then
                sitzung.aktVorgang.Stammdaten.GemKRZ = sitzung.aktADR.Gisadresse.gemeindeName.Substring(0, 2).ToUpper
            End If

                detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "GEMKRZ")
         '   glob2.GEMKRZ_speichern(sitzung.VorgangsID,sitzung.aktVorgang.Stammdaten.GemKRZ)
            Return True
        Else
            nachricht("Gemkrz war schon gesetzt worden!")
            Return False
        End If
    End Function

    Shared Sub loeschenRBAdresse()
        If Not glob2.istloeschenErnstgemeint() Then Exit Sub
        loeschenRBAdresseOhneNachfrage()
        ' todo: shapeFile: shape loeschen
    End Sub

    Public Shared Sub loeschenRBAdresseOhneNachfrage()
        Dim erfolg%
        If CInt(myGlobalz.sitzung.aktADR.Status) = 0 Then
            erfolg = RBtoolsns.AdresseLoeschen_alleDB.execute(CInt(myGlobalz.sitzung.aktADR.SekID))
            '   erfolg% = DBraumbezug_Mysql.RB_Adresse_loeschen(CInt(myGlobalz.sitzung.aktADR.SekID))
            If erfolg > 0 Then
                erfolg = RBtoolsns.Raumbezug_loeschen_byid_alleDB.execute(CInt(myGlobalz.sitzung.aktADR.RaumbezugsID))
                'erfolg = DBraumbezug_Mysql.RB_loeschenByID(CInt(myGlobalz.sitzung.aktADR.RaumbezugsID))
                If erfolg > 0 Then
                    ADR_undVorgang_Entkoppeln(erfolg)
                Else
                    My.Log.WriteEntry("Problem beim Löschen des Raumbezugs. Abbruch.")
                    nachricht_und_Mbox("Problem beim Löschen des Raumbezugs. Abbruch.")
                End If
            Else
                My.Log.WriteEntry("Problem beim Löschen der Raumbezugs-Adresse. Abbruch.")
                nachricht_und_Mbox("Problem beim Löschen der Raumbezugs-Adresse. Abbruch.")
            End If
        Else
            ADR_undVorgang_Entkoppeln(erfolg)
        End If
    End Sub
    Public Shared Sub ADR_undVorgang_Entkoppeln(ByVal erfolg%)
        '   erfolg = DBactionParadigma.Entkoppelung_Raumbezug_Vorgang(CInt(myGlobalz.sitzung.aktADR.RaumbezugsID), myGlobalz.sitzung.VorgangsID)
        'erfolg = RBtoolsns.Entkoppelung_Raumbezug_Vorgang_alleDB.exe(CInt(myGlobalz.sitzung.aktADR.RaumbezugsID), myGlobalz.sitzung.aktVorgangsID)
        'If erfolg > 0 Then
        '    My.Log.WriteEntry("Adresse wurde erfolgreich gelöscht")
        'Else
        '    My.Log.WriteEntry("Adresse wurde erfolgreich gelöscht")
        '    nachricht_und_Mbox("Problem beim Löschen des Raumbezugs aus dem Vorgang. Abbruch.")
        'End If
    End Sub

    Public Shared Function eingabeOK() As Boolean
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName) Then
            MsgBox("Bitte einen Gemeindenamen angeben!")
            Return False
        End If
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktADR.Gisadresse.strasseName) Then
            MsgBox("Bitte einen Straßennamen angeben!")
            Return False
        End If
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktADR.Gisadresse.HausKombi) Then
            MsgBox("Bitte eine Hausnummer angeben!" & vbCrLf &
                   " Falls die Hausnummer in der Liste fehlt:" & vbCrLf &
                   "1 - Freie Texteingabe einschalten" & vbCrLf &
                   "2 - Hausnummer freihändig eingeben (notfalls '1')" & vbCrLf &
                   "3 - Google-Koordinaten nochmal aufrufen")
            Return False
        End If
        Return True
    End Function

    'Public Shared Sub eigentuemer_fuer_adresse_holen(ByVal adr As ParaAdresse, insarchiv As Boolean, mitereignis As Boolean) 'myGlobalz.sitzung.aktADR
    '    Dim weistauf$ = DBraumbezug_Mysql.hole_Weistauf_GMLid(adr)
    '    Dim EreignisLocationAbstract As String=adr.abstract
    '    Dim aktFlurstueck As New ParaFlurstueck()
    '    Dim erfolg As Boolean = DBraumbezug_Mysql.FlurstuecksDatenAusBasisHolen(weistauf, aktFlurstueck)
    '    If erfolg Then
    '        'FST_tools.eigentuemerALKIS(aktFlurstueck, insarchiv, mitereignis,EreignisLocationAbstract)
    '    Else
    '        MessageBox.Show("Der Eigentümer kann  für diese Adresse nicht ermittelt werden: " & vbCrLf & adr.ToString)
    '    End If
    'End Sub

    Shared Function DTaufAdressObjektabbilden(ByVal meineDT As DataTable, ByVal padr As ParaAdresse) As Boolean 'myGlobalz.sitzung.tempREC.dt, myGlobalz.sitzung.aktADR
        Try
            Dim kurzgemeindenr As integer
            With padr.Gisadresse
                .strasseName = clsDBtools.fieldvalue(meineDT.Rows(0).Item("strassenname"))
                .strasseCode = CInt(clsDBtools.fieldvalue(meineDT.Rows(0).Item("strcode")))
                .HausKombi = clsDBtools.fieldvalue(meineDT.Rows(0).Item("HausnrKombi"))
                .gemeindeNr = CInt(clsDBtools.fieldvalue(meineDT.Rows(0).Item("gemeindeNr")))
                .gemeindeName = (clsDBtools.fieldvalue(meineDT.Rows(0).Item("gemeindetext")))
                kurzgemeindenr = CInt(padr.Gisadresse.gemeindeNr)
                If kurzgemeindenr > 1000 Then
                    kurzgemeindenr = kurzgemeindenr - 438000
                End If
            End With
            padr.PLZ = ((clsDBtools.fieldvalue(meineDT.Rows(0).Item("plz")))) 'ddr
            padr.Postfach = (clsDBtools.fieldvalue(meineDT.Rows(0).Item("postfach")))
            padr.Adresstyp = CType(clsDBtools.fieldvalue(meineDT.Rows(0).Item("adresstyp")), adressTyp)
            padr.FS = (clsDBtools.fieldvalue(meineDT.Rows(0).Item("FS")))
            Return True
        Catch ex As Exception
            'major
            My.Log.WriteEntry("	DTaufAdressObjektabbilden schwerer fehler es fehlt die paraadressDT")
            'todo nachricht an admin
            Return False
        End Try
    End Function

    Public Shared Sub schreibeVorgangslisteInDatei(ByVal datei As String)
        Dim summe As String = Environment.NewLine
        Using sw As New IO.StreamWriter(datei)
            For i = 0 To myGlobalz.sitzung.VorgangREC.dt.Rows.Count - 1
                summe = _
                myGlobalz.sitzung.VorgangREC.dt.Rows(i).Item("vorgangsid").ToString & ", " &
                myGlobalz.sitzung.VorgangREC.dt.Rows(i).Item("Beschreibung").ToString & ", " &
                myGlobalz.sitzung.VorgangREC.dt.Rows(i).Item("eingang").ToString & ", " &
                myGlobalz.sitzung.VorgangREC.dt.Rows(i).Item("az2").ToString & ", " &
                myGlobalz.sitzung.VorgangREC.dt.Rows(i).Item("sachgebietstext").ToString &
                "---------------------------------" &
                Environment.NewLine
                sw.WriteLine(summe)
            Next
        End Using
    End Sub

    Shared Function adresseSchonInVorgangVorhanden(ByVal paraAdresse As ParaAdresse, ByVal vorgangsid As Integer) As Boolean
        nachricht("adresseSchonInVorgangVorhanden: ")
        Try
            myGlobalz.sitzung.tempREC.mydb.SQL =
                  "select * from " & CLstart.myViewsNTabs.tabPARAADRESSE & "  p , pa_sekid2vid s where gemeindenr=" & paraAdresse.Gisadresse.gemeindeNrBig &
                  " and strassenname='" & paraAdresse.Gisadresse.strasseName & "'" &
                  " and strcode=" & paraAdresse.Gisadresse.strasseCode &
                  " and hausnrkombi='" & paraAdresse.Gisadresse.HausKombi & "'" &
                  " and s.vorgangsid= " & vorgangsid &
                  " and s.sekid=p.id"
            nachricht("adresseSchonVorhanden sql: " & myGlobalz.sitzung.tempREC.mydb.SQL)
           myGlobalz.sitzung.tempREC.getDataDT
            If Not myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
                nachricht("adresse ist schon vorhanden")
                Return True
            Else
                nachricht("adresse ist noch nicht vorhanden")
                Return False
            End If


        Catch ex As Exception
            nachricht("fehler in adresseSchonInVorgangVorhanden: " & ex.ToString)
        End Try
    End Function

    Shared Sub koordinatenErgaenzen(padr As ParaAdresse, ByRef ismapenabled As Boolean)
        nachricht("koordinatenErgaenzen------------------------")
        ' im falle von >lage und >fehlt sind keine koordinaten vorhanden!!!  myGlobalz.sitzung.aktADR.Gisadresse
        Try

            If padr.Gisadresse.Quelle.IsNothingOrEmpty Then
                padr.Gisadresse.Quelle = "lage"
            End If
            If padr.Gisadresse.Quelle = "lage" Or
                padr.Gisadresse.Quelle = "fehlt" Then
                'über google ermitteln
                If padr.punkt.X < 30000 Then
                    Dim erfolg As Boolean
                    Dim googlepunkt As clsGEOPoint
                    'plz holen weil plz macht die googleabfrage eindeutiger
                    Dim gemparms As New clsGemarkungsParams
                    gemparms.init() ': Dim result$ = "ERROR"
                    Dim a = From item In gemparms.parms Where item.gemeindetext.ToLower = padr.Gisadresse.gemeindeName Select item.gemarkungsPLZ
                    If a.ToArray.Length > 0 Then padr.PLZ = a.ToList(0).ToString

                    erfolg = AdressTools.AdresseInGoogleKoordinatenUmrechnen(googlepunkt, padr.Gisadresse, padr.PLZ)
                    If erfolg Then
                        If Not koordinateLiegtImKreisOffenbach(googlepunkt) Then
                            ismapenabled = False
                            'ismapenabled = False
                            MsgBox("Die von Google gelieferte Koordinate (" & googlepunkt.X & " " & googlepunkt.Y & ")" & Environment.NewLine &
                                   "liegt nicht im Kreis Offenbach." & Environment.NewLine &
                                   "Ggf. hat sich Google vertan. " & Environment.NewLine &
                                   "Die Koordinate wird in der Kartendarstellung NICHT berücksichtigt.")
                            If padr.Freitext.IsNothingOrEmpty Then
                                padr.Freitext = "Koodinate liegt ausserhalb Kreis Offenbach (Qu: Google)"
                            End If
                        End If
                        AdressTools.GoogleKoordinaten_uebernehmenNachAdresse(googlepunkt, padr)
                    End If
                End If


            End If
        Catch ex As Exception
            nachricht("fehler in koordinatenErgaenzen: " & ex.ToString)
        End Try
    End Sub

    Public Shared Sub GoogleKoordinaten_uebernehmenNachAdresse(ByVal googlepunkt As clsGEOPoint, adress As ParaAdresse)
        nachricht("GoogleKoordinaten_uebernehmenNachAdresse:----------------------------------- ")
        Try
            adress.coordsAbstract = "Adresse: Freitext " & googlepunkt.GKrechts & ", " & googlepunkt.GKhoch
            adress.punkt.X = googlepunkt.GKrechts
            adress.punkt.Y = googlepunkt.GKhoch
        Catch ex As Exception
            nachricht("fehler in GoogleKoordinaten_uebernehmenNachAdresse: " & ex.ToString)
        End Try
    End Sub

    Public Shared Function AdresseInGoogleKoordinatenUmrechnen(ByRef googlepunkt As clsGEOPoint, adress As clsAdress, plz As String) As Boolean
        Dim erfolg As Boolean
        nachricht("AdresseInGoogleKoordinatenUmrechnen:----------------------------------- ")
        Try
            Dim adresse As String = adress.clean(plz)
            googlepunkt = New clsGEOPoint
            nachricht("vor getgooglecoordinatenMitUmrechnung: " & adresse)
            Dim hinweis As String = ""
            erfolg = clsGooglecoordPrep.getgooglecoordinatenMitUmrechnung(adresse, googlepunkt, myGlobalz.ProxyString, hinweis,
                                                                          CType(clstart.mycsimple.iniDict("Beide.coordZiel"), String) )
            Return erfolg
        Catch ex As Exception
            nachricht("fehler in AdresseInGoogleKoordinatenUmrechnen: " & ex.ToString)
            Return False
        End Try
    End Function
    Public Sub buildStrassenListe(ByVal buchstabe As String, ByVal StrassenListe As List(Of strassenUiObj))
        DB_Oracle_sharedfunctions.holeStrasseDTausHalo(buchstabe)
        Dim strassseninstanz As New strassenUiObj
        For i = 0 To myGlobalz.sitzung.postgresREC.dt.Rows.Count - 1
            strassseninstanz = New strassenUiObj
            strassseninstanz.sname = CStr(myGlobalz.sitzung.postgresREC.dt.Rows(i).Item(1))
            strassseninstanz.strcode = CStr(myGlobalz.sitzung.postgresREC.dt.Rows(i).Item(0))
            strassseninstanz.quelle = CStr(myGlobalz.sitzung.postgresREC.dt.Rows(i).Item(2))
            StrassenListe.Add(strassseninstanz)
        Next

        DB_Oracle_sharedfunctions.holeStrasseDTausLageschluessel(buchstabe)

        strassseninstanz = New strassenUiObj
        strassseninstanz.sname = "___ mehr (Straßen OHNE Hausnummern): ________________"
        strassseninstanz.strcode = "mehr"
        strassseninstanz.quelle = "mehr"
        StrassenListe.Add(strassseninstanz)

        For i = 0 To myGlobalz.sitzung.postgresREC.dt.Rows.Count - 1
            strassseninstanz = New strassenUiObj
            strassseninstanz.sname = CStr(myGlobalz.sitzung.postgresREC.dt.Rows(i).Item(1))
            strassseninstanz.strcode = CStr(myGlobalz.sitzung.postgresREC.dt.Rows(i).Item(0))
            strassseninstanz.quelle = CStr(myGlobalz.sitzung.postgresREC.dt.Rows(i).Item(2))
            If isNotinList(StrassenListe, strassseninstanz.sname) Then
                StrassenListe.Add(strassseninstanz)
            End If
        Next

        strassseninstanz = New strassenUiObj
        strassseninstanz.sname = "___ Strasse fehlt, WEITER ________________"
        strassseninstanz.strcode = "fehlt"
        strassseninstanz.quelle = "fehlt"
        StrassenListe.Add(strassseninstanz)
    End Sub

    Public Function isNotinList(alist As List(Of strassenUiObj), candidate As String) As Boolean
        For Each eintrag In alist
            If eintrag.sname.ToLower.Trim = candidate.ToLower.Trim Then
                Return False
            End If
        Next
        Return True
    End Function
    Class strassenUiObj
        '"sname"  SelectedValuePath="strcode" 
        Property sname As String
        Property strcode As String
        Property quelle As String
    End Class

    Private Shared Function koordinateLiegtImKreisOffenbach(googlepunkt As clsGEOPoint) As Boolean
        'grobesFenster
       
        googlepunkt.X=googlepunkt.GKrechts
        googlepunkt.Y=googlepunkt.GKhoch
        If googlepunkt.x < CDbl(initP.getValue("MiniMap.GIS_Rahmen_Fit_Xmin")) Then Return False
        If googlepunkt.x > CDbl(initP.getValue("MiniMap.GIS_Rahmen_Fit_Xmax")) Then Return False
        If googlepunkt.y <  CDbl(initP.getValue("MiniMap.GIS_Rahmen_Fit_Ymin")) Then Return False
        If googlepunkt.y >  CDbl(initP.getValue("MiniMap.GIS_Rahmen_Fit_ymax"))Then Return False
        'FrankfurtOffenbachCheck
        If googlepunkt.Y > 5546597 And googlepunkt.X < 486136 Then Return False
        Return True
    End Function

End Class
