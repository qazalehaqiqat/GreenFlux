using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Demo.Controllers;
using Demo.Models;
using Demo.Services.ChargeStationService;
using Demo.Services.GroupService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Demo.Test
{
    public class GroupControllerTest
    {
        private readonly Mock<IGroupService> groupServiceStub = new Mock<IGroupService>();

        public GroupControllerTest()
        {
            var builder = new DbContextOptionsBuilder<DemoContext>().EnableSensitiveDataLogging().UseInMemoryDatabase(Guid.NewGuid().ToString());
            using (var context = new DemoContext(builder.Options))
            {
             
            context.Group.Add(new Group
            {
                Id = 2,
                Name = "test",
                Capacity = 100,
                ChargeStations = new List<ChargeStation>
                {
                    new ChargeStation
                    {
                        ChargeStationId =1,
                        Name="ChargeStation1",
                        GroupId=1
                    }
                }
            });
            context.SaveChanges();
            }
        }
        [Fact]
        public async Task CreateGroup_WhenGivesAGroupObject_ShoulReturnGroupObject()
        {
            var group = new Group
            {
                Id=3,
                Name = "test",
                Capacity = 100
            };
            groupServiceStub.Setup(p => p.CreateGroup(group)).ReturnsAsync(group);
            var controller = new GroupController(groupServiceStub.Object);
            var createdGroup = await controller.PostGroup(group);
            Assert.IsType<CreatedAtActionResult>(createdGroup.Result);
        }
        [Fact]
        public async Task UpdateGroup_WhenIdIsDifferentFromGroupId_ShouldReturnNotFound()
        {
            var group = new Group
            {
                Id = 3,
                Name = "test",
                Capacity = 200
            };
            groupServiceStub.Setup(p => p.UpdateGroup(4,group)).ReturnsAsync(new APIResponse<Group> { Data = null});
            var controller = new GroupController(groupServiceStub.Object);
            var updatedGroup = await controller.PutGroup(4,group);
            Assert.IsType<NotFoundResult>(updatedGroup.Result);
        }
        [Fact]
        public async Task UpdateGroup_WhenGivesObjectGroupAndId_ShouldReturnUpdatedGroup()
        {
            var group = new Group
            {
                Id = 3,
                Name = "test",
                Capacity = 200
            };
            groupServiceStub.Setup(p => p.UpdateGroup(3, group)).ReturnsAsync(new APIResponse<Group> { Data = group,Succeeded=true,Message= "Group updated successfully." });
            var controller = new GroupController(groupServiceStub.Object);
            var updatedGroup = await controller.PutGroup(3, group);
            Assert.IsType<CreatedAtActionResult>(updatedGroup.Result);
        }
        [Fact]
        public async Task DeleteGroup_WhenExistsGroupById_ShouldReturnDeletedGroup()
        {
            var group = new Group
            {
                Id = 2,
                Name = "test",
                Capacity = 100,
                ChargeStations = new List<ChargeStation>
                {
                    new ChargeStation
                    {
                        ChargeStationId =1,
                        Name="ChargeStation1",
                        GroupId=1
                    }
                }
            };
            groupServiceStub.Setup(p => p.DeleteGroup(2)).ReturnsAsync(
                new APIResponse<Group> {Data = group, Succeeded= true, StatusCode = 200, Message= "Group deleted successfully" });
            var controller = new GroupController(groupServiceStub.Object);
            var deletedGroup = await controller.DeleteGroup(2);
            Assert.IsType<CreatedAtActionResult>(deletedGroup.Result);
        }
        [Fact]
        public async Task DeleteGroup_WhenNotExistsGroupById_ShouldReturnNotFound()
        {
            var group = new Group
            {
                Id = 3,
                Name = "test",
                Capacity = 200
            };
            groupServiceStub.Setup(p => p.DeleteGroup(4)).ReturnsAsync(
                new APIResponse<Group> { Data = null, Succeeded = false, StatusCode = 400, Message = "Not Found" });
            var controller = new GroupController(groupServiceStub.Object);
            var deletedGroup = await controller.DeleteGroup(4);
            Assert.IsType<CreatedAtActionResult>(deletedGroup.Result);
        }
    }
}
