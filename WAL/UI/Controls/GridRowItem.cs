using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WAL.UI.Controls.Models;
using WAL.UI.Controls.Static.Enums;
using static WAL.UI.Controls.GridContainer;

namespace WAL.UI.Controls
{
    public partial class GridRowItem : UserControl
    {
        public event NotifyParentEventHandler NotifyParentEvent;

        private IEnumerable<HeaderOptionsModel> _headers;
        private int _Id;
        public bool Selected { get; set; }

        public GridRowItem(int Index, IEnumerable<HeaderOptionsModel> headers)
        {
            InitializeComponent();

            this.MouseClick += new MouseEventHandler(MainClick);

            _Id = Index;
            _headers = headers;

            var index = 0;

            var r = new Random();
            var _r = r.Next(0, 255);
            var _b = r.Next(0, 255);
            var _g = r.Next(0, 255);

            var prevPanel = (Control)null;
            foreach (var column in _headers)
            {
                var name = $"col{index}";
                this.Controls.Add(new Panel
                {
                    Name = name,
                    Size = new Size(150, 50),
                    BackColor = Color.FromArgb(24, 24, 27),
                    Top = 0,
                    Visible = true
                });
                var panel = this.Controls.Find(name, false).Where(x => x.Name.Equals(name)).First();

                prevPanel = panel;
                index++;
            }
        }

        public void ShowInfo(IEnumerable<RowItemModel> items)
        {
            var index = 0;
            var name = string.Empty;
            var panel = (Control)null;

            foreach (var item in items)
            {       
                switch (item.PanelType)
                {
                    case PanelTypes.Text:
                        name = $"col{index}";
                        panel = this.Controls.Find(name, false).Where(x => x.Name.Equals(name)).First();

                        panel.Controls.Add(new Label
                        {
                            Name = name,
                            TextAlign = item.ContentAlignment,
                            AutoSize = false,
                            Dock = DockStyle.Fill,
                            Text = item.Name,
                            Font = this.Font,
                            Visible = true,
                            Padding = new Padding(15,3,15,3)
                        });

                        panel.Controls[0].MouseClick += new MouseEventHandler(RowMouseClick);
                        break;
                    case PanelTypes.Image:
                        name = $"col{index}";
                        panel = this.Controls.Find(name, false).Where(x => x.Name.Equals(name)).First();

                        panel.Controls.Add(new PictureBox
                        {
                            Name = name,
                            BorderStyle = BorderStyle.None,
                            WaitOnLoad = false,
                            SizeMode = PictureBoxSizeMode.StretchImage,
                            Image = item.Bitmap,
                            Size = new Size(panel.Height, panel.Height),
                            Visible = true,
                            Padding = new Padding(7)
                        });
                        break;
                    default:
                        break;
                }
                index++;
            }
        }

        private void RowMouseClick(object sender, MouseEventArgs e)
        {
            this.MainClick(sender, e);
        }

        private void MainClick(object sender, MouseEventArgs e)
        {
            this.Selected = !this.Selected;
            NotifyParentEvent?.Invoke(this._Id);
        }

        public void OnSelected(int RowId)
        {
            if (_Id != RowId)
                Selected = false;
            foreach(Control item in Controls)
            {
                item.BackColor = Selected ? Color.FromArgb(48, 48, 56) : Color.FromArgb(24, 24, 27);
            }
        }

        public void ResizeRow(object sender, EventArgs e)
        {
            var prevPanel = (Control)null;
            var index = 0;

            foreach (var col in _headers)
            {
                var name = $"col{index}";
                var panel = this.Controls.Find(name, false).Where(x => x.Name.Equals(name)).First();

                if (!InvokeRequired)
                {
                    panel.Left = prevPanel == null ? 0 : prevPanel.Left + prevPanel.Width;
                    panel.Width = col.IfFixedWidth
                        ? col.WidthPesantage
                        : (((Control)sender).Width * col.WidthPesantage) / 100;
                }
                else
                {
                    Invoke(new Action(() =>
                    {
                        panel.Left = prevPanel == null ? 0 : prevPanel.Left + prevPanel.Width;
                        panel.Width = col.IfFixedWidth
                        ? col.WidthPesantage
                        : (((Control)sender).Width * col.WidthPesantage) / 100;
                    }));
                }

                prevPanel = panel;
                index++;
            }
        }
    }
}
