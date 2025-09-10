namespace ReminderRest
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtWorkMinutes = new ReminderRest.CustomerControl.PositiveIntTextBox();
            this.txtRestMinutes = new ReminderRest.CustomerControl.PositiveIntTextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtWorkHours = new ReminderRest.CustomerControl.PositiveIntTextBox();
            this.txtNoonBreakMinutes = new ReminderRest.CustomerControl.PositiveIntTextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbMinute = new System.Windows.Forms.ComboBox();
            this.cmbHour = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(113, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "工作时长（分钟）：";
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnSave.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.ForeColor = System.Drawing.Color.White;
            this.btnSave.Location = new System.Drawing.Point(102, 177);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(63, 23);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = "保存";
            this.btnSave.UseVisualStyleBackColor = false;
            // 
            // btnCancel
            // 
            this.btnCancel.BackColor = System.Drawing.Color.DodgerBlue;
            this.btnCancel.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btnCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancel.ForeColor = System.Drawing.Color.White;
            this.btnCancel.Location = new System.Drawing.Point(185, 177);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(40, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "取消";
            this.btnCancel.UseVisualStyleBackColor = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 45);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(113, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "休息时长（分钟）：";
            // 
            // txtWorkMinutes
            // 
            this.txtWorkMinutes.Location = new System.Drawing.Point(159, 13);
            this.txtWorkMinutes.Name = "txtWorkMinutes";
            this.txtWorkMinutes.Size = new System.Drawing.Size(101, 21);
            this.txtWorkMinutes.TabIndex = 4;
            // 
            // txtRestMinutes
            // 
            this.txtRestMinutes.Location = new System.Drawing.Point(159, 42);
            this.txtRestMinutes.Name = "txtRestMinutes";
            this.txtRestMinutes.Size = new System.Drawing.Size(101, 21);
            this.txtRestMinutes.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(137, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "今天考勤时长（小时）：";
            // 
            // txtWorkHours
            // 
            this.txtWorkHours.Location = new System.Drawing.Point(159, 75);
            this.txtWorkHours.Name = "txtWorkHours";
            this.txtWorkHours.Size = new System.Drawing.Size(101, 21);
            this.txtWorkHours.TabIndex = 4;
            // 
            // txtNoonBreakMinutes
            // 
            this.txtNoonBreakMinutes.Location = new System.Drawing.Point(159, 141);
            this.txtNoonBreakMinutes.Name = "txtNoonBreakMinutes";
            this.txtNoonBreakMinutes.Size = new System.Drawing.Size(101, 21);
            this.txtNoonBreakMinutes.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 144);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "午休时长（分钟）：";
            // 
            // cmbMinute
            // 
            this.cmbMinute.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbMinute.FormattingEnabled = true;
            this.cmbMinute.ItemHeight = 12;
            this.cmbMinute.Location = new System.Drawing.Point(215, 108);
            this.cmbMinute.Name = "cmbMinute";
            this.cmbMinute.Size = new System.Drawing.Size(45, 20);
            this.cmbMinute.TabIndex = 7;
            // 
            // cmbHour
            // 
            this.cmbHour.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbHour.FormattingEnabled = true;
            this.cmbHour.Location = new System.Drawing.Point(159, 108);
            this.cmbHour.Name = "cmbHour";
            this.cmbHour.Size = new System.Drawing.Size(46, 20);
            this.cmbHour.TabIndex = 8;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(57, 111);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 12);
            this.label5.TabIndex = 5;
            this.label5.Text = "午休开始时间：";
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(276, 214);
            this.Controls.Add(this.cmbMinute);
            this.Controls.Add(this.cmbHour);
            this.Controls.Add(this.txtNoonBreakMinutes);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtWorkHours);
            this.Controls.Add(this.txtRestMinutes);
            this.Controls.Add(this.txtWorkMinutes);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "SettingsForm";
            this.Text = "StartWorkTimeForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label2;
        private CustomerControl.PositiveIntTextBox txtWorkMinutes;
        private CustomerControl.PositiveIntTextBox txtRestMinutes;
        private System.Windows.Forms.Label label3;
        private CustomerControl.PositiveIntTextBox txtWorkHours;
        private CustomerControl.PositiveIntTextBox txtNoonBreakMinutes;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbMinute;
        private System.Windows.Forms.ComboBox cmbHour;
        private System.Windows.Forms.Label label5;
    }
}