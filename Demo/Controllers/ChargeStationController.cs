using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Demo.Models;
using Demo.Services.ChargeStationService;

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
        public async Task<ActionResult<IEnumerable<ChargeStation>>> GetChargeStation()
        {
            return await _chargeStationService.GetAllChargeStations();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ChargeStation>> GetChargeStation(int id)
        {
            return await _chargeStationService.GetChargeStationById(id);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ChargeStation>> PutChargeStation(int id, ChargeStation chargeStation)
        {
            var updatedChargeRequest = await _chargeStationService.UpdateChargeStation(id, chargeStation);
            if(updatedChargeRequest != null)
            return CreatedAtAction("GetChargeStation", new { id = updatedChargeRequest.ChargeStationId }, updatedChargeRequest);
            else
                return BadRequest();
        }

        [HttpPost]
        public async Task<ActionResult<ChargeStation>> PostChargeStation(int groupId, ChargeStation chargeStation)
        {
            var createdChargeStation = await _chargeStationService.AddChargeStationToGroup(groupId, chargeStation);
            if (createdChargeStation != null)
                return CreatedAtAction("GetChargeStation", new { id = createdChargeStation.ChargeStationId }, createdChargeStation);
            else
                return BadRequest();
        }

        
        [HttpDelete("{id}")]
        public async Task<ActionResult<ChargeStation>> DeleteChargeStation(int id)
        {
            var deletedChargeStation = await _chargeStationService.DeleteChargeStation(id);
            if (deletedChargeStation != null)
                return CreatedAtAction("GetChargeStation", new { id = deletedChargeStation.ChargeStationId }, deletedChargeStation);
            else
                return BadRequest();
        }
    }
}
