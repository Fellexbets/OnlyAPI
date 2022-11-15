
namespace Igor_AIS_Proj.Business.Interfaces
{
    public interface ITransferBusiness
    {
        Task<Transfer> GetById(int id);
        Task<bool> Delete(int id);
        Task<List<Transfer>> GetAllTransfersAccount(int id);
        List<Transfer> GetAll();
        Task<bool> Update(Transfer transfer);
        Task<Transfer> Create(Transfer transfer);

    }
}
