
namespace Igor_AIS_Proj.Models
{
    public class TransferTopicInfo : Entity
    {

        [Required]
        public decimal Amount { get; set; }

        public string Currency { get; set; }

        public int RecipientAccountId { get; set; }

        public string RecipientName { get; set; }

        public string RecipientMail { get; set; }

        public string FromUserName { get; set; }

        public int FromAccountId { get; set; }

        public string FromUserMail { get; set; }


    }
}
