using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAL.Static.Enums;

namespace WAL.Models
{
    public class AddonFileModuleModel
    {
        public string Foldername { get; set; }

        public long Fingerprint { get; set; }

        public ProjectFileFingerprintType Type { get; set; }
    }
}
