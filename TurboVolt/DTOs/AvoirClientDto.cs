using Microsoft.AspNetCore.Razor.TagHelpers;

namespace TurboVolt.DTOs
{
    public class AvoirClientDto
    {
        public int IdAvoirClient { get; set; }
        public int? IdClient { get; set; }
        public string? NomClient { get; set; }
        public string? CodeAvoir { get; set; }
        public int? NumAvoir { get; set; }
        public string? Remarques { get; set; }
        public DateTime? DateCreation { get; set; }
        public decimal? TotatHt { get; set; }
        public decimal? TotalTtc { get; set; }
        public decimal? ResteADonner { get; set; }
        public int? IdbonLivraison { get; set; }
        public int? IdFacture { get; set; }
        public List<AvoirClientXArticleDto> Lignes { get; set; } = new();
    }

    public class AvoirClientXArticleDto
    {
        public int Id { get; set; }
        public int IdAvoirClient { get; set; }
        public int IdArticle { get; set; }
        public string? Designation { get; set; }
        public decimal? PrixAvoir { get; set; }
        public decimal? PrixAvoirTtc { get; set; }
        public double? QuantiteRetournee { get; set; }
        public double? Tva { get; set; }
        public double? TauxRemise { get; set; }
        public decimal? QuantiteReparer { get; set; }
        public decimal? QteColis { get; set; }
        public decimal? QteParColis { get; set; }
    }
    public class AvoirBenificeDetailDto
    {
        public string? CodeAvoir { get; set; }
        public string? NomClient { get; set; }
        public DateTime? DateCreation { get; set; }
        public string? ReferenceArticle { get; set; }
        public string? UserName { get; set; }
        public double QuantiteRetournee { get; set; }
        public decimal PrixAvoir { get; set; }
        public int BlMatchCount { get; set; }
        public decimal BenificeAvoirCalculated { get; set; }
        public string? MethodeCalcul { get; set; }
    }
    public class AvoirBenificeByUserDto
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public int NombreAvoirs { get; set; }
        public int NombreLignes { get; set; }
        public int LignesCalculeesParBL { get; set; }
        public int LignesCalculeesParPMP { get; set; }
        public decimal TotalBenificeAvoirCalculated { get; set; }
    }
    public class BenificeDifferenceByUserDto
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public decimal GlobalBlBenifice { get; set; }
        public decimal GlobalAvoirBenifice { get; set; }
        public decimal DifferenceBenifice { get; set; }
    }
}