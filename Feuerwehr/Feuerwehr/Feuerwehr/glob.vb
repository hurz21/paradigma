Imports System.Data
Imports System.Text

Public Class glob
	Public Shared StandortDBREC As clsDBspecMDB = GetPlanDBREC()
	Public Shared haloDBREC As New clsDBspecMDB
	Public Shared zielTab As DataTable
	Public Shared pdfStammroot$
	Public Shared gemeindeText$, gemeindeKuerzel$
	Public Shared ServerHTTPdomainIntranet$
	Public Shared mylog As New clsLogging("c:\Feuerwehr.log")
	Public Shared MedienInitialDir$

	Public Shared Function GetPlanDBREC() As clsDBspecMDB
		Dim planDBREC As New clsDBspecMDB
		Return planDBREC
	End Function
	Public hinweis$ = StandortDBREC.getDataDT()
#Region "Initialisierung"
	Private Shared Function InifileVorhanden(ByVal inifile$) As Boolean
		Dim test As New IO.FileInfo(inifile)
		If Not test.Exists Then
			MsgBox("Kann die Inidatei nicht finden: " & inifile)
			Return False
		End If
		Return True
	End Function

	Private Shared Function StandortDBvorhanden() As Boolean
		Dim test = New IO.FileInfo(StandortDBREC.mydb.Schema)
		If Not test.Exists Then
			MsgBox("Kann die Standort-Datenbank nicht finden: " & (StandortDBREC.mydb.Schema))
			Return False
		End If
		Return True
	End Function

	Private Shared Function AdressDBvorhanden() As Boolean
		Dim test As New IO.FileInfo(haloDBREC.mydb.Schema)
		If Not test.Exists Then
			MsgBox("Kann die Adress-Datenbank nicht finden: " & (haloDBREC.mydb.Schema) & vbCrLf & _
			"Bitte wenden Sie sich an den Hersteller: j.feinen@gmx.net" & vbCrLf & "Es werden keine automatisierten Koordinatenvorschläge gestellt werden können !!!")
			Return False
		End If
		Return True
	End Function

	Public Shared Function KONFIGURATOR() As Boolean
		Try
			IO.Directory.CreateDirectory(System.Environment.GetEnvironmentVariable("APPDATA") & "\feuerwehr")
            Dim inifile$ = System.Environment.GetEnvironmentVariable("APPDATA") & "\feuerwehr\feuerwehr.xml"
            inifile$ = "c:\appsconfig\feuerwehr.xml"
			If Not InifileVorhanden(inifile) Then Return False
			Dim xxx As New clsINIXML(inifile$)
			Dim iniDict As Dictionary(Of String, String) = xxx.XMLiniReader(inifile$)

			StandortDBREC.mydb.MySQLServer = CType(iniDict("StandortDBREC.mysqlserver"), String)
			StandortDBREC.mydb.Schema = CType(iniDict("StandortDBREC.schema"), String)
			StandortDBREC.mydb.Tabelle = CType(iniDict("StandortDBREC.Tabelle"), String)
			StandortDBREC.mydb.dbtyp = CType(iniDict("StandortDBREC.dbtyp"), String)

			If Not StandortDBvorhanden() Then Return False

			haloDBREC.mydb.MySQLServer = CType(iniDict("haloDBREC.mysqlserver"), String)
			haloDBREC.mydb.Schema = CType(iniDict("haloDBREC.schema"), String)
			haloDBREC.mydb.Tabelle = CType(iniDict("haloDBREC.Tabelle"), String)
			haloDBREC.mydb.dbtyp = CType(iniDict("haloDBREC.dbtyp"), String)

			AdressDBvorhanden()

			pdfStammroot$ = CType(iniDict("Div.pdfStammroot"), String)
			ServerHTTPdomainIntranet = CType(iniDict("Div.ServerHTTPdomainIntranet"), String)
			MedienInitialDir = CStr(System.Environment.SpecialFolder.MyPictures) '()' "c:\eigene Dateien\eigene Bilder\"
			Return True
		Catch ex As Exception
			MsgBox("Fehler beim Initialisieren: " & vbCrLf & ex.ToString)
			Return False
		End Try
	End Function
