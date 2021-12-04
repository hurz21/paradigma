Imports paradigma.CLstart

Public Class clsStartup
    Public Shared vorgangsvorschlag As String
    'Shared Sub neuerVorgang3()
    '    Dim si As New ProcessStartInfo
    '    'si.FileName = initP.getValue("ExterneAnwendungen.Application_Stakeholder")
    '    si.FileName = "C:\kreisoffenbach\paradigmaNeuerVorgang\paradigmaNeuerVorgang.exe "
    '    si.WorkingDirectory = "C:\kreisoffenbach\paradigmaNeuerVorgang"
    '    si.Arguments = "modus=normal"
    '    Process.Start(si)
    '    si = Nothing
    'End Sub
    'Public Shared Sub NeuerVorgang2()
    '    If myGlobalz.einVorgangistgeoeffnet Then
    '        MessageBox.Show("Es ist bereits ein Vorgang geöffnet! Es kann immer nur EIN Vorgang geöffnet werden. Abbruch!", "Vorgang öffnen", MessageBoxButton.OK, MessageBoxImage.Exclamation)

    '        Exit Sub
    '    End If
    '    glob3.allAktobjReset.execute(myGlobalz.sitzung)

    '    Dim az As String = ""
    '    Dim header As String = ""
    '    Dim alter_titel As String = ""
    '    Dim alter_probaugAz As String = ""
    '    Dim altergemKRZ As String = ""
    '    Dim vorgangsvorschlag As String = LocalParameterFiles.parameterEinlesen(az, header, alter_titel, alter_probaugAz, altergemKRZ)
    '    myGlobalz.sitzung.aktVorgang.Stammdaten.Beschreibung = alter_titel
    '    myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz = alter_probaugAz
    '    myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ = altergemKRZ

    '    Debug.Print(myGlobalz.sitzung.aktBearbeiter.ID.ToString)
    '    Debug.Print(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.ID.ToString)

    '    myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter = CLstart.myc.userIniProfile.WertLesen("Stammdaten", "weiterebearbeiter")


    '    Dim nnn As New Win_Stamm("neu", myGlobalz.sitzung.aktVorgang.Stammdaten, alter_titel, altergemKRZ, alter_probaugAz)
    '    nnn.ShowDialog()
    '    If nnn.DialogResult.HasValue Then
    '        If nnn.DialogResult.Value Then
    '            LocalParameterFiles.erzeugeParameterDatei(False, False)
    '            glob2.editVorgang(CInt(myGlobalz.sitzung.aktVorgangsID), myGlobalz.testmode)
    '        End If
    '    End If
    '    GC.Collect()
    'End Sub

    Shared Sub suchenNachVorgaengen(ByRef az As String, ByRef header As String)
        vorgangsvorschlag = LocalParameterFiles.parameterEinlesen(az, header, "", "", "")
        Dim vorgangsid As String = Microsoft.VisualBasic.Interaction.InputBox(
                         "Bitte geben Sie die VorgangsNr. ein: " & vbCrLf &
                         "  " & vbCrLf &
                         "Zuletzt bearbeitet: " & vbCrLf & header & vbCrLf & "(" & az & ")", "Vorgang direkt aufrufen:",
                         vorgangsvorschlag)
        If Not String.IsNullOrEmpty(vorgangsid) Then
            myGlobalz.sitzung.aktVorgangsID = CInt(vorgangsid)
            LocalParameterFiles.erzeugeParameterDatei(False, False)
            glob2.editVorgang(CInt(vorgangsid), myGlobalz.testmode)
            vorgangsvorschlag = LocalParameterFiles.parameterEinlesen(az, header, "", "", "")
        End If
    End Sub

    Public Shared Function Historydatenrecherchieren(ByVal vorgangsid As String, ByRef az As String, ByRef header As String) As Boolean
        Dim hinweis As String
        Try
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "select beschreibung , az2 from " & CLstart.myViewsNTabs.tabStammdaten & " where vorgangsid=" & vorgangsid
            hinweis = myGlobalz.sitzung.VorgangREC.getDataDT()
            header = clsDBtools.fieldvalue(myGlobalz.sitzung.VorgangREC.dt.Rows(0).Item("beschreibung"))
            az = clsDBtools.fieldvalue(myGlobalz.sitzung.VorgangREC.dt.Rows(0).Item("az2"))
            Return True
        Catch ex As Exception
            nachricht("Fehler in Historydatenrecherchieren: " & ex.ToString)
            Return False
        End Try
    End Function

    Public Shared Sub initBestandsSQL()
        nachricht("initBestandsSQL")
        If (clsParadigmaRechte.istUser_admin_oder_vorzimmer()) Then
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
            myGlobalz.sitzung.VorgangREC.mydb.SQL =
  String.Format("SELECT * FROM (" & CLstart.myViewsNTabs.view_vsk_d & ") " &
                                     " where      ROWNUM <=  1000 " &
                                     " and (Bearbeiterid={0}) " &
                                     " order by vorgangsid desc ",
                     myGlobalz.sitzung.aktBearbeiter.ID)
        End If
        nachricht("initBestandsSQL 2: " & myGlobalz.sitzung.VorgangREC.mydb.SQL)
    End Sub

    Public Shared Sub FormularBestandStammdaten(nurzumkuckenmodus As Boolean, Optional modalOeffnen As Boolean = True)
        nachricht("FormularBestandStammdaten1")
        initBestandsSQL()
        nachricht("FormularBestandStammdaten2")
        Dim wz As New winStammBestand(False, nurzumkuckenmodus)
        If modalOeffnen Then
            wz.ShowDialog()
        Else
            wz.Show()
        End If
        nachricht("FormularBestandStammdaten3")
    End Sub

    Public Shared Sub FormularBestandBeteiligte(nurZumKuckenModus As Boolean)
        Dim wz As New winBestandBeteiligte(nurZumKuckenModus)
        wz.ShowDialog()
    End Sub

    Public Shared Sub FormularBestandEreignis(_nurZumKuckenModus As Boolean)
        Dim wz As New winEreignisfilter(_nurZumKuckenModus)
        wz.ShowDialog()
    End Sub

    Public Shared Sub FormularBestandFlurstueckfilter(_nurZumKuckenModus As Boolean)
        Dim wz As New winFlurstueckFilter(_nurZumKuckenModus)
        wz.ShowDialog()
    End Sub

    Public Shared Sub FormularBestandAdressFilter(_nurZumKuckenModus As Boolean)
        Dim wz As New winAdressBestand(_nurZumKuckenModus)
        wz.ShowDialog()
    End Sub

    Public Shared Sub FormularBestandDokuFilter(_nurZumKuckenModus As Boolean)
        Dim wz As New winDokuFilter(_nurZumKuckenModus)
        wz.ShowDialog()

    End Sub

    Shared Sub FormularBestandProjektFilter(_nurZumKuckenModus As Boolean)
        Dim wz As New winfilterProjekte(_nurZumKuckenModus)
        wz.ShowDialog()
    End Sub

    Shared Sub FormularBestandWvFilter(_nurZumKuckenModus As Boolean)
        Try
            Dim wvtool As New clsWVTOOLS
            wvtool.getWiedervorlageDT(CType(myGlobalz.sitzung.aktBearbeiter.ID, String))
            Dim wz As New WinWvFilter(_nurZumKuckenModus)
            wz.Show() 'ihah
        Catch ex As Exception

        End Try

    End Sub

    Shared Sub FormularBestandNaturegFilter(_nurZumKuckenModus As Boolean)
        Dim wz As New NaturegFilter(_nurZumKuckenModus)
        wz.ShowDialog()
    End Sub
    Shared Sub FormularBestandIllegbauFilter(_nurZumKuckenModus As Boolean)
        Dim wz As New IllegbauFilter(_nurZumKuckenModus)
        wz.ShowDialog()
    End Sub
    Shared Sub ShowSplashScreen()
        'Dim asplish As New SplishSplash
        'asplish.Show()
        'asplish.Close()

    End Sub

End Class
