
using System.Collections.Generic;

namespace Igor_AIS_Proj.Persistence
{
    public class TransferPersistence : BasePersistence<Transfer>, ITransferPersistence
    {
        public TransferPersistence(IAccountPersistence accountPersistence = null)
        {
            _contextEntity = _context.Transfers;
            _accountPersistence = accountPersistence;
        }
        private readonly IAccountPersistence _accountPersistence;

        public async Task<Transfer> GetById(int id1) => await _contextEntity.FindAsync(id1);
        public async Task<bool> Delete(int id1) => await Delete(_contextEntity.Find(id1));

        public async Task<List<Transfer>> GetAllTransfersAccount(int id)
        {
            return await _contextEntity.AsNoTracking().Where(e => e.OriginaccountId == id).ToListAsync();
        }
    }
}
