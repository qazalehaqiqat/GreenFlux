using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.Models;
using Microsoft.EntityFrameworkCore;

namespace Demo.Services.ChargeStationService
{
    public class ChargeStationService : IChargeStationService
    {
        private readonly DemoContext _context;

        public ChargeStationService(DemoContext context)
        {
            _context = context;
        }

        public async Task<ChargeStation> AddChargeStationToGroup(int groupId, ChargeStation chargeStation)
        {
            if (!_context.Group.Include(c => c.ChargeStations).SelectMany(c => c.ChargeStations)
                .Select(c => c.ChargeStationId).Contains(chargeStation.ChargeStationId))
            {
                _context.ChargeStation.Add(chargeStation);
                await _context.SaveChangesAsync();
                return chargeStation;
            }

            return null;
        }

        public async Task<APIResponse<ChargeStation>> DeleteChargeStation(int id)
        {
            try
            {
                var chargeStation = await _context.ChargeStation.FindAsync(id);
                if (chargeStation == null) return null;

                _context.ChargeStation.Remove(chargeStation);
                await _context.SaveChangesAsync();

                return new APIResponse<ChargeStation> { Data = chargeStation, StatusCode = 200, Message = "ChargeStation deleted successfully."};
            }
            catch(Exception ex)
            {
                return new APIResponse<ChargeStation> { Data = null, StatusCode = 500, Message = ex.Message };
            }
            
        }

        public async Task<IEnumerable<ChargeStation>> GetAllChargeStations()
        {
            return await _context.ChargeStation.Include(c => c.Connectors).ToListAsync();
        }

        public ChargeStation GetChargeStationById(int id)
        {
            var chargeStation = _context.ChargeStation.Include(c => c.Connectors)
                .FirstOrDefault(c => c.ChargeStationId == id);

            if (chargeStation == null) return null;
            return chargeStation;
        }

        public async Task<APIResponse<ChargeStation>> UpdateChargeStation(ChargeStation chargeStation)
        {
            var sumCurrentUpdatedConnectors = chargeStation.Connectors.Sum(c => c.MaxCurrent);
            if (!_context.ChargeStation.Any(c => c.ChargeStationId == chargeStation.ChargeStationId))
                return new APIResponse<ChargeStation>
                    { Data = null, StatusCode = 404, Message = "ChargeStation not found!" };

            var used = UsedCapacityGroup(chargeStation.GroupId);
            var cap = _context.Group.Find(chargeStation.GroupId).Capacity;
            if (used + sumCurrentUpdatedConnectors > cap)
                return new APIResponse<ChargeStation>
                {
                    Data = null, StatusCode = 400,
                    Message = "You can't update chargeStation because group capacity is not enough."
                };
            try
            {
                _context.Entry(chargeStation).State = EntityState.Modified;
                foreach (var conn in _context.Connector.Where(c => c.ChargeStationId == chargeStation.ChargeStationId))
                    _context.Connector.Remove(conn);
                _context.SaveChanges();

                foreach (var conn in chargeStation.Connectors) _context.Connector.Add(conn);
                await _context.SaveChangesAsync();
                return new APIResponse<ChargeStation>
                    { Data = chargeStation, StatusCode = 200, Message = "ChargeStation updated successfully" };
            }
            catch (Exception ex)
            {
                return new APIResponse<ChargeStation> { Data = null, StatusCode = 500, Message = ex.Message };
            }
        }

        private double UsedCapacityGroup(int groupId)
        {
            return _context.ChargeStation.Where(c => c.GroupId == groupId).Include(c => c.Connectors)
                .SelectMany(c => c.Connectors).Sum(c => c.MaxCurrent);
        }
    }
}