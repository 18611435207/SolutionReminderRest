using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReminderRest
{
    public partial class StartWorkTimeForm : CommonForm
    {
        int type = 0; //0工作时间 1生日设置

        public Action<string> onSaveMsg;

        public StartWorkTimeForm(int type = 0)
        {
            InitializeComponent();
            this.type = type;
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

            this.Load += StartWorkTimeForm_Load;
        }

        private void StartWorkTimeForm_Load(object sender, EventArgs e)
        {
            if (type == 1)//生日设置
            {
                cmbHour.Visible = false;
                cmbMinute.Visible = false;
                label1.Text = "生日日期:";
                this.Width = 178;
                string birthDayStr = ConfigurationManager.AppSettings["BirthDay"];
                if (!string.IsNullOrWhiteSpace(birthDayStr))
                {
                    DateTime.TryParse(birthDayStr, out DateTime birthDay);
                    dateTimePicker1.Value = birthDay;
                }

                string sex = ConfigurationManager.AppSettings["Sex"];
                if (!string.IsNullOrEmpty(sex))
                {
                    if (int.TryParse(sex, out int value))
                    {
                        if (value == 1 || value == 2)
                        {
                            cmbSex.SelectedIndex = value - 1;
                        }
                    }
                }

                return;
            }
            this.lblSex.Visible = false;
            this.cmbSex.Visible = false;
            InitCmb(23, cmbHour);
            InitCmb(59, cmbMinute);

            //给cmb加入当前时间默认值 
            DateTime startWorkTime = DateTime.Now;
            string savedTimeStr = ConfigurationManager.AppSettings["StartWorkTime"];
            if (!string.IsNullOrWhiteSpace(savedTimeStr))
            {
                DateTime.TryParse(savedTimeStr, out startWorkTime);
            }
            //如果startWorkTime的日期小于今天，则设置为今天
            if (startWorkTime.Date < DateTime.Now.Date)
            {
                startWorkTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, startWorkTime.Hour, startWorkTime.Minute, 0);
                //保存设置
                //SaveWorkTime(startWorkTime, "StartWorkTime");
            }
            dateTimePicker1.Value = startWorkTime;
            cmbHour.SelectedItem = startWorkTime.Hour.ToString("D2");
            cmbMinute.SelectedItem = startWorkTime.Minute.ToString("D2");
        }

        void InitCmb(int maxValue, ComboBox cmb)
        {
            for (int i = 0; i <= maxValue; i++)
            {
                cmb.Items.Add(i.ToString("D2"));
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (this.type == 0 && (cmbHour.SelectedItem == null || cmbMinute.SelectedItem == null))
            {
                onSaveMsg?.Invoke("请选择时间"); 
                return;
            }
            if (dateTimePicker1 == null)
            {
                //MessageBox.Show("请选择日期");
                onSaveMsg?.Invoke("请选择日期");
                return;
            }

            DateTime selectedDate = dateTimePicker1.Value.Date; // 用户选的时间

            if (type == 1)
            {
                if (cmbSex.SelectedItem == null)
                {
                    onSaveMsg?.Invoke("请选择性别"); 
                    return;
                }
                SaveWorkTime(selectedDate, "BirthDay");
                SaveStringValue((cmbSex.SelectedIndex + 1).ToString(), "Sex");
                this.DialogResult = DialogResult.OK;
                this.Close();
                return;
            }
            if (selectedDate.Date > DateTime.Now.Date)
            {
                //MessageBox.Show("打卡日期不能大于今天");
                onSaveMsg?.Invoke("打卡日期不能大于今天");
                return;
            }
            int hour = int.Parse(cmbHour.SelectedItem.ToString());
            int minute = int.Parse(cmbMinute.SelectedItem.ToString());

            DateTime selectedTime = new DateTime(
                               selectedDate.Year, selectedDate.Month, selectedDate.Day, hour, minute, 0);

            SaveWorkTime(selectedTime, "StartWorkTime");
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void SaveWorkTime(DateTime time, string keys)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[keys].Value = time.ToString("yyyy-MM-dd HH:mm");
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        private void SaveStringValue(string value, string keys)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[keys].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }




    }
}
