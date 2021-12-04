Imports System
Imports System.ComponentModel

Public Class clsWiedervorlage
    Implements INotifyPropertyChanged

    Public DBwvcrud As IWiedervorlageCRUD
    Sub New()

    End Sub
    Sub New(ByVal eDBwvcrud As IWiedervorlageCRUD)
        DBwvcrud = eDBwvcrud
    End Sub
    Public Function bildeErgeignisBeschreibung() As String
        Try
            Dim erledigttext As String = ""
            erledigttext$ = If(Erledigt, "- erledigt -", "- unerledigt -")
            Return String.Format("Fällig: {0}, Warten auf: {1}, {2}, {3}, {4}", Format(datum, "dd.MM.yyyy"), WartenAuf, ToDo, Bemerkung, erledigttext)
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Public Event PropertyChanged(ByVal sender As Object, ByVal e As PropertyChangedEventArgs) _
    Implements INotifyPropertyChanged.PropertyChanged
    Protected Sub OnPropertyChanged(ByVal prop As String)
        anychange = True
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(prop))
    End Sub

    Public anychange As Boolean
    Private _datum As Date
    Public Property datum() As Date
        Get
            Return _datum
        End Get
        Set(ByVal Value As Date)
            _datum = Value
            OnPropertyChanged("datum")
        End Set
    End Property
    Private _toDo As String
    Public Property ToDo() As String
        Get
            Return _toDo
        End Get
        Set(ByVal Value As String)
            _toDo = Value
            OnPropertyChanged("ToDo")
        End Set
    End Property
    Private _bemerkung As String
    Public Property Bemerkung() As String
        Get
            Return _bemerkung
        End Get
        Set(ByVal Value As String)
            _bemerkung = Value
            OnPropertyChanged("Bemerkung")
        End Set
    End Property
    Private _erledigt As Boolean
    Public Property Erledigt() As Boolean
        Get
            Return _erledigt
        End Get
        Set(ByVal Value As Boolean)
            _erledigt = Value
            OnPropertyChanged("Erledigt")
        End Set
    End Property
    Private _erledigtAm As Date
    Public Property erledigtAm() As Date
        Get
            Return _erledigtAm
        End Get
        Set(ByVal Value As Date)
            _erledigtAm = Value
            OnPropertyChanged("erledigtAm")
        End Set
    End Property
    Private _vorgangsID As Integer
    Public Property VorgangsID() As Integer
        Get
            Return _vorgangsID
        End Get
        Set(ByVal Value As Integer)
            _vorgangsID = Value
            OnPropertyChanged("VorgangsID")
        End Set
    End Property
    Private _bearbeiter As String
    Public Property Bearbeiter() As String
        Get
            Return _bearbeiter
        End Get
        Set(ByVal Value As String)
            _bearbeiter = Value
            OnPropertyChanged("Bearbeiter")
        End Set
    End Property


    Private _wartenAuf As String
    Public Property WartenAuf() As String
        Get
            Return _wartenAuf
        End Get
        Set(ByVal Value As String)
            _wartenAuf = Value
            OnPropertyChanged("WartenAuf")
        End Set
    End Property
    Public Property WiedervorlageID() As Integer
    Public Property BearbeiterID As Integer = 0

    Sub clear()
        WiedervorlageID = 0
        WartenAuf = ""
        _erledigtAm = CLstart.mycsimple.MeinNULLDatumAlsDate
        Erledigt = False
        Bemerkung = ""
        ToDo = ""
        BearbeiterID = 0
        datum = CLstart.mycsimple.MeinNULLDatumAlsDate
    End Sub

    Function updateWV() As Integer
        Dim querie As String
        If myGlobalz.sitzung.aktWiedervorlage.Bearbeiter Is Nothing Then myGlobalz.sitzung.aktWiedervorlage.Bearbeiter = myGlobalz.sitzung.aktBearbeiter.Initiale
        querie = "update " & CLstart.myViewsNTabs.TABWV & "  set " &
                         " VORGANGSID=@VORGANGSID" &
                         ",TODO=@TODO" &
                          ",BEARBEITER=@BEARBEITER" &
                          ",BEARBEITERID=@BEARBEITERID" &
                         ",BEMERKUNG=@BEMERKUNG" &
                         ",WARTENAUF=@WARTENAUF" &
                         ",DATUM=@DATUM" &
                         ",ERLEDIGTAM=@ERLEDIGTAM" &
                         ",ERLEDIGT=@ERLEDIGT" &
                       "  where ID=@ID"
        clsSqlparam.paramListe.Clear()
        populateParamListeWV(clsSqlparam.paramListe, Me)
        clsSqlparam.paramListe.Add(New clsSqlparam("id", Me.WiedervorlageID))
        '--------------------------------
        Dim anz = myglobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, False, "ID")
        Return 1
        'Return DBwvcrud.update(Me)
    End Function
    Shared Sub populateParamListeWV(paramListe As List(Of clsSqlparam), twv As clsWiedervorlage)
        paramListe.Add(New clsSqlparam("VORGANGSID", myGlobalz.sitzung.aktVorgangsID))
        paramListe.Add(New clsSqlparam("TODO", twv.ToDo))
        paramListe.Add(New clsSqlparam("BEMERKUNG", twv.Bemerkung))
        paramListe.Add(New clsSqlparam("WARTENAUF", twv.WartenAuf))
        paramListe.Add(New clsSqlparam("BEARBEITER", twv.Bearbeiter))
        paramListe.Add(New clsSqlparam("BEARBEITERID", twv.BearbeiterID))

        If myGlobalz.sitzung.VorgangREC.mydb.dbtyp = "sqls" Then
            paramListe.Add(New clsSqlparam("DATUM", clsDBtools.makedateMssqlConform(CDate(twv.datum), myGlobalz.sitzung.VorgangREC.mydb.dbtyp).ToString("yyyy-MM-ddTHH:mm:ss.fffffff")))
            paramListe.Add(New clsSqlparam("ERLEDIGTAM", clsDBtools.makedateMssqlConform(CDate(twv.erledigtAm), myGlobalz.sitzung.VorgangREC.mydb.dbtyp).ToString("yyyy-MM-ddTHH:mm:ss.fffffff")))
        End If
        If myGlobalz.sitzung.VorgangREC.mydb.dbtyp = "oracle" Then
            paramListe.Add(New clsSqlparam("DATUM", clsDBtools.makedateMssqlConform(twv.datum, myGlobalz.sitzung.VorgangREC.mydb.dbtyp)))
            paramListe.Add(New clsSqlparam("ERLEDIGTAM", clsDBtools.makedateMssqlConform(twv.erledigtAm, myGlobalz.sitzung.VorgangREC.mydb.dbtyp)))
            'com.Parameters.AddWithValue(":DATUM", twv.datum)
            'com.Parameters.AddWithValue(":ERLEDIGTAM", twv.erledigtAm)
        End If
        paramListe.Add(New clsSqlparam("ERLEDIGT", Math.Abs(CInt(twv.Erledigt))))

        'com.Parameters.AddWithValue(":VORGANGSID", myGlobalz.sitzung.aktVorgangsID)
        'com.Parameters.AddWithValue(":TODO", twv.ToDo)
        'com.Parameters.AddWithValue(":BEMERKUNG", twv.Bemerkung)
        'com.Parameters.AddWithValue(":WARTENAUF", twv.WartenAuf)
        'com.Parameters.AddWithValue(":BEARBEITER", twv.Bearbeiter)
        'com.Parameters.AddWithValue(":DATUM", twv.datum)
        'com.Parameters.AddWithValue(":ERLEDIGTAM", twv.erledigtAm)
        'com.Parameters.AddWithValue(":ERLEDIGT", Math.Abs(CInt(twv.Erledigt)))
    End Sub

    Function createWV() As Integer
        Me.Erledigt = False
        Me.erledigtAm = CLstart.mycSimple.MeinNULLDatumAlsDate
        If myglobalz.sitzung.aktWiedervorlage.Bearbeiter Is Nothing Then myglobalz.sitzung.aktWiedervorlage.Bearbeiter = myglobalz.sitzung.aktBearbeiter.Initiale

        Dim querie As String = "INSERT INTO " & CLstart.myViewsNTabs.TABWV & "  " &
                 " (VORGANGSID,TODO,BEARBEITER,BEMERKUNG,WARTENAUF,DATUM,ERLEDIGTAM,ERLEDIGT,BEARBEITERID) VALUES " &
                 "(@VORGANGSID,@TODO,@BEARBEITER,@BEMERKUNG,@WARTENAUF,@DATUM,@ERLEDIGTAM,@ERLEDIGT,@BEARBEITERID) "
        clsSqlparam.paramListe.Clear()
        populateParamListeWV(clsSqlparam.paramListe, myglobalz.sitzung.aktWiedervorlage)
        myglobalz.sitzung.aktWiedervorlage.WiedervorlageID = myglobalz.sitzung.VorgangREC.manipquerie(querie, clsSqlparam.paramListe, True, "ID")
        nachricht("neue ID:" & myGlobalz.sitzung.aktWiedervorlage.WiedervorlageID)

        Return myGlobalz.sitzung.aktWiedervorlage.WiedervorlageID
    End Function

    Function delete() As Integer
        Dim hinweis As String = ""
        myGlobalz.sitzung.VorgangREC.mydb.SQL = "delete from " & CLstart.myViewsNTabs.tabWV & "  where id=" & WiedervorlageID.ToString
        myGlobalz.sitzung.VorgangREC.dt = getDT4Query(myglobalz.sitzung.VorgangREC.mydb.SQL, myglobalz.sitzung.VorgangREC, hinweis)
        Return 1
    End Function
End Class
