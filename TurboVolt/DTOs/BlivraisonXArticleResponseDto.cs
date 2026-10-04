namespace TurboVolt.DTOs
{
    public class BlivraisonXArticleResponseDto
    {
        public int Id { get; set; }
        public int IdBonLivraison { get; set; }
        public int IdArticle { get; set; }
        public string? RefArticle { get; set; }
        public string? Designation { get; set; }
        public string? LibelleFamille { get; set; }
        public string? LibelleSousFamille { get; set; }
        public double? QuantiteLivree { get; set; }
        public decimal? PrixBonLivraison { get; set; }
        public decimal? TauxTva { get; set; }
        public double? Remise { get; set; }
        public decimal? MontantRemise { get; set; }
        public decimal? MontantHt { get; set; }
        public decimal? MontantTtc { get; set; }
    }
}
