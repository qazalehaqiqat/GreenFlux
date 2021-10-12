using System;
using Demo.Controllers;
using Xunit;
using Moq;
using Demo.Services.ConnectorService;
using Demo.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace Demo.Test
{
    public class ConnectorServiceTests
    {
        private readonly Mock<IConnectorService> connectorServiceStub = new Mock<IConnectorService>();

        public ConnectorServiceTests()
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
                    GroupId = 2,
                    Connectors = new List<Connector>
                    {
                        new Connector
                        {
                            ConnectorId = 1,
                            ChargeStationId = 1,
                            MaxCurrent = 50
                        }
                    }
                });
                context.Connector.Add(new Connector
                {
                    ConnectorId = 2,
                    ChargeStationId = 1,
                    MaxCurrent = 20
                });
                context.SaveChanges();
            }
        }
        [Fact]
        public async Task AddConnectorToChargeStation_WhenGroupHasEnoughCapacity_ShouldReturnConnector()
        {
            Connector connector = new Connector
            {
                ChargeStationId = 1,
                ConnectorId = 2,
                MaxCurrent = 8
            };
            connectorServiceStub.Setup(p => p.AddConnectorToChargeStation(1, connector)).ReturnsAsync(new APIResponse<Connector> { Data = connector });
            var controller = new ConnectorController(connectorServiceStub.Object);
            var addedConnector = await controller.PostConnector(1, connector);
            Assert.IsType<CreatedAtActionResult>(addedConnector.Result);
        }
        [Fact]
        public async Task AddConnectorToChargeStation_whenGroupHasNotEnoughCapacity_ShouldReturnBadRequest()
        {
            Connector connector = new Connector
            {
                ChargeStationId = 1,
                ConnectorId = 2,
                MaxCurrent = 120
            };
            connectorServiceStub.Setup(p => p.AddConnectorToChargeStation(1, connector)).ReturnsAsync(new APIResponse<Connector> { Data = null });
            var controller = new ConnectorController(connectorServiceStub.Object);
            var addedConnector = await controller.PostConnector(1, connector);
            Assert.IsType<BadRequestResult>(addedConnector.Result);
        }
        [Fact]
        public async Task AddConnectorToChargeStation_whenConnectorIdIsNotValid_ShouldReturnBadRequest()
        {
            Connector connector = new Connector
            {
                ChargeStationId = 1,
                ConnectorId = 6,
                MaxCurrent = 50
            };
            connectorServiceStub.Setup(p => p.AddConnectorToChargeStation(1, connector)).ReturnsAsync(new APIResponse<Connector> { Data = null });
            var controller = new ConnectorController(connectorServiceStub.Object);
            var addedConnector = await controller.PostConnector(1, connector);
            Assert.IsType<BadRequestResult>(addedConnector.Result);
        }
        [Fact]
        public async Task AddConnectorToChargeStation_whenConnectorIdExist_ShouldReturnBadRequest()
        {
            Connector connector = new Connector
            {
                ChargeStationId = 1,
                ConnectorId = 1,
                MaxCurrent = 50
            };
            connectorServiceStub.Setup(p => p.AddConnectorToChargeStation(1, connector)).ReturnsAsync(new APIResponse<Connector> { Data = null });
            var controller = new ConnectorController(connectorServiceStub.Object);
            var addedConnector = await controller.PostConnector(1, connector);
            Assert.IsType<BadRequestResult>(addedConnector.Result);
        }
        [Fact]
        public async Task DeleteConnector_WhenConnectorIdAndChargeStationIdAreValid_ShouldReturnDeletedConnector()
        {
            connectorServiceStub.Setup(p => p.DeleteConnector(2, 1)).ReturnsAsync(new Connector { ChargeStationId = 1, ConnectorId = 2 });
            var controller = new ConnectorController(connectorServiceStub.Object);
            var deletedConnector = await controller.DeleteConnector(2, 1);
            Assert.IsType<ActionResult<Connector>>(deletedConnector);
        }
        [Fact]
        public async Task UpdateConnector()
        {
            var connector = new Connector { ChargeStationId = 1, ConnectorId = 2, MaxCurrent = 40 };
            connectorServiceStub.Setup(p => p.UpdateCurrent(2, 1, 40)).ReturnsAsync(new APIResponse<Connector> { Data = connector, Succeeded =true });
            var controller = new ConnectorController(connectorServiceStub.Object);
            var updatedConnector = await controller.PutConnector(connector);
            Assert.IsType<CreatedAtActionResult>(updatedConnector.Result);
        }
    }
}
