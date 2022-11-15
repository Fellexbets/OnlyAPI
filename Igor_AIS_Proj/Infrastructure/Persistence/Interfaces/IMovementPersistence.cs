
namespace Igor_AIS_Proj.Persistence.Interfaces
{
    public interface IMovementPersistence
    {
        List<Movement> GetAll();

        Task<bool> Update(Movement movement);

        Task<bool> Delete(Movement movement);

        Task<Movement> Create(Movement movement);

        Task<Movement> GetById(int id);

        Task<bool> Delete(int id);

    }
}
