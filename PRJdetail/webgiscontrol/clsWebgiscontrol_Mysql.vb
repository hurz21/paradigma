#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB.LIBDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports System.Data
Imports MySql.Data.MySqlClient
'Imports Layer2shpFileServer.LIBDB

Public Class clsWebgiscontrol_Mysql
    Implements IDisposable
    Public MeineDBConnection As New MySqlConnection
    Sub New(ByVal conn As System.Data.Common.DbConnection)
        MeineDBConnection = CType(conn, MySqlConnection)
    End Sub
    Private disposed As Boolean = False
    'Implement IDisposable.
    Public Overloads Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub
    Protected Overridable Overloads Sub Dispose(disposing As Boolean)
        If disposed = False Then
            If disposing Then
                ' Free other state (managed objects).
                'dt.Dispose
                '_dt.Dispose
                MeineDBConnection.Dispose()
                disposed = True
            End If
            ' Free your own state (unmanaged objects).
            ' Set large fields to null.
        End If
    End Sub
    Protected Overrides Sub Finalize()
        ' Simply call Dispose(False).
        Dispose(False)
    End Sub

    Sub genLayer(ByVal featureclassname$, ByVal directory$, ByVal dbpfad$, ByVal dbdatei$, ByVal webgisREC As clsDBspecMYSQL)
        'wird in paradigma nicht genutzt
        Dim hinweis As String = ""
        Try
            nachricht("Diese Routine sollte nicht genutzt werden, da unvollständig. besser insertFeatureClassIntoWebgiscontrolDB")
            Dim com As MySqlCommand
            webgisREC.dboeffnen(hinweis$)

            '##################################
            com = New MySqlCommand("insert into  webgiscontrol.featureclasses " & _
             " set featureclass =@featureclass, titel=@titel ", MeineDBConnection)
            com.Parameters.AddWithValue("@featureclass", (featureclassname$))
            com.Parameters.AddWithValue("@titel", (featureclassname))
            Dim n% = com.ExecuteNonQuery
            If n < 1 Then
                MsgBox("genLayer  Problem beim anlegen in FeatureClasses")
            End If
            '##################################
            com = New MySqlCommand("insert into  webgiscontrol.featurerange " & _
             " set featureclass =@featureclass ,Directory=@Directory ,graphictype=@graphictype,mitimagemap=@mitimagemap ", MeineDBConnection)
            com.Parameters.AddWithValue("@featureclass", (featureclassname$))
            com.Parameters.AddWithValue("@Directory", (directory))
            com.Parameters.AddWithValue("@graphictype", (11))
            com.Parameters.AddWithValue("@mitimagemap", (1))
            ' com.Parameters.AddWithValue("@mitsachgebiet", (1))
            n% = com.ExecuteNonQuery
            If n < 1 Then
                MsgBox("Problem beim anlegen in featurerange")
            End If

            com = New MySqlCommand("insert into  webgiscontrol.featurezusachgebiete " & _
             " set featureclass =@featureclass  ", MeineDBConnection)
            com.Parameters.AddWithValue("@featureclass", (featureclassname$))
            n% = com.ExecuteNonQuery
            If n < 1 Then
                MsgBox("Problem beim anlegen in featurezusachgebiete")
            End If
            '##################################
            nachricht(" erfolg As Boolean = " & insertInfeaturedatabase(featureclassname$, dbpfad$, dbdatei$, webgisREC).ToString)
            webgisREC.dbschliessen(hinweis$)
        Catch ex As Exception
            nachricht_und_Mbox(ex.ToString)
        End Try
    End Sub

    Shared Sub nachricht(ByVal text$)
        My.Log.WriteEntry(text)
    End Sub
    Public Sub nachricht(ByVal text As String, ByVal ex As System.Exception)
        Dim anhang As String = ""
        text = text & ToLogString(ex, text)
        'myGlobalz.sitzung.nachrichtenText = text
        My.Log.WriteEntry(text)
        'mitFehlerMail(text, anhang)
    End Sub
    Shared Sub nachricht_und_Mbox(ByVal text$)
        MsgBox(text)
        My.Log.WriteEntry(text)
    End Sub

    Public Function insertInfeaturedatabase(ByVal featureclassname$, ByVal dbpfad$, ByVal dbdatei$, ByVal webgisREC As IDB_grundfunktionen) As Boolean
        Try
            Dim n%
            Dim comm As MySqlCommand
            comm = New MySqlCommand("insert into  webgiscontrol.featuredatabase " & _
             " set featureclass =@featureclass,dbtyp=@dbtyp,server=@server,db=@db,tabelle=@tabelle,Link_spalte_name=@Link_spalte_name", MeineDBConnection)
            comm.Parameters.AddWithValue("@featureclass", (featureclassname$))
            comm.Parameters.AddWithValue("@dbtyp", ("dbf"))
            comm.Parameters.AddWithValue("@server", ("default"))
            comm.Parameters.AddWithValue("@db", (dbpfad))
            comm.Parameters.AddWithValue("@tabelle", (dbdatei))
            comm.Parameters.AddWithValue("@Link_spalte_name", ("SHAPEID"))
            MeineDBConnection.Close()
            MeineDBConnection.Open()
            n% = comm.ExecuteNonQuery
            If n < 1 Then
                'MsgBox("Problem beim anlegen in featuredatabase")
                nachricht("Problem beim anlegen in featuredatabase")
                Return False
            Else
                nachricht("anlegen in featuredatabase erfolgreich")
                Return True
            End If
        Catch ex As Exception
            nachricht_und_Mbox(ex.ToString)
            Return False
        End Try
    End Function

    ''' <summary>
    ''' gibt zurück ob die ebene schon existiert
    ''' dann braucht auch die dateistruktur nicht mehr angelegt werden
    ''' </summary>
    ''' <param name="featureclassname"></param>
    ''' <param name="Titel"></param>
    ''' <param name="directory"></param>
    ''' <param name="dbpfad"></param>
    ''' <param name="dbdatei"></param>
    ''' <param name="sachgebiet"></param>
    ''' <param name="georead"></param>
    ''' <param name="webgisREC"></param>
    ''' <returns>lEbeneExistiertSchon</returns>
    ''' <remarks></remarks>
    Function insertFeatureClassIntoWebgiscontrolDB(ByVal featureclassname$, _
                                    ByVal Titel$, _
                                    ByVal directory$, _
                                    ByVal dbpfad$, _
                                    ByVal dbdatei$, _
                                    ByVal sachgebiet$, _
                                    ByVal georead$, _
                                    ByVal webgisrec As IDB_grundfunktionen) As Boolean
        Dim hinweis As String = ""
        Try
            nachricht("insertIntoWebgiscontrolDB ------------------------------------")
            Dim com As MySqlCommand
            com = New MySqlCommand("")
            MeineDBConnection.Open()
            '##################################
            Dim lEbeneExistiertSchon As Boolean = ebeneExistiertSchon(featureclassname, webgisrec, com)
            If Not lEbeneExistiertSchon Then
                Dim n%
                Dim erfolg As Boolean = InsertInfeatureClasses(Titel, georead, webgisrec, n, com, featureclassname)
                '##################################
                erfolg = insertInFeatureRange(directory, webgisrec, com, n, featureclassname)
                If Not erfolg Then
                    nachricht("Fehler bei insertInFeatureRange")
                    Return False
                End If
                '##################################
                erfolg = InsertIntoDoku(com)

                '##################################
                erfolg = InsertInFeaturezusachgebiete(sachgebiet, webgisrec, com, featureclassname$)
                If Not erfolg Then
                    nachricht("Fehler bei InsertInFeaturezusachgebiete")
                    Return False
                End If
                '##################################
                erfolg = insertInfeaturedatabase(featureclassname$, dbpfad$, dbdatei$, webgisrec)
            Else
                nachricht("ebeneExistiertSchon")
            End If

            MeineDBConnection.Close()

            nachricht("insertIntoWebgiscontrolDB ############################## ende")
            Return lEbeneExistiertSchon
        Catch ex As Exception
            nachricht("Fehler in insertIntoWebgiscontrolDB ############################## ende" ,ex)
            nachricht_und_Mbox(ex.ToString)
            Return False
        End Try
    End Function

    Function ebeneExistiertSchon(ByVal featureclassname$,
                                 ByVal webgisREC As IDB_grundfunktionen, _
                                 ByRef com As MySqlCommand) As Boolean
        Dim _mycount% = 0, dt As New DataTable
        com = New MySqlCommand(String.Format("select * from webgiscontrol.featureclasses  where featureclass ='{0}'", featureclassname), MeineDBConnection)
        Dim da As New MySqlDataAdapter(com) With {.MissingSchemaAction = MissingSchemaAction.AddWithKey}
        dt = New DataTable
        _mycount = da.Fill(dt)
        da.Dispose()
        dt.Dispose()
        If _mycount > 0 Then
            Return True
        Else
            Return False
        End If
    End Function

    Private Function InsertInfeatureClasses(ByVal Titel As String, _
                                        ByVal georead As String, _
                                        ByVal webgisREC As IDB_grundfunktionen, _
                                        ByRef n%, _
                                        ByRef com As MySqlCommand, _
                                        ByVal featureclassname$) As Boolean

        com = New MySqlCommand("insert into webgiscontrol.featureclasses " & _
         " set featureclass = @featureclass, titel=@titel,georead=@georead,dbread=@dbread ", MeineDBConnection)
        com.Parameters.AddWithValue("@featureclass", (featureclassname$))
        com.Parameters.AddWithValue("@titel", (Titel))
        com.Parameters.AddWithValue("@georead", (georead))
        com.Parameters.AddWithValue("@dbread", (georead))
        n = com.ExecuteNonQuery
        If n < 1 Then
            ' MsgBox("Problem beim anlegen in FeatureClasses")
            nachricht("FeatureClasses nicht ok")
            Return False
        Else
            nachricht("FeatureClasses ok")
            Return True
        End If
    End Function

    ''' <summary>
    ''' standardwerte_ transparenz 50, grafiktyp 11,mitimagemap=1
    ''' </summary>
    ''' <param name="directory"></param>
    ''' <param name="webgisREC"></param>
    ''' <param name="com"></param>
    ''' <param name="n"></param>
    ''' <param name="featureclassname"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function insertInFeatureRange(ByVal directory$, _
                                    ByVal webgisREC As IDB_grundfunktionen, _
                                    ByRef com As MySqlCommand, _
                                    ByRef n%, _
                                    ByVal featureclassname$) As Boolean
        Try
            com = New MySqlCommand("insert into  webgiscontrol.featurerange " & _
             " set featureclass = @featureclass ,Directory=@Directory ,graphictype=@graphictype,mitimagemap=@mitimagemap,transparency=@transparency  ", _
                   MeineDBConnection)
            com.Parameters.AddWithValue("@featureclass", (featureclassname$))
            com.Parameters.AddWithValue("@Directory", (directory))
            com.Parameters.AddWithValue("@graphictype", (11))
            com.Parameters.AddWithValue("@transparency", (50))
            com.Parameters.AddWithValue("@mitimagemap", (1))
            'com.Parameters.AddWithValue("@mitsachgebiet", (1))
            n = com.ExecuteNonQuery
            If n < 1 Then
                '  MsgBox("Problem beim anlegen in featurerange")
                nachricht("featurerange nicht ok")
                Return False
            Else
                nachricht("featurerange ok")
                Return True
            End If
        Catch ex As Exception
            nachricht("Fehler in insertInFeatureRange: " ,ex)
            Return False
        End Try
    End Function

    Private Function InsertInFeaturezusachgebiete(ByVal sachgebiet$, ByVal webgisREC As IDB_grundfunktionen, ByRef com As MySqlCommand, ByVal featureclassname$) As Boolean
        Dim n%
        com = New MySqlCommand("insert into  webgiscontrol.featurezusachgebiete " & _
         " set featureclass = @featureclass, sachgebiet = @sachgebiet", MeineDBConnection)
        com.Parameters.AddWithValue("@featureclass", (featureclassname$))
        com.Parameters.AddWithValue("@sachgebiet", (sachgebiet))
        n% = com.ExecuteNonQuery
        If n < 1 Then
            'MsgBox("Problem beim anlegen in featurezusachgebiete")
            nachricht("featurezusachgebiete nicht ok")
            Return False
        Else
            nachricht("featurezusachgebiete ok")
            Return True
        End If

    End Function
    Private Shared Function InsertIntoDoku(ByRef com As MySqlCommand) As Boolean

        Return False
    End Function



End Class
