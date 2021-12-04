Public Class clsIllegbauTools
       Public shared   Function statusIndexNachText(statusindex As String) As String
        Dim temptext As String
         Select Case statusindex
            Case "0"
                temptext = ""
            Case "1"
                temptext = "planmäßig"
            Case "2"
                temptext = "laufend"
            Case "3"
                temptext = "erledigt"
            Case "4"
                temptext = "recherche"
            Case Else
                temptext = ""
        End Select
        Return temptext
    End Function

    Public  shared   Function TextNachStatusIndex(text As String) As String
        Dim temptext As String
        Select Case text
            Case ""
                temptext = "0"
            Case "planmäßig"
                temptext = "1"
            Case "laufend"
                temptext = "2"
            Case "erledigt"
                temptext = "3"
            Case "recherche"
                temptext = "4"
            Case Else
                temptext = "0"
        End Select
        Return temptext
    End Function

    Public  shared   Function gebietsIndexNachText(gebietsindex As String) As String
        Dim temptext As String
        Select Case gebietsindex
            Case "0"
                temptext = ""
            Case "1"
                temptext = "Außenbereich"
            Case "2"
                temptext = "LSG Kreis Offenbach"
            Case "3"
                temptext = "LSG Hess. Mainauen"
            Case "4"
                temptext = "LSG Zellerbruch"
            Case "5"
                temptext = "NSG"
            Case Else
                temptext = ""
        End Select
        Return temptext
    End Function
    Public  shared   Function TextNachGebietsIndex(text As String) As String
        Dim temptext As String
        Select Case text
            Case ""
                temptext = "0"
            Case "Außenbereich"
                temptext = "1"
            Case "LSG Kreis Offenbach"
                temptext = "2"
            Case "LSG Hess. Mainauen"
                temptext = "3"
            Case "LSG Zellerbruch"
                temptext = "4"
            Case "NSG"
                temptext = "5"
            Case Else
                temptext = ""
        End Select
        Return temptext
    End Function
    Public shared    Function RaeumungsTypIndexNachText(ri As String) As String
        '0="",1=freiwillig,2=Abräumvertrag,3=Rechtsstreit
        Dim temptext As String
        Select Case ri
            Case "0"
                temptext = ""
            Case "1"
                temptext = "freiwillig"
            Case "2"
                temptext = "Abräumvertrag"
            Case "3"
                temptext = "Rechtsstreit"
            Case Else
                temptext = ""
        End Select
        Return temptext
    End Function

    Public  shared   Function TextNachRaeumungsTypIndex(text As String) As String
        Dim temptext As String
        Select Case text
            Case "", "0", "-"
                temptext = "0"
            Case "freiwillig"
                temptext = "1"
            Case "Abräumvertrag"
                temptext = "2"
            Case "Rechtsstreit"
                temptext = "3"
            Case Else
                temptext = "0"
        End Select
        Return temptext
    End Function

    shared    Function     getRaeumungstitel(raeumungsZiffer As String) As String
        Select Case raeumungsZiffer
            Case "1"
                Return "Freiwillige Räumung"
            Case "2"
                Return "Räumung mit Abräumvertrag"
            Case "3"
                Return "Rechtsstreit"
            Case Else
                Return "?"
        End Select
    End Function
End Class
