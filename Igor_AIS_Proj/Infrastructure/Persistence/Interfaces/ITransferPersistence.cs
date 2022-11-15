
namespace Igor_AIS_Proj.Persistence.Interfaces
{
    public interface ITransferPersistence
    {
        List<Transfer> GetAll();

        Task<bool> Update(Transfer transfer);

        Task<bool> Delete(Transfer transfer);

        Task<Transfer> Create(Transfer transfer);
        Task<Transfer> GetById(int id);

        Task<bool> Delete(int id);

        Task<List<Transfer>> GetAllTransfersAccount(int id);

    }
}
