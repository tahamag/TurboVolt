using Dapper;
using Microsoft.Data.SqlClient;
using TurboVolt.DTOs;

namespace TurboVolt.Services
{
    public interface IAvoirBenificeService
    {
        Task<List<AvoirBenificeDetailDto>> GetAvoirBenificesAsync(
            int? idUser = null, 
            int? articleId = null, 
            string? articleSearch = null,
            DateTime? dateDebut = null, 
            DateTime? dateFin = null);
        Task<List<AvoirBenificeByUserDto>> GetAvoirBenificeByUserAsync(
            int? idUser = null,
            int? articleId = null,
            string? articleSearch = null,
            DateTime? dateDebut = null,
            DateTime? dateFin = null);
    }

    public class AvoirBenificeService : IAvoirBenificeService
    {
        private readonly string _connectionString;

        public AvoirBenificeService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        private const string BaseSql = @"
            WITH ArticlePMP AS (
                SELECT 
                    bra.IdArticle,
                    STUFF((
                        SELECT DISTINCT ', ' + br_sub.RefBonReceptionInterne
                        FROM dbo.BRECEPTION_X_ARTICLE bra_sub
                        INNER JOIN Breception br_sub ON bra_sub.IdBonReception = br_sub.IdBonReception
                        WHERE bra_sub.IdArticle = bra.IdArticle 
                          AND ISNULL(bra_sub.Supprime, 0) = 0 
                          AND ISNULL(br_sub.Supprime, 0) = 0
                        FOR XML PATH('')
                    ), 1, 2, '') AS RefBonReception,
                    CASE 
                        WHEN SUM(ISNULL(bra.QuantiteReceve, 0)) > 0 
                        THEN SUM(ISNULL(bra.PrixBonReception, 0) * ISNULL(bra.QuantiteReceve, 0)) / SUM(ISNULL(bra.QuantiteReceve, 0))
                        ELSE 0 
                    END AS PmpArticle 
                FROM dbo.BRECEPTION_X_ARTICLE bra
                INNER JOIN Breception br ON bra.IdBonReception = br.IdBonReception
                WHERE ISNULL(bra.Supprime, 0) = 0 AND ISNULL(br.Supprime, 0) = 0
                GROUP BY bra.IdArticle
            ),
            AvoirDetails AS (
                SELECT 
                    ac.IdAvoirClient,
                    ac.CodeAvoir,
                    ac.NumAvoir,
                    ac.DateCreation,
                    ac.NomClient AS NomClientAvoir,
                    c.NomClient AS NomClientTable,
                    ac.IdClient,
                    ac.UserId,
                    ax.Id AS IdAvoirItem,
                    ax.IdArticle,
                    a.RefArticle AS ReferenceArticle,
                    ax.Designation AS DesignationArticle,
                    ISNULL(ax.QuantiteRetournee, 0) AS QuantiteRetournee,
                    ISNULL(ax.PrixAvoir, 0) AS PrixAvoir, 
                    u.Nom AS UserName
                FROM Avoirclient ac
                LEFT JOIN Client c ON ac.IdClient = c.IdClient
                INNER JOIN dbo.AVOIRCLIENT_X_ARTICLE ax ON ac.IdAvoirClient = ax.IdAvoirClient
                JOIN dbo.ARTICLE A ON ax.IdArticle = A.IdArticle
                JOIN dbo.[USER] U ON ac.UserId = u.IdUser
                WHERE ISNULL(ac.Supprime, 0) = 0 
                  AND ISNULL(ax.Supprime, 0) = 0
                  -- Filtres appliqués directement sur la source
                  AND (@UserId IS NULL OR ac.UserId = @UserId)
                  AND (@ArticleId IS NULL OR ax.IdArticle = @ArticleId)
                  AND (@ArticleSearch IS NULL OR a.RefArticle LIKE '%' + @ArticleSearch + '%' OR ax.Designation LIKE '%' + @ArticleSearch + '%')
                  AND (@DateDebut IS NULL OR ac.DateCreation >= @DateDebut)
                  AND (@DateFin IS NULL OR ac.DateCreation < DATEADD(DAY, 1, CAST(@DateFin AS DATE)))
            ),
            MatchingBLCandidates AS (
                SELECT 
                    ad.IdAvoirItem,
                    bl.IdBonLivraison,
                    ISNULL(bl.RefBonLivraison, CAST(bl.NoBonLivraison AS VARCHAR(50))) AS RefBlUtilise,
                    lbl.Id AS IdLineBl,
                    COUNT(bl.IdBonLivraison) OVER (PARTITION BY ad.IdAvoirItem) as BlMatchCount,
                    (
                        ISNULL(lbl.PrixBonLivraison, 0) - 
                        (ISNULL(lbl.DernierPrixAchatTtc, 0) / NULLIF(1.0 + (ISNULL(lbl.TauxTva, 0) / 100.0), 0))
                    ) AS BenificeUnitBl
                FROM AvoirDetails ad
                INNER JOIN Blivraison bl ON bl.IdClient = ad.IdClient
                    AND bl.DateCreation >= DATEADD(MONTH, -3, ad.DateCreation) 
                    AND bl.DateCreation <= ad.DateCreation
                    AND ISNULL(bl.Supprime, 0) = 0
                INNER JOIN dbo.BLIVRAISON_X_ARTICLE lbl ON lbl.IdBonLivraison = bl.IdBonLivraison
                    AND lbl.IdArticle = ad.IdArticle
            )
            SELECT 
                ad.CodeAvoir,
                ISNULL(ad.NomClientAvoir, ad.NomClientTable) AS NomClient,
                ad.DateCreation,
                ad.ReferenceArticle,
                ad.UserName,
                ad.QuantiteRetournee,
                ad.PrixAvoir,
                ISNULL(m.BlMatchCount, 0) AS BlMatchCount,
                CASE 
                    WHEN ISNULL(m.BlMatchCount, 0) = 1 
                        THEN (m.BenificeUnitBl * CAST(ad.QuantiteRetournee AS DECIMAL(18,2)))
                    ELSE 
                        ((ad.PrixAvoir - ISNULL(pmp.PmpArticle, 0)) * CAST(ad.QuantiteRetournee AS DECIMAL(18,2)))
                END AS BenificeAvoirCalculated,
                CASE 
                    WHEN ISNULL(m.BlMatchCount, 0) = 1 
                        THEN 'BL : ' + ISNULL(m.RefBlUtilise, '')
                    ELSE 
                        'PMP (Réf BRs : ' + ISNULL(pmp.RefBonReception, 'N/C') + ' | PMP = ' + CAST(ISNULL(pmp.PmpArticle, 0) AS VARCHAR(50)) + ')'
                END AS MethodeCalcul
            FROM AvoirDetails ad
            LEFT JOIN (
                SELECT IdAvoirItem, RefBlUtilise, BenificeUnitBl, BlMatchCount,
                       ROW_NUMBER() OVER (PARTITION BY IdAvoirItem ORDER BY IdBonLivraison DESC) as rn
                FROM MatchingBLCandidates
            ) m ON ad.IdAvoirItem = m.IdAvoirItem AND m.rn = 1
            LEFT JOIN ArticlePMP pmp ON ad.IdArticle = pmp.IdArticle
            ORDER BY ad.DateCreation DESC;
        ";

