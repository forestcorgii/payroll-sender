
Imports MySql.Data.MySqlClient

Public Class clsNewPayroll
    Public ReadOnly Property Fullname As String
        Get
            Dim mi As Char = ""
            If middle_name.Length > 0 Then mi = middle_name(0)
            Return String.Format("{0},{1} {2}", last_name, first_name, mi)
        End Get
    End Property

    Public ReadOnly Property Valid As Boolean
        Get
            If employee_id Is Nothing Then Return False
            If first_name Is Nothing Then Return False

            If logs Is Nothing Then Return False
            If logs.Count <= 0 Then Return False
            If logs(0).total_hours = 0 Then Return False
            Return True
        End Get
    End Property

    Public employee_id As String
    Public first_name As String
    Public last_name As String
    Public middle_name As String
    Public location As Object
    Public bank_category As String
    Public payroll_code As String
    Public rec_type As String
    Public job_title As String

    Public code As Integer

    Public ID As String
    Public Schedule As String
    Public logs As List(Of clsNewTimeLog)
    Public CeilingHours As Integer

    Public ReadOnly Property PayrollCode As String
        Get
            Dim pCode As String = payroll_code.Split("-")(0).Replace("PAY", "P")
            If pCode.Contains("K12AA") Then Return "K12A"
            If pCode.Contains("K12AT") Then Return "K12"

            If pCode.Contains("K12A") Then Return "K12A"
            If pCode.Contains("K12") Then Return "K12"

            If pCode.Contains("K13") Then Return "K13"
            If pCode.Contains("P1A") Then Return "P1A"
            If pCode.Contains("P4A") Then Return "P4A"
            If pCode.Contains("P7A") Then Return "P7A"
            If pCode.Contains("P10A") Then Return "P10A"
            If pCode.Contains("P11A") Then Return "P11A"

            If pCode = "" Then pCode = "NOCODE"
            Return pCode
        End Get
    End Property

    Public ReadOnly Property BankCategory As String
        Get
            Dim props As String() = payroll_code.Split("-")
            Dim bankCat As String = bank_category
            If props.Length = 2 Then
                If props(1) <> "NO BANK" Then
                    bankCat = props(1)
                End If
            End If

            Select Case bankCat
                Case "ATM", "ATM1"
                    bankCat = "ATM1"
                Case "ATM2"
                Case "CHECK", "CHEQUE", "", "NO BANK"
                    bankCat = "CHK"
                Case "CASHCARD", "CCARD"
                    bankCat = "CCARD"
                Case "CASH"
                    bankCat = "CASH"
                Case Else
                    'If bankCat = "" Then
                    'bankCat = "NOBANKCATEGORY"
                    'Else
                    MsgBox(bankCat)
                    'End If
            End Select
            Return bankCat
        End Get
    End Property

    Public Function GetValue(field As String, Optional ignoreCeiling As Boolean = False)
        Dim value = Nothing
        Select Case field
            Case "DATA"
                value = 0
            Case "CODE"
                value = code
            Case "ID"
                value = employee_id
            Case "REG_HRS"
                value = Double.Parse(logs(0).total_hours.ToString("0.00"))
            Case "R_OT"
                value = Double.Parse(logs(0).total_ots.ToString("0.00"))
            Case "RD_OT"
                value = Double.Parse(logs(0).total_rd_ot.ToString("0.00"))
            Case "RD_8"
                value = 0.00
            Case "HOL_OT"
                value = Double.Parse(logs(0).total_h_ot.ToString("0.00"))
            Case "HOL_OT8"
                value = 0.00
            Case "ND"
                value = Double.Parse(logs(0).total_nd.ToString("0.00"))
            Case "ABS_TAR"
                value = Double.Parse(logs(0).total_tardy.ToString("0.00"))
            Case "ADJUST1"
                value = Double.Parse(logs(0).allowance.ToString("0.00"))
            Case "INCENTIVE"
                value = Double.Parse(logs(0).incentive.ToString("0.00"))
            Case "EXCESS"
                value = Double.Parse(logs(0).total_excess.ToString("0.00"))
            Case "GROSS_PAY"
                value = 0.00
            Case "ADJUST2"
                value = 0.00
            Case "TAX"
                value = 0.00
            Case "SSS_EE"
                value = 0.00
            Case "SSS_ER"
                value = 0.00
            Case "PHIC"
                value = 0.00
            Case "NET_PAY"
                value = 0.00
            Case "REG_PAY"
                value = 0.00
            Case "TAG"
            Case "PCV"
                value = logs(0).pcv
            Case "TIMESHEET_GUIDE_REMARKS"
                value = logs(0).timesheet_guide_remarks
        End Select

        Try
            If value Is Nothing OrElse value.ToString = "0" OrElse value.ToString = "" Then
                Return Nothing
            ElseIf IsNumeric(value) Then
                If value > CeilingHours And Not ignoreCeiling Then
                    value = CeilingHours
                End If
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        Return value
    End Function


    Public Sub SavePayroll(payrollDate As Date, databaseManager As utility_service.Manager.Mysql)
        'check if employee exists in the database
        Try
            Dim command As New MySqlCommand
            Dim ee_id As Integer = GetEEId(databaseManager) 'employee table primary key
            If ee_id = -1 Then
                command = New MySqlCommand("INSERT INTO payroll_management.employee (employee_id, first_name, last_name,middle_name)VALUES(?,?,?,?)", databaseManager.Connection)
                command.Parameters.AddWithValue("p1", employee_id)
                command.Parameters.AddWithValue("p2", first_name)
                command.Parameters.AddWithValue("p3", last_name)
                command.Parameters.AddWithValue("p4", middle_name)
                command.ExecuteNonQuery()

                ee_id = GetEEId(databaseManager)
            End If

            'delete existing payroll to avoid duplication
            command = New MySqlCommand("DELETE FROM payroll_management.payroll_time WHERE employee_id=? AND payroll_date=?", databaseManager.Connection)
            command.Parameters.AddWithValue("p1", employee_id)
            command.Parameters.AddWithValue("p2", payrollDate)

            'insert payroll
            command = New MySqlCommand("INSERT INTO payroll_management.payroll_time (ee_id, location,job_title,payroll_code,bank_category,total_hours,total_ots,total_rd_ot,total_h_ot,total_nd,total_tardy,allowance,incentive,has_pcv,timesheet_guide_remarks,payroll_date)VALUES(?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)", databaseManager.Connection)
            command.Parameters.AddWithValue("p1", ee_id)
            command.Parameters.AddWithValue("p2", location)
            command.Parameters.AddWithValue("p3", job_title)
            command.Parameters.AddWithValue("p4", PayrollCode)
            command.Parameters.AddWithValue("p5", BankCategory)
            command.Parameters.AddWithValue("p6", logs(0).total_hours)
            command.Parameters.AddWithValue("p7", logs(0).total_ots)
            command.Parameters.AddWithValue("p8", logs(0).total_rd_ot)
            command.Parameters.AddWithValue("p9", logs(0).total_h_ot)
            command.Parameters.AddWithValue("p10", logs(0).total_nd)
            command.Parameters.AddWithValue("p11", logs(0).total_tardy)
            command.Parameters.AddWithValue("p12", logs(0).allowance)
            command.Parameters.AddWithValue("p13", logs(0).incentive)
            command.Parameters.AddWithValue("p14", logs(0).pcv)
            command.Parameters.AddWithValue("p15", logs(0).timesheet_guide_remarks)
            command.Parameters.AddWithValue("p16", payrollDate)

            command.ExecuteNonQuery()
        Catch ex As Exception
            Console.WriteLine(ex.Message)
        End Try
    End Sub



    Private Function GetEEId(databaseManager As utility_service.Manager.Mysql) As Integer
        Dim ee_id As Integer = -1
        Using reader As MySqlDataReader = databaseManager.ExecuteDataReader(String.Format("SELECT `id` FROM payroll_management.employee WHERE employee_id='{0}' LIMIT 1;", employee_id))
            If reader.HasRows Then 'means user doesn't exists
                reader.Read()
                ee_id = reader.Item("id").ToString
            End If
        End Using
        Return ee_id
    End Function



    Public Overrides Function ToString() As String
        Return employee_id & " - " & logs(0).total_hours
    End Function

