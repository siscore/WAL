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

            this.Text += $" v{Application.ProductVersion}";
        }
        
        private void Main_Load(object sender, EventArgs e)
        {
            if (!IOHelper.CheckWoWPath(Settings.Default.WoWRetailPath))
            {
                OpenChildForm(new UI.AppSettings());
            }
            else
            {
                var form = OpenChildForm(new UI.AddonsList(TwitchConstants.WoWRetail)) as AddonsList;
                form.SearchAddons(this.BackColor);
            }
        }

        private Form OpenChildForm(Form childForm)
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

            return childForm;
        }

        private void SettingButton_Click(object sender, EventArgs e)
        {
            OpenChildForm(new UI.AppSettings());
        }

        private void ShowWoWRetailButton_Click(object sender, EventArgs e)
        {
            var form = OpenChildForm(new UI.AddonsList(TwitchConstants.WoWRetail)) as AddonsList;
            form.SearchAddons(form.BackColor);
        }

        private void ShowWoWClassicButton_Click(object sender, EventArgs e)
        {
            var form = OpenChildForm(new UI.AddonsList(TwitchConstants.WoWClassic)) as AddonsList;
            form.SearchAddons(form.BackColor);
        }
    }
}
