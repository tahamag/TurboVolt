namespace TurboVolt.DTOs;

// --- DTOs pour le filtrage et la pagination ---
public class BreceptionFilterDto
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public DateTime? DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public int? IdFournisseur { get; set; }
    public int? IdDepot { get; set; }
    public int? IdUser { get; set; }
    public string? RefArticle { get; set; }
    public int? IdFamille { get; set; }
}

// --- DTOs de Réponse (Lecture) ---
public class BreceptionResponseDto
{
    public int IdBonReception { get; set; }
    public int? IdCommandeFournisseur { get; set; }
    public int? IdFournisseur { get; set; }
    public string? NomFournisseur { get; set; }
    public string? NoBl { get; set; }
    public string? RefBonReceptionInterne { get; set; }
    public string? RefFactureFrs { get; set; }
    public string? Description { get; set; }
    public DateTime? DateCreation { get; set; }
    public DateTime? DateModification { get; set; }
    public decimal? TotalBonReceptionHt { get; set; }
    public decimal? TotalBonReceptionTtc { get; set; }
    public decimal? ResteADonner { get; set; }
    public int? IdDepot { get; set; }
    public string? NomDepot { get; set; }

    public UserResponseDto? AgentCreateur { get; set; }
    public UserResponseDto? AgentModificateur { get; set; }

    public List<BreceptionXArticleResponseDto> LignesArticles { get; set; } = new();
}
