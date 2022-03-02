Imports System.IO

Public Class Form1

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles btnEditFacialDir.Click
        FBD.SelectedPath = tbFacialDir.Text
        If FBD.ShowDialog = DialogResult.OK Then
            tbFacialDir.Text = FBD.SelectedPath
        End If
    End Sub

    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles btnEditHRDir.Click
        FBD.SelectedPath = tbHRDir.Text
        If FBD.ShowDialog = DialogResult.OK Then
            tbHRDir.Text = FBD.SelectedPath
        End If
    End Sub

    Private Sub tbFacialDir_TextChanged(sender As Object, e As EventArgs) Handles tbFacialDir.TextChanged
        Try
            DataGridView1.Rows.Clear()
            Dim fs As String() = Directory.GetFiles(tbFacialDir.Text, "*.DBF")
            For Each f As String In fs
                DataGridView1.Rows.Add(Path.GetFileName(f))
            Next
        Catch ex As Exception
            'MsgBox(ex.Message)
        End Try
    End Sub


    'Private Sub fixFilenames()
    '    Dim fls As String() = Directory.GetFiles(tbHRDir.Text)
    '    For i As Integer = 0 To fls.Length - 1
    '        Dim fi As New FileInfo(fls(i))
    '        Dim newFilename As String = Path.GetFileNameWithoutExtension(fls(i)).ToUpper
    '        With newFilename
    '            newFilename = .Replace("-", "")
    '            newFilename = .Replace("PAY", "P")
    '            newFilename = .Replace("CHECK", "CHK")
    '            newFilename = .Replace("CASHCARD", "CCARD")
    '        End With
    '    Next
    'End Sub

    <Obsolete>
    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles btnSave.Click
        For Each rw In DataGridView1.Rows
            Dim facialpays = readDBF(tbFacialDir.Text & "\" & rw.Cells(0).Value)
            Dim hrpays = readDBF(tbHRDir.Text & "\" & rw.Cells(0).Value)
            If facialpays IsNot Nothing And hrpays IsNot Nothing Then

                Dim matchCount As Integer = 0, mismatchCount As Integer = 0

                Using wrtr As New StreamWriter(String.Format("{0}\{1}.csv", Application.StartupPath, facialpays.filename))
                    wrtr.WriteLine("FR = Facial Rec")
                    wrtr.WriteLine("HR = Human Resource")
                    wrtr.WriteLine("")

                    wrtr.WriteLine("Employee ID,Source,Total hours,OT,RD OT,HOLIDAY,ND,TARDY,Mismatch on,Remarks")
                    For fidx As Integer = facialpays.Records.Count - 1 To 0 Step -1
                        Dim facialrec As clsRecord = facialpays.Records(fidx)
                        Dim hrrec As clsRecord = (From res In hrpays.Records Where res.employee_id = facialrec.employee_id Take 1 Select res).FirstOrDefault
                        If hrrec IsNot Nothing Then
                            With facialrec
                                Dim mismatches As String = ""
                                If hrrec.total_hours - .total_hours < 20 Then
                                    If hrrec.total_hours <> .total_hours Then mismatches &= "; total hours"
                                    If hrrec.total_ots <> .total_ots Then mismatches &= "; OT"
                                    If hrrec.total_rd_ot <> .total_rd_ot Then mismatches &= "; RD OT"
                                    If hrrec.total_h_ot <> .total_h_ot Then mismatches &= "; HOLIDAY"
                                    If hrrec.total_nd <> .total_nd Then mismatches &= "; ND"
                                    If hrrec.total_tardy <> .total_tardy Then mismatches &= "; TARDY"
                                End If

                                mismatches = mismatches.Trim(";")
                                If mismatches <> "" Then
                                    wrtr.WriteLine(generateWriteString(facialrec, mismatches, facialrec.employee_id, "FR"))
                                    wrtr.WriteLine(generateWriteString(hrrec, "", "", "HR"))
                                    mismatchCount += 1
                                Else matchcount += 1
                                End If
                                facialpays.Records.Remove(facialrec)
                                hrpays.Records.Remove(hrrec)
                            End With
                        End If
                    Next

                    wrtr.WriteLine("")
                    wrtr.WriteLine(String.Format("{0},{1}", "Match:", matchCount))
                    wrtr.WriteLine(String.Format("{0},{1}", "Mismatch:", mismatchCount))
                    wrtr.WriteLine("")
                    wrtr.WriteLine("")
                    wrtr.WriteLine("IDs not found in HR DBF: {0}", facialpays.Records.Count)
                    If facialpays.Records.Count > 0 Then
                        For Each facialrec As clsRecord In facialpays.Records
                            wrtr.WriteLine(facialrec.employee_id)
                        Next
                    Else
                        wrtr.WriteLine("NONE")
                    End If

                    wrtr.WriteLine("")
                    wrtr.WriteLine("")
                    wrtr.WriteLine("IDs not found in FR DBF: {0}", hrpays.Records.Count)
                    If hrpays.Records.Count > 0 Then
                        For Each hrrec As clsRecord In hrpays.Records
                            wrtr.WriteLine(hrrec.employee_id)
                        Next
                    Else
                        wrtr.WriteLine("NONE")
                    End If

                End Using
            End If
        Next
    End Sub
    Private Function generateWriteString(rec As clsRecord, Optional mismatch As String = "", Optional employee_id As String = "", Optional source As String = "") As String
        With rec
            Return String.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9}", employee_id, source, .total_hours, .total_ots, .total_rd_ot, .total_h_ot, .total_nd, .total_tardy, mismatch, "")
        End With
    End Function

    <Obsolete>
    Private Function readDBF(path As String) As clsPays
        If File.Exists(path) Then
            Dim dbfrdr As New DotNetDBF.DBFReader(path) With {
            .CharEncoding = System.Text.Encoding.Default
        }

            Dim pays As New clsPays With {.filename = IO.Path.GetFileNameWithoutExtension(path)}
            Dim dt As New clsRecord(dbfrdr.NextRecord)
            While dt.invalid = False
                pays.Records.Add(dt)
                dt = New clsRecord(dbfrdr.NextRecord)
            End While

            Return pays
        End If
        Return Nothing
    End Function


    Public Class clsPays
        Public filename As String
        Public Records As New List(Of clsRecord)
    End Class

    Public Class clsRecord
        Sub New(obj As Object())
            If obj IsNot Nothing Then
                employee_id = obj(2)
                total_hours = obj(3)
                total_ots = obj(4)
                total_rd_ot = obj(5)
                total_h_ot = obj(7)
                total_nd = obj(9)
                total_tardy = obj(10)
            Else invalid = True
            End If
        End Sub


        Public invalid As Boolean
        Public employee_id As String
        Public total_hours As Double
        Public total_ots As Double
        Public total_rd_ot As Double
        Public total_h_ot As Double
        Public total_nd As Double
        Public total_tardy As Double
        Public allowance As Integer
    End Class


    Public Class InactiveID
        Public id As String
        Public cutoffs As New List(Of String)

    End Class

    Private Sub btnReport_Click(sender As Object, e As EventArgs) Handles btnReport.Click
        ' "P1CHK",
        ' "P1AATM1", "P1AATM2", "P1ACASH", "P1ACCARD",
        ' "P4AATM1", "P4ACCARD", "P4ACHK",
        ' "P10ACCARD", "P10ACHK", "P10CCARD", "P10CHK",
        ' "P11AATM1", "P11ACCARD",
        ' "K12AATM1", "K12ATM1"
        For Each paycode As String In {
            "P7ACHK", "P7ACCARD", "P7AATM1", "P7CCARD", "P7CHK"
            }


            Dim comparisons As String() = Directory.GetFiles(Application.StartupPath, "*" & paycode & "_*")

            Dim inactiveIDS As New List(Of InactiveID)
            For i As Integer = 0 To comparisons.Length - 1
                Dim readingAllowed As Boolean = False
                Dim filename As String = Path.GetFileNameWithoutExtension(comparisons(i))
                Using reader As New StreamReader(comparisons(i))
                    Dim lineText As String = reader.ReadLine
                    While lineText IsNot Nothing
                        If Not readingAllowed And lineText.Contains("IDs not found in FR DBF") Then
                            readingAllowed = True
                            lineText = reader.ReadLine
                        End If
                        If readingAllowed Then
                            Dim lineArr As String() = lineText.Split(",")
                            Dim inid As InactiveID = (From res In inactiveIDS Where res.id = lineArr(0) Select res).FirstOrDefault
                            If inid IsNot Nothing Then
                                inid.cutoffs.Add(filename)
                            Else
                                inid = New InactiveID
                                inid.id = lineArr(0)
                                inid.cutoffs.Add(filename)
                                inactiveIDS.Add(inid)
                            End If
                        End If
                        lineText = reader.ReadLine
                    End While
                End Using
            Next

            'generate csv named on the following paycode
            Using writer As StreamWriter = File.CreateText(String.Format("{0}/{1}.csv", Application.StartupPath, paycode))
                'header
                writer.Write("id")
                For i As Integer = 0 To comparisons.Length - 1
                    writer.Write(",{0}", Path.GetFileNameWithoutExtension(comparisons(i)))
                Next
                writer.Write(vbNewLine)

                'content
                For idsidx As Integer = 0 To inactiveIDS.Count - 1
                    writer.Write(inactiveIDS(idsidx).id)
                    For i As Integer = 0 To comparisons.Count - 1
                        Dim filename As String = Path.GetFileNameWithoutExtension(comparisons(i))
                        If inactiveIDS(idsidx).cutoffs.Contains(filename) Then
                            writer.Write(",{0}", "Not Found")
                        Else writer.Write(",")
                        End If
                    Next
                    writer.Write(vbNewLine)
                Next
            End Using
        Next
        MsgBox("Done")
    End Sub


End Class
