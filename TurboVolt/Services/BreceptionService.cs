using System.Data;
using Dapper;
using FluentValidation;
using Microsoft.Data.SqlClient;
using TurboVolt.DTOs;

namespace TurboVolt.Services;
public interface IBreceptionService
{
    Task<PagedResult<BreceptionResponseDto>> GetBreceptionsPagedAsync(BreceptionFilterDto filter);
    Task<BreceptionResponseDto?> GetBreceptionByIdAsync(int id);
}
public class BreceptionService : IBreceptionService
{
    private readonly string _connectionString;
    private readonly IValidator<BreceptionFilterDto> _validator;

    public BreceptionService(IConfiguration configuration, IValidator<BreceptionFilterDto> validator)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new ArgumentNullException(nameof(configuration), "Chaine de connexion manquante.");
        _validator = validator;
    }

    private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

    public async Task<PagedResult<BreceptionResponseDto>> GetBreceptionsPagedAsync(BreceptionFilterDto filter)
    {
        await _validator.ValidateAndThrowAsync(filter);

        using var connection = CreateConnection();

        var builder = new SqlBuilder();
        var selector = builder.AddTemplate(@"
            SELECT COUNT(1) FROM BRECEPTION br /**where**/;

            WITH PagedBr AS (

                SELECT 
                    br.IdBonReception AS IdBonReception,
                    br.IdFournisseur AS IdFournisseur,
                    f.NomFournisseur AS NomFournisseur,
                    br.NoBl AS NoBl,
                    br.RefBonReceptionInterne AS RefBonReceptionInterne,
                    br.RefFactureFrs AS RefFactureFrs,
                    br.Description AS Description,
                    br.DateCreation AS DateCreation,
                    br.DateModification AS DateModification,
                    br.TotalBonReceptionHt AS TotalBonReceptionHt,
                    br.TotalBonReceptionTtc AS TotalBonReceptionTtc,
                    br.Reste_A_Donner AS ResteADonner,
                    br.Iddepot AS IdDepot,
                    
                    -- User Créateur (u1) avec alias uniques
                    u1.IdUser AS IdUserCreateur, 
                    u1.NOM AS NomCreateur, 
                    u1.PRENOM AS PrenomCreateur,
                    
                    -- User Modificateur (u2) avec alias uniques
                    u2.IdUser AS IdUserModificateur, 
                    u2.NOM AS NomModificateur, 
                    u2.PRENOM AS PrenomModificateur,
                    
                    ROW_NUMBER() OVER (ORDER BY br.DateCreation DESC) AS RowNum
                FROM BRECEPTION br
                LEFT JOIN FOURNISSEUR f ON br.IdFournisseur = f.IdFournisseur
                LEFT JOIN DEPOT d ON br.Iddepot = d.IdDepot
                LEFT JOIN dbo.[USER] u1 ON br.Iduser = u1.IdUser
                LEFT JOIN dbo.[USER] u2 ON br.IduserModification = u2.IdUser
                /**where**/
            )
            SELECT * FROM PagedBr
            WHERE RowNum BETWEEN @StartRow AND @EndRow;
        ", new
        {
            StartRow = ((filter.PageNumber - 1) * filter.PageSize) + 1,
            EndRow = filter.PageNumber * filter.PageSize
        });

        // Filtre par défaut sur la suppression logique
        builder.Where("(br.Supprime IS NULL OR br.Supprime = 0)");

        if (filter.DateDebut.HasValue) builder.Where("br.DateCreation >= @DateDebut", new { filter.DateDebut });
        if (filter.DateFin.HasValue) builder.Where("br.DateCreation <= @DateFin", new { filter.DateFin });
        if (filter.IdFournisseur.HasValue) builder.Where("br.IdFournisseur = @IdFournisseur", new { filter.IdFournisseur });
        if (filter.IdDepot.HasValue) builder.Where("br.Iddepot = @IdDepot", new { filter.IdDepot });
        if (filter.IdUser.HasValue) builder.Where("(br.Iduser = @IdUser OR br.IduserModification = @IdUser)", new { filter.IdUser });

        if (!string.IsNullOrEmpty(filter.RefArticle) || filter.IdFamille.HasValue)
        {
            builder.Where(@"EXISTS (
                SELECT 1 FROM BRECEPTION_X_ARTICLE l
                INNER JOIN ARTICLE a ON l.IdArticle = a.IdArticle
                WHERE l.IdBonReception = br.IdBonReception
                AND (@RefArticle IS NULL OR a.RefArticle LIKE '%' + @RefArticle + '%')
                AND (@IdFamille IS NULL OR a.IdFamille = @IdFamille)
            )", new { filter.RefArticle, filter.IdFamille });
        }

        using var multi = await connection.QueryMultipleAsync(selector.RawSql, selector.Parameters);
        int totalCount = await multi.ReadFirstAsync<int>();

        var brList = multi.Read<BreceptionResponseDto, UserResponseDto, UserResponseDto, BreceptionResponseDto>(
            (br, createur, modificateur) =>
            {
                br.AgentCreateur = createur;
                br.AgentModificateur = modificateur;
                return br;
            },
            splitOn: "IdUserCreateur,IdUserModificateur"
        ).ToList();

        // Chargement différé des lignes d'articles pour les éléments affichés
        if (brList.Any())
        {
            var brIds = brList.Select(x => x.IdBonReception).ToList();
            string linesSql = @"
            SELECT 
                l.Id AS Id,
                l.IdBonReception AS IdBonReception,
                l.IdArticle AS IdArticle,
                a.RefArticle AS RefArticle,
                a.DESIGNATION AS Designation,
                f.LibelleFamArticle AS LibelleFamille,
                sf.LibelleSousFamille AS LibelleSousFamille,
                l.QuantiteReceve AS QuantiteReceve,
                l.PrixBonReception AS PrixBonReception,
                l.TauxTva AS TauxTva,
                l.RemiseFournisseur AS RemiseFournisseur,
                l.MontantHt AS MontantHt,
                l.MontantTtc AS MontantTtc,
                l.PriceIsTtc AS PriceIsTtc,
                l.IdDepot AS IdDepot,
                l.IndexOrder AS IndexOrder
            FROM BRECEPTION_X_ARTICLE l
            INNER JOIN ARTICLE a ON l.IdArticle = a.IdArticle
            LEFT JOIN FAMILLEARTICLE f ON a.IdFamille = f.IdFamilleArticle
            LEFT JOIN SOUSFAMILLEARTICLE sf ON a.IdFamilleArticle = sf.IdSousFamille
            WHERE l.IdBonReception IN @brIds AND (l.Supprime IS NULL OR l.Supprime = 0)
            ORDER BY l.IndexOrder ASC;";

            var lines = (await connection.QueryAsync<BreceptionXArticleResponseDto>(linesSql, new { brIds })).ToList();

            foreach (var br in brList)
            {
                br.LignesArticles = lines.Where(x => x.IdBonReception == br.IdBonReception).ToList();
            }
        }

        return new PagedResult<BreceptionResponseDto>
        {
            Items = brList,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<BreceptionResponseDto?> GetBreceptionByIdAsync(int id)
    {
        using var connection = CreateConnection();

        string sql = @"
        SELECT 
            br.IdBonReception AS IdBonReception,
            br.IdCommandeFournisseur AS IdCommandeFournisseur,
            br.IdFournisseur AS IdFournisseur,
            f.NomFournisseur AS NomFournisseur,
            br.NoBl AS NoBl,
            br.RefBonReceptionInterne AS RefBonReceptionInterne,
            br.RefFactureFrs AS RefFactureFrs,
            br.Description AS Description,
            br.DateCreation AS DateCreation,
            br.DateModification AS DateModification,
            br.TotalBonReceptionHt AS TotalBonReceptionHt,
            br.TotalBonReceptionTtc AS TotalBonReceptionTtc,
            br.Reste_A_Donner AS ResteADonner,
            br.Iddepot AS IdDepot,
            d.NomDepot AS NomDepot,
            
            u1.IdUser AS IdUserCreateur, u1.NOM AS NomCreateur, u1.PRENOM AS PrenomCreateur,
            u2.IdUser AS IdUserModificateur, u2.NOM AS NomModificateur, u2.PRENOM AS PrenomModificateur
        FROM BRECEPTION br
        LEFT JOIN FOURNISSEUR f ON br.IdFournisseur = f.IdFournisseur
        LEFT JOIN DEPOT d ON br.Iddepot = d.IdDepot
        LEFT JOIN dbo.[USER] u1 ON br.Iduser = u1.IdUser
        LEFT JOIN dbo.[USER] u2 ON br.IduserModification = u2.IdUser
        WHERE br.IdBonReception = @id AND (br.Supprime IS NULL OR br.Supprime = 0);

        SELECT 
            l.Id AS Id,
            l.IdBonReception AS IdBonReception,
            l.IdArticle AS IdArticle,
            a.RefArticle AS RefArticle,
            a.DESIGNATION AS Designation,
            f.LibelleFamArticle AS LibelleFamille,
            sf.LibelleSousFamille AS LibelleSousFamille,
            l.QuantiteReceve AS QuantiteReceve,
            l.PrixBonReception AS PrixBonReception,
            l.TauxTva AS TauxTva,
            l.RemiseFournisseur AS RemiseFournisseur,
            l.MontantHt AS MontantHt,
            l.MontantTtc AS MontantTtc,
            l.PriceIsTtc AS PriceIsTtc,
            l.IdDepot AS IdDepot,
            l.IndexOrder AS IndexOrder
        FROM BRECEPTION_X_ARTICLE l
        INNER JOIN ARTICLE a ON l.IdArticle = a.IdArticle
        LEFT JOIN FAMILLEARTICLE f ON a.IdFamille = f.IdFamilleArticle
        LEFT JOIN SOUSFAMILLEARTICLE sf ON a.IdFamilleArticle = sf.IdSousFamille
        WHERE l.IdBonReception = @id AND (l.Supprime IS NULL OR l.Supprime = 0)
        ORDER BY l.IndexOrder ASC;";

        using var multi = await connection.QueryMultipleAsync(sql, new { id });

        var br = multi.Read<BreceptionResponseDto, UserResponseDto, UserResponseDto, BreceptionResponseDto>(
            (b, createur, modificateur) =>
            {
                b.AgentCreateur = createur;
                b.AgentModificateur = modificateur;
                return b;
            },
            splitOn: "IdUserCreateur,IdUserModificateur"
        ).FirstOrDefault();

        if (br != null)
        {
            br.LignesArticles = (await multi.ReadAsync<BreceptionXArticleResponseDto>()).ToList();
        }

        return br;
    }
}