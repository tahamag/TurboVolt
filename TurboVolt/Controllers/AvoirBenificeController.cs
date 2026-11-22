using Microsoft.AspNetCore.Mvc;
using TurboVolt.DTOs;
using TurboVolt.Services;

namespace TurboVolt.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AvoirBenificeController : ControllerBase
    {
        private readonly IAvoirBenificeService _avoirBenificeService;

        public AvoirBenificeController(IAvoirBenificeService avoirBenificeService)
        {
            _avoirBenificeService = avoirBenificeService;
        }

        // GET: api/AvoirBenifice?userId=1&articleId=42&articleSearch=REF01&dateDebut=2026-01-01&dateFin=2026-11-19
        [HttpGet]
        public async Task<ActionResult<List<AvoirBenificeDetailDto>>> GetAvoirBenifices(
            [FromQuery] int? userId = null,
            [FromQuery] int? articleId = null,
            [FromQuery] string? articleSearch = null,
            [FromQuery] DateTime? dateDebut = null,
            [FromQuery] DateTime? dateFin = null)
        {
            var result = await _avoirBenificeService.GetAvoirBenificesAsync(
                userId,
                articleId,
                articleSearch,
                dateDebut,
                dateFin);

            return Ok(result);
        }

        // GET: api/AvoirBenifice/by-user?userId=1&articleId=42&articleSearch=REF01&dateDebut=2026-01-01&dateFin=2026-11-19
        [HttpGet("by-user")]
        public async Task<ActionResult<List<AvoirBenificeByUserDto>>> GetAvoirBenificeByUser(
            [FromQuery] int? userId = null,
            [FromQuery] int? articleId = null,
            [FromQuery] string? articleSearch = null,
            [FromQuery] DateTime? dateDebut = null,
            [FromQuery] DateTime? dateFin = null)
        {
            var result = await _avoirBenificeService.GetAvoirBenificeByUserAsync(
                userId,
                articleId,
                articleSearch,
                dateDebut,
                dateFin);

            return Ok(result);
        }

        // GET: api/AvoirBenifice/difference-by-user?userId=1&articleId=42&articleSearch=REF01&dateDebut=2026-01-01&dateFin=2026-11-22
        [HttpGet("difference-by-user")]
        public async Task<ActionResult<List<BenificeDifferenceByUserDto>>> GetBenificeDifferenceByUser(
        [FromQuery] int? userId = null,
        [FromQuery] int? articleId = null,
        [FromQuery] string? articleSearch = null,
        [FromQuery] DateTime? dateDebut = null,
        [FromQuery] DateTime? dateFin = null)
        {
            var result = await _avoirBenificeService.GetBenificeDifferenceByUserAsync(
                userId,
                articleId,
                articleSearch,
                dateDebut,
                dateFin);

            return Ok(result);
        }
    }
}