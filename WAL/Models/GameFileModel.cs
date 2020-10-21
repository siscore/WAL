using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAL.Static.Enums;

namespace WAL.Models
{
    public class GameFileModel
    {
        public int Id { get; set; }

        public int GameId { get; set; }

        public bool IsRequired { get; set; }

        public string FileName { get; set; }

        public GameFileType FileType { get; set; }

        public GamePlatformType PlatformType { get; set; }
    }
}
