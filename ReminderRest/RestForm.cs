using Microsoft.Win32;
using ReminderRest.Util;
using System;
using System.Configuration;
using System.Drawing;
using System.Windows.Forms;

namespace ReminderRest
{
    public partial class RestForm : Form
    {
        private Timer restTimer;
        private TimeSpan remainingTime;

        private bool isAfterWork = false;

        private NotifyIcon NotifyIcon;
        int workHour = int.TryParse(Util.Utils.GetAppSetting("WorkHour"), out int wh) ? wh : 9;
        string startWorkStr = ConfigurationManager.AppSettings["StartWorkTime"];
        string MaxAgeAvg = Util.Utils.GetAppSetting("MaxAgeAVG");
        string MaxAgeMan = Util.Utils.GetAppSetting("MaxAgeMan");
        string MaxAgeWoman = Util.Utils.GetAppSetting("MaxAgeWoman");
        string Sex = Util.Utils.GetAppSetting("Sex");
        int StopWorkAgeMan = int.TryParse(Util.Utils.GetAppSetting("StopWorkAgeMan"), out int swa) ? swa : 60;
        int StopWorkAgeWoman = int.TryParse(Util.Utils.GetAppSetting("StopWorkAgeWoman"), out int swaw) ? swaw : 50;
        //int daysStopWork = 0;
        string BirthDayStr = Util.Utils.GetAppSetting("BirthDay");


        public RestForm(int minutes = 10, bool isAfterWork = false, NotifyIcon trayIcon = null)
        {
            InitializeComponent();
            labelAfterwork.Text = isAfterWork ? "哞哞辛苦了 已经下班喽！" : "哞哞辛苦了 休息一下吧！";
            this.NotifyIcon = trayIcon;
            this.isAfterWork = isAfterWork;
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            //this.BackColor = System.Drawing.Color.Black;
            this.Opacity = 0.95;
            this.ShowInTaskbar = false;
            this.Load += RestForm_Load;
            this.lblClose.Click += LblClose_Click;

            //如果电脑名称包含特定字符串 则TopMost为False，保留键盘使用
            if (Environment.UserName.Contains("LarryYu"))
            {
                TopMost = false;
                KeyboardHookManager.UnInstallHook(); // 卸载键盘钩子
            }

            SetBackFromWallpaper();

            StartRest(minutes);

            ShowAvgAge();

            SetProgressBar();



            this.progress.ValueChanged += (s, e) => UpdateMarkerPosition();

        }

        // ⭐新增方法：更新小人位置
        private void UpdateMarkerPosition()
        {
            if (progress.Maximum <= 0) return;

            float percent = (float)progress.Value / progress.Maximum;

            int markerX = progress.Left + (int)(progress.Width * percent) - picMarker.Width / 2;
            int markerY = progress.Top - picMarker.Height - 5; // 在进度条上方，留5px空隙

            picMarker.Location = new Point(markerX, markerY);

            lblProgress.Text = $"人生进度---{Math.Round(percent * 100, 2)}%";
            this.lblDays.Text = $"{progress.Value}/{progress.Maximum} 天";
        }

        public DateTime? GetBirthDay()
        {
            if (!string.IsNullOrWhiteSpace(BirthDayStr) && DateTime.TryParse(BirthDayStr, out DateTime birthDay))
            {
                return birthDay;
            }
            return null;
        }

        public void SetStopWorkPic()
        {
            if (!string.IsNullOrWhiteSpace(BirthDayStr) && DateTime.TryParse(BirthDayStr, out DateTime birthDay))
            {
                int stopWorkAge = 0;
                if (Sex == "1")//男
                    stopWorkAge = StopWorkAgeMan;
                else if (Sex == "2")
                    stopWorkAge = StopWorkAgeWoman;
                else
                    return;
                DateTime stopWorkDate = birthDay.AddYears(stopWorkAge);
                double targetDays = (stopWorkDate - birthDay).TotalDays;//目标天数（退休年龄-出生日期）
                TimeSpan toStopWork = stopWorkDate - DateTime.Now;
                double daysStopWork = toStopWork.TotalDays;//距离退休天数

                this.lblStopWork.Text = $"离退休({Math.Truncate(daysStopWork)})天\r\n[{stopWorkDate.ToString("yyyy-MM-dd")}]";
                //按照比例计算小人位置
                if (targetDays > 0)
                {
                    float percent = (float)(targetDays) / progress.Maximum;
                    int stopWorkX = progress.Left + (int)(progress.Width * percent) - picStopWork.Width / 2;
                    int stopWorkY = progress.Bottom + 5; // 在进度条上方，留5px空隙
                    picStopWork.Location = new Point(stopWorkX, stopWorkY);

                    this.lblStopWork.Location = new Point(stopWorkX - 10, this.lblStopWork.Location.Y);
                }

            }
        }

