Imports System
Imports System.IO.Compression
Public Class clsStartup
    Public Shared Property abbruch As Boolean = False
    Public Shared vorgangsvorschlag As String = "0"
    Public Shared vorherigerVorgang As String = "0"
    'Public Shared Sub NeuerVorgang2()
    '    If Application.Application_StartupExtracted() = 2 Then
    '        MessageBox.Show("Es sind bereits zwei Vorgänge geöffnet! Schließen sie bitte einen Vorgang. Abbruch!", "Vorgang öffnen", MessageBoxButton.OK, MessageBoxImage.Exclamation)
    '        Exit Sub
    '    End If
    '    clsTools.allAktobjReset.execute(myGlobalz.sitzung)
    '    Dim vorherigervorgang As Integer = myGlobalz.sitzung.aktVorgangsID
    '    Dim az As String = ""
    '    Dim header As String = ""
    '    Dim alter_titel As String = ""
    '    Dim alter_probaugAz As String = ""
    '    Dim altergemKRZ As String = ""
    '    Dim vorgangsvorschlag As String = LocalParameterFiles.parameterEinlesen(az, header, alter_titel, alter_probaugAz, altergemKRZ)
    '    myGlobalz.sitzung.aktVorgang.Stammdaten.Beschreibung = alter_titel
    '    myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz = alter_probaugAz
    '    myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ = altergemKRZ

    '    myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter = CLstart.myc.userIniProfile.WertLesen("Stammdaten", "weiterebearbeiter")
    '    CLstart.VIDuebergabe.erzeugeParameterDatei(CInt(myGlobalz.sitzung.aktVorgangsID), myGlobalz.sitzung.aktBearbeiter.username, CLstart.mycSimple.Paradigma_local_root, "vorherigerVorgang")

    '    Dim nnn As New Win_Stamm("neu", myGlobalz.sitzung.aktVorgang.Stammdaten, alter_titel, altergemKRZ, alter_probaugAz)
    '    nnn.ShowDialog()
    '    If nnn.abbruch Then
    '        abbruch = True
    '    Else
    '        If nnn.DialogResult.HasValue Then
    '            If nnn.DialogResult.Value Then
    '                Threading.Thread.Sleep(10000)


    '                glob2.editVorgang(CInt(myGlobalz.sitzung.aktVorgangsID))
    '            End If
    '        End If
    '    End If
    '    CLstart.VIDuebergabe.erzeugeParameterDatei(vorherigervorgang, myGlobalz.sitzung.aktBearbeiter.username, CLstart.mycSimple.Paradigma_local_root, "vorherigerVorgang")
    '    GC.Collect()
    'End Sub
    Shared Sub neuerVorgang3()
        Dim si As New ProcessStartInfo
        'si.FileName = initP.getValue("ExterneAnwendungen.Application_Stakeholder")
        si.FileName = "C:\kreisoffenbach\paradigmaNeuerVorgang\paradigmaNeuerVorgang.exe "
        si.WorkingDirectory = "C:\kreisoffenbach\paradigmaNeuerVorgang"
        si.Arguments = "modus=normal"
        Process.Start(si)
        si = Nothing
    End Sub
    Shared Function suchenNachVorgaengen(ByRef az As String, ByRef header As String) As String
        vorgangsvorschlag = LocalParameterFiles.parameterEinlesen(az, header, "", "", "")
        Dim vorgangsid As String = Microsoft.VisualBasic.Interaction.InputBox(glob2.getMsgboxText("bitteVorgangsNrEingeben", New List(Of String)(New String() {})) & header & vbCrLf & "(" & az & ")", "Vorgang direkt aufrufen:",
                         vorgangsvorschlag)
        If Not String.IsNullOrEmpty(vorgangsid) Then

            LocalParameterFiles.erzeugeParameterDateiAktvorgang_txt(False, False)
            CLstart.VIDuebergabe.erzeugeParameterDatei(CInt(vorgangsid), myGlobalz.sitzung.aktBearbeiter.username, CLstart.mycSimple.Paradigma_local_root, "aktvorgang2")
            '  glob2.editVorgang(CInt(vorgangsid))
            '  vorgangsvorschlag = LocalParameterFiles.parameterEinlesen(az, header, "", "", "")
        End If
        Return vorgangsid
    End Function

    Public Shared Function Historydatenrecherchieren(ByVal vorgangsid As String, ByRef az As String, ByRef header As String) As Boolean
        Dim hinweis As String
        Try
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "select beschreibung , az2 from " & CLstart.myViewsNTabs.tabStammdaten & " where vorgangsid=" & vorgangsid
            hinweis = myGlobalz.sitzung.VorgangREC.getDataDT()
            header = clsDBtools.fieldvalue(myGlobalz.sitzung.VorgangREC.dt.Rows(0).Item("beschreibung"))
            az = clsDBtools.fieldvalue(myGlobalz.sitzung.VorgangREC.dt.Rows(0).Item("az2"))
            Return True
        Catch ex As Exception
            nachricht("Fehler in Historydatenrecherchieren: " ,ex)
            Return False
        End Try
    End Function

    Public Shared Sub initBestandsSQL()
        If myGlobalz.sitzung.aktBearbeiter.istUser_admin_oder_vorzimmer Then
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "SELECT * FROM (" & CLstart.myViewsNTabs.view_vsk_d & ") " &
                                                       " where ROWNUM <=  1000 " &
                                                       " order by LetzteBearbeitung desc  "
        Else
            myGlobalz.sitzung.VorgangREC.mydb.SQL =
                               String.Format("SELECT * FROM (" & CLstart.myViewsNTabs.view_vsk_d & ") " &
                                               " where      ROWNUM <=  1000 " &
                                               " and (Bearbeiter='{0}' or Bearbeiter='{1}') " &
                                               " order by vorgangsid desc ",
                               myGlobalz.sitzung.aktBearbeiter.Initiale, myGlobalz.sitzung.aktBearbeiter.Kuerzel2Stellig)
        End If
    End Sub

    'Public Shared Sub FormularBestandStammdaten(nurzumkuckenmodus As Boolean, Optional modalOeffnen As Boolean = True)
    '    initBestandsSQL()
    '    Dim wz As New winBestandStammdaten(False, nurzumkuckenmodus)
    '    If modalOeffnen Then
    '        wz.ShowDialog()
    '    Else
    '        wz.Show()
    '    End If
    'End Sub

    'Public Shared Sub FormularBestandBeteiligte(nurZumKuckenModus As Boolean)
    '    Dim wz As New winBestandBeteiligte(nurZumKuckenModus)
    '    wz.ShowDialog()
    'End Sub

    'Public Shared Sub FormularBestandEreignis(_nurZumKuckenModus As Boolean)
    '    Dim wz As New winEreignisfilter(_nurZumKuckenModus)
    '    wz.ShowDialog()
    'End Sub

    'Public Shared Sub FormularBestandFlurstueckfilter(_nurZumKuckenModus As Boolean)
    '    Dim wz As New winFlurstueckFilter(_nurZumKuckenModus)
    '    wz.ShowDialog()
    'End Sub

    'Public Shared Sub FormularBestandAdressFilter(_nurZumKuckenModus As Boolean)
    '    Dim wz As New winAdressBestand(_nurZumKuckenModus)
    '    wz.ShowDialog()
    'End Sub

    'Public Shared Sub FormularBestandDokuFilter(_nurZumKuckenModus As Boolean)
    '    Dim wz As New winDokuFilter(_nurZumKuckenModus)
    '    wz.ShowDialog()

    'End Sub

    'Shared Sub FormularBestandProjektFilter(_nurZumKuckenModus As Boolean)
    '    Dim wz As New winfilterProjekte(_nurZumKuckenModus)
    '    wz.ShowDialog()
    'End Sub

    'Shared Sub FormularBestandWvFilter(_nurZumKuckenModus As Boolean)
    '    Dim wvtool As New clsWVTOOLS
    '    wvtool.getWiedervorlageDT(myGlobalz.sitzung.aktBearbeiter.Initiale)
    '    Dim wz As New WinWvFilter(_nurZumKuckenModus)
    '    wz.ShowDialog()
    'End Sub

    'Shared Sub FormularBestandNaturegFilter(_nurZumKuckenModus As Boolean)
    '    Dim wz As New NaturegFilter(_nurZumKuckenModus)
    '    wz.ShowDialog()
    'End Sub

    Shared Sub ShowSplashScreen()
        'Dim asplish As New SplishSplash
        'asplish.Show()
        'asplish.Close()

    End Sub

End Class
