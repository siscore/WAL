namespace WAL.Models
{
    public class SearchRequestModel
    {
        public SearchRequestModel()
        {
            GameId = 1;
            SectionId = 1;
            SortDescending = true;
        }

        public int GameId { get; set; }
        public string GameVersionFlavor { get; set; }
        public int CategoryId { get; set; }
        public int Index { get; set; }
        public int PageSize { get; set; }
        public string SearchFilter { get; set; }
        public int SectionId { get; set; }
        public int Sort { get; set; }
        public bool SortDescending { get; set; }

        public override string ToString()
        {
            return string.Format("/api/v2/addon/search?categoryId={0}&gameId={1}&gameVersionFlavor={2}&index={3}&pageSize={4}&searchFilter={5}&sectionId={6}&sort={7}",
                CategoryId,
                GameId,
                GameVersionFlavor,
                Index,
                PageSize,
                SearchFilter,
                SectionId,
                SortDescending ? "true": "false");
        }
    }
}
