namespace TurboVolt.DTOs
{
    public class SousFamilleArticleResponseDto
    {
        public int IdSousFamille { get; set; }
        public int? IdFamilleArticle { get; set; }
        public string? LibelleSousFamille { get; set; }

        // Relation vers la famille parente
        public string? LibelleFamilleParente { get; set; }
    }
}
