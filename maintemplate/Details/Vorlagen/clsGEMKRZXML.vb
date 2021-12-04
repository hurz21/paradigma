Public Class clsGEMKRZXML
    Public Shared Function LoadVariablenGemarkung(ByVal suchstring As String) As String
        ' result = clsGEMKRZXML.LoadVariablenGemarkung(GEMKRZ)
        Try
            Dim result As String = ""
            Dim pfad As String = IO.Path.Combine(myGlobalz.appdataDir, "config\Combos\GemKRZn.xml")
            nachricht("GemKRZn.xml pfad=" & pfad)
            Dim doc = XDocument.Load(pfad)
            Dim query = From cust In doc...<GEMKRZ>
                      Select New GEMKRZ With {.Name = cust.@Name, .ID = cust.@ID, .GEME = cust.@GEME, .GEMA = cust.@GEMA}
            For Each cust In query
                If cust.ID = suchstring Then
                    Return cust.GEMA
                End If
            Next
            Return result
        Catch ex As Exception
            MsgBox(ex.ToString)
            Return Nothing
        End Try
    End Function

    Public Shared Function LoadVariablenGemeinde(ByVal suchstring As String) As String
        Try
            Dim result As String = ""
            Dim pfad As String = IO.Path.Combine(myGlobalz.appdataDir, "config\Combos\GemKRZn.xml")
            nachricht("GemKRZn.xml pfad=" & pfad)
            Dim doc = XDocument.Load(pfad)
            Dim query = From cust In doc...<GEMKRZ>
                      Select New GEMKRZ With {.Name = cust.@Name, .ID = cust.@ID, .GEME = cust.@GEME, .GEMA = cust.@GEMA}
            For Each cust In query
                If cust.ID = suchstring Then
                    Return cust.GEME
                End If
            Next
            Return result
        Catch ex As Exception
            MsgBox(ex.ToString)
            Return Nothing
        End Try
    End Function
End Class

Public Class GEMKRZ
    Public Property Name As String
    Public Property ID As String
    Public Property GEME As String
    Public Property GEMA As String
End Class

