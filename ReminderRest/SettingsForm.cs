using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace ReminderRest
{
    public partial class SettingsForm : Form
    {
        internal Action<string> actionMsg;

        public SettingsForm()
        {
            InitializeComponent();
            this.btnSave.Click += BtnSave_Click;
            this.btnCancel.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
            };
            this.StartPosition = FormStartPosition.Manual; // 自定义位置
                                                           // 获取主屏幕工作区大小（不包含任务栏）
            Rectangle workingArea = Screen.PrimaryScreen.WorkingArea;

            // 设置窗体位置（右下角，任务栏上方）
            this.Location = new Point(
                workingArea.Right - this.Width - 10, // 离右边 10px
                workingArea.Bottom - this.Height - 10 // 离底部 10px
            );
            this.Load+=(s,e)=>
            {
                this.txtRestMinutes.actionMsg = this.actionMsg;
                this.txtWorkMinutes.actionMsg = this.actionMsg;
                this.txtWorkHours.actionMsg = this.actionMsg;
            };
        }

        public int WorkMinutes
        {
            get
            {

                if (string.IsNullOrWhiteSpace(this.txtWorkMinutes.Text))
                    return 0;
                return int.Parse(this.txtWorkMinutes.Text);
            }
            set { this.txtWorkMinutes.Text = value.ToString(); }
        }
        public int RestMinutes
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.txtRestMinutes.Text))
                    return 0;
                return int.Parse(this.txtRestMinutes.Text);
            }
            set { this.txtRestMinutes.Text = value.ToString(); }
        }
        public int WorkHour
        {
            get
            {
                if (string.IsNullOrWhiteSpace(this.txtWorkHours.Text))
                    return 0;
                return int.Parse(this.txtWorkHours.Text);
            }
            set { this.txtWorkHours.Text = value.ToString(); }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (WorkMinutes < 1)
            {
                //MessageBox.Show("请输入有效的工作时长（分钟）大于1的正整数！", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                actionMsg?.Invoke("请输入有效的工作时长（分钟）大于1的正整数！");
                return;
            }
            if (RestMinutes < 1)
            {
                //MessageBox.Show("请输入有效的休息时长（分钟）大于1的正整数！", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                actionMsg?.Invoke("请输入有效的休息时长（分钟）大于1的正整数！");
                return;
            }
            if (WorkHour < 1)
            {
                //MessageBox.Show("请输入有效的每日工作时长（小时）大于1的正整数！", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                actionMsg?.Invoke("请输入有效的每日工作时长（小时）大于1的正整数！");
                return;
            }
             
            SaveWorkTime();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SaveWorkTime()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["WorkMinutes"].Value = WorkMinutes.ToString();
            config.AppSettings.Settings["RestMinutes"].Value = RestMinutes.ToString();
            config.AppSettings.Settings["WorkHour"].Value = WorkHour.ToString();
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }
    }
}
