
#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports System.Data

Public Class kostenORACLE
      Implements IDisposable
   #Region "IDisposable Support"
    Private disposedValue As Boolean' So ermitteln Sie überflüssige Aufrufe
    Protected     Overridable     Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                MeineDBConnection.Dispose
            End If
        End If
        Me.disposedValue = True
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
    Public MeineDBConnection As New OracleConnection
    Sub New(ByVal conn As System.Data.Common.DbConnection)
        MeineDBConnection = CType(conn, OracleConnection)
    End Sub

    Private Shared Sub avoidNUlls(ByVal nat As clsKosten)
        'If String.IsNullOrEmpty(lpers.Kontakt.Anschrift.PostfachPLZ) Then lpers.Kontakt.Anschrift.PostfachPLZ = ""
        'If String.IsNullOrEmpty(lpers.Name) Then lpers.Name = " "
        'If String.IsNullOrEmpty(lpers.Vorname) Then lpers.Vorname = " "
        'If String.IsNullOrEmpty(lpers.Bezirk) Then lpers.Bezirk = " "
        'If String.IsNullOrEmpty(lpers.Quelle) Then lpers.Quelle = myGlobalz.sitzung.Bearbeiter.Initiale
    End Sub
    Shared Function setSQLbody() As String
        Return " SET VORGANGSID=:VORGANGSID" & _
                    ",INTERNEZAHLUNG=:INTERNEZAHLUNG" & _
                    ",VERWALTUNGSGEBUEHR=:VERWALTUNGSGEBUEHR" & _
                    ",VERWALTUNGSGEBUEHR_BEZAHLT=:VERWALTUNGSGEBUEHR_BEZAHLT" & _
                    ",ERSATZGELD=:ERSATZGELD" & _
                    ",ERSATZGELD_BEZAHLT=:ERSATZGELD_BEZAHLT" & _
                    ",SICHERHEIT=:SICHERHEIT" & _
                    ",SICHERHEIT_BEZAHLT=:SICHERHEIT_BEZAHLT" & _
                    ",VERWARNUNGSGELD=:VERWARNUNGSGELD" & _
                    ",VERWARNUNGSGELD_BEZAHLT=:VERWARNUNGSGELD_BEZAHLT" & _
                    ",BUSSGELD=:BUSSGELD" & _
                    ",BUSSGELD_BEZAHLT=:BUSSGELD_BEZAHLT" & _
                    ",ERSATZGELD_AUSGEZAHLT=:ERSATZGELD_AUSGEZAHLT" & _
                    ",ZWANGSGELD=:ZWANGSGELD" & _
                    ",ZWANGSGELD_BEZAHLT=:ZWANGSGELD_BEZAHLT" & _
                    ",BEIHILFE=:BEIHILFE" & _
                    ",BEIHILFE_BEZAHLT=:BEIHILFE_BEZAHLT" & _
                    ",QUELLE=:QUELLE "


        
            '            .ersatzgeldAUSGEzahlt = CBool(kostendt.Rows(0).Item("ERSATZGELD_AUSGEZAHLT"))
            '.ZWANGSGELD = CBool(kostendt.Rows(0).Item("ZWANGSGELD"))
            '.ZWANGSGELDBezahlt = CBool(kostendt.Rows(0).Item("ZWANGSGELD_BEZAHLT"))
            '.BEIHILFE = CBool(kostendt.Rows(0).Item("BEIHILFE"))
            '.BEIHILFEBezahlt = CBool(kostendt.Rows(0).Item("BEIHILFE_BEZAHLT"))
    End Function
    Shared Sub setSQLParams(ByRef com As OracleCommand, ByVal nat As clsKosten)
        avoidNUlls(nat)
        Try
            With nat
                com.Parameters.AddWithValue(":VORGANGSID", .vorgangsid)
                com.Parameters.AddWithValue(":INTERNEZAHLUNG", (.InterneZahlung))
                com.Parameters.AddWithValue(":VERWALTUNGSGEBUEHR", (.verwaltungsgebuehr))
                com.Parameters.AddWithValue(":VERWALTUNGSGEBUEHR_BEZAHLT", (.verwaltungsgebuehrBezahlt))
                com.Parameters.AddWithValue(":ERSATZGELD", (.ersatzgeld))
                com.Parameters.AddWithValue(":ERSATZGELD_BEZAHLT", (.ersatzgeldBezahlt))
                com.Parameters.AddWithValue(":SICHERHEIT", (.sicherheit))
                com.Parameters.AddWithValue(":SICHERHEIT_BEZAHLT", (.sicherheitBezahlt))

                com.Parameters.AddWithValue(":VERWARNUNGSGELD", (.VERWARNUNGSGELD))
                com.Parameters.AddWithValue(":VERWARNUNGSGELD_BEZAHLT", (.VerwarnungsgeldBezahlt))
                com.Parameters.AddWithValue(":BUSSGELD", (.BUSSGELD))
                com.Parameters.AddWithValue(":BUSSGELD_BEZAHLT", (.BUSSGELDBezahlt))

                com.Parameters.AddWithValue(":ERSATZGELD_AUSGEZAHLT", (.ersatzgeldAUSGEzahlt))
                com.Parameters.AddWithValue(":ZWANGSGELD", (.ZWANGSGELD))
                com.Parameters.AddWithValue(":ZWANGSGELD_BEZAHLT", (.ZWANGSGELDBezahlt))
                com.Parameters.AddWithValue(":BEIHILFE", (.BEIHILFE))
                com.Parameters.AddWithValue(":BEIHILFE_BEZAHLT", (.BEIHILFEBezahlt))

                com.Parameters.AddWithValue(":QUELLE", .QUELLE.Trim)
            End With
            '  com.Parameters.AddWithValue(":VORGANGSID", vid)



        Catch ex As Exception
            nachricht("Fehler in setSQLParams beteiligte: " ,ex)
        End Try

    End Sub


    'Public Function Kosten_abspeichern_EditExtracted(ByVal lnat As clsKosten) As Integer
    '    Dim anzahlTreffer& = 0, hinweis$ = ""
    '    Dim com As OracleCommand
    '    Try
    '        If lnat.id < 1 Then
    '            nachricht_und_Mbox("FEHLER:kosten_abspeichern_EditExtracted updateid =0. Abbruch")
    '            Return 0
    '        End If
    '        myGlobalz.sitzung.VorgangREC.mydb.Tabelle = " & CLstart.myViewsNTabs.tabKosten & " 
    '        myGlobalz.sitzung.VorgangREC.mydb.SQL = _
    '         "UPDATE  " & myGlobalz.sitzung.VorgangREC.mydb.Tabelle & setSQLbody() & " WHERE ID=:ID"

    '        MeineDBConnection.Open()
    '        com = New OracleCommand(myGlobalz.sitzung.VorgangREC.mydb.SQL, MeineDBConnection) ' myGlobalz.sitzung.personenRec.myconn)
    '        setSQLParams(com, lnat)
    '        com.Parameters.AddWithValue(":ID", lnat.id)
    '        anzahlTreffer = CInt(com.ExecuteNonQuery)
    '        MeineDBConnection.Close()

    '        If anzahlTreffer < 1 Then
    '            nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.VorgangREC.mydb.SQL)
    '            Return -1
    '        Else
    '            Return CInt(anzahlTreffer)
    '        End If
    '    Catch ex As Exception
    '        nachricht_und_Mbox("Bet4 Fehler beim Abspeichern: " ,ex)
    '        Return -2
    '    End Try
    'End Function

    'Public Function kosten_abspeichern_Neu(ByVal lnat As clsKosten) As Integer
    '    Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
    '    Dim com As OracleCommand
    '    Try
    '        myGlobalz.sitzung.VorgangREC.mydb.Tabelle =" " & CLstart.myViewsNTabs.tabKosten & " "

    '        Dim SQLUPDATE$ = _
    '     String.Format("INSERT INTO {0} (VORGANGSID,INTERNEZAHLUNG,VERWALTUNGSGEBUEHR,VERWALTUNGSGEBUEHR_BEZAHLT," &
    '                            "ERSATZGELD,ERSATZGELD_BEZAHLT,SICHERHEIT,SICHERHEIT_BEZAHLT,QUELLE," &
    '                            "VERWARNUNGSGELD,VERWARNUNGSGELD_BEZAHLT,BUSSGELD,BUSSGELD_BEZAHLT," &
    '                            "ERSATZGELD_AUSGEZAHLT,ZWANGSGELD,ZWANGSGELD_BEZAHLT,BEIHILFE,BEIHILFE_BEZAHLT) " +
    '                           " VALUES (:VORGANGSID,:INTERNEZAHLUNG,:VERWALTUNGSGEBUEHR,:VERWALTUNGSGEBUEHR_BEZAHLT," &
    '                            ":ERSATZGELD,:ERSATZGELD_BEZAHLT,:SICHERHEIT,:SICHERHEIT_BEZAHLT,:QUELLE," &
    '                            ":VERWARNUNGSGELD,:VERWARNUNGSGELD_BEZAHLT,:BUSSGELD,:BUSSGELD_BEZAHLT," &
    '                            ":ERSATZGELD_AUSGEZAHLT,:ZWANGSGELD,:ZWANGSGELD_BEZAHLT,:BEIHILFE,:BEIHILFE_BEZAHLT)",
    '                             myGlobalz.sitzung.VorgangREC.mydb.Tabelle)






    '        SQLUPDATE$ = SQLUPDATE$ & " RETURNING ID INTO :R1"
    '        MeineDBConnection.Open()
    '        com = New OracleCommand(SQLUPDATE$, MeineDBConnection) ' myGlobalz.sitzung.personenRec.myconn)
    '        setSQLParams(com, lnat)

    '        newid = clsOracleIns.GetNewid(com, SQLUPDATE)
    '        MeineDBConnection.Close()
    '        Return clsOracleIns.gebeNeuIDoderFehler(newid, SQLUPDATE)
    '    Catch ex As Exception
    '        nachricht_und_Mbox("kosten Fehler beim Abspeichern: " ,ex)
    '        Return -2
    '    End Try
    'End Function

    Shared Function getkostenDatatable(vorgangsid As Integer) As DataTable
        Dim hinweis As String
        Try
            'myGlobalz.sitzung.VorgangREC.mydb.Tabelle = " & CLstart.myViewsNTabs.tabKosten & " 
            myGlobalz.sitzung.VorgangREC.mydb.SQL = "select * from  " & CLstart.myViewsNTabs.tabKosten & "  where vorgangsid=" & vorgangsid
            hinweis = myGlobalz.sitzung.VorgangREC.getDataDT()
            Return myGlobalz.sitzung.VorgangREC.dt
        Catch ex As Exception
            nachricht("fehler in getkostenDatatable : " ,ex)
            Return Nothing
        End Try
    End Function




End Class
