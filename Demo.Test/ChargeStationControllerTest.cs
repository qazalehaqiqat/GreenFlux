using System;
using System.Threading.Tasks;
using Demo.Models;
using Demo.Services.ChargeStationService;
using Demo.Controllers;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;

namespace Demo.Test
{
    public class ChargeStationControllerTest
    {
        public readonly Mock<IChargeStationService> chargeStationServiceStub = new Mock<IChargeStationService>();
        public ChargeStationControllerTest()
        {
            var builder = new DbContextOptionsBuilder<DemoContext>().EnableSensitiveDataLogging().UseInMemoryDatabase(Guid.NewGuid().ToString());
            using (var context = new DemoContext(builder.Options))
            {
                context.Group.Add(new Group
                {
                    Id = 2,
                    Name = "test",
                    Capacity = 100
                });
                context.ChargeStation.Add(new ChargeStation
                {
                    ChargeStationId = 1,
                    Name = "ChargeStation1",
                    GroupId = 2
                });
                context.SaveChanges();
            }
        }

        [Fact]
        public async Task AddChargeStation_WhenAlreadyExistInGroup_ShouldReturnNull()
        {
            var chargeStation = new ChargeStation
            {
                ChargeStationId = 1,
                Name = "chs1",
                GroupId = 2
            };
            chargeStationServiceStub.Setup(p => p.AddChargeStationToGroup(2, chargeStation)).ReturnsAsync((ChargeStation)null);
            var controller = new ChargeStationController(chargeStationServiceStub.Object);
            var createdChargeStation = await controller.PostChargeStation(2, chargeStation);
            Assert.IsType<BadRequestResult>(createdChargeStation.Result);
        }
        [Fact]
        public async Task AddChargeStation_WhenNotExistInGroup_ShouldReturnAddedChargeStation()
        {
            var chargeStation = new ChargeStation
            {
                ChargeStationId = 2,
                Name = "ChargeStation2",
                GroupId = 2
            };
            chargeStationServiceStub.Setup(p => p.AddChargeStationToGroup(2, chargeStation)).ReturnsAsync(chargeStation);
            var controller = new ChargeStationController(chargeStationServiceStub.Object);
            var createdChargeStation = await controller.PostChargeStation(2, chargeStation);
            Assert.IsType<CreatedAtActionResult>(createdChargeStation.Result);
        }
        [Fact]
        public async Task UpdateChargeStation_WhenExistInGroup_ShouldReturnUpdatedChargeStation()
        {
            var chargeStation = new ChargeStation
            {
                ChargeStationId = 2,
                Name = "ch2",
                GroupId = 2
            };
            chargeStationServiceStub.Setup(p => p.UpdateChargeStation(2, chargeStation)).ReturnsAsync(chargeStation);
            var controller = new ChargeStationController(chargeStationServiceStub.Object);
            var createdChargeStation = await controller.PutChargeStation(2, chargeStation);
            Assert.IsType<CreatedAtActionResult>(createdChargeStation.Result);

        }
        [Fact]
        public async Task DeletehargeStation_WhenNotExist_ShouldReturnBadRequest()
        {
            chargeStationServiceStub.Setup(p => p.DeleteChargeStation(4)).ReturnsAsync((ChargeStation)null);
            var controller = new ChargeStationController(chargeStationServiceStub.Object);
            var deletedChargeStation = await controller.DeleteChargeStation(4);
            Assert.IsType<BadRequestResult>(deletedChargeStation.Result);

        }
        [Fact]
        public async Task DeletehargeStation_WhenExist_ShouldReturnDeletedChargeStation()
        {
            chargeStationServiceStub.Setup(p => p.DeleteChargeStation(1)).ReturnsAsync(new ChargeStation { ChargeStationId = 1});
            var controller = new ChargeStationController(chargeStationServiceStub.Object);
            var deletedChargeStation = await controller.DeleteChargeStation(1);
            Assert.IsType<CreatedAtActionResult>(deletedChargeStation.Result);

        }
    }
}
