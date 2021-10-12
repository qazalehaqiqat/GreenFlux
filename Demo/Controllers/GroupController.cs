using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Demo.Models;
using Demo.Services.GroupService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public async Task<ActionResult<Group>> GetGroup(int? id)
        {
            if(id != null)
            {
                var group = await _groupService.GetGroupById(id.Value);

                if (group == null)
                {
                    return NotFound();
                }
                return group;
            }
            return Ok(_groupService.GetAllGroups());
        }

        [HttpPost]
        public async Task<ActionResult<Group>> PostGroup(Group group)
        {
            var createdGroup = await _groupService.CreateGroup(group);
            return CreatedAtAction("GetGroup", new { id = createdGroup.Id }, createdGroup);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Group>> DeleteGroup(int id)
        {
            var group = await _groupService.DeleteGroup(id);
            if (group == null) return NotFound();
            return CreatedAtAction("GetGroup", group);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<Group>> PutGroup(int id, Group group)
        {
            var updattetdGroup = await _groupService.UpdateGroup(id, group);
            if (updattetdGroup.Data == null)
                return NotFound();
            return CreatedAtAction("GetGroup", updattetdGroup);
        }
    }
}

