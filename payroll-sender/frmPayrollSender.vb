Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports Newtonsoft.Json
Imports System.Text
Imports NPOI.HSSF.UserModel

Imports NPOI.SS.UserModel
Imports MySql.Data.MySqlClient

Public Class frmPayrollSender
    Private bin As String
    Private payrollDirectory As String
    Private dataDirectory As String

    Private logInfo As DBFLog

    Private Sub frmPayrollSender_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Text = Application.ProductName & " v" & Application.ProductVersion

        DatabaseConfiguration = New utility_service.Configuration.MysqlConfiguration()
        DatabaseConfiguration.Setup("ACCOUNTING_DB_URL")

        DatabaseManager = New utility_service.Manager.Mysql
        DatabaseManager.Connection = DatabaseConfiguration.ToMysqlConnection

        bin = New DirectoryInfo(Application.StartupPath).Parent.FullName & "\"
        dataDirectory = bin & "\Data"

        Directory.CreateDirectory(dataDirectory)

        logInfo = New DBFLog
        LoadDownloadLog()

        bgProcessPayroll.RunWorkerAsync()
    End Sub

    Private Sub LoadDownloadLog()
        DatabaseManager.Connection.Open()
        Try
            Using reader As MySqlDataReader = DatabaseManager.ExecuteDataReader("SELECT * FROM payroll_management.download_log;")
                If reader.HasRows Then
                    While reader.Read()
                        Dim downloadLog As New DownloadLogDetail(reader)
                        downloadLog.Row = New DataGridViewRow
                        downloadLog.Row.CreateCells(dgv)
                        downloadLog.RefreshRow()

                        dgv.Rows.Add(downloadLog.Row)
                        logInfo.Payrolls.Add(downloadLog)
                    End While
                End If
            End Using
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error in Loading Download Log.", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        DatabaseManager.Connection.Close()
    End Sub

