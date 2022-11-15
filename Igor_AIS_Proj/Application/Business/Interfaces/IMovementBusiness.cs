
namespace Igor_AIS_Proj.Business.Interfaces
{
    public interface IMovementBusiness
    {
        Task<Movement> GetById(int id);

        Task<bool> Delete(int id);

        List<Movement> GetAll();
       
        Task<bool> Update(Movement movement);

        Task<Movement> Create(Movement movement);
    }
}
