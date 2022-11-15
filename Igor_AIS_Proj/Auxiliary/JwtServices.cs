
using Microsoft.Extensions.Primitives;
using System.Security.Cryptography;

namespace Igor_AIS_Proj.Auxiliary
{
    public class JwtServices : IJwtServices
    {

        //static JwtServices()
        //{
            


        //}


        public Session GenerateToken(User user)
        {
            Session session = new Session
            {
                SessionId = Guid.NewGuid()
            };
            session = GenerateAccessToken(session, user);
            session = RefreshToken(session);
            return session;
        }
        public static int? ValidateJwt(string token)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                   .AddJsonFile("appsettings.json")
                   .Build();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(configuration["Secret"]);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var IdToken = int.Parse(jwtToken.Claims.First(x => x.Type == "nameid").Value);
                return IdToken;
            }
            catch
            {
                return null;
            }
        }

        public Session RenewToken(User user, Session session)
        {
            session = GenerateAccessToken(session, user);
            return session;
        }
        private static Session GenerateAccessToken(Session session, User user)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                   .AddJsonFile("appsettings.json")
                   .Build();
            var key = Encoding.ASCII.GetBytes(configuration["Secret"]);
            var tokenHandler = new JwtSecurityTokenHandler();
            session.TokenAccessExpireAt = DateTime.UtcNow.AddMinutes(10);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Sid, session.SessionId.ToString())
            }),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            session.TokenAccess = tokenHandler.WriteToken(token);
            return session; 
        }

        
        private static Session RefreshToken(Session session)
        {
            var randomNumber = new byte[32];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(randomNumber);
                session.RefreshToken = Convert.ToBase64String(randomNumber);
                session.Refresh_Token_expire_At = DateTime.UtcNow.AddMinutes(2);
                return session;
            }
        }

        public (bool, string) CleanToken(StringValues authToken)
        {
            string authHeader = authToken.First();
            string token = authHeader.Substring("Bearer ".Length).Trim();
            return (token is not null, token);
        }

    }
}
