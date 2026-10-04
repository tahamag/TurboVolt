namespace TurboVolt.DTOs
{
    public class BreceptionXArticleResponseDto
    {
        public int Id { get; set; }
        public int IdBonReception { get; set; }
        public int IdArticle { get; set; }

        // Informations détaillées de l'article
        public string? RefArticle { get; set; }
        public string? Designation { get; set; }
        public string? LibelleFamille { get; set; }
        public string? LibelleSousFamille { get; set; }

        // Quantités & Prix
        public double? QuantiteReceve { get; set; }
        public decimal? PrixBonReception { get; set; }
        public decimal? PrixBonReceptionTtc { get; set; }
        public decimal? MontantHt { get; set; }
        public decimal? MontantTtc { get; set; }
    }
}
