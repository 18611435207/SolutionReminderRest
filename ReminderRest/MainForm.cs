using Microsoft.Win32;
using ReminderRest.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReminderRest
{
    public partial class MainForm : Form
    {
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private Timer timer;
        private int elapsedMinutes = 0;//已经工作了多少分钟
        private int totalWorkMinutes = 0;//总共工作多少分钟
        private bool isResting = false;//是否正在休息
        bool isAfterWork = false;//下班
        RestType restType = RestType.Custom;
        private int workMinutes = int.Parse(Utils.GetAppSetting("WorkMinutes", "30") ?? "30");//默认30分钟

        private int restMinutes = int.Parse(Utils.GetAppSetting("RestMinutes", "5") ?? "5");//默认5分钟

        private int workHour = int.Parse(Utils.GetAppSetting("WorkHour", "9") ?? "9");//默认9小时

        //午休 NoonBreakMinutes
        private int noonBreakMinutes = int.Parse(Utils.GetAppSetting("NoonBreakMinutes", "60") ?? "60");//默认60分钟

        //NoonBreakHours 午休时间小时
        private int noonBreakHour = int.Parse(Utils.GetAppSetting("NoonBreakHour", "11") ?? "11");//从几点开始
        private int noonBreakMin = int.Parse(Utils.GetAppSetting("NoonBreakMin", "30") ?? "30");//从几分开始

        public MainForm()
        {
            InitializeComponent();
            this.Load += MainForm_Load;
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false; // 不显示在任务栏
            this.Hide();
            // 托盘菜单
            trayMenu = new ContextMenuStrip();

            trayMenu.Items.Add("打卡上班时间", null, (s, e) =>
            {
                using (var f = new StartWorkTimeForm() { onSaveMsg = ShowTooltip })
                {
                    if (f.ShowDialog() == DialogResult.OK)
                    {

                        ShowTooltip("打卡上班时间成功");
                    }
                }
            });
            //设置生日日期
            trayMenu.Items.Add("设置生日", null, (s, e) =>
            {
                using (var f = new StartWorkTimeForm(1))
                {
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        ShowTooltip("生日日期设置成功");
                    }
                }
            });

            trayMenu.Items.Add("设置休息间隔", null, (s, e) =>
            {
                using (var f = new SettingsForm())
                {
                    f.WorkMinutes = workMinutes;
                    f.RestMinutes = restMinutes;
                    f.WorkHour = workHour;
                    f.NoonBreakMinutes = noonBreakMinutes;
                    f.NoonBreakHour = noonBreakHour;
                    f.NoonBreakMin = noonBreakMin;
                    f.actionMsg = ShowTooltip;
                    if (f.ShowDialog() == DialogResult.OK)
                    {
                        workMinutes = f.WorkMinutes;
                        restMinutes = f.RestMinutes;
                        noonBreakMinutes = f.NoonBreakMinutes;
                        workHour = f.WorkHour;
                        noonBreakHour = f.NoonBreakHour;
                        noonBreakMin = f.NoonBreakMin;
                        ShowTooltip("设置成功");
                    }
                }
            });
            //trayMenu.Items.Add("停止休息", null, StopRest);
            trayMenu.Items.Add("开始休息", null, (s, e) =>
            {
                if (!isResting)
                {
                    restType = RestType.WorkRest;
                    HaveARestNow(restType);
                }
            });

            trayMenu.Items.Add("开始午休", null, (s, e) =>
            {
                StopRest(null, null);//如果在休息中，先停止休息
                restType = RestType.NoonBreak;
                HaveARestNow(RestType.NoonBreak);
            });

            trayMenu.Items.Add("下班", null, (s, e) =>
            {
                AfterWorkNow();
            });
            trayMenu.Items.Add("退出", null, ExitApp);

            // 托盘图标
            trayIcon = new NotifyIcon();
            trayIcon.Text = isResting ? $"正在休息，还剩{restMinutes - elapsedMinutes}分钟" : $"工作中，距离休息还有{workMinutes - elapsedMinutes}分钟";
            //using (var bmp = Properties.Resources.哞哞兽LOGO)  // 这是 Bitmap
            //{
            //    IntPtr hIcon = bmp.GetHicon();  // 生成 GDI 图标句柄
            //    trayIcon.Icon = Icon.FromHandle(hIcon);  // 转换成 Icon
            //} 
            trayIcon.Icon = Properties.Resources.cow1; // 直接使用 Icon 资源
            trayIcon.Visible = true;
            //最小化时显示气泡提示

            trayIcon.ContextMenuStrip = trayMenu;


            // 设置气泡提示内容
            trayIcon.BalloonTipTitle = "哞哞休息提醒~~";
            trayIcon.BalloonTipText = "程序已在后台运行，会提醒你定时休息哦~";
            trayIcon.BalloonTipIcon = ToolTipIcon.Info;

            // 🚀 程序启动时，显示一次托盘气泡提示（3 秒）
            trayIcon.ShowBalloonTip(3000);


            // 计时器
            timer = new Timer();
            timer.Interval = 60 * 1000; // 每分钟触发一次
            timer.Tick += Timer_Tick;
            timer.Start();

            //每次点击PictureBox  都让整个Form随机换位置
            this.pictureBox1.Click += (s, e) =>
            {
                Random rand = new Random();
                int x = rand.Next(0, Screen.PrimaryScreen.WorkingArea.Width - this.Width);
                int y = rand.Next(0, Screen.PrimaryScreen.WorkingArea.Height - this.Height);
                this.Location = new System.Drawing.Point(x, y);
            };
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            CheckFirstRun();
        }
        RemindLater remindLater = null;
        private void Timer_Tick(object sender, EventArgs e)
        {

            //It's time to have lunch
            totalWorkMinutes += 1;
            HaveLunch();

            if (!isAfterWork&& restType==RestType.Custom)//如果下班或者休息了，这里就不继续计时了
            {
                elapsedMinutes++;
                //显示NotifyIcon 还有多少分钟休息
                trayIcon.Text = isResting ? $"正在休息，还剩{restMinutes - elapsedMinutes}分钟" : $"工作中，距离休息还有{workMinutes - elapsedMinutes}分钟";
                if (!isResting && elapsedMinutes >= workMinutes)
                {
                    //时间到，开始休息
                    restType = RestType.WorkRest;
                    HaveARestNow(restType);
                }

                int remaining = workMinutes - elapsedMinutes;
                if (remaining == 1)
                {
                    trayIcon.BalloonTipTitle = "休息提醒";
                    trayIcon.BalloonTipText = "还有 1 分钟就要进入休息了，请准备~";
                    trayIcon.ShowBalloonTip(10000);

                    if (remindLater == null)
                    {
                        remindLater = new RemindLater()
                        {
                            StartPosition = FormStartPosition.Manual,
                            Location = new System.Drawing.Point(
                                Screen.PrimaryScreen.WorkingArea.Right - 300 - 10,
                                Screen.PrimaryScreen.WorkingArea.Bottom - 250 - 10)

                        };
                        remindLater.OnRemindLater += (s, e1) =>
                        {
                            workMinutes += e1; // 计时增加
                            trayIcon.BalloonTipTitle = "休息提醒";
                            trayIcon.BalloonTipText = $"已延迟休息，计时已添加[{e1}]分钟~";
                            trayIcon.ShowBalloonTip(10000);
                            remindLater = null;
                        };
                        remindLater.FormClosed += (s, e1) => { remindLater = null; };
                        remindLater.Show();


                    }
                }
            }

            //当前时间 与 endWork 在5分钟内 提示马上下班了
            if (!isAfterWork && endWork != null)
            {
                TimeSpan toEnd = endWork.Value - DateTime.Now;
                if (toEnd.TotalMinutes <= 5 && toEnd.TotalMinutes > 4)
                {
                    trayIcon.BalloonTipTitle = "下班提醒";
                    trayIcon.BalloonTipText = "距离下班还有5分钟，请做好收工准备~";
                    trayIcon.ShowBalloonTip(10000);
                }
                else if (toEnd.TotalMinutes <= 1 && toEnd.TotalMinutes > 0)
                {
                    trayIcon.BalloonTipTitle = "下班提醒";
                    trayIcon.BalloonTipText = "距离下班还有1分钟，请做好收工准备~";
                    trayIcon.ShowBalloonTip(10000);
                }
            }


            if (!isAfterWork)
            {
                CheckWorkTime();
            }
            //每20分钟提醒一下喝水
            if (totalWorkMinutes % 20 == 0)
            {
                trayIcon.BalloonTipTitle = "喝水提醒~~";
                trayIcon.BalloonTipText = "记得喝杯水补充水分哦~";
                trayIcon.BalloonTipIcon = ToolTipIcon.Info;
                trayIcon.ShowBalloonTip(10000);
            }
        }

        private void HaveLunch()
        {
            //如果现在是11点30分，提醒一下该吃饭了
            if (DateTime.Now.Hour == noonBreakHour && DateTime.Now.Minute == (noonBreakMin-1))
            {
                trayIcon.BalloonTipTitle = "午餐时间到~~";
                trayIcon.BalloonTipText = "现在是11点29分，记得还有1分钟吃午饭哦~";
                trayIcon.BalloonTipIcon = ToolTipIcon.Info;
                trayIcon.ShowBalloonTip(10000);
            }
            else if (DateTime.Now.Hour == noonBreakHour && DateTime.Now.Minute == noonBreakMin)
            {
                trayIcon.BalloonTipTitle = "午餐时间到~~";
                trayIcon.BalloonTipText = "现在是11点30分，记得吃午饭哦~";
                trayIcon.BalloonTipIcon = ToolTipIcon.Info;
                trayIcon.ShowBalloonTip(10000);

                StopRest(null, null);//如果在休息中，先停止休息
                restType = RestType.NoonBreak;
                HaveARestNow(restType);
            }
        }

        // 记录所有休息窗体
        private List<RestForm> restForms = new List<RestForm>();

        public void HaveARestNow(RestType restType)
        {
            //timer.Stop();
            // 进入休息
            if (isAfterWork)
                return;
            isResting = true;
            elapsedMinutes = 0;
             KeyboardHookManager.InstallHook(); // 安装键盘钩子，屏蔽键盘输入
                                                // 设置气泡提示内容 
            if (restType == RestType.NoonBreak)
            {
                restMinutes = noonBreakMinutes; //午休1小时 
                trayIcon.BalloonTipText = $"进入{restMinutes}分钟 午间休息时间 快去吃饭吧~";
            }
            else
            {
                restMinutes = int.Parse(Utils.GetAppSetting("RestMinutes", "5") ?? "5");
                trayIcon.BalloonTipText = $"进入{restMinutes}分钟 休息时间 请劳逸结合哟~";
            }
            trayIcon.BalloonTipTitle = "哞哞休息提醒~~"; 
            trayIcon.BalloonTipIcon = ToolTipIcon.Warning;
            trayIcon.ShowBalloonTip(3000);
            if (remindLater != null && !remindLater.IsDisposed)
            {
                remindLater.Close();
                remindLater = null;
            }
           
            // 每个屏幕一个 RestForm
            foreach (var screen in Screen.AllScreens)
            { 
                var restForm = new RestForm(minutes: restMinutes, false, trayIcon, restType);
                restForm.StartPosition = FormStartPosition.Manual;
                restForm.Bounds = screen.Bounds; // 覆盖整个屏幕
                 
                restForm.FormClosed += (s, args) =>
                {
                    // 只在最后一个窗口关闭时恢复计时
                    restForms.Remove(restForm);
                    if (restForms.Count == 0)
                    {
                        //timer.Start();
                        timer.Stop();
                        timer.Start();
                        isResting = false;
                        this.restType = RestType.Custom;
                        elapsedMinutes = 0; // 休息结束后重置工作计时
                        restMinutes = int.Parse(Utils.GetAppSetting("RestMinutes", "5") ?? "5");
                        //重新读取工作时间，防止被延迟修改了
                        workMinutes = int.Parse(Utils.GetAppSetting("WorkMinutes", "30") ?? "30");
                        trayIcon.BalloonTipTitle = "哞哞休息提醒~~";
                        // 设置气泡提示内容 
                        trayIcon.BalloonTipText = "程序已在后台运行，会提醒你定时休息哦~";
                        trayIcon.BalloonTipIcon = ToolTipIcon.Info;
                        trayIcon.ShowBalloonTip(3000);
                        KeyboardHookManager.UnInstallHook(); // 卸载键盘钩子，恢复键盘输入
                    }
                };

                restForms.Add(restForm);
                restForm.Show();

            }
        }


        private void StopRest(object sender, EventArgs e)
        {
            if (isResting && restForms != null && restForms.Count > 0)
                restForms.ForEach(f => f.Close());
        }

        private void ExitApp(object sender, EventArgs e)
        {
            //确认一下
            if (MessageBox.Show("我能提醒你适度休息 你要离我而去吗？", "确认退出", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }
            trayIcon.Visible = false;
            Application.Exit();
        }

        private void SetStartup(bool enable)
        {
            try
            {
                string appName = "哞哞休息提醒";
                string exePath = Application.ExecutablePath; // 使用当前运行程序的完整路径

                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (enable)
                    {
                        key.SetValue(appName, "\"" + exePath + "\""); // 注意用双引号包裹路径
                    }
                    else
                    {
                        key.DeleteValue(appName, false);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("设置开机启动失败：" + ex.Message);
            }
        }


        private void CheckFirstRun()
        {
            const string appKey = @"Software\哞哞休息提醒";
            const string runKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
            string appName = "哞哞休息提醒";
            string exePath = "\"" + Application.ExecutablePath + "\""; // 当前 EXE 路径，带双引号

            using (RegistryKey key = Registry.CurrentUser.CreateSubKey(appKey))
            {
                object hasRun = key.GetValue("HasRunBefore");
                if (hasRun == null)
                {
                    // 首次启动，设置开机启动
                    SetStartup(true);

                    // 标记已经运行
                    key.SetValue("HasRunBefore", "1");
                }
                else
                {
                    // 检查注册表开机启动路径是否正确
                    using (RegistryKey run = Registry.CurrentUser.OpenSubKey(runKey, true))
                    {
                        object value = run.GetValue(appName);
                        if (value == null || value.ToString() != exePath)
                        {
                            // 注册表不存在或路径已改变，更新为当前路径
                            run.SetValue(appName, exePath);
                        }
                    }
                }
            }
        }

        #region 下班
        private void ClearWorkTime()
        {
            Utils.ClearAppSettingsValue("StartWorkTime", "");
        }

        private void ShowTooltip(string message)
        {
            trayIcon.BalloonTipTitle = "提示";
            trayIcon.BalloonTipText = message;
            trayIcon.ShowBalloonTip(6000);
        }

        DateTime? lastOffWorkPrompt = null;
        private DateTime? endWork = null;
        private void CheckWorkTime()
        {
            if (endWork == null)
            {
                if (lastOffWorkPrompt == null)
                    lastOffWorkPrompt = DateTime.Now;
                else if (lastOffWorkPrompt.Value.Day != DateTime.Now.Day)
                    lastOffWorkPrompt = DateTime.Now;
                else if (lastOffWorkPrompt.Value.Hour == DateTime.Now.Hour)
                    return;
                else
                    lastOffWorkPrompt = DateTime.Now;
            }
            string startWorkStr = Util.Utils.GetAppSetting("StartWorkTime");
            if (string.IsNullOrEmpty(startWorkStr))
            {
                ShowTooltip("未设置上班时间  请在托盘图标上右键设置今天打卡时间");
                return;
            }

            if (!DateTime.TryParse(startWorkStr, out DateTime startWork))
            {
                ShowTooltip("未设置上班时间  请在托盘图标上右键设置今天打卡时间");
                return;
            }

            // 判断是否跨天
            if (startWork.Date != DateTime.Now.Date)
            {
                ClearWorkTime();
                ShowTooltip("未设置上班时间  请在托盘图标上右键设置今天打卡时间");
                return;
            }

            endWork = startWork.AddHours(workHour).AddMinutes(1);

            if (DateTime.Now >= endWork)
            {

                AfterWorkNow();
            }
        }

        public void AfterWorkNow()
        {
            if (!isAfterWork)
            {
                isAfterWork = true;
                //timer.Stop();
                // 下班后进入休息，直到第二天8点
                DateTime tomorrow9 = DateTime.Today.AddDays(1).AddHours(9);
                TimeSpan restTime = tomorrow9 - DateTime.Now;
                // 弹出下班窗口
                ShowOffWorkForm(restTime.TotalMinutes);
            }
        }

        private void ShowOffWorkForm(double totalMinutes)
        {
            KeyboardHookManager.InstallHook(); // 安装键盘钩子，屏蔽键盘输入
            // 每个屏幕一个 RestForm
            foreach (var screen in Screen.AllScreens)
            {
                var restForm = new RestForm(minutes: Convert.ToInt32(totalMinutes), isAfterWork, trayIcon);
                restForm.StartPosition = FormStartPosition.Manual;
                restForm.Bounds = screen.Bounds; // 覆盖整个屏幕

                restForm.FormClosed += (s, args) =>
                {
                    // 只在最后一个窗口关闭时恢复计时
                    restForms.Remove(restForm);
                    if (restForms.Count == 0)
                    {
                        //timer.Start();
                        isResting = false;
                        elapsedMinutes = 0; // 休息结束后重置工作计时
                        restMinutes = int.Parse(Utils.GetAppSetting("RestMinutes", "5") ?? "5");
                        trayIcon.BalloonTipTitle = "哞哞休息提醒~~";
                        // 设置气泡提示内容 
                        trayIcon.BalloonTipText = "程序已在后台运行，会提醒你定时休息哦~";
                        trayIcon.BalloonTipIcon = ToolTipIcon.Info;
                        trayIcon.ShowBalloonTip(3000); 
                        isAfterWork = false;
                        KeyboardHookManager.UnInstallHook(); // 卸载键盘钩子，恢复键盘输入
                    }
                };

                restForms.Add(restForm);
                restForm.Show();
            }
        }


        #endregion



    }
}