#Region "timesheet"

    Public Function SendAPIMessage(PostData As String, Address As String) As String
        Try
            Dim w As WebRequest = WebRequest.Create(Address)
            w.Timeout = 1000000
            w.Method = "POST"
            Dim byteArray As Byte() = Encoding.UTF8.GetBytes(PostData)
            w.ContentType = "application/x-www-form-urlencoded"
            w.ContentLength = byteArray.Length

            Dim dataStream As Stream = w.GetRequestStream()
            dataStream.Write(byteArray, 0, byteArray.Length)

            Dim response As WebResponse = w.GetResponse()
            dataStream = response.GetResponseStream()

            Dim reader As New StreamReader(dataStream)
            Dim responseFromServer As String = reader.ReadToEnd()

            reader.Close()
            dataStream.Close()
            response.Close()

            Return responseFromServer
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Sending API Call.", MessageBoxButtons.OK, MessageBoxIcon.Error)
            'Return SendAPIMessage(PostData, Address)
        End Try
        Return Nothing
    End Function

    Public Class clss
        Public info As String = "getTimeLogs"
        Public api_token As String = "40jhwWlphorjv40"
        Public date_from As String
        Public date_to As String
        Public page As String
    End Class

    Private Sub subtract15(ByRef oldPayrollDate As Date)
        Dim newPayrollDate As Date
        If oldPayrollDate.Day = 30 Then
            newPayrollDate = String.Format("{0}-{1:00}-15", oldPayrollDate.Year, oldPayrollDate.Month)
        Else
            Dim day As String = "30"
            Dim _date = oldPayrollDate.AddMonths(-1)
            If _date.Month = 2 Then
                day = IIf(DateTime.IsLeapYear(_date.Year), 29, 28)
            End If

            newPayrollDate = String.Format("{0}-{1:00}-{2}", oldPayrollDate.AddMonths(-1).Year, oldPayrollDate.AddMonths(-1).Month, day)
        End If
        oldPayrollDate = Date.Parse(newPayrollDate)
    End Sub


    'Private newPayroll As New DownloadLog
    Private Sub collectDBFData()
        Dim dbfData As New List(Of List(Of clsNewPayroll))
        Dim payrollDate As Date
        If Now.Day > 25 Then
            payrollDate = Date.Parse(String.Format("{0}-{1}-30", Now.Year, Now.Month))
        ElseIf Now.Day >= 5 Then
            payrollDate = Date.Parse(String.Format("{0}-{1}-15", Now.Year, Now.Month))
        Else
            payrollDate = Date.Parse(String.Format("{0}-{1}-15", Now.Year, Now.Month))
            subtract15(payrollDate)
        End If

        DatabaseManager.Connection.Open()
        'RUN PENDING DOWNLOAD LOG
        For Each downloadLog As DownloadLogDetail In logInfo.Payrolls
            If downloadLog.Status = DownloadLogDetail.DownloadStatusChoices.PENDING Then
                RequestPayrolls(payrollDate, downloadLog)
            End If
        Next

        While payrollDate > Now.AddMonths(-4)
            Try
                If Not logInfo.Contains(payrollDate.ToString("yyyyMMdd")) Then
                    dbfData = New List(Of List(Of clsNewPayroll))
                    Dim newPayroll As New DownloadLogDetail With {.Payroll_Date = payrollDate, .Status = DownloadLogDetail.DownloadStatusChoices.PENDING, .Row = New DataGridViewRow}
                    newPayroll.TotalPage = RequestPayrollInfo(payrollDate)
                    newPayroll.DateTimeCreated = Now
                    newPayroll.Row.CreateCells(dgv)
                    Invoke(Sub()
                               newPayroll.RefreshRow()
                               dgv.Rows.Add(newPayroll.Row)
                           End Sub)
                    newPayroll.SaveDownloadLog(DatabaseManager)
                    newPayroll.GetId() ' get Log ID
                    logInfo.Payrolls.Add(newPayroll)

                    RequestPayrolls(payrollDate, newPayroll)
                End If

                subtract15(payrollDate)
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error in Collecting Payroll Data.", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End While

        DatabaseManager.Connection.Close()
    End Sub

    Private Function RequestPayrolls(payrollDate As Date, newPayroll As DownloadLogDetail) As List(Of List(Of clsNewPayroll))
        Dim rawPayrollData As String = ""
        Dim tm As clss = generatePayrollRange(payrollDate)
        Dim tms As New List(Of clsNewPayroll)

        'newPayroll.Last_Page_Downloaded = 0

        While newPayroll.Last_Page_Downloaded < newPayroll.TotalPage
            Try
                tm.page = newPayroll.Last_Page_Downloaded
                Dim postData As String = "postData=" & JsonConvert.SerializeObject(tm, Formatting.Indented)
                rawPayrollData = SendAPIMessage(postData, "http://idcsi-officesuites.com:8080/mail/pages/send_timelog")
                If rawPayrollData <> "" Then
                    Dim response As New resp
                    response = JsonConvert.DeserializeObject(rawPayrollData, response.GetType)

                    For i As Integer = 0 To response.message.Count - 1
                        'insert to payroll database
                        response.message(i).SavePayroll(payrollDate, DatabaseManager)
                    Next

                    newPayroll.Last_Page_Downloaded += 1
                    newPayroll.UpdateStatus()
                    Invoke(Sub() newPayroll.RefreshRow())
                End If
            Catch ex As Exception
                MessageBox.Show(ex.Message, "Error in Requesting Payroll Data.", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
        End While
        newPayroll.Status = DownloadLogDetail.DownloadStatusChoices.DONE
        newPayroll.UpdateStatus()

        Return Nothing
    End Function

    Private Function RequestPayrollInfo(payrollDate As Date) As Integer
        Dim rawPayrollData As String = ""
        Dim tm As clss = generatePayrollRange(payrollDate)
        tm.page = -1
        Try
            Dim postData As String = "postData=" & JsonConvert.SerializeObject(tm, Formatting.Indented)
            rawPayrollData = SendAPIMessage(postData, "http://idcsi-officesuites.com:8080/mail/pages/send_timelog")
            If rawPayrollData <> "" Then
                Dim response As New resp
                response = JsonConvert.DeserializeObject(rawPayrollData, response.GetType)
                Return response.totalPage
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "Error in Requesting Payroll Info.", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Return Nothing
    End Function

    Public Class resp
        Public status As String
        Public totalPage As String
        Public message As New List(Of clsNewPayroll)
    End Class

    'Private Sub tmWatcher_Tick(sender As Object, e As EventArgs) Handles tmWatcher.Tick
    '    If newPayroll.Row IsNot Nothing AndAlso newPayroll.Row.Cells.Count > 0 Then
    '        newPayroll.RefreshRow()
    '    End If
    'End Sub

    Private Function generatePayrollRange(payrollDate As Date) As clss
        Dim startDate As Date
        Dim endDate As Date
        If payrollDate.Day = 15 Then
            startDate = Date.Parse(String.Format("{0}-{1:00}-20", payrollDate.AddMonths(-1).Year, payrollDate.AddMonths(-1).Month))
            endDate = Date.Parse(String.Format("{0}-{1:00}-04", payrollDate.Year, payrollDate.Month))
        Else '30
            startDate = Date.Parse(String.Format("{0}-{1:00}-05", payrollDate.Year, payrollDate.Month))
            endDate = Date.Parse(String.Format("{0}-{1:00}-19", payrollDate.Year, payrollDate.Month))
        End If
        Return New clss With {.date_from = startDate.ToString("yyyy-MM-dd"), .date_to = endDate.ToString("yyyy-MM-dd")}
    End Function

    Private Sub writeExcel(arg As Object(), dbfName As String, payrollDate As Date)
        Try
            Dim payroll_codes As List(Of clsNewPayroll) = arg(0)
            If payroll_codes.Count > 0 Then
                Dim filename As String = String.Format("{0}{1}_{2}", payroll_codes(0).PayrollCode, payroll_codes(0).BankCategory, payrollDate.ToString("yyyyMMdd"))
                Dim filePath As String = String.Format("{0}\{1}\{2}.DBF", payrollDirectory, dbfName, filename)
                SaveToDBF(filePath, arg, payrollDate)
                'write in excel
            End If
        Catch ex As Exception
            MessageBox.Show(ex.Message, "error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub

    Private Function payrollNameExist(name As String) As Boolean
        For i As Integer = 0 To dgv.Rows.Count - 1
            If dgv.Rows(i).Cells(0).Value = name Then
                Return True
            End If
        Next
        Return False
    End Function

#End Region

    Public dbfFlds As String() = {"DATER SmallInt", "CODE SmallInt", "ID Text(4)", "REG_HRS Float", "R_OT Float", "RD_OT Float", "RD_8 Float",
                                  "HOL_OT Float", "HOL_OT8 Float", "ND Float", "ABS_TAR Float", "ADJUST1 Float", "GROSS_PAY Float", "ADJUST2 Float",
                                  "TAX Float", "SSS_EE Float", "SSS_ER Float", "PHIC Float", "NET_PAY Float", "REG_PAY Float", "TAG Text(1)"}
    Public Sub SaveToDBF(dbfPath As String, arg As Object(), payrollDate As Date)
        Try
            Dim flds As New List(Of DotNetDBF.DBFField)
            For Each col In dbfFlds
                Dim fldinf As String() = col.Split(" ")
                Select Case fldinf(1).ToUpper
                    Case "FLOAT"
                        flds.Add(New DotNetDBF.DBFField(fldinf(0), DotNetDBF.NativeDbType.Float, 10, 2))
                    Case "SMALLINT"
                        flds.Add(New DotNetDBF.DBFField(fldinf(0), DotNetDBF.NativeDbType.Numeric, 10, 0))
                    Case "TEXT(4)"
                        flds.Add(New DotNetDBF.DBFField(fldinf(0), DotNetDBF.NativeDbType.Char, 4, 0))
                    Case "TEXT(1)"
                        flds.Add(New DotNetDBF.DBFField(fldinf(0), DotNetDBF.NativeDbType.Char, 1, 0))
                End Select
            Next

            Dim records As New List(Of String())
            Dim values As List(Of clsNewPayroll) = arg(0)
            For r As Integer = 0 To values.Count - 1
                If values(r).Valid Then
                    values(r).code = arg(1)
                    Dim record(flds.Count - 1) As String
                    For c As Integer = 0 To dbfFlds.Length - 1
                        Dim fldinf As String() = dbfFlds(c).Split(" ")
                        record(c) = values(r).GetValue(fldinf(0))
                    Next
                    records.Add(record)
                End If
            Next

            If records.Count > 0 Then
                Dim s As Stream = File.Create(dbfPath)
                Dim writer As New DotNetDBF.DBFWriter(s)
                writer.CharEncoding = Encoding.UTF8
                writer.Fields = flds.ToArray
                For Each record As String() In records
                    writer.WriteRecord(record)
                Next
                writer.Close()
                s.Close()



                WriteTimesheetExcelReport(Path.ChangeExtension(dbfPath, "xls"), arg(0), values(0).PayrollCode, values(0).BankCategory, payrollDate)

            End If

        Catch ex As Exception
            MessageBox.Show(ex.Message, "error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub


    Public Function WriteTimesheetExcelReport(filePath As String, employees As List(Of clsNewPayroll), payroll_code As String, bank_category As String, payrollDate As String) As Boolean
        Try
            Dim nWorkBook As IWorkbook = New HSSFWorkbook()
            Dim nSheet As ISheet = nWorkBook.CreateSheet(bank_category)
            Dim rng As clss = generatePayrollRange(payrollDate)
            Dim _to As Date = Date.Parse(rng.date_to)
            Dim _from As Date = Date.Parse(rng.date_from)

            createCellLines(0, {"", "", payroll_code & " - " & bank_category}, nSheet)
            createCellLines(1, {"", "", String.Format("{0} - {1}", _from.ToString("MMMM d"), _to.ToString("MMMM d, yyyy"))}, nSheet)
            createCellLines(2, {""}, nSheet)
            createCellLines(3, {"#", "ID #", "NAMES", "TITLE", "DEPT", "REG HRS", "R_OT", "SUN", "H_OT", "ND", "TARDY", "PCV(Yes/No)", "ALLOWANCE", "INCENTIVE", "EXCESS", "REMARKS"}, nSheet)

            Dim rowidx As Integer = 4
            For Each payroll As clsNewPayroll In employees
                If payroll.logs(0).is_confirmed Then
                    createCellLines(rowidx, {rowidx - 3, payroll.employee_id, payroll.Fullname, payroll.job_title, payroll.location, payroll.GetValue("REG_HRS"), payroll.GetValue("R_OT"),
                                             payroll.GetValue("RD_OT"), payroll.GetValue("HOL_OT"), payroll.GetValue("ND"), payroll.GetValue("ABS_TAR"), payroll.GetValue("PCV"), payroll.GetValue("ADJUST1"), payroll.GetValue("INCENTIVE"), payroll.GetValue("EXCESS"), payroll.GetValue("TIMESHEET_GUIDE_REMARKS")}, nSheet) 'TIMESHEET_GUIDE_REMARKS
                    rowidx += 1
                End If
            Next

            createCellLines(rowidx, {""}, nSheet) : rowidx += 1
            createCellLines(rowidx, {""}, nSheet) : rowidx += 1
            createCellLines(rowidx, {"", "", "NOT CONFIRMED"}, nSheet) : rowidx += 1

            For Each payroll As clsNewPayroll In employees
                If payroll.logs(0).is_confirmed = False Then
                    createCellLines(rowidx, {rowidx - 3, payroll.employee_id, payroll.Fullname, payroll.job_title, payroll.location, payroll.GetValue("REG_HRS"), payroll.GetValue("R_OT"),
                                             payroll.GetValue("RD_OT"), payroll.GetValue("HOL_OT"), payroll.GetValue("ND"), payroll.GetValue("ABS_TAR"), payroll.GetValue("PCV"), payroll.GetValue("ADJUST1"), payroll.GetValue("INCENTIVE"), payroll.GetValue("EXCESS"), payroll.GetValue("TIMESHEET_GUIDE_REMARKS")}, nSheet) 'TIMESHEET_GUIDE_REMARKS
                    'createCellLines(rowidx, {"", payroll.employee_id, payroll.Fullname, payroll.location}, nSheet)
                    rowidx += 1
                End If
            Next
            Using wrtr As FileStream = New IO.FileStream(filePath, FileMode.Create, FileAccess.Write)
                nWorkBook.Write(wrtr)
            End Using

            Return False
        Catch ex As Exception
            MsgBox(ex.Message)
            Return False
        End Try
    End Function

    Private Sub createCellLines(rowidx As Integer, cellnames As String(), nsheet As ISheet)
        Dim row As IRow = nsheet.CreateRow(rowidx)
        For colidx As Integer = 0 To cellnames.Length - 1
            Dim cel As ICell = row.CreateCell(colidx)
            cel.SetCellValue(cellnames(colidx))
        Next
    End Sub


    Private lastListing As Date
    Private Sub tmLister_Tick(sender As Object, e As EventArgs) Handles tmLister.Tick
        Me.Invoke(Sub() tmLister.Enabled = False)
        Dim tm As TimeSpan = (Now - lastListing)
        If tm.TotalSeconds >= 120 Then
            'list
            If Not bgProcessPayroll.IsBusy Then
                bgProcessPayroll.RunWorkerAsync()
            End If
            lastListing = Now
        Else
            lbStatus.Text = 120 - tm.TotalSeconds.ToString("##") & "sec/s before listing..."
        End If

        Me.Invoke(Sub() tmLister.Enabled = True)
    End Sub

    Private Sub bgProcessPayroll_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles bgProcessPayroll.DoWork
        Try
            collectDBFData()
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

End Class
