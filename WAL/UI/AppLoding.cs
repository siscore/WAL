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
using WAL.Models;
using WAL.Service;
using WAL.Static.Const;
using WAL.UI.Controls.Models;

namespace WAL.UI
{
    public partial class AppLoding : Form
    {
        private readonly Bitmap _animatedImage;
        private bool _currentlyAnimating = false;

        public AppLoding(Color backColor)
        {
            InitializeComponent();

            if (backColor != null)
                this.BackColor = backColor;

            _animatedImage = new Bitmap(IOHelper.WriteTempFile(Properties.Resources.loading_14));

            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);
        }

        private void AnimateImage()
        {
            if (!_currentlyAnimating)
            {
                ImageAnimator.Animate(_animatedImage, new EventHandler(this.OnFrameChanged));
                _currentlyAnimating = true;
            }
        }

        private void OnFrameChanged(object sender, EventArgs e)
        {
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            AnimateImage();
            ImageAnimator.UpdateFrames();
            e.Graphics.DrawImage(_animatedImage, new Point(0, -100));
        }

        public async Task<GameModel> LoadGameInfo()
        {
            var result = await new AddonsService().LoadData(TwitchConstants.WoWGameId);

            return result;
        }

        public async Task<List<RowItemsModel>> LoadAddonsInfo(string addonType, GameModel game)
        {
            var result = await new AddonsService().SearchSupportedInstalledAddons(addonType, game);

            return result;
        }
    }
}
