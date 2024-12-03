using Domain;

namespace DAL;

public class UserRepositoryJson : IUserRepository
{
    public void CreateUser(UserEntity newUser)
    {
        throw new NotImplementedException();
    }

    public UserEntity LogIn(string username, string passHash)
    {
        throw new NotImplementedException();
    }
}