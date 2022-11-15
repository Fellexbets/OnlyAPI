
namespace Igor_AIS_Proj.Models.Responses
{
    public class AccountMovims
    {
        public AccountMovims()
        {
        }

        public AccountMovims(CreateAccountResponse account, ICollection<Movim> movims)
        {
            Account = account;
            Movims = movims;
        }

        public CreateAccountResponse Account { get; set; }

        public virtual ICollection<Movim> Movims { get; set; }


    }
}
