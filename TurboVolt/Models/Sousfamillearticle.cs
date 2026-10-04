using System;
using System.Collections.Generic;

namespace TurboVolt.Models;

public partial class Sousfamillearticle
{
    public int IdSousFamille { get; set; }

    public string? LibelleSousFamille { get; set; }

    public string? Description { get; set; }

    public int? IdFamilleArticle { get; set; }

    public DateTime? DateCreation { get; set; }

    public DateTime? DateModification { get; set; }

    public bool? Supprimer { get; set; }

    public int? ProgId { get; set; }

    public bool? ActivePourMobile { get; set; }

    public byte[]? Image { get; set; }

    public string? Icon { get; set; }

    public string? LibelleSousFamilleAr { get; set; }

    public string? DescriptionAr { get; set; }
}
