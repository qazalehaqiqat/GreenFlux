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

        public async Task<ChargeStation> DeleteChargeStation(int id)
        {
            var chargeStation = await _context.ChargeStation.FindAsync(id);
            if (chargeStation == null) return null;

            _context.ChargeStation.Remove(chargeStation);
            await _context.SaveChangesAsync();

            return chargeStation;
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

        public async Task<ChargeStation> UpdateChargeStation(int id, ChargeStation chargeStation)
        {
            if (id != chargeStation.ChargeStationId) return null;
            if (chargeStation.Connectors != null)
            {
                var current = _context.ChargeStation.FindAsync(id).Result.Connectors.Sum(c => c.MaxCurrent);
            }

            UsedCapacityGroup(chargeStation.GroupId);
            _context.Entry(chargeStation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return chargeStation;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.ChargeStation.Any(c => c.ChargeStationId == id))
                    return null;
                throw;
            }
        }

        private double UsedCapacityGroup(int groupId)
        {
            return _context.ChargeStation.Where(c => c.GroupId == groupId).Include(c => c.Connectors)
                .SelectMany(c => c.Connectors).Sum(c => c.MaxCurrent);
        }
    }
}