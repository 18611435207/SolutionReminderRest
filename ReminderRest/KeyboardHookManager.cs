using System;
using System.Collections.Generic;
using System.Diagnostics;   // 用于获取当前进程信息
using System.Linq;
using System.Net;
using System.Runtime.InteropServices; // 用于调用 WinAPI
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms; // 用于键盘按键枚举 Keys

namespace ReminderRest
{
    /// <summary>
    /// 全局键盘钩子管理类
    /// - 通过 Windows API 设置一个全局低级键盘钩子
    /// - 可以拦截键盘输入，实现屏蔽或自定义处理
    /// </summary>
    public class KeyboardHookManager
    {
        // ===== 全局键盘钩子相关 =====
        private static IntPtr hookId = IntPtr.Zero; // 保存钩子句柄（用于卸载）
        private static LowLevelKeyboardProc proc = HookCallback; // 保存钩子回调，防止被 GC 回收

        // ===== WinAPI 声明 =====

        /// <summary>
        /// 安装钩子
        /// </summary>
        /// <param name="idHook">钩子类型 (13 = WH_KEYBOARD_LL, 即全局低级键盘钩子)</param>
        /// <param name="lpfn">回调函数委托</param>
        /// <param name="hMod">包含回调函数的模块句柄</param>
        /// <param name="dwThreadId">线程 ID（0 = 全局钩子）</param>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        /// <summary>
        /// 卸载钩子
        /// </summary>
        /// <param name="hhk">钩子句柄</param>
        /// <returns>true 表示成功</returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        /// <summary>
        /// 将钩子信息传递给下一个钩子（保持链条）
        /// </summary>
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 获取指定模块的句柄
        /// </summary>
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        // 定义低级键盘钩子委托
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

        // ===== 设置钩子的方法 =====
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            // 获取当前进程和主模块
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                // 安装低级键盘钩子 (WH_KEYBOARD_LL = 13)
                return SetWindowsHookEx(13, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        // 放在类里
        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;      // 用这个判断 Alt 是否按下
            public int time;
            public IntPtr dwExtraInfo;
        }

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104; // 带 Alt 的按键
        private const int WM_SYSKEYUP = 0x0105;
        private const int LLKHF_ALTDOWN = 0x20;   // Alt 被按下时置位

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                var kbd = Marshal.PtrToStructure<KBDLLHOOKSTRUCT>(lParam);
                bool isDown = (wParam == (IntPtr)WM_KEYDOWN) || (wParam == (IntPtr)WM_SYSKEYDOWN);
                bool altDown = (kbd.flags & LLKHF_ALTDOWN) != 0;

                // 1) 放行 Alt 自身（否则后续 Alt+Tab 根本不会成立）
                if (kbd.vkCode == (int)Keys.LMenu || kbd.vkCode == (int)Keys.RMenu)
                    return CallNextHookEx(hookId, nCode, wParam, lParam);

                //// 2) 放行 Alt+Tab（包含 Alt+Shift+Tab）
                //if (isDown && kbd.vkCode == (int)Keys.Tab && altDown)
                //    return CallNextHookEx(hookId, nCode, wParam, lParam);

                // 3) 其它按键：只在“按下”时拦截；抬起一律放行以免“卡键”
                if (isDown)
                    return (IntPtr)1;

                return CallNextHookEx(hookId, nCode, wParam, lParam);
            }

            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }


        //// ===== 钩子回调函数 =====
        ///// <summary>
        ///// 当有键盘事件发生时，Windows 会调用此方法
        ///// </summary>
        //private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        //{
        //    // nCode >= 0 表示有键盘消息需要处理
        //    if (nCode >= 0)
        //    {
        //        // 从 lParam 读取按键的虚拟键码 (VK Code)
        //        int vkCode = Marshal.ReadInt32(lParam);

        //        // 转换为 Keys 枚举
        //        Keys key = (Keys)vkCode;

        //        // 获取修饰键（Ctrl、Alt、Shift 等）
        //        Keys modifiers = Control.ModifierKeys;

        //        // ===== 示例：允许 Alt+Tab 不被屏蔽 =====
        //        if (key == Keys.Tab && modifiers == Keys.Alt)
        //        {
        //            return CallNextHookEx(hookId, nCode, wParam, lParam);
        //        }

        //        // 默认情况：屏蔽所有键盘输入（返回非零值）
        //        return (IntPtr)1;
        //    }

        //    // 如果没有处理，继续传递给下一个钩子
        //    return CallNextHookEx(hookId, nCode, wParam, lParam);
        //}

        // ===== 公共方法 =====

        /// <summary>
        /// 安装键盘钩子
        /// </summary>
        public static void InstallHook()
        {
            if (hookId == IntPtr.Zero) // 避免重复安装
            {
                hookId = SetHook(proc);
            }
        }

        /// <summary>
        /// 卸载键盘钩子
        /// </summary>
        public static void UnInstallHook()
        {
            if (hookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hookId);
                hookId = IntPtr.Zero;
            }
        }
    }
}
