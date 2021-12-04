Imports MySql.Data.MySqlClient

Public Class FST_serialShape_mysql
    
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
    Public MeineDBConnection As New MySqlConnection
    Sub New(ByVal conn As System.Data.Common.DbConnection)
        MeineDBConnection = CType(conn, MySqlConnection)
    End Sub
    Public Function RB_Flurstueck_Serial_loeschen(ByVal raumbezugsid as integer) as  Integer
        'Dim anzahlTreffer, newid As Long
        'Try
        '    glob2.initTemprecAusVorgangRecMysql()
        '    myGlobalz.sitzung.tempREC.mydb.Tabelle ="" & CLstart.myViewsNTabs.tabRaumbezug2geopolygon      & "   "
        '    myGlobalz.sitzung.tempREC.mydb.SQL = _
        '     "delete from " & myGlobalz.sitzung.tempREC.mydb.Tabelle & _
        '     " where raumbezugsid=" & raumbezugsid% 'sollte eindeutig sein
        '    anzahlTreffer = myGlobalz.sitzung.tempREC.sqlexecute(newid) ')
        '    If anzahlTreffer < 1 Then
        '        nachricht("Fehler: Problem beim Löschen: ggf. ist das Objekt bereits gelöscht a." & vbCrLf &
        '                                 myGlobalz.sitzung.tempREC.mydb.SQL)
        '        Return -1
        '    Else
        '        Return CInt(anzahlTreffer)
        '    End If
        'Catch ex As Exception
        '    nachricht_und_Mbox("Problem beim löschen: " & ex.ToString)
        '    Return -2
        'End Try
    End Function


#Region "Serial"
    Public Shared Function RB_FLST_Serial_abspeichern_Neu(ByVal vid%,
                                                            ByVal rbid%,
                                                            ByVal serial$,
                                                            ByVal typ%,
                                                            ByVal area As Double) As Integer
        'Dim anzahlTreffer& = 0, hinweis$ = "", newid& = 0
        'Dim com As MySqlCommand
        'Dim lokrec As New clsDBspecMYSQL
        'nachricht("RB_FLST_Serial_abspeichern_Neu -------------------------------------")
        ''  lokrec = CType(myGlobalz.sitzung.tempREC.Clone(), clsDBspecMYSQL)'ihah
        'Try
        '    glob2.initTemprecAusVorgangRecMysql()
        '    lokrec.mydb.Tabelle ="" & CLstart.myViewsNTabs.tabRaumbezug2geopolygon      & "   "
        '    lokrec.mydb.SQL = _
        '     "insert into " & lokrec.mydb.Tabelle & _
        '     setSQLBodyFLST_serial()


        '    lokrec.dboeffnen(hinweis$)

        '    com = New MySqlCommand(lokrec.mydb.SQL, lokrec.myconn)
        '    setSQLParamsFLST_serial(com, vid%, rbid%, serial$, 0, typ, area)

        '    anzahlTreffer& = CInt(com.ExecuteNonQuery)

        '    com.CommandText = "Select LAST_INSERT_ID()"
        '    newid = CLng(com.ExecuteScalar)

        '    lokrec.dbschliessen(hinweis$)

        '    'anzahlTreffer = lokrec.sqlexecute(newid)
        '    If newid < 1 Then
        '        nachricht_und_Mbox("Problem beim Abspeichern:" & lokrec.mydb.SQL)
        '        Return -1
        '    Else
        '        Return CInt(newid)
        '    End If
        'Catch mex As MySqlException
        '    nachricht_und_Mbox("Fehler in RB_FLST_Serial_abspeichern_Neu mex: " & vbCrLf & mex.ToString)
        '    Return -2
        'Catch ex As Exception
        '    nachricht_und_Mbox("Fehler in RB_FLST_Serial_abspeichern_Neu: " & vbCrLf & ex.ToString)
        '    Return -2
        'Finally
        '    lokrec = Nothing
        'End Try
    End Function

    Shared Function setSQLBodyFLST_serial() As String
        Return " set " & _
         " RaumbezugsID=@RaumbezugsID" & _
         ",VorgangsID=@VorgangsID" & _
          ",Typ=@Typ" & _
           ",AreaQm=@AreaQm" & _
         ",SerialShape=@SerialShape"
    End Function

    Shared Sub setSQLParamsFLST_serial(ByVal com As MySqlCommand, ByVal vid%, ByVal rbid%, ByVal serial$, ByVal id%, ByVal Typ%, ByVal areaqm As Double)
        com.Parameters.AddWithValue("@RaumbezugsID", rbid%)
        com.Parameters.AddWithValue("@VorgangsID", vid%)
        com.Parameters.AddWithValue("@SerialShape", serial$)
        com.Parameters.AddWithValue("@Typ", Typ)
        com.Parameters.AddWithValue("@AreaQm", areaqm)
        com.Parameters.AddWithValue("@ID", id)
    End Sub

    Sub serialInDbSpeichern(ByVal vid%, ByVal rbid%, ByVal typ%, ByVal serial$, ByVal Area As Double)
        'vid,rbid%,typ%,serial$
        nachricht("serialSpeichern: vid%:" & vid% & "rbid: " & rbid% & "serial: " & serial$)
        FST_serialShape_mysql.RB_FLST_Serial_abspeichern_Neu(vid%, rbid%, serial$, typ, Area)
    End Sub

#End Region
End Class
