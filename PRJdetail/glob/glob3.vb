Imports System
Imports System.Data

Namespace clsTools
    Class tools
        Shared Function korrigiereThumbnail(tnFullname As String) As String
            Dim result As String = ""
            Dim fi As IO.FileInfo
            Try
                If tnFullname.IsNothingOrEmpty Then
                    result = "" ' msg
                Else
                    fi = New IO.FileInfo(tnFullname)
                    If fi.Exists Then
                        result = tnFullname
                    Else
                        result = IO.Path.Combine(initP.getValue("Haupt.ThumbNailsRoot")) & "\tn_fehlt.jpg"
                    End If
                End If
                fi = Nothing
                Return result
            Catch ex As Exception
                nachricht("fehler in korrigiereThumbnail ", ex)
                Return result
            End Try
        End Function
    End Class

    Class allAktobjReset
        Private Sub New()

        End Sub
        Public Shared Sub execute(ByVal sitz As Psession)          'myGlobalz.sitzung
            sitz.aktVorgang.Stammdaten.clear()
            sitz.aktEreignis.clearValues()
            sitz.aktFST.clear()
            sitz.aktADR.clear()
            sitz.aktDokument.clear(CLstart.mycSimple.MeinNULLDatumAlsDate)
            sitz.aktParaFoto.clear()
            sitz.aktPerson.clear()
            sitz.aktZahlung.clear()
            ''folgendes ist neu
            CLstart.myc.kartengen.aktMap.clear()
        End Sub
    End Class

End Namespace
