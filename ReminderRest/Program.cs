using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReminderRest
{
    internal static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {  // 设置当前进程的 AppUserModelID
            SetCurrentProcessExplicitAppUserModelID("WorkReminderApp");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }


        // 引入 SetCurrentProcessExplicitAppUserModelID 函数
        [System.Runtime.InteropServices.DllImport("shell32.dll")]
        private static extern int SetCurrentProcessExplicitAppUserModelID([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPWStr)] string AppID);
    }
}
