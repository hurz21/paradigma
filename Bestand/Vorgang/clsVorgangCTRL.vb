Imports System.Data

Public Class clsVorgangCTRL
    Public Shared Function leseVorgangvonDBaufObjekt(ByVal vid%, ByVal astamm As Stamm, ByVal myneREC As IDB_grundfunktionen) As Boolean 'myGlobalz.sitzung.VorgangsID	,myGlobalz.sitzung.Vorgang.Stammdaten		,myGlobalz.sitzung.VorgangREC
        If VSTTools.leseAktenzeichen.exe(vid%, myneREC) Then
            VorgangToObj_Aktenzeichen(astamm, myneREC.dt)
            If DB_Oracle_sharedfunctions.leseStammdaten(myneREC, vid%) Then
                ToObj_Stammdaten(astamm, myneREC.dt)
                NSBearbeiter.BearbeiterTools.istUserBekannt()
                Return True
            Else
                nachricht("Stammdaten des Vorgangs existieren nicht! ")
                Return False
            End If
        Else
            nachricht_und_Mbox(String.Format("Ein Vorgang mit der Nummer '{0}' existiert nicht! ", vid))
            Return False
        End If
    End Function

    Public Shared Function ToObj_Stammdaten(ByVal std As Stamm, ByVal vdt As DataTable) As Boolean
        Try
            If Not vdt.IsNothingOrEmpty Then
                std.hauptBearbeiter.Initiale = vdt.Rows(0).Item("Bearbeiter").ToString
                std.Eingangsdatum = clsDBtools.fieldvalueDate(vdt.Rows(0).Item("Eingang"))
                std.Aufnahmedatum = clsDBtools.fieldvalueDate(vdt.Rows(0).Item("Aufnahme"))
                std.Beschreibung = clsDBtools.fieldvalue(vdt.Rows(0).Item("Beschreibung"))
                std.Probaugaz = clsDBtools.fieldvalue(vdt.Rows(0).Item("Probaugaz"))
                std.AltAz = clsDBtools.fieldvalue(vdt.Rows(0).Item("AltAz"))

                Dim tttt As String = clsDBtools.fieldvalue(vdt.Rows(0).Item("DARFNICHTVERNICHTETWERDEN")).ToString

                std.darfNichtVernichtetWerden = CBool(False)
                If String.IsNullOrEmpty(tttt) Then
                    std.darfNichtVernichtetWerden = CBool(False)
                Else
                    If tttt = "0" Then
                        std.darfNichtVernichtetWerden = CBool(False)
                    End If
                    If tttt = "1" Then
                        std.darfNichtVernichtetWerden = CBool(True)
                    End If
                End If


                tttt = clsDBtools.fieldvalue(vdt.Rows(0).Item("ABGABEBA")).ToString

                std.AbgabeBA = CBool(False)
                If String.IsNullOrEmpty(tttt) Then
                    std.AbgabeBA = CBool(False)
                Else
                    If tttt = "0" Then
                        std.AbgabeBA = CBool(False)
                    End If
                    If tttt = "1" Then
                        std.AbgabeBA = CBool(True)
                    End If
                End If
                std.hatraumbezug = CBool((vdt.Rows(0).Item("HATRAUMBEZUG")))

                std.meinGutachten.existiert = CBool((vdt.Rows(0).Item("GUTACHTENMIT")))
                std.meinGutachten.UnterDokumente = CBool((vdt.Rows(0).Item("GUTACHTENDRIN")))
                std.Standort.RaumNr = (clsDBtools.fieldvalue(vdt.Rows(0).Item("STORAUMNR")).ToString)
                std.Standort.Titel = (clsDBtools.fieldvalue(vdt.Rows(0).Item("STOTITEL")).ToString)
                std.Paragraf = (clsDBtools.fieldvalue(vdt.Rows(0).Item("PARAGRAF")).ToString)
                std.InterneNr = (clsDBtools.fieldvalue(vdt.Rows(0).Item("INTERNENR")).ToString)

                std.GemKRZ = clsDBtools.fieldvalue(vdt.Rows(0).Item("GemKRZ"))
                std.LastActionHeroe = clsDBtools.fieldvalue(vdt.Rows(0).Item("LastActionHeroe"))
                std.Bemerkung = clsDBtools.fieldvalue(clsDBtools.fieldvalue(vdt.Rows(0).Item("Bemerkung")))
                std.az.gesamt = vdt.Rows(0).Item("AZ2").ToString
                std.WeitereBearbeiter = vdt.Rows(0).Item("weitereBearb").ToString
                std.ArchivSubdir = vdt.Rows(0).Item("arcdir").ToString
                If String.IsNullOrEmpty(clsDBtools.fieldvalue(vdt.Rows(0).Item("erledigt")).ToString) Then
                    std.erledigt = False
                Else
                    std.erledigt = CBool(vdt.Rows(0).Item("erledigt"))
                End If
                If String.IsNullOrEmpty(clsDBtools.fieldvalue(vdt.Rows(0).Item("LetzteBearbeitung"))) Then
                    'e
                Else
                    std.LetzteBearbeitung = CDate(vdt.Rows(0).Item("LetzteBearbeitung"))
                End If
            Else
                nachricht("Achtung : es konnten keine Stammdaten gefunden werden!")
            End If
        Catch ex As Exception
            nachricht_und_Mbox("Fehler ToObj_Stammdaten:  " & ex.ToString)
        End Try
    End Function

    Public Shared Function VorgangToObj_Aktenzeichen(ByVal std As Stamm, ByVal vdt As DataTable) As Boolean  'myGlobalz.sitzung.Vorgang.Stammdaten		 myGlobalz.sitzung.VorgangREC.dt
        Try
            ' std.az.gesamt = vdt.Rows(0).Item("AZ2").ToString ist jetzt bei den stammdaten
            If String.IsNullOrEmpty(clsDBtools.fieldvalue(vdt.Rows(0).Item("istUNB"))) Then
                std.az.sachgebiet.isUNB = False
            Else
                std.az.sachgebiet.isUNB = CBool(vdt.Rows(0).Item("istUNB"))
            End If
            std.az.Prosa = vdt.Rows(0).Item("vorgangsgegenstand").ToString
            std.az.Vorgangsnummer = CInt(clsDBtools.fieldvalue(vdt.Rows(0).Item("vorgangsnr")))

            If String.IsNullOrEmpty(clsDBtools.fieldvalue(vdt.Rows(0).Item("Sachgebietstext"))) Then
                std.az.sachgebiet.Header = ""
            Else
                std.az.sachgebiet.Header = glob2.klammerraus(CStr(vdt.Rows(0).Item("Sachgebietstext")))
            End If
            Dim a$ = clsDBtools.fieldvalue(vdt.Rows(0).Item("Sachgebietnr"))
            std.az.sachgebiet.Zahl = a$

        Catch ex As Exception
            nachricht_und_Mbox("ToObj_Aktenzeichen " & ex.ToString)
        End Try
    End Function

    Public Shared Sub LoescheVorgang()
        '	speichernAllgemein()
        Dim erfolg As Boolean
        If myGlobalz.sitzung.modus = "edit" Then
            '	If Not glob2.EDIT_VorgangStamm_2DBOk() Then Exit Sub
            erfolg = VSTTools.LoescheStammdaten_alleDB.exe(myGlobalz.sitzung.aktVorgangsID, myGlobalz.sitzung.VorgangREC, myGlobalz.sitzung.aktVorgang.Stammdaten)
            If erfolg Then
                ' MessageBox.Show("Stammdaten  gelöscht")
                erfolg = VSTTools.LoescheVorgang_alleDB.exe(myGlobalz.sitzung.aktVorgangsID)
                If erfolg Then
                    MessageBox.Show("Vorgangsdaten vollständig gelöscht")
                End If
            End If
        Else
            nachricht("Fehler: löschevorgang ist nicht im editmodus: " & myGlobalz.sitzung.modus)
        End If
    End Sub
End Class
