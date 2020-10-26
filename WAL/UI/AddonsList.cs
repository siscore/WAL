using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WAL.Static.Enums;
using WAL.Models;
using WAL.Service;
using WAL.UI.Controls.Models;
using WAL.UI.Controls.Static.Enums;
using WAL.Static.Const;
using System.Threading;
using System.IO;
using System.IO.Compression;
using WAL.Helpers;

namespace WAL.UI
{
    public partial class AddonsList : Form
    {
        private readonly string PageType;
        private List<AddonListModel> _addons;

        public delegate void UpdateAddonEventHandler(int AddonId);

        public AddonsList(string pageType)
        {
            InitializeComponent();

            PageType = pageType;

            gridContainer1.OnUpdateAddonEvent = UpdateAddon;
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
                _addons = addons;

                gridContainer1.Clear();

                var rowItems = addons.Select((item, index) => new RowItemsModel
                {
                    Id = index,
                    PriorityOrder = item.StatusType == AddonStatusType.Update,
                    RowItems = new List<RowItemModel>
                    {
                        new RowItemModel { Bitmap = item.AddonAvatar, PanelType = PanelTypes.Image },
                        new RowItemModel { Name = item.DisplayName },
                        new RowItemModel { Name = item.StatusName, PanelType = PanelTypes.Status, AddonStatusType = item.StatusType, ContentAlignment = ContentAlignment.MiddleCenter },
                        new RowItemModel { Name = item.LatestFileVersion, ContentAlignment = ContentAlignment.MiddleCenter },                       
                        new RowItemModel { Name = item.GameVersion, ContentAlignment = ContentAlignment.MiddleCenter },
                        new RowItemModel { Name = item.Author, ContentAlignment = ContentAlignment.MiddleCenter }
                    }
                }).ToList();

                gridContainer1.AddRange(rowItems);
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

        private async Task<List<AddonListModel>> LoadAddonsInfo(string addonType, GameModel game)
        {
            var result = await new AddonsService().SearchSupportedInstalledAddons(addonType, game);

            return result;
        }

        private async void UpdateAddon(int AddonIndex)
        {
            await UpdateAddonAsync(AddonIndex);
        }

        private async Task UpdateAddonAsync(int AddonIndex)
        {
            var addon = _addons[AddonIndex];

            if (addon != null)
            {
                var row = gridContainer1.GetItem(AddonIndex);
                row.RowItems[2].AddonStatusType = AddonStatusType.UpToDate;
                row.RowItems[2].Name = "Updating...";

                gridContainer1.UpdateRow(AddonIndex, row);

                var newAddon = await new AddonsService().UpdateAddon(addon);
                if (newAddon != null)
                {
                    _addons[AddonIndex] = newAddon;

                    row.RowItems[1].Name = newAddon.DisplayName;
                    row.RowItems[2].AddonStatusType = newAddon.StatusType;
                    row.RowItems[2].Name = EnumHelper.GetEnumDescription(newAddon.StatusType);
                }
                else
                {
                    row.RowItems[2].AddonStatusType = AddonStatusType.Update;
                    row.RowItems[2].Name = EnumHelper.GetEnumDescription(AddonStatusType.Update);
                }

                gridContainer1.UpdateRow(AddonIndex, row);
            }
        }
    }
}