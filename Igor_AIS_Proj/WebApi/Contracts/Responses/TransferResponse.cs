

namespace Igor_AIS_Proj.Models.Responses
{
    public partial class TransferResponse 
    {
        public int FromAccountId { get; set; }
        public int ToAccountId { get; set; }
        public decimal Amount { get; set; }

        [JsonIgnore]
        public virtual CreateAccountResponse FromAccount { get; set; } = null!;
        [JsonIgnore]
        public virtual CreateAccountResponse ToAccount { get; set; } = null!;




    }
}