using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAL.UI.Controls.Models
{
    public class RowItemsModel
    {
        public int Id { get; set; }
        public List<RowItemModel> RowItems { get; set; }
        public bool PriorityOrder { get; set; }
    }
}
