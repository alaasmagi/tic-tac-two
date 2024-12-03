using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks.Dataflow;
using Domain;

namespace Authentication;

public class AccountCreation
{
    public static bool CreateAccount(string username, string password)
    {
        string hashedPassword = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
        UserEntity newUser = new UserEntity();
        return true;
    }
    
}