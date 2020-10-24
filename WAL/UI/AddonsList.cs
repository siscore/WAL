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
using System.Threading;

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
            var _preloader = new Thread(() =>
            {
                Application.Run(new AppLoding(backColor));
            });
            _preloader.SetApartmentState(ApartmentState.STA);
            _preloader.Start();

            var game = await LoadGameInfo();
            var addons = await LoadAddonsInfo(PageType, game);

            if (addons != null)
            {
                gridContainer1.Clear();
                gridContainer1.AddRange(addons);
            }

            _preloader.Abort();
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            SearchAddons(Color.FromArgb(31, 31, 35));
        }

        private async Task<GameModel> LoadGameInfo()
        {
            var result = await new AddonsService().LoadData(TwitchConstants.WoWGameId);

            return result;
        }

        private async Task<List<RowItemsModel>> LoadAddonsInfo(string addonType, GameModel game)
        {
            var result = await new AddonsService().SearchSupportedInstalledAddons(addonType, game);

            return result;
        }
    }
}