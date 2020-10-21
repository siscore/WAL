using WAL.Static.Enums;

namespace WAL.Models
{
    public class CategorySectionModel
    {
        public int Id { get; set; }

        public int GameId { get; set; }

        public string Name { get; set; }

        public GameSectionPackageMapPackageType PackageType { get; set; }

        public string Path { get; set; }

        public string InitialInclusionPattern { get; set; }

        public string ExtraIncludePattern { get; set; }

        public int GameCategoryId { get; set; }
    }
}