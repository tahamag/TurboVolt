using System;
using System.Collections.Generic;

namespace TurboVolt.Models;

public partial class AvoirclientXArticle
{
    public int Id { get; set; }

    public int IdAvoirClient { get; set; }

    public int IdArticle { get; set; }

    public decimal? PrixAvoir { get; set; }

    public double? Tva { get; set; }

    public double? QuantiteRetournee { get; set; }

    public bool? Supprime { get; set; }

    public int? Iddepot { get; set; }

    public decimal? QuantiteReparer { get; set; }

    public decimal? PrixAvoirTtc { get; set; }

    public double? TauxRemise { get; set; }

    public string? Designation { get; set; }

    public decimal? QteColis { get; set; }

    public decimal? QteParColis { get; set; }

    public int? IdCouleur { get; set; }

    public int? IdTaille { get; set; }

    public int? IdLineBl { get; set; }

    public virtual Avoirclient IdAvoirClientNavigation { get; set; } = null!;
}
