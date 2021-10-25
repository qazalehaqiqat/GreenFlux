using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Models;

namespace Demo.Services.ChargeStationService
{
    public interface IChargeStationService
    {
        public ChargeStation GetChargeStationById(int id);
        public Task<IEnumerable<ChargeStation>> GetAllChargeStations();
        public Task<ChargeStation> AddChargeStationToGroup(int groupId, ChargeStation chargeStation);
        public Task<APIResponse<ChargeStation>> UpdateChargeStation(ChargeStation chargeStation);
        public Task<ChargeStation> DeleteChargeStation(int id);
    }
}