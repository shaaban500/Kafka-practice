using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Producer.API.Services;

namespace Producer.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProducerController : ControllerBase
    {
        private readonly ProducerService _producerService;
        private readonly ILogger<ProducerController> _logger;

        public ProducerController(ProducerService producerService, ILogger<ProducerController> logger)
        {
            _producerService = producerService;
            _logger = logger;
        }

        [HttpGet("event-producing")]
        public async Task<IActionResult> Produce(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Producing event to Kafka.");
            try
            {
                await _producerService.ProduceAsync(cancellationToken);
                return Ok("Event produced successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error producing event: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
        