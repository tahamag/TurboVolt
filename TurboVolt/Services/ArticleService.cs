using System.Data;
using Dapper;
using FluentValidation;
using Microsoft.Data.SqlClient;
using TurboVolt.DTOs;

namespace TurboVolt.Services;

public interface IArticleService
{
    Task<PagedResult<ArticleResponseDto>> GetArticlesPagedAsync(ArticleFilterDto filter);
    Task<ArticleResponseDto?> GetArticleByIdAsync(int id);
}

public class ArticleService : IArticleService
{
    private readonly string _connectionString;
    private readonly IValidator<ArticleFilterDto> _validator;

    public ArticleService(IConfiguration configuration, IValidator<ArticleFilterDto> validator)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException(nameof(configuration));
        _validator = validator;
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<PagedResult<ArticleResponseDto>> GetArticlesPagedAsync(ArticleFilterDto filter)
    {
        await _validator.ValidateAndThrowAsync(filter);

        using var connection = CreateConnection();

        var builder = new SqlBuilder();
        var selector = builder.AddTemplate(@"
            SELECT COUNT(1) FROM ARTICLE a /**where**/;
            SELECT 
                a.IdArticle AS IdArticle,
                a.RefArticle AS RefArticle,
                a.DESIGNATION AS Designation,
                a.NumArticle AS NumArticle,
                a.IdFamille AS IdFamille,
                f.LibelleFamArticle AS LibelleFamille,
                a.IdFamilleArticle AS IdFamilleArticle,
                sf.LibelleSousFamille AS LibelleSousFamille,
                a.PrixVenteArticleHT AS PrixVenteArticleHt,
                a.PrixVenteArticleTTC AS PrixVenteArticleTtc,
                a.PrixAchat AS PrixAchat,
                a.PrixAchatTTC AS PrixAchatTtc,
                a.DernierPrixAchat AS DernierPrixAchat,
                a.DernierPrixVente AS DernierPrixVente,
                a.PrixPublique AS PrixPublique
            FROM ARTICLE a
            LEFT JOIN FAMILLEARTICLE f ON a.IdFamille = f.IdFamilleArticle
            LEFT JOIN SOUSFAMILLEARTICLE sf ON a.IdFamilleArticle = sf.IdSousFamille
            
            /**where**/
            ORDER BY a.DESIGNATION
            OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        ", new { Offset = (filter.PageNumber - 1) * filter.PageSize, PageSize = filter.PageSize });

        builder.Where("a.SUPPRIME IS NULL OR a.SUPPRIME = 0");

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            builder.Where("(a.REF_ARTICLE LIKE '%' + @SearchTerm + '%' OR a.DESIGNATION LIKE '%' + @SearchTerm + '%')",
                new { SearchTerm = filter.SearchTerm.Trim() });
        }

        if (filter.IdFamille.HasValue) builder.Where("a.ID_FAMILLE = @IdFamille", new { filter.IdFamille });
        if (filter.IdSousFamille.HasValue) builder.Where("a.ID_FAMILLE_ARTICLE = @IdSousFamille", new { filter.IdSousFamille });

        using var multi = await connection.QueryMultipleAsync(selector.RawSql, selector.Parameters);
        int totalCount = await multi.ReadFirstAsync<int>();
        var items = (await multi.ReadAsync<ArticleResponseDto>()).ToList();

        return new PagedResult<ArticleResponseDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<ArticleResponseDto?> GetArticleByIdAsync(int id)
    {
        using var connection = CreateConnection();

        string sql = @"
            SELECT 
                a.ID_ARTICLE AS IdArticle,
                a.REF_ARTICLE AS RefArticle,
                a.NUM_ARTICLE AS NumArticle,
                a.DESIGNATION AS Designation,
                a.ID_FAMILLE AS IdFamille,
                f.LIBELLE_FAM_ARTICLE AS LibelleFamille,
                a.ID_FAMILLE_ARTICLE AS IdFamilleArticle,
                sf.LIBELLE_SOUS_FAMILLE AS LibelleSousFamille,
                a.PRIX_VENTE_ARTICLE_HT AS PrixVenteArticleHt,
                a.PRIX_VENTE_ARTICLE_TTC AS PrixVenteArticleTtc,
                a.PRIX_ACHAT AS PrixAchat,
                a.PRIX_ACHAT_TTC AS PrixAchatTtc,
                a.DERNIER_PRIX_ACHAT AS DernierPrixAchat,
                a.DERNIER_PRIX_VENTE AS DernierPrixVente,
                a.PRIX_PUBLIQUE AS PrixPublique
            FROM ARTICLE a
            LEFT JOIN FAMILLEARTICLE f ON a.ID_FAMILLE = f.ID_FAMILLE_ARTICLE
            LEFT JOIN SOUSFAMILLEARTICLE sf ON a.ID_FAMILLE_ARTICLE = sf.ID_SOUS_FAMILLE
            WHERE a.ID_ARTICLE = @id AND (a.SUPPRIME IS NULL OR a.SUPPRIME = 0);";

        return await connection.QueryFirstOrDefaultAsync<ArticleResponseDto>(sql, new { id });
    }
}