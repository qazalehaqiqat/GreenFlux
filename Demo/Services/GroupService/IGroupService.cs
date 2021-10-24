using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Models;

namespace Demo.Services.GroupService
{
    public interface IGroupService
    {
        public Task<Group> GetGroupById(int id);
        public List<Group> GetAllGroups();
        public Task<APIResponse<Group>> UpdateGroup(int groupId, Group group);
        public Task<APIResponse<Group>> DeleteGroup(int id);
        public Task<Group> CreateGroup(Group group);
    }
}