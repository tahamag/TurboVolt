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
    public class UserBenificeDto
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }

        // Somme des bénéfices générés par les Bons de Livraison (BL) de cet utilisateur
        public decimal BenificeBl { get; set; }

        // Somme des bénéfices/valeurs restituées par les Avoirs (déduits/impactés par article)
        public decimal BenificeAvoir { get; set; }

        // Bénéfice Réel Net = BenificeBl - BenificeAvoir
        public decimal BenificeReal { get; set; }
    }
}