using System.ComponentModel.DataAnnotations;

namespace ruhanBack.models{
    public class Otp{
        [Key]
        public string Id { get; set; }
        public string email{ get; set; }

        public string otp { get; set; }

        public DateTime date { get; set; }

    }
}