using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Models;
using Demo.Services.ChargeStationService;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChargeStationController : ControllerBase
    {
        private readonly IChargeStationService _chargeStationService;

        public ChargeStationController(IChargeStationService chargeStationService)
        {
            _chargeStationService = chargeStationService;
        }

        [HttpGet]
        public Task<IEnumerable<ChargeStation>> GetAllChargeStations()
        {
            return _chargeStationService.GetAllChargeStations();
        }

        [HttpGet("GetChargeStationById/{id}")]
        public ActionResult<ChargeStation> GetChargeStationById(int id)
        {
            var chargeStation = _chargeStationService.GetChargeStationById(id);
            if (chargeStation != null)
                return chargeStation;
            return NotFound();
        }

        [HttpPut]
        public async Task<ActionResult<ChargeStation>> PutChargeStation(ChargeStation chargeStation)
        {
            var updatedChargeRequest = await _chargeStationService.UpdateChargeStation(chargeStation);
            if (updatedChargeRequest.StatusCode == 200)
                return CreatedAtAction("GetChargeStation", new { id = updatedChargeRequest.Data.ChargeStationId },
                    updatedChargeRequest.Data);
            return BadRequest(updatedChargeRequest.Message);
        }

        [HttpPost]
        public async Task<ActionResult<ChargeStation>> PostChargeStation(int groupId, ChargeStation chargeStation)
        {
            var createdChargeStation = await _chargeStationService.AddChargeStationToGroup(groupId, chargeStation);
            if (createdChargeStation != null)
                return CreatedAtAction("GetChargeStation", new { id = createdChargeStation.ChargeStationId },
                    createdChargeStation);
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ChargeStation>> DeleteChargeStation(int id)
        {
            var deletedChargeStation = await _chargeStationService.DeleteChargeStation(id);
            if (deletedChargeStation != null)
                return Ok(deletedChargeStation);
            return BadRequest();
        }
    }
}