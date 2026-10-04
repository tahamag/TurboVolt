using TurboVolt.Models;

namespace TurboVolt.DTOs
{
    public class BlivraisonResponseDto
    {
        public int IdBonLivraison { get; set; }
        public string? RefBonLivraison { get; set; }
        public int? NoBonLivraison { get; set; }
        public string? Description { get; set; }
        public DateTime? DateLivraison { get; set; }

        // Infos Client
        public int? IdClient { get; set; }
        public string? NomClient { get; set; }
        public string? Telephone { get; set; }
        public string? Ville { get; set; }

        // Totaux & Montants
        public decimal? TotalBonLivraisonHt { get; set; }
        public decimal? TotalBonLivraisonTtc { get; set; }
        public decimal? TotalRemise { get; set; }
        public decimal? ResteAPayer { get; set; }

        // Relations (Utilisateurs)
        public UserResponseDto? AgentCreateur { get; set; }
        public UserResponseDto? AgentModificateur { get; set; }

        // Relation (Lignes d'articles du BL)
        public List<BlivraisonXArticleResponseDto> BL_Articles { get; set; } = new();
    }
}
