using api.BusinessLogic;
using Microsoft.AspNetCore.Mvc;
using System;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ConfigController : ControllerBase
    {
        private readonly BllModeManager _bllModeManager;

        public ConfigController(BllModeManager bllModeManager)
        {
            _bllModeManager = bllModeManager;
        }

        [HttpPost("bll-mode/{mode}")]
        public IActionResult SetBllMode(string mode)
        {
            if (Enum.TryParse<BllMode>(mode, true, out var bllMode))
            {
                _bllModeManager.SetBllMode(bllMode);
                return Ok($"BLL mode set to: {_bllModeManager.CurrentBllMode}");
            }
            return BadRequest($"Invalid BLL mode: {mode}. Accepted values are 'Linq' or 'Sproc'.");
        }
    }
}
