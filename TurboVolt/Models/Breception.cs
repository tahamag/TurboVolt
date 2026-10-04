using System;
using System.Collections.Generic;

namespace TurboVolt.Models;

public partial class Breception
{
    public int IdBonReception { get; set; }

    public int? IdCommandeFournisseur { get; set; }

    public int? IdFournisseur { get; set; }

    public string? NoBl { get; set; }

    public string? Description { get; set; }

    public DateTime? DateCreation { get; set; }

    public DateTime? DateModification { get; set; }

    public decimal? TotalBonReceptionHt { get; set; }

    public decimal? TotalBonReceptionTtc { get; set; }

    public decimal? ResteADonner { get; set; }

    public bool? Supprime { get; set; }

    public int? Iddepot { get; set; }

    public int? Iduser { get; set; }

    public int? IduserModification { get; set; }

    public string? MotifModification { get; set; }

    public int? IdMagasin { get; set; }

    public int? IdFacture { get; set; }

    public int? IdExercice { get; set; }

    public string? RefFactureFrs { get; set; }

    public DateTime? DateCreationAuto { get; set; }

    public string? RefBonReceptionInterne { get; set; }

    public int? IdUserSuppression { get; set; }

    public DateTime? DateSuppression { get; set; }

    public virtual ICollection<BreceptionXArticle> BreceptionXArticle { get; set; } = new List<BreceptionXArticle>();
}
