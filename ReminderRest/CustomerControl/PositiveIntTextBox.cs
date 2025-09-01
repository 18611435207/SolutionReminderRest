using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReminderRest.CustomerControl
{
    public class PositiveIntTextBox : TextBox
    {
        public PositiveIntTextBox()
        {
            this.KeyPress += PositiveIntTextBox_KeyPress;
            this.Leave += PositiveIntTextBox_Leave;
        }

        private void PositiveIntTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            // 允许输入数字和退格
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true; // 拦截非数字
            }
            // 只允许输入正整数，禁止输入 '0' 开头
            if (this.Text.Length == 0 && e.KeyChar == '0')
            {
                e.Handled = true; // 拦截 '0' 开头
            }
            // 限制输入长度，防止溢出
            if (this.Text.Length >= 10 && !char.IsControl(e.KeyChar))
            {
                e.Handled = true; // 限制最大长度为 10
            }
            // 允许输入回车键，触发离开事件
            if (e.KeyChar == (char)Keys.Enter)
            {
                this.Parent.SelectNextControl(this, true, true, true, true);
                e.Handled = true; // 拦截回车
            }
            //拦截Ctrl+V
            if (e.KeyChar == 22) // Ctrl+V 的 ASCII 码是 22
            {
                e.Handled = true; // 拦截粘贴
            }
        }

        private void PositiveIntTextBox_Leave(object sender, EventArgs e)
        {
            if (int.TryParse(this.Text, out int value))
            {
                if (value <= 1)
                {
                    MessageBox.Show("请输入大于 1 的正整数！", "输入错误", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Text = "";
                }
            }
            else
            {
                this.Text = "";
            }
        }

        /// <summary>
        /// 获取数值，未输入有效数字时返回 null
        /// </summary>
        public int? IntValue
        {
            get
            {
                if (int.TryParse(this.Text, out int value) && value > 1)
                {
                    return value;
                }
                return null;
            }
        }
    }
}
