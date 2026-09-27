using System;
using System.Collections.Generic;

namespace TurboVolt.Models;

public partial class BlivraisonXArticle
{
    public int Id { get; set; }

    public int IdBonLivraison { get; set; }

    public int IdArticle { get; set; }

    public double? QuantiteLivree { get; set; }

    public string? Designation { get; set; }

    public decimal? PrixBonLivraison { get; set; }

    public decimal? TauxTva { get; set; }

    public DateTime? DateCreation { get; set; }

    public DateTime? DateModification { get; set; }

    public int? Iddepot { get; set; }

    public decimal? TauxRemise { get; set; }

    public int? IdPromotion { get; set; }

    public decimal? PrixBonLivraisonTtc { get; set; }

    public int? Caisse { get; set; }

    public int? Carreaux { get; set; }

    public int? IndexOrder { get; set; }

    public decimal? MontantRemise { get; set; }

    public double? Remise { get; set; }

    public string? RemiseSur { get; set; }

    public int? IdCommande { get; set; }

    public decimal? RemiseFamClt { get; set; }

    public decimal? MontantHt { get; set; }

    public decimal? MontantTtc { get; set; }

    public decimal? QteColis { get; set; }

    public decimal? QteParColis { get; set; }

    public int? IdCouleur { get; set; }

    public int? IdTaille { get; set; }

    public decimal? QteAvoirRetour { get; set; }

    public decimal? QteColisAvoirRetour { get; set; }

    public int? IdLineArticleCommande { get; set; }

    public decimal? DernierPrixAchatTtc { get; set; }

    public virtual Blivraison IdBonLivraisonNavigation { get; set; } = null!;
}
