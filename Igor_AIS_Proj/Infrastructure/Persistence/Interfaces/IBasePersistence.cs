
namespace Igor_AIS_Proj.Persistence.Interfaces
{
    public interface IBasePersistence<T> where T : class
    {
        List<T> GetAll();
        Task<T> Create(T entity);
        //Task<T> GetById(int entityId);
        Task<bool> Update(T entity);


    }
}
