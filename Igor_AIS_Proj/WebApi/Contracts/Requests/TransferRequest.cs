
namespace Igor_AIS_Proj.Models
{
    public class TransferRequest : Entity
    {

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public int FromAccountId { get; set; }

        [Required]
        public int ToAccountId { get; set; }

    }
}
