using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Models;
using Demo.Services.GroupService;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Controllers
{
    [Route("api/[controller]")]
    public class GroupController : Controller
    {
        private readonly IGroupService _groupService;

        public GroupController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet]
        public List<Group> GetGroup()
        {
            return _groupService.GetAllGroups();
        }

        [HttpGet("GetGroupById/{id}")]
        public async Task<ActionResult<Group>> GetGroupById(int id)
        {
            var group = await _groupService.GetGroupById(id);

            if (group == null) return NotFound();
            return group;
        }

        [HttpPost]
        public async Task<ActionResult<Group>> PostGroup(Group group)
        {
            var createdGroup = await _groupService.CreateGroup(group);
            return CreatedAtAction("GetGroup", new { id = createdGroup.Id }, createdGroup);
        }

        [HttpDelete("{id}")]
        public ActionResult<Group> DeleteGroup(int id)
        {
            var group = _groupService.DeleteGroup(id);
            if (group == null) return NotFound();
            return Ok(group);
        }

        [HttpPut]
        public async Task<ActionResult<Group>> PutGroup(int id, Group group)
        {
            var updattetdGroup = await _groupService.UpdateGroup(id, group);
            if (updattetdGroup.Data == null)
                return NotFound();
            return CreatedAtAction("GetGroup", updattetdGroup);
        }
    }
}