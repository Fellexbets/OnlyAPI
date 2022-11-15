
namespace Igor_AIS_Proj.Persistence.Interfaces
{
    public interface IAccountPersistence
    {
        List<Account> GetAll();

        Task<bool> Update(Account account);

        Task<bool> Delete(Account account);

        Task<CreateAccountResponse> Create(CreateAccountRequest request, int userId);

        Account GetById(int id);

        Task<bool> Delete(int id);

        Task<List<Account>> GetAllAccountsUser(int id);


    }
}
