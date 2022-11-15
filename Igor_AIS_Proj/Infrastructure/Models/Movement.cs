
namespace Igor_AIS_Proj.Models
{
    public class Movement : Entity
    {
        public int MovementId { get; set; }

        [ForeignKey("AccountId")]
        public int AccountId { get; set; }

        public int? UserId { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; } 
        public DateTime MovimentTime { get; set; }

        
    }
}
