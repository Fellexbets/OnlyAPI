
namespace Igor_AIS_Proj.Models
{
    public partial class Transfer : Entity
    {
        public int TransferId { get; set; }
        public int OriginaccountId { get; set; }
        public int DestinationaccountId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = null!;
        public DateTime TransferDate { get; set; }
    }
}
