using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Demo.Models
{
    public class DemoContext : DbContext
    {
        public string DbPath { get; private set; }

        public DemoContext(DbContextOptions<DemoContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Group group = new Group
            {
                Id = 1,
                Name = "Group1",
                Capacity = 10
            };
            modelBuilder.Entity<Connector>().HasKey(c => new { c.ConnectorId, c.ChargeStationId });
            modelBuilder.Entity<Group>().HasData(group);
            modelBuilder
                .Entity<ChargeStation>()
                .HasData(new ChargeStation { ChargeStationId = 1, Name = "ChargeStation1", GroupId = 1});

            modelBuilder
                .Entity<Connector>()
                .HasData(new Connector { ConnectorId = 1, ChargeStationId= 1, MaxCurrent = 5 });
        }

        public DbSet<Group> Group { get; set; }

        public DbSet<ChargeStation> ChargeStation { get; set; }

        public DbSet<Connector> Connector { get; set; }

    }
}
