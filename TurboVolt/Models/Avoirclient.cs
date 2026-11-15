using System;
using System.Collections.Generic;

namespace TurboVolt.Models;

public partial class Avoirclient
{
    public int IdAvoirClient { get; set; }

    public int? IdClient { get; set; }

    public string? Remarques { get; set; }

    public DateTime? DateCreation { get; set; }

    public DateTime? DateModification { get; set; }

    public decimal? TotatHt { get; set; }

    public decimal? TotalTtc { get; set; }

    public decimal? ResteADonner { get; set; }

    public bool? Supprime { get; set; }

    public string? CodeAvoir { get; set; }

    public int? UserId { get; set; }

    public int? Idmagasin { get; set; }

    public int? Idtype { get; set; }

    public int? IdExercice { get; set; }

    public int? IdbonLivraison { get; set; }

    public int? IdFacture { get; set; }

    public DateTime? IsReported { get; set; }

    public int? NumAvoir { get; set; }

    public string? NomClient { get; set; }

    public DateTime? DateCreationAuto { get; set; }

    public virtual ICollection<AvoirclientXArticle> AvoirclientXArticle { get; set; } = new List<AvoirclientXArticle>();
}
