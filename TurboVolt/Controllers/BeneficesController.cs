using Microsoft.AspNetCore.Mvc;
using TurboVolt.DTOs;
using TurboVolt.Services;

namespace TurboVolt.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BeneficesController : ControllerBase
    {
        private readonly IBeneficeService _beneficeService;

        public BeneficesController(IBeneficeService beneficeService)
        {
            _beneficeService = beneficeService;
        }

        /// <summary>
        /// Récupère la liste des bénéfices par vente avec filtres optionnels par utilisateur et par période.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<BeneficeResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<BeneficeResponseDto>>> GetBenefices(
            [FromQuery] int? idUser,
            [FromQuery] DateTime? dateDebut,
            [FromQuery] DateTime? dateFin)
        {
            var result = await _beneficeService.GetBeneficesAsync(idUser, dateDebut, dateFin);
            return Ok(result);
        }
    }
}