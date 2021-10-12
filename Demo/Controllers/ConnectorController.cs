using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Demo.Models;
using Demo.Services.ConnectorService;

namespace Demo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConnectorController : ControllerBase
    {
        private readonly IConnectorService _connectorService;

        public ConnectorController(IConnectorService connectorService)
        {
            _connectorService = connectorService;
        }

        [HttpGet]
        public async Task<IActionResult> GetConnector()
        {
            return Ok(await _connectorService.GetAllConnectors());
        }
        [HttpGet("{connectorId}/{chargeStationId}")]
        public async Task<ActionResult<Connector>> GetConnector(int connectorId, int chargeStationId)
        {
            var connector = await _connectorService.GetConnectorById(connectorId, chargeStationId);

            if (connector == null)
            {
                return NotFound();
            }

            return Ok(connector);
        }
        [HttpPut]
        public async Task<ActionResult<Connector>> PutConnector(Connector connector)
        {
            var conn = await _connectorService.UpdateCurrent(connector.ConnectorId, connector.ChargeStationId, connector.MaxCurrent);
            if (!conn.Succeeded) return BadRequest(conn.Message);
            return CreatedAtAction("GetConnector", new { id = conn.Data.ConnectorId }, conn);
        }

        [HttpPost]
        public async Task<ActionResult<Connector>> PostConnector(int chargeStationId,Connector connector)
        {
            var conn = await _connectorService.AddConnectorToChargeStation(chargeStationId, connector);
            if (conn.Data == null)
                return BadRequest();
            else
                return CreatedAtAction("GetConnector", new { id = conn.Data.ConnectorId }, conn.Data);
        }
        [HttpDelete]
        public async Task<ActionResult<Connector>> DeleteConnector(int connectorId, int chargeStationId)
        {
            var conn = await _connectorService.DeleteConnector(connectorId, chargeStationId);
            if (conn == null)
            {
                return BadRequest();
            }
            return conn;
        }
    }
}
