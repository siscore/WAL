using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAL.Models
{
    public class AddonListModel
    {
        public int Id { get; set; }
        public string Fingerprint { get; set; }

        public Bitmap AddonAvatar { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public DateTime FileDate { get; set; }
        public int FileId { get; set; }

        public string LastVersion { get; set; }
        public int LastVesionFileId { get; set; }

        public string GameVersion { get; set; }
        public string Author { get; set; }
    }
}
