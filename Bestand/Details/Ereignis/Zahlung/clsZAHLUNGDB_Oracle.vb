﻿'Imports System.Data.OracleClient
Imports LibDB
Imports System.Data

Public Class clsZAHLUNGDB_Oracle
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

    Private Shared Function setSQLbody$()
        Return " set " & _
         " SACHGEBIETSNR=:SACHGEBIETSNR" & _
         ",GRUPPE=:GRUPPE" & _
         ",VORGANGSID=:VORGANGSID " & _
         ",EREIGNISID=:EREIGNISID " & _
         ",AKTENZEICHEN=:AKTENZEICHEN " & _
         ",BEARBEITERINITIAL=:BEARBEITERINITIAL " & _
         ",TYP=:TYP " & _
         ",RICHTUNG=:RICHTUNG " & _
         ",VERSCHICKTAM=:VERSCHICKTAM " & _
         ",ANGEORDNETAM=:ANGEORDNETAM " & _
         ",EINGANGAM=:EINGANGAM " & _
         ",NOTIZ=:NOTIZ " & _
         ",BESCHREIBUNG=:BESCHREIBUNG " & _
         ",ZAHLER=:ZAHLER " & _
         ",BETRAG=:BETRAG " & _
         ",HHST=:HHST " & _
         ",ISTANORDNUNGBESTELLT=:ISTANORDNUNGBESTELLT" & _
         ",ISTANGEORDNET=:ISTANGEORDNET "
    End Function

    Shared Function setCOMParams(ByVal com As OracleCommand, ByVal zahlungsid as integer) as  Boolean
        If String.IsNullOrEmpty(myGlobalz.sitzung.aktZahlung.HausHaltsstelle) Then myGlobalz.sitzung.aktZahlung.HausHaltsstelle = ""
        com.Parameters.AddWithValue(":SACHGEBIETSNR", myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl)
        com.Parameters.AddWithValue(":GRUPPE", "")
        com.Parameters.AddWithValue(":VORGANGSID", CInt(myGlobalz.sitzung.aktVorgangsID))
        com.Parameters.AddWithValue(":EREIGNISID", CInt(myGlobalz.sitzung.aktEreignis.ID))
        com.Parameters.AddWithValue(":AKTENZEICHEN", myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt)
        com.Parameters.AddWithValue(":BEARBEITERINITIAL", myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale)
        com.Parameters.AddWithValue(":TYP", (myGlobalz.sitzung.aktZahlung.Typ))
        com.Parameters.AddWithValue(":RICHTUNG", CBool(myGlobalz.sitzung.aktZahlung.Eingang))
        com.Parameters.AddWithValue(":VERSCHICKTAM", myGlobalz.sitzung.aktZahlung.VerschicktAm)
          com.Parameters.AddWithValue(":ANGEORDNETAM", myGlobalz.sitzung.aktZahlung.ANGEORDNETAM)
        com.Parameters.AddWithValue(":EINGANGAM", myGlobalz.sitzung.aktZahlung.EingangAm)
        
        com.Parameters.AddWithValue(":NOTIZ", myGlobalz.sitzung.aktEreignis.Notiz)
        com.Parameters.AddWithValue(":BESCHREIBUNG", myGlobalz.sitzung.aktEreignis.Beschreibung)
        com.Parameters.AddWithValue(":ZAHLER", myGlobalz.sitzung.aktZahlung.Zahler)
        com.Parameters.AddWithValue(":BETRAG", CDbl(myGlobalz.sitzung.aktZahlung.Betrag))
        com.Parameters.AddWithValue(":HHST", CStr(myGlobalz.sitzung.aktZahlung.HausHaltsstelle))
        com.Parameters.AddWithValue(":ISTANORDNUNGBESTELLT", CBool(myGlobalz.sitzung.aktZahlung.istAnordnungbestellt))
        com.Parameters.AddWithValue(":ISTANGEORDNET", CBool(myGlobalz.sitzung.aktZahlung.istAngeordnet))
        Return True
    End Function

    Public Function Edit_speichern_zahlung(ByVal zahlungsid as integer) as  Integer
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As OracleCommand
        Dim SQLupdate$ = ""
        Try
            myGlobalz.sitzung.VorgangREC.mydb.Tabelle = "zahlungen"
            If zahlungsid < 1 Then
                nachricht_und_Mbox("Fehler: Edit_speichern_zahlung Updateid<1. abbruch.Edit_speichern_zahlung")
                Return 0
            End If
            SQLupdate$ = _
            String.Format("update {0}{1} where zahlungsID=:zahlungsID", myGlobalz.sitzung.VorgangREC.mydb.Tabelle, setSQLbody())
            MeineDBConnection.Open()
            com = New OracleCommand(SQLupdate$, MeineDBConnection)
            setCOMParams(com, zahlungsid%)
            com.Parameters.AddWithValue(":ZAHLUNGSID", zahlungsid%)
            anzahlTreffer = CInt(com.ExecuteNonQuery)
            MeineDBConnection.Close()
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Edit_speichern_zahlung:" & myGlobalz.sitzung.VorgangREC.mydb.SQL)
                Return -1
            Else
                Return CInt(anzahlTreffer)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
            Return -2
        End Try
    End Function


    Public Function Neu_speichern_zahlung() As Integer
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As OracleCommand
        Dim SQLupdate$ = ""
        Try
            myGlobalz.sitzung.VorgangREC.mydb.Tabelle = "zahlungen"

            SQLupdate$ =
             String.Format("INSERT INTO {0} (SACHGEBIETSNR,GRUPPE,VORGANGSID,EREIGNISID,AKTENZEICHEN,BEARBEITERINITIAL," +
                             "TYP,RICHTUNG,VERSCHICKTAM,ANGEORDNETAM,EINGANGAM,NOTIZ,BESCHREIBUNG," +
                                "ZAHLER,BETRAG,HHST,ISTANORDNUNGBESTELLT,ISTANGEORDNET) " +
                          " VALUES (:SACHGEBIETSNR,:GRUPPE,:VORGANGSID,:EREIGNISID,:AKTENZEICHEN,:BEARBEITERINITIAL," +
                             ":TYP,:RICHTUNG,:VERSCHICKTAM,:ANGEORDNETAM,:EINGANGAM,:NOTIZ,:BESCHREIBUNG," +
                               ":ZAHLER,:BETRAG,:HHST,:ISTANORDNUNGBESTELLT,:ISTANGEORDNET)",
                                  myGlobalz.sitzung.VorgangREC.mydb.Tabelle)
            SQLupdate$ = SQLupdate$ & " RETURNING ZAHLUNGSID INTO :R1"

            nachricht("nach setSQLbody : " & SQLupdate)
            MeineDBConnection.Open()
            nachricht("nach dboeffnen  ")

            com = New OracleCommand(SQLupdate$, MeineDBConnection)
            nachricht("vor setParams  ")
            setCOMParams(com, 0)


            'com.CommandText = SQLupdate$
            'com.CommandType = CommandType.Text
            'Dim p_theid As New OracleParameter

            'p_theid.DbType = DbType.Decimal
            'p_theid.Direction = ParameterDirection.ReturnValue
            'p_theid.ParameterName = ":R1"
            'com.Parameters.Add(p_theid)
            'Dim rtn = CInt(com.ExecuteNonQuery)
            'newid = CLng(p_theid.Value)
            'MeineDBConnection.Close()
            newid = LIBoracle.clsOracleIns.GetNewid(com, SQLupdate$)
            MeineDBConnection.Close()
            Return LIBoracle.clsOracleIns.gebeNeuIDoderFehler(newid, SQLupdate$)

            'If anzahlTreffer < 1 Then
            '    nachricht_und_Mbox("Problem beim Abspeichern:" & SQLupdate$)
            '    Return -1
            'Else
            '    Return CInt(newid)
            'End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Abspeichern: " & ex.ToString)
            Return -2
        End Try
    End Function



    Public Function getDT_zahlung(ByVal sql as string) as  Boolean
        Try
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "zahlungen"
            myGlobalz.sitzung.tempREC.mydb.SQL = sql
            nachricht(" hinweis$ = " & myGlobalz.sitzung.tempREC.getDataDT())
            If myGlobalz.sitzung.tempREC.dt.IsNothingOrEmpty Then
                My.Log.WriteEntry("Fatal Error ID " & myGlobalz.sitzung.aktEreignis.ID & " konnte nicht gefunden werden!" & _
                myGlobalz.sitzung.tempREC.mydb.getDBinfo(""))
            End If
        Catch ex As Exception
            nachricht_und_Mbox("FEhler: getDT_" & vbCrLf & ex.ToString)
        End Try
    End Function

    Public Function Zahlung_loeschen(ByVal zahlungsid as integer) as  Integer
        Dim anzahlTreffer&, newid&
        Try
            myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, LIBoracle.clsDBspecOracle)
            myGlobalz.sitzung.tempREC.mydb.Host = myGlobalz.sitzung.VorgangREC.mydb.Host
            myGlobalz.sitzung.tempREC.mydb.Schema = myGlobalz.sitzung.VorgangREC.mydb.Schema
            myGlobalz.sitzung.tempREC.mydb.Tabelle = "zahlungen"
            myGlobalz.sitzung.tempREC.mydb.SQL = _
             String.Format("delete from {0} where zahlungsid={1}", myGlobalz.sitzung.tempREC.mydb.Tabelle, zahlungsid%)
            anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ')
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Zahlung_loeschen:" & myGlobalz.sitzung.tempREC.mydb.SQL)
                Return -1
            Else
                Return CInt(anzahlTreffer)
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler beim Zahlung_loeschen: " & ex.ToString)
            Return -2
        End Try
    End Function
End Class
