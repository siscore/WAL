using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAL.Models
{
    public class CategoryAvatar
    {
        public long Id { get; set; }
        public string Url { get; set; }
        public Bitmap Bitmap { get; set; }
    }
}
