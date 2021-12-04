#Disable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Imports LibDB.LIBDB
#Enable Warning BC40056 ' Namespace or type specified in the Imports 'LibDB.LIBDB' doesn't contain any public member or cannot be found. Make sure the namespace or the type is defined and contains at least one public member. Make sure the imported element name doesn't use any aliases.
Partial Public Class myglobalz

    Public Shared zahlung_MYDB As New clsDatenbankZugriff
    Public Shared bearbeiter_MYDB As New clsDatenbankZugriff
    Public Shared raumbezug_MYDB As New clsDatenbankZugriff
    Public Shared Ereignisse_MYDB As New clsDatenbankZugriff
    Public Shared wiedervorlage_MYDB As New clsDatenbankZugriff
    Public Shared probaug_MYDB As New clsDatenbankZugriff
    Public Shared vorgangsbeteiligte_MYDB As New clsDatenbankZugriff
    Public Shared temp_MYDB As New clsDatenbankZugriff

    Public Shared vorlagen_MYDB As New clsDatenbankZugriff
    Public Shared beteiligte_MYDB As New clsDatenbankZugriff
    'Public Shared halo_MYDB As New clsDatenbankZugriff
    Public Shared webgis_MYDB As New clsDatenbankZugriff
    Public Shared vorgang_MYDB As New clsDatenbankZugriff
    Public Shared alb_MYDB As New clsDatenbankZugriff
    Public Shared kontaktdaten_MYDB As New clsDatenbankZugriff
    Public Shared ARC_MYDB As New clsDatenbankZugriff
    Public Shared postgres_MYDB As New clsDatenbankZugriff
    Public Shared gesetzdb_MYDB As New clsDatenbankZugriff
    Public Shared Property didEverOpenAWordDocInSession As Boolean = False

End Class
