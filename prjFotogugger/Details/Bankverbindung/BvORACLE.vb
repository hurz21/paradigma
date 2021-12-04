
#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports System.Data

Public Class BvORACLE
    'Implements IDisposable
    'Public MeineDBConnection As New OracleConnection
    'Sub New(ByVal conn As System.Data.Common.DbConnection)
    '    MeineDBConnection = CType(conn, OracleConnection)
    'End Sub

    Private Shared Sub avoidNUlls(ByVal nat As clsBankverbindungSEPA)
        'If String.IsNullOrEmpty(lpers.Kontakt.Anschrift.PostfachPLZ) Then lpers.Kontakt.Anschrift.PostfachPLZ = ""
        'If String.IsNullOrEmpty(lpers.Name) Then lpers.Name = " "
        'If String.IsNullOrEmpty(lpers.Vorname) Then lpers.Vorname = " "
        'If String.IsNullOrEmpty(lpers.Bezirk) Then lpers.Bezirk = " "
        'If String.IsNullOrEmpty(lpers.Quelle) Then lpers.Quelle = myGlobalz.sitzung.Bearbeiter.Initiale
    End Sub
    Shared Function setSQLbodyAt() As String
        Return " set IBAN=@IBAN" &
                ",BIC=@BIC" &
                ",BANKNAME=@BANKNAME " &
                ",QUELLE=@QUELLE " &
                ",PERSONENID=@PERSONENID " &
                ",VORLAGE1=@VORLAGE1 " &
                ",TITEL=@TITEL "

        '",TS=@TS " &
    End Function
    Shared Function setSQLbody() As String
        Return " set IBAN=:IBAN" &
                ",BIC=:BIC" &
                ",BANKNAME=:BANKNAME " &
                ",QUELLE=:QUELLE " &
                ",PERSONENID=:PERSONENID " &
                ",VORLAGE1=:VORLAGE1 " &
                ",TITEL=:TITEL "

        '",TS=:TS " &
    End Function
    Shared Sub setSQLParams(ByRef com As OracleCommand, ByVal nat As clsBankverbindungSEPA)
        avoidNUlls(nat)
        Try
            With nat
                com.Parameters.AddWithValue(":IBAN", .IBAN)
                com.Parameters.AddWithValue(":BIC", (.BIC))
                com.Parameters.AddWithValue(":BANKNAME", (.BankName))
                com.Parameters.AddWithValue(":QUELLE", (.Quelle))
                com.Parameters.AddWithValue(":PERSONENID", (.personenID))
                com.Parameters.AddWithValue(":TITEL", (.Titel))
                'com.Parameters.AddWithValue(":TS", (.tss))
                com.Parameters.AddWithValue(":VORLAGE1", (.istVORLAGE1))
            End With
        Catch ex As Exception
            nachricht("Fehler in setSQLParams bv: " ,ex)
        End Try

    End Sub
    'Public Function bv_abspeichern_Edit(ByVal lnat As clsBankverbindungSEPA) As Integer
    '    Dim anzahlTreffer& = 0
    '    Dim com As OracleCommand
    '    Try
    '        If lnat.BANKKONTOID < 1 Then
    '            nachricht_und_Mbox("FEHLER:bv_abspeichern_Edit updateid =0. Abbruch")
    '            Return 0
    '        End If
    '        myglobalz.sitzung.VorgangREC.mydb.Tabelle ="bankverbindung"
    '        myglobalz.sitzung.VorgangREC.mydb.SQL = "UPDATE  " & myglobalz.sitzung.VorgangREC.mydb.Tabelle & setSQLbody() &
    '                " WHERE BANKKONTOID=:BANKKONTOID"
    '        MeineDBConnection.Open()
    '        com = New OracleCommand(myglobalz.sitzung.VorgangREC.mydb.SQL, MeineDBConnection)
    '        setSQLParams(com, lnat)
    '        com.Parameters.AddWithValue(":BANKKONTOID", lnat.BANKKONTOID)
    '        anzahlTreffer = CInt(com.ExecuteNonQuery)
    '        MeineDBConnection.Close()

    '        If anzahlTreffer < 1 Then
    '            nachricht_und_Mbox("Problem beim Abspeichern:" & myglobalz.sitzung.VorgangREC.mydb.SQL)
    '            Return -1
    '        Else
    '            Return CInt(anzahlTreffer)
    '        End If
    '    Catch ex As Exception
    '        nachricht_und_Mbox("Fehler beim bv_abspeichern_Edit: " ,ex)
    '        Return -2
    '    End Try
    'End Function

    'Public Function bv_abspeichern_Neu(ByVal lnat As clsBankverbindungSEPA) As Integer
    '    Dim newid& = 0
    '    Dim com As OracleCommand
    '    Try
    '        myglobalz.sitzung.VorgangREC.mydb.Tabelle ="bankverbindung"

    '        Dim SQLUPDATE$ =
    '     String.Format("INSERT INTO {0} (IBAN,BIC,BANKNAME,QUELLE,PERSONENID,TITEL,VORLAGE1) " &
    '                           " VALUES (:IBAN,:BIC,:BANKNAME,:QUELLE,:PERSONENID,:TITEL,:VORLAGE1)",
    '                             myglobalz.sitzung.VorgangREC.mydb.Tabelle)

    '        SQLUPDATE$ = SQLUPDATE$ & " RETURNING BANKKONTOID INTO :R1"
    '        MeineDBConnection.Open()
    '        com = New OracleCommand(SQLUPDATE$, MeineDBConnection) ' myGlobalz.sitzung.personenRec.myconn)
    '        setSQLParams(com, lnat)

    '        newid = clsOracleIns.GetNewid(com, SQLUPDATE)
    '        MeineDBConnection.Close()
    '        Return clsOracleIns.gebeNeuIDoderFehler(newid, SQLUPDATE)
    '    Catch ex As Exception
    '        nachricht_und_Mbox("Fehler bv_abspeichern_Neu: " ,ex)
    '        Return -2
    '    End Try
    'End Function

    Shared Function getbvDatatable(personenid As Integer) As DataTable

        Try
            'myglobalz.sitzung.tempREC2.mydb.Tabelle ="bankverbindung"
            myGlobalz.sitzung.tempREC2.mydb.SQL = "select * from " & CLstart.myViewsNTabs.tabbankverbindung & "  " &
                " where personenid=" & personenid &
                " and vorlage1<1"
            myGlobalz.sitzung.tempREC2.getDataDT()
            Return myglobalz.sitzung.tempREC2.dt
        Catch ex As Exception
            nachricht("fehler in getbvDatatable : " ,ex)
            Return Nothing
        End Try
    End Function

    Shared Function getbvVorlagenDatatable() As DataTable
        Try
            'myglobalz.sitzung.tempREC.mydb.Tabelle ="bankverbindung"
            myGlobalz.sitzung.tempREC.mydb.SQL = "select * from " & CLstart.myViewsNTabs.tabbankverbindung & " " &
                " where vorlage1 !=0"
            myGlobalz.sitzung.tempREC.getDataDT()
            Return myglobalz.sitzung.tempREC.dt
        Catch ex As Exception
            nachricht("fehler in getbvVorlagenDatatable : " ,ex)
            Return Nothing
        End Try
    End Function

    'Shared Function bv3PersonenID(persID As Integer) As String
    '    Try
    '        'myglobalz.sitzung.tempREC.mydb.Tabelle ="bankverbindung"
    '        myGlobalz.sitzung.tempREC.mydb.SQL = "select iban + ' ' + bankname from " & CLstart.myViewsNTabs.tabbankverbindung & "  " &
    '            " where personenid=" & persID
    '        myGlobalz.sitzung.tempREC.getDataDT()
    '        Return CStr(myglobalz.sitzung.tempREC.dt.Rows(0).Item(0))
    '    Catch ex As Exception
    '        ' nachricht("warnung in bv3PersonenID : " & myglobalz.sitzung.tempREC.mydb.SQL & " " ,ex)
    '        Return ""
    '    End Try
    'End Function

End Class
