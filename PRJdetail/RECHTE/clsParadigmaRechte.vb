
Public Class clsParadigmaRechte
    'Shared Function istUser_admin_oder_vorzimmer(user As clsBearbeiter) As Boolean '
    '    If user.Rang.ToLower = "admin" Or
    '       user.Rang.ToLower = "vorzimmer" Then
    '        nachricht("Vorzimmer Zugriff erteilt für user: " & user.Initiale)
    '        Return True
    '    Else
    '        nachricht("Vorzimmer Zugriff NICHT erteilt für user: " & user.Initiale)
    '        Return False
    '    End If
    'End Function


    'Private Shared Function binWeitererBearbeiter() As Boolean
    '    Dim binWeiterer As Boolean
    '    binWeiterer = LIBgemeinsames.clsString.textEnthaelt(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale.ToLower, myGlobalz.sitzung.aktBearbeiter.Initiale.ToLower) Or
    '                LIBgemeinsames.clsString.textEnthaelt(myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter.ToLower, myGlobalz.sitzung.aktBearbeiter.Kuerzel2Stellig.ToLower) Or
    '                LIBgemeinsames.clsString.textEnthaelt(myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter.ToLower, myGlobalz.sitzung.aktBearbeiter.Initiale.ToLower)
    '    Return binWeiterer
    'End Function

    'Shared Function binEignerOderAdmin() As Boolean
    '    If binEignerExtracted() Or binWeitererBearbeiter() Then
    '        nachricht("Zugriff erteilt für User: " & myGlobalz.sitzung.aktBearbeiter.Initiale)
    '        Return True
    '    Else
    '        Return myGlobalz.sitzung.aktBearbeiter.istUser_admin_oder_vorzimmer
    '    End If
    'End Function

    Public Overloads Shared Sub buttons_schalten(ByVal btnSave As Button, ByVal btnLoeschs As Button)
        If myGlobalz.sitzung.aktBearbeiter.binEignerOderAdmin(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter, myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter) Then
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
        If myGlobalz.sitzung.aktBearbeiter.binEignerOderAdmin(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter, myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter) Or
            myGlobalz.sitzung.aktBearbeiter.binWeitererBearbeiter(myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter, myGlobalz.sitzung.aktVorgang.Stammdaten.WeitereBearbeiter) Then
            btnSave.Visibility = Windows.Visibility.Visible
            '  btnSave.IsEnabled = True
        Else
            btnSave.Visibility = Windows.Visibility.Hidden
            'btnSave.IsEnabled = False
            nachricht("Sie sind nicht autorisiert die Daten zu ändern.")
        End If
    End Sub
End Class
