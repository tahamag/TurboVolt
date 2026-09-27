using System;
using System.Collections.Generic;

namespace TurboVolt.Models;

public partial class User
{
    public int IdUser { get; set; }

    public string? Nom { get; set; }

    public string? Prenom { get; set; }

    public string? Fonction { get; set; }

    public string? DateCreation { get; set; }

    public string? DateModification { get; set; }

    public bool? Supprime { get; set; }

    public int? Idrole { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string? DeviceId { get; set; }

    public string? DeviceName { get; set; }

    public bool? IsAdmin { get; set; }

    public bool? Validation { get; set; }

    public DateTime? ValidationDate { get; set; }

    public bool? IsDeviceLogin { get; set; }

    public string? PlayerId { get; set; }

    public bool? IsCoWorker { get; set; }

    public bool? IsCoupon { get; set; }

    public bool? IsTransport { get; set; }

    public bool? IsCommerciale { get; set; }

    public bool? IsLivreur { get; set; }

    public virtual ICollection<Blivraison> BlivraisonIdUserModificationNavigation { get; set; } = new List<Blivraison>();

    public virtual ICollection<Blivraison> BlivraisonIduserNavigation { get; set; } = new List<Blivraison>();
}
