
namespace Igor_AIS_Proj.Business.Interfaces
{
    public interface IUserBusiness
    {
        User GetById(int id);

        Task<bool> Delete(int id);

        Task<(bool, string?, User?, Session?)> Authenticate(LoginUserRequest model);

        Task<(bool, string?, Session)> Logout(Session session);
        List<User> GetAll();

        Task<bool> Update(User user);
        Task<CreateUserResponse> Create(CreateUserRequest request);
        User GetByEmail(string email);

        Task<(bool, string?, Session?)> VerifySession(Session session);
        Task<(bool, string?, User?, Session?)> RenewLogin(Session session, string refreshToken);

    }
}
