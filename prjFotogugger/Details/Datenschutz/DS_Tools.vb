#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB.LIBDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports System.Data



Module DS_Tools
        Public trenn As String = ";"


        Private Sub bildeGruppenListeOutput(ByRef summe As String)
            For i = 0 To myglobalz.sitzung.VorgangREC.dt.Rows.Count - 1
                summe = summe & CStr(myglobalz.sitzung.VorgangREC.dt.Rows(i).Item(0))
                If i <> myglobalz.sitzung.VorgangREC.dt.Rows.Count - 1 Then
                    summe = summe & trenn & Environment.NewLine
                End If
            Next
        End Sub

        Public Sub gruppenzusammensetzung(ByVal gruppennummer As String)
            Dim summe As String = ""
        ds1Tools.GetGruppenDT4Gruppennummer(gruppennummer, myGlobalz.sitzung.VorgangREC)
        bildeGruppenListeOutput(summe)
        GetGruppenBeschreibung(gruppennummer)
        Dim info As String
        info = "Gruppe: >" & CStr(myGlobalz.sitzung.VorgangREC.dt.Rows(0).Item(0)) &
                "< (" & CStr(myGlobalz.sitzung.VorgangREC.dt.Rows(0).Item(1)) & ")" &
                Environment.NewLine & Environment.NewLine

        MsgBox(info & "Zur Gruppe gehören: " & Environment.NewLine &
                   summe,
                   MsgBoxStyle.OkOnly,
                   "Gruppendetail")
    End Sub

    Function vorgangsgruppeSpeichern(aktVGR As cls_ds_vorgangsgruppe, modus As String) As Boolean
        If modus = "edit" Then
            'zuerst loeschen oder update
            Dim newid As Integer = vorgangsgruppe_abspeichern_Edit_AlleDB(aktVGR)
        End If
        If modus = "neu" Then
            Dim newid As Integer = vorgangsgruppeAbspeichernNeu_AlleDB(aktVGR)
        End If
        Return True
    End Function
    Public Function vorgangsgruppe_abspeichern_Edit_AlleDB(ByVal vg As cls_ds_vorgangsgruppe) As Integer
        Dim natid%
        Dim querie As String
        clsSqlparam.paramListe.Clear()
        With vg
            clsSqlparam.paramListe.Add(New clsSqlparam("VORGANGSID", .vid))
            clsSqlparam.paramListe.Add(New clsSqlparam("gruppentext", (.VIDgruppentext)))
        End With
        querie = "UPDATE  " & CLstart.myViewsNTabs.tabDS_Vid2Gruppentext & "  " & " SET VORGANGSID=@VORGANGSID" &
                    ",gruppentext=@gruppentext " & " WHERE ID=@ID"
        clsSqlparam.paramListe.Add(New clsSqlparam("ID", vg.id))
        natid% = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, False, "ID")
        Return natid
    End Function

    Public Function vorgangsgruppeAbspeichernNeu_AlleDB(ByVal vg As cls_ds_vorgangsgruppe) As Integer
        Dim natid%
        Dim querie As String
        clsSqlparam.paramListe.Clear()
        With vg
            clsSqlparam.paramListe.Add(New clsSqlparam("VORGANGSID", .vid))
            clsSqlparam.paramListe.Add(New clsSqlparam("gruppentext", (.VIDgruppentext)))
        End With

        querie = "INSERT INTO " & CLstart.myViewsNTabs.tabDS_Vid2Gruppentext & "  (VORGANGSID,gruppentext) " &
                               " VALUES (@VORGANGSID,@gruppentext)"
        natid% = myGlobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "ID")

        Return natid
    End Function

    Function getgruppentext4Vid(ByVal lnat As cls_ds_vorgangsgruppe, irec As IDB_grundfunktionen) As String
        Dim dt As DataTable : Dim hinweis As String = ""
        'irec.mydb.Tabelle ="" & CLstart.myViewsNTabs.tabDS_Vid2Gruppentext & " "
        irec.mydb.SQL = "select * from " & CLstart.myViewsNTabs.tabDS_Vid2Gruppentext & "  " &
                 " where vorgangsid=" & lnat.vid
        dt = getDT4Query(irec.mydb.SQL, irec, hinweis)
        If dt.IsNothingOrEmpty Then
            'keine daten vorhanden
            lnat.VIDgruppentext = ""

            l(" vehler in getgruppentext4Vid:  dt.IsNothingOrEmpty " & irec.mydb.SQL)
        Else
            lnat.VIDgruppentext = clsDBtools.fieldvalue(dt.Rows(0).Item("gruppentext"))
            lnat.id = CInt(clsDBtools.fieldvalue(dt.Rows(0).Item("id")))
        End If
        dt = Nothing
        Return lnat.VIDgruppentext
    End Function

    Function getDS_userStandard(bearbeiterid As Integer, irec As IDB_grundfunktionen) As String
        Dim dt As DataTable : Dim hinweis As String = ""
        Dim gr As String = "1"
        If bearbeiterid < 1 Then
            Return "1"
        End If
        gr = "1"
        'irec.mydb.Tabelle =" " & CLstart.myViewsNTabs.tabDS_Standards & " "
        irec.mydb.SQL = "select gruppentext from  " & CLstart.myViewsNTabs.tabDS_Standards & "   where bearbeiterid=" & bearbeiterid
        dt = getDT4Query(irec.mydb.SQL, irec, hinweis)
        If Not dt.IsNothingOrEmpty Then
            gr = clsDBtools.fieldvalue(dt.Rows(0).Item(0))
        Else
            gr = "1"
        End If
        'Else
        '    gr = "1"
        'End If
        dt = Nothing
        Return gr
    End Function




    Function istHauptBearbeiter(ByRef info As String) As Boolean
        info = myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.username & ", " &
                myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Kontakt.elektr.Telefon1
        'Dim aktKue As String = myGlobalz.sitzung.aktBearbeiter.Kuerzel2Stellig.ToLower.Trim
        'Dim hauptKue As String = myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Kuerzel2Stellig.ToLower.Trim
        If (myGlobalz.sitzung.aktBearbeiter.ID = myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.ID) Then
            'If aktKue = hauptKue Then
            nachricht("istHauptBearbeiter" & True)
            Return True
        End If
        nachricht("istHauptBearbeiter" & False)
        Return False
    End Function



    Public Function istFachdienstAssistenz() As Boolean
        Dim summe As String = ""
        'GetGruppenDT4Gruppennummer("13")
        ds1Tools.GetGruppenDT4Gruppennummer(CStr(13), myGlobalz.sitzung.VorgangREC)
        ds1Tools.bildeGruppenString(summe, trenn, myGlobalz.sitzung.VorgangREC)
        If LIBgemeinsames.clsString.isinarray(summe, CStr(myGlobalz.sitzung.aktBearbeiter.username), trenn) Then
            Return True
        Else
            Return False
        End If
    End Function

    Function DS_formularZugriffErlaubt(ByRef HauptBearbeiter As String) As Short
        If DS_Tools.istWeitererBearbeiter Then Return 1
        If DS_Tools.istHauptBearbeiter(HauptBearbeiter) Then Return 2
        If ds1Tools.istFachdienstLeitung(myGlobalz.sitzung.VorgangREC, myGlobalz.sitzung.aktBearbeiter.username, trenn) Then Return 3
        If DS_Tools.istFachdienstAssistenz Then Return 4
            If DS_Tools.istEinzelerlaubnis(myglobalz.sitzung.aktVorgangsID, myglobalz.sitzung.aktBearbeiter.username) Then Return 5
            Return 0
        End Function

        Public Function istWeitererBearbeiter() As Boolean
            If LIBgemeinsames.clsString.isinarray(myglobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter.ToLower, myglobalz.sitzung.aktBearbeiter.Initiale.ToLower, trenn) Then
                Return True
            End If
            Return False
        End Function

    Function getGruppen4user(bearbeiterid As Integer, irec As IDB_grundfunktionen) As String
        Dim dt As DataTable
        Dim hinweis As String = ""
        Dim KlartextSumme As String = ""
        ' Dim summe As String = ""
        'irec.mydb.Tabelle =""
        irec.mydb.SQL = "select g.name,u.bearbeiterid,u.gruppenid from  " & CLstart.myViewsNTabs.tabDS_USER2GRUPPE & "  u, " & CLstart.myViewsNTabs.tabDS_Gruppen & "  g " &
                    " where bearbeiterid= " & bearbeiterid &
                    " and u.gruppenid=g.gruppenid"
        dt = getDT4Query(irec.mydb.SQL, myglobalz.sitzung.VorgangREC, hinweis)
        KlartextSumme = " keine angaben"
        If Not dt.IsNothingOrEmpty Then
            For i = 0 To dt.Rows.Count - 1
                KlartextSumme = KlartextSumme & clsDBtools.fieldvalue(dt.Rows(i).Item("name")) & ", " & Environment.NewLine
            Next
        Else
            KlartextSumme = " keine angaben"
        End If
        'Else
        '    KlartextSumme = " keine angaben"
        'End If
        dt = Nothing
        Return KlartextSumme
    End Function

    Private Sub GetGruppenBeschreibung(gruppennummer As String)
        myGlobalz.sitzung.VorgangREC.mydb.SQL = "select name,beschreibung from  " & CLstart.myViewsNTabs.tabDS_Gruppen & "  " &
                                                    " where gruppenid=" & gruppennummer
        Dim hinweis As String = myglobalz.sitzung.VorgangREC.getDataDT()
        End Sub

        Sub MailanHauptbearbeiterWgDS_aenderung(aktbearbeiter As String, cls_ds_vorgangsgruppe As cls_ds_vorgangsgruppe,
                                                clsBearbeiter As clsBearbeiter)
            Dim an As String = clsBearbeiter.Kontakt.elektr.Email
            'Dim an As String = "dr.j.feinen@kreis-offenbach.de"
            Dim betreff As String = "Leserechte für Vorgang " & cls_ds_vorgangsgruppe.vid & " wurden geändert"
            Dim mailtext As String = "Hallo," & Environment.NewLine &
                "Die Leserechte für diesen Vorgang wurden geändert: " & Environment.NewLine &
                "von: " & aktbearbeiter & " (" & Environment.UserName & ") " & Environment.NewLine &
                "am: " & Now.ToString() & Environment.NewLine &
                "------------------------" & Environment.NewLine &
              "(" & cls_ds_vorgangsgruppe.VIDgruppentext & ")" & Environment.NewLine
            Dim test As Boolean = clsMailsenden.mailrausSMTP(aktbearbeiter, an,
                                                           betreff, mailtext.Replace(vbCrLf, "<br>"),
                                                        "", False, "", "", "")
        End Sub

        Function einzelstring_bilden(presentationsstring As String) As String
            If presentationsstring.Trim.IsNothingOrEmpty Then Return ""
            Dim a As String() = presentationsstring.Trim.Split(" "c)
            Dim summe As String = ""
            For i = 0 To a.Count - 1
                summe = summe & trenn & a(i).Trim
            Next
            Return summe
        End Function

        Public Function istEinzelerlaubnis(vid As Integer, username As String) As Boolean
            Dim lnat As New cls_ds_vorgangsgruppe
        lnat.vid = vid
        'Debug.Print("" & myGlobalz.sitzung.VorgangREC)
        Dim gruppentext As String = DS_Tools.getgruppentext4Vid(lnat, myGlobalz.sitzung.VorgangREC)
        If gruppentext.IsNothingOrEmpty Then
                If gruppentext.IsNothingOrEmpty Then gruppentext = "1"
            End If
            Dim a As String() = gruppentext.Split(CChar(trenn))
            For i = 0 To a.GetUpperBound(0)
                If Not IsNumeric(a(i)) Then
                    If a(i).ToLower.Trim = username.ToLower.Trim Then
                        Return True
                    End If

                End If
            Next
            Return False
        End Function

        Private Function GetVIDgruppentext(ByVal lnat As cls_ds_vorgangsgruppe) As String
            Dim VIDgruppentext As String
        VIDgruppentext = DS_Tools.getgruppentext4Vid(lnat, myGlobalz.sitzung.VorgangREC)
        If VIDgruppentext.IsNothingOrEmpty Then
            VIDgruppentext = DS_Tools.getDS_userStandard(myGlobalz.sitzung.aktBearbeiter.ID, myGlobalz.sitzung.VorgangREC)
            If VIDgruppentext.IsNothingOrEmpty Then VIDgruppentext = "1"
            End If
            Return VIDgruppentext
        End Function

        Private Sub usergruppentextholen(ByRef USERgruppentext As String)
            Dim dt As DataTable
        'Dim zzz As New clsDS(clsDBspecOracle.getConnection(myglobalz.beteiligte_MYDB))
        Dim query, hinweis As String
        query = "select g.name,u.bearbeiterid,u.gruppenid from  " & CLstart.myViewsNTabs.tabDS_USER2GRUPPE & "  u, " & CLstart.myViewsNTabs.tabDS_Gruppen & "  g " &
                    " where bearbeiterid= " & myGlobalz.sitzung.aktBearbeiter.ID &
                    " and u.gruppenid=g.gruppenid"
