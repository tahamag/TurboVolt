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

        WITH PagedBl AS (
            SELECT 
                b.IdBonLivraison AS IdBonLivraison,
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
                
                -- Alias uniques pour l'agent créateur (u1)
                u1.IdUser AS IdUserCreateur, 
                u1.NOM AS NomCreateur, 
                u1.PRENOM AS PrenomCreateur,
                
                -- Alias uniques pour l'agent modificateur (u2)
                u2.IdUser AS IdUserModificateur, 
                u2.NOM AS NomModificateur, 
                u2.PRENOM AS PrenomModificateur,
                
                ROW_NUMBER() OVER (ORDER BY b.DateLivraison DESC) AS RowNum
            FROM BLIVRAISON b
            LEFT JOIN CLIENT c ON b.IdClient = c.IdClient
            LEFT JOIN dbo.[USER] u1 ON b.IDUser = u1.IdUser
            LEFT JOIN dbo.[USER] u2 ON b.IdUserModification = u2.IdUser
            /**where**/
        )
        SELECT * FROM PagedBl
        WHERE RowNum BETWEEN @StartRow AND @EndRow;
    ", new
            {
                StartRow = ((filter.PageNumber - 1) * filter.PageSize) + 1,
                EndRow = filter.PageNumber * filter.PageSize
            });

            builder.Where("b.SUPPRIME IS NULL OR b.SUPPRIME = 0");

            if (filter.DateDebut.HasValue) builder.Where("b.DateLivraison >= @DateDebut", new { filter.DateDebut });
            if (filter.DateFin.HasValue) builder.Where("b.DateLivraison <= @DateFin", new { filter.DateFin });
            if (filter.IdClient.HasValue) builder.Where("b.IdClient = @IdClient", new { filter.IdClient });
            if (filter.IdUser.HasValue) builder.Where("(b.IdUser = @IdUser OR b.IdUserModification = @IdUser)", new { filter.IdUser });

            if (!string.IsNullOrEmpty(filter.RefArticle) || filter.IdFamille.HasValue || filter.IdSousFamille.HasValue)
            {
                builder.Where(@"EXISTS (
            SELECT 1 FROM BLIVRAISON_X_ARTICLE l
            INNER JOIN ARTICLE a ON l.IdArticle = a.IdArticle
            WHERE l.IdBonLivraison = b.IdBonLivraison
            AND (@RefArticle IS NULL OR a.RefArticle LIKE '%' + @RefArticle + '%')
            AND (@IdFamille IS NULL OR a.IdFamille = @IdFamille)
            AND (@IdSousFamille IS NULL OR a.IdFamilleArticle = @IdSousFamille)
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
                splitOn: "IdClient,IdUserCreateur,IdUserModificateur"
            ).ToList();

            if (blList.Any())
            {
                var blIds = blList.Select(x => x.IdBonLivraison).ToList();
                string linesSql = @"
        SELECT 
            l.ID AS Id,
            l.IdBonLivraison AS IdBonLivraison,
            l.IdArticle AS IdArticle,
            a.RefArticle AS RefArticle,
            a.DESIGNATION AS Designation,
            f.LibelleFamArticle AS LibelleFamille,
            sf.LibelleSousFamille AS LibelleSousFamille,
            l.QuantiteLivree AS QuantiteLivree,
            l.PrixBonLivraison AS PrixBonLivraison,
            l.TauxTVA AS TauxTva,
            l.REMISE AS Remise,
            l.MontantRemise AS MontantRemise,
            l.MontantHT AS MontantHt,
            l.MontantTTC AS MontantTtc
        FROM BLIVRAISON_X_ARTICLE l
        INNER JOIN ARTICLE a ON l.IdArticle = a.IdArticle
        LEFT JOIN FAMILLEARTICLE f ON a.IdFamille = f.IdFamilleArticle
        LEFT JOIN SOUSFAMILLEARTICLE sf ON a.IdFamilleArticle = sf.IdSousFamille
        WHERE l.IdBonLivraison IN @blIds;";

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
                     b.IdBonLivraison AS IdBonLivraison,
                     b.RefBonLivraison AS RefBonLivraison,
                     b.NoBonLivraison AS NoBonLivraison,
                     b.DESCRIPTION AS Description,
                     b.DateLivraison AS DateLivraison,
                     b.TotalBonLivraisonHT AS TotalBonLivraisonHt,
                     b.TotalBonLivraisonTTC AS TotalBonLivraisonTtc,
                     b.TotalRemise AS TotalRemise,
                     b.RESTE_A_PAYER AS ResteAPayer,
                     c.IdClient AS IdClient, c.NomClient AS NomClient, c.NumTele AS NumTele, c.VILLE AS Ville,
                     u1.IdUser AS IdUser, u1.NOM AS Nom, u1.PRENOM AS Prenom,
                     u2.IdUser AS IdUser, u2.NOM AS Nom, u2.PRENOM AS Prenom
                 FROM BLIVRAISON b
                 LEFT JOIN CLIENT c ON b.IdClient = c.IdClient
                 LEFT JOIN dbo.[USER] u1 ON b.IDUser = u1.IdUser
                  LEFT JOIN dbo.[USER] u2 ON b.IdUserModification = u2.IdUser
                 WHERE b.IdBonLivraison = @id AND (b.SUPPRIME IS NULL OR b.SUPPRIME = 0);

                 SELECT 
                     l.ID AS Id,
                     l.IdBonLivraison AS IdBonLivraison,
                     l.IdArticle AS IdArticle,
                     a.RefArticle AS RefArticle,
                     a.DESIGNATION AS Designation,
                     f.LibelleFamArticle AS LibelleFamille,
                     sf.LibelleSousFamille AS LibelleSousFamille,
                     l.QuantiteLivree AS QuantiteLivree,
                     l.PrixBonLivraison AS PrixBonLivraison,
                     l.TauxTVA AS TauxTva,
                     l.REMISE AS Remise,
                     l.MontantRemise AS MontantRemise,
                     l.MontantHT AS MontantHt,
                     l.MontantTTC AS MontantTtc
                 FROM BLIVRAISON_X_ARTICLE l
                 INNER JOIN ARTICLE a ON l.IdArticle = a.IdArticle
                 LEFT JOIN FAMILLEARTICLE f ON a.IdFamille = f.IdFamilleArticle
                 LEFT JOIN SOUSFAMILLEARTICLE sf ON a.IdFamilleArticle = sf.IdSousFamille
                 WHERE l.IdBonLivraison = @id ;";

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
