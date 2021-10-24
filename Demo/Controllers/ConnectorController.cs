using System.Collections.Generic;
using Demo.Models;
using Demo.Services.ConnectorService;
using Microsoft.AspNetCore.Mvc;

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
        public IAsyncEnumerable<Connector> GetConnector()
        {
            return _connectorService.GetAllConnectors();
        }

        [HttpGet("GetConnectorById/{connectorId}/{chargeStationId}")]
        public ActionResult<Connector> GetConnectorById(int connectorId, int chargeStationId)
        {
            var connector = _connectorService.GetConnectorById(connectorId, chargeStationId);

            if (connector == null) return NotFound();
            return connector;
        }

        [HttpGet("GetConnectorsOfChargeStation/{chargeStationId}")]
        public ActionResult<List<Connector>> GetConnectorsOfChargeStation(int chargeStationId)
        {
            var connectors = _connectorService.GetConnectorsOfChargeStation(chargeStationId);
            if (connectors == null) return NotFound();
            return connectors;
        }

        [HttpPut]
        public IActionResult PutConnector(Connector connector)
        {
            var conn = _connectorService.UpdateCurrent(connector.ConnectorId, connector.ChargeStationId,
                connector.MaxCurrent);
            if (conn.Result.StatusCode == 400)
                return BadRequest(conn.Result.Message);
            if (conn.Result.StatusCode == 404)
                return NotFound(conn.Result.Message);
            return CreatedAtAction("GetConnector", new { id = conn.Result.Data.ConnectorId }, conn);
        }

        [HttpPost]
        public IActionResult PostConnector(int chargeStationId, Connector connector)
        {
            var conn = _connectorService.AddConnectorToChargeStation(chargeStationId, connector);
            if (conn.Result.Data == null)
                return BadRequest(conn.Result.Message);
            return CreatedAtAction("GetConnector", new { id = conn.Result.Data.ConnectorId }, conn.Result.Data);
        }

        [HttpDelete]
        public IActionResult DeleteConnector(int connectorId, int chargeStationId)
        {
            var conn = _connectorService.DeleteConnector(connectorId, chargeStationId);
            if (conn.Result == null) return BadRequest();
            return Ok(conn.Result);
        }
    }
}