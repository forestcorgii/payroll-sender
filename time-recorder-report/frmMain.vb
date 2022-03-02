Imports System.IO

Public Class frmMain
    Private Logs As New LogCollection
    Public Settings As Configuration.General
    Public MysqlManager As Manager.MySql
    Private Sub frmMain_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Settings = New Configuration.General
        MysqlManager = New Manager.MySql(Settings.Database)
        MysqlManager.SetupConnection()
    End Sub

    Private Sub btnRun_Click(sender As Object, e As EventArgs) Handles btnRun.Click
        Dim query As String = String.Format("SELECT * FROM `{0}` WHERE `date_scanned` BETWEEN '{1}' AND '{2}';", Logs.table, dtFrom.Value.ToString("yyyy-MM-dd"), dtTo.Value.ToString("yyyy-MM-dd"))
        MysqlManager.CollectItems(Logs, New LogItem, "", query)

        Logs.Summarize()

        lstLogs.Items.Clear()

        For Each emp In Logs.Employees
            lstLogs.Items.Add(New ListViewItem(emp.GetArgument))
        Next
    End Sub

    Private Sub btnGenerateCSV_Click(sender As Object, e As EventArgs) Handles btnGenerateCSV.Click
        Using writer As New StreamWriter(String.Format("{1}-{2} REPORT.csv", Logs.table, dtFrom.Value.ToString("yyyy-MM-dd"), dtTo.Value.ToString("yyyy-MM-dd")))
            writer.WriteLine()
            writer.WriteLine()
            writer.WriteLine()
            writer.WriteLine()
            writer.WriteLine("Employee ID,Prompt Count,Other Matched ID/Count", "Mismatch Count")
            For Each l As ListViewItem In lstLogs.Items
                writer.WriteLine("{0},{1},{2},{3}", l.SubItems(0).Text, l.SubItems(1).Text, l.SubItems(2).Text, l.SubItems(3).Text)
            Next
        End Using
        MsgBox("done")
    End Sub

    Public Class LogCollection
        Inherits List(Of LogItem)

        Public Employees As New List(Of EmployeeItem)
        Public ReadOnly Property table = "matching_scores_tbl"

        Public Sub Summarize()
            Dim grouped_logs As List(Of List(Of LogItem)) = (From res In Me Order By res.employee_id Group By res.employee_id Into emp = Group Select emp.ToList).ToList
            For Each _Logs In grouped_logs
                Dim employee As New EmployeeItem
                employee.employee_id = _Logs(0).employee_id
                employee.MismatchCount = 0

                For Each log As LogItem In _Logs
                    If log.is_match = False Then employee.MismatchCount += 1
                    log.ProcessLog()
                    If log.matched_with Is Nothing Then Continue For
                    For Each m In log.matched_with
                        employee.Feed(m)
                    Next
                Next

                Employees.Add(employee.Clone)
            Next
        End Sub
    End Class

    Public Class EmployeeItem
        Implements ICloneable

        Public employee_id As String
        Public Matches As New List(Of MatchItem)
        Public MismatchCount As Integer

        Public Sub Feed(employee_id As String)
            Dim match As MatchItem = (From res In Matches Where res.employee_id = employee_id).FirstOrDefault
            If match IsNot Nothing Then
                match.matchcount += 1
            Else
                Matches.Add(New frmMain.MatchItem With {.employee_id = employee_id, .matchcount = 1})
            End If
        End Sub

        Public Function Clone() As Object Implements ICloneable.Clone
            Return MemberwiseClone()
        End Function

        Public Function GetArgument() As String()
            Dim summary As String = ""
            For i As Integer = 1 To Matches.Count - 1
                Dim m As MatchItem = Matches(i)
                summary &= String.Format("{0}:{1}   ", m.employee_id, m.matchcount)
            Next
            Return New String() {employee_id, Matches(0).matchcount, summary, MismatchCount}
        End Function

    End Class

    Public Class LogItem
        Implements ICloneable

        Public employee_id As String
        Public score As Integer
        Public date_scanned As Date
        Public is_match As SByte
        Public raw_matches As String

        Public matched_with As New List(Of String)

        Public Sub ProcessLog()
            matched_with = New List(Of String)
            matched_with.Add(employee_id)
            For Each m As String In raw_matches.Split(",")
                Dim i As String() = m.Split(" ")
                If i(0).Substring(0, 4) <> employee_id And CInt(i(1)) >= 60 Then
                    matched_with.Add(i(0).Substring(0, 4))
                End If
            Next
        End Sub

        Public Function Clone() As Object Implements ICloneable.Clone
            Return MemberwiseClone()
        End Function
    End Class

    Public Class MatchItem
        Public employee_id As String
        Public matchcount As Integer
    End Class


End Class
