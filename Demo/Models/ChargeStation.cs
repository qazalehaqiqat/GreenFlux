using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Demo.Models
{
    public class ChargeStation
    {
        [Key] public int ChargeStationId { get; set; }

        public string Name { get; set; }

        //[JsonIgnore]
        public ICollection<Connector> Connectors { get; set; }
        public int GroupId { get; set; }

        [JsonIgnore] public Group Group { get; set; }
    }
}