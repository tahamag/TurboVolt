using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using TurboVolt.DTOs;

namespace TurboVolt.Services
{
    public interface IBeneficeService
    {
        Task<List<BeneficeResponseDto>> GetBeneficesAsync(int? idUser = null, DateTime? dateDebut = null, DateTime? dateFin = null);
    }

    public class BeneficeService : IBeneficeService
    {
        private readonly string _connectionString;

        public BeneficeService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentException(nameof(configuration));
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<List<BeneficeResponseDto>> GetBeneficesAsync(int? idUser = null, DateTime? dateDebut = null, DateTime? dateFin = null)
        {
            using var connection = CreateConnection();

            string sql = @"SELECT 
                            bl.RefBonLivraison AS RefBonLivraison,
                            ISNULL(bl.DateLivraison, bl.DateCreation) AS DateHeureVente,
                            u.NOM AS Nom,
    
                            a.IdArticle AS IdArticle,
                            a.RefArticle AS RefArticle,
                            la.Designation AS DesignationArticle,
                            la.QuantiteLivree AS QuantiteLivree,
    
                            la.PrixBonLivraison AS PrixVenteUnitaireHt,
                            la.MontantHt AS MontantTotalVenteHt,
    
                            ISNULL(achat.PrixAchatHistorique, a.PrixAchat) AS PrixAchatUnitaireHt,
                            ISNULL(achat.RefBonReceptionInterne, '') AS RefBonReceptionInterne,
    
                            (ISNULL(la.PrixBonLivraison, 0) - ISNULL(achat.PrixAchatHistorique, ISNULL(a.PrixAchat, 0))) 
                                * ISNULL(la.QuantiteLivree, 0) AS BeneficeHt

                        FROM BLIVRAISON_X_ARTICLE la
                        INNER JOIN BLIVRAISON bl ON la.IdBonLivraison = bl.IdBonLivraison
                        INNER JOIN ARTICLE a ON la.IdArticle = a.IdArticle
                        INNER JOIN dbo.[USER] u ON bl.IDUser = u.IdUser

                        -- Recherche intelligente : d'abord le plus récent avant la vente, sinon le plus proche après la vente
                        OUTER APPLY (
                            SELECT TOP 1 
                                bra.PrixBonReception AS PrixAchatHistorique,
                                br.RefBonReceptionInterne
                            FROM BRECEPTION_X_ARTICLE bra
                            INNER JOIN BRECEPTION br ON bra.IdBonReception = br.IdBonReception
                            WHERE bra.IdArticle = la.IdArticle
                              AND (br.Supprime IS NULL OR br.Supprime = 0)
                            ORDER BY 
                                -- Priorité 1 : Si la date du bon de réception est <= à la date de vente (cherche le plus proche en descendant)
                                -- Priorité 2 : Si aucun n'est antérieur, prend le premier qui vient après (en montant)
                                CASE 
                                    WHEN br.DateCreation <= ISNULL(bl.DateLivraison, bl.DateCreation) THEN 0 
                                    ELSE 1 
                                END ASC,
                                -- Si on est dans le cas antérieur (0), on trie du plus récent au plus ancien (DESC)
                                -- Si on est dans le cas postérieur (1), on trie du plus proche au plus lointain (ASC)
                                CASE 
                                    WHEN br.DateCreation <= ISNULL(bl.DateLivraison, bl.DateCreation) THEN br.DateCreation 
                                END DESC,
                                CASE 
                                    WHEN br.DateCreation > ISNULL(bl.DateLivraison, bl.DateCreation) THEN br.DateCreation 
                                END ASC
                        ) achat

                        WHERE (bl.Supprime IS NULL OR bl.Supprime = 0)
                          AND (@IdUser IS NULL OR bl.IDUser = @IdUser)
                          AND (@DateDebut IS NULL OR ISNULL(bl.DateLivraison, bl.DateCreation) >= @DateDebut)
                          AND (@DateFin IS NULL OR ISNULL(bl.DateLivraison, bl.DateCreation) <= @DateFin)
                        ORDER BY bl.DateLivraison DESC;
            ";

            var result = await connection.QueryAsync<BeneficeResponseDto>(sql, new
            {
                IdUser = idUser,
                DateDebut = dateDebut,
                DateFin = dateFin
            });

            return result.ToList();
        }
    }
}