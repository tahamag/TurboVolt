namespace TurboVolt.DTOs
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }
    public class BlivraisonFilterDto
    {
        public DateTime? DateDebut { get; set; }
        public DateTime? DateFin { get; set; }
        public int? IdClient { get; set; }
        public int? IdUser { get; set; }
        public string? RefArticle { get; set; }
        public int? IdFamille { get; set; }
        public int? IdSousFamille { get; set; }

        // Pagination
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
    public class ArticleFilterDto
    {
        public string? SearchTerm { get; set; } // Ref ou Désignation
        public int? IdFamille { get; set; }
        public int? IdSousFamille { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
