Public Class clsStartup
    Public Shared vorgangsvorschlag As String
    Public Shared Sub NeuerVorgang2()
        If myGlobalz.einVorgangistgeoeffnet Then
            MessageBox.Show("Es ist bereits ein Vorgang geöffnet! Es kann immer nur EIN Vorgang geöffnet werden. Abbruch!", "Vorgang öffnen", MessageBoxButton.OK, MessageBoxImage.Exclamation)

            Exit Sub
        End If
        glob3.allAktobjReset.execute(myGlobalz.sitzung)

        Dim az As String = ""
        Dim header As String = ""
        Dim alter_titel As String = ""
        Dim alter_probaugAz As String = ""
        Dim altergemKRZ As String = ""
        Dim vorgangsvorschlag As String = LocalParameterFiles.parameterEinlesen(az, header, alter_titel, alter_probaugAz, altergemKRZ)
        myGlobalz.sitzung.aktVorgang.Stammdaten.Beschreibung = alter_titel
        myGlobalz.sitzung.aktVorgang.Stammdaten.Probaugaz = alter_probaugAz
        myGlobalz.sitzung.aktVorgang.Stammdaten.GemKRZ = altergemKRZ

        Dim nnn As New Win_Stamm("neu", myGlobalz.sitzung.aktVorgang.Stammdaten, alter_titel, altergemKRZ, alter_probaugAz)
        nnn.ShowDialog()
        If nnn.DialogResult.HasValue Then
            If nnn.DialogResult.Value Then
                glob2.editVorgang(CInt(myGlobalz.sitzung.aktVorgangsID))
            End If
        End If
        GC.Collect()
    End Sub

    Shared Sub suchenNachVorgaengen(ByRef az As String, ByRef header As String)
        vorgangsvorschlag = LocalParameterFiles.parameterEinlesen(az, header, "", "", "")
        Dim vorgangsid As String = Microsoft.VisualBasic.Interaction.InputBox( _
                         "Bitte geben Sie die VorgangsNr. ein: " & vbCrLf & _
                         "  " & vbCrLf &
                         "Zuletzt bearbeitet: " & vbCrLf & header & vbCrLf & "(" & az & ")", "Vorgang direkt aufrufen:",
                         vorgangsvorschlag)
        If Not String.IsNullOrEmpty(vorgangsid) Then


            '  If String.IsNullOrEmpty(az) And String.IsNullOrEmpty(header) Then
            'If Historydatenrecherchieren(vorgangsid, az, header) Then
            '    nachricht("daten wurden recherchiert")
            'Else
            '    nachricht("daten wurden Nicht recherchiert")
            'End If
            'End If

            '  HistoryKookie.schreibeVerlaufsCookie.exe(CStr(vorgangsid), az, header)
            glob2.editVorgang(CInt(vorgangsid))
            vorgangsvorschlag = LocalParameterFiles.parameterEinlesen(az, header, "", "", "")
        End If
    End Sub

    Public Shared Function Historydatenrecherchieren(ByVal vorgangsid As String, ByRef az As String, ByRef header As String) As Boolean
        Dim hinweis As String
        Try
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "select beschreibung , az2 from stammdaten where vorgangsid=" & vorgangsid
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
        If (clsParadigmaRechte.istUser_admin_oder_vorzimmer()) Then
            'myGlobalz.sitzung.VorgangREC.mydb.SQL = "SELECT * FROM vsk_d " & _
            '                                                               "    where ROWNUM <=  1000 " & _
            '                                                               " order by LetzteBearbeitung desc  ", _
            '                                                               "ALLE-"
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "SELECT * FROM vsk_d " & _
                                                       " where ROWNUM <=  1000 " & _
                                                       " order by LetzteBearbeitung desc  "
        Else
            myGlobalz.sitzung.VorgangREC.mydb.SQL = _
                               String.Format("SELECT * FROM vsk_d " & _
                                               " where      ROWNUM <=  1000 " & _
                                               " and (Bearbeiter='{0}' or Bearbeiter='{1}') " & _
                                               " order by vorgangsid desc ", _
                               myGlobalz.sitzung.aktBearbeiter.Initiale, myGlobalz.sitzung.aktBearbeiter.Kuerzel2Stellig)
        End If
    End Sub

    Public Shared Sub FormularBestandStammdaten(nurzumkuckenmodus As Boolean, Optional modalOeffnen As Boolean = True)
        initBestandsSQL()
        Dim wz As New winBestandStammdaten(False, nurzumkuckenmodus)
        If modalOeffnen Then
            wz.ShowDialog()
        Else
            wz.Show()
        End If
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
        Dim wvtool As New clsWVTOOLS
        wvtool.getWiedervorlageDT(myGlobalz.sitzung.aktBearbeiter.Initiale)
        Dim wz As New WinWvFilter(_nurZumKuckenModus)
        wz.ShowDialog()
    End Sub

    Shared Sub FormularBestandNaturegFilter(_nurZumKuckenModus As Boolean)
        Dim wz As New NaturegFilter(_nurZumKuckenModus)
        wz.ShowDialog()
    End Sub

    Shared Sub ShowSplashScreen()
        'Dim asplish As New SplishSplash
        'asplish.Show()
        'asplish.Close()

    End Sub

End Class
