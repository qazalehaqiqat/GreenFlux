using System.ComponentModel.DataAnnotations;

namespace Demo.Models
{
    public class Connector
    {
        [Range(double.Epsilon, double.MaxValue, ErrorMessage = "Please enter a value bigger than zero")]
        public double MaxCurrent { get; set; }

        [Range(1, 5)] public int ConnectorId { get; set; }

        public int ChargeStationId { get; set; }
    }
}