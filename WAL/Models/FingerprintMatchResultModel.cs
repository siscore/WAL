using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAL.Models
{
    public class FingerprintMatchResultModel
    {
        public bool IsCacheBuilt { get; set; }

        public List<FingerprintMatchModel> ExactMatches { get; set; }

        public List<long> ExactFingerprints { get; set; }

        public List<FingerprintMatchModel> PartialMatches { get; set; }

        public IDictionary<string, List<long>> PartialMatchFingerprints { get; set; }

        public List<long> InstalledFingerprints { get; set; }

        public List<long> UnmatchedFingerprints { get; set; }
    }
}
