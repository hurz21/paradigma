Imports System.DirectoryServices
Imports System.DirectoryServices.ActiveDirectory
Imports System.Security
Imports System.Data
Namespace JFactiveDirectory

    Public Class clsActiveDir
        Private Shared Function makeListeOfproperties() As List(Of String)
            Dim liste As New List(Of String)
            liste.Add("givenName") 'vorname
            liste.Add("displayName")
            liste.Add("name")
            liste.Add("sn") 'nachname
            liste.Add("cn")
            liste.Add("sAMAccountName")
            liste.Add("userPrincipalName") 'userid
            liste.Add("telephoneNumber")
            liste.Add("physicalDeliveryOfficeName")
            liste.Add("mail")
            liste.Add("company")
            liste.Add("department")
            liste.Add("manager")
            liste.Add("mobile")
            liste.Add("ou")
            liste.Add("streetAddress")
            Return liste
        End Function
        Public Shared Function GetLDAPUserAttributsOBJLIST(ByVal SearchPerson As String,
                                         ByVal LDAPProperties As List(Of String),
                                         ByVal GlobalCatalog As Boolean) As List(Of clsTupelString)
            SearchPerson = "*" & SearchPerson & "*"
            'Dim dt As New DataTable()
            'Dim dr As DataRow
            Dim tupList As New List(Of clsTupelString)
            Dim tup As New clsTupelString
            Dim Searcher As New DirectorySearcher()
            Dim SearchResults As SearchResultCollection = Nothing
            Searcher.Filter = "(&(objectClass=user)(|(displayName=" & SearchPerson & ")(cn=" & SearchPerson & ")(sAMAccountName=" & SearchPerson & ")(department=" & SearchPerson & ")))"
            Searcher.SearchScope = SearchScope.Subtree
            'dt.Columns.Add(New DataColumn("Domain", GetType(String)))
            tup.titel = "Domain"
            For Each Name As String In LDAPProperties
                'dt.Columns.Add(New DataColumn(Name, GetType(String)))
                'tup.Value=
                Searcher.PropertiesToLoad.Add(Name)
            Next
            If GlobalCatalog Then
                Dim d As Domain = System.DirectoryServices.ActiveDirectory.Domain.GetCurrentDomain()
                Dim gc As GlobalCatalog = d.Forest.FindGlobalCatalog()
                Searcher.SearchRoot = New DirectoryEntry("GC://" + gc.Name)
            Else
                Dim adsiRoot As New System.DirectoryServices.DirectoryEntry("LDAP://RootDSE")
                Searcher.SearchRoot = New DirectoryEntry("LDAP://" + (adsiRoot.Properties("defaultNamingContext")(0)).ToString)
            End If
            SearchResults = Searcher.FindAll()
            For Each Result As SearchResult In SearchResults
                'dr = dt.NewRow()
                tup = New clsTupelString
                Dim Domain As String = Result.Path
                Domain = Domain.Substring(Domain.IndexOf("DC="))
                Domain = Domain.Replace("DC=", "")
                Domain = Domain.Replace(",", ".")
                'dr("Domain") = Domain
                tup.titel = "Domain"
                For Each Name As String In LDAPProperties
                    If Result.Properties(Name).Count >= 1 Then
                        'dr(Name) = Result.Properties(Name)(0)
                        tup.Value = CType(Result.Properties(Name)(0), String)
                    End If
                Next
                'dt.Rows.Add(dr)
                tupList.Add(tup)
            Next
            Return tupList
        End Function

        Public Shared Function GetLDAPUserAttributsOLD(ByVal SearchPerson As String,
                                         ByVal LDAPProperties As List(Of String),
                                         ByVal GlobalCatalog As Boolean) As DataTable
            SearchPerson = "*" & SearchPerson & "*"
            Dim dt As New DataTable()
            Dim dr As DataRow
            Dim Searcher As New DirectorySearcher()
            Dim SearchResults As SearchResultCollection = Nothing
            Searcher.Filter = "(&(objectClass=user)(|(displayName=" & SearchPerson & ")(cn=" & SearchPerson & ")(sAMAccountName=" & SearchPerson & ")(department=" & SearchPerson & ")))"
            Searcher.SearchScope = SearchScope.Subtree
            dt.Columns.Add(New DataColumn("Domain", GetType(String)))
            For Each Name As String In LDAPProperties
                dt.Columns.Add(New DataColumn(Name, GetType(String)))
                Searcher.PropertiesToLoad.Add(Name)
            Next
            If GlobalCatalog Then
                Dim d As Domain = System.DirectoryServices.ActiveDirectory.Domain.GetCurrentDomain()
                Dim gc As GlobalCatalog = d.Forest.FindGlobalCatalog()
                Searcher.SearchRoot = New DirectoryEntry("GC://" + gc.Name)
            Else
                Dim adsiRoot As New System.DirectoryServices.DirectoryEntry("LDAP://RootDSE")
                Searcher.SearchRoot = New DirectoryEntry("LDAP://" + (adsiRoot.Properties("defaultNamingContext")(0)).ToString)
            End If
            SearchResults = Searcher.FindAll()
            For Each Result As SearchResult In SearchResults
                dr = dt.NewRow()
                Dim Domain As String = Result.Path
                Domain = Domain.Substring(Domain.IndexOf("DC="))
                Domain = Domain.Replace("DC=", "")
                Domain = Domain.Replace(",", ".")
                dr("Domain") = Domain
                For Each Name As String In LDAPProperties
                    If Result.Properties(Name).Count >= 1 Then
                        dr(Name) = Result.Properties(Name)(0)
                    End If
                Next
                dt.Rows.Add(dr)
            Next
            Return dt
        End Function

        Shared Function suchepersonOBJ(ByVal Name$) As List(Of clsTupelString)
            Dim liste As List(Of String)
            Dim dt As New List(Of clsTupelString)
            Try
                liste = makeListeOfproperties()

                'GetUserMemberOf("kreis-of", "a670024", "snoopy14")
                ' Authenticate("kreis-of", "a670024", "snoopy14")
                ' dt = GetLDAPUserAttributs("a670024", liste, True)
                'dt = GetLDAPUserAttributs("Schöniger", liste, True)
                dt = GetLDAPUserAttributsOBJLIST(Name, liste, True)
            Catch ex As Exception
                Return Nothing
            End Try

            Return dt
        End Function
        Shared Function sucheperson(ByVal Name$) As DataTable
            Dim liste As List(Of String)
            Dim dt As New DataTable
            Try
                liste = makeListeOfproperties()

                'GetUserMemberOf("kreis-of", "a670024", "snoopy14")
                ' Authenticate("kreis-of", "a670024", "snoopy14")
                ' dt = GetLDAPUserAttributs("a670024", liste, True)
                'dt = GetLDAPUserAttributs("Schöniger", liste, True)
                dt = GetLDAPUserAttributsOLD(Name, liste, True)
            Catch ex As Exception
                Return Nothing
            End Try

            Return dt
        End Function
    End Class

    Public Class umweltamt
        Public Shared Function istImUmweltamtOBJ(ByVal lokdt As List(Of clsTupelString), ByRef dep As String) As Integer
            Dim a As String = ""
            Try
                For i = 0 To lokdt.Count - 1
                    'a = CStr(lokdt.Item("department"))
                    If Not a.ToLower.Contains("umwelt") Then
                        dep = a
                        Return -1 'vorhanden aber nicht im UA
                    Else
                        dep = a
                        Return 1
                    End If
                Next
                Return 0 ' nicht vorhanden
            Catch ex As Exception
                'nachricht("Fehler in istImUmweltamt: ", ex)
                Return -2
            End Try
        End Function
        Public Shared Function istImUmweltamt(ByVal lokdt As DataTable, ByRef dep As String) As Integer
            Dim a As String = ""
            Dim retvalue As Integer = 0
            Dim temp As String = ""
            Try
                For j = 0 To lokdt.Rows.Count - 1
                    temp = CStr(lokdt.Rows(j).Item("department"))
                    a = a & temp & ", " & Environment.NewLine
                    If temp.ToLower.Contains("umwelt") Then
                        retvalue = 1
                    End If
                Next
                dep = a.Trim
                Return retvalue ' nicht vorhanden
            Catch ex As Exception
                Return -2
            End Try
        End Function
    End Class
End Namespace

