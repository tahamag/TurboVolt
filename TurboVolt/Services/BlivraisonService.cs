using System.ComponentModel.DataAnnotations;
using System.Data;
using Dapper;
using FluentValidation;
using Microsoft.Data.SqlClient;
using TurboVolt.DTOs;

namespace TurboVolt.Services
{
    public interface IBlivraisonService
    {
        Task<PagedResult<BlivraisonResponseDto>> GetBlivraisonsPagedAsync(BlivraisonFilterDto filter);
        Task<BlivraisonResponseDto?> GetBlivraisonByIdAsync(int id);
    }
    public class BlivraisonService : IBlivraisonService
    {
        private readonly string _connectionString;
        private readonly IValidator<BlivraisonFilterDto> _validator;

        public BlivraisonService( IConfiguration configuration , IValidator<BlivraisonFilterDto> validator)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new ArgumentException(nameof(configuration));
            _validator = validator;
        }
        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<PagedResult<BlivraisonResponseDto>> GetBlivraisonsPagedAsync(BlivraisonFilterDto filter)
        {
            await _validator.ValidateAndThrowAsync(filter);

            using var connection = CreateConnection();

            var builder = new SqlBuilder();
            var selector = builder.AddTemplate(@"
            SELECT COUNT(1) FROM BLIVRAISON b /**where**/;

            SELECT 
                b.IdBL AS IdBonLivraison,
                b.RefBonLivraison AS RefBonLivraison,
                b.NoBonLivraison AS NoBonLivraison,
                b.DESCRIPTION AS Description,
                b.DateLivraison AS DateLivraison,
                b.TotalBonLivraisonHT AS TotalBonLivraisonHt,
                b.TotalBonLivraisonTTC AS TotalBonLivraisonTtc,
                b.TotalRemise AS TotalRemise,
                b.RESTE_A_PAYER AS ResteAPayer,
                c.IdClient AS IdClient,
                c.NomClient AS NomClient,
                c.NumClient AS Telephone,
                c.VILLE AS Ville,
                u1.IdUser AS IdUser, u1.NOM AS Nom, u1.PRENOM AS Prenom,
                u2.IdUser AS IdUser, u2.NOM AS Nom, u2.PRENOM AS Prenom
            FROM BLIVRAISON b
            LEFT JOIN CLIENT c ON b.IdClient = c.IdClient
            LEFT JOIN dbo.[USER] u1 ON b.IDUser = u1.IdUser
            LEFT JOIN dbo.[USER] u2 ON b.IDUser = u2.IdUser
            /**where**/
            ORDER BY b.DATE_LIVRAISON DESC
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        ", new { Offset = (filter.PageNumber - 1) * filter.PageSize, PageSize = filter.PageSize });

            builder.Where("b.SUPPRIME IS NULL OR b.SUPPRIME = 0");

            if (filter.DateDebut.HasValue) builder.Where("b.DATE_LIVRAISON >= @DateDebut", new { filter.DateDebut });
            if (filter.DateFin.HasValue) builder.Where("b.DATE_LIVRAISON <= @DateFin", new { filter.DateFin });
            if (filter.IdClient.HasValue) builder.Where("b.ID_CLIENT = @IdClient", new { filter.IdClient });
            if (filter.IdUser.HasValue) builder.Where("(b.ID_AGENT_CREATION = @IdUser OR b.ID_AGENT_MODIFICATION = @IdUser)", new { filter.IdUser });

            if (!string.IsNullOrEmpty(filter.RefArticle) || filter.IdFamille.HasValue || filter.IdSousFamille.HasValue)
            {
                builder.Where(@"EXISTS (
                SELECT 1 FROM BLIVRAISON_X_ARTICLE l
                INNER JOIN ARTICLE a ON l.ID_ARTICLE = a.ID_ARTICLE
                WHERE l.ID_BON_LIVRAISON = b.ID_BON_LIVRAISON
                AND (@RefArticle IS NULL OR a.REF_ARTICLE LIKE '%' + @RefArticle + '%')
                AND (@IdFamille IS NULL OR a.ID_FAMILLE = @IdFamille)
                AND (@IdSousFamille IS NULL OR a.ID_FAMILLE_ARTICLE = @IdSousFamille)
            )", new { filter.RefArticle, filter.IdFamille, filter.IdSousFamille });
            }

            using var multi = await connection.QueryMultipleAsync(selector.RawSql, selector.Parameters);
            int totalCount = await multi.ReadFirstAsync<int>();

            var blList = multi.Read<BlivraisonResponseDto, ClientResponseDto, UserResponseDto, UserResponseDto, BlivraisonResponseDto>(
                (bl, client, createur, modificateur) =>
                {
                    if (client != null)
                    {
                        bl.IdClient = client.IdClient;
                        bl.NomClient = client.NomClient;
                        bl.Telephone = client.NumTele;
                        bl.Ville = client.Ville;
                    }
                    bl.AgentCreateur = createur;
                    bl.AgentModificateur = modificateur;
                    return bl;
                },
                splitOn: "IdClient,IdUser,IdUser"
            ).ToList();

            // Récupération globale des lignes d'articles associées pour les BLs récupérés
            if (blList.Any())
            {
                var blIds = blList.Select(x => x.IdBonLivraison).ToList();
                string linesSql = @"
                SELECT 
                    l.ID AS Id,
                    l.ID_BON_LIVRAISON AS IdBonLivraison,
                    l.ID_ARTICLE AS IdArticle,
                    a.REF_ARTICLE AS RefArticle,
                    a.DESIGNATION AS Designation,
                    f.LIBELLE_FAM_ARTICLE AS LibelleFamille,
                    sf.LIBELLE_SOUS_FAMILLE AS LibelleSousFamille,
                    l.QUANTITE_LIVREE AS QuantiteLivree,
                    l.PRIX_BON_LIVRAISON AS PrixBonLivraison,
                    l.TAUX_TVA AS TauxTva,
                    l.REMISE AS Remise,
                    l.MONTANT_REMISE AS MontantRemise,
                    l.MONTANT_HT AS MontantHt,
                    l.MONTANT_TTC AS MontantTtc
                FROM BLIVRAISON_X_ARTICLE l
                INNER JOIN ARTICLE a ON l.ID_ARTICLE = a.ID_ARTICLE
                LEFT JOIN FAMILLEARTICLE f ON a.ID_FAMILLE = f.ID_FAMILLE_ARTICLE
                LEFT JOIN SOUSFAMILLEARTICLE sf ON a.ID_FAMILLE_ARTICLE = sf.ID_SOUS_FAMILLE
                WHERE l.ID_BON_LIVRAISON IN @blIds AND (l.SUPPRIME IS NULL OR l.SUPPRIME = 0);";

                var lines = (await connection.QueryAsync<BlivraisonXArticleResponseDto>(linesSql, new { blIds })).ToList();

                foreach (var bl in blList)
                {
                    bl.BL_Articles = lines.Where(x => x.IdBonLivraison == bl.IdBonLivraison).ToList();
                }
            }

            return new PagedResult<BlivraisonResponseDto>
            {
                Items = blList,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            };
        }

        public async Task<BlivraisonResponseDto?> GetBlivraisonByIdAsync(int id)
        {
            using var connection = CreateConnection();

            string sql = @"
            SELECT 
                b.ID_BON_LIVRAISON AS IdBonLivraison,
                b.REF_BON_LIVRAISON AS RefBonLivraison,
                b.NO_BON_LIVRAISON AS NoBonLivraison,
                b.DESCRIPTION AS Description,
                b.DATE_LIVRAISON AS DateLivraison,
                b.TOTAL_BON_LIVRAISON_HT AS TotalBonLivraisonHt,
                b.TOTAL_BON_LIVRAISON_TTC AS TotalBonLivraisonTtc,
                b.TOTAL_REMISE AS TotalRemise,
                b.RESTE_A_PAYER AS ResteAPayer,
                c.ID_CLIENT AS IdClient, c.NOM_CLIENT AS NomClient, c.NUM_TELE AS NumTele, c.VILLE AS Ville,
                u1.ID_USER AS IdUser, u1.NOM AS Nom, u1.PRENOM AS Prenom,
                u2.ID_USER AS IdUser, u2.NOM AS Nom, u2.PRENOM AS Prenom
            FROM BLIVRAISON b
            LEFT JOIN CLIENT c ON b.ID_CLIENT = c.ID_CLIENT
            LEFT JOIN UTILISATEUR u1 ON b.ID_AGENT_CREATION = u1.ID_USER
            LEFT JOIN UTILISATEUR u2 ON b.ID_AGENT_MODIFICATION = u2.ID_USER
            WHERE b.ID_BON_LIVRAISON = @id AND (b.SUPPRIME IS NULL OR b.SUPPRIME = 0);

            SELECT 
                l.ID AS Id,
                l.ID_BON_LIVRAISON AS IdBonLivraison,
                l.ID_ARTICLE AS IdArticle,
                a.REF_ARTICLE AS RefArticle,
                a.DESIGNATION AS Designation,
                f.LIBELLE_FAM_ARTICLE AS LibelleFamille,
                sf.LIBELLE_SOUS_FAMILLE AS LibelleSousFamille,
                l.QUANTITE_LIVREE AS QuantiteLivree,
                l.PRIX_BON_LIVRAISON AS PrixBonLivraison,
                l.TAUX_TVA AS TauxTva,
                l.REMISE AS Remise,
                l.MONTANT_REMISE AS MontantRemise,
                l.MONTANT_HT AS MontantHt,
                l.MONTANT_TTC AS MontantTtc
            FROM BLIVRAISON_X_ARTICLE l
            INNER JOIN ARTICLE a ON l.ID_ARTICLE = a.ID_ARTICLE
            LEFT JOIN FAMILLEARTICLE f ON a.ID_FAMILLE = f.ID_FAMILLE_ARTICLE
            LEFT JOIN SOUSFAMILLEARTICLE sf ON a.ID_FAMILLE_ARTICLE = sf.ID_SOUS_FAMILLE
            WHERE l.ID_BON_LIVRAISON = @id AND (l.SUPPRIME IS NULL OR l.SUPPRIME = 0);";

            using var multi = await connection.QueryMultipleAsync(sql, new { id });

            var bl = multi.Read<BlivraisonResponseDto, ClientResponseDto, UserResponseDto, UserResponseDto, BlivraisonResponseDto>(
                (b, client, createur, modificateur) =>
                {
                    if (client != null)
                    {
                        b.IdClient = client.IdClient;
                        b.NomClient = client.NomClient;
                        b.Telephone = client.NumTele;
                        b.Ville = client.Ville;
                    }
                    b.AgentCreateur = createur;
                    b.AgentModificateur = modificateur;
                    return b;
                },
                splitOn: "IdClient,IdUser,IdUser"
            ).FirstOrDefault();

            if (bl != null)
            {
                bl.BL_Articles = (await multi.ReadAsync<BlivraisonXArticleResponseDto>()).ToList();
            }

            return bl;
        }
    }
}
