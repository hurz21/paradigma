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
        Dim querie As String
        clsSqlparam.paramListe.Clear()
        ' populateParamListeEreignis(zielvorgangsid, ereignis, clsSqlparam.paramListe)
        'clsSqlparam.paramListe.Add(New clsSqlparam("eid", 0))
        querie = "INSERT INTO  " & CLstart.myViewsNTabs.tabKosten & "  (VORGANGSID,INTERNEZAHLUNG,VERWALTUNGSGEBUEHR,VERWALTUNGSGEBUEHR_BEZAHLT," &
                                "ERSATZGELD,ERSATZGELD_BEZAHLT,SICHERHEIT,SICHERHEIT_BEZAHLT,QUELLE," &
                                "VERWARNUNGSGELD,VERWARNUNGSGELD_BEZAHLT,BUSSGELD,BUSSGELD_BEZAHLT," &
                                "ERSATZGELD_AUSGEZAHLT,ZWANGSGELD,ZWANGSGELD_BEZAHLT,BEIHILFE,BEIHILFE_BEZAHLT) " +
                               " VALUES (@VORGANGSID,@INTERNEZAHLUNG,@VERWALTUNGSGEBUEHR,@VERWALTUNGSGEBUEHR_BEZAHLT," &
                                "@ERSATZGELD,@ERSATZGELD_BEZAHLT,@SICHERHEIT,@SICHERHEIT_BEZAHLT,@QUELLE," &
                                "@VERWARNUNGSGELD,@VERWARNUNGSGELD_BEZAHLT,@BUSSGELD,@BUSSGELD_BEZAHLT," &
                                "@ERSATZGELD_AUSGEZAHLT,@ZWANGSGELD,@ZWANGSGELD_BEZAHLT,@BEIHILFE,@BEIHILFE_BEZAHLT)"
        populateKosten(lnat)
        natid = myglobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "ID")
        Return natid
    End Function

    Private Sub populateKosten(lnat As clsKosten)
        With lnat
            clsSqlparam.paramListe.Add(New clsSqlparam("VORGANGSID", .vorgangsid))
            clsSqlparam.paramListe.Add(New clsSqlparam("INTERNEZAHLUNG", (.InterneZahlung)))
            clsSqlparam.paramListe.Add(New clsSqlparam("VERWALTUNGSGEBUEHR", (.verwaltungsgebuehr)))
            clsSqlparam.paramListe.Add(New clsSqlparam("VERWALTUNGSGEBUEHR_BEZAHLT", (.verwaltungsgebuehrBezahlt)))
            clsSqlparam.paramListe.Add(New clsSqlparam("ERSATZGELD", (.ersatzgeld)))
            clsSqlparam.paramListe.Add(New clsSqlparam("ERSATZGELD_BEZAHLT", (.ersatzgeldBezahlt)))
            clsSqlparam.paramListe.Add(New clsSqlparam("SICHERHEIT", (.sicherheit)))
            clsSqlparam.paramListe.Add(New clsSqlparam("SICHERHEIT_BEZAHLT", (.sicherheitBezahlt)))

            clsSqlparam.paramListe.Add(New clsSqlparam("VERWARNUNGSGELD", (.VERWARNUNGSGELD)))
            clsSqlparam.paramListe.Add(New clsSqlparam("VERWARNUNGSGELD_BEZAHLT", (.VerwarnungsgeldBezahlt)))
            clsSqlparam.paramListe.Add(New clsSqlparam("BUSSGELD", (.BUSSGELD)))
            clsSqlparam.paramListe.Add(New clsSqlparam("BUSSGELD_BEZAHLT", (.BUSSGELDBezahlt)))

            clsSqlparam.paramListe.Add(New clsSqlparam("ERSATZGELD_AUSGEZAHLT", (.ersatzgeldAUSGEzahlt)))
            clsSqlparam.paramListe.Add(New clsSqlparam("ZWANGSGELD", (.ZWANGSGELD)))
            clsSqlparam.paramListe.Add(New clsSqlparam("ZWANGSGELD_BEZAHLT", (.ZWANGSGELDBezahlt)))
            clsSqlparam.paramListe.Add(New clsSqlparam("BEIHILFE", (.BEIHILFE)))
            clsSqlparam.paramListe.Add(New clsSqlparam("BEIHILFE_BEZAHLT", (.BEIHILFEBezahlt)))

            clsSqlparam.paramListe.Add(New clsSqlparam("QUELLE", .QUELLE.Trim))
        End With
    End Sub

    Public Function kostenAbspeichernEdit_AlleDB(ByVal lnat As clsKosten) As Integer
        Dim anzahl As Integer
        Dim querie As String
        clsSqlparam.paramListe.Clear()
        ' populateParamListeEreignis(zielvorgangsid, ereignis, clsSqlparam.paramListe)
        'clsSqlparam.paramListe.Add(New clsSqlparam("eid", 0))
        querie = "UPDATE   " & CLstart.myViewsNTabs.tabKosten & "  " & " SET VORGANGSID=@VORGANGSID" &
                    ",INTERNEZAHLUNG=@INTERNEZAHLUNG" &
                    ",VERWALTUNGSGEBUEHR=@VERWALTUNGSGEBUEHR" &
                    ",VERWALTUNGSGEBUEHR_BEZAHLT=@VERWALTUNGSGEBUEHR_BEZAHLT" &
                    ",ERSATZGELD=@ERSATZGELD" &
                    ",ERSATZGELD_BEZAHLT=@ERSATZGELD_BEZAHLT" &
                    ",SICHERHEIT=@SICHERHEIT" &
                    ",SICHERHEIT_BEZAHLT=@SICHERHEIT_BEZAHLT" &
                    ",VERWARNUNGSGELD=@VERWARNUNGSGELD" &
                    ",VERWARNUNGSGELD_BEZAHLT=@VERWARNUNGSGELD_BEZAHLT" &
                    ",BUSSGELD=@BUSSGELD" &
                    ",BUSSGELD_BEZAHLT=@BUSSGELD_BEZAHLT" &
                    ",ERSATZGELD_AUSGEZAHLT=@ERSATZGELD_AUSGEZAHLT" &
                    ",ZWANGSGELD=@ZWANGSGELD" &
                    ",ZWANGSGELD_BEZAHLT=@ZWANGSGELD_BEZAHLT" &
                    ",BEIHILFE=@BEIHILFE" &
                    ",BEIHILFE_BEZAHLT=@BEIHILFE_BEZAHLT" &
                    ",QUELLE=@QUELLE " &
                    " WHERE ID=@ID"
        populateKosten(lnat)
        clsSqlparam.paramListe.Add(New clsSqlparam("ID", lnat.id))
        anzahl = myglobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, False, "ID")
        If anzahl < 1 Then
            nachricht_und_Mbox("Problem beim Abspeichern:" & myglobalz.sitzung.VorgangREC.mydb.SQL)
            Return -1
        Else
            Return CInt(anzahl)
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

                .VERWARNUNGSGELD = CBool(kostendt.Rows(0).Item("VERWARNUNGSGELD"))
                .VerwarnungsgeldBezahlt = CBool(kostendt.Rows(0).Item("VERWARNUNGSGELD_BEZAHLT"))

                .BUSSGELD = CBool(kostendt.Rows(0).Item("BUSSGELD"))
                .BUSSGELDBezahlt = CBool(kostendt.Rows(0).Item("BUSSGELD_BEZAHLT"))

                .ersatzgeldAUSGEzahlt = CBool(kostendt.Rows(0).Item("ERSATZGELD_AUSGEZAHLT"))
                .ZWANGSGELD = CBool(kostendt.Rows(0).Item("ZWANGSGELD"))
                .ZWANGSGELDBezahlt = CBool(kostendt.Rows(0).Item("ZWANGSGELD_BEZAHLT"))
                .BEIHILFE = CBool(kostendt.Rows(0).Item("BEIHILFE"))
                .BEIHILFEBezahlt = CBool(kostendt.Rows(0).Item("BEIHILFE_BEZAHLT"))

                .QUELLE = CStr(kostendt.Rows(0).Item("QUELLE"))
                .timestamp = CDate(clsDBtools.fieldvalueDate(kostendt.Rows(0).Item("TS")))
                .id = CInt(kostendt.Rows(0).Item("ID"))
            End With
        Catch ex As Exception
            nachricht("fehler in AufObjUmschichten: " ,ex)
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
            nachricht("FEHLER in getKostenOjbFromDb: " ,ex)
            Return False
        End Try
    End Function

End Module
