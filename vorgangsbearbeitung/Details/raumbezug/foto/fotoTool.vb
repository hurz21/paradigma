Imports System.Data

Public Class fotoTool
    Shared Sub loeschenRBFotoBULK(dokumentid As Integer)
        'nachricht("loeschenRBFlurstueckExtracted --------------------------------------------------")
        'Dim erfolg As Integer
        'If CInt(myGlobalz.sitzung.aktParaFoto.Status) = 0 Then
        '    If myGlobalz.sitzung.aktParaFoto.SekID > 0 Then
        '        If dokumentid > 0 Then
        '            erfolg = RBtoolsns.RBFotoLoeschen_alleDB.execute(dokumentid)
        '        End If
        '        nachricht("ARCHIV: RB_FOTO_loeschen:  " & erfolg)
        '        erfolg = RBtoolsns.Raumbezug_loeschen_bySEKid_alleDB.execute(CInt(myGlobalz.sitzung.aktParaFoto.SekID), "5")
        '        nachricht("ARCHIV: RB_loeschenBySekIDTyp:  : " & erfolg)
        '    End If
        '    erfolg = RBtoolsns.Entkoppelung_Raumbezug_Vorgang_alleDB.exe(CInt(myGlobalz.sitzung.aktParaFoto.RaumbezugsID), myGlobalz.sitzung.aktVorgangsID)
        'Else
        '    FST_tools.RB_Und_Vorgang_Entkoppeln(erfolg)
        'End If
    End Sub
End Class
