using System;
using System.Collections.Generic;

namespace TurboVolt.Models;

public partial class Article
{
    public int IdArticle { get; set; }

    public int? IdTva { get; set; }

    public int? IdFamilleArticle { get; set; }

    public string? RefArticle { get; set; }

    public long? NumArticle { get; set; }

    public string? Designation { get; set; }

    public string? Description { get; set; }

    public DateTime? DateCreation { get; set; }

    public DateTime? DateModification { get; set; }

    public decimal? PrixVenteArticleHt { get; set; }

    public bool? Supprime { get; set; }

    public string? Unite { get; set; }

    public int? IdFournisseur { get; set; }

    public decimal? PrixAchat { get; set; }

    public DateTime? DateDebut { get; set; }

    public DateTime? DateFin { get; set; }

    public decimal? StockMin { get; set; }

    public decimal? StockMax { get; set; }

    public decimal? Pmp { get; set; }

    public decimal? PrixMin { get; set; }

    public byte[]? Image { get; set; }

    public string? NomImage { get; set; }

    public string? TypeImage { get; set; }

    public decimal? PrixVenteArticleTtc { get; set; }

    public decimal? PrixMinTtc { get; set; }

    public decimal? PrixAchatTtc { get; set; }

    public string? CodeAbarre { get; set; }

    public string? EncodageType { get; set; }

    public decimal? NbreParColis { get; set; }

    public decimal? QteParColis { get; set; }

    public string? CodeFrs { get; set; }

    public string? CodeF { get; set; }

    public string? CodeSf { get; set; }

    public decimal? Colis { get; set; }

    public int? IsPlinthe { get; set; }

    public decimal? Stock { get; set; }

    public decimal? DernierPrixAchat { get; set; }

    public decimal? DernierPrixVente { get; set; }

    public decimal? Pmpachat { get; set; }

    public decimal? Pmpvente { get; set; }

    public bool? IsMouvemente { get; set; }

    public decimal? Marge { get; set; }

    public int? IsTtc { get; set; }

    public decimal? RemiseMax { get; set; }

    public decimal? RemiseFrs { get; set; }

    public DateTime? DatePeremption { get; set; }

    public int? IdMarque { get; set; }

    public bool? AlertPeremption { get; set; }

    public decimal? PrixAchatBrutHt { get; set; }

    public decimal? PrixAchatBrutTtc { get; set; }

    public string? CodeAbarre2 { get; set; }

    public string? CodeAbarreTitle { get; set; }

    public int? IdColeur { get; set; }

    public decimal? PrixRevendeur { get; set; }

    public decimal? PrixPublique { get; set; }

    public int? IsService { get; set; }

    public int? IdProd { get; set; }

    public int? IdEmplacement { get; set; }

    public int? IdArticle2016 { get; set; }

    public decimal? OldPrix { get; set; }

    public decimal? RatingStar { get; set; }

    public bool? IsPromotion { get; set; }

    public bool? MobileActive { get; set; }

    public bool? MobileAcceuil { get; set; }

    public bool? PromotionMobileActive { get; set; }

    public decimal? PromotionMobile { get; set; }

    public int? Notation { get; set; }

    public int? IdFamille { get; set; }

    public bool? IsDisponible { get; set; }

    public bool? IsActive { get; set; }

    public string? DesignationAr { get; set; }

    public byte[]? CodeAbarreImage { get; set; }

    public string? QrCodeText { get; set; }

    public byte[]? QrCodeImage { get; set; }

    public bool? IsNew { get; set; }

    public bool? IsDestockage { get; set; }

    public decimal? PrixParUnite { get; set; }

    public decimal? QteUnite { get; set; }

    public decimal? PrixParUniteAchat { get; set; }

    public decimal? PrixVenteMin { get; set; }

    public decimal? PrixVenteMax { get; set; }

    public decimal? PrixMaxTtc { get; set; }

    public decimal? PrixParUniteAchatMin { get; set; }

    public decimal? PrixParUniteAchatMax { get; set; }

    public Famillearticle Famillearticle { get; set; }
    public Sousfamillearticle Sousfamillearticle { get; set; }
}
