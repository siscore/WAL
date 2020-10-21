using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAL.Models
{
    public class SortableGameVersionModel
    {
        public string GameVersionPadded { get; set; }

        public string GameVersion { get; set; }

        public DateTime GameVersionReleaseDate { get; set; }

        public string GameVersionName { get; set; }
    }
}
