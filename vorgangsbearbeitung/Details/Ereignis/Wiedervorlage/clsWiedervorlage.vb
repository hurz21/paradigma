Imports System
Imports System.ComponentModel

Public Class clsWiedervorlage
    Implements INotifyPropertyChanged

    Public DBwvcrud As IWiedervorlageCRUD

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
    Friend gemkrz As String
    Friend az2 As String

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

    Sub clear()
        WiedervorlageID = 0
        WartenAuf = ""
        _erledigtAm = CLstart.mycsimple.MeinNULLDatumAlsDate
        Erledigt = False
        Bemerkung = ""
        ToDo = ""
        datum = CLstart.mycsimple.MeinNULLDatumAlsDate
    End Sub

    Function updateWV() As Integer
        'Dim anzahl%
        'If myGlobalz.wiedervorlage_MYDB.dbtyp = "mysql" Then
        '    Dim zzz As New clsWiedervorlageDB_CRUD_MYSQL(clsDBspecMYSQL.getConnection(myGlobalz.wiedervorlage_MYDB))
        '    anzahl% = zzz.update(Me)
        '      zzz.Dispose
        'End If
        'If myGlobalz.wiedervorlage_MYDB.dbtyp = "oracle" Then
        '    Dim zzz As New clsWiedervorlageDB_CRUD_ORACLE(clsDBspecOracle.getConnection(myGlobalz.wiedervorlage_MYDB))
        '    anzahl% = zzz.update(Me)
        '    zzz.Dispose()
        'End If
        'Return anzahl%
        ''Return DBwvcrud.update(Me)
    End Function

    Function createWV() As Integer
        'Dim newid%
        'If myGlobalz.wiedervorlage_MYDB.dbtyp = "mysql" Then
        '    Dim zzz As New clsWiedervorlageDB_CRUD_MYSQL(clsDBspecMYSQL.getConnection(myGlobalz.wiedervorlage_MYDB))
        '    newid% = zzz.create(Me)
        '    zzz.Dispose()
        'End If
        'If myGlobalz.wiedervorlage_MYDB.dbtyp = "oracle" Then
        '    Dim zzz As New clsWiedervorlageDB_CRUD_ORACLE(clsDBspecOracle.getConnection(myGlobalz.wiedervorlage_MYDB))
        '    newid% = zzz.create(Me)
        '    zzz.Dispose()
        'End If
        'Return newid%
    End Function

    Function delete() As Integer
        'Dim anzahl%
        'If myGlobalz.wiedervorlage_MYDB.dbtyp = "mysql" Then
        '    Dim zzz As New clsWiedervorlageDB_CRUD_MYSQL(clsDBspecMYSQL.getConnection(myGlobalz.wiedervorlage_MYDB))
        '    anzahl% = zzz.delete(Me)
        '    zzz.Dispose()
        'End If
        'If myGlobalz.wiedervorlage_MYDB.dbtyp = "oracle" Then
        '    Dim zzz As New clsWiedervorlageDB_CRUD_ORACLE(clsDBspecOracle.getConnection(myGlobalz.wiedervorlage_MYDB))
        '    anzahl% = zzz.delete(Me)
        '       zzz.Dispose
        'End If
        'Return anzahl%
        ''Return DBwvcrud.delete(Me)
    End Function
End Class
