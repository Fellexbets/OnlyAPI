
namespace Igor_AIS_Proj.Models
{
    public class Session : Entity
    {
  
        public Guid SessionId { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

        public string TokenAccess { get; set; }

        public DateTime TokenAccessExpireAt { get; set; }
        public bool Active { get; set; }
        public DateTime Created_At { get; set; }
        public string RefreshToken { get; set; }
        public DateTime Refresh_Token_expire_At { get; set; }

    }
}
