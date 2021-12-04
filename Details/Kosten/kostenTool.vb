Imports System.Data

Module kostenTool

    Private Function eintrag_existiert_schon(ByVal clsKosten As clsKosten) As Boolean
        Return clsKosten.id > 0
    End Function
    Function speichern(clsKosten As clsKosten, clsBearbeiter As String, vorgangsid As Integer) As Boolean
        clsKosten.QUELLE = clsBearbeiter
        clsKosten.vorgangsid = vorgangsid


        ' Dim newid As Integer
        If eintrag_existiert_schon(clsKosten) Then
            'eintrag_existiert_schon
            Dim anz As Integer = kostenAbspeichernEdit_AlleDB(clsKosten)
            If anz < 1 Then
                nachricht("Fehler Fehler beim Abspeichern der Kosten: " & myGlobalz.sitzung.aktVorgangsID)
                MsgBox("Fehler beim Abspeichern der Kosten. Der Admin wird informiert")
            End If
        Else
            clsKosten.id = kostenAbspeichernNeu_AlleDB(clsKosten)

        End If
        Return True
    End Function
    Public Function kostenAbspeichernNeu_AlleDB(ByVal lnat As clsKosten) As Integer
        Dim natid%

        If (myGlobalz.beteiligte_MYDB.dbtyp = "oracle") Then
            Dim zzz As New kostenORACLE(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.beteiligte_MYDB))
            natid = zzz.kosten_abspeichern_Neu(lnat)
                zzz.dispose
        End If
        Return natid
    End Function

    Public Function kostenAbspeichernEdit_AlleDB(ByVal lnat As clsKosten) As Integer
        Dim anzahl As Integer
        If (myGlobalz.beteiligte_MYDB.dbtyp = "oracle") Then
            Dim zzz As New kostenORACLE(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.beteiligte_MYDB))
            anzahl = zzz.Kosten_abspeichern_EditExtracted(lnat)
                zzz.dispose
        End If
        Return anzahl
    End Function

    Private Sub AufObjUmschichten(ByVal kostendt As DataTable, kosten As clsKosten)
        'auf obj umschichten
        Try
            With kosten
                .InterneZahlung = CBool(kostendt.Rows(0).Item("INTERNEZAHLUNG"))
                .verwaltungsgebuehr = CBool(kostendt.Rows(0).Item("VERWALTUNGSGEBUEHR"))
                .verwaltungsgebuehrBezahlt = CBool(kostendt.Rows(0).Item("VERWALTUNGSGEBUEHR_BEZAHLT"))
                .ersatzgeld = CBool(kostendt.Rows(0).Item("ERSATZGELD"))
                .ersatzgeldBezahlt = CBool(kostendt.Rows(0).Item("ERSATZGELD_BEZAHLT"))
                .sicherheit = CBool(kostendt.Rows(0).Item("SICHERHEIT"))
                .sicherheitBezahlt = CBool(kostendt.Rows(0).Item("SICHERHEIT_BEZAHLT"))

                  .ersatzgeldAUSGEzahlt = CBool(kostendt.Rows(0).Item("ERSATZGELD_AUSGEZAHLT"))
                  .ZWANGSGELD = CBool(kostendt.Rows(0).Item("ZWANGSGELD"))
                  .ZWANGSGELDBezahlt = CBool(kostendt.Rows(0).Item("ZWANGSGELD_BEZAHLT"))
                  .BEIHILFE = CBool(kostendt.Rows(0).Item("BEIHILFE"))
                  .BEIHILFEBezahlt = CBool(kostendt.Rows(0).Item("BEIHILFE_BEZAHLT"))

                .QUELLE = CStr(kostendt.Rows(0).Item("QUELLE"))
                .timestamp = CDate(kostendt.Rows(0).Item("TS"))
                .id = CInt(kostendt.Rows(0).Item("ID"))
            End With
        Catch ex As Exception
            nachricht("fehler in AufObjUmschichten: " & ex.ToString)
        End Try
    End Sub

    Function getKostenOjbFromDb(vid As Integer) As Boolean
        Dim kostendt As New DataTable
        Try
            kostendt = kostenORACLE.getkostenDatatable(myGlobalz.sitzung.aktVorgangsID)
            If kostendt.IsNothingOrEmpty Then
                Return False
            Else
                AufObjUmschichten(kostendt, myGlobalz.sitzung.aktVorgang.KostenStatus)
                Return True
            End If
        Catch ex As Exception
            nachricht("FEHLER in getKostenOjbFromDb: " & ex.ToString)
            Return False
        End Try
    End Function

End Module