#End Region

	Public Shared Sub halodtFuellen(ByVal sql$)
		haloDBREC.mydb.SQL = sql '"select * from " & planDBREC.mydb.Tabelle
		Dim hinweis = haloDBREC.getDataDT()
	End Sub

	Public Shared Sub dtFuellen(ByVal sql$)
		StandortDBREC.mydb.SQL = sql '"select * from " & planDBREC.mydb.Tabelle
		Dim hinweis = StandortDBREC.getDataDT()
	End Sub

	''' <summary>
	''' Öffnet das Dokument mit der im System festgelegten Standard-Anwendung
	''' </summary>
	''' <param name="DocumentFile">Dokument-Dateiname</param>
	''' <returns>True, wenn das Dokument geöffnet werden konnte, andernfalls False.</returns>
	Public Shared Function OpenDocument(ByVal DocumentFile As String) As Boolean
		Try
			'	glob2.nachricht("OpenDocument DocumentFile:" & vbCrLf & DocumentFile)
			Dim pInfo As New Diagnostics.ProcessStartInfo
			Dim test As New IO.FileInfo(DocumentFile)
			If Not test.Exists Then
				MessageBox.Show("Die Datei existiert nicht im _Archiv." & test.Name)
				'		glob2.nachricht_an_admin("FEHLER:	 Die Datei existiert nicht im _Archiv: " & test.FullName)
				Return False
			End If
			With pInfo
				' Dokument	
				.FileName = DocumentFile
				' verknüpfte Anwendung starten
				.Verb = "open"
			End With
			Process.Start(pInfo)
			'	glob2.nachricht("OpenDocument erfolgreich: ")
			Return True
		Catch ex As Exception
			'	glob2.nachricht("OpenDocument FEHLER: " & vbCrLf & ex.ToString)
			Return False
		End Try
	End Function



	Public Shared Sub editStandort(ByVal stammID%)
		Dim st As New winStandortDetail(stammID%)
		st.ShowDialog()
		'glob.dtFuellen("select * from medien where stammid=" & stammID%)
		'dgMeien.DataContext = glob.planDBREC.dt
	End Sub
	Shared Sub zeigeStandortImBgis(ByVal aktStandort As clsStandortPlan)
		GISAufruf_Mittelpunkt(aktStandort.pt)
	End Sub
	Public Shared Sub GISAufruf_Mittelpunkt(ByVal pt As myPoint)
		If pt.X < 10000 Then
			MessageBox.Show("Es konnten keine brauchbaren Koordinaten gefunden werden!", "GIS", MessageBoxButton.OK, MessageBoxImage.Error)
			Exit Sub
		End If
		Dim radius$ = "200"

        Dim http$ = ServerHTTPdomainIntranet$ & "/cgi-bin/suchdb.cgi?modus=42" & _
         "&rechts=" & CInt(pt.X) & _
         "&hoch=" & CInt(pt.Y) & _
         "&abstand=" & radius$ & _
         "&username=" & "Werner_R" & _
         "&password=" & "2483e14219cce6fe63d8ac91afc92618" & _
         "&thema=" & "stadtplan;brandschutz;"   '& _
		'"&format=fix800x600"
		'OpenDocument(http)
		Process.Start(New ProcessStartInfo(http$))
	End Sub

	Shared Function googlemaps(ByVal item As DataRowView) As String
		Try
			Dim gemeinde$ = CStr((item("ort")))
			Dim strasse$ = CStr((item("strasse")))
			Dim hausnr$ = CStr((item("hausnr")))
			Dim strassenr$ = strasse$ & "+" & hausnr
			Dim gm As New clsGoogleMein
			Dim queryAddress$ = gm.Googleadress(strassenr$, gemeinde, "", "")
			Return queryAddress
		Catch ex As Exception
			Return ""
		End Try
	End Function

	Public Shared Sub nachricht(ByVal text$)
		glob.mylog.log(text$)
	End Sub

	Public Shared Sub nachricht_und_Mbox(ByVal text$)
		glob.mylog.log(text$)
		MessageBox.Show(text)
	End Sub
	Shared Function Medienauswaehlen(ByRef filenames$(), ByVal initalDir$) As Boolean
		Dim ofd As New Microsoft.Win32.OpenFileDialog() With {.Title = "Bitte wählen Sie die Dateien aus!", _
		 .InitialDirectory = initalDir$, _
		 .Multiselect = True}
		If Not ofd.ShowDialog Then
			glob.nachricht_und_Mbox("Es wurde keine Auswahl getroffen!")
			Return False
		End If
		Dim message As String = "Sind Sie sicher, daß die ausgewählten Dokumente dem Standort " & vbCrLf & _
		 "		Nr: " & aktStandort.StammID & vbCrLf & _
		 "zugeordnet werden sollen ? " & vbCrLf
		Dim red As MessageBoxResult = MessageBox.Show(message, "Hinweis", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes)
		If red = MessageBoxResult.Yes Then
			filenames = ofd.FileNames
			Return True
		Else
			Return False
		End If
	End Function


	Shared Function medienHinzufuegen(ByVal filenames$()) As Boolean
        Dim iii%
		Try
			For Each medium As String In filenames
				'relativPfad bestimmen
				aktMedium.Relativpfad = Relativen_PFad_bestimmen()
				Dim gesamtpfad$ = clsString.Normalize_Slashes(glob.StandortDBREC.mydb.MySQLServer & "\" & pdfStammroot$ & "\" & aktMedium.Relativpfad)

				If String.IsNullOrEmpty(aktMedium.Relativpfad) Then
					glob.nachricht_und_Mbox("Fehler: Relativer Pfad konnte nicht bestimmt werden!")
					Return False
				Else
                    If Relativen_PFad_anlegen(gesamtpfad$) Then
                        'filename_normieren
                        If String.IsNullOrEmpty(medium) Then
                            glob.nachricht_und_Mbox("Fehler: Dateiname isdt unbrauchbar: " & medium)
                            Return False
                        Else
                            Dim quelle As New IO.FileInfo(medium)
                            Dim kopiert As Boolean
                            aktMedium.Dateiname = clsString.normalize_Filename(quelle.Name)
                            aktMedium.Dateiname = clsString.Normalize_Slashes(aktMedium.Dateiname)
                            'Datei kopieren
                            aktMedium.Archiv_FullName = clsString.Normalize_Slashes(gesamtpfad$ & "\" & aktMedium.Dateiname)
                            If quelle.FullName.ToLower <> aktMedium.Archiv_FullName.ToLower Then
                                iii% = 5
                                If quelle.Exists Then
                                    quelle.CopyTo(aktMedium.Archiv_FullName, True)
                                Else
                                    quelle.CopyTo(aktMedium.Archiv_FullName, True)
                                End If
                            End If
                            'datei in DB neu aufnahmen
                            aktMedium.StammID = aktStandort.StammID
                            aktMedium.Titel = aktMedium.Archiv_FullName
                            iii = 6
                            checkinDb(0)
                        End If
                    End If
				End If
			Next
			Return True
		Catch ex As Exception
            glob.nachricht_und_Mbox(iii & " Fehler : " & ex.ToString)
			Return False
		End Try
	End Function

	Shared Function Relativen_PFad_bestimmen() As String
		Dim prob$ = _
		 clsString.normalize_Filename(aktStandort.adr.Gemeindename) & "\" & _
		 clsString.normalize_Filename(aktStandort.adr.Strassenname) & "_" & _
		 clsString.normalize_Filename(aktStandort.adr.HausnrKombi)
		prob = clsString.Normalize_Slashes(prob)
		Return prob
	End Function

	Shared Function Relativen_PFad_anlegen(ByRef prob$) As Boolean
		Try
			If Not IO.Directory.Exists(prob$) Then
				IO.Directory.CreateDirectory(prob$)
			End If
			Return True
		Catch ex As Exception
            glob.nachricht_und_Mbox(prob$ & vbCrLf & ", Fehler in Relativen_PFad_anlegen: " & ex.ToString)
			Return False
		End Try
	End Function
	Shared Function checkinDb(ByVal stammid%) As Boolean
		aktMedium.speichern(stammid)
	End Function

	Public Shared Function killDocument(ByVal id%, ByVal medienDatei$) As Boolean
		aktMedium.clear()
		aktMedium.ID = id
		aktMedium.Archiv_FullName = medienDatei
		If aktMedium.loeschen(aktMedium.ID) > 0 Then
			MessageBox.Show("Die Datei wurde aus der Datenbank entfernt." & vbCrLf & _
			 "Die Datei selber bleibt aber erhalten und liegt weiterhin unter: " & vbCrLf & vbCrLf & _
			 aktMedium.Archiv_FullName & vbCrLf & vbCrLf & _
			 "Bitte entfernen Sie die Datei von Hand.", "Löschen von Medien aus der Datenbank", MessageBoxButton.OK, MessageBoxImage.Exclamation)
		Else
			MessageBox.Show("Datei konnte nicht aus der DB entfernt werden. ")
		End If
	End Function
	Public Shared Sub istTextzulang(ByVal maxlen%, ByVal tb As TextBox)
		Try
			If tb Is Nothing Then Exit Sub
			If tb.Text.Length > maxlen% Then
				MessageBox.Show("Der Text ist zu lang: " & vbCrLf & _
				 tb.Text.Length & " statt maximal " & maxlen & " Zeichen." & vbCrLf _
				 & "Der Text wird am Ende abgeschnitten!", "Eingabe zu lang", MessageBoxButton.OK, MessageBoxImage.Exclamation, MessageBoxResult.OK)
				tb.Text = tb.Text.Substring(0, maxlen - 1)
			End If
		Catch ex As Exception
			glob.nachricht_und_Mbox(ex.ToString)
		End Try
	End Sub
	Public Shared Function pruefeObZahl(ByVal cnt As TextBox) As Boolean
		Try
			If Not IsNumeric(cnt.Text) Then
				MessageBox.Show("Es ist hier eine Zahl gefordert. Texte sind ungültig!")
				Return False
			Else
				Return True
			End If
		Catch ex As Exception
			glob.nachricht_und_Mbox(ex.ToString)
		End Try
	End Function
	Shared Sub schliessenButton_einschalten(ByVal btn As Button)
		If Not btn Is Nothing Then
			btn.IsEnabled = True
			btn.Visibility = Visibility.Visible
		End If
	End Sub
End Class
