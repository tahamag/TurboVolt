using Microsoft.AspNetCore.Mvc;
using TurboVolt.DTOs;
using TurboVolt.Services;

namespace TurboVolt.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArticlesController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticlesController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    /// <summary>
    /// Récupère la liste paginée et filtrée des articles.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ArticleResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<ArticleResponseDto>>> GetArticles([FromQuery] ArticleFilterDto filter)
    {
        var result = await _articleService.GetArticlesPagedAsync(filter);
        return Ok(result);
    }

    /// <summary>
    /// Récupère un article par son identifiant unique.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ArticleResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ArticleResponseDto>> GetArticleById(int id)
    {
        var article = await _articleService.GetArticleByIdAsync(id);

        if (article == null)
            return NotFound(new { message = $"L'article avec l'identifiant {id} n'a pas été trouvé." });

        return Ok(article);
    }
}