End Class

Public Class clsNewTimeLog
    Public total_hours As Double
    Public total_ots As Double
    Public total_rd_ot As Double
    Public total_h_ot As Double
    Public total_nd As Double
    Public total_tardy As Double
    Public allowance As Double
    Public incentive As Double
    Public total_excess As Double
    Public is_confirmed As Boolean
    Public pcv As String
    Public timesheet_guide_remarks As String

End Class

Public Class ForPAY
    Public Employees As List(Of clsNewPayroll)
    Public Holiday As List(Of Date)

    Sub New()
        Employees = New List(Of clsNewPayroll)
        Holiday = New List(Of Date)
    End Sub

End Class

Public Class DBFLog
    Public Payrolls As New List(Of DownloadLogDetail)

    Public Shadows Function Contains(_name As String) As Boolean
        Dim nameFound As DownloadLogDetail = (From res In Payrolls Where res.Name = _name).FirstOrDefault
        Return nameFound IsNot Nothing
    End Function
End Class

Public Class DownloadLogDetail
    Public Row As DataGridViewRow

    Public id As Integer

    Public Payroll_Date As Date
    Public TotalPage As Integer
    Public Last_Page_Downloaded As Integer = 0

    Public Status As DownloadStatusChoices
    Public DateTimeCreated As Date

    Sub New()

    End Sub

    Sub New(reader As MySqlDataReader)
        id = reader.Item("id")
        Payroll_Date = reader.Item("payroll_date")
        TotalPage = reader.Item("total_page")
        Last_Page_Downloaded = reader.Item("last_page_downloaded")
        Status = reader.Item("status")
        DateTimeCreated = reader.Item("log_created")
    End Sub

    Public Sub RefreshRow()
        If Last_Page_Downloaded <> TotalPage Then
            Row.SetValues(Name, Last_Page_Downloaded - 1 & "/" & TotalPage)
        End If
    End Sub

    Public Sub GetId()
        Using reader As MySqlDataReader = DatabaseManager.ExecuteDataReader(String.Format("SELECT id FROM payroll_management.download_log WHERE payroll_date='{0}' ORDER BY log_created DESC LIMIT 1;", Payroll_Date.ToString("yyyy-MM-dd")))
            If reader.HasRows Then
                reader.Read()
                id = reader.Item("id")
            End If
        End Using
    End Sub
    Public Sub SaveDownloadLog(databaseManager As utility_service.Manager.Mysql)
        Dim command As New MySqlCommand("INSERT INTO payroll_management.download_log (payroll_date,total_page,last_page_downloaded,status)VALUES(?,?,?,?);", databaseManager.Connection)
        command.Parameters.AddWithValue("p1", Payroll_Date)
        command.Parameters.AddWithValue("p2", TotalPage)
        command.Parameters.AddWithValue("p3", Last_Page_Downloaded)
        command.Parameters.AddWithValue("p4", Status)

        command.ExecuteNonQuery()
    End Sub
    Public Sub UpdateStatus()
        Dim command As New MySqlCommand("UPDATE payroll_management.download_log SET last_page_downloaded=?, status=? WHERE id=?;", DatabaseManager.Connection)
        command.Parameters.AddWithValue("p1", Last_Page_Downloaded)
        command.Parameters.AddWithValue("p2", Status)
        command.Parameters.AddWithValue("p3", id)

        command.ExecuteNonQuery()
    End Sub
    Public ReadOnly Property Name As String
        Get
            Return Payroll_Date.ToString("yyyyMMdd")
        End Get
    End Property
    Public Overrides Function ToString() As String
        Return Name
    End Function

    Public Enum DownloadStatusChoices
        PENDING
        DONE
    End Enum
End Class
