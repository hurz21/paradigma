Public Class winWebLink
    Property modus As String

 

    Sub New(_modus As String)
        InitializeComponent()
        modus = _modus
    End Sub
    'Private Sub Button_Click(sender As Object, e As RoutedEventArgs)
    '    e.Handled = True
    '    Close()
    'End Sub

    Private Sub BtnAbbruch_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        Close()
    End Sub

    Private Sub BtnSpeichern_Click(sender As Object, e As RoutedEventArgs)
        If istEingabeOk() Then
            weblinkSpeichern(modus)
        End If
        e.Handled = True
        Close()
    End Sub

    Private Sub BtnWeb_Click(sender As Object, e As RoutedEventArgs)
        Process.Start(tbWebLink.Text)
        e.Handled = True
        Close()
    End Sub

    Private Sub winWebLink_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        If modus = "edit" Then
            tbWebLink.Text = myGlobalz.sitzung.aktEreignis.Notiz
            tbWebLinkDesc.Text = myGlobalz.sitzung.aktEreignis.Beschreibung
            Title = "Weblink ändern"
        End If
        If modus = "neu" Then
            tbWebLink.Text = "http://"
            tbWebLinkDesc.Text = "Beschreibung"
            Title = "Weblink erzeugen"
        End If
          gastLayout()
        e.Handled = True
    End Sub

            Private Sub gastLayout()
        Background = myGlobalz.GetSecondBackground()
       
    End Sub

    Private Sub BtnLoeschen_Click(sender As Object, e As RoutedEventArgs)
        If Not glob2.istloeschenErnstgemeint Then Exit Sub
        clsEreignisTools.ereignisLoeschen_alleDB(myGlobalz.sitzung.aktEreignis.ID)
        e.Handled = True
        Me.Close()
    End Sub

    Private Function istEingabeOk() As Boolean
        If tbWebLink.Text.IsNothingOrEmpty Then Return False
        Return True
    End Function

    Private Sub weblinkSpeichern(modus As String)
        If modus = "neu" Then
            myGlobalz.sitzung.aktEreignis.clearValues()
            myGlobalz.sitzung.aktEreignis.Datum = Now
            myGlobalz.sitzung.aktEreignis.Beschreibung = tbWebLinkDesc.Text
            myGlobalz.sitzung.aktEreignis.Notiz = tbWebLink.Text
            myGlobalz.sitzung.aktEreignis.istRTF = False
            myGlobalz.sitzung.aktEreignis.Art = "Weblink"
            myGlobalz.sitzung.aktEreignis.typnr = 4
            myGlobalz.sitzung.aktEreignis.Quelle = myGlobalz.sitzung.aktBearbeiter.Initiale
            'myGlobalz.sitzung.aktEreignis. = myGlobalz.sitzung.aktBearbeiter.Initiale
            If Not clsEreignisTools.NeuesEreignisSpeichern_alleDB(myGlobalz.sitzung.aktVorgangsID, "neu", myGlobalz.sitzung.aktEreignis) Then
                MsgBox("fehler beim speichern des weblink!")
            End If
            nachricht("USERAKTION:   ereignis hinzufügen weblink")
            CLstart.myc.aLog.wer = myGlobalz.sitzung.aktBearbeiter.Initiale
            CLstart.myc.aLog.vorgang = myGlobalz.sitzung.aktVorgangsID.ToString
            CLstart.myc.aLog.komponente = "detail"
            CLstart.myc.aLog.aktion = "verlauf: weblink hinzugefügt"
            CLstart.myc.aLog.log()
        End If
        If modus = "edit" Then
            myGlobalz.sitzung.aktEreignis.Beschreibung = tbWebLinkDesc.Text
            myGlobalz.sitzung.aktEreignis.Notiz = tbWebLink.Text
            myGlobalz.sitzung.aktEreignis.typnr = 4
            If Not clsEreignisTools.EDITobj2DBOk_Ereignis_alledb(myGlobalz.sitzung.aktEreignis.ID, myGlobalz.sitzung.aktVorgangsID,
                                                                 myGlobalz.sitzung.aktEreignis) Then Exit Sub

        End If

    End Sub

End Class
