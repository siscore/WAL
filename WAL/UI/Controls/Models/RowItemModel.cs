using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAL.Static.Enums;
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

        /// <summary>
        /// Cell text
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Cell Bitmap only for PanelTypes.Image
        /// </summary>
        public Bitmap Bitmap { get; set; }
        /// <summary>
        /// Status only for PanelTypes.Status
        /// </summary>
        public AddonStatusType AddonStatusType { get; set; }
        /// <summary>
        /// Type of panel
        /// </summary>
        public PanelTypes PanelType { get; set; }
        /// <summary>
        /// Align
        /// </summary>
        public ContentAlignment ContentAlignment { get; set; }
    }
}
