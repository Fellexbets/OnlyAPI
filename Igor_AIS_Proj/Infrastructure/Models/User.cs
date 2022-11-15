
namespace Igor_AIS_Proj.Models
{
    public partial class User : Entity
    {
        public User()
        {
            Accounts = new HashSet<Account>();
        }

        public int UserId { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Email { get; set; } 
        public string Username { get; set; } 

        public string? FullName { get; set; }
        public string UserPassword { get; set; } 
        public string? PasswordSalt { get; set; }
        public string? UserToken { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Account>? Accounts { get; set; }
    }
}
