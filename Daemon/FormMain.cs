using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Daemon
{
    public partial class FormMain : Form
    {
        public FormMain()
        {
            InitializeComponent();

            NormalToMinimized();

            Daemon.Start();
        }

        protected sealed override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(value);
        }

        private void NormalToMinimized()
        {
            this.WindowState = FormWindowState.Minimized;
            this.ShowInTaskbar = false;
            this.Visible = false;
            this.notifyIcon.Visible = true;
        }

        private void close_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}
