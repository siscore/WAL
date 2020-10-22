using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAL.Static.Enums;

namespace WAL.Models
{
    public class GameVersionFileModel
    {
        public ProjectFileReleaseType FileType { get; set; }

        public string GameVersion { get; set; }

        public string GameVersionFlavor { get; set; }

        public Int64 ProjectFileId { get; set; }

        public string ProjectFileName { get; set; }
    }
}
