Imports System.Text

Public Class mapgeneratortools
    'Shared Sub l(text As String)
    '    My.Log.WriteEntry(text)
    'End Sub
    Shared Sub makeMapFile(ByVal inTemplateMapfile As String,
                        ByVal outKartenMAPfile As String,
                        ByVal KartenEbenenName As String,
                        ByVal mitetikett As Boolean,
                        enc As System.Text.Encoding,
                        GISusername As String)
        l("makeMapFile -----------------------------------------------")
        l(" templateMapfile$: " & inTemplateMapfile)
        l(" KartenMAPfile$$: " & outKartenMAPfile)
        Dim tempsafe As String
        If IO.File.Exists(inTemplateMapfile) Then
            l("Vorlage existiert")
            Using selVorlage As New IO.StreamReader(inTemplateMapfile, enc)
                tempsafe = selVorlage.ReadToEnd
                tempsafe = tempsafe.Replace("[FEATURECLASS]", KartenEbenenName)
                tempsafe = tempsafe.Replace("[SHAPEFILELOCATIONDIR]", "/paradigmacache/" & GISusername)
                If Not mitetikett Then
                    tempsafe = tempsafe.Replace("Labelitem 'RBTITEL'#beipoint", "Labelitem 'RBTYP'")
                End If
            End Using
            My.Computer.FileSystem.WriteAllText(outKartenMAPfile, tempsafe, False, enc)
            l("Mapfile$ wurde erzeugt: " & KartenEbenenName)
        Else
            l("FEHLER: Vorlage exitiert nicht")
        End If
    End Sub
    Shared Sub makeMapFilePostgis(ByVal inTemplateMapfile As String,
                    ByVal outKartenMAPfile As String,
                    ByVal KartenEbenenName As String,
                    ByVal mitetikett As Boolean,
                    enc As System.Text.Encoding,
                    tableName As String,
                                  userlayeraidNKATDIR As String)
        l("makeMapFile -----------------------------------------------")
        l(" inTemplateMapfile: " & inTemplateMapfile)
        l(" outKartenMAPfile: " & outKartenMAPfile)
        l(" tableName: " & tableName)
        l(" KartenEbenenName: " & KartenEbenenName)
        l(" userlayeraidNKATDIR: " & userlayeraidNKATDIR)
        Dim tempsafe, neusave As String
        Try
            If IO.File.Exists(inTemplateMapfile) Then
                l("Vorlage existiert")
                Using selVorlage As New IO.StreamReader(inTemplateMapfile, enc)
                    tempsafe = selVorlage.ReadToEnd
                    neusave = tempsafe
                    tempsafe = tempsafe.Replace("[FEATURECLASS]", KartenEbenenName)

                    tempsafe = tempsafe.Replace("[PG_SCHEMA.TABELLE]", "paradigma_userdata." & tableName.ToLower)
                    tempsafe = tempsafe.Replace("[TABLENAME]", tableName.Trim.ToLower)
                    If Not mitetikett Then
                        tempsafe = tempsafe.Replace("Labelitem 'RBTITEL'#beipoint", "Labelitem 'RBTYP'")
                    End If
                End Using
                My.Computer.FileSystem.WriteAllText(outKartenMAPfile, tempsafe, False, enc)

                neusave = tempsafe.Replace("Imagemapmaxscale", "#Imagemapmaxscale")
                neusave = neusave.Replace("Imagemap", "#Imagemap")
                neusave = neusave.Replace("#Template", " Template ")
                neusave = neusave.Replace("#Header", " Header ")
                neusave = neusave.Replace("#Footer", " Footer ")
                My.Computer.FileSystem.WriteAllText(userlayeraidNKATDIR & "layer.map", neusave, False, enc)

                l("Mapfile$ wurde erzeugt: " & KartenEbenenName)
            Else
                l("FEHLER: Vorlage exitiert nicht")
            End If
        Catch ex As Exception
            l("fehler in makeMapFilePostgis " ,ex)
        End Try
    End Sub

    '    Friend Shared Sub makeDBTemplateFilePostgis(inTemplateMapfile As String, outKartenMAPfile As String, kartenEbenenName As String,
    '                                                mitetikett As Boolean, enc As Encoding, tableName As String,
    '                                                userlayeraidNKATDIR As String)
    '        l("makeDBTemplateFilePostgis -----------------------------------------------")
    '        l(" templateMapfile$: " & inTemplateMapfile)
    '        l(" KartenMAPfile$$: " & outKartenMAPfile)
    '#Disable Warning BC42024 ' Unused local variable: 'tempsafe'.
    '#Disable Warning BC42024 ' Unused local variable: 'neusave'.
    '        Dim tempsafe, neusave As String
    '#Enable Warning BC42024 ' Unused local variable: 'neusave'.
    '#Enable Warning BC42024 ' Unused local variable: 'tempsafe'.
    '        inTemplateMapfile = inTemplateMapfile.Replace("/", "\")
    '        outKartenMAPfile = outKartenMAPfile.Replace("/", "\")

    '        inTemplateMapfile = inTemplateMapfile.Replace("raumbezug.map", "raumbezug_templ.htm")
    '        outKartenMAPfile = userlayeraidNKATDIR & "raumbezug_templ.htm"
    '        ausschreiben(inTemplateMapfile, outKartenMAPfile)

    '        inTemplateMapfile = inTemplateMapfile.Replace("raumbezug_templ.htm", "raumbezug_circle_templ.htm")
    '        outKartenMAPfile = userlayeraidNKATDIR & "raumbezug_circle_templ.htm"
    '        ausschreiben(inTemplateMapfile, outKartenMAPfile)

    '        inTemplateMapfile = inTemplateMapfile.Replace("raumbezug_circle_templ.htm", "raumbezug_line_templ.htm")
    '        outKartenMAPfile = userlayeraidNKATDIR & "raumbezug_line_templ.htm"
    '        ausschreiben(inTemplateMapfile, outKartenMAPfile)
    '    End Sub

    'Private Shared Sub ausschreiben(inTemplateMapfile As String, outKartenMAPfile As String)
    '    Try
    '        Dim fi As New IO.FileInfo(inTemplateMapfile)
    '        If fi.Exists Then
    '            l("Vorlage existiert")
    '            fi.CopyTo(outKartenMAPfile, True)
    '            l(outKartenMAPfile & "  erzeugt")

    '        Else
    '            l("FEHLER: Vorlage exitiert nicht")
    '        End If
    '    Catch ex As Exception
    '        l("fehler in makeDBTemplateFilePostgis " ,ex)
    '    End Try
    'End Sub
End Class
