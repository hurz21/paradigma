Public Class clsGEMKRZXML
    Public Shared Function LoadVariablenGemarkung(ByVal suchstring As String) As String
        Try
            Dim result As String = "" 
            Dim pfad As String = IO.Path.Combine("c:\", "GemKRZn.xml")

            Dim doc = XDocument.Load(pfad)
            Dim query = From cust In doc...<GEMKRZ>
                      Select New GEMKRZ With {.Name = cust.@Name, .ID = cust.@ID, .GEME = cust.@GEME, .GEMA = cust.@GEMA}
            For Each cust In query
                If cust.GEMA = suchstring Then
                    Return cust.ID
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
            Dim pfad As String = IO.Path.Combine("c:\", "GemKRZn.xml")

            Dim doc = XDocument.Load(pfad)
            Dim query = From cust In doc...<GEMKRZ>
                      Select New GEMKRZ With {.Name = cust.@Name, .ID = cust.@ID, .GEME = cust.@GEME, .GEMA = cust.@GEMA}
            For Each cust In query
                If cust.GEME = suchstring Then
                    Return cust.ID
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

