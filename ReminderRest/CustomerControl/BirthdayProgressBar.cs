using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReminderRest.CustomerControl
{
    public class BirthdayProgressBar : ProgressBar
    {
        public Color BackgroundColor { get; set; } = Color.White;
        public Color BorderColor { get; set; } = Color.Transparent;



        public BirthdayProgressBar()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint
       | ControlStyles.OptimizedDoubleBuffer
       | ControlStyles.UserPaint
       | ControlStyles.ResizeRedraw, true);
            //this.SetStyle(ControlStyles.UserPaint, true); // 自绘
            this.DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            Rectangle rec = e.ClipRectangle;
            rec.Inflate(-1, -1);

            // 绘制背景（圆角矩形）
            using (GraphicsPath path = RoundedRect(rec, 6))
            {
                using (SolidBrush bgBrush = new SolidBrush(BackgroundColor))
                {
                    e.Graphics.FillPath(bgBrush, path);
                }
                using (Pen borderPen = new Pen(BorderColor, 1))
                {
                    e.Graphics.DrawPath(borderPen, path);
                }
            }

            // 当前百分比
            double percent = (double)Value / Maximum;
            int progressWidth = (int)(rec.Width * percent);

            if (progressWidth > 0)
            {
                Rectangle progressRect = new Rectangle(rec.X, rec.Y, progressWidth, rec.Height);

                // 渐变色：绿色 -> 橙色 -> 红色
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    progressRect, Color.Green, Color.Red, LinearGradientMode.Horizontal))
                {
                    ColorBlend blend = new ColorBlend(3);
                    blend.Colors = new Color[] { Color.Green, Color.Orange, Color.Red };
                    blend.Positions = new float[] { 0f, 0.5f, 1f };
                    brush.InterpolationColors = blend;

                    using (GraphicsPath progressPath = RoundedRect(progressRect, 6))
                    {
                        e.Graphics.FillPath(brush, progressPath);
                    }
                }
            }

            // 百分比文字
            string text = $"{(int)(percent * 100)}%";
            using (var sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                e.Graphics.DrawString(text, this.Font, Brushes.Black, rec, sf);
            }
        }

        // 工具方法：画圆角矩形
        private GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int d = radius * 2;
            GraphicsPath path = new GraphicsPath();
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}
