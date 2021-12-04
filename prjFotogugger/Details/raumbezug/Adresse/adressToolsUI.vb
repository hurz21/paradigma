Public Class adressToolsUI
    Private Shared ReadOnly Property Radius() As Double
        Get
            Return CDbl(initP.getValue("MiniMap.radiusAdresse"))
        End Get
    End Property

    Public Shared Sub Adresse_speichern(ByVal formchen As System.Windows.Window,
                                        ByVal btnspeichern As Button,
                                        ByVal tbgemeinde As TextBox,
                                        ByVal tbStrasse As TextBox,
                                        ByVal tbhausnr As TextBox,
                                        ByVal tbkurzbeschreibung As String,
                                        ByVal tbfreitext As String,
                                        ismapenabled As Boolean)
        If CInt(myGlobalz.sitzung.aktADR.Status) = 1 Then
            MsgBox("Es handelt sich um die Adresse eines Verwandte Vorgangs. Sie kann nicht geändert werden!")
            Exit Sub
        End If
        If Not istEingabe_vorhanden(tbgemeinde, tbStrasse, tbhausnr) Then Exit Sub
        AdressTools.koordinatenErgaenzen(myGlobalz.sitzung.aktADR, ismapenabled)
        Adresse_imKreisOffenbach(tbgemeinde)
        myGlobalz.sitzung.aktVorgang.Stammdaten.hatraumbezug = True
        detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "HATRAUMBEZUG")

        myGlobalz.sitzung.akt_raumbezugsTyp = RaumbezugsTyp.Adresse

        myGlobalz.sitzung.aktADR.Name = tbkurzbeschreibung
        myGlobalz.sitzung.aktADR.Freitext = tbfreitext
        myGlobalz.sitzung.aktADR.Typ = RaumbezugsTyp.Adresse
        myGlobalz.sitzung.aktADR.isMapEnabled = ismapenabled


        If myGlobalz.sitzung.raumbezugsmodus = "neu" Then
            glob2.Adresse_Neu(Radius)
            btnspeichern.IsEnabled = False

            myGlobalz.sitzung.aktVorgang.Stammdaten.hatraumbezug = True
            detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "HATRAUMBEZUG")
            AdressTools.setzeeNeuesGemKRZ(myGlobalz.sitzung)
            'glob2.EDIT_VorgangStamm_2DBOk()
            formchen.Close()
        End If
        If myGlobalz.sitzung.raumbezugsmodus = "edit" Then
            Adresse_Edit(CInt(myGlobalz.sitzung.aktADR.SekID), btnspeichern, formchen)
        End If

        myGlobalz.sitzung.aktVorgang.Stammdaten.hatraumbezug = True
        detailsTools.Edit_singleUpdate_Stammdaten(myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung, "HATRAUMBEZUG")
        AdressTools.setzeeNeuesGemKRZ(myGlobalz.sitzung)
        nachricht("hatraumbezug sepeicher in stammdaten ")
        '   glob2.EDIT_VorgangStamm_2DBOk()
        'If AdressTools.setzeeNeuesGemKRZ(myGlobalz.sitzung) Then
        '    glob2.EDIT_VorgangStamm_2DBOk()
        'End If
    End Sub

    Public Shared Function istEingabe_vorhanden(ByVal tbGemeinde As TextBox,
                                                ByVal tbStrasse As TextBox,
                                                ByVal tbHausnr As TextBox
                                                ) As Boolean
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName) Then
            If String.IsNullOrEmpty(tbGemeinde.Text) Then
                nachricht_und_Mbox("Sie müssen eine Gemeinde angeben.")
                Return False
            Else
                myGlobalz.sitzung.aktADR.Gisadresse.gemeindeName = tbGemeinde.Text.Trim
            End If
        End If

        If String.IsNullOrEmpty(myGlobalz.sitzung.aktADR.Gisadresse.strasseName) Then
            If String.IsNullOrEmpty(tbStrasse.Text) Then
                nachricht_und_Mbox("Sie müssen eine Strasse angeben.")
                Return False
            Else
                myGlobalz.sitzung.aktADR.Gisadresse.strasseName = tbStrasse.Text.Trim
            End If
        End If

        If String.IsNullOrEmpty(myGlobalz.sitzung.aktADR.Gisadresse.HausKombi) Then
            If String.IsNullOrEmpty(tbHausnr.Text) Then
                nachricht_und_Mbox("Sie sollten eine Hausnummer angeben.")
                myGlobalz.sitzung.aktADR.Gisadresse.HausKombi = "0"
                Return False
            Else
                myGlobalz.sitzung.aktADR.Gisadresse.HausKombi = tbHausnr.Text.Trim
            End If
        End If

        Return True
    End Function

    Public Shared Sub Adresse_imKreisOffenbach(ByVal tbGemeinde As TextBox)
        Using neuadr As New clsAdress(tbGemeinde.Text)
            If clsGemarkungsParams.liegtGemeindeImKreisOffenbach(tbGemeinde.Text) Then
                Dim test$ = neuadr.gemparms.gemeindetext2gemeindenr(tbGemeinde.Text)
                If test$.ToLower = "error" Then
                    MessageBox.Show("Die Gemeinde: " & tbGemeinde.Text & " wird vermutlich anders geschrieben. " & vbCrLf & _
                     "Sie wird trotzdem gespeichert!", "Adresse speichern", MessageBoxButton.OK, MessageBoxImage.Exclamation)
                End If

            Else
                MessageBox.Show("Die Gemeinde: " & tbGemeinde.Text & " liegt nicht im Kreis Offenbach. " & vbCrLf & _
                 "Sie wird trotzdem gespeichert!", "Adresse speichern", MessageBoxButton.OK, MessageBoxImage.Exclamation)
            End If
        End Using
    End Sub


    Public Shared Sub Adresse_Edit(ByVal sekid%, ByVal btnSpeichern As Button, ByVal formchen As System.Windows.Window)
        myGlobalz.sitzung.aktADR.Gisadresse.hauskombiZerlegen()
        'aenderung vorhanden ? vergleichen aktadr mit tbn
        'todo änderung beim raumbezug muss auch in die datenbank
        Dim erfolg As Integer = RBtoolsns.AdresseEdit_alleDB.execute(sekid)
        nachricht("Adresse_Edit " & erfolg)
        If erfolg > 0 Then
            btnSpeichern.IsEnabled = False
            myGlobalz.sitzung.aktADR.defineAbstract()
            DBraumbezug_Mysql.defineBBOX(Radius, myGlobalz.sitzung.aktADR)
            RBtoolsns.Raumbezug_edit_alleDB.execute(CInt(myGlobalz.sitzung.aktADR.RaumbezugsID), myGlobalz.sitzung.aktADR)
            '      DBraumbezug_Mysql.Raumbezug_abspeichern_Edit(CInt(myGlobalz.sitzung.aktADR.RaumbezugsID), myGlobalz.sitzung.aktADR)
            formchen.Close()
        Else
            nachricht("Problem beim Abspeichern!Adresse_Edit: " & erfolg)
        End If
    End Sub

    Public Shared Sub windRoseAnzeigen(ByVal x As Double, ByVal y As Double)
        If x < 1 Or y < 1 Then
            MsgBox("Sie haben keine gültige Koordinate. Somit ist es nicht möglich eine Windrose zu bekommen!")
            Exit Sub
        Else
            Dim windrosenHyperlink$ = clsWindrose.GetWindrosenHyperlink(x, y)
            Process.Start(windrosenHyperlink$)
            MessageBox.Show(glob2.getMsgboxText("windrose", New List(Of String)(New String() {})), "Empfehlung", MessageBoxButton.OK, MessageBoxImage.Information)

        End If
    End Sub

End Class
