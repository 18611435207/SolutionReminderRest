using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReminderRest.CustomerControl
{
    public class GradientProgressBar : Control
    {
        private int value = 0;       // 当前值
        private int maximum = 100;   // 最大值
        private int targetValue = 0; // 动画目标值
        private Timer timer;
        // 新增事件
        public event EventHandler ValueChanged;

        public int Value
        {
            get => value;
            set
            {
                if (value < 0) value = 0;
                if (value > maximum) value = maximum;
                targetValue = value; // 设置动画目标值
                if (!timer.Enabled) timer.Start(); // 启动动画

                // ⭐ 新增：触发 ValueChanged 事件
                ValueChanged?.Invoke(this, EventArgs.Empty);
                Invalidate();

            }
        }

        public int Maximum
        {
            get => maximum;
            set
            {
                if (value <= 0) value = 1;
                maximum = value;
                if (this.value > maximum) this.value = maximum;
                Invalidate();
            }
        }
        public int SetpSpeed { get; set; } = 25;
        public Color StartColor { get; set; } = Color.MediumSeaGreen;
        public Color EndColor { get; set; } = Color.DodgerBlue;
        public Color BackgroundColor { get; set; } = Color.LightGray;

        public bool IsSetText { get; set; } = true;

        // 新增属性
        public Image MarkerImage { get; set; } = Properties.Resources.牛马兽LOGO;
        public Size MarkerSize { get; set; }

        // 新增属性
        public Color BorderColor { get; set; } = Color.Black;
        public int BorderWidth { get; set; } = 2;

        public GradientProgressBar()
        {
            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            this.BackColor = Color.Transparent;
            this.DoubleBuffered = true;
            this.Size = new Size(200, 30);
            MarkerSize = new Size(Height, Height);
            timer = new Timer();
            timer.Interval = SetpSpeed; // 刷新速度（越小越流畅）
            timer.Tick += (s, e) =>
            {
                if (value < targetValue)
                {
                    value += Math.Max(1, (targetValue - value) / 10); // 逐步逼近
                    if (value >= targetValue)
                    {
                        value = targetValue; timer.Stop();
                    }

                }
                else if (value > targetValue)
                {
                    value -= Math.Max(1, (value - targetValue) / 10);
                    if (value <= targetValue)
                    {
                        value = targetValue; timer.Stop();
                    }
                }

                Invalidate();
                // ⭐ 每一帧都通知外部
                ValueChanged?.Invoke(this, EventArgs.Empty);
            };

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            int borderRadius = this.Height;
            Rectangle rect = new Rectangle(0, 0, this.Width, this.Height);

            // 背景
            if (this.BackgroundColor != Color.Transparent)
            {
                using (GraphicsPath bgPath = RoundedRect(rect, borderRadius))
                using (SolidBrush bgBrush = new SolidBrush(BackgroundColor))
                {
                    e.Graphics.FillPath(bgBrush, bgPath);
                }
            }

            // 前景（渐变色）
            float percent = (float)value / maximum;
            Rectangle progressRect = new Rectangle(0, 0, (int)(this.Width * percent), this.Height);

            if (progressRect.Width > 0)
            {
                using (GraphicsPath fgPath = RoundedRect(progressRect, borderRadius))
                using (LinearGradientBrush brush = new LinearGradientBrush(progressRect, StartColor, EndColor, LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillPath(brush, fgPath);
                }
            }

            // 绘制边框
            if (BorderWidth > 0)
            {
                using (GraphicsPath borderPath = RoundedRect(rect, borderRadius))
                using (Pen pen = new Pen(BorderColor, BorderWidth))
                {
                    e.Graphics.DrawPath(pen, borderPath);
                }
            }

            // 百分比文字
            if (IsSetText)
            {
                string text = $"{percent * 100:0}%";
                SizeF textSize = e.Graphics.MeasureString(text, this.Font);
                using (Brush brush1 = new SolidBrush(ForeColor))
                {
                    e.Graphics.DrawString(text, this.Font, brush1,
                        (this.Width - textSize.Width) / 2, (this.Height - textSize.Height) / 2);
                }
            }

            // 绘制 MarkerImage
            if (MarkerImage != null)
            {
                int markerX = (int)(this.Width * percent) + 1;
                int markerY = -MarkerSize.Height / 2; // 在进度条上方
                e.Graphics.DrawImage(MarkerImage, markerX, markerY, MarkerSize.Width, MarkerSize.Height);
            }


        }

        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
            path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
            path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
