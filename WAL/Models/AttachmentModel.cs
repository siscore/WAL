using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAL.Models
{
    public class AttachmentModel
    {
        public string Description { get; set; }

        public Int64 Id { get; set; }

        public bool IsDefault { get; set; }

        public Int64 ProjectId { get; set; }

        public int Status { get; set; }

        public string ThumbnailUrl { get; set; }

        public string Title { get; set; }

        public string Url { get; set; }
    }
}
