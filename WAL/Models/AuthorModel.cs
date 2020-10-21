using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAL.Models
{
    public class AuthorModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int ProjectId { get; set; }

        public int? ProjectTitleId { get; set; }

        public string ProjectTitleTitle { get; set; }

        public Int64? TwitchId { get; set; }

        public string Url { get; set; }

        public Int64? UserId { get; set; }
    }
}
