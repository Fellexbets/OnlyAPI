

using System.ComponentModel.DataAnnotations;

namespace Igor_AIS_Proj.Models
{
    public class CreateAccountRequest : Entity
    {
        [Required]
        public decimal Balance { get; set; }
        [Required, StringLength(3, MinimumLength = 3)]
        public string Currency { get; set; }
    }
}
