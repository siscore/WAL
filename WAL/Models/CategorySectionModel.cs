using System.Text.RegularExpressions;
using WAL.Static.Enums;

namespace WAL.Models
{
    public class CategorySectionModel
    {
        private Regex _mInitialInclusionRegex;
        private Regex _mExtraIncludeRegex;

        public int Id { get; set; }

        public int GameId { get; set; }

        public string Name { get; set; }

        public GameSectionPackageMapPackageType PackageType { get; set; }

        public string Path { get; set; }

        public string InitialInclusionPattern { get; set; }

        public string ExtraIncludePattern { get; set; }

        public int GameCategoryId { get; set; }

        public Regex InitialInclusionRegex
        {
            get
            {
                if (this._mInitialInclusionRegex == null)
                    this._mInitialInclusionRegex = new Regex(this.InitialInclusionPattern);
                return this._mInitialInclusionRegex;
            }
        }

        public Regex ExtraIncludeRegex
        {
            get
            {
                if (this.ExtraIncludePattern != null && this._mExtraIncludeRegex == null)
                    this._mExtraIncludeRegex = new Regex(this.ExtraIncludePattern);
                return this._mExtraIncludeRegex;
            }
        }
    }
}