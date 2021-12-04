Public Class winKfa2
    Public Property dokumenteRitemousekeypressed As Boolean
    Public auswahl As New clsKfas
    Public Shared KFAliste As New List(Of clsKfas)
    Property kfaroot As String
    Sub New(_kfaroot As String)
        InitializeComponent()
        kfaroot = _kfaroot
    End Sub

    Private Sub winKfa2_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        e.Handled = True
        kfatools.KFAeingangsDir = kfaroot
        refreshListe(kfatools.KFAeingangsDir)
    End Sub

    Private Sub refreshListe(dirmy As String)
        Dim www As New kfatools
        Dim vorgaenge As List(Of clsKfas) = www.getvorgaenge(dirmy, kfatools.KFAeingangsDir)
        If vorgaenge.Count > 0 Then
            dgKFASuebersicht.DataContext = vorgaenge
            tbPaare.Text = vorgaenge(0).paare
        Else

            dgKFASuebersicht.DataContext = vorgaenge
        End If
        www = Nothing
    End Sub


    Private Sub dgKFASuebersicht_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)
        e.Handled = True

        auswahl = CType(dgKFASuebersicht.SelectedItem, clsKfas)
        If auswahl IsNot Nothing Then
            tbPaare.Text = auswahl.paare
            'Dim www As New winKfaAction(auswahl)
            'www.ShowDialog()
            'www = Nothing
        End If
        btnAuswahlselect.IsEnabled = True
    End Sub

    Private Sub dgKFASuebersicht_MouseRightButtonDown(sender As Object, e As MouseButtonEventArgs)

    End Sub

    Private Sub btnAuswahlselect_Click(sender As Object, e As RoutedEventArgs)
        e.Handled = True
        auswahl = CType(dgKFASuebersicht.SelectedItem, clsKfas)
        Dim info = "Ausgewählt wurde:" & Environment.NewLine &
             auswahl.AS_person.Name & ", " & auswahl.AS_person.Vorname & ", " & auswahl.dateidatum & ", " & Environment.NewLine &
             auswahl.dateikurzname
        'Dim ddd As New winboxKFAS(info)
        'ddd.ShowDialog()
        Dim www As New winKfaAction(auswahl)
        www.ShowDialog()
        Dim auswahltaste = www.keynummer '1-vorgang anlegen
        www = Nothing
        If auswahltaste = "1" Then
            CLstart.mycSimple.neuerVorgang3("modus=kfa transid=" & auswahl.transId)
        End If
        If auswahltaste = "2" Then
            CLstart.mycSimple.neuerVorgang3("modus=kfa transid=" & auswahl.transId)
        End If
        refreshListe(kfatools.KFAeingangsDir)
    End Sub

    'Private Sub TextBlock_MouseDown(sender As Object, e As MouseButtonEventArgs)
    '    e.Handled = True
    '    Dim www As New winKfaAction(auswahl)
    '    www.ShowDialog()
    '    www = Nothing
    'End Sub
    'Private Sub dgKFASuebersicht_MouseDoubleClick(sender As Object, e As MouseButtonEventArgs)
    '    e.Handled = True
    '    auswahl = CType(dgKFASuebersicht.SelectedItem, clsKfas)
    '    Dim info = "Ausgewählt wurde:" & Environment.NewLine &
    '         auswahl.AS_person.Name & ", " & auswahl.AS_person.Vorname & ", " & auswahl.dateidatum & ", " & Environment.NewLine &
    '         auswahl.dateikurzname
    '    Dim ddd As New winboxKFAS(info)
    '    ddd.ShowDialog()
    '    MessageBox.Show(ddd.action)
    '    refreshListe(kfatools.KFAeingangsDir)
    'End Sub
End Class
