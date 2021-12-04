Imports System.Data
Imports System.Collections

Public Class myGlobalz
    Public Shared Paradigma_root$ = String.Empty
    Public Shared Paradigma_Sachgebietsdatei$
    Public Shared MeinNULLDatumAlsDate As Date
    Public Shared logdir$
    Public Shared MeinNULLDatumDatumAlsString$
    Public Shared radiusAdresse, radiusFlst, radiusFoto As Integer
    Public Shared ServerNetworkShare$
    Public Shared ServerHTTPdomainIntranet$
    Public Shared OhneObsoletenDokus As Boolean = False
    ' Public Shared mylog As clsLogging
    Public Shared ereignisTypen As ArrayList
    Public Shared supporterListe As ArrayList
    Public Shared ZIELLOGFILE$

    Public Shared enc As System.Text.Encoding = System.Text.Encoding.GetEncoding("iso-8859-1")
    Public Shared weyers$ = "WeyG"
    Public Shared DokumenteRoot$
    Public Shared Birdsviewpfad$
    Public Shared TransferString$   'für den datenaustausch zwischen fenstern
    Public Shared callmodus$ = "neu"
    Public Shared bearbeiterDT As DataTable
    Public Shared gebaeudeListe As New List(Of clsGebaeude)
    Public Shared winkarte As New WindowKarte
    Shared Sub nachricht(ByVal text$)
        My.Log.WriteEntry(text$)
    End Sub
    Public Shared Property callREC() As liboracle.
    Private Shared _dokuREC As New callmanagerDAL.clsDBspecOracle
    Public Shared Property dokuREC() As callmanagerDAL.clsDBspecOracle
        Get
            Return _dokuREC
        End Get
        Set(ByVal Value As callmanagerDAL.clsDBspecOracle)
            _dokuREC = Value
        End Set
    End Property
    Public Shared Property raumdbREC As callmanagerDAL.clsDBspecOracle
    Public Shared Property personenRec As callmanagerDAL.clsDBspecOracle
    Public Shared Property tempREC As callmanagerDAL.clsDBspecOracle
    Public Shared Property aktCall As clsCallStammdaten
    Public Shared Property LoesungsRec As New callmanagerDAL.clsDBspecOracle
    ''' <summary>
    ''' der vor dem bildschirm sitzt
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Property aktBearbeiter As clsBearbeiter
    Public Shared Property aktPerson As clsPerson
    Public Shared Property aktEreignis As clsEreignis
    Public Shared Property aktExperte As clsExperte
End Class
