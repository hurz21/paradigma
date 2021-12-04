Imports System.Data
Namespace VSTTools
    Public Class editStammdaten_alleDB
        Public Shared Function exe(ByRef vid%, ByVal stamm As Stamm) As Boolean ', myGlobalz.sitzung.Vorgang.Stammdaten
            Dim erfolg As Boolean
            If myGlobalz.vorgang_MYDB.dbtyp = "mysql" Then
                Dim zzz As New clsStammCRUD_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.vorgang_MYDB))
                erfolg = zzz.EDIT_speichern_stammdaten(vid%, myGlobalz.sitzung.VorgangREC, stamm)
            End If
            If myGlobalz.vorgang_MYDB.dbtyp = "oracle" Then
                Dim zzz As New clsStammCRUD_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB))
                erfolg = zzz.EDIT_speichern_stammdaten(vid%, myGlobalz.sitzung.VorgangREC, 
                                                       stamm, 
                                                       myGlobalz.sitzung.aktVorgang.Stammdaten.LetzteBearbeitung)
            End If
            Return erfolg
        End Function


        'Public Shared Sub speichernEreignisStammdaten()
        '    If myGlobalz.vorgang_MYDB.dbtyp = "mysql" Then
        '        Dim zzz As New clsStammCRUD_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.vorgang_MYDB))
        '        If zzz.EDIT_speichern_stammdaten(myGlobalz.sitzung.VorgangsID, myGlobalz.sitzung.VorgangREC, myGlobalz.sitzung.Vorgang.Stammdaten) Then
        '            nachricht("Stammdaten wurden angepasst")
        '        End If
        '    End If
        '    If myGlobalz.vorgang_MYDB.dbtyp = "oracle" Then
        '        Dim zzz As New clsStammCRUD_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB))
        '        If zzz.EDIT_speichern_stammdaten(myGlobalz.sitzung.VorgangsID, myGlobalz.sitzung.VorgangREC, myGlobalz.sitzung.Vorgang.Stammdaten) Then
        '            nachricht("Stammdaten wurden angepasst")
        '        End If
        '    End If
        'End Sub

    End Class

    Public Class NEU_StammSpeichern_alleDB
        Public Shared Function exe(ByVal zeitstempel As Date) As Boolean
            Dim erfolg As Boolean
            If myGlobalz.vorgang_MYDB.dbtyp = "mysql" Then
                Dim zzz As New clsStammCRUD_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.vorgang_MYDB))
                erfolg = zzz.Neu_speichern_stammdaten(myGlobalz.sitzung.VorgangREC, myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.aktVorgang.Stammdaten)
            End If
            If myGlobalz.vorgang_MYDB.dbtyp = "oracle" Then
                Dim zzz As New clsStammCRUD_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB))
                erfolg = zzz.Neu_speichern_stammdaten(myGlobalz.sitzung.VorgangREC,
                                                      myGlobalz.sitzung.aktVorgangsID,
                                                      myGlobalz.sitzung.aktVorgang.Stammdaten,
                                                      zeitstempel)
            End If
            Return erfolg
        End Function
    End Class


    Public Class leseAktenzeichen
        Public Shared Function exe(ByVal vorgangsid%, ByVal dbrec As IDB_grundfunktionen) As Boolean 'myGlobalz.sitzung.VorgangsID	 ,myGlobalz.sitzung.VorgangREC
            dbrec.mydb.Tabelle = "Vorgang"
            Return DB_Oracle_sharedfunctions.getDT_("", vorgangsid, dbrec)
        End Function
    End Class

    Public Class holeFlureInVorgaengenDT
        Public Shared Sub exe()
            Dim resultDT As New DataTable
            myGlobalz.sitzung.tempREC.mydb.SQL = "select distinct flur  from paraflurstueck" & _
             " where gemcode = " & myGlobalz.sitzung.aktFST.normflst.gemcode & _
             " order by flur "
            Dim anzahl As Integer = selectFromParadigmaTabelle_alleDB.exe(myGlobalz.sitzung.tempREC.mydb.SQL, "paraflurstueck", resultDT)
            myGlobalz.sitzung.tempREC.dt = resultDT.Copy
            'If myGlobalz.vorgang_MYDB.dbtyp = "mysql" Then
            '    nachricht(myGlobalz.sitzung.tempREC.getDataDT())
            'End If
        End Sub
    End Class


    Public Class LoescheStammdaten_alleDB
        Public Shared Function exe(ByVal vid%,
                                    ByVal vorgangsREC As IDB_grundfunktionen,
                                    ByVal stamm As Stamm) As Boolean 'myGlobalz.sitzung.VorgangsID, myGlobalz.sitzung.VorgangREC, myGlobalz.sitzung.Vorgang.Stammdaten
            Dim erfolg As Boolean
            If myGlobalz.vorgang_MYDB.dbtyp = "mysql" Then
                Dim zzz As New clsStammCRUD_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.vorgang_MYDB))
                erfolg = zzz.DELETE_stammdaten(vid, vorgangsREC, stamm)
            End If
            If myGlobalz.vorgang_MYDB.dbtyp = "oracle" Then
                Dim zzz As New clsStammCRUD_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB))
                erfolg = zzz.DELETE_stammdaten(vid, vorgangsREC, stamm)
            End If
            Return erfolg
        End Function
    End Class


    Public Class LoescheVorgang_alleDB
        Public Shared Function exe(ByVal vid as integer) as  Boolean 'myGlobalz.sitzung.VorgangsID, myGlobalz.sitzung.VorgangREC, myGlobalz.sitzung.Vorgang.Stammdaten
            Dim erfolg As Boolean
            If myGlobalz.vorgang_MYDB.dbtyp = "mysql" Then
                Dim zzz As New clsVorgangDB_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.vorgang_MYDB))
                erfolg = zzz.Delete_Vorgang(vid)
                    zzz.dispose
            End If
            If myGlobalz.vorgang_MYDB.dbtyp = "oracle" Then
                Dim zzz As New clsVorgangDB_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB))
                erfolg = zzz.Delete_Vorgang(vid)
                    zzz.dispose
            End If
            Return erfolg
        End Function
    End Class
    ''' <summary>
    ''' test
    ''' </summary>
    ''' <remarks></remarks>
    Public Class SpeichernVorgang_alleDB
        Public Shared Function exe(ByVal vid as integer) as  Boolean 'myGlobalz.sitzung.VorgangsID, myGlobalz.sitzung.VorgangREC, myGlobalz.sitzung.Vorgang.Stammdaten
            Dim erfolg As Boolean
            If myGlobalz.vorgang_MYDB.dbtyp = "mysql" Then
                Dim zzz As New clsVorgangDB_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.vorgang_MYDB))
                erfolg = zzz.Edit_speichern_Vorgang(vid)
                    zzz.dispose
            End If
            If myGlobalz.vorgang_MYDB.dbtyp = "oracle" Then
                Dim zzz As New clsVorgangDB_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB))
                erfolg = zzz.Edit_speichern_Vorgang(vid)
                    zzz.dispose
            End If
            Return erfolg
        End Function
    End Class

    Public Class EinfuegeVorgang_AlleDB
        Public Shared Function exe() As Boolean
            Dim erfolg As Boolean
            If myGlobalz.vorgang_MYDB.dbtyp = "mysql" Then
                Dim zzz As New clsVorgangDB_Oracle(clsDBspecMYSQL.getConnection(myGlobalz.vorgang_MYDB))
                erfolg = zzz.Neu_speichern_Vorgang()
                   zzz.dispose
            End If
            If myGlobalz.vorgang_MYDB.dbtyp = "oracle" Then
                Dim zzz As New clsVorgangDB_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB))
                erfolg = zzz.Neu_speichern_Vorgang()
                   zzz.dispose
            End If
            Return erfolg
        End Function
    End Class

    Public Class selectFromParadigmaTabelle_alleDB
        Public Shared Function exe(ByVal SQL$, ByVal Tabelle$, ByRef resultDT As DataTable) As Integer 'myGlobalz.sitzung.VorgangsID, myGlobalz.sitzung.VorgangREC, myGlobalz.sitzung.Vorgang.Stammdaten
            Dim erfolg As Integer
            If myGlobalz.vorgang_MYDB.dbtyp = "mysql" Then
                Dim zzz As New clsVorgangDB_Mysql(clsDBspecMYSQL.getConnection(myGlobalz.vorgang_MYDB))
                erfolg = zzz.selectFromParadigmaTabelle(SQL, Tabelle, resultDT)
                    zzz.dispose
            End If
            If myGlobalz.vorgang_MYDB.dbtyp = "oracle" Then
                Dim zzz As New clsVorgangDB_Oracle(LIBoracle.clsDBspecOracle.getConnection(myGlobalz.vorgang_MYDB))
                erfolg = zzz.selectFromParadigmaTabelle(SQL, Tabelle, resultDT)
                    zzz.dispose
            End If
            Return erfolg
        End Function
    End Class
End Namespace
