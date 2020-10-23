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
using WAL.Service;
using WAL.Static.Const;

namespace WAL.UI
{
    public partial class AppLoding : Form
    {
        private readonly Bitmap _animatedImage;
        //private readonly Bitmap _animatedImage = new Bitmap(@"D:\Sources\Bitbucket\WAL\WAL\icons\loading-14.gif");
        private bool _currentlyAnimating = false;

        public AppLoding()
        {
            InitializeComponent();

            _animatedImage = new Bitmap(IOHelper.WriteTempFile(Properties.Resources.loading_14));

            this.SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.DoubleBuffer, true);

            LoadData();
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

        public async void LoadData()
        {
            Program._game = await new AddonsService().LoadData(TwitchConstants.WoWGameId);

            this.Close();
        }
    }
}