        public async Task<List<AvoirBenificeDetailDto>> GetAvoirBenificesAsync(
            int? userId = null,
            int? articleId = null,
            string? articleSearch = null,
            DateTime? dateDebut = null,
            DateTime? dateFin = null)
        {
            using var connection = new SqlConnection(_connectionString);
            var rows = await connection.QueryAsync<AvoirBenificeDetailDto>(
                BaseSql,
                new
                {
                    UserId = userId,
                    ArticleId = articleId,
                    ArticleSearch = string.IsNullOrWhiteSpace(articleSearch) ? null : articleSearch.Trim(),
                    DateDebut = dateDebut,
                    DateFin = dateFin
                }
            );
            return rows.ToList();
        }

        private const string ByUserSql = @"
                    WITH ArticlePMP AS (
                        SELECT 
                            bra.IdArticle,
                            CASE 
                                WHEN SUM(ISNULL(bra.QuantiteReceve, 0)) > 0 
                                THEN SUM(ISNULL(bra.PrixBonReception, 0) * ISNULL(bra.QuantiteReceve, 0)) / SUM(ISNULL(bra.QuantiteReceve, 0))
                                ELSE 0 
                            END AS PmpArticle
                        FROM dbo.BRECEPTION_X_ARTICLE bra
                        INNER JOIN Breception br ON bra.IdBonReception = br.IdBonReception
                        WHERE ISNULL(bra.Supprime, 0) = 0 
                          AND ISNULL(br.Supprime, 0) = 0
                        GROUP BY bra.IdArticle
                    ),
                    AvoirDetails AS (
                        SELECT 
                            ac.IdAvoirClient,
                            ac.IdClient,
                            ac.UserId,
                            u.Nom AS UserName,
                            ac.DateCreation,
                            ax.Id AS IdAvoirItem,
                            ax.IdArticle,
                            ISNULL(ax.QuantiteRetournee, 0) AS QuantiteRetournee,
                            ISNULL(ax.PrixAvoir, 0) AS PrixAvoir
                        FROM Avoirclient ac
                        INNER JOIN dbo.AVOIRCLIENT_X_ARTICLE ax ON ac.IdAvoirClient = ax.IdAvoirClient
                        INNER JOIN dbo.ARTICLE a ON ax.IdArticle = a.IdArticle
                        INNER JOIN dbo.[USER] u ON ac.UserId = u.IdUser
                        WHERE ISNULL(ac.Supprime, 0) = 0 
                          AND ISNULL(ax.Supprime, 0) = 0
                          AND (@UserId IS NULL OR ac.UserId = @UserId)
                          AND (@ArticleId IS NULL OR ax.IdArticle = @ArticleId)
                          AND (@ArticleSearch IS NULL OR a.RefArticle LIKE '%' + @ArticleSearch + '%' OR ax.Designation LIKE '%' + @ArticleSearch + '%')
                          AND (@DateDebut IS NULL OR ac.DateCreation >= @DateDebut)
                          AND (@DateFin IS NULL OR ac.DateCreation < DATEADD(DAY, 1, CAST(@DateFin AS DATE)))
                    ),
                    MatchingBLCandidates AS (
                        SELECT 
                            ad.IdAvoirItem,
                            bl.IdBonLivraison,
                            COUNT(bl.IdBonLivraison) OVER (PARTITION BY ad.IdAvoirItem) AS BlMatchCount,
                            (
                                ISNULL(lbl.PrixBonLivraison, 0) - 
                                (ISNULL(lbl.DernierPrixAchatTtc, 0) / NULLIF(1.0 + (ISNULL(lbl.TauxTva, 0) / 100.0), 0))
                            ) AS BenificeUnitBl
                        FROM AvoirDetails ad
                        INNER JOIN Blivraison bl ON bl.IdClient = ad.IdClient
                            AND bl.DateCreation >= DATEADD(MONTH, -3, ad.DateCreation)
                            AND bl.DateCreation <= ad.DateCreation
                            AND ISNULL(bl.Supprime, 0) = 0
                        INNER JOIN dbo.BLIVRAISON_X_ARTICLE lbl ON lbl.IdBonLivraison = bl.IdBonLivraison
                            AND lbl.IdArticle = ad.IdArticle
                    ),
                    AvoirItemCalculated AS (
                        SELECT 
                            ad.UserId,
                            ad.UserName,
                            ad.IdAvoirClient,
                            ad.IdAvoirItem,
                            CASE 
                                WHEN ISNULL(m.BlMatchCount, 0) = 1 
                                    THEN (m.BenificeUnitBl * CAST(ad.QuantiteRetournee AS DECIMAL(18,2)))
                                ELSE 
                                    ((ad.PrixAvoir - ISNULL(pmp.PmpArticle, 0)) * CAST(ad.QuantiteRetournee AS DECIMAL(18,2)))
                            END AS BenificeAvoirCalculated,
                            CASE WHEN ISNULL(m.BlMatchCount, 0) = 1 THEN 1 ELSE 0 END AS UsedBlCount,
                            CASE WHEN ISNULL(m.BlMatchCount, 0) = 1 THEN 0 ELSE 1 END AS UsedPmpCount
                        FROM AvoirDetails ad
                        LEFT JOIN (
                            SELECT IdAvoirItem, BenificeUnitBl, BlMatchCount,
                                   ROW_NUMBER() OVER (PARTITION BY IdAvoirItem ORDER BY IdBonLivraison DESC) AS rn
                            FROM MatchingBLCandidates
                        ) m ON ad.IdAvoirItem = m.IdAvoirItem AND m.rn = 1
                        LEFT JOIN ArticlePMP pmp ON ad.IdArticle = pmp.IdArticle
                    )
                    SELECT 
                        UserId,
                        UserName,
                        COUNT(DISTINCT IdAvoirClient) AS NombreAvoirs,
                        COUNT(IdAvoirItem) AS NombreLignes,
                        SUM(UsedBlCount) AS LignesCalculeesParBL,
                        SUM(UsedPmpCount) AS LignesCalculeesParPMP,
                        ISNULL(SUM(BenificeAvoirCalculated), 0) AS TotalBenificeAvoirCalculated
                    FROM AvoirItemCalculated
                    GROUP BY UserId, UserName
                    ORDER BY TotalBenificeAvoirCalculated DESC;
                ";

        public async Task<List<AvoirBenificeByUserDto>> GetAvoirBenificeByUserAsync(
            int? userId = null,
            int? articleId = null,
            string? articleSearch = null,
            DateTime? dateDebut = null,
            DateTime? dateFin = null)
        {
            using var connection = new SqlConnection(_connectionString);
            var rows = await connection.QueryAsync<AvoirBenificeByUserDto>(
                ByUserSql,
                new
                {
                    UserId = userId,
                    ArticleId = articleId,
                    ArticleSearch = string.IsNullOrWhiteSpace(articleSearch) ? null : articleSearch.Trim(),
                    DateDebut = dateDebut,
                    DateFin = dateFin
                }
            );
            return rows.ToList();
        }
    }
}