

using System.ComponentModel.DataAnnotations;

namespace Igor_AIS_Proj.Models
{
    public class LoginUserRequest : Entity
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, MinLength(3), MaxLength(20)]
        public string UserPassword { get; set; }

    }
}
