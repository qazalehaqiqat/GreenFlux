using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Models
{
    public class Group
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        [Range(double.Epsilon, double.MaxValue, ErrorMessage = "Please enter a value bigger than zero")]
        public double Capacity { get; set; }

        public ICollection<ChargeStation> ChargeStations { get; set; }
    }
}