using Domain;

namespace DAL;

public class UserRepositoryDb : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepositoryDb()
    {
        var contextFactory = new AppDbContextFactory();
        _context = contextFactory.CreateDbContext([]);
    }

    public void CreateUser(UserEntity newUser)
    {
        _context.Users.Add(newUser);
        _context.SaveChanges();
    }

    public UserEntity LogIn(string username, string passHash)
    {
        UserEntity ?loadedUser = _context.Users
            .FirstOrDefault(u => u.UserName == username);
        
        if (loadedUser == null || loadedUser.PassHash != passHash)
        {
            return null!;
        }
        
        return loadedUser;
    }
}