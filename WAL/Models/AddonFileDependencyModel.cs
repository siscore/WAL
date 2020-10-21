using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WAL.Static.Enums;

namespace WAL.Models
{
    public class AddonFileDependencyModel
    {
        public int Id { get; set; }

        public int AddonId { get; set; }

        public ProjectFileRelationType Type { get; set; }

        public int FileId { get; set; }
    }
}
