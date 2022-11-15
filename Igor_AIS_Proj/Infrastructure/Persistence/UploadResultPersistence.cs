

namespace Igor_AIS_Proj.Persistence
{
    public interface IUploadResultPersistence
    {
        Task<UploadResult> DownloadFile(string fileName);

        Task Create(UploadResult uploadResult);

        Task<List<UploadResult>> GetAllUploads(int id);
    }
    public class UploadResultPersistence : BasePersistence<UploadResult>, IUploadResultPersistence
    {
        public UploadResultPersistence()
        {
            _contextEntity = _context.Uploads;
        }

        public async Task<UploadResult> DownloadFile(string fileName)
        {
            var uploadResult = await _contextEntity.FirstOrDefaultAsync(u => u.StoredFileName.Equals(fileName));
            return uploadResult;
        }

        public async Task<List<UploadResult>> GetAllUploads(int id) => 
            await _contextEntity.Where(e => e.UserId == id).ToListAsync();
 
        
        public async Task Create(UploadResult uploadResult)
        {
            _contextEntity.Add(uploadResult);
            await _context.SaveChangesAsync();
 
        }
        //public async Task<Transfer> GetById(int id1) => await _contextEntity.FindAsync(id1);


    }
}
