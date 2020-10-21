using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WAL.Properties;
using WAL.Helpers;
using WAL.Static.Const;

namespace WAL.UI
{
    public partial class Main : Form
    {
        private Form _currentChildForm;
        
        public Main()
        {
            InitializeComponent();

            new UI.AppLoding().ShowDialog(this);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (!IOHelper.CheckWoWPath(Settings.Default.WoWRetailPath))
            {
                OpenChildForm(new UI.AppSettings());
            }
            else
            {
                OpenChildForm(new UI.AddonsList(TwitchConstants.WoWRetail));
            }
        }

        private void OpenChildForm(Form childForm)
        {
            if(_currentChildForm != null)
            {
                _currentChildForm.Close();
            }

            _currentChildForm = childForm;

            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            panelDesktop.Controls.Add(childForm);
            panelDesktop.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenChildForm(new UI.AppSettings());
        }

        private void btnWoWRetail_Click(object sender, EventArgs e)
        {
            OpenChildForm(new UI.AddonsList(TwitchConstants.WoWRetail));
        }
    }
}
