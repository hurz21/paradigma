Public Class clsParadigmaRechte
    Shared Function istUser_admin_oder_vorzimmer() As Boolean
        If myGlobalz.sitzung.aktBearbeiter.Rang.ToLower = "admin" Or _
         myGlobalz.sitzung.aktBearbeiter.Rang.ToLower = "vorzimmer" Then
            nachricht("Vorzimmer Zugriff erteilt für user: " & myGlobalz.sitzung.aktBearbeiter.Initiale)
            Return True
        Else
            nachricht("Vorzimmer Zugriff NICHT erteilt für user: " & myGlobalz.sitzung.aktBearbeiter.Initiale)
            Return False
        End If
    End Function

    Private Shared Function binEignerExtracted() As Boolean
        Dim binEigner As Boolean
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale) Then
            Return True ' der fall kommt vor!
        End If
        binEigner = (myGlobalz.sitzung.aktBearbeiter.Initiale.ToLower = myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale.ToLower) Or _
                    (myGlobalz.sitzung.aktBearbeiter.Kuerzel2Stellig.ToLower = myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale.ToLower)
        Return binEigner
    End Function


    Private Shared Function enthaelt(ByVal Text$, ByVal Teil As String) As Boolean
        If String.IsNullOrEmpty(Text) Then Return False
        If String.IsNullOrEmpty(Teil$) Then Return False
        If Text.Contains(Teil) Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Shared Function binWeitererBearbeiter() As Boolean
        Dim binWeiterer As Boolean
        binWeiterer = enthaelt(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale.ToLower, myGlobalz.sitzung.aktBearbeiter.Initiale.ToLower) Or
                    enthaelt(myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter.ToLower, myGlobalz.sitzung.aktBearbeiter.Kuerzel2Stellig.ToLower) Or
                    enthaelt(myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter.ToLower, myGlobalz.sitzung.aktBearbeiter.Initiale.ToLower)
        Return binWeiterer
    End Function

    Shared Function binEignerOderAdmin() As Boolean
        If binEignerExtracted() Or binWeitererBearbeiter() Then
            nachricht("Zugriff erteilt für User: " & myGlobalz.sitzung.aktBearbeiter.Initiale)
            Return True
        Else
            Return istUser_admin_oder_vorzimmer()
        End If
    End Function

    Public Overloads Shared Sub buttons_schalten(ByVal btnSave As Button, ByVal btnLoeschs As Button)
        If binEignerOderAdmin() Then
            'btnSave.IsEnabled = True
            'btnLoeschs.IsEnabled = True
            btnSave.Visibility = Windows.Visibility.Visible
            btnLoeschs.Visibility = Windows.Visibility.Visible
        Else
            btnSave.Visibility = Windows.Visibility.Hidden
            btnLoeschs.Visibility = Windows.Visibility.Hidden
            'btnSave.IsEnabled = False 
            'btnLoeschs.IsEnabled = False
           
            nachricht("Sie sind nicht autorisiert die Daten zu ändern.")
        End If
    End Sub

    'Public Overloads Shared Sub buttons_schalten_alt(ByVal btnSave As Button, ByVal btnLoeschs As Button)
    '    If binEignerOderAdmin() Then
    '        btnSave.Visibility = Windows.Visibility.Visible
    '        btnLoeschs.Visibility = Windows.Visibility.Visible
    '    Else
    '        btnSave.Visibility = Windows.Visibility.Hidden
    '        btnLoeschs.Visibility = Windows.Visibility.Hidden
    '        nachricht("Sie sind nicht autorisiert die Daten zu ändern.")
    '    End If
    'End Sub


    Public Overloads Shared Sub buttons_schalten(ByVal btnSave As Button)
        If binEignerOderAdmin() Or binWeitererBearbeiter() Then
            btnSave.Visibility = Windows.Visibility.Visible
            '  btnSave.IsEnabled = True
        Else
            btnSave.Visibility = Windows.Visibility.Hidden
            'btnSave.IsEnabled = False
            nachricht("Sie sind nicht autorisiert die Daten zu ändern.")
        End If
    End Sub
End Class
