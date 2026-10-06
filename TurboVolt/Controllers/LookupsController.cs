using Microsoft.AspNetCore.Mvc;
using TurboVolt.DTOs;
using TurboVolt.Services;

namespace TurboVolt.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LookupsController : ControllerBase
{
    private readonly ILookupService _lookupService;

    public LookupsController(ILookupService lookupService)
    {
        _lookupService = lookupService;
    }

    /// <summary>
    /// Récupère la liste d'aide au choix (lookup) pour les clients.
    /// </summary>
    [HttpGet("clients")]
    [ProducesResponseType(typeof(List<ClientResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ClientResponseDto>>> GetClients()
    {
        var result = await _lookupService.GetClientsLookupAsync();
        return Ok(result);
    }

    /// <summary>
    /// Récupère la liste d'aide au choix (lookup) pour les utilisateurs.
    /// </summary>
    [HttpGet("users")]
    [ProducesResponseType(typeof(List<UserResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<UserResponseDto>>> GetUsers()
    {
        var result = await _lookupService.GetUsersLookupAsync();
        return Ok(result);
    }

    /// <summary>
    /// Récupère la liste d'aide au choix (lookup) pour les familles d'articles.
    /// </summary>
    [HttpGet("familles")]
    [ProducesResponseType(typeof(List<FamilleArticleResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<FamilleArticleResponseDto>>> GetFamilles()
    {
        var result = await _lookupService.GetFamillesLookupAsync();
        return Ok(result);
    }

    /// <summary>
    /// Récupère la liste des sous-familles d'articles (optionnellement filtrée par famille).
    /// </summary>
    [HttpGet("sous-familles")]
    [ProducesResponseType(typeof(List<SousFamilleArticleResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<SousFamilleArticleResponseDto>>> GetSousFamilles([FromQuery] int? idFamille)
    {
        var result = await _lookupService.GetSousFamillesLookupAsync(idFamille);
        return Ok(result);
    }
}