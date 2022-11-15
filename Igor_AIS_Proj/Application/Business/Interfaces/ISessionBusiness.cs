
namespace Igor_AIS_Proj.Business.Interfaces
{
    public interface ISessionBusiness
    {

        Task<bool> Delete(Session session);

        List<Session> GetAll();
       
        Task<bool> Update(Session session);

        Task<Session> Create(Session session);

        Task<Session> GetById(Guid id);

        Task<(bool, string?, Session?)> ValidateSession(Session session);

        void DeleteInactiveSessions();
    }
}
