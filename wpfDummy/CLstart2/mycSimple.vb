Imports System.IO
Namespace CLstart
    Public Class mycSimple

        Public Shared enc As System.Text.Encoding = System.Text.Encoding.GetEncoding("iso-8859-1")
        Public Shared MeinNULLDatumAlsDate As Date = CDate("0001-01-01 01:01:01")
        Public Shared MeinNULLDatumDatumAlsString As String = "0001-01-01 01:01:01"
        Public Shared Property outlookAnzeigen As Boolean = False  'die application wird nicht angezeigt
        Public Shared Paradigma_local_root As String = String.Empty
        Public Shared iniDict As New Dictionary(Of String, String)
        Public Shared ParadigmaVersion As String
        Public Shared wordDocWatcher As FileSystemWatcher
        Public Shared excelDocWatcher As FileSystemWatcher

        Shared Sub neuerVorgang3(modus As String) '"normal, kfa
            Dim si As New ProcessStartInfo
            'si.FileName = initP.getValue("ExterneAnwendungen.Application_Stakeholder")
            si.FileName = "C:\kreisoffenbach\paradigmaNeuerVorgang\paradigmaNeuerVorgang.exe "
            si.WorkingDirectory = "C:\kreisoffenbach\paradigmaNeuerVorgang"
            si.Arguments = modus
            Process.Start(si)
            si = Nothing
        End Sub

        Shared Function getDokArcPfad() As String
            Return CType(iniDict("Myglobalz.dokArcPfad"), String)
        End Function

        Shared Function getServerHTTPdomainIntranet() As String
            Return CType(iniDict("GisServer.ServerHTTPdomainIntranet"), String)
        End Function

        Shared Function getparadigmaDateiServerRoot() As String

            Return CType(iniDict("Haupt.paradigmaDateiServerRoot"), String) 'noch unbenutzt
        End Function


        Shared Function getParadigma_checkout() As String
            Return CStr(Environment.GetFolderPath((System.Environment.SpecialFolder.DesktopDirectory)).ToString &
                                    CType(iniDict("Myglobalz.Paradigma_checkout"), String)) ' "\Paradigma\Archiv_Checkout\"
        End Function

        Shared Function getParadigma_archiv_temp() As String
            Return Environment.GetFolderPath((System.Environment.SpecialFolder.DesktopDirectory)).ToString &
                                    CType(iniDict("Myglobalz.paradigma_archiv_temp"), String) '"\Paradigma\Archiv_temp\"
        End Function

        Shared Function getModuleParadigmaDetail() As String

            'Return CType(iniDict("ExterneAnwendungen.APPLICATION_ParadigmaDetailTEST"), String)
            Return CType(iniDict("ExterneAnwendungen.APPLICATION_ParadigmaDetail"), String)

        End Function

        Shared Sub startbplankataster()
            Dim handle As Process
            Try
                l(" startbplankataster ---------------------- anfang")

                Dim bat = "\\gis\gdvell\apps\bplankat\bplanupdate.bat"
                handle = Process.Start(bat)
                l(" startbplankataster ---------------------- ende")
            Catch ex As Exception
                l("Fehler in startbplankataster: " ,ex)
            End Try
        End Sub
    End Class
End Namespace