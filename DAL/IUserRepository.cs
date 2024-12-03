using Domain;

namespace DAL;

public interface IUserRepository
{
    public void CreateUser(UserEntity newUser);
    public UserEntity LogIn(string username, string passHash);
}