#Disable Warning BC42030 ' Variable 'hinweis' is passed by reference before it has been assigned a value. A null reference exception could result at runtime.
        dt = getDT4Query(query, myGlobalz.sitzung.VorgangREC, hinweis)
#Enable Warning BC42030 ' Variable 'hinweis' is passed by reference before it has been assigned a value. A null reference exception could result at runtime.
        '(myGlobalz.sitzung.aktBearbeiter.ID, myGlobalz.sitzung.VorgangREC)
        'zzz.Dispose()

        If Not dt.IsNothingOrEmpty Then
                For i = 0 To dt.Rows.Count - 1
                    USERgruppentext = USERgruppentext & clsDBtools.fieldvalue(dt.Rows(i).Item("gruppenid")) & ";"
                Next
            Else
                USERgruppentext = " keine angaben"
            End If
        End Sub
        Private Sub usergruppentextKuerzen(ByRef USERgruppentext As String)
            If USERgruppentext.Contains(";") Then
                USERgruppentext = USERgruppentext.Substring(0, USERgruppentext.Length - 1)
            End If
        End Sub
        Private Sub istEineGruppeGemeinsam(ByRef gruppe As String, ByRef returncode As Boolean, ByVal VIDgruppen As String(), ByVal USERgruppen As String())
            For i = 0 To VIDgruppen.GetUpperBound(0)
                For j = 0 To USERgruppen.GetUpperBound(0)
                    If IsNumeric(VIDgruppen(i)) Then
                        If VIDgruppen(i).Trim = USERgruppen(j).ToLower.Trim Then
                            gruppe = (VIDgruppen(i))
                            returncode = True
                            Exit Sub
                        End If
                    End If
                Next
            Next
        End Sub
        Function aktuserIstTeilDerGruppe(ByRef gruppe As String) As Boolean
            Dim lnat As New cls_ds_vorgangsgruppe
            Dim returncode As Boolean = False
            Dim USERgruppen As String()
            Dim VIDgruppentext As String
            Dim VIDgruppen As String()
            Dim USERgruppentext As String = ""
            Try
                lnat.vid = myglobalz.sitzung.aktVorgangsID
                VIDgruppentext = GetVIDgruppentext(lnat)
                VIDgruppen = VIDgruppentext.Split(CChar(trenn))
                usergruppentextholen(USERgruppentext)
                usergruppentextKuerzen(USERgruppentext)
                USERgruppen = USERgruppentext.Split(";"c)
                istEineGruppeGemeinsam(gruppe, returncode, VIDgruppen, USERgruppen)
                Return returncode
            Catch ex As Exception
                nachricht("fehler in aktuserIstTeilDerGruppe: ", ex)
                Return False
            End Try
        End Function

        Private Sub ausschreibenProtokoll(ByVal text As String, ByVal pfad As String)
            Try
                Dim sr As New IO.StreamWriter(pfad, True)
                sr.Write(text)
                sr.Close()
                sr = Nothing
            Catch ex As Exception
                nachricht("fehler in ausschreibenProtokoll: " & text & "/ " & pfad, ex)
            End Try
        End Sub

        Private Sub DS_protokollTextbilden(ByVal modul As String, ByVal trenn As String, ByRef text As String, code As Integer, gruppe As String)
            text = text & Now & trenn
            text = text & modul & trenn
            text = text & myglobalz.sitzung.aktBearbeiter.username & trenn
            text = text & Environment.UserName & trenn
            text = text & myglobalz.sitzung.aktVorgangsID & trenn
            text = text & myglobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.username & trenn
            text = text & code & trenn
            text = text & gruppe & trenn
            text = text & Environment.NewLine
        End Sub
        Sub DS_protokoll(modul As String, code As Integer, gruppe As String)
            Dim trenn As String = ";"
            Dim text As String = ""
            DS_protokollTextbilden(modul, trenn, text, code, gruppe)
        Dim pfad As String = "O:\UMWELT\B\GISDatenEkom\div\DS_protokoll\fremdzugriff.txt"
        ausschreibenProtokoll(text, pfad)
        End Sub

        Function istPersonalVorgang(sachgebietZahl As String) As Boolean
            If sachgebietZahl.IsNothingOrEmpty Then
                nachricht("fehler in istPersonalVorang wert ist leer")
                Return False
            End If
            If Not IsNumeric(sachgebietZahl) Then
            'nachricht("warnung in istPersonalVorgang wert ist nicht numerisch: " & sachgebietZahl)
            Return False
            End If
            If sachgebietZahl.Trim = "1500" Then
                nachricht(" istPersonalVorang !!!!!!!!!!!!!!!")
                Return True
            End If
            Return False
        End Function
        Public Function getMehrereBearbeiter(mehrfachauswahl As String,tbEinzelpersonenText As string) As String
        Dim tempResult As String
        If String.IsNullOrEmpty(mehrfachauswahl) Then
            tempResult = ""
        Else
            If mehrfachauswahl = "####" Then
                tempResult = tbEinzelpersonenText
            Else
                tempResult = mehrfachauswahl.Replace(";", " ").Trim
            End If
        End If

        Return tempResult
    End Function
    End Module
 