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

namespace WAL.UI.Controls
{
    public partial class GridContainer : UserControl
    {
        public delegate void NotifyParentEventHandler(int RowId);

        public class HeaderOptionsModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public int Height { get; set; }
            public bool IfFixedWidth { get; set; }
            public int WidthPesantage { get; set; }
            public Control Control { get; set; }
        }

        private readonly List<HeaderOptionsModel> HeaderOptions;

        private List<GridRowItem> RowItems { get; set; }

        public GridContainer()
        {
            InitializeComponent();

            RowItems = new List<GridRowItem>();
            HeaderOptions = new List<HeaderOptionsModel>
            {
                new HeaderOptionsModel{ Id = 0, Name = "", Height = 50, WidthPesantage = 50, IfFixedWidth = true }
                //new HeaderOptionsModel{ Id = 1, Name = "Addon", Height = 50, WidthPesantage = 27 },
                //new HeaderOptionsModel{ Id = 2, Name = "Status", Height = 50, WidthPesantage = 15 },
                //new HeaderOptionsModel{ Id = 3, Name = "Last Version", Height = 50, WidthPesantage = 25 },
                //new HeaderOptionsModel{ Id = 4, Name = "Game Version", Height = 50, WidthPesantage = 15 },
                //new HeaderOptionsModel{ Id = 5, Name = "Author", Height = 50, WidthPesantage = 16 }
            };

            var prevHeader = (Control)null;

            foreach (var header in HeaderOptions)
            {
                var parent = this.HeaderPanel;
                var name = $"panel{header.Id}";

                //parent.Height = _headerOptions.First().Height;

                parent.Controls.Add(new Panel
                {
                    Name = name,
                    Visible = true,
                    BackColor = Color.FromArgb(31, 31, 35),
                    Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
                });

                var panel = this.HeaderPanel.Controls.Find(name, false).Where(x => x.Name.Equals(name)).First();
                panel.BringToFront();

                var labelName = $"Caption{header.Id}";

                panel.Controls.Add(new Label
                {
                    Name = labelName,
                    TextAlign = ContentAlignment.MiddleCenter,
                    AutoSize = false,
                    Dock = DockStyle.Fill,
                    Text = header.Name,
                    Font = this.HeaderPanel.Font,
                    Visible = true,          
                });

                header.Control = panel;
                prevHeader = panel;

            }
        }

        private void HeaderPanel_Resize(object sender, EventArgs e)
        {
            var prevHeader = (Control)null;

            foreach (var header in HeaderOptions)
            {
                if (header.Control == null)
                    break;
                header.Control.Top = 0;
                header.Control.Left = prevHeader == null ? 0 : prevHeader.Left + prevHeader.Width;
                header.Control.Height = header.Height;
                header.Control.Width = header.IfFixedWidth 
                    ? header.WidthPesantage
                    : (((Control)sender).Width * header.WidthPesantage) / 100;

                prevHeader = header.Control;
            }
        }

        private void RowPanelResize(object sender, EventArgs e)
        {
            foreach (var item in RowItems)
            {
                item.ResizeRow(HeaderPanel, e);
            }
        }

        private void GridRow_MouseClick(int RowId)
        {
            foreach(Control item in this.RowPanel.Controls)
            {
                ((GridRowItem)item).OnSelected(RowId);
            }
        }

        public GridRowItem Add(RowItemsModel item)
        {
            var name = $"row{item.Id}";
            this.RowPanel.Controls.Add(new GridRowItem(item.Id, HeaderOptions) { Name = name });
            var rowItem = this.RowPanel.Controls.Find(name, false).Where(x => x.Name.Equals(name)).First();

            //rowItem.Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right;
            rowItem.Dock = DockStyle.Top;
            //rowItem.Top = (RowItems.Count) * HeaderOptions.First().Height;
            //rowItem.Left = 0;
            //rowItem.Size = new Size(this.RowPanel.Width, HeaderOptions.First().Height);
            rowItem.Font = RowPanel.Font;

            //((GridRowItem)rowItem).ResizeRow(this.HeaderPanel, null);

            RowItems.Add(((GridRowItem)rowItem));
            ((GridRowItem)rowItem).ShowInfo(item.RowItems);

            ((GridRowItem)rowItem).NotifyParentEvent += new NotifyParentEventHandler(GridRow_MouseClick);

            return RowItems.Last();
        }

        public IEnumerable<GridRowItem> AddRange(IEnumerable<RowItemsModel> items)
        {
            var result = new List<GridRowItem>();
            foreach(var item in items)
            {
                var newItem = Add(item);
                result.Add(newItem);
            }

            return result;
        }

        public void Clear()
        {
            RowPanel.Controls.Clear();
            RowItems.Clear();
        }
    }
}
