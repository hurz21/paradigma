Imports System
Imports System.Collections.Generic
Imports System.Text
'Imports System.Text.RegularExpressions

' vCard Reader

Namespace jfcardreader.vCard 'MyProject.vCard
    ''' <summary>
    ''' You may combine HomeWorkType and PhoneType, and FLAG them to reflect the attributes of vCard.
    ''' </summary>
    Public Enum HomeWorkType
        home
        work
    End Enum

    Public Enum PhoneType
        VOICE
        FAX
        MSG
        CELL
        PAGER

    End Enum
    ''' <summary>
    ''' If you flag the enume types, you may use flags.
    ''' </summary>
    Public Structure Phone
        Public number As String
        Public homeWorkType As HomeWorkType
        Public pref As Boolean
        Public phoneType As PhoneType

        Sub clear()
            homeWorkType = vCard.HomeWorkType.work
            phoneType = vCard.PhoneType.CELL
        End Sub

    End Structure

    Public Structure Email
        Public address As String
        Public homeWorkType As HomeWorkType
        Public pref As Boolean
    End Structure

    Public Structure Address
        Public po As String
        Public ext As String
        Public street As String
        Public locality As String
        Public region As String
        Public postcode As String
        Public country As String

        Public homeWorkType As HomeWorkType

        Sub clear()
            po = ""
            ext = ""
            street = ""
            locality = ""
            region = ""
            postcode = ""
            country = ""
            homeWorkType = vCard.HomeWorkType.work
        End Sub

    End Structure

    Public Enum LabelType
        DOM
        INTL
        POSTAL
        PARCEL
    End Enum

    ''' <summary>
    ''' Not used yet. You may use regular expressions or String.Replace() to replace =0D=0A to line breaks.
    ''' </summary>
    Public Structure Label
        Public address As String
        Public labelType As LabelType
    End Structure

    ''' <summary>
    ''' Read text and create data fields of collections.
    ''' </summary>
    Public Class vCardReader

