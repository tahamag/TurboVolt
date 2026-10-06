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
                        IdClient AS IdClient,
                        RefClt AS RefClt,
                        NomClient AS NomClient,
                        NumTele AS NumTele,
                        EmailClient AS EmailClient,
                        VILLE AS Ville,
                        ICE AS Ice
                    FROM CLIENT
                    WHERE SUPPRIME IS NULL OR SUPPRIME = 0
                    ORDER BY NomClient;";

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
                        IdUser AS IdUser,
                        NOM AS Nom,
                        PRENOM AS Prenom
                    FROM dbo.[USER]
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
                        IdFamilleArticle AS IdFamilleArticle,
                        CodeFamille AS CodeFamille,
                        LibelleFamArticle AS LibelleFamArticle
                    FROM FAMILLEARTICLE
                    WHERE SUPPRIME IS NULL OR SUPPRIME = 0
                    ORDER BY LibelleFamArticle;";

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
                        sf.IdSousFamille AS IdSousFamille,
                        sf.IdFamilleArticle AS IdFamilleArticle,
                        sf.LibelleSousFamille AS LibelleSousFamille,
                        f.LibelleFamArticle AS LibelleFamilleParente
                    FROM SOUSFAMILLEARTICLE sf
                    LEFT JOIN FAMILLEARTICLE f ON sf.IdFamilleArticle = f.IdFamilleArticle
                    WHERE (sf.SUPPRIMER IS NULL OR sf.SUPPRIMER = 0)
                    AND (@idFamille IS NULL OR sf.IdFamilleArticle = @idFamille)
                    ORDER BY sf.LibelleSousFamille;";

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