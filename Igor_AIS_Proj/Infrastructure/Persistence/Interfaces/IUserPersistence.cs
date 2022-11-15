
namespace Igor_AIS_Proj.Persistence.Interfaces
{
    public interface IUserPersistence 
    {
        List<User> GetAll();

        Task<bool> Update(User user);

        Task<bool> Delete(User user);

        Task<CreateUserResponse> Create(CreateUserRequest request);
        User GetById(int id);

        Task<bool> Delete(int id);

        //Task<LoginUserResponse> Authenticate(LoginUserRequest model);

        User GetByEmail(string email);




    }
}
