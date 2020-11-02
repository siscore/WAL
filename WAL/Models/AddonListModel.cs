using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAL.Static.Enums;

namespace WAL.Models
{
    public class AddonListModel
    {
        public int Id { get; set; }

        public Bitmap AddonAvatar { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public DateTime FileDate { get; set; }
        public int FileId { get; set; }
        public AddonFileModel File { get; set; }

        public string LatestFileVersion { get; set; }
        public int LatestFileVesionFileId { get; set; }
        public AddonFileModel LatestFile { get; set; }

        public string GameVersion { get; set; }
        public string Author { get; set; }

        public AddonStatusType StatusType { get; set; }
        public string StatusName { get; set; }
    }
}
