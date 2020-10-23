using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WAL.Static.Enums;
using WAL.Helpers;
using WAL.Models;
using WAL.Service;
using System.Text.RegularExpressions;
using WAL.UI.Controls.Models;
using WAL.UI.Controls.Static.Enums;
using WAL.Static.Const;

namespace WAL.UI
{
    public partial class AddonsList : Form
    {
        private readonly string PageType;

        public AddonsList(string pageType)
        {
            InitializeComponent();

            PageType = pageType;

        }

        public async void SearchAddons(Color backColor)
        {
            var preloader = new AppLoding(backColor);
            preloader.Show(this);

            var game = await preloader.LoadGameInfo();
            var addons = await preloader.LoadAddonsInfo(PageType, game);

            if (addons != null)
            {
                gridContainer1.Clear();
                gridContainer1.AddRange(addons);
            }

            preloader.Close();
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            SearchAddons(Color.FromArgb(31, 31, 35));
        }
    }
}
