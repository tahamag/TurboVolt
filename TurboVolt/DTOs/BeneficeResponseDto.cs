namespace TurboVolt.DTOs
{
    public class BeneficeResponseDto
    {
        public string? RefBonLivraison { get; set; }
        public DateTime? DateHeureVente { get; set; }
        public string? Nom { get; set; }

        public int IdArticle { get; set; }
        public string? RefArticle { get; set; }
        public string? DesignationArticle { get; set; }
        public double? QuantiteLivree { get; set; }

        public decimal? PrixVenteUnitaireHt { get; set; }
        public decimal? MontantTotalVenteHt { get; set; }

        public decimal? PrixAchatUnitaireHt { get; set; }
        public string? RefBonReceptionInterne { get; set; }
        public decimal? BeneficeHt { get; set; }
    }
}