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
            builder.Where("(a.RefArticle LIKE '%' + @SearchTerm + '%' OR a.DESIGNATION LIKE '%' + @SearchTerm + '%')",
                new { SearchTerm = filter.SearchTerm.Trim() });
        }

        if (filter.IdFamille.HasValue) builder.Where("a.IdFamille = @IdFamille", new { filter.IdFamille });
        if (filter.IdSousFamille.HasValue) builder.Where("a.IdFamilleArticle = @IdSousFamille", new { filter.IdSousFamille });

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
                a.IdArticle AS IdArticle,
                a.RefArticle AS RefArticle,
                a.NumArticle AS NumArticle,
                a.DESIGNATION AS Designation,
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
            WHERE a.IdArticle = @id AND (a.SUPPRIME IS NULL OR a.SUPPRIME = 0);";

        return await connection.QueryFirstOrDefaultAsync<ArticleResponseDto>(sql, new { id });
    }
}