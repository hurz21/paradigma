Imports MySql.Data.MySqlClient

Public Class clsZAHLUNGDB_Mysql
      Implements IDisposable
   #Region "IDisposable Support"
    Private disposedValue As Boolean' So ermitteln Sie überflüssige Aufrufe
    Protected     Overridable     Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                MeineMySqlConnection.Dispose
            End If
        End If
        Me.disposedValue = True
    End Sub
    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
#End Region
    Public MeineMySqlConnection As New MySqlConnection
    Sub New(ByVal conn As System.Data.Common.DbConnection)
        MeineMySqlConnection = CType(conn, MySqlConnection)
    End Sub

    Private Shared Function setSQLbody$()
        Return " set " & _
         " Sachgebietsnr=@Sachgebietsnr" & _
         ",gruppe=@gruppe" & _
         ",VorgangsID=@VorgangsID " & _
         ",EreignisID=@EreignisID " & _
         ",Aktenzeichen=@Aktenzeichen " & _
         ",Bearbeiterinitial=@Bearbeiterinitial " & _
         ",typ=@typ " & _
         ",Richtung=@Richtung " & _
         ",verschicktam=@verschicktam " & _
         ",eingangam=@eingangam " & _
         ",notiz=@notiz " & _
         ",Beschreibung=@Beschreibung " & _
         ",Zahler=@Zahler " & _
         ",betrag=@betrag " & _
         ",HHST=@HHST " & _
         ",istAnordnungbestellt=@istAnordnungbestellt" & _
         ",istAngeordnet=@istAngeordnet "
    End Function
    Shared Function setCOMParams(ByVal com As MySqlCommand, ByVal zahlungsid as integer) as  Boolean
        com.Parameters.AddWithValue("@Sachgebietsnr", myGlobalz.sitzung.aktVorgang.Stammdaten.az.sachgebiet.Zahl)
        com.Parameters.AddWithValue("@gruppe", "")
        com.Parameters.AddWithValue("@VorgangsID", CInt(myGlobalz.sitzung.aktVorgangsID))
        com.Parameters.AddWithValue("@EreignisID", CInt(myGlobalz.sitzung.aktEreignis.ID))
        com.Parameters.AddWithValue("@Aktenzeichen", myGlobalz.sitzung.aktVorgang.Stammdaten.az.gesamt)
        com.Parameters.AddWithValue("@Bearbeiterinitial", myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter.Initiale)
        com.Parameters.AddWithValue("@typ", (myGlobalz.sitzung.aktZahlung.Typ))
        com.Parameters.AddWithValue("@Richtung", CBool(myGlobalz.sitzung.aktZahlung.Eingang))
        com.Parameters.AddWithValue("@verschicktam", Convert.ToDateTime(Format(myGlobalz.sitzung.aktZahlung.VerschicktAm, "yyyy-MM-dd HH:mm:ss")))
        com.Parameters.AddWithValue("@eingangam", Convert.ToDateTime(Format(myGlobalz.sitzung.aktZahlung.EingangAm, "yyyy-MM-dd HH:mm:ss")))
        com.Parameters.AddWithValue("@notiz", myGlobalz.sitzung.aktEreignis.Notiz)
        com.Parameters.AddWithValue("@Beschreibung", myGlobalz.sitzung.aktEreignis.Beschreibung)
        com.Parameters.AddWithValue("@Zahler", myGlobalz.sitzung.aktZahlung.Zahler)
        com.Parameters.AddWithValue("@betrag", CDbl(myGlobalz.sitzung.aktZahlung.Betrag))
        com.Parameters.AddWithValue("@zahlungsID", zahlungsid%)
        com.Parameters.AddWithValue("@HHST", CStr(myGlobalz.sitzung.aktZahlung.HausHaltsstelle))
        com.Parameters.AddWithValue("@istAnordnungbestellt", CBool(myGlobalz.sitzung.aktZahlung.istAnordnungbestellt))
        com.Parameters.AddWithValue("@istAngeordnet", CBool(myGlobalz.sitzung.aktZahlung.istAngeordnet))
        Return True
    End Function
    Public Function Edit_speichern_zahlung(ByVal zahlungsid as integer) as  Integer
        Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        Dim com As MySqlCommand
        Dim SQLupdate$ = ""
        Try
            myGlobalz.sitzung.VorgangREC.mydb.Tabelle = "zahlungen"
            If zahlungsid < 1 Then
                nachricht_und_Mbox("Fehler: Edit_speichern_zahlung Updateid<1. abbruch.Edit_speichern_zahlung")
                Return 0
            End If
            SQLupdate$ = _
            String.Format("update {0}{1} where zahlungsID=@zahlungsID", myGlobalz.sitzung.VorgangREC.mydb.Tabelle, setSQLbody())
            myGlobalz.sitzung.VorgangREC.dboeffnen(hinweis$)
            com = New MySqlCommand(SQLupdate$, MeineMySqlConnection)
            setCOMParams(com, zahlungsid%)

            anzahlTreffer& = CInt(com.ExecuteNonQuery)

            myGlobalz.sitzung.VorgangREC.dbschliessen(hinweis$)

            myGlobalz.sitzung.aktEreignis.ID = CInt(newid)
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
        Dim com As MySqlCommand
        Dim SQLupdate$ = ""
        Try
            myGlobalz.sitzung.VorgangREC.mydb.Tabelle = "zahlungen"

            SQLupdate$ = _
            String.Format("insert into {0}{1}", myGlobalz.sitzung.VorgangREC.mydb.Tabelle, setSQLbody$())

            myGlobalz.sitzung.VorgangREC.dboeffnen(hinweis$)
            com = New MySqlCommand(SQLupdate$, MeineMySqlConnection)
            setCOMParams(com, 0)


            anzahlTreffer& = CInt(com.ExecuteNonQuery)
            com.CommandText = "Select LAST_INSERT_ID()"
            newid = CLng(com.ExecuteScalar)
            myGlobalz.sitzung.VorgangREC.dbschliessen(hinweis$)

            myGlobalz.sitzung.aktEreignis.ID = CInt(newid)
            If anzahlTreffer < 1 Then
                nachricht_und_Mbox("Problem beim Abspeichern:" & myGlobalz.sitzung.VorgangREC.mydb.SQL)
                Return -1
            Else
                Return CInt(newid)
            End If
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
            myGlobalz.sitzung.tempREC = CType(myGlobalz.sitzung.VorgangREC, clsDBspecMYSQL)
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
