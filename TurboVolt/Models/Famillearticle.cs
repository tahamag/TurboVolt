using System;
using System.Collections.Generic;

namespace TurboVolt.Models;

public partial class Famillearticle
{
    public int IdFamilleArticle { get; set; }

    public string? LibelleFamArticle { get; set; }

    public string? Description { get; set; }

    public DateTime? DateCreation { get; set; }

    public DateTime? DateModification { get; set; }

    public bool? Supprime { get; set; }

    public string? CodeFamille { get; set; }

    public int? IsCarreaux { get; set; }

    public byte[]? Image { get; set; }

    public string? Icon { get; set; }

    public int? IndexHome { get; set; }

    public bool? ActivePourMobile { get; set; }

    public string? LibelleFamArticleAr { get; set; }

    public string? DescriptionAr { get; set; }

    public decimal? PrixParUnite { get; set; }
}
