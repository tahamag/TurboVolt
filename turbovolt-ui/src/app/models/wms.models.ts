export interface PagedResult<T> {
    items: T[];
    totalCount: number;
    pageNumber: number;
    pageSize: number;
    totalPages: number;
}

export interface UserResponseDto {
    idUser: number;
    nom?: string;
    prenom?: string;
}

export interface ClientResponseDto {
    idClient: number;
    refClt?: string;
    nomClient?: string;
    numTele?: string;
    emailClient?: string;
    ville?: string;
    ice?: string;
}

export interface FamilleArticleResponseDto {
    idFamilleArticle: number;
    codeFamille?: string;
    libelleFamArticle?: string;
}

export interface SousFamilleArticleResponseDto {
    idSousFamille: number;
    idFamilleArticle: number;
    libelleSousFamille?: string;
    libelleFamilleParente?: string;
}

export interface ArticleFilterDto {
    searchTerm?: string;
    idFamille?: number;
    idSousFamille?: number;
    pageNumber: number;
    pageSize: number;
}

export interface ArticleResponseDto {
    idArticle: number;
    refArticle?: string;
    numArticle?: string;
    designation?: string;
    idFamille?: number;
    libelleFamille?: string;
    idFamilleArticle?: number;
    libelleSousFamille?: string;
    prixVenteArticleHt?: number;
    prixVenteArticleTtc?: number;
    prixAchat?: number;
    prixAchatTtc?: number;
    dernierPrixAchat?: number;
    dernierPrixVente?: number;
    prixPublique?: number;
}

export interface BlivraisonXArticleResponseDto {
    id: number;
    idBonLivraison: number;
    idArticle: number;
    refArticle?: string;
    designation?: string;
    libelleFamille?: string;
    libelleSousFamille?: string;
    quantiteLivree?: number;
    prixBonLivraison?: number;
    tauxTva?: number;
    remise?: number;
    montantRemise?: number;
    montantHt?: number;
    montantTtc?: number;
}

export interface BlivraisonFilterDto {
    dateDebut?: string;
    dateFin?: string;
    idClient?: number;
    idUser?: number;
    refArticle?: string;
    idFamille?: number;
    idSousFamille?: number;
    pageNumber: number;
    pageSize: number;
}

export interface BlivraisonResponseDto {
    idBonLivraison: number;
    refBonLivraison?: string;
    noBonLivraison?: string;
    description?: string;
    dateLivraison?: string;
    totalBonLivraisonHt?: number;
    totalBonLivraisonTtc?: number;
    totalRemise?: number;
    resteAPayer?: number;
    idClient?: number;
    nomClient?: string;
    telephone?: string;
    ville?: string;
    agentCreateur?: UserResponseDto;
    agentModificateur?: UserResponseDto;
    bL_Articles: BlivraisonXArticleResponseDto[];
}

export interface BreceptionXArticleResponseDto {
    id: number;
    idBonReception: number;
    idArticle: number;
    refArticle?: string;
    designation?: string;
    libelleFamille?: string;
    libelleSousFamille?: string;
    quantiteReceve?: number;
    prixBonReception?: number;
    tauxTva?: number;
    remiseFournisseur?: number;
    montantHt?: number;
    montantTtc?: number;
    priceIsTtc?: boolean;
    idDepot?: number;
    indexOrder?: number;
}

export interface BreceptionFilterDto {
    dateDebut?: string;
    dateFin?: string;
    idFournisseur?: number;
    idDepot?: number;
    idUser?: number;
    refArticle?: string;
    idFamille?: number;
    pageNumber: number;
    pageSize: number;
}

export interface BreceptionResponseDto {
    idBonReception: number;
    idCommandeFournisseur?: number;
    idFournisseur?: number;
    nomFournisseur?: string;
    noBl?: string;
    refBonReceptionInterne?: string;
    refFactureFrs?: string;
    description?: string;
    dateCreation?: string;
    dateModification?: string;
    totalBonReceptionHt?: number;
    totalBonReceptionTtc?: number;
    resteADonner?: number;
    idDepot?: number;
    nomDepot?: string;
    agentCreateur?: UserResponseDto;
    agentModificateur?: UserResponseDto;
    lignesArticles: BreceptionXArticleResponseDto[];
}

export interface BeneficeResponseDto {
    refBonLivraison?: string;
    dateHeureVente?: string;
    nom?: string;
    idArticle: number;
    refArticle?: string;
    designationArticle?: string;
    quantiteLivree?: number;
    prixVenteUnitaireHt?: number;
    montantTotalVenteHt?: number;
    prixAchatUnitaireHt?: number;
    refBonReceptionInterne?: string;
    beneficeHt?: number;
}