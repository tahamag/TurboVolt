using System;
using System.Collections.Generic;

namespace TurboVolt.Models;

public partial class BreceptionXArticle
{
    public int Id { get; set; }

    public int IdBonReception { get; set; }

    public int IdArticle { get; set; }

    public double? QuantiteReceve { get; set; }

    public decimal? PrixBonReception { get; set; }

    public decimal? TauxTva { get; set; }

    public DateTime? DateCreation { get; set; }

    public bool? Supprime { get; set; }

    public decimal? PrixBonReceptionTtc { get; set; }

    public int? Caisse { get; set; }

    public int? Carreaux { get; set; }

    public int? IdDepot { get; set; }

    public int? IndexOrder { get; set; }

    public decimal? RemiseFournisseur { get; set; }

    public decimal? MontantHt { get; set; }

    public decimal? MontantTtc { get; set; }

    public bool? PriceIsTtc { get; set; }

    public int? IdCouleur { get; set; }

    public int? IdTaille { get; set; }

    public int? IdCommandeFournisseur { get; set; }

    public int? IdLigneArticleCommandeFournisseur { get; set; }

    public virtual Breception IdBonReceptionNavigation { get; set; } = null!;
}
