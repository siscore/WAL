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
using System.Diagnostics;
using WAL.UI.Controls.Helpers;

namespace WAL.UI.Controls
{
    public partial class GridContainer : UserControl
    {
        public delegate void NotifySelectRowEventHandler(int RowId);
        public delegate void NotifyUpdateAddonEventHandler(int RowId);

        private readonly List<HeaderOptionsModel> HeaderOptions;

        private List<GridRowItemModel> RowItems { get; set; }

        public GridContainer()
        {
            InitializeComponent();

            RowItems = new List<GridRowItemModel>();
            HeaderOptions = new List<HeaderOptionsModel>
            {
                new HeaderOptionsModel{ Id = 0, Name = "", Height = 50, WidthPesantage = 50, IfFixedWidth = true },
                new HeaderOptionsModel{ Id = 1, Name = "Addon", Height = 50, WidthPesantage = 27 },
                new HeaderOptionsModel{ Id = 2, Name = "Status", Height = 50, WidthPesantage = 15 },
                new HeaderOptionsModel{ Id = 3, Name = "Last Version", Height = 50, WidthPesantage = 25 },
                new HeaderOptionsModel{ Id = 4, Name = "Game Version", Height = 50, WidthPesantage = 15 },
                new HeaderOptionsModel{ Id = 5, Name = "Author", Height = 50, WidthPesantage = 16 }
            };

            var prevHeader = (Control)null;

            foreach (var header in HeaderOptions)
            {
                var parent = this.HeaderPanel;
                var name = $"panel{header.Id}";

                parent.Controls.Add(new Panel
                {
                    Name = name,
                    Visible = true,
                    BackColor = Color.FromArgb(31, 31, 35),
                    Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
                });

                var panel = this.HeaderPanel.Controls.Find(name, false).Where(x => x.Name.Equals(name)).First();

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

                panel.Controls[0].MouseClick += new MouseEventHandler(Header_MouseClisk);

                header.Control = panel;
                prevHeader = panel;

            }
        }

        private void Header_MouseClisk(object sender, MouseEventArgs e)
        {
            var header = HeaderOptions.First(x => ((Label)sender).Parent.Name.EndsWith(x.Id.ToString()));

            if (header.Sortable)
                return;

            HeaderOptions.ForEach(x => 
            {
                x.Sortable = false;
                x.Control.Controls[0].ForeColor = Color.White;
            });

            
            header.Sortable = true;
            header.Control.Controls[0].ForeColor = Color.FromArgb(64, 176,250);

            Sort();
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
            foreach (var item in RowItems.Where(x => x.GridRowItem != null))
            {
                item.GridRowItem.ResizeRow(HeaderPanel, e);
            }
        }

        private void GridRowUpdateAddon_MouseClick(int RowId)
        {
            Debug.WriteLine($"Update Row: {RowId}");
        }

        private void GridRow_MouseClick(int RowId)
        {
            foreach (GridRowItem item in this.RowPanel.Controls)
            {
                item.OnSelected(RowId);
            }
        }

        private void Sort()
        {
            using (var s = new SuspendDrawingUpdate(this))
            {
                var sortRow = HeaderOptions.FirstOrDefault(x => x.Sortable);

                RowItems = sortRow != null
                    ? RowItems
                        .OrderBy(x => !x.RowItemsModel.PriorityOrder)
                        .ThenBy(x => x.RowItemsModel.RowItems[sortRow.Id].Name).ToList()
                    : RowItems
                        .OrderBy(x => !x.RowItemsModel.PriorityOrder).ToList();

                Clear(true);

                var rowIndex = 0;
                RowItems.ForEach(item =>
                {
                    var name = $"row{item.RowItemsModel.Id}";

                    item.GridRowItem = new GridRowItem(item.RowItemsModel.Id, HeaderOptions)
                    {
                        Name = name,
                        Anchor = AnchorStyles.Left | AnchorStyles.Top | AnchorStyles.Right,
                        Padding = new Padding(0),
                        Font = RowPanel.Font,
                        Top = (rowIndex) * HeaderOptions.First().Height,
                        Left = 0,
                        Size = new Size(this.RowPanel.Width, HeaderOptions.First().Height)
                    };

                    RowPanel.Controls.Add(item.GridRowItem);
                    item.GridRowItem.ResizeRow(HeaderPanel, null);
                    item.GridRowItem.ShowInfo(item.RowItemsModel.RowItems);
                    item.GridRowItem.NotifyRowSelectEvent += new NotifySelectRowEventHandler(GridRow_MouseClick);
                    item.GridRowItem.NotifyUpdateAddonEvent += new NotifyUpdateAddonEventHandler(GridRowUpdateAddon_MouseClick);

                    rowIndex++;
                });
            }

            this.Refresh();
        }

        private void Clear(bool OnlyVisual)
        {
            foreach(Control item in RowPanel.Controls)
            {
                item.Dispose();
            }
            RowPanel.Controls.Clear();
            if (!OnlyVisual)
                RowItems.Clear();
        }

        private GridRowItemModel AddMethod(RowItemsModel item)
        {
            RowItems.Add(new GridRowItemModel
            {
                GridRowItem = null,
                RowItemsModel = item
            });

            return RowItems.Last();
        }

        public GridRowItemModel Add(RowItemsModel item)
        {
            var result = AddMethod(item);
            Sort();

            return result;
        }

        public IEnumerable<GridRowItemModel> AddRange(IEnumerable<RowItemsModel> items)
        {
            var result = new List<GridRowItemModel>();

            foreach (var item in items)
            {
                var newItem = AddMethod(item);
                result.Add(newItem);
            }

            Sort();

            return result;
        }

        public void Clear()
        {
            Clear(false);
        }
    }
}
