
namespace Igor_AIS_Proj.Business
{
    public class TransferBusiness : ITransferBusiness
    {


        public TransferBusiness(ITransferPersistence transferPersistence, IAccountBusiness accountBusiness)
        {
            _transferPersistence = transferPersistence;
            _accountBusiness = accountBusiness;
        }
        private readonly ITransferPersistence _transferPersistence;
        private readonly IAccountBusiness _accountBusiness;

        public async Task<Transfer> GetById(int id) => await _transferPersistence.GetById(id);
        public async Task<bool> Delete(int id) => await _transferPersistence.Delete(id);
        public async Task<List<Transfer>> GetAllTransfersAccount(int id)
        {
            
            return await _transferPersistence.GetAllTransfersAccount(id);
        }

        public List<Transfer> GetAll() => _transferPersistence.GetAll();

        public async Task<Transfer> Create(Transfer transfer) => await _transferPersistence.Create(transfer);
        public async Task<bool> Update(Transfer transfer) => await _transferPersistence.Update(transfer);


    }
}
