using Microsoft.AspNetCore.Mvc;
using TurboVolt.DTOs;
using TurboVolt.Services;

namespace TurboVolt.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BreceptionsController : ControllerBase
{
    private readonly IBreceptionService _breceptionService;

    public BreceptionsController(IBreceptionService breceptionService)
    {
        _breceptionService = breceptionService;
    }

    /// <summary>
    /// Récupère la liste paginée et filtrée des bons de réception.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<BreceptionResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<BreceptionResponseDto>>> GetBreceptions([FromQuery] BreceptionFilterDto filter)
    {
        var result = await _breceptionService.GetBreceptionsPagedAsync(filter);
        return Ok(result);
    }

    /// <summary>
    /// Récupère un bon de réception avec ses détails et ses lignes d'articles par son ID.
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(BreceptionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BreceptionResponseDto>> GetBreceptionById(int id)
    {
        var breception = await _breceptionService.GetBreceptionByIdAsync(id);

        if (breception == null)
            return NotFound(new { message = $"Le bon de réception N° {id} n'a pas été trouvé." });

        return Ok(breception);
    }
}