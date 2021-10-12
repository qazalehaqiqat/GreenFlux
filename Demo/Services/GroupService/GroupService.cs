using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.Models;
using Demo.Services.ChargeStationService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.Services.GroupService
{
    public class GroupService : IGroupService
    {
        private readonly DemoContext _context;
        private readonly IChargeStationService _chargeStationService;
        public GroupService(DemoContext context, IChargeStationService chargeStationService)
        {
            _context = context;
            _chargeStationService = chargeStationService;
        }

        public async Task<Group> CreateGroup(Group group)
        {
            _context.Group.Add(group);
            await _context.SaveChangesAsync();
            return group;
        }

        public async Task<APIResponse<Group>> DeleteGroup(int id)
        {
            try
            {
                var group = await _context.Group.Include(c => c.ChargeStations).FirstOrDefaultAsync(c => c.Id == id);
                if (group == null)
                {
                    throw new Exception("Not Found");
                }
                foreach (var station in group.ChargeStations)
                {
                    await _chargeStationService.DeleteChargeStation(station.ChargeStationId);
                }
                _context.Group.Remove(@group);
                await _context.SaveChangesAsync();
                return new APIResponse<Group> { Data = group, Succeeded = true, Message = "Group deleted successfully", StatusCode = 200 };
            }
            catch(Exception ex)
            {
                return new APIResponse<Group> { Data = null, Succeeded = false, Message = ex.Message, StatusCode = 400 };
            }
            
        }

        public List<Group> GetAllGroups()
        {
            return _context.Group.Include(c => c.ChargeStations).ToList();
        }

        public async Task<Group> GetGroupById(int id)
        {
            var group =  await _context.Group.Include(c => c.ChargeStations).FirstOrDefaultAsync(c => c.Id == id);
            return group;
        }

        public async Task<APIResponse<Group>> UpdateGroup(int groupId, Group group)
        {
            try
            {
                if (groupId != group.Id)
            {
                    throw new Exception("Not Found");
            }
                _context.Entry(group).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return new APIResponse<Group> { Data = group, Message = "Group updated successfully.", Succeeded = true };
            }
            catch (Exception ex)
            {
                return new APIResponse<Group> { Data = null, Message = ex.Message, Succeeded = false, StatusCode = 400 };
            }
        }
    }
}