        public void SetProgressBar()
        {
            DateTime birthDay = DateTime.MinValue;


            if (!string.IsNullOrWhiteSpace(BirthDayStr) && DateTime.TryParse(BirthDayStr, out DateTime bd))
            {
                birthDay = bd;
            }
            else
            {
                //如果没有配置生日 就不显示进度条
                lblProgress.Text = "在托盘配置生日";
                lblDays.Text = "";
                return;
            }

            TimeSpan age = DateTime.Now - birthDay;
            int days = (int)age.TotalDays;

            // 设置渐变色
            //progress.BackColor = Color.Transparent;
            //progress.BackgroundColor = Color.DarkGray;
            progress.ForeColor = Color.White;
            progress.StartColor = Color.LimeGreen;
            progress.EndColor = Color.Red;
            progress.BorderColor = Color.Gray;
            progress.BorderWidth = 3;
            progress.IsSetText = false;
            for (int i = 0; i < days; i++)
            {
                if (this.progress.Value < this.progress.Maximum)
                {
                    this.progress.Value = i;
                }
            }
        }

        public void ShowAvgAge()
        {
            //计算年龄天数
            string MaxAge = MaxAgeAvg;
            if (Sex == "1" && !string.IsNullOrEmpty(MaxAgeMan))//男
            { MaxAge = MaxAgeMan; }
            else if (Sex == "2" && !string.IsNullOrEmpty(MaxAgeWoman))
            { MaxAge = MaxAgeWoman; }

            label1.Text = $"平均：{MaxAge}";
            if (!string.IsNullOrWhiteSpace(MaxAge) && double.TryParse(MaxAge, out double maxAge))
            {
                //计算具体天数
                if (!string.IsNullOrWhiteSpace(BirthDayStr) && DateTime.TryParse(BirthDayStr, out DateTime birthDay))
                {
                    DateTime deathDay = CalculateStopWorkDate(birthDay, maxAge);
                    TimeSpan age = DateTime.Now - birthDay;
                    int days = (int)age.TotalDays;

                    this.lblStopLife.Text = $"预期：{(Math.Truncate((deathDay - birthDay).TotalDays) - days)}天\r\n[{deathDay.ToString("yyyy-MM-dd")}]";
                    TimeSpan lifeSpan = deathDay - birthDay;
                    int totalDays = (int)lifeSpan.TotalDays;
                    progress.Maximum = totalDays;
                }
                else
                {
                    progress.Maximum = (int)(maxAge * 365.25); //默认365.25天/年
                }
            }
            else
            {
                progress.Maximum = 30000; //默认30000天
            }
        }
        DateTime CalculateStopWorkDate(DateTime birthDay, double maxAge)
        {
            // 拆成年数和小数部分
            int years = (int)Math.Floor(maxAge);
            double fraction = maxAge - years;

            // 先加整年
            DateTime result = birthDay.AddYears(years);

            // 把小数部分换算成天数（按 365.2425 天 = 1 年更准确）
            int extraDays = (int)Math.Round(fraction * 365.2425);

            result = result.AddDays(extraDays);

            return result;
        }
        //休息结束关闭窗口
        public void StartRest(int minutes)
        {
            remainingTime = TimeSpan.FromMinutes(minutes);

            restTimer = new Timer();
            restTimer.Interval = 1000; // 1少 
            UpdateLabel();

            restTimer.Tick += (s, e) =>
            {
                this.lblCurrentTime.Text = DateTime.Now.ToString("HH:mm:ss");
                if (remainingTime.TotalSeconds > 1)
                {
                    remainingTime = remainingTime.Subtract(TimeSpan.FromSeconds(1));
                    UpdateLabel();
                }
                else
                {
                    restTimer.Stop();
                    this.Close(); // 休息结束自动关闭
                }
            };
            restTimer.Start();
        }

