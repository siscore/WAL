using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WAL.Helpers;
using WAL.Properties;

namespace WAL.UI
{
    public partial class AppSettings : Form
    {
        public AppSettings()
        {
            InitializeComponent();

            if (IOHelper.CheckWoWPath(Settings.Default.WoWRetailPath))
            {
                textBox1.Text = Settings.Default.WoWRetailPath;
            }
        }

        private void RetailOFD_Click(object sender, EventArgs e)
        {
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                var newPath = FBD.SelectedPath;

                if (IOHelper.CheckWoWPath(newPath))
                {
                    Settings.Default.WoWRetailPath = FBD.SelectedPath;
                    Settings.Default.Save();

                    textBox1.Text = FBD.SelectedPath;
                }
            }
        }
    }
}
