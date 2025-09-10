using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReminderRest
{
    public partial class RemindLater : CommonForm
    {
        public RemindLater()
        {
            InitializeComponent();
            this.TopMost = true;
            this.cmbHour.SelectedIndex = 0;
            this.btnConfirm.Click += (s, e) =>
            {
                OnRemindLater?.Invoke(this, LaterMinate);
                this.Close();
            };
            this.btnCancel.Click += (s, e) => { this.Close(); };
        }

        public int LaterMinate { get { return int.Parse(cmbHour.Text); } }

        public Action<object, int> OnRemindLater { get; internal set; }

    }
}
