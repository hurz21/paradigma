Imports System
Namespace glob3
    Class allAktobjReset
        Private Sub new

        End Sub
        Public Shared Sub execute(ByVal sitz As Psession)          'myGlobalz.sitzung
            sitz.aktVorgang.Stammdaten.clear()
            sitz.aktEreignis.clearValues()
            sitz.aktFST.clear()
            sitz.aktADR.clear()
            sitz.aktDokument.clear(CLstart.mycsimple.MeinNULLDatumAlsDate)
            sitz.aktParaFoto.clear()
            sitz.aktPerson.clear()
            sitz.aktZahlung.clear()
            ''folgendes ist neu
            clstart.myc.kartengen.aktMap.clear()
        End Sub
    End Class

End Namespace
