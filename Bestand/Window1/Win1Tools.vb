Imports System.Data
Namespace Paradigma_start


    Public Class Win1Tools


        Shared Sub initgisuser(ByRef GISuser As String)
            'passowrd aus internuserdb holen
            myGlobalz.sitzung.aktBearbeiter.GISPassword =
                clsMiniMapTools.holePasswordFuerGisUser_dballe(myGlobalz.sitzung.aktBearbeiter.username, myGlobalz.sitzung.webgisREC)
            If String.IsNullOrEmpty(myGlobalz.sitzung.aktBearbeiter.GISPassword) Then
                GISuser = ""
            End If
        End Sub
        Public Shared Function initAktuellenBearbeiter() As Boolean
            My.Log.WriteEntry("myGlobalz.sitzung.Bearbeiter" & myGlobalz.sitzung.aktBearbeiter.username)
            If glob2.userIstinGastModus Then
                If Not userTools.initBearbeiterByUserid_ausParadigmadb(myGlobalz.sitzung.aktBearbeiter, "username", System.Environment.UserName) Then
                    Return False
                End If
            Else
                If Not userTools.initBearbeiterByUserid_ausParadigmadb(myGlobalz.sitzung.aktBearbeiter, "username", myGlobalz.sitzung.aktBearbeiter.username) Then
                    Return False
                End If
                'If Application.UserIsNoAdmin Then
                '    If Not String.IsNullOrEmpty(myGlobalz.sitzung.aktBearbeiter.Rang) Then
                '        If myGlobalz.sitzung.aktBearbeiter.Rang.ToLower = "vorzimmer" Then
                '            myGlobalz.sitzung.aktBearbeiter.Rang = "user"
                '            nachricht("Application.UserIsNoAdmin korrektur durchgeführt")
                '        End If
                '    End If
                'End If
            End If
            Return True
        End Function


        Public Shared Sub DokumentenArchiv_Anlegen(ByVal rd As String)
            Try
                If Not IO.Directory.Exists(rd) Then
                    IO.Directory.CreateDirectory(rd)
                End If
            Catch ex As Exception
                Dim t As String = String.Format("Fehler: Sie haben keinen Zugriff auf das Archiv !!! {0}" +
                                       " Bitte benachrichtigen sie den Admin{0}. " +
                                       " Das Programm wird beendet!!!", vbCrLf)
                nachricht_und_Mbox(t) '& ex.ToString)
                nachricht(t & ex.ToString)
            End Try
        End Sub

   
        Public Shared Sub Myglobalz_init(ByVal localAppDataParadigmaDir As String)
            Try
                My.Log.WriteEntry("in window1_loaded: nach Myglobalz_init")

                myGlobalz.sitzung.SendMail = New SendEmailTexte
                myGlobalz.sitzung = New Psession
                myGlobalz.sitzung.aktADR = New ParaAdresse
                myGlobalz.sitzung.aktParaFoto = New clsParaFoto
                myGlobalz.sitzung.aktPMU = New clsParaUmkreis
                myGlobalz.sitzung.aktPolygon = New clsParapolygon
                myGlobalz.sitzung.aktPolyline = New clsParapolyline

                myGlobalz.sitzung.aktFST = New ParaFlurstueck
                myGlobalz.sitzung.aktBearbeiter = New clsBearbeiter
                myGlobalz.sitzung.aktPerson = New Person
                'myGlobalz.sitzung.tempKontakt = New Kontaktdaten	 
                myGlobalz.sitzung.aktVorgang = New Vorgang
              '  myGlobalz.sitzung.modus = "neu"
                myGlobalz.sitzung.aktEreignis = New clsEreignis
                myGlobalz.sitzung.aktZahlung = New clsZahlung
                myGlobalz.sitzung.aktDokument = New Dokument
                myGlobalz.appdataDir = localAppDataParadigmaDir

                myGlobalz.sitzung.VerwandteDT = New DataTable
                myGlobalz.sitzung.VerwandteDTServer = New DataTable
                myGlobalz.VorlagenRoot = CType(clstart.mycsimple.iniDict("Myglobalz.VorlagenRoot"), String)

             'Debug.Print(   CLstart.mycsimple.MeinNULLDatumAlsDate.ToShortDateString)' = CDate("0001-01-01 01:01:01"))
             '   myGlobalz.MeinNULLDatumDatumAlsString = "0001-01-01 01:01:01"                 

                ' =CLstart.mycsimple.GetGisServerNetworkShare()
              '  myGlobalz.GisServerHTTPdomainIntranet =CLstart.mycsimple.getServerHTTPdomainIntranet()' 
                Dim dokArcPfad As String =CLstart.mycsimple.getDokArcPfad()
                myGlobalz.Birdsviewpfad = dokArcPfad

                DokumentenArchiv_Anlegen(dokArcPfad)

                clstart.mycsimple.Paradigma_local_root = Environment.GetFolderPath((System.Environment.SpecialFolder.DesktopDirectory)) &
                            CType(clstart.mycsimple.iniDict("Myglobalz.Paradigma_root"), String) '"\Paradigma"
                'myGlobalz.Paradigma_checkout = clstart.mycsimple.getParadigma_checkout()

                'myGlobalz.paradigma_archiv_temp = clstart.mycsimple.getParadigma_archiv_temp()'

                If Not IO.Directory.Exists(clstart.mycsimple.getParadigma_archiv_temp()) Then IO.Directory.CreateDirectory(clstart.mycsimple.getParadigma_archiv_temp())
                If Not IO.Directory.Exists(clstart.mycsimple.Paradigma_local_root) Then IO.Directory.CreateDirectory(clstart.mycsimple.Paradigma_local_root)
                If Not IO.Directory.Exists(clstart.mycsimple.getParadigma_checkout()) Then IO.Directory.CreateDirectory(clstart.mycsimple.getParadigma_checkout())
                
                If Not IO.Directory.Exists(localAppDataParadigmaDir) Then IO.Directory.CreateDirectory(localAppDataParadigmaDir)
                myGlobalz.Arc = New DokArc(dokArcPfad, clstart.mycsimple.getParadigma_checkout()) 
            Catch ex As Exception
                nachricht_und_Mbox(ex.ToString)
            End Try
        End Sub

        Public Shared Sub defineAktuellenBearbeiter(ByVal userid As String)
            My.Log.WriteEntry("in window1_loaded: in defineAktuellenBearbeiter ")
            If String.IsNullOrEmpty(userid) Then
                myGlobalz.sitzung.aktBearbeiter.username = System.Environment.GetEnvironmentVariable("username")
                'If Application.zweiteInstanz Then
                '    myGlobalz.sitzung.aktBearbeiter.username = "Gast" & "_" & myGlobalz.sitzung.aktBearbeiter.username
                '    MsgBox("Username = " & myGlobalz.sitzung.aktBearbeiter.username)
                'End If
            Else
                myGlobalz.sitzung.aktBearbeiter.username = userid
            End If
        End Sub

        Shared Sub init_archiv()
            If Not IO.Directory.Exists(myGlobalz.Arc.lokalerCheckoutcache) Then
                IO.Directory.CreateDirectory(myGlobalz.Arc.lokalerCheckoutcache)
            End If
        End Sub


        Public Shared Function initVorgaengeDatatable(ByVal hinweis As String) As Long
            Try
                hinweis = myGlobalz.sitzung.VorgangREC.getDataDT()
                Return myGlobalz.sitzung.VorgangREC.mycount
            Catch ex As Exception
                nachricht_und_Mbox(ex.ToString)
                Return -1
            End Try
        End Function
        Public Shared Function holevorgaenge(ByRef hinweis As String) As Boolean
            If Win1Tools.initVorgaengeDatatable(hinweis$) < 1 Then
                hinweis$ = "Es wurden keine Vorgänge in der DB gefunden"
                nachricht(hinweis)
                Return False
            Else
                hinweis = String.Format("Es wurden {0} Vorgänge in der DB gefunden", myGlobalz.sitzung.VorgangREC.mycount)
                nachricht(hinweis)
                Return True
            End If
        End Function

        Public Shared Function BildeBearbeiterProfilalsString(ByVal apers As clsBearbeiter) As String
            Dim info As New Text.StringBuilder
            With apers
                info.Append(String.Format("Namenszusatz: {0}{1}", .Namenszusatz, vbCrLf))
                info.Append(String.Format("Vorname: {0}{1}", .Vorname, vbCrLf))
                info.Append(String.Format("Name: {0}{1}", .Name, vbCrLf))
                info.Append("-----------------" & vbCrLf)
                info.Append(String.Format("Email: {0}{1}", .Kontakt.elektr.Email, vbCrLf))
                info.Append(String.Format("Telefon1: {0}{1}", .Kontakt.elektr.Telefon1, vbCrLf))
                info.Append(String.Format("Fax1: {0}{1}", .Kontakt.elektr.Fax1, vbCrLf))
                info.Append("-----------------" & vbCrLf)
                info.Append(String.Format("Initialen: {0}{1}", .Initiale, vbCrLf))
                info.Append(String.Format("Rolle: {0}{1}", .Rolle, vbCrLf))
                info.Append(String.Format("Kürzel: {0}{1}", .Kuerzel2Stellig, vbCrLf))
                info.Append(String.Format("Abteilung: {0}{1}", .Bemerkung, vbCrLf))
                Return info.ToString
                ' nachricht_und_Mbox(String.Format("Ihre Daten als Bearbeiter:{0}{1}", vbCrLf, info))
            End With
            info = Nothing
        End Function

        Public Shared Sub UserinifileAnlegen()
            CLstart.myc.userIniProfile = New clstart.clsINIDatei(myGlobalz.ClientCookieDir & "user.ini")
            CLstart.myc.userIniProfile.WertSchreiben("test", "bla", "jawoll")
            'Standardwerte zu Beginn einer session
            CLstart.myc.userIniProfile.WertSchreiben("Minimap", "Ausschnitt_info", "1")
            If String.IsNullOrEmpty(CLstart.myc.userIniProfile.WertLesen("Outlook", "anzeigen")) Then
                CLstart.myc.userIniProfile.WertSchreiben("Outlook", "anzeigen", "False")
            Else
                CLstart.mycsimple.outlookAnzeigen = CBool(CLstart.myc.userIniProfile.WertLesen("Outlook", "anzeigen"))
            End If
            If String.IsNullOrEmpty(CLstart.myc.userIniProfile.WertLesen("Outlook", "mailsender2Beteiligte")) Then
                CLstart.myc.userIniProfile.WertSchreiben("Outlook", "mailsender2Beteiligte", "True")
            Else
                CLstart.mycSimple.outlookAnzeigen = CBool(CLstart.myc.userIniProfile.WertLesen("Outlook", "mailsender2Beteiligte"))
            End If
            If String.IsNullOrEmpty(CLstart.myc.userIniProfile.WertLesen("Outlook", "mailCC2Beteiligte")) Then
                CLstart.myc.userIniProfile.WertSchreiben("Outlook", "mailCC2Beteiligte", "True")
            Else
                CLstart.mycSimple.outlookAnzeigen = CBool(CLstart.myc.userIniProfile.WertLesen("Outlook", "mailCC2Beteiligte"))
            End If
            If String.IsNullOrEmpty(CLstart.myc.userIniProfile.WertLesen("Outlook", "mailRecipients2Beteiligte")) Then
                CLstart.myc.userIniProfile.WertSchreiben("Outlook", "mailRecipients2Beteiligte", "True")
            Else
                CLstart.mycSimple.outlookAnzeigen = CBool(CLstart.myc.userIniProfile.WertLesen("Outlook", "mailRecipients2Beteiligte"))
            End If
            If String.IsNullOrEmpty(CLstart.myc.userIniProfile.WertLesen("Boot", "wiedervorlagenpoppen")) Then
                CLstart.myc.userIniProfile.WertSchreiben("Boot", "wiedervorlagenpoppen", "True")
            Else
                CLstart.mycSimple.outlookAnzeigen = CBool(CLstart.myc.userIniProfile.WertLesen("Boot", "wiedervorlagenpoppen"))
            End If


            If Not String.IsNullOrEmpty(CLstart.myc.userIniProfile.WertLesen("WINDOWS_SYSTEM_ANZEIGE", "FONT")) Then
                myGlobalz.WINDOWS_SYSTEM_ANZEIGE_FONT = CInt(CLstart.myc.userIniProfile.WertLesen("WINDOWS_SYSTEM_ANZEIGE", "FONT")) '#0-klein 1=mittel
            Else
                myGlobalz.WINDOWS_SYSTEM_ANZEIGE_FONT = 0 'standard ist klein
                CLstart.myc.userIniProfile.WertSchreiben("WINDOWS_SYSTEM_ANZEIGE", "FONT", "0")
            End If

            If Not String.IsNullOrEmpty(CLstart.myc.userIniProfile.WertLesen("Verlauf", "mitDokumenten")) Then
                CLstart.myc.userIniProfile.WertLesen("Verlauf", "mitDokumenten") '#0-ohne 1=mit
            Else
                CLstart.myc.userIniProfile.WertSchreiben("Verlauf", "mitDokumenten", "1")
            End If
        End Sub

    End Class

End Namespace
