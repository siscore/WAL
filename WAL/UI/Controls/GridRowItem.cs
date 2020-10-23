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
using System.Diagnostics;
using WAL.Static.Enums;

namespace WAL.UI.Controls
{
    public partial class GridRowItem : UserControl
    {
        public event NotifySelectRowEventHandler NotifyRowSelectEvent;
        public event NotifyUpdateAddonEventHandler NotifyUpdateAddonEvent;
        public bool Selected { get; set; }

        private readonly IEnumerable<HeaderOptionsModel> Headers;
        private readonly int Id;

        public GridRowItem()
        {
            InitializeComponent();
        }

        public GridRowItem(int Index, IEnumerable<HeaderOptionsModel> headers)
        {
            InitializeComponent();

            this.MouseClick += new MouseEventHandler(MainClick);

            Id = Index;
            Headers = headers;

            var index = 0;

            var r = new Random();
            var _r = r.Next(0, 255);
            var _b = r.Next(0, 255);
            var _g = r.Next(0, 255);

            var prevPanel = (Control)null;
            foreach (var column in Headers)
            {
                var name = $"col{index}";
                this.Controls.Add(new Panel
                {
                    Name = name,
                    Size = new Size(150, 50),
                    BackColor = Color.FromArgb(24, 24, 27),
                    Top = 0,
                    Height = Headers.First().Height,
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
                name = $"col{index}";
                panel = this.Controls.Find(name, false).Where(x => x.Name.Equals(name)).FirstOrDefault();

                if (panel.Controls.Count != 0)
                {
                    foreach(Control control in panel.Controls)
                    {
                        control.Dispose();
                    }
                }

                if (panel == null)
                    continue;

                switch (item.PanelType)
                {
                    case PanelTypes.Text:
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

                        break;
                    case PanelTypes.Image:
                        panel.Controls.Add(new PictureBox
                        {
                            Name = name,
                            BorderStyle = BorderStyle.None,
                            Dock = DockStyle.Fill,
                            Padding = new Padding(7),
                            WaitOnLoad = false,
                            SizeMode = PictureBoxSizeMode.StretchImage,
                            Image = item.Bitmap,
                            Visible = true
                        });

                        break;
                    case PanelTypes.Status:
                        if (item.AddonStatusType == AddonStatusType.UpToDate)
                        {
                            panel.Controls.Add(new Label
                            {
                                Name = name,
                                TextAlign = item.ContentAlignment,
                                AutoSize = false,
                                Dock = DockStyle.Fill,
                                Text = item.Name,
                                Font = this.Font,
                                Visible = true,
                                Padding = new Padding(15, 3, 15, 3)
                            });
                        }
                        else
                        {
                            panel.Controls.Add(new Button
                            {
                                Name = name,
                                Text = item.Name,
                                FlatStyle = FlatStyle.Flat,
                                Font = this.Font,
                                Size = new Size(75, 30),
                                ForeColor = Color.FromArgb(255, 255, 255)
                            });

                            var button = (Button)panel.Controls[0];
                            button.FlatAppearance.BorderColor = Color.FromArgb(64, 176, 250);
                            button.FlatAppearance.BorderSize = 2;
                            button.FlatAppearance.MouseDownBackColor = Color.FromArgb(33, 99, 154);
                            button.FlatAppearance.MouseOverBackColor = Color.FromArgb(44, 124, 193);

                            button.Left = (panel.Width - button.Width) / 2;
                            button.Top = (panel.Height - button.Height) / 2;

                            //button.MouseClick += new MouseEventHandler(UpdateButtonClick);
                            button.Click += new EventHandler(this.UpdateButtonClick);
                            panel.MouseClick += new MouseEventHandler(RowMouseClick);
                        }

                        break;
                    default:
                        break;
                }

                panel.Controls[0].MouseClick += new MouseEventHandler(RowMouseClick);

                index++;
            }
        }
        private void UpdateButtonClick(object sender, EventArgs e)
        {
            NotifyUpdateAddonEvent?.Invoke(this.Id);
        }

        private void RowMouseClick(object sender, MouseEventArgs e)
        {
            this.MainClick(sender, e);
        }

        private void MainClick(object sender, MouseEventArgs e)
        {
            this.Selected = !this.Selected;
            NotifyRowSelectEvent?.Invoke(this.Id);
        }

        public void OnSelected(int RowId)
        {
            if (Id != RowId)
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

            foreach (var col in Headers)
            {
                var name = $"col{index}";
                var panel = this.Controls.Find(name, false).Where(x => x.Name.Equals(name)).FirstOrDefault();

                if (panel == null)
                    return;

                if (!InvokeRequired)
                {
                    panel.Left = prevPanel == null ? 0 : prevPanel.Left + prevPanel.Width;
                    panel.Width = col.IfFixedWidth
                        ? col.WidthPesantage
                        : (((Control)sender).Width * col.WidthPesantage) / 100;
                    panel.Height = col.Height;
                    Debug.WriteLine($"Row Top: {panel.Top} Height: {panel.Height}");

                }
                else
                {
                    Invoke(new Action(() =>
                    {
                        panel.Left = prevPanel == null ? 0 : prevPanel.Left + prevPanel.Width;
                        panel.Width = col.IfFixedWidth
                            ? col.WidthPesantage
                            : (((Control)sender).Width * col.WidthPesantage) / 100;
                        panel.Height = col.Height;
                        Debug.WriteLine($"Row Top: {panel.Top} Height: {panel.Height}");
                    }));
                }

                prevPanel = panel;
                index++;
            }
        }
    }
}
