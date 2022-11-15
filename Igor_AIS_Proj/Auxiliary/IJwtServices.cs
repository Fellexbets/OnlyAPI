
using Microsoft.Extensions.Primitives;
using System.Security.Cryptography;

namespace Igor_AIS_Proj.Auxiliary
{
    public interface IJwtServices
    {
        Session GenerateToken(User user);


        Session RenewToken(User user, Session session);

        (bool, string) CleanToken(StringValues authToken);


    }
}
