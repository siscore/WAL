using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAL.Static.Enums
{
    public enum AddonStatusType
    {
        [Description("Up to date")]
        UpToDate = 0,

        [Description("Update")]
        Update = 1
    }
}
