using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Models;

namespace Demo.Services.ConnectorService
{
    public interface IConnectorService
    {
        public Connector GetConnectorById(int connectorId, int chargeStationId);
        public IAsyncEnumerable<Connector> GetAllConnectors();
        public Task<APIResponse<Connector>> AddConnectorToChargeStation(int chargeStationId, Connector connector);
        public Task<APIResponse<Connector>> UpdateCurrent(int connectorId, int chargeStationId, double maxCurrent);
        public Task<Connector> DeleteConnector(int connectorId, int ChargeStationId);
        public List<Connector> GetConnectorsOfChargeStation(int chargeStationId);
    }
}