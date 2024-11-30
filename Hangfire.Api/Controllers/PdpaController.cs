using Hangfire.Services.Interface;
using Microsoft.AspNetCore.Mvc;

namespace Hangfire.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PdpaController : ControllerBase
    {
        private readonly IPdpaService _pdpaService;
        private readonly ILogger<PdpaController> _logger;

        public PdpaController(IPdpaService pdpaService, ILogger<PdpaController> logger)
        {
            _pdpaService = pdpaService;
            _logger = logger;
        }

        /// <summary>
        /// API UPD-PDPA
        /// </summary>
        /// <remarks>
        /// - Download UPD file
        /// - Import UPD to temp account
        /// </remarks>
        [HttpGet("UpdProcess")]
        public IActionResult UpdProcess()
        {
            BackgroundJob.Enqueue<IPdpaService>(x => x.UpdPdpaProcess());
            return Ok("Import account from UPD file successfully.");
        }

        /// <summary>
        /// API DLD-PDPA
        /// </summary>
        /// <remarks>
        /// - Download DlD file
        /// - Import DLD to temp account
        /// - Cleansing pdpa account
        /// - Upload file log to DWH
        /// </remarks>
        [HttpGet("DldProcess")]
        public IActionResult DldProcess()
        {
            BackgroundJob.Enqueue<IPdpaService>(x => x.DldPdpaProcess());
            return Ok("Import account from DLD file and delete account successfully.");
        }


        /// <summary>
        /// API DLD-PDPA Manual
        /// </summary>
        /// <remarks>
        /// Cleansing account pdpa all table
        /// </remarks>
        [HttpGet("DldProcessManual")]
        public IActionResult DldProcessManual(string date = "dd/MM/yyyy")
        {
            BackgroundJob.Enqueue<IPdpaService>(x => x.DldPdpaProcessManual(date));
            return Ok("Manual import account from DLD file and delete account successfully.");
        }

        /// <summary>
        /// API DLD-PDPA Manual Initialize
        /// </summary>
        /// <remarks>
        /// Cleansing account pdpa all table
        /// File DLD format : DLD_STATUS_PG_yyyyMMdd
        /// </remarks>
        [HttpGet("DldProcessManualInitialize")]
        public IActionResult DldPdpaProcessManualInitialize(string date = "dd/MM/yyyy")
        {
            BackgroundJob.Enqueue<IPdpaService>(x => x.DldPdpaProcessManualInitialize(date));
            return Ok("Manual import account from DLD file and delete account successfully.");
        }
    }
}
