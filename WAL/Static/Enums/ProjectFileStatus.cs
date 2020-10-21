using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAL.Static.Enums
{
    public enum ProjectFileStatus
    {
        Processing = 1,
        ChangesRequired = 2,
        UnderReview = 3,
        Approved = 4,
        Rejected = 5,
        MalwareDetected = 6,
        Deleted = 7,
        Archived = 8,
        Testing = 9,
        Released = 10, // 0x0000000A
        ReadyForReview = 11, // 0x0000000B
        Deprecated = 12, // 0x0000000C
        Baking = 13, // 0x0000000D
        AwaitingPublishing = 14, // 0x0000000E
        FailedPublishing = 15, // 0x0000000F
    }
}
