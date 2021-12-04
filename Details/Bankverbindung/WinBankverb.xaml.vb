Imports System.Data

Public Class WinBankverb
    Property aktbv As New clsBankverbindungSEPA
    Property modus As String = "" ' "edit" 'neu
    Property vorlagemodus As Boolean = False
    Property pid As Integer
    'Property _personenid As Integer
    Sub New(personenid As Integer)

        ' Dieser Aufruf ist für den Designer erforderlich.
        InitializeComponent()

        ' Fügen Sie Initialisierungen nach dem InitializeComponent()-Aufruf hinzu.
        aktbv.personenID = personenid
        pid = personenid
    End Sub
    Private Sub WinBankverb_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        refreshBVliste()
        refreshVorlagenliste()
        'erstenEintragAktivieren()
        ' setzeAbstractBox()
        setzeModus()
        'EditmodusSetzen
        If modus = "neu" Or modus = "" Then
            aktbv.clear()
            aktbv.tss = Now
            aktbv.Quelle = myGlobalz.sitzung.aktBearbeiter.Initiale.Trim
        End If
        If modus = "edit" Then

        End If
        setWinTitle()
        dockMainBV.DataContext = aktbv
    End Sub
    Private Sub btnSpeichern_Click(sender As Object, e As RoutedEventArgs)
        aktbv.IBAN = tbIBAN.Text.Trim
        aktbv.BIC = tbBIC.Text.Trim
        aktbv.BankName = bvTools.getBankname(tbName.Text.Trim)
        aktbv.Titel = bvTools.getTitel(tbTitel.Text.Trim)
        ' aktbv.istVORLAGE1 = CBool((chkistVorlage.IsChecked))
        aktbv.personenID = pid
        If aktbv.istVORLAGE1 Then
            If aktbv.BANKKONTOID < 1 Then
                modus = "neu"
            Else
                modus = "edit"
            End If
        Else
            If aktbv.BANKKONTOID < 1 Then
                modus = "neu"
            Else
                modus = "edit"
            End If
        End If
        aktbv.istVORLAGE1 = False
        If bvTools.eingabeistok(aktbv) Then
            If aktbv.personenID < 1 Then
                MsgBox("Der Bezug zur Person ist verloren (PersoneniD) Major Klopps- bitte Admin benachrichtigen")
                Exit Sub
            End If
            If modus = "neu" Then
                If bvTools.bvNormalspeichernNEU(aktbv) Then
                    refreshBVliste()
                    refreshVorlagenliste()
                    MsgBox("gepeichert")
                Else
                    MsgBox("nicht gepeichert")
                End If
            End If

            If modus = "edit" Then
                If bvTools.bvNORMALspeichernEdit(aktbv) Then
                    refreshBVliste()
                    refreshVorlagenliste()
                    MsgBox("gepeichert")
                Else
                    MsgBox("nicht gepeichert")
                End If
            End If
        End If
        Me.Close()
        e.Handled = True
    End Sub

    Private Sub btnAbbruch_Click(sender As Object, e As RoutedEventArgs)
        Close()
        e.Handled = True
    End Sub

    Private Sub btnLoeschen_Click(sender As Object, e As RoutedEventArgs)
        If bvTools.bvLoeschen(aktbv) > 0 Then
            MsgBox("Ein Obj. wurde gelöscht!")
            refreshBVliste()
            refreshVorlagenliste()
        End If
        e.Handled = True
    End Sub



    Private Sub btnNEU_Click(sender As Object, e As RoutedEventArgs)
        modus = "neu"
        aktbv.clear()
        tbIBAN.Text = ""
        tbAbstract.Text = ""
        aktbv.tss = Now
        aktbv.Quelle = myGlobalz.sitzung.aktBearbeiter.Initiale.Trim
        setWinTitle()
    End Sub

    Private Sub refreshBVliste()
        myGlobalz.sitzung.tempREC2.dt = bvTools.BVsladen(aktbv.personenID)
        dgBVPerson.DataContext = myGlobalz.sitzung.tempREC2.dt
    End Sub


    'Private Sub setzeAbstractBox()
    '    Try
    '        tbAbstract.Text = aktbv.BankName & Environment.NewLine &
    '            aktbv.IBAN & Environment.NewLine &
    '            aktbv.BIC & Environment.NewLine &
    '            aktbv.Titel & Environment.NewLine

    '    Catch ex As Exception

    '    Finally

    '    End Try
    'End Sub

    'Private Sub erstenEintragAktivieren()
    '    Dim i As Integer = 0
    '    If myGlobalz.sitzung.tempREC2.dt.Rows.Count > 0 Then
    '        bvitem2obj2(i, aktbv)

    '    End If
    'End Sub

    Private Sub setzeModus()
        If myGlobalz.sitzung.tempREC2.dt.Rows.Count > 0 Then
            modus = "edit"
        Else
            modus = "neu"
        End If
    End Sub

    Private Sub setWinTitle()
        Title = "Bankverbindungen " & modus
    End Sub

    Private Sub refreshVorlagenliste()
        myGlobalz.sitzung.tempREC.dt = bvTools.BVvorlagenLaden()
        dgVorlagen.DataContext = myGlobalz.sitzung.tempREC.dt
    End Sub



    Private Sub dgBVPerson_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles dgBVPerson.SelectionChanged
        Dim item As DataRowView
        Try
            item = CType(dgBVPerson.SelectedItem, DataRowView)
            If item Is Nothing Then Return
        Catch ex As Exception
            e.Handled = True
            Exit Sub
        End Try
        aktbv = New clsBankverbindungSEPA
        modus = "edit"
        bvTools.bvitem2Obj(item, aktbv)
        dockMainBV.DataContext = aktbv
        setWinTitle()
        dgBVPerson.SelectedItem = Nothing
        e.Handled = True
    End Sub

    Private Sub dgVorlagen_SelectionChanged(sender As Object, e As SelectionChangedEventArgs) Handles dgVorlagen.SelectionChanged
        Dim item As DataRowView
        Try
            item = CType(dgVorlagen.SelectedItem, DataRowView)
            If item Is Nothing Then Return
        Catch ex As Exception
            e.Handled = True
            Exit Sub
        End Try
        aktbv = New clsBankverbindungSEPA
        bvTools.bvitem2Obj(item, aktbv)
        dockMainBV.DataContext = aktbv
        chkistVorlage.IsChecked = True
        modus = "edit"
        vorlagemodus = True
        setWinTitle()
        dgVorlagen.SelectedItem = Nothing
        e.Handled = True
    End Sub

    Private Sub btnSpeichernVorlage_Click(sender As Object, e As RoutedEventArgs)
        aktbv.IBAN = tbIBAN.Text.Trim
        aktbv.BIC = tbBIC.Text.Trim
        aktbv.BankName = bvTools.getBankname(tbName.Text.Trim)
        aktbv.Titel = bvTools.getTitel(tbTitel.Text.Trim)
        aktbv.BANKKONTOID=0
        If aktbv.istVORLAGE1 Then
            If aktbv.BANKKONTOID < 1 Then
                modus = "neu"
            Else
                modus = "edit"
            End If
        Else
            If aktbv.BANKKONTOID < 1 Then
                modus = "neu"
            Else
                modus = "edit"
            End If
        End If
        aktbv.istVORLAGE1 = True

        If bvTools.eingabeistok(aktbv) Then
            If modus = "neu" Then
                If bvTools.bvVorlagespeichernNEU(aktbv) Then
                    refreshBVliste()
                    refreshVorlagenliste()
                    MsgBox("gepeichert")
                Else
                    MsgBox("nicht gepeichert")
                End If
            End If

            If modus = "edit" Then
                If bvTools.bvVORLAGEspeichernEdit(aktbv) Then
                    refreshBVliste()
                    refreshVorlagenliste()
                    MsgBox("gepeichert")
                Else
                    MsgBox("nicht gepeichert")
                End If
            End If
        End If
        Me.Close()
        e.Handled = True
    End Sub
End Class
