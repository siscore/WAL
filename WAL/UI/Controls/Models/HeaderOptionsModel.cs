using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WAL.UI.Controls.Models
{
    public class HeaderOptionsModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Height { get; set; }
        public bool IfFixedWidth { get; set; }
        public int WidthPesantage { get; set; }
        public bool Sortable { get; set; }
        public Control Control { get; set; }
    }
}
