using System;
using System.Collections.Generic;

namespace TurboVolt.Models;

public partial class Blivraison
{
    public int IdBonLivraison { get; set; }

    public int? IdCommande { get; set; }

    public int? IdClient { get; set; }

    public string? RefBonLivraison { get; set; }

    public int? NoBonLivraison { get; set; }

    public string? Description { get; set; }

    public DateTime? DateCreation { get; set; }

    public DateTime? DateModification { get; set; }

    public string? EtatBonLivraison { get; set; }

    public decimal? TotalBonLivraisonHt { get; set; }

    public decimal? TotalBonLivraisonTtc { get; set; }

    public decimal? ResteAPayer { get; set; }

    public bool? Supprime { get; set; }

    public int? Iduser { get; set; }

    public int? Idmagasin { get; set; }

    public decimal? TotalRemise { get; set; }

    public int? IdUserModification { get; set; }

    public string? MotifModification { get; set; }

    public int? IdExercice { get; set; }

    public DateTime? IsReported { get; set; }

    public DateTime? DateLivraison { get; set; }

    public int? IdDevis { get; set; }

    public int? IdRepresentant { get; set; }

    public string? NomClient { get; set; }

    public int? IsFromStock { get; set; }

    public string? IsFromFacture { get; set; }

    public string? Telephone { get; set; }

    public string? Ville { get; set; }

    public string? Adresse { get; set; }

    public string? RemiseSur { get; set; }

    public string? TypeRemise { get; set; }

    public int? IdBonLivraisonStock { get; set; }

    public int? IsTransfere { get; set; }

    public decimal? RemiseFamClt { get; set; }

    public decimal? Escompte { get; set; }

    public string? RefCommande { get; set; }

    public int? IdBl { get; set; }

    public decimal? DroitTimbre { get; set; }

    public string? NumeroFacture { get; set; }

    public int? IsBloque { get; set; }

    public int? IdModeReglement { get; set; }

    public int? IdCommandeMobile { get; set; }

    public string? ConditionReglement { get; set; }

    public int? IdUserSupprimer { get; set; }

    public DateTime? DateSupprimer { get; set; }

    public virtual ICollection<BlivraisonXArticle> BlivraisonXArticle { get; set; } = new List<BlivraisonXArticle>();

    public virtual User? IdUserModificationNavigation { get; set; }

    public virtual User? IduserNavigation { get; set; }
}
