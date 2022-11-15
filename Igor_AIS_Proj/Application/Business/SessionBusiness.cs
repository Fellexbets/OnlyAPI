using Igor_AIS_Proj.Business.Interfaces;
using Igor_AIS_Proj.Models;


namespace Igor_AIS_Proj.Business
{
    public class SessionBusiness : ISessionBusiness
    {
        private ISessionPersistence _sessionPersistence;
        public SessionBusiness(ISessionPersistence sessionPersistence) => _sessionPersistence = sessionPersistence;

        public async Task<bool> Delete(Session session) => await _sessionPersistence.Delete(session);

        public List<Session> GetAll() => _sessionPersistence.GetAll();

        public async Task<Session> Create(Session session) => await _sessionPersistence.Create(session);
        public async Task<bool> Update(Session session) => await _sessionPersistence.Update(session);

        public async Task<Session> GetById(Guid id) => await _sessionPersistence.GetById(id);

        public async Task<(bool, string?, Session?)> ValidateSession(Session session)
        {
            var currentSession = await _sessionPersistence.GetById(session.SessionId);
            if (currentSession == null)
                return (false, "No session found", null);
            if (session.Active == false)
                return (false, "session is not active", currentSession);
            return (true, null, currentSession);
        }

        public void DeleteInactiveSessions()
        {
            _sessionPersistence.DeleteInactiveSessions();
        }

    }
}
