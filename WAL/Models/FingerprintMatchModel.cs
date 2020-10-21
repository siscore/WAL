using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAL.Models
{
    public class FingerprintMatchModel
    {
        public int Id { get; set; }

        public AddonFileModel File { get; set; }

        public List<AddonFileModel> LatestFiles { get; set; }
    }
}
