Imports System.Data
Namespace Paradigma_start


    Public Class Win1Tools


        'Shared Sub initgisuser(ByRef GISuser As String)
        '    myglobalz.sitzung.aktBearbeiter.GISPassword = clsMiniMapTools.holePasswordFuerGisUser_dballe(myglobalz.sitzung.aktBearbeiter.username, myglobalz.sitzung.webgisREC)
        '    If String.IsNullOrEmpty(myglobalz.sitzung.aktBearbeiter.GISPassword) Then
        '        GISuser = Environment.UserName
        '    End If
        'End Sub
        Public Shared Function initAktuellenBearbeiter() As Boolean
            My.Log.WriteEntry("myGlobalz.sitzung.Bearbeiter" & myglobalz.sitzung.aktBearbeiter.username)
            If myGlobalz.sitzung.aktBearbeiter.username = "hurz" Or myGlobalz.sitzung.aktBearbeiter.username = "zahnlückenpimpf" Then
                myGlobalz.sitzung.aktBearbeiter.username = "Feinen_J"
            End If
            'If glob2.userIstinGastModus Then
            '    If Not userTools.initBearbeiterByUserid_ausParadigmadb(myglobalz.sitzung.aktBearbeiter, "username", System.Environment.UserName) Then
            '        Return False
            '    End If
            'Else
            If Not userTools.initBearbeiterByUserid_ausParadigmadb(myGlobalz.sitzung.aktBearbeiter) Then
                Return False
            End If
            If Application.UserIsNoAdmin Then
                    If Not String.IsNullOrEmpty(myglobalz.sitzung.aktBearbeiter.Rang) Then
                        If myglobalz.sitzung.aktBearbeiter.Rang.ToLower = "vorzimmer" Then
                            myglobalz.sitzung.aktBearbeiter.Rang = "user"
                            nachricht("Application.UserIsNoAdmin korrektur durchgeführt")
                        End If
                    End If
                End If
            'End If
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
                nachricht_und_Mbox(t) ',ex)
                nachricht(t ,ex)
            End Try
        End Sub


        Public Shared Sub Myglobalz_init(ByVal localAppDataParadigmaDir As String)
            Try
                My.Log.WriteEntry("in window1_loaded: nach Myglobalz_init")

                myglobalz.sitzung.SendMail = New SendEmailTexte
                myglobalz.sitzung = New Psession
                myglobalz.sitzung.aktADR = New ParaAdresse
                myglobalz.sitzung.aktParaFoto = New clsParaFoto
                myglobalz.sitzung.aktPMU = New clsParaUmkreis
                myglobalz.sitzung.aktPolygon = New clsParapolygon
                myglobalz.sitzung.aktPolyline = New clsParapolyline

                myglobalz.sitzung.aktFST = New ParaFlurstueck
                myglobalz.sitzung.aktBearbeiter = New clsBearbeiter
                myglobalz.sitzung.aktPerson = New Person
                'myGlobalz.sitzung.tempKontakt = New Kontaktdaten	 
                myglobalz.sitzung.aktVorgang = New Vorgang
                '  myGlobalz.sitzung.modus = "neu"
                myglobalz.sitzung.aktEreignis = New clsEreignis
                myglobalz.sitzung.aktZahlung = New clsZahlung
                myglobalz.sitzung.aktDokument = New Dokument
                myglobalz.appdataDir = localAppDataParadigmaDir

                myglobalz.sitzung.VerwandteDT = New DataTable
                myglobalz.sitzung.VerwandteDTServer = New DataTable
                myglobalz.VorlagenRoot = CType(CLstart.mycSimple.iniDict("Myglobalz.VorlagenRoot"), String)

                'Debug.Print(   CLstart.mycsimple.MeinNULLDatumAlsDate.ToShortDateString)' = CDate("0001-01-01 01:01:01"))
                '   myGlobalz.MeinNULLDatumDatumAlsString = "0001-01-01 01:01:01"                 

                ' =CLstart.mycsimple.GetGisServerNetworkShare()

                Dim dokArcPfad As String = CLstart.mycSimple.getDokArcPfad()
                myglobalz.Birdsviewpfad = dokArcPfad

                DokumentenArchiv_Anlegen(dokArcPfad)

                CLstart.mycSimple.Paradigma_local_root = Environment.GetFolderPath((System.Environment.SpecialFolder.DesktopDirectory)) &
                            CType(CLstart.mycSimple.iniDict("Myglobalz.Paradigma_root"), String) '"\Paradigma"
                'myGlobalz.Paradigma_checkout = clstart.mycsimple.getParadigma_checkout()

                'myGlobalz.paradigma_archiv_temp = clstart.mycsimple.getParadigma_archiv_temp()'

                If Not IO.Directory.Exists(CLstart.mycSimple.getParadigma_archiv_temp()) Then IO.Directory.CreateDirectory(CLstart.mycSimple.getParadigma_archiv_temp())
                If Not IO.Directory.Exists(CLstart.mycSimple.Paradigma_local_root) Then IO.Directory.CreateDirectory(CLstart.mycSimple.Paradigma_local_root)
                If Not IO.Directory.Exists(CLstart.mycSimple.getParadigma_checkout()) Then IO.Directory.CreateDirectory(CLstart.mycSimple.getParadigma_checkout())

                If Not IO.Directory.Exists(localAppDataParadigmaDir) Then IO.Directory.CreateDirectory(localAppDataParadigmaDir)
                myglobalz.Arc = New DokArc(dokArcPfad, CLstart.mycSimple.getParadigma_checkout())
            Catch ex As Exception
                nachricht_und_Mbox(ex.ToString)
            End Try
        End Sub

        Public Shared Sub defineAktuellenBearbeiter(ByVal userid As String)
            My.Log.WriteEntry("in window1_loaded: in defineAktuellenBearbeiter ")
            If String.IsNullOrEmpty(userid) Then
                myGlobalz.sitzung.aktBearbeiter.username = Environment.UserName
                'If Application.zweiteInstanz Then
                '    myglobalz.sitzung.aktBearbeiter.username = "Gast" & "_" & myglobalz.sitzung.aktBearbeiter.username
                '    MsgBox("Username = " & myglobalz.sitzung.aktBearbeiter.username)
                'End If
            Else
                myglobalz.sitzung.aktBearbeiter.username = userid
            End If
        End Sub

        Shared Sub init_archiv()
            If Not IO.Directory.Exists(myglobalz.Arc.lokalerCheckoutcache) Then
                IO.Directory.CreateDirectory(myglobalz.Arc.lokalerCheckoutcache)
            End If
        End Sub


        Public Shared Function initVorgaengeDatatable(ByVal hinweis As String) As Long
            Try
                hinweis = myglobalz.sitzung.VorgangREC.getDataDT()
                Return myglobalz.sitzung.VorgangREC.mycount
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
                hinweis = String.Format("Es wurden {0} Vorgänge in der DB gefunden", myglobalz.sitzung.VorgangREC.mycount)
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




    End Class

End Namespace
