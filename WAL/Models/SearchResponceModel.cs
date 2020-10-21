using System;
using System.Collections.Generic;

namespace WAL.Models
{
    public class SearchResponceModel
    {
        public List<AttachmentModel> Attachments { get; set; }

        public List<AuthorModel> Authors { get; set; }

        public List<CategoryModel> Categories { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateModified { get; set; }

        public DateTime DateReleased { get; set; }

        public int DefaultFileId { get; set; }

        public Int64 DownloadCount { get; set; }

        public int GameId { get; set; }

        public string GameName { get; set; }

        public int GamePopularityRank { get; set; }

        public string GameSlug { get; set; }

        public List<GameVersionFileModel> GameVersionLatestFiles { get; set; }

        public int Id { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsExperiemental { get; set; }

        public bool IsFeatured { get; set; }

        public List<AddonFileModel> LatestFiles {get;set;}

        public string Name { get; set; }

        public decimal PopularityScore { get; set; }

        public string PortalName { get; set; }

        public int PrimaryCategoryId { get; set; }

        public string PrimaryLanguage { get; set; }

        public string Slug { get; set;}

        public int Status { get; set; }

        public string Summary { get; set; }

        public string WebSiteUrl { get; set; }
    }
}