#Region "Singlar Properties"

        Private m_formattedName As String

        Public Property FormattedName() As String
            Get
                Return m_formattedName
            End Get
            Set(ByVal value As String)
                m_formattedName = value
            End Set
        End Property

        Private m_surname As String
        Public Property familyname() As String
            Get
                Return m_surname
            End Get
            Set(ByVal value As String)
                m_surname = value
            End Set
        End Property

        Private m_givenName As String

        Public Property Vorname() As String
            Get
                Return m_givenName
            End Get
            Set(ByVal value As String)
                m_givenName = value
            End Set
        End Property
        Private m_middleName As String

        Public Property MiddleName() As String
            Get
                Return m_middleName
            End Get
            Set(ByVal value As String)
                m_middleName = value
            End Set
        End Property

        Private m_prefix As String

        Public Property Prefix() As String
            Get
                Return m_prefix
            End Get
            Set(ByVal value As String)
                m_prefix = value
            End Set
        End Property
        Private m_suffix As String

        Public Property Suffix() As String
            Get
                Return m_suffix
            End Get
            Set(ByVal value As String)
                m_suffix = value
            End Set
        End Property

        Private m_title As String

        Public Property Title() As String
            Get
                Return m_title
            End Get
            Set(ByVal value As String)
                m_title = value
            End Set
        End Property
        Property URL As String
        Private bday As DateTime

        Public Property Birthday() As DateTime
            Get
                Return bday
            End Get
            Set(ByVal value As DateTime)
                bday = value
            End Set
        End Property

        Private m_rev As DateTime
        ''' <summary>
        ''' If Rev in vCard is UTC, Rev will convert utc to local datetime.
        ''' </summary>
        Public Property Rev() As DateTime
            Get
                Return m_rev
            End Get
            Set(ByVal value As DateTime)
                m_rev = value
            End Set
        End Property

        Private m_org As String

        Public Property Org() As String
            Get
                Return m_org
            End Get
            Set(ByVal value As String)
                m_org = value
            End Set
        End Property

        Private m_note As String

        Public Property Note() As String
            Get
                Return m_note
            End Get
            Set(ByVal value As String)
                m_note = value
            End Set
        End Property



#End Region



        ''' <summary>
        ''' Analyze s into vCard structures.
        ''' </summary>




        Sub getnames(ByVal lines As String(), ByRef family As String, ByRef given As String, ByRef mid As String, ByRef pref As String, ByRef suf As String)
            Dim result As String = ""
            Dim parts, mainparts As String()
            family = "" : given = "" : mid = "" : pref = "" : suf = ""
            Try
                For Each line In lines
                    If line.Trim.ToUpper.StartsWith("N") Then

                        line = line.Replace("N", "").Trim
                        mainparts = line.Split(CChar(":"))
                        parts = mainparts(1).Split(CChar(";"))
                        If parts IsNot Nothing Then
                            If parts.Length > 0 AndAlso Not String.IsNullOrEmpty(parts(0)) Then family = parts(0)
                            If parts.Length > 1 AndAlso Not String.IsNullOrEmpty(parts(1)) Then given = parts(1)
                            If parts.Length > 2 AndAlso Not String.IsNullOrEmpty(parts(2)) Then mid = parts(2)
                            If parts.Length > 3 AndAlso Not String.IsNullOrEmpty(parts(3)) Then pref = parts(3)
                            If parts.Length > 4 AndAlso Not String.IsNullOrEmpty(parts(4)) Then suf = parts(4)
                            Exit Sub
                        End If
                    End If
                Next
            Catch ex As Exception

            End Try
        End Sub

        Sub getformattedname(ByVal lines As String(), ByRef fn As String)
            Dim result As String = ""
            Dim parts As String()
            Try
                For Each line In lines
                    If line.Trim.ToUpper.StartsWith("FN") Then
                        parts = line.Split(CChar(":"))
                        If parts.Length > 1 AndAlso Not String.IsNullOrEmpty(parts(1)) Then fn = parts(1)
                        Exit Sub
                    End If
                Next
            Catch ex As Exception
                fn = ""
            End Try
        End Sub

        Sub getORG(ByVal lines As String(), ByRef p2 As String)
            Dim parts As String()
            Try
                For Each line In lines
                    If line.Trim.ToUpper.StartsWith("ORG") Then
                        parts = line.Split(CChar(":"))
                        If parts.Length > 1 AndAlso Not String.IsNullOrEmpty(parts(1)) Then p2 = parts(1).Trim(CChar(";")).Trim
                        Exit Sub
                    End If
                Next
            Catch ex As Exception
                p2 = ""
            End Try
        End Sub
        Sub getTITLE(ByVal lines As String(), ByRef p2 As String)
            Dim parts As String()
            Try
                For Each line In lines
                    If line.Trim.ToUpper.StartsWith("TITLE") Then
                        parts = line.Split(CChar(":"))
                        If parts.Length > 1 AndAlso Not String.IsNullOrEmpty(parts(1)) Then p2 = parts(1)
                        Exit Sub
                    End If
                Next
            Catch ex As Exception
                p2 = ""
            End Try
        End Sub
        Sub getEMAIL(ByVal lines As String(), ByRef p2 As String)
            Dim parts As String()
            Try
                For Each line In lines
                    If line.Trim.ToUpper.StartsWith("EMAIL") Then
                        parts = line.Split(CChar(":"))
                        If parts.Length > 1 AndAlso Not String.IsNullOrEmpty(parts(1)) Then p2 = parts(1)
                        Exit Sub
                    End If
                Next
            Catch ex As Exception
                p2 = ""
            End Try
        End Sub

        Sub getphones(ByVal lines As String(), ByVal phones As List(Of Phone))
            Dim parts, mainparts As String()
            Dim aktphone As New Phone
            Try
                For Each line In lines
                    If line.Trim.ToUpper.StartsWith("TEL") Then
                        line = line.Replace("TEL:", "").Trim
                        mainparts = line.Split(CChar(":"))
                        If mainparts IsNot Nothing Then
                            'typ festlegen
                            parts = mainparts(0).Split(CChar(";"))
                            If parts.Length > 0 AndAlso Not String.IsNullOrEmpty(parts(1)) Then
                                If parts(1) = "WORK" Then aktphone.homeWorkType = HomeWorkType.work
                                If parts(1) = "HOME" Then aktphone.homeWorkType = HomeWorkType.home
                                If parts(1) = "CELL" Then aktphone.phoneType = PhoneType.CELL
                            End If
                            If parts.Length > 2 AndAlso Not String.IsNullOrEmpty(parts(2)) Then
                                If parts(2) = "VOICE" Then aktphone.phoneType = PhoneType.VOICE
                                If parts(2) = "FAX" Then aktphone.phoneType = PhoneType.FAX
                                If parts(2) = "CELL" Then aktphone.phoneType = PhoneType.CELL
                            End If
                            'Wert festlegen 
                            aktphone.number = mainparts(1)
                            phones.Add(aktphone)
                            aktphone.clear()
                        End If
                    End If
                Next
                Exit Sub
            Catch ex As Exception

            End Try
        End Sub

        Sub getadresses(ByVal lines As String(), ByVal addresses As List(Of Address))
            Dim parts, mainparts As String()
            Dim aktadress As New Address
            Try
                For Each line In lines
                    If line.Trim.ToUpper.StartsWith("ADR") Then
                        line = line.Replace("ADR", "").Trim
                        mainparts = line.Split(CChar(":"))
                        If mainparts IsNot Nothing Then
                            'typ festlegen
                            parts = mainparts(0).Split(CChar(";"))
                            If parts.Length > 0 AndAlso Not String.IsNullOrEmpty(parts(1)) Then
                                If parts(1) = "WORK" Then aktadress.homeWorkType = HomeWorkType.work
                                If parts(1) = "HOME" Then aktadress.homeWorkType = HomeWorkType.home
                            End If
                            'Wert festlegen 
                            parts = mainparts(1).Split(CChar(";"))
                            If parts.Length > 0 AndAlso Not String.IsNullOrEmpty(parts(0)) Then aktadress.po = parts(0)
                            If parts.Length > 1 AndAlso Not String.IsNullOrEmpty(parts(1)) Then aktadress.ext = parts(1)
                            If parts.Length > 2 AndAlso Not String.IsNullOrEmpty(parts(2)) Then aktadress.street = parts(2)
                            If parts.Length > 3 AndAlso Not String.IsNullOrEmpty(parts(3)) Then aktadress.locality = parts(3)
                            If parts.Length > 4 AndAlso Not String.IsNullOrEmpty(parts(4)) Then aktadress.region = parts(4)
                            If parts.Length > 5 AndAlso Not String.IsNullOrEmpty(parts(5)) Then aktadress.po = parts(5)

                            addresses.Add(aktadress)
                            aktadress.clear()
                        End If
                    End If
                Next
                Exit Sub
            Catch ex As Exception

            End Try

        End Sub

        Function zeilen_zusammnenfuegen(ByRef lines As String(), ByVal fortsetzung As String) As String()
            Try
                For i = 0 To lines.GetUpperBound(0)
                    If lines(i).EndsWith(fortsetzung) Then
                        lines(i) = lines(i).Replace(fortsetzung, "")
                        lines(i) = lines(i) & lines(i + 1)
                        lines(i + 1) = " "
                    End If
                Next
                Return lines
            Catch ex As Exception
                lines = Nothing
                Return Nothing
            End Try
        End Function


        Sub getNote(ByVal lines As String(), ByRef note As String)
            Dim parts As String()
            Try
                For Each line In lines
                    If line.Trim.ToUpper.StartsWith("NOTE") Then
                        parts = line.Split(CChar(":"))
                        If parts.Length > 1 AndAlso Not String.IsNullOrEmpty(parts(1)) Then note = parts(1)
                        Exit Sub
                    End If
                Next
            Catch ex As Exception
                note = ""
            End Try
        End Sub

        Sub getURL(ByVal lines As String(), ByRef url As String)
            Dim parts As String()
            Try
                For Each line In lines
                    If line.Trim.ToUpper.StartsWith("URL") Then
                        parts = line.Split(CChar(":"))
                        If parts.Length > 1 AndAlso Not String.IsNullOrEmpty(parts(1)) Then url = parts(1) & ":" & parts(2)
                        Exit Sub
                    End If
                Next
            Catch ex As Exception
                url = ""
            End Try
        End Sub


    End Class
End Namespace

