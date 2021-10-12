using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.Services.ConnectorService
{
    public class ConnectorService : IConnectorService
    {
        private readonly DemoContext _context;
        public ConnectorService(DemoContext context)
        {
            _context = context;
        }

        public async Task<APIResponse<Connector>> AddConnectorToChargeStation(int chargeStationId, Connector connector)
        {
            try
            {
                var groupId = _context.ChargeStation.Find(chargeStationId).GroupId;
                var groupCapacity = _context.Group.Find(groupId).Capacity;

                var chargeStation = _context.ChargeStation.Include(c => c.Connectors).FirstOrDefault(c => c.ChargeStationId == chargeStationId);
                if (!chargeStation.Connectors.Any(c => c.ConnectorId == connector.ConnectorId))
                {
                    Connector c = _context.Connector.FirstOrDefault(c => c.ConnectorId == connector.ConnectorId && c.ChargeStationId == chargeStationId);

                    if (c != null)
                    {
                        if ((UsedCapacityGroup(groupId) + c.MaxCurrent) > groupCapacity)
                        {
                            throw new Exception("You can't add connector because group capacity is not enough.");
                        }
                        chargeStation.Connectors.Add(c);
                    }
                    else
                    {
                        if ((UsedCapacityGroup(groupId) + connector.MaxCurrent) > groupCapacity)
                        {
                            throw new Exception("You can't add connector because group capacity is not enough.");
                        }
                        _context.Connector.Add(connector);
                        chargeStation.Connectors.Add(connector);
                    }
                }
                else
                {
                    throw new Exception("You've already added this connector. Please try again!");
                }
                await _context.SaveChangesAsync();
                return new APIResponse<Connector> { Data = connector, Succeeded = true, Message = "Connector just added to charge station sucessfully." };
            }
            catch (Exception ex)
            {
                return new APIResponse<Connector> { Data = null, Succeeded = false, Message = ex.Message };
            }
        }

        public async Task<Connector> DeleteConnector(int connectorId, int chargeStationId)
        {
            var connector = await _context.Connector.FirstOrDefaultAsync(c => c.ConnectorId == connectorId && c.ChargeStationId == chargeStationId);

            try
            {
                if (connector != null)
                {
                    _context.Connector.Remove(connector);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Not found!");
                 }
            }
            catch (Exception ex)
            {
                return null;
            }

            return connector;
        }

        public async Task<IEnumerable<Connector>> GetAllConnectors()
        {
            return await _context.Connector.ToListAsync();
        }

        public async Task<Connector> GetConnectorById(int connectorId, int chargeStationId)
        {
            var connector = await _context.Connector.FirstOrDefaultAsync(p => p.ConnectorId == connectorId && p.ChargeStationId == chargeStationId);

            return connector;
        }

        public async Task<APIResponse<Connector>> UpdateCurrent(int connectorId, int chargeStationId, double maxCurrent)
        {
            try
            {
                var connector = await _context.Connector.FirstOrDefaultAsync(c => c.ConnectorId == connectorId && c.ChargeStationId == chargeStationId);
                if (connector == null)
                {
                    throw new Exception("Not Found");
                }
                else
                {
                    var current = connector.MaxCurrent;

                    var groupId = _context.ChargeStation.Find(chargeStationId).GroupId;
                    var groupCapacity = _context.Group.Find(groupId).Capacity;

                    if ((UsedCapacityGroup(groupId) - current) + maxCurrent > groupCapacity)
                    {
                        throw new Exception("You can't update connector's max current because group capacity is not enough.");
                    }

                    var station = _context.ChargeStation.Where(c => c.Connectors.Any(s => s.ConnectorId == connectorId && s.ChargeStationId == chargeStationId)).ToList().GroupBy(c => c.GroupId);
                    connector.MaxCurrent = maxCurrent;
                    await _context.SaveChangesAsync();
                    return new APIResponse<Connector> {Data =  connector, Succeeded=true} ;
                }
            }
            catch (Exception ex)
            {
                return new APIResponse<Connector> { Data = null, Message = ex.Message, Succeeded = false };
            }
        }

        private double UsedCapacityGroup(int groupId)
        {
            return _context.ChargeStation.Where(c => c.GroupId == groupId).Include(c => c.Connectors).SelectMany(c => c.Connectors).Sum(c => c.MaxCurrent);
        }
    }
}
