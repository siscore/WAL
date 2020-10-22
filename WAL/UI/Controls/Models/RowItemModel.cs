using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAL.UI.Controls.Static.Enums;

namespace WAL.UI.Controls.Models
{
    public class RowItemModel
    {
        public RowItemModel()
        {
            ContentAlignment = ContentAlignment.MiddleLeft;
            PanelType = PanelTypes.Text;
        }

        public string Name { get; set; }
        public Bitmap Bitmap { get; set; }
        public PanelTypes PanelType { get; set; }
        public ContentAlignment ContentAlignment { get; set; }
    }
}
