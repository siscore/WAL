using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WAL.Models
{
    public class CategoryModel
    {
        public Int64 AvatarId { get; set; }

        public string AvatarUrl { get; set; }

        public Int64 CategoryId { get; set; }

        public Int64 GameId { get; set; }

        public string Name { get; set; }

        public Int64 ParentId { get; set; }

        public Int64 ProjectId { get; set; }

        public Int64 RootId { get; set; }

        public string Url { get; set; }
    }
}
