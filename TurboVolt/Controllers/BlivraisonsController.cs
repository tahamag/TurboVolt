using Microsoft.AspNetCore.Mvc;
using TurboVolt.DTOs;
using TurboVolt.Services;

namespace TurboVolt.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlivraisonsController : ControllerBase
{
    private readonly IBlivraisonService _blivraisonService;

    public BlivraisonsController(IBlivraisonService blivraisonService)
    {
        _blivraisonService = blivraisonService;
    }

    /// <summary>
    /// Récupère la liste paginée des bons de livraison selon les filtres fournis.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<BlivraisonResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<BlivraisonResponseDto>>> GetBlivraisons([FromQuery] BlivraisonFilterDto filter)
    {
        var result = await _blivraisonService.GetBlivraisonsPagedAsync(filter);
        return Ok(result);
    }

    /// <summary>
    /// Récupère un bon de livraison avec ses détails et ses lignes d'articles.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BlivraisonResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BlivraisonResponseDto>> GetBlivraisonById(int id)
    {
        var blivraison = await _blivraisonService.GetBlivraisonByIdAsync(id);

        if (blivraison == null)
            return NotFound(new { message = $"Le bon de livraison N° {id} n'a pas été trouvé." });

        return Ok(blivraison);
    }
}