        //设置墙纸作为背景
        public void SetBackFromWallpaper()
        {
            // 设置壁纸作为背景
            string wallpaperPath = GetWallpaperPath();
            if (!string.IsNullOrEmpty(wallpaperPath) && System.IO.File.Exists(wallpaperPath))
            {
                this.BackgroundImage = Image.FromFile(wallpaperPath);
                this.BackgroundImageLayout = ImageLayout.Stretch; // 拉伸铺满
            }
            else
            {
                this.BackColor = Color.Black; // 如果没取到壁纸就用黑色
            }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (this.BackgroundImage != null)
            {
                e.Graphics.DrawImage(this.BackgroundImage, this.ClientRectangle);
            }
            else
            {
                base.OnPaintBackground(e);
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);
            // 关闭休息窗体时，卸载键盘钩子 
            if (restTimer != null)
            {
                restTimer.Stop();
                restTimer.Dispose();
                restTimer = null;
            }
        }

        private void LblClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void UpdateLabel()
        {
            lblStopSeconds.Text = remainingTime.ToString(@"hh\:mm\:ss");
            // 格式化成 00:10:00 这种形式

            //下班倒计时

            if (!isAfterWork)
            {
                if (string.IsNullOrEmpty(startWorkStr))
                {
                    lblAfterWork.Text = "";
                    return;
                }

                if (!DateTime.TryParse(startWorkStr, out DateTime startWork))
                {
                    lblAfterWork.Text = "";
                    return;
                }
                TimeSpan toEndWork = startWork.AddHours(workHour).AddMinutes(1) - DateTime.Now;

                if (toEndWork.TotalSeconds > 0)
                {
                    if (toEndWork.Days > 0)
                        lblAfterWork.Text = $"距离下班还有 {toEndWork.Days}天 {toEndWork.Hours}小时 {toEndWork.Minutes}分钟 {toEndWork.Seconds}秒";
                    else
                        lblAfterWork.Text = $"距离下班还有 {toEndWork.Hours}小时 {toEndWork.Minutes}分钟 {toEndWork.Seconds}秒";
                }
            }
            else
                lblAfterWork.Text = "";
        }


        //计算 lblStopSeconds 的长度和高度，设置显示在窗口的正中间
        private void RestForm_Load(object sender, EventArgs e)
        {
            this.lblStopSeconds.Location = new Point((this.ClientSize.Width - lblStopSeconds.Width) / 2, (this.ClientSize.Height - lblStopSeconds.Height) / 2);
            this.lblCurrentTime.Location = new Point((this.ClientSize.Width - lblCurrentTime.Width) / 2, (this.ClientSize.Height - lblCurrentTime.Height - lblStopSeconds.Height - 250) / 2);

            //计算labelAfterwork.Text 实际宽度，并将labelAfterwork的宽度设置为实际宽度
            SizeF textSize = this.CreateGraphics().MeasureString(labelAfterwork.Text, labelAfterwork.Font);
            labelAfterwork.Width = (int)textSize.Width; // 加10像素的边距 

            this.labelAfterwork.Location = new Point((this.ClientSize.Width - labelAfterwork.Width) / 2, (this.ClientSize.Height - labelAfterwork.Height - lblStopSeconds.Height - 100) / 2);

            linkDo.Location = new Point(labelAfterwork.Right + 5, labelAfterwork.Top + labelAfterwork.Height / 2 - linkDo.Height / 2);

            lblAfterWork.Location = new Point((this.ClientSize.Width - lblAfterWork.Width) / 2, lblStopSeconds.Bottom + 5);

            this.lblProgress.Location = new Point((this.ClientSize.Width - lblProgress.Width) / 2, this.lblProgress.Top);

            this.lblDays.Location = new Point(lblProgress.Right + 10, this.lblDays.Top);

            this.progress.Width = this.Width - 140;

            this.picEnd.Location = new Point(this.progress.Right + 10, this.picEnd.Top);

            SetStopWorkPic();

            lblStopLife.Location = new Point(progress.Right - 10, this.picStopWork.Bottom);
        }

        // 读取当前 Windows 壁纸路径
        private string GetWallpaperPath()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Control Panel\Desktop", false))
                {
                    return key?.GetValue("WallPaper")?.ToString();
                }
            }
            catch
            {
                return null;
            }
        }
    }
}
