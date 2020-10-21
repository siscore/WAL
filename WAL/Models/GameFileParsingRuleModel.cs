using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WAL.Models
{
    public class GameFileParsingRuleModel
    {
        private Regex _commentStripRegex;
        private Regex _inclusionRegex;

        public string CommentStripPattern { get; set; }

        public string FileExtension { get; set; }

        public string InclusionPattern { get; set; }

        public int GameId { get; set; }

        public int Id { get; set; }

        public Regex CommentStripRegex
        {
            get
            {
                if (this.CommentStripPattern != null && this._commentStripRegex == null)
                    this._commentStripRegex = new Regex(this.CommentStripPattern);
                return this._commentStripRegex;
            }
        }

        public Regex InclusionRegex
        {
            get
            {
                if (this._inclusionRegex == null)
                    this._inclusionRegex = new Regex(this.InclusionPattern);
                return this._inclusionRegex;
            }
        }
    }
}
