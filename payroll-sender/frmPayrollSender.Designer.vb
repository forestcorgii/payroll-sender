<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class frmPayrollSender
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.label1 = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lbStatus = New System.Windows.Forms.ToolStripStatusLabel()
        Me.lbQueue = New System.Windows.Forms.ToolStripStatusLabel()
        Me.tmLister = New System.Windows.Forms.Timer(Me.components)
        Me.dgv = New System.Windows.Forms.DataGridView()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.dtFrom = New System.Windows.Forms.DateTimePicker()
        Me.dtTo = New System.Windows.Forms.DateTimePicker()
        Me.bgProcessPayroll = New System.ComponentModel.BackgroundWorker()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.tmWatcher = New System.Windows.Forms.Timer(Me.components)
        Me.clFileName = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.clStatus = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.StatusStrip1.SuspendLayout()
        CType(Me.dgv, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'StatusStrip1
        '
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.label1, Me.lbStatus, Me.lbQueue})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 248)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(399, 22)
        Me.StatusStrip1.TabIndex = 4
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'label1
        '
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(42, 17)
        Me.label1.Text = "Status:"
        '
        'lbStatus
        '
        Me.lbStatus.Name = "lbStatus"
        Me.lbStatus.Size = New System.Drawing.Size(59, 17)
        Me.lbStatus.Text = "Standby..."
        '
        'lbQueue
        '
        Me.lbQueue.Name = "lbQueue"
        Me.lbQueue.Size = New System.Drawing.Size(73, 17)
        Me.lbQueue.Text = "On Queue: 0"
        '
        'tmLister
        '
        Me.tmLister.Interval = 1000
        '
        'dgv
        '
        Me.dgv.AllowUserToAddRows = False
        Me.dgv.AllowUserToDeleteRows = False
        Me.dgv.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.dgv.BackgroundColor = System.Drawing.Color.White
        Me.dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgv.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.clFileName, Me.clStatus})
        Me.dgv.GridColor = System.Drawing.SystemColors.ControlDarkDark
        Me.dgv.Location = New System.Drawing.Point(1, 2)
        Me.dgv.Name = "dgv"
        Me.dgv.RowHeadersVisible = False
        Me.dgv.Size = New System.Drawing.Size(397, 238)
        Me.dgv.TabIndex = 5
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(68, 196)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(70, 13)
        Me.Label2.TabIndex = 6
        Me.Label2.Text = "Payroll Date: "
        Me.Label2.Visible = False
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(218, 196)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(16, 13)
        Me.Label3.TabIndex = 7
        Me.Label3.Text = " - "
        Me.Label3.Visible = False
        '
        'dtFrom
        '
        Me.dtFrom.CustomFormat = "yyyy-MM-dd"
        Me.dtFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtFrom.Location = New System.Drawing.Point(135, 190)
        Me.dtFrom.Name = "dtFrom"
        Me.dtFrom.Size = New System.Drawing.Size(77, 20)
        Me.dtFrom.TabIndex = 8
        Me.dtFrom.Visible = False
        '
        'dtTo
        '
        Me.dtTo.CustomFormat = "yyyy-MM-dd"
        Me.dtTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtTo.Location = New System.Drawing.Point(240, 190)
        Me.dtTo.Name = "dtTo"
        Me.dtTo.Size = New System.Drawing.Size(77, 20)
        Me.dtTo.TabIndex = 9
        Me.dtTo.Visible = False
        '
        'bgProcessPayroll
        '
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ProgressBar1.Location = New System.Drawing.Point(0, 242)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(399, 5)
        Me.ProgressBar1.TabIndex = 10
        '
        'tmWatcher
        '
        Me.tmWatcher.Enabled = True
        Me.tmWatcher.Interval = 1000
        '
        'clFileName
        '
        Me.clFileName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.clFileName.HeaderText = "File Name"
        Me.clFileName.Name = "clFileName"
        Me.clFileName.ReadOnly = True
        '
        'clStatus
        '
        Me.clStatus.HeaderText = "Status"
        Me.clStatus.Name = "clStatus"
        Me.clStatus.Resizable = System.Windows.Forms.DataGridViewTriState.[True]
        Me.clStatus.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.clStatus.Width = 75
        '
        'frmPayrollSender
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(399, 270)
        Me.Controls.Add(Me.ProgressBar1)
        Me.Controls.Add(Me.dgv)
        Me.Controls.Add(Me.dtTo)
        Me.Controls.Add(Me.dtFrom)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.StatusStrip1)
        Me.Name = "frmPayrollSender"
        Me.Text = "Payroll Sender"
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        CType(Me.dgv, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents StatusStrip1 As System.Windows.Forms.StatusStrip
    Friend WithEvents label1 As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents lbStatus As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents lbQueue As System.Windows.Forms.ToolStripStatusLabel
    Friend WithEvents tmLister As System.Windows.Forms.Timer
    Friend WithEvents dgv As System.Windows.Forms.DataGridView
    Friend WithEvents Label2 As Label
    Friend WithEvents Label3 As Label
    Friend WithEvents dtFrom As DateTimePicker
    Friend WithEvents dtTo As DateTimePicker
    Friend WithEvents bgProcessPayroll As System.ComponentModel.BackgroundWorker
    Friend WithEvents ProgressBar1 As ProgressBar
    Friend WithEvents tmWatcher As Timer
    Friend WithEvents clFileName As DataGridViewTextBoxColumn
    Friend WithEvents clStatus As DataGridViewTextBoxColumn
End Class
