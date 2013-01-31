using System.ComponentModel.DataAnnotations;
using WebApiDoodle.Net.Http.Client.Model;

namespace WebApiDoodle.Net.Http.Client.Sample45.Models {

    public class Car : IDto {

        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Make { get; set; }

        [Required]
        [StringLength(20)]
        public string Model { get; set; }

        public int Year { get; set; }

        [Range(0, 500000)]
        public float Price { get; set; }
    }
}