
Imports System.Data

Namespace nsDivers
    Public Class clsdivers
        Shared Sub externerFotogugger(vid As String, eid As Integer, docid As Integer, nurVerwandteFotos As Integer)
            Dim si As New ProcessStartInfo
            Try
                l(" MOD externerFotogugger anfang")
                'si.FileName = initP.getValue("ExterneAnwendungen.Application_Stakeholder")
                si.FileName = "C:\kreisoffenbach\prjfotogugger\fotogugger.exe "
                si.WorkingDirectory = "C:\kreisoffenbach\prjfotogugger"
                si.Arguments = " /vid=" & vid & " /eid=" & eid & " /docid=" & docid & " /nurverwandte=" & nurVerwandteFotos
                Process.Start(si)
                si = Nothing
                l(" MOD externerFotogugger ende")
            Catch ex As Exception
                l("Fehler in externerFotogugger: " & ex.ToString())
            End Try
        End Sub
        Friend Shared Function getAlleVerwandtennummern(verwandteDTServer As DataTable, verwandteDT As DataTable, aktvorgangsid As Integer) As String
            Dim summe As String = ""
            Try
                For i = 0 To verwandteDTServer.Rows.Count - 1
                    If verwandteDTServer.Rows(i).Item("VORGANGSID").ToString = aktvorgangsid.ToString Then
                        summe = verwandteDTServer.Rows(i).Item("FREMDVORGANGSID").ToString & "," & summe
                    Else
                        summe = verwandteDTServer.Rows(i).Item("VORGANGSID").ToString & "," & summe
                    End If
                Next
                summe = summe.Trim(CChar(","))
                For i = 0 To verwandteDT.Rows.Count - 1
                    If verwandteDT.Rows(i).Item("VORGANGSID").ToString = aktvorgangsid.ToString Then
                        summe = verwandteDT.Rows(i).Item("FREMDVORGANGSID").ToString & "," & summe
                    Else
                        summe = verwandteDT.Rows(i).Item("VORGANGSID").ToString & "," & summe
                    End If
                Next
                summe = summe.Trim(CChar(","))
                summe = LIBgemeinsames.clsString.nodoubleStrings(summe, CChar(","))
                summe = summe.Trim(CChar(","))
                Return summe
            Catch ex As Exception
                nachricht("fehler in getAlleVerwandtennummern " & ex.ToString)
                Return ""
            End Try
        End Function
        Shared Sub nachricht(text As String)
            My.Log.WriteEntry(text)
        End Sub
    End Class
End Namespace

