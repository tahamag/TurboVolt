namespace TurboVolt.DTOs
{
    public class ArticleResponseDto
    {
        public int IdArticle { get; set; }
        public string? RefArticle { get; set; }
        public long? NumArticle { get; set; }
        public string? Designation { get; set; }

        // --- Clés et libellés de Catégorisation ---
        // IdFamille = Clé étrangère de FamilleArticle
        public int? IdFamille { get; set; }
        public string? LibelleFamille { get; set; }

        // IdFamilleArticle = Clé étrangère de SousFamilleArticle
        public int? IdFamilleArticle { get; set; }
        public string? LibelleSousFamille { get; set; }

        // --- Prix & Tarification ---
        public decimal? PrixVenteArticleHt { get; set; }
        public decimal? PrixVenteArticleTtc { get; set; }
        public decimal? PrixAchat { get; set; }
        public decimal? PrixAchatTtc { get; set; }
        public decimal? DernierPrixAchat { get; set; }
        public decimal? DernierPrixVente { get; set; }
        public decimal? PrixPublique { get; set; }
    }
}
