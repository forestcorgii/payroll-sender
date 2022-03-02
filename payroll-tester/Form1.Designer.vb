<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
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
        Me.DataGridView1 = New System.Windows.Forms.DataGridView()
        Me.Column2 = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.btnEditFacialDir = New System.Windows.Forms.Button()
        Me.btnEditHRDir = New System.Windows.Forms.Button()
        Me.FBD = New System.Windows.Forms.FolderBrowserDialog()
        Me.btnSave = New System.Windows.Forms.Button()
        Me.tbHRDir = New System.Windows.Forms.TextBox()
        Me.tbFacialDir = New System.Windows.Forms.TextBox()
        Me.btnReport = New System.Windows.Forms.Button()
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'DataGridView1
        '
        Me.DataGridView1.AllowUserToAddRows = False
        Me.DataGridView1.AllowUserToDeleteRows = False
        Me.DataGridView1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridView1.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.Column2})
        Me.DataGridView1.Location = New System.Drawing.Point(12, 72)
        Me.DataGridView1.MultiSelect = False
        Me.DataGridView1.Name = "DataGridView1"
        Me.DataGridView1.RowHeadersVisible = False
        Me.DataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridView1.Size = New System.Drawing.Size(553, 235)
        Me.DataGridView1.TabIndex = 0
        '
        'Column2
        '
        Me.Column2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.Column2.HeaderText = "DBF"
        Me.Column2.Name = "Column2"
        Me.Column2.ReadOnly = True
        '
        'btnEditFacialDir
        '
        Me.btnEditFacialDir.Location = New System.Drawing.Point(12, 13)
        Me.btnEditFacialDir.Name = "btnEditFacialDir"
        Me.btnEditFacialDir.Size = New System.Drawing.Size(151, 23)
        Me.btnEditFacialDir.TabIndex = 2
        Me.btnEditFacialDir.Text = "Edit Facial Rec Directory"
        Me.btnEditFacialDir.UseVisualStyleBackColor = True
        '
        'btnEditHRDir
        '
        Me.btnEditHRDir.Location = New System.Drawing.Point(12, 42)
        Me.btnEditHRDir.Name = "btnEditHRDir"
        Me.btnEditHRDir.Size = New System.Drawing.Size(151, 23)
        Me.btnEditHRDir.TabIndex = 3
        Me.btnEditHRDir.Text = "Edit HR DBF Directory"
        Me.btnEditHRDir.UseVisualStyleBackColor = True
        '
        'btnSave
        '
        Me.btnSave.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.btnSave.Location = New System.Drawing.Point(490, 313)
        Me.btnSave.Name = "btnSave"
        Me.btnSave.Size = New System.Drawing.Size(75, 23)
        Me.btnSave.TabIndex = 4
        Me.btnSave.Text = "Run"
        Me.btnSave.UseVisualStyleBackColor = True
        '
        'tbHRDir
        '
        Me.tbHRDir.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbHRDir.Location = New System.Drawing.Point(169, 44)
        Me.tbHRDir.Name = "tbHRDir"
        Me.tbHRDir.Size = New System.Drawing.Size(396, 20)
        Me.tbHRDir.TabIndex = 5
        '
        'tbFacialDir
        '
        Me.tbFacialDir.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.tbFacialDir.Location = New System.Drawing.Point(169, 15)
        Me.tbFacialDir.Name = "tbFacialDir"
        Me.tbFacialDir.Size = New System.Drawing.Size(396, 20)
        Me.tbFacialDir.TabIndex = 6
        '
        'btnReport
        '
        Me.btnReport.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnReport.Location = New System.Drawing.Point(12, 313)
        Me.btnReport.Name = "btnReport"
        Me.btnReport.Size = New System.Drawing.Size(151, 23)
        Me.btnReport.TabIndex = 7
        Me.btnReport.Text = "Create Overall Report"
        Me.btnReport.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(577, 340)
        Me.Controls.Add(Me.btnReport)
        Me.Controls.Add(Me.tbFacialDir)
        Me.Controls.Add(Me.tbHRDir)
        Me.Controls.Add(Me.btnSave)
        Me.Controls.Add(Me.btnEditHRDir)
        Me.Controls.Add(Me.btnEditFacialDir)
        Me.Controls.Add(Me.DataGridView1)
        Me.Name = "Form1"
        Me.Text = "Form1"
        CType(Me.DataGridView1, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents DataGridView1 As DataGridView
    Friend WithEvents btnEditFacialDir As Button
    Friend WithEvents btnEditHRDir As Button
    Friend WithEvents FBD As FolderBrowserDialog
    Friend WithEvents btnSave As Button
    Friend WithEvents tbHRDir As TextBox
    Friend WithEvents tbFacialDir As TextBox
    Friend WithEvents Column2 As DataGridViewTextBoxColumn
    Friend WithEvents btnReport As Button
End Class
