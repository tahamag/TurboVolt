using Microsoft.AspNetCore.Mvc;
using TurboVolt.DTOs;
using TurboVolt.Services;

namespace TurboVolt.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Authentifie un utilisateur et retourne un jeton JWT.
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var response = await _authService.AuthenticateAsync(request);

        if (response == null)
            return Unauthorized(new { message = "Nom d'utilisateur ou mot de passe incorrect." });

        return Ok(response);
    }
}