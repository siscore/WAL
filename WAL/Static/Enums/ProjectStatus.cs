using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAL.Static.Enums
{
    public enum ProjectStatus
    {
        New = 1,
        ChangesRequired = 2,
        UnderSoftReview = 3,
        Approved = 4,
        Rejected = 5,
        ChangesMade = 6,
        Inactive = 7,
        Abandoned = 8,
        Deleted = 9,
        UnderReview = 10, // 0x0000000A
    }
}
