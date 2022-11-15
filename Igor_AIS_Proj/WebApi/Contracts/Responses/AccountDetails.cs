
namespace Igor_AIS_Proj.Models.Responses
{
    public class AccountDetails
    {
        public Account Account { get; set; }
        public IEnumerable<Movim>? Movs { get; set; }
    }
}
