using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Hybrid;
using TurboVolt.DTOs;

namespace TurboVolt.Services;

public interface ILookupService
{
    Task<List<ClientResponseDto>> GetClientsLookupAsync();
    Task<List<UserResponseDto>> GetUsersLookupAsync();
    Task<List<FamilleArticleResponseDto>> GetFamillesLookupAsync();
    Task<List<SousFamilleArticleResponseDto>> GetSousFamillesLookupAsync(int? idFamille = null);
}

public class LookupService : ILookupService
{
    private readonly string _connectionString;
    private readonly HybridCache _cache;

    public LookupService(IConfiguration configuration, HybridCache cache)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException(nameof(configuration));
        _cache = cache;
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<List<ClientResponseDto>> GetClientsLookupAsync()
    {
        return await _cache.GetOrCreateAsync(
            "lookup-clients",
            async cancel =>
            {
                using var connection = CreateConnection();
                string sql = @"
                    SELECT 
                        ID_CLIENT AS IdClient,
                        REF_CLT AS RefClt,
                        NOM_CLIENT AS NomClient,
                        NUM_TELE AS NumTele,
                        EMAIL_CLIENT AS EmailClient,
                        VILLE AS Ville,
                        ICE AS Ice
                    FROM CLIENT
                    WHERE SUPPRIME IS NULL OR SUPPRIME = 0
                    ORDER BY NOM_CLIENT;";

                var result = await connection.QueryAsync<ClientResponseDto>(sql);
                return result.ToList();
            },
            options: new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromHours(1)
            }
        );
    }

    public async Task<List<UserResponseDto>> GetUsersLookupAsync()
    {
        return await _cache.GetOrCreateAsync(
            "lookup-users",
            async cancel =>
            {
                using var connection = CreateConnection();
                string sql = @"
                    SELECT 
                        ID_USER AS IdUser,
                        NOM AS Nom,
                        PRENOM AS Prenom
                    FROM UTILISATEUR
                    WHERE SUPPRIME IS NULL OR SUPPRIME = 0
                    ORDER BY NOM;";

                var result = await connection.QueryAsync<UserResponseDto>(sql);
                return result.ToList();
            },
            options: new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromHours(2)
            }
        );
    }

    public async Task<List<FamilleArticleResponseDto>> GetFamillesLookupAsync()
    {
        return await _cache.GetOrCreateAsync(
            "lookup-familles",
            async cancel =>
            {
                using var connection = CreateConnection();
                string sql = @"
                    SELECT 
                        ID_FAMILLE_ARTICLE AS IdFamilleArticle,
                        CODE_FAMILLE AS CodeFamille,
                        LIBELLE_FAM_ARTICLE AS LibelleFamArticle
                    FROM FAMILLEARTICLE
                    WHERE SUPPRIME IS NULL OR SUPPRIME = 0
                    ORDER BY LIBELLE_FAM_ARTICLE;";

                var result = await connection.QueryAsync<FamilleArticleResponseDto>(sql);
                return result.ToList();
            },
            options: new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromHours(4)
            }
        );
    }

    public async Task<List<SousFamilleArticleResponseDto>> GetSousFamillesLookupAsync(int? idFamille = null)
    {
        string cacheKey = idFamille.HasValue ? $"lookup-sousfamilles-fam-{idFamille.Value}" : "lookup-sousfamilles-all";

        return await _cache.GetOrCreateAsync(
            cacheKey,
            async cancel =>
            {
                using var connection = CreateConnection();
                string sql = @"
                    SELECT 
                        sf.ID_SOUS_FAMILLE AS IdSousFamille,
                        sf.ID_FAMILLE_ARTICLE AS IdFamilleArticle,
                        sf.LIBELLE_SOUS_FAMILLE AS LibelleSousFamille,
                        f.LIBELLE_FAM_ARTICLE AS LibelleFamilleParente
                    FROM SOUSFAMILLEARTICLE sf
                    LEFT JOIN FAMILLEARTICLE f ON sf.ID_FAMILLE_ARTICLE = f.ID_FAMILLE_ARTICLE
                    WHERE (sf.SUPPRIMER IS NULL OR sf.SUPPRIMER = 0)
                    AND (@idFamille IS NULL OR sf.ID_FAMILLE_ARTICLE = @idFamille)
                    ORDER BY sf.LIBELLE_SOUS_FAMILLE;";

                var result = await connection.QueryAsync<SousFamilleArticleResponseDto>(sql, new { idFamille });
                return result.ToList();
            },
            options: new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromHours(4)
            }
        );
    }
}