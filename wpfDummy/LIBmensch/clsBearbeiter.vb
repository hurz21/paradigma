Imports System.Text
Public Class clsBearbeiter
    Inherits Person
    Implements ICloneable

    Private _username As String
    Property gruppentext As String = ""
    Property istausgewaehlt As Boolean = False
    Property istOnline As Boolean = False
    Property ipadresse As String = ""
    Property ImageFilePath As String = ""

    Public Property ID As Integer
    Public Function getString(trenner As String) As String
        'Dim summe As New Text.StringBuilder
        Dim summe As New StringBuilder
        Try
            summe.Append("Name: " & Name & trenner)
            summe.Append("Vorname: " & Vorname & trenner)
            summe.Append("username: " & username & trenner)
            summe.Append("Raum: " & Raum & trenner)
            summe.Append("Tel: " & Kontakt.elektr.Telefon1 & trenner)
            summe.Append("Fax: " & Kontakt.elektr.Fax1 & trenner)
            summe.Append("Kuerzel: " & Kuerzel2Stellig & trenner)
            summe.Append("Abteilung: " & Bemerkung & trenner)
            summe.Append("Rolle: " & Rolle & trenner)
            summe.Append("Rang: " & Rang & trenner)
            summe.Append("Explorer: " & ExpandHeaderInSachgebiet & trenner)
            summe.Append("aktiv?: " & Status & trenner)
            summe.Append("Initial: " & Initiale & trenner)
            summe.Append("bearbeiterid: " & ID & trenner)
            summe.Append("personenid: " & PersonenID & trenner)
            Return summe.ToString

        Catch ex As Exception
            Return "fehler: " & ex.ToString
        End Try
    End Function
    Public Property username() As String
        Get
            Return _username
        End Get
        Set(ByVal Value As String)
            _username = Value
            OnPropertyChanged("username")
        End Set
    End Property


    Public Property GISPassword() As String


    Private _defaultKontakt As New Kontaktdaten
    Public Property defaultKontakt() As Kontaktdaten
        Get
            Return _defaultKontakt
        End Get
        Set(ByVal Value As Kontaktdaten)
            _defaultKontakt = Value
        End Set
    End Property
    Public Property Rang() As String
    ''' <summary>
    ''' legt fest welche gruppen das standardmaessig auf die vorgaenge zugreifen duerfen
    ''' </summary>
    ''' <remarks></remarks>
    Private _STDGRANTS As String = ""
    Public Property STDGRANTS() As String
        Get
            Return _STDGRANTS
        End Get
        Set(ByVal value As String)
            _STDGRANTS = value
            OnPropertyChanged("STDGRANTS")
        End Set
    End Property


    Private _initiale As String = ""
    Public Property Initiale() As String
        Get
            Return _initiale
        End Get
        Set(ByVal Value As String)
            _initiale = Value
            OnPropertyChanged("Initiale")
        End Set
    End Property

    Public Function getInitial() As String
        If String.IsNullOrEmpty(Name) Then
            Name = "???"
        End If
        If String.IsNullOrEmpty(Vorname) Then
            Vorname = "???"
        End If
        Try
            Initiale = (Name.Substring(0, 3) & Vorname.Substring(0, 1)).ToLower
            Return Initiale
        Catch ex As Exception
            Return "???"
        End Try
    End Function
    ''' <summary>
    ''' raum des bearbeiters
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Property Raum() As String = ""
    Property Kuerzel2Stellig As String
    Property tooltip As String = ""
    Sub buildtooltip()
        tooltip = "Tel.: " & Kontakt.elektr.Telefon1 & Environment.NewLine &
                Name & ", " & Vorname & Environment.NewLine &
                  Kontakt.elektr.Email & ", " & Environment.NewLine & Raum
    End Sub

    Sub bearbeiterclear()
        username = ""
        defaultKontakt.clear()
        Initiale = ""
        Raum = ""
        STDGRANTS = ""
        Kuerzel2Stellig = ""
        ID = 0
    End Sub

    Public Function Clone() As Object Implements System.ICloneable.Clone
        Return MemberwiseClone()
    End Function
    Public Function istUser_admin_oder_vorzimmer() As Boolean '
        If Rang.ToLower = "admin" Or
           Rang.ToLower = "vorzimmer" Then
            l("Vorzimmer Zugriff erteilt für user: " & Initiale)
            Return True
        Else
            l("Vorzimmer Zugriff NICHT erteilt für user: " & Initiale)
            Return False
        End If
    End Function
    Public Function binEignerExtracted(hauptBearbeiter As clsBearbeiter) As Boolean 'myGlobalz.sitzung.aktVorgang.Stammdaten.hauptBearbeiter
        Dim binEigner As Boolean
        If String.IsNullOrEmpty(hauptBearbeiter.Initiale) Then
            Return True
        End If
        binEigner = (hauptBearbeiter.ID = ID)


        'binEigner = (Initiale.ToLower.Trim = hauptBearbeiter.Initiale.ToLower.Trim) Or
        '            (Kuerzel2Stellig.ToLower.Trim = hauptBearbeiter.Initiale.ToLower.Trim)
        Return binEigner
    End Function

    Public Function binWeitererBearbeiter(hauptBearbeiter As clsBearbeiter, weitererBearbeiter As String) As Boolean
        Dim binWeiterer As Boolean
        binWeiterer = LIBgemeinsames.clsString.textEnthaelt(hauptBearbeiter.Initiale.ToLower, Initiale.ToLower) Or
                    LIBgemeinsames.clsString.textEnthaelt(weitererBearbeiter.ToLower, Kuerzel2Stellig.ToLower) Or
                    LIBgemeinsames.clsString.textEnthaelt(weitererBearbeiter.ToLower, Initiale.ToLower)
        Return binWeiterer
    End Function
    Public Function binEignerOderAdmin(hauptBearbeiter As clsBearbeiter, weitereBearbeiter As String) As Boolean
        If binEignerExtracted(hauptBearbeiter) Or binWeitererBearbeiter(hauptBearbeiter, weitereBearbeiter) Then
            l("Zugriff erteilt für User: " & Initiale)
            Return True
        Else
            Return istUser_admin_oder_vorzimmer()
        End If
    End Function
End Class
