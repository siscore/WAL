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
        public IEnumerable<RowItemModel> RowItems { get; set; }
    }
}
