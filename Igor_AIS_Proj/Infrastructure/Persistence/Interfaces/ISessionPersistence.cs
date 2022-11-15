
namespace Igor_AIS_Proj.Persistence.Interfaces
{
    public interface ISessionPersistence
    {
        List<Session> GetAll();

        Task<bool> Update(Session session);

        Task<bool> Delete(Session session);

        Task<Session> Create(Session session);

        Task<Session> GetById(Guid id);

        void DeleteInactiveSessions();

    }
}
