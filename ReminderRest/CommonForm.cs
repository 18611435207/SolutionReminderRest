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
    public partial class CommonForm : Form
    {
        public CommonForm()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.cow1;
            this.ShowInTaskbar = false;
        }
    }